using System;
using System.Collections.Generic;

using lingvo.core;
using lingvo.tokenizing;

namespace lingvo.postagger
{
    /// <summary>
    /// 
    /// </summary>
    internal struct PosTaggerInputTypeProcessorFactory : IPosTaggerInputTypeProcessorFactory
    {
        private readonly IPosTaggerInputTypeProcessor _PosTaggerInputTypeProcessor;

        internal PosTaggerInputTypeProcessorFactory( PosTaggerResourcesModel model, LanguageTypeEnum languageType )
        {
            switch ( languageType )
            {
                case LanguageTypeEnum.Ru:
                    _PosTaggerInputTypeProcessor = new PosTaggerInputTypeProcessor_Ru( model.Numbers, model.Abbreviations );
                break;

                case LanguageTypeEnum.En:
                    _PosTaggerInputTypeProcessor = new PosTaggerInputTypeProcessor_En( model.Numbers, model.Abbreviations );
                break;

                default:
                    throw (new ArgumentException( languageType.ToString() ));
            }
        }
        public IPosTaggerInputTypeProcessor CreateInstance() => _PosTaggerInputTypeProcessor;
    }

    /// <summary>
    /// Обработчик Графематических характеристик.Подкрепляет к словам определенные признаки
    /// </summary>
    unsafe internal sealed class PosTaggerInputTypeProcessor_Ru : IPosTaggerInputTypeProcessor
    {
        private readonly HashSet< string > _Numbers;
        private readonly HashSet< string > _Abbreviations;
        private readonly CharType* _CTM;

        public PosTaggerInputTypeProcessor_Ru( HashSet< string > numbers, HashSet< string > abbreviations )
        {
            _Numbers       = numbers;
            _Abbreviations = abbreviations;
            _CTM           = xlat_Unsafe.Inst._CHARTYPE_MAP;
        }

        unsafe private static int LastPositionOfHyphen( char* _base, int length )
        {
            for ( length--; 0 <= length; length-- )
            {
                var ct = *(xlat_Unsafe.Inst._CHARTYPE_MAP + *(_base + length));
                if ( (ct & CharType.IsHyphen) == CharType.IsHyphen )
                {
                    return (length + 1);
                }
            }
            return (0);
        }

        /// <summary>
        /// Слово на латыни?
        /// </summary>
        unsafe private static bool IsLatin( char* _base, int length )
        {
            var hasLatinLetter = false;
            for ( int i = 0; i < length; i++ )
            {
                var ch = *(_base + i);

                if ( ('a' <= ch && ch <= 'z') || ('A' <= ch && ch <= 'Z') )
                {
                    hasLatinLetter = true;
                    continue;
                }

                if ( (*(xlat_Unsafe.Inst._CHARTYPE_MAP + ch) & CharType.IsLetter) == CharType.IsLetter )
                {
                    return (false);
                }
            }

            return (hasLatinLetter);
        }

        /// <summary>
        /// Римская цифра?
        /// </summary>
        private static bool IsRomanSymbol( char ch )
		{
            switch ( ch )
            {
                case 'I':
                case 'V':
                case 'X':
                case 'L':
                case 'C':
                case 'D':
                case 'M':
                    return (true);
            }
			return (false);
		}

        /// <summary>
        /// 
        /// </summary>
        unsafe public PosTaggerInputTypeResult GetResult( char* _base, int length, word_t word ) //, string valueUpper )
        {
            //-1-
            int digitsCount      = 0,
                upperCount       = 0,
                lowerCount       = 0,
                hyphenCount      = 0,
                pointCount       = 0,
                romanNumberCount = 0;
            int firstHyphenIndex = -1;

            //-2-
            #region [.main cycle.]
            for ( int i = 0; i < length; i++ )
            {
                var ch = *(_base + i);
                var ct = *(_CTM + ch);
                if ( (ct & CharType.IsDigit) == CharType.IsDigit )
                {
                    digitsCount++;
                }
                else if ( (ct & CharType.IsLower) == CharType.IsLower )
                {
                    lowerCount++;
                }
                else if ( (ct & CharType.IsUpper) == CharType.IsUpper )
                {
                    upperCount++;
                    if ( IsRomanSymbol( ch ) )
                        romanNumberCount++;
                }
                else if ( (ct & CharType.IsHyphen) == CharType.IsHyphen ) //if ( xlat.IsHyphen( ch ) )
                {
                    hyphenCount++;

                    if ( (firstHyphenIndex == -1) && (i != 0) && (digitsCount == 0) && (i == lowerCount + upperCount) )
                    {
                        firstHyphenIndex = i;
                    }
                }
                else if ( xlat.IsDot( ch ) )
                {
                    pointCount++;
                }
            }
            #endregion

            if ( pointCount == 0 )
            {
                if ( (digitsCount == 0) && (0 < romanNumberCount) && ((romanNumberCount == length) || (romanNumberCount == length - hyphenCount)) )
                    return (PosTaggerInputTypeResult.Num);

                if ( IsLatin( _base, length ) )
                    return (PosTaggerInputTypeResult.AllLat);
            }


            if ( (lowerCount == 0) && (upperCount == 0) )
            {
                /// цифры в любой комбинации со знаками препинаний без букв - NUM
                if ( digitsCount != 0 )
                    return (PosTaggerInputTypeResult.Num);

                var _first_ch = *_base;
                switch ( _first_ch )
                {
                    // запятая - Com
                    case ',': return (PosTaggerInputTypeResult.Com);
                    // двоеточие - Col
                    case ':': return (PosTaggerInputTypeResult.Col);
                }

                var _first_ct = *(_CTM + _first_ch);
                // дефис - Dush
                if ( (_first_ct & CharType.IsHyphen) == CharType.IsHyphen ) //if ( xlat.IsHyphen( _first_ch ) )
                    return (PosTaggerInputTypeResult.Dush);
            }
            else
            if ( (digitsCount == 0) && (firstHyphenIndex == -1) )
            {
                switch ( pointCount )
                {
                    case 0:
                        if ( (hyphenCount == 0) &&
                              _Numbers.Contains( word.valueUpper ) 
                            )
                        {
                            return (PosTaggerInputTypeResult.CreateNum()); // return (PosTaggerInputTypeResult.Num);
                        }
                    break;

                    case 1:                        
                        if ( (hyphenCount == 0) &&                    //no hyphen
                             (xlat.IsDot( *(_base + length - 1) )) && //if dot is last char
                              _Numbers.Contains( word.valueUpper ) 
                            )
                        {
                            return (PosTaggerInputTypeResult.CreateNum());
                        }
                    break;
                    /*---previuos commented---case 1: break;*/

                    default: //1 < pointCount
                        if ( (hyphenCount == 0) &&
                              _Abbreviations.Contains( word.valueOriginal ) //-регистрозависимый!!!-_Abbreviations.Contains( word.valueUpper ) 
                            )
                        {
                            return (PosTaggerInputTypeResult.IsAbbreviation);
                        }
                    break;
                }
            }

            var first_ch = *_base;
            var first_ct = *(_CTM + first_ch);
            
            var isFirstUpper = (1 < length) && ((first_ct & CharType.IsUpper) == CharType.IsUpper);
            if ( isFirstUpper )
            {
                if ( (lowerCount != 0) && (0 < upperCount) && (pointCount == 0) )
                    return (PosTaggerInputTypeResult.FstC); 

                if ( pointCount != 0 )
                {
                    var ch = *(_base + 1);
                    if ( xlat.IsDot( ch ) )
                        return (PosTaggerInputTypeResult.OneCP);
                }
            }


            if ( (first_ct & CharType.IsDigit) == CharType.IsDigit )
                return (PosTaggerInputTypeResult.Num);

            if ( firstHyphenIndex != -1 )
            {
                var firstNumberWord = word.valueUpper.Substring( 0, firstHyphenIndex );
                if ( _Numbers.Contains( firstNumberWord ) )
                {
                    var p = LastPositionOfHyphen( _base, length );
                    //---var v = new string( _base, p, length - p ); //original-value
                    var v = word.valueUpper.Substring( p );          //upper-case-value
                    return (PosTaggerInputTypeResult.CreateNum( v )); // return (PosTaggerInputTypeResult.Num);
                }
            }

            if ( (digitsCount == 0) && (lowerCount == 0) && (upperCount == 0) )
            {
                return (PosTaggerInputTypeResult.IsPunctuation);
            }

            return (PosTaggerInputTypeResult.O);
        }        
    }

    /// <summary>
    /// Обработчик Графематических характеристик.Подкрепляет к словам определенные признаки
    /// </summary>
    unsafe internal sealed class PosTaggerInputTypeProcessor_En : IPosTaggerInputTypeProcessor
    {
        private readonly HashSet< string > _Numbers;
        private readonly HashSet< string > _Abbreviations;
        private readonly CharType* _CTM;

        public PosTaggerInputTypeProcessor_En( HashSet< string > numbers, HashSet< string > abbreviations )
        {
            _Numbers       = numbers;
            _Abbreviations = abbreviations;
            _CTM           = xlat_Unsafe.Inst._CHARTYPE_MAP;
        }

        unsafe private static int LastPositionOfHyphen( char* _base, int length )
        {
            for ( length--; 0 <= length; length-- )
            {
                var ct = *(xlat_Unsafe.Inst._CHARTYPE_MAP + *(_base + length));
                if ( (ct & CharType.IsHyphen) == CharType.IsHyphen )
                {
                    return (length + 1);
                }
            }
            return (0);
        }

        /// <summary>
        /// Римская цифра?
        /// </summary>
        private static bool IsRomanSymbol( char ch )
		{
            switch ( ch )
            {
                case 'I':
                case 'V':
                case 'X':
                case 'L':
                case 'C':
                case 'D':
                case 'M':
                    return (true);
            }
			return (false);
		}

        /// <summary>
        /// 
        /// </summary>
        unsafe public PosTaggerInputTypeResult GetResult( char* _base, int length, word_t word ) //, string valueUpper )
        {
            //-1-
            int digitsCount      = 0,
                upperCount       = 0,
                lowerCount       = 0,
                hyphenCount      = 0,
                pointCount       = 0,
                romanNumberCount = 0;
            int firstHyphenIndex = -1;

            //-2-
            #region [.main cycle.]
            for ( int i = 0; i < length; i++ )
            {
                var ch = *(_base + i);
                var ct = *(_CTM + ch);
                if ( (ct & CharType.IsDigit) == CharType.IsDigit )
                {
                    digitsCount++;
                }
                else if ( (ct & CharType.IsLower) == CharType.IsLower )
                {
                    lowerCount++;
                }
                else if ( (ct & CharType.IsUpper) == CharType.IsUpper )
                {
                    upperCount++;
                    if ( IsRomanSymbol( ch ) )
                        romanNumberCount++;
                }
                else if ( (ct & CharType.IsHyphen) == CharType.IsHyphen ) //if ( xlat.IsHyphen( ch ) )
                {
                    hyphenCount++;

                    if ( (firstHyphenIndex == -1) && (i != 0) && (digitsCount == 0) && (i == lowerCount + upperCount) )
                    {
                        firstHyphenIndex = i;
                    }
                }
                else if ( xlat.IsDot( ch ) )
                {
                    pointCount++;
                }
            }
            #endregion

            if ( pointCount == 0 )
            {
                if ( (digitsCount == 0) && (0 < romanNumberCount) && ((romanNumberCount == length) || (romanNumberCount == length - hyphenCount)) )
                    return (PosTaggerInputTypeResult.Num);

                /*-different-from-russian-
                if ( IsLatin( _base, length ) )
                    return (PosTaggerInputTypeResult.AllLat);
                */
            }


            if ( (lowerCount == 0) && (upperCount == 0) )
            {
                /// цифры в любой комбинации со знаками препинаний без букв - NUM
                if ( digitsCount != 0 )
                    return (PosTaggerInputTypeResult.Num);

                var _first_ch = *_base;
                switch ( _first_ch )
                {
                    // запятая - Com
                    case ',': return (PosTaggerInputTypeResult.Com);
                    // двоеточие - Col
                    case ':': return (PosTaggerInputTypeResult.Col);
                }

                var _first_ct = *(_CTM + _first_ch);
                // дефис - Dush
                if ( (_first_ct & CharType.IsHyphen) == CharType.IsHyphen ) //if ( xlat.IsHyphen( _first_ch ) )
                    return (PosTaggerInputTypeResult.Dush);
            }
            else
            if ( (digitsCount == 0) && (firstHyphenIndex == -1) )
            {
                switch ( pointCount )
                {
                    case 0:
                        if ( (hyphenCount == 0) &&
                              _Numbers.Contains( word.valueUpper ) 
                            )
                        {
                            return (PosTaggerInputTypeResult.CreateNum()); // return (PosTaggerInputTypeResult.Num);
                        }
                    break;

                    case 1:                        
                        if ( (hyphenCount == 0) &&                    //no hyphen
                             (xlat.IsDot( *(_base + length - 1) )) && //if dot is last char
                              _Numbers.Contains( word.valueUpper ) 
                            )
                        {
                            return (PosTaggerInputTypeResult.CreateNum());
                        }
                    break;
                    /*---previuos commented---case 1: break;*/

                    default: //1 < pointCount
                        if ( (hyphenCount == 0) &&
                              _Abbreviations.Contains( word.valueOriginal ) //-регистрозависимый!!!-_Abbreviations.Contains( word.valueUpper ) 
                            )
                        {
                            return (PosTaggerInputTypeResult.IsAbbreviation);
                        }
                    break;
                }
            }


            var first_ch = *_base;
            var first_ct = *(_CTM + first_ch);
            
            var isFirstUpper = (1 < length) && ((first_ct & CharType.IsUpper) == CharType.IsUpper);
            if ( isFirstUpper )
            {
                if ( (lowerCount != 0) && (0 < upperCount) && (pointCount == 0) )
                    return (PosTaggerInputTypeResult.FstC); 

                if ( pointCount != 0 )
                {
                    var ch = *(_base + 1);
                    if ( xlat.IsDot( ch ) )
                        return (PosTaggerInputTypeResult.OneCP);
                }
            }

            if ( (first_ct & CharType.IsDigit) == CharType.IsDigit )
                return (PosTaggerInputTypeResult.Num);

            if ( firstHyphenIndex != -1 )
            {
                var firstNumberWord = word.valueUpper.Substring( 0, firstHyphenIndex );
                if ( _Numbers.Contains( firstNumberWord ) )
                {
                    var p = LastPositionOfHyphen( _base, length );
                    //---var v = new string( _base, p, length - p ); //original-value
                    var v = word.valueUpper.Substring( p );          //upper-case-value
                    return (PosTaggerInputTypeResult.CreateNum( v )); // return (PosTaggerInputTypeResult.Num);
                }
            }

            if ( (digitsCount == 0) && (lowerCount == 0) && (upperCount == 0) )
            {
                return (PosTaggerInputTypeResult.IsPunctuation);
            }

            return (PosTaggerInputTypeResult.O);
        }        
    }
}
