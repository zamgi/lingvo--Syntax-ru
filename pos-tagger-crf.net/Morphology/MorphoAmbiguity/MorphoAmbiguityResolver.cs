using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

using lingvo.core;
using lingvo.crfsuite;
using lingvo.morphology;

namespace lingvo.postagger
{
    #region [.description.]
    /*
    'Собака на сене'
    
    'Собака'
O	w[0]|a[0]|w[1]|a[1]=N|U|E|U     w[0]|b[0]|w[1]|b[1]=N|A:2|E|A:2	    w[0]|c[0]|w[1]|c[1]=N|S|E|U	    w[0]|d[0]|w[1]|d[1]=N|F|E|U	    __BOS__
O	w[-1]|a[-1]|w[0]|a[0]=N|U|E|U	w[-1]|b[-1]|w[0]|b[0]=N|A:2|E|A:2	w[-1]|c[-1]|w[0]|c[0]=N|S|E|U	w[-1]|d[-1]|w[0]|d[0]=N|F|E|U	w[0]|a[0]|w[1]|a[1]=E|U|N|U	w[0]|b[0]|w[1]|b[1]=E|A:2|N|I:2	w[0]|c[0]|w[1]|c[1]=E|U|N|S	w[0]|d[0]|w[1]|d[1]=E|U|N|N	w[-1]|a[-1]|w[1]|a[1]=N|U|N|U	w[-1]|b[-1]|w[1]|b[1]=N|A:2|N|I:2	w[-1]|c[-1]|w[1]|c[1]=N|S|N|S	w[-1]|d[-1]|w[1]|d[1]=N|F|N|N
O	w[-1]|a[-1]|w[0]|a[0]=E|U|N|U	w[-1]|b[-1]|w[0]|b[0]=E|A:2|N|I:2	w[-1]|c[-1]|w[0]|c[0]=E|U|N|S	w[-1]|d[-1]|w[0]|d[0]=E|U|N|N	__EOS__

O	w[0]|a[0]|w[1]|a[1]=N|U|E|U	    w[0]|b[0]|w[1]|b[1]=N|A:2|E|I:2	    w[0]|c[0]|w[1]|c[1]=N|S|E|U	    w[0]|d[0]|w[1]|d[1]=N|F|E|U	    __BOS__
O	w[-1]|a[-1]|w[0]|a[0]=N|U|E|U	w[-1]|b[-1]|w[0]|b[0]=N|A:2|E|I:2	w[-1]|c[-1]|w[0]|c[0]=N|S|E|U	w[-1]|d[-1]|w[0]|d[0]=N|F|E|U	w[0]|a[0]|w[1]|a[1]=E|U|N|U	w[0]|b[0]|w[1]|b[1]=E|I:2|N|I:2	w[0]|c[0]|w[1]|c[1]=E|U|N|S	w[0]|d[0]|w[1]|d[1]=E|U|N|N	w[-1]|a[-1]|w[1]|a[1]=N|U|N|U	w[-1]|b[-1]|w[1]|b[1]=N|A:2|N|I:2	w[-1]|c[-1]|w[1]|c[1]=N|S|N|S	w[-1]|d[-1]|w[1]|d[1]=N|F|N|N
O	w[-1]|a[-1]|w[0]|a[0]=E|U|N|U	w[-1]|b[-1]|w[0]|b[0]=E|I:2|N|I:2	w[-1]|c[-1]|w[0]|c[0]=E|U|N|S	w[-1]|d[-1]|w[0]|d[0]=E|U|N|N	__EOS__

    
    'на'
O	w[0]|a[0]|w[1]|a[1]=N|U|E|U     w[0]|b[0]|w[1]|b[1]=N|A:2|E|A:2	    w[0]|c[0]|w[1]|c[1]=N|S|E|U	    w[0]|d[0]|w[1]|d[1]=N|F|E|U	    __BOS__
O	w[-1]|a[-1]|w[0]|a[0]=N|U|E|U	w[-1]|b[-1]|w[0]|b[0]=N|A:2|E|A:2	w[-1]|c[-1]|w[0]|c[0]=N|S|E|U	w[-1]|d[-1]|w[0]|d[0]=N|F|E|U	w[0]|a[0]|w[1]|a[1]=E|U|N|U	w[0]|b[0]|w[1]|b[1]=E|A:2|N|I:2	w[0]|c[0]|w[1]|c[1]=E|U|N|S	w[0]|d[0]|w[1]|d[1]=E|U|N|N	w[-1]|a[-1]|w[1]|a[1]=N|U|N|U	w[-1]|b[-1]|w[1]|b[1]=N|A:2|N|I:2	w[-1]|c[-1]|w[1]|c[1]=N|S|N|S	w[-1]|d[-1]|w[1]|d[1]=N|F|N|N
O	w[-1]|a[-1]|w[0]|a[0]=E|U|N|U	w[-1]|b[-1]|w[0]|b[0]=E|A:2|N|I:2	w[-1]|c[-1]|w[0]|c[0]=E|U|N|S	w[-1]|d[-1]|w[0]|d[0]=E|U|N|N	__EOS__

O	w[0]|a[0]|w[1]|a[1]=N|U|E|U	    w[0]|b[0]|w[1]|b[1]=N|A:2|E|I:2	    w[0]|c[0]|w[1]|c[1]=N|S|E|U	    w[0]|d[0]|w[1]|d[1]=N|F|E|U	    __BOS__
O	w[-1]|a[-1]|w[0]|a[0]=N|U|E|U	w[-1]|b[-1]|w[0]|b[0]=N|A:2|E|I:2	w[-1]|c[-1]|w[0]|c[0]=N|S|E|U	w[-1]|d[-1]|w[0]|d[0]=N|F|E|U	w[0]|a[0]|w[1]|a[1]=E|U|N|U	w[0]|b[0]|w[1]|b[1]=E|I:2|N|I:2	w[0]|c[0]|w[1]|c[1]=E|U|N|S	w[0]|d[0]|w[1]|d[1]=E|U|N|N	w[-1]|a[-1]|w[1]|a[1]=N|U|N|U	w[-1]|b[-1]|w[1]|b[1]=N|A:2|N|I:2	w[-1]|c[-1]|w[1]|c[1]=N|S|N|S	w[-1]|d[-1]|w[1]|d[1]=N|F|N|N
O	w[-1]|a[-1]|w[0]|a[0]=E|U|N|U	w[-1]|b[-1]|w[0]|b[0]=E|I:2|N|I:2	w[-1]|c[-1]|w[0]|c[0]=E|U|N|S	w[-1]|d[-1]|w[0]|d[0]=E|U|N|N	__EOS__
     
    */
    #endregion

    /// <summary>
    /// 
    /// </summary>
    internal static class MA
    {
        private const byte   ZERO = (byte) '\0';
        private const byte   O    = (byte) 'O';
        private const double PROBABILITY_EQUAL_THRESHOLD = 0.000001d;

        /// <summary>
        /// Part-Of-Speech => (p/w) crf-field
        /// </summary>
        public static byte get_CRF_W_field_value( MorphoAmbiguityTuple_t mat )
        {
            /*
            if ( mat.Word.posTaggerOutputType == PosTaggerOutputType.Punctuation )
            {
                switch ( mat.PunctuationType )
                {                        
                    case MorphoAmbiguityTuple_t.PunctuationTypeEnum.PunctuationQuote:   
                        return ((byte) 'Q'); //Quote
                    case MorphoAmbiguityTuple_t.PunctuationTypeEnum.PunctuationBracket: 
                        return ((byte) 'B'); //Bracket                        
                }
            }

            return (mat.Word.posTaggerOutputType.ToCrfByte());
            */

            switch ( mat.Word.posTaggerOutputType )
            {
                case PosTaggerOutputType.Adjective:           return ((byte) 'J');
                case PosTaggerOutputType.AdjectivePronoun:    return ((byte) 'R');
                case PosTaggerOutputType.Adverb:              return ((byte) 'D');
                case PosTaggerOutputType.AdverbialParticiple: return ((byte) 'X');
                case PosTaggerOutputType.AdverbialPronoun:    return ((byte) 'H');
                case PosTaggerOutputType.Article:             return ((byte) 'A');
                case PosTaggerOutputType.AuxiliaryVerb:       return ((byte) 'G');
                case PosTaggerOutputType.Conjunction:         return ((byte) 'C');
                case PosTaggerOutputType.Infinitive:          return ((byte) 'F');
                case PosTaggerOutputType.Interjection:        return ((byte) 'I');
                case PosTaggerOutputType.Noun:                return ((byte) 'N');
                case PosTaggerOutputType.Numeral:             return ((byte) 'M');
                case PosTaggerOutputType.Participle:          return ((byte) 'Z');
                case PosTaggerOutputType.Particle:            return ((byte) 'W');
                case PosTaggerOutputType.PossessivePronoun:   return ((byte) 'S');
                case PosTaggerOutputType.Preposition:         return ((byte) 'E');
                case PosTaggerOutputType.Pronoun:             return ((byte) 'Y');
                case PosTaggerOutputType.Verb:                return ((byte) 'V');
                case PosTaggerOutputType.Punctuation:
                    switch ( mat.PunctuationType )
                    {                        
                        case MorphoAmbiguityTuple_t.PunctuationTypeEnum.PunctuationQuote:   
                            return ((byte) 'Q'); //Quote
                        case MorphoAmbiguityTuple_t.PunctuationTypeEnum.PunctuationBracket: 
                            return ((byte) 'B'); //Bracket
                        //case MorphoAmbiguityTuple_t.PunctuationTypeEnum.Punctuation:                            
                    }
                    return ((byte) 'T'); //all-other-punctuation
            }

            //---case PosTaggerOutputType.Other: return (O);
            return (O);
        }
        /// <summary>
        /// Person => (a) crf-field
        /// </summary>
        public static byte get_CRF_A_field_value( MorphoAmbiguityTuple_t mat )
        {
            switch ( mat.WordFormMorphology.PartOfSpeech )
            {
                case PartOfSpeechEnum.Noun:
                case PartOfSpeechEnum.Verb:
                case PartOfSpeechEnum.Pronoun:
                case PartOfSpeechEnum.Adjective:
                    var ma = mat.WordFormMorphology.MorphoAttribute;
                    if ( (ma & MorphoAttributeEnum.First) == MorphoAttributeEnum.First )
                    {
                        return ((byte) 'F');
                    }
                    if ( (ma & MorphoAttributeEnum.Second) == MorphoAttributeEnum.Second )
                    {
                        return ((byte) 'S');
                    }
                    if ( (ma & MorphoAttributeEnum.Third) == MorphoAttributeEnum.Third )
                    {
                        return ((byte) 'T');
                    }
                break;
            }

            return ((byte) 'U');
        }

        #region [.description.]
        /*
        *VerbTransitivity – этот атрибут есть только у класса глаголов, при этом у этого класса отсутствует падеж; 
          поэтому, дабы не увеличивать кол-во параметров CRF, 
          есть предложение использовать транзитивность вместо падежа (на месте падежа, т.е. в столбце b) 
          у всех элементов класса (Verb, Predicative, Infinitive, AdverbialParticiple, AuxiliaryVerb, Participle) 
          за исключением Participle и AuxiliaryVerb – там просто проставить то что есть. 
        ** Intransitive – стандартное сокращение I, но тогда будет путаться с I падежным (Instrumental). 
           Поэтому I заменяем на R. (Transitive) => T, (Intransitive) => R
        */
        #endregion

        #region [.commented. previous.]
        /*/// <summary>
        /// Case => (b) crf-field, (e) for Verb-class (Verb, Predicative, Infinitive, AdverbialParticiple, AuxiliaryVerb, Participle)
        /// </summary>
        public static byte get_CRF_B_field_value( MorphoAmbiguityTuple_t mat )
        {
            switch ( mat.WordFormMorphology.PartOfSpeech )
            {
                case PartOfSpeechEnum.Noun:
                case PartOfSpeechEnum.Verb:
                case PartOfSpeechEnum.Pronoun:                
                case PartOfSpeechEnum.Numeral:
                case PartOfSpeechEnum.Adjective:
                case PartOfSpeechEnum.Preposition:
                    var ma = mat.WordFormMorphology.MorphoAttribute;
                    if ( (ma & MorphoAttributeEnum.Nominative) == MorphoAttributeEnum.Nominative )
                    {
                        return ((byte) 'N');
                    }
                    if ( (ma & MorphoAttributeEnum.Genitive) == MorphoAttributeEnum.Genitive )
                    {
                        return ((byte) 'G');
                    }
                    if ( (ma & MorphoAttributeEnum.Dative) == MorphoAttributeEnum.Dative )
                    {
                        return ((byte) 'D');
                    }
                    if ( (ma & MorphoAttributeEnum.Accusative) == MorphoAttributeEnum.Accusative )
                    {
                        return ((byte) 'A');
                    }
                    if ( (ma & MorphoAttributeEnum.Instrumental) == MorphoAttributeEnum.Instrumental )
                    {
                        return ((byte) 'I');
                    }
                    if ( (ma & MorphoAttributeEnum.Prepositional) == MorphoAttributeEnum.Prepositional )
                    {
                        return ((byte) 'P');
                    }
                    if ( (ma & MorphoAttributeEnum.Locative) == MorphoAttributeEnum.Locative )
                    {
                        return ((byte) 'L');
                    }
                break;
            }

            return ((byte) 'U');
        }*/
        #endregion
        /// <summary>
        /// Case => (b) crf-field, (e) for Verb-class (Verb, Predicative, Infinitive, AdverbialParticiple, AuxiliaryVerb, Participle)
        /// </summary>
        public static byte get_CRF_B_field_value( MorphoAmbiguityTuple_t mat )
        {
            switch ( mat.WordFormMorphology.PartOfSpeech )
            {
                case PartOfSpeechEnum.Noun:
                case PartOfSpeechEnum.Pronoun:                
                case PartOfSpeechEnum.Numeral:
                case PartOfSpeechEnum.Adjective:
                case PartOfSpeechEnum.Preposition:
                #region [.get {Case}.]
                {
                    var ma = mat.WordFormMorphology.MorphoAttribute;
                    if ( (ma & MorphoAttributeEnum.Nominative) == MorphoAttributeEnum.Nominative )
                    {
                        return ((byte) 'N');
                    }
                    if ( (ma & MorphoAttributeEnum.Genitive) == MorphoAttributeEnum.Genitive )
                    {
                        return ((byte) 'G');
                    }
                    if ( (ma & MorphoAttributeEnum.Dative) == MorphoAttributeEnum.Dative )
                    {
                        return ((byte) 'D');
                    }
                    if ( (ma & MorphoAttributeEnum.Accusative) == MorphoAttributeEnum.Accusative )
                    {
                        return ((byte) 'A');
                    }
                    if ( (ma & MorphoAttributeEnum.Instrumental) == MorphoAttributeEnum.Instrumental )
                    {
                        return ((byte) 'I');
                    }
                    if ( (ma & MorphoAttributeEnum.Prepositional) == MorphoAttributeEnum.Prepositional )
                    {
                        return ((byte) 'P');
                    }
                    if ( (ma & MorphoAttributeEnum.Locative) == MorphoAttributeEnum.Locative )
                    {
                        return ((byte) 'L');
                    }
                }
                #endregion
                break;

                case PartOfSpeechEnum.Verb:
                {
                    switch ( mat.Word.posTaggerOutputType )
                    {
                        case PosTaggerOutputType.AuxiliaryVerb:
                            break;

                        case PosTaggerOutputType.Participle:
                        #region [.try get {Case}.]
                        {
                            #region [.try get {Case}.]
                            var ma = mat.WordFormMorphology.MorphoAttribute;
                            if ( (ma & MorphoAttributeEnum.Nominative) == MorphoAttributeEnum.Nominative )
                            {
                                return ((byte) 'N');
                            }
                            if ( (ma & MorphoAttributeEnum.Genitive) == MorphoAttributeEnum.Genitive )
                            {
                                return ((byte) 'G');
                            }
                            if ( (ma & MorphoAttributeEnum.Dative) == MorphoAttributeEnum.Dative )
                            {
                                return ((byte) 'D');
                            }
                            if ( (ma & MorphoAttributeEnum.Accusative) == MorphoAttributeEnum.Accusative )
                            {
                                return ((byte) 'A');
                            }
                            if ( (ma & MorphoAttributeEnum.Instrumental) == MorphoAttributeEnum.Instrumental )
                            {
                                return ((byte) 'I');
                            }
                            if ( (ma & MorphoAttributeEnum.Prepositional) == MorphoAttributeEnum.Prepositional )
                            {
                                return ((byte) 'P');
                            }
                            if ( (ma & MorphoAttributeEnum.Locative) == MorphoAttributeEnum.Locative )
                            {
                                return ((byte) 'L');
                            }
                            #endregion

                            #region [.commented. {VerbTransitivity}.]
                            /*
                            #region [.try get {VerbTransitivity}.]
                            if ( (ma & MorphoAttributeEnum.Transitive) == MorphoAttributeEnum.Transitive )
                            {
                                return ((byte) 'T');
                            }
                            if ( (ma & MorphoAttributeEnum.Intransitive) == MorphoAttributeEnum.Intransitive )
                            {
                                return ((byte) 'R');
                            }
                            #endregion
                            */
                            #endregion
                        }
                        #endregion
                        break;

                        default:
                        #region [.get {VerbTransitivity}.]
                        {
                            /*
                            case PosTaggerOutputType.Verb:
                            case PosTaggerOutputType.Infinitive:
                            case PosTaggerOutputType.AdverbialParticiple:
                            //case PosTaggerOutputType.Predicative:
                            break;
                            */

                            var ma = mat.WordFormMorphology.MorphoAttribute;
                            if ( (ma & MorphoAttributeEnum.Transitive) == MorphoAttributeEnum.Transitive )
                            {
                                return ((byte) 'T');
                            }
                            if ( (ma & MorphoAttributeEnum.Intransitive) == MorphoAttributeEnum.Intransitive )
                            {
                                return ((byte) 'R');
                            }
                        }
                        #endregion
                        break;
                    }
                }
                break;
            }

            return ((byte) 'U');
        }

        /// <summary>
        /// Number => (c) crf-field
        /// </summary>
        public static byte get_CRF_C_field_value( MorphoAmbiguityTuple_t mat )
        {
            switch ( mat.WordFormMorphology.PartOfSpeech )
            {
                case PartOfSpeechEnum.Noun:
                case PartOfSpeechEnum.Verb:
                case PartOfSpeechEnum.Pronoun:                
                case PartOfSpeechEnum.Numeral:
                case PartOfSpeechEnum.Adjective:
                    var ma = mat.WordFormMorphology.MorphoAttribute;
                    if ( (ma & MorphoAttributeEnum.Singular) == MorphoAttributeEnum.Singular )
                    {
                        return ((byte) 'S');
                    }
                    if ( (ma & MorphoAttributeEnum.Plural) == MorphoAttributeEnum.Plural )
                    {
                        return ((byte) 'P');
                    }
                break;
            }

            return ((byte) 'U');
        }
        /// <summary>
        /// Gender => (d) crf-field
        /// </summary>
        public static byte get_CRF_D_field_value( MorphoAmbiguityTuple_t mat )
        {
            switch ( mat.WordFormMorphology.PartOfSpeech )
            {
                case PartOfSpeechEnum.Noun:
                case PartOfSpeechEnum.Verb:
                case PartOfSpeechEnum.Pronoun:                
                case PartOfSpeechEnum.Numeral:
                case PartOfSpeechEnum.Adjective:
                    var ma = mat.WordFormMorphology.MorphoAttribute;
                    if ( (ma & MorphoAttributeEnum.Feminine) == MorphoAttributeEnum.Feminine )
                    {
                        return ((byte) 'F');
                    }
                    if ( (ma & MorphoAttributeEnum.Masculine) == MorphoAttributeEnum.Masculine )
                    {
                        return ((byte) 'M');
                    }
                    if ( (ma & MorphoAttributeEnum.Neuter) == MorphoAttributeEnum.Neuter )
                    {
                        return ((byte) 'N');
                    }
                break;
            }

            return ((byte) 'U');
        }
        /// <summary>
        /// O => (y) crf-field
        /// </summary>
        public static byte get_CRF_Y_field_value()
        {
            return (O);
        }

        unsafe public static MorphoAttributeEnum ToMorphoAttributes( byte* value )
        {
            var morphoAttributes = default(MorphoAttributeEnum);

            //first letter - part-of-speech
            if ( (*value++) == ZERO )
            {
                return (morphoAttributes);
            }

            //Person
            switch ( (*value++) )
            {
                //Person - First
                case (byte) 'F': morphoAttributes |= MorphoAttributeEnum.First; break;
                //Person - Second
                case (byte) 'S': morphoAttributes |= MorphoAttributeEnum.Second; break;
                //Person - Third
                case (byte) 'T': morphoAttributes |= MorphoAttributeEnum.Third; break;
                case (byte) 'U': break;

                default:
                    return (morphoAttributes); //Debugger.Break(); break;
            }

            //Case
            switch ( (*value++) )
            {
                //Case - Nominative
                case (byte) 'N': morphoAttributes |= MorphoAttributeEnum.Nominative; break;
                //Case - Genitive
                case (byte) 'G': morphoAttributes |= MorphoAttributeEnum.Genitive; break;
                //Case - Dative
                case (byte) 'D': morphoAttributes |= MorphoAttributeEnum.Dative; break;
                //Case - Accusative
                case (byte) 'A': morphoAttributes |= MorphoAttributeEnum.Accusative; break;
                //Case - Instrumental
                case (byte) 'I': morphoAttributes |= MorphoAttributeEnum.Instrumental; break;
                //Case - Prepositional
                case (byte) 'P': morphoAttributes |= MorphoAttributeEnum.Prepositional; break;
                //Case - Locative
                case (byte) 'L': morphoAttributes |= MorphoAttributeEnum.Locative; break;
                case (byte) 'U': break;

                default:
                    return (morphoAttributes); //Debugger.Break(); break;
            }

            //Number
            switch ( (*value++) )
            {
                //Number - Singular
                case (byte) 'S': morphoAttributes |= MorphoAttributeEnum.Singular; break;
                //Number - Plural
                case (byte) 'P': morphoAttributes |= MorphoAttributeEnum.Plural; break;
                case (byte) 'U': break;

                default:
                    return (morphoAttributes); //Debugger.Break(); break;
            }

            //Gender
            switch ( (*value++) )
            {
                //Gender - Feminine
                case (byte) 'F': morphoAttributes |= MorphoAttributeEnum.Feminine; break;
                //Gender - Masculine
                case (byte) 'M': morphoAttributes |= MorphoAttributeEnum.Masculine; break;
                //Gender - Neuter
                case (byte) 'N': morphoAttributes |= MorphoAttributeEnum.Neuter; break;
                case (byte) 'U': break;

                default:
                    return (morphoAttributes); //Debugger.Break(); break;
            }

            Debug.Assert( *value == ZERO, "*value != '\0'" );

            return (morphoAttributes);
        }

        public static bool IsSingleItemAndEmptyMorphoAttribute( List< MorphoAmbiguityTuple_t > mats )
        {
            return ((mats.Count == 1) && mats[ 0 ].WordFormMorphology.IsEmptyMorphoAttribute());
        }

        /*
        Так же возможен вариант, когда два токена объединились по четырем морфохарактеристикам, но при это имеют разный регистр первой буквы:
            Путин		Noun	Genitive, Singular, Masculine, Animate, Proper
            Путин		Noun	Accusative, Singular, Masculine, Animate, Proper
            путина		Noun	Nominative, Singular, Feminine, Inanimate, Common
        В этом случае отбирать тот вариант, с каким регистром написано в тексте.                     
        */
        public static bool IsFirstPreference( MorphoAmbiguityTuple_t first, MorphoAmbiguityTuple_t second )
        {
            if ( second == default(MorphoAmbiguityTuple_t) )
                return (true);

            if ( first.Word.posTaggerInputType != second.Word.posTaggerInputType )
            {
                if ( xlat_Unsafe.Inst.IsUpper( first.WordFormMorphology.NormalForm[ 0 ] ) )
                {
                    return (first.Word.posTaggerInputType == PosTaggerInputType.FstC);
                }
            }
            return (false);
        }
        public static bool IsProbabilityEqual( double probability1, double probability2 )
        {
            return (Math.Abs( probability2 - probability1 ) <= PROBABILITY_EQUAL_THRESHOLD);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal struct probability_t
    {
        public double                 max_probability;
        public MorphoAmbiguityTuple_t max_morphoAmbiguityTuple_0;
        public MorphoAmbiguityTuple_t max_morphoAmbiguityTuple_1;
        public MorphoAmbiguityTuple_t max_morphoAmbiguityTuple_2;
        public MorphoAmbiguityTuple_t max_morphoAmbiguityTuple_3;
        public MorphoAmbiguityTuple_t max_morphoAmbiguityTuple_4;

        public static probability_t Create()
        {
            var prob = new probability_t()
            {
                max_probability = double.MinValue,
            };
            return (prob);
        }
    }


    //========================= 3-Grams =========================//

    /// <summary>
    /// Конвертор в формат CRF
    /// </summary>
    unsafe internal sealed class MorphoAmbiguityResolver_3g : IDisposable
	{
        #region [.private field's.]
        //private const double ZERO_DOUBLE = 0.0;
        private const byte ZERO           = (byte) '\0';
        private const byte VERTICAL_SLASH = (byte) '|';
        private const byte SEMICOLON      = (byte) ';';
        private const int UTF8_BUFFER_SIZE            = 1024 * 4;             //4KB
        private const int ATTRIBUTE_MAX_LENGTH        = UTF8_BUFFER_SIZE / 4; //1KB
        private const int MAT_5_CHARS_LENGTH          = 5;
        private const int MAT_THREEGRAMS_SIZE         = 3;
        private const int MAT_THREEGRAMS_FIRST_INDEX  = 0;
        private const int MAT_THREEGRAMS_MIDDLE_INDEX = 1;
        private const int MAT_THREEGRAMS_LAST_INDEX   = 2;

        /*private readonly Dictionary< string, float > _ModelDictionary;*/
        private readonly MorphoAmbiguityResolverModel _Model;
        private readonly Dictionary< IntPtr, float >  _ModelDictionaryBytes;
        private readonly CRFNgram[]           _NGramsMiddle;
        private readonly int                  _NGramsMiddleLength;
        private readonly CRFNgram[]           _NGramsFirst;
        private readonly int                  _NGramsFirstLength;
        private readonly CRFNgram[]           _NGramsLast;
        private readonly int                  _NGramsLastLength;        
        private readonly GCHandle             _AttributeBufferGCHandle;
        private readonly byte[]               _AttributeBuffer;
        private byte*                         _AttributeBufferPtrBase;
        private byte*                         _AttributeBufferPtr;
        //private readonly GCHandle             _Mat5CharsBufferGCHandle;
        //private byte*                         _Mat5CharsBufferPtrBase;
        private List< WordMorphoAmbiguity_t > _WordMorphoAmbiguities;
        private int                           _WordMorphoAmbiguitiesCount;
        private WordMorphoAmbiguity_t         _Wma_0, _Wma_1, _Wma_2;
        private MorphoAmbiguityTuple_t        _MatThreegram_0, _MatThreegram_1, _MatThreegram_2;

        #if DEBUG
            private readonly System.Text.StringBuilder _sb_attr_debug = new System.Text.StringBuilder();
            private readonly char[] _chars_attr_debug = new char[ ATTRIBUTE_MAX_LENGTH * 10 ];
        #endif
        #endregion

        #region [.ctor().]
        public MorphoAmbiguityResolver_3g( MorphoAmbiguityResolverModel model )
		{
            //-1-
            _Model = model;
            /*_ModelDictionary = model.Dictionary;*/
            _ModelDictionaryBytes = model.DictionaryBytes;

            //-2-
            var crfTemplateFile_3g = LoadTemplate( model.Config.TemplateFilename_3g );
            _NGramsFirst        = crfTemplateFile_3g.GetCRFNgramsWhichCanTemplateBeApplied( MAT_THREEGRAMS_FIRST_INDEX  /*first-word-(morpho-attr)-index == 0*/ , MAT_THREEGRAMS_SIZE /*3*/ );
            _NGramsFirstLength  = _NGramsFirst.Length;
            _NGramsMiddle       = crfTemplateFile_3g.GetCRFNgramsWhichCanTemplateBeApplied( MAT_THREEGRAMS_MIDDLE_INDEX /*middle-word-(morpho-attr)-index == 1*/, MAT_THREEGRAMS_SIZE /*3*/ );
            _NGramsMiddleLength = _NGramsMiddle.Length;
            _NGramsLast         = crfTemplateFile_3g.GetCRFNgramsWhichCanTemplateBeApplied( MAT_THREEGRAMS_LAST_INDEX   /*last-word-(morpho-attr)-index == 2*/  , MAT_THREEGRAMS_SIZE /*3*/ );
            _NGramsLastLength   = _NGramsLast.Length;

            //-3-
            //var attributeBuffer      = new byte[ ATTRIBUTE_MAX_LENGTH + 1 ];
            _AttributeBuffer         = new byte[ ATTRIBUTE_MAX_LENGTH + 1 ];
            var attributeBuffer = _AttributeBuffer;
            _AttributeBufferGCHandle = GCHandle.Alloc( attributeBuffer, GCHandleType.Pinned );
            _AttributeBufferPtrBase  = (byte*) _AttributeBufferGCHandle.AddrOfPinnedObject().ToPointer();

            //-4-
            //var mat5CharsBuffer      = new byte[ MAT_5_CHARS_LENGTH ];
            //_Mat5CharsBufferGCHandle = GCHandle.Alloc( mat5CharsBuffer, GCHandleType.Pinned );
            //_Mat5CharsBufferPtrBase  = (byte*) _Mat5CharsBufferGCHandle.AddrOfPinnedObject().ToPointer();

            //-5-
            //_MatThreegrams = new MorphoAmbiguityTuple_t[ MAT_THREEGRAMS_SIZE ];
		}

        ~MorphoAmbiguityResolver_3g()
        {
            DisposeNativeResources();
        }
        public void Dispose()
        {
            DisposeNativeResources();

            GC.SuppressFinalize( this );
        }
        private void DisposeNativeResources()
        {
            if ( _AttributeBufferPtrBase != null )
            {
                _AttributeBufferGCHandle.Free();
                _AttributeBufferPtrBase = null;
            }

            //if ( _Mat5CharsBufferPtrBase != null )
            //{
            //    _Mat5CharsBufferGCHandle.Free();
            //    _Mat5CharsBufferPtrBase = null;
            //}
        }

        /// <summary>
        /// Получить шаблон 
		/// </summary>
		/// <param name="templatePath">путь к файлу шаблона</param>
		/// <returns>Шаблон</returns>
        private static CRFTemplateFile LoadTemplate( string templatePath )
		{
			var result = CRFTemplateFileLoader.Load( templatePath );
			CheckTemplate( result );
			return (result);
		}
		/// <summary>
        /// Проверить правильность шаблона
		/// </summary>
        /// <param name="crfTemplateFile">Шаблон</param>
		private static void CheckTemplate( CRFTemplateFile crfTemplateFile )
		{
			foreach ( CRFNgram ngram in crfTemplateFile.CRFNgrams )
			{
				foreach ( CRFAttribute crfAttribute in ngram.CRFAttributes )
				{					
                    switch ( crfAttribute.Position )
                    {
                        case -1:
                        case  0:
                        case  1: 
                            break;
                        default:
					        throw (new Exception("Аттрибут '" + crfAttribute.AttributeName + "' содержащит недопустимое значение индекса морфо-атрибута: '" + crfAttribute.Position + '\''));                            
                    }
				}
			}
		}
        #endregion


        public void Resolve( List< WordMorphoAmbiguity_t > wordMorphoAmbiguities )
        {
            #region [.init.]
            _WordMorphoAmbiguities      = wordMorphoAmbiguities;
            _WordMorphoAmbiguitiesCount = _WordMorphoAmbiguities.Count;
            #if DEBUG
                _sb_attr_debug.Clear();
            #endif
            #endregion

            #region [.special-case-with-0,1,2-WMA.]
            switch ( _WordMorphoAmbiguitiesCount )
            {
                case 0: goto UNINIT;
                case 1: 
                    _WordMorphoAmbiguities[ 0 ].SetWordMorphologyAsUndefined();
                goto UNINIT;

                case 2:
                    _Wma_0 = _WordMorphoAmbiguities[ 0 ];
                    _Wma_1 = _WordMorphoAmbiguities[ 1 ];

                    var prob_42 = probability_t.Create();
                    
                    foreach ( var _ in IteratedOverThreeGramsMAT_42WMA() )
                    {
                        //crf-tagging
                        var probability = TaggingBiGramsMAT_42WMA();
                        if ( !probability.HasValue )
                        {
                            continue;
                        }

                        #region [.choose max-probability.]
                        var __probability = probability.Value;
                        if ( prob_42.max_probability < __probability )
                        {
                            prob_42.max_morphoAmbiguityTuple_0 = _MatThreegram_0;
                            prob_42.max_morphoAmbiguityTuple_1 = _MatThreegram_1;
                            prob_42.max_probability = __probability;
                        }
                        #region commented
                        /*else
                        if ( MA.IsProbabilityEqual( prob.max_probability, probability ) &&
                             MA.IsFirstPreference( _MatThreegram_0, prob.max_morphoAmbiguityTuple ) )
                        {
                            prob.max_morphoAmbiguityTuple = _MatThreegram_0;
                        }*/
                        #endregion
                        #endregion
                    }

                    //set best word-morphology
                    SetBestWordMorphology( prob_42.max_morphoAmbiguityTuple_0, _Wma_0 );
                    SetBestWordMorphology( prob_42.max_morphoAmbiguityTuple_1, _Wma_1 );
                goto UNINIT;
            }
            #endregion

            #region [.first-wma.]
            _Wma_0 = _WordMorphoAmbiguities[ 0 ];
            _Wma_1 = _WordMorphoAmbiguities[ 1 ];
            _Wma_2 = _WordMorphoAmbiguities[ 2 ];

            var prob = probability_t.Create();

            foreach ( var _ in IteratedOverThreeGramsMAT_4FirstWMA() )
            {
                //crf-tagging
                var probability = TaggingThreeGramsMAT();
                if ( !probability.HasValue )
                {
                    continue;
                }
                 
                #region [.choose max-probability.]
                var __probability = probability.Value;
                if ( prob.max_probability < __probability )
                {
                    prob.max_morphoAmbiguityTuple_0 = _MatThreegram_0;
                    prob.max_morphoAmbiguityTuple_1 = _MatThreegram_1;
                    prob.max_morphoAmbiguityTuple_2 = _MatThreegram_2;
                    prob.max_probability = __probability;
                }
                #region commented
                /*else
                if ( MA.IsProbabilityEqual( prob.max_probability, probability ) &&
                     MA.IsFirstPreference( _MatThreegram_0, prob.max_morphoAmbiguityTuple ) )
                {
                    prob.max_morphoAmbiguityTuple = _MatThreegram_0;
                }*/
                #endregion
                #endregion
            }

            //set best word-morphology
            SetBestWordMorphology( prob.max_morphoAmbiguityTuple_0, _Wma_0 );
            SetBestWordMorphology( prob.max_morphoAmbiguityTuple_1, _Wma_1 );
            #endregion

            #region [.middle-wma.]
            foreach ( var wmaIndex in IteratedOverThreeGramsWMA() )
            {
                prob = probability_t.Create();

                foreach ( var _ in IteratedOverThreeGramsMAT() )
                {
                    //crf-tagging
                    var probability = TaggingThreeGramsMAT();
                    if ( !probability.HasValue )
                    {
                        continue;
                    }

                    #region [.choose max-probability.]
                    var __probability = probability.Value;
                    if ( prob.max_probability < __probability )
                    {
                        prob.max_morphoAmbiguityTuple_1 = _MatThreegram_1;
                        prob.max_morphoAmbiguityTuple_2 = _MatThreegram_2;
                        prob.max_probability = __probability;
                    }
                    #region commented
                    /*else 
                    if ( MA.IsProbabilityEqual( prob.max_probability, probability ) )
                    {
                        if ( MA.IsFirstPreference( _MatThreegram_1, prob.max_morphoAmbiguityTuple_1 ) )
                            prob.max_morphoAmbiguityTuple_1 = _MatThreegram_1;
                        if ( MA.IsFirstPreference( _MatThreegram_2, prob.max_morphoAmbiguityTuple_2 ) )
                            prob.max_morphoAmbiguityTuple_2 = _MatThreegram_2;
                    }*/
                    #endregion
                    #endregion
                }

                //set best word-morphology
                SetBestWordMorphology( prob.max_morphoAmbiguityTuple_1, _Wma_1 );
            }
            #endregion

            #region [.last-wma.]
            prob = probability_t.Create();

            foreach ( var _ in IteratedOverThreeGramsMAT_4LastWMA() )
            {
                //crf-tagging
                var probability = TaggingThreeGramsMAT();
                if ( !probability.HasValue )
                {
                    continue;
                }

                #region [.choose max-probability.]
                var __probability = probability.Value;
                if ( prob.max_probability < __probability )
                {
                    prob.max_morphoAmbiguityTuple_2 = _MatThreegram_2;
                    prob.max_probability = __probability;
                }
                #region commented
                /*else
                if ( MA.IsProbabilityEqual( prob.max_probability, probability ) &&
                     MA.IsFirstPreference( _MatThreegram_2, prob.max_morphoAmbiguityTuple_2 ) )
                {
                    prob.max_morphoAmbiguityTuple_2 = _MatThreegram_2;
                }*/
                #endregion
                #endregion
            }

            //set best word-morphology
            SetBestWordMorphology( prob.max_morphoAmbiguityTuple_2, _Wma_2 );
            #endregion

            #region [.un-init.]
        UNINIT:
            _WordMorphoAmbiguities = null;
            _MatThreegram_0 = _MatThreegram_1 = _MatThreegram_2 = default(MorphoAmbiguityTuple_t);
            _Wma_0 = _Wma_1 = _Wma_2 = default(WordMorphoAmbiguity_t);
            #if DEBUG
                var _attr_debug = _sb_attr_debug.ToString();
            #endif
            #endregion            
        }
        
        private IEnumerable< int > IteratedOverThreeGramsWMA()
        {
            for ( int i = 3, len = _WordMorphoAmbiguitiesCount; i < len; i++ )
            {
                _Wma_0 = _Wma_1;
                _Wma_1 = _Wma_2;
                _Wma_2 = _WordMorphoAmbiguities[ i ];

                yield return (i - 2);
            }
        }

        private IEnumerable< int > IteratedOverThreeGramsMAT()
        {
            if ( _Wma_1.IsPunctuation() )
            {
                yield break;
            }
            var mats_1 = _Wma_1.MorphoAmbiguityTuples;
            var len_1  = mats_1.Count;

            if ( 1 < len_1 )
            {
                var mats_0 = _Wma_0.MorphoAmbiguityTuples;
                var mats_2 = _Wma_2.MorphoAmbiguityTuples;

                var len_0 = mats_0.Count;
                var len_2 = mats_2.Count;

                for ( var i = 0; i < len_0; i++ )
                {
                    _MatThreegram_0 = mats_0[ i ];

                    for ( var j = 0; j < len_1; j++ )
                    {
                        _MatThreegram_1 = mats_1[ j ];

                        for ( var k = 0; k < len_2; k++ )
                        {
                            _MatThreegram_2 = mats_2[ k ];

                            yield return (0);
                        }
                    }
                }
            }
        }
        private IEnumerable< int > IteratedOverThreeGramsMAT_4FirstWMA()
        {
            /*if ( _Wma_0.IsPunctuation() )
            {
                yield break;
            }*/
            var mats_0 = _Wma_0.MorphoAmbiguityTuples;
            var len_0  = mats_0.Count;

            var mats_1 = _Wma_1.MorphoAmbiguityTuples;
            var len_1  = mats_1.Count;

            var mats_2 = _Wma_2.MorphoAmbiguityTuples;
            var len_2  = mats_2.Count;

            if ( (1 < len_0) || (1 < len_1) ||
                 ((3 < _WordMorphoAmbiguitiesCount) /*||*/ && (1 < len_2))
               )
            {
                for ( var i = 0; i < len_0; i++ )
                {
                    _MatThreegram_0 = mats_0[ i ];

                    for ( var j = 0; j < len_1; j++ )
                    {
                        _MatThreegram_1 = mats_1[ j ];

                        for ( var k = 0; k < len_2; k++ )
                        {
                            _MatThreegram_2 = mats_2[ k ];

                            yield return (0);
                        }
                    }
                }
            }
        }
        private IEnumerable< int > IteratedOverThreeGramsMAT_4LastWMA()
        {
            if ( _Wma_2.IsPunctuation() )
            {
                yield break;
            }

            var mats_2 = _Wma_2.MorphoAmbiguityTuples;
            var len_2  = mats_2.Count;

            if ( 1 < len_2 )
            {
                var mats_0 = _Wma_0.MorphoAmbiguityTuples;
                var len_0  = mats_0.Count;

                var mats_1 = _Wma_1.MorphoAmbiguityTuples;
                var len_1  = mats_1.Count;

                for ( var i = 0; i < len_0; i++ )
                {
                    _MatThreegram_0 = mats_0[ i ];

                    for ( var j = 0; j < len_1; j++ )
                    {
                        _MatThreegram_1 = mats_1[ j ];

                        for ( var k = 0; k < len_2; k++ )
                        {
                            _MatThreegram_2 = mats_2[ k ];

                            yield return (0);
                        }
                    }
                }
            }
        }
        private IEnumerable< int > IteratedOverThreeGramsMAT_42WMA()
        {
            /*if ( _Wma_0.IsPunctuation() )
            {
                yield break;
            }*/
            var mats_0 = _Wma_0.MorphoAmbiguityTuples;
            var len_0  = mats_0.Count;

            var mats_1 = _Wma_1.MorphoAmbiguityTuples;
            var len_1  = mats_1.Count;

            if ( (1 < len_0) || (1 < len_1) )
            {
                for ( var i = 0; i < len_0; i++ )
                {
                    _MatThreegram_0 = mats_0[ i ];

                    for ( var j = 0; j < len_1; j++ )
                    {
                        _MatThreegram_1 = mats_1[ j ];

                        yield return (0);
                    }
                }
            }
        }

        private double? TaggingThreeGramsMAT()
        {
            var marginal = default(double?);

            #region [.first.]
                marginal = BrimFirstMAT();

                #region [.BOS.]
                _AttributeBufferPtr = CopyToZero( xlat_Unsafe.Inst._BeginOfSentencePtrBase, _AttributeBufferPtrBase );
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _MatThreegram_0 );
                var f1 = default(float);
                #region commented
                /*var attr1 = System.Text.Encoding.UTF8.GetString( _AttributeBuffer, 0, (int) (_AttributeBufferPtr - _AttributeBufferPtrBase) ); //new string( _AttributeBufferPtrBase );                
                if ( _ModelDictionary.TryGetValue( attr1, out f1 ) )
                {
                    if ( marginal.HasValue )
                        marginal *= f1;
                    else
                        marginal = f1;
                }*/
                #endregion
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f1 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f1;
                }
                #if DEBUG
                    _sb_attr_debug.Append( xlat_Unsafe.BEGIN_OF_SENTENCE ).Append( '\t' );
                #endif
                #endregion

                #if DEBUG
                    _sb_attr_debug.Append( '\n' );
                #endif
            #endregion

            #region [.middle.]
                var f = BrimMiddleMAT();
                if ( f.HasValue )
                {
                    marginal = marginal.GetValueOrDefault() + f.Value;
                }
                
                #if DEBUG
                    _sb_attr_debug.Append( '\n' );
                #endif
            #endregion

            #region [.last.]
                f = BrimLastMAT();
                if ( f.HasValue )
                {
                    marginal = marginal.GetValueOrDefault() + f.Value;
                }

                #region [.EOS.]
                _AttributeBufferPtr = CopyToZero( xlat_Unsafe.Inst._EndOfSentencePtrBase, _AttributeBufferPtrBase );
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _MatThreegram_2 );
                var f2 = default(float);
                #region commented
                /*var attr2 = System.Text.Encoding.UTF8.GetString( _AttributeBuffer, 0, (int) (_AttributeBufferPtr - _AttributeBufferPtrBase) ); //new string( _AttributeBufferPtrBase );                
                if ( _ModelDictionary.TryGetValue( attr2, out f2 ) )
                {
                    if ( marginal.HasValue )
                        marginal *= f2;
                    else
                        marginal = f2;
                }*/                
                #endregion
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f2 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f2;
                }
                #if DEBUG
                    _sb_attr_debug.Append( xlat_Unsafe.END_OF_SENTENCE ).Append( '\t' );
                #endif
                #endregion

                #if DEBUG
                    _sb_attr_debug.Append( '\n' );
                #endif
            #endregion

            #if DEBUG
                _sb_attr_debug.Append( '\n' );
            #endif

            return (marginal);
        }
        private double? TaggingBiGramsMAT_42WMA()
        {
            var marginal = default(double?);

            #region [.first.]
                marginal = BrimFirstMAT();

                #region [.BOS.]
                _AttributeBufferPtr = CopyToZero( xlat_Unsafe.Inst._BeginOfSentencePtrBase, _AttributeBufferPtrBase );
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _MatThreegram_0 );
                var f1 = default(float);
                #region commented
                /*var attr1 = System.Text.Encoding.UTF8.GetString( _AttributeBuffer, 0, (int) (_AttributeBufferPtr - _AttributeBufferPtrBase) ); //new string( _AttributeBufferPtrBase );                
                if ( _ModelDictionary.TryGetValue( attr1, out f1 ) )
                {
                    if ( marginal.HasValue )
                        marginal *= f1;
                    else
                        marginal = f1;
                }*/
                #endregion
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f1 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f1;
                }
                #if DEBUG
                    _sb_attr_debug.Append( xlat_Unsafe.BEGIN_OF_SENTENCE ).Append( '\t' );
                #endif
                #endregion

                #if DEBUG
                    _sb_attr_debug.Append( '\n' );
                #endif
            #endregion

            _MatThreegram_2 = _MatThreegram_1;
            _MatThreegram_1 = _MatThreegram_0;

            #region [.last.]
                var f = BrimLastMAT();
                if ( f.HasValue )
                {
                    marginal = marginal.GetValueOrDefault() + f.Value;
                }

                #region [.EOS.]
                _AttributeBufferPtr = CopyToZero( xlat_Unsafe.Inst._EndOfSentencePtrBase, _AttributeBufferPtrBase );
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _MatThreegram_2 );
                var f2 = default(float);
                #region commented
                /*var attr2 = System.Text.Encoding.UTF8.GetString( _AttributeBuffer, 0, (int) (_AttributeBufferPtr - _AttributeBufferPtrBase) ); //new string( _AttributeBufferPtrBase );                
                if ( _ModelDictionary.TryGetValue( attr2, out f2 ) )
                {
                    if ( marginal.HasValue )
                        marginal *= f2;
                    else
                        marginal = f2;
                }*/
                #endregion
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f2 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f2;
                }
                #if DEBUG
                    _sb_attr_debug.Append( xlat_Unsafe.END_OF_SENTENCE ).Append( '\t' );
                #endif
                #endregion

                #if DEBUG
                    _sb_attr_debug.Append( '\n' );
                #endif
            #endregion

            _MatThreegram_0 = _MatThreegram_1;
            _MatThreegram_1 = _MatThreegram_2;

            #if DEBUG
                _sb_attr_debug.Append( '\n' );
            #endif

            return (marginal);
        }

        private double? BrimFirstMAT()
        {
            var marginal = default(double?);

            for ( var i = 0; i < _NGramsFirstLength; i++ )
            {
                var ngram = _NGramsFirst[ i ];

                _AttributeBufferPtr = ngram.CopyAttributesHeaderChars( _AttributeBufferPtrBase );

                #region [.build attr-values.]
                switch ( ngram.CRFAttributesLength )
                {
                    case 4:
                        AppendAttrValueFirstMAT( ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueFirstMAT( ngram.CRFAttribute_1 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueFirstMAT( ngram.CRFAttribute_2 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueFirstMAT( ngram.CRFAttribute_3 );
                    break;

                    default:
                    #region
                    {
                        for ( var j = 0; j < ngram.CRFAttributesLength; j++ )
			            {
                            var crfAttr = ngram.CRFAttributes[ j ];
                            AppendAttrValueFirstMAT( crfAttr ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			            }
			            // Удалить последний '|'
                        _AttributeBufferPtr--;
                    }
                    #endregion
                    break;
                }
                #endregion

                #region [.retrieve-attr-values.]
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _MatThreegram_0 );
                var f0 = default(float);
                #region commented
                /*var attr1 = System.Text.Encoding.UTF8.GetString( _AttributeBuffer, 0, (int) (_AttributeBufferPtr - _AttributeBufferPtrBase) ); //new string( _AttributeBufferPtrBase );                
                if ( _ModelDictionary.TryGetValue( attr1, out f1 ) )
                {
                    if ( marginal.HasValue )
                        marginal *= f1;
                    else
                        marginal  = f1;
                }*/
                #endregion
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f0 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f0;
                }

                #if DEBUG
                    var attr_len = (int) (_AttributeBufferPtr - _AttributeBufferPtrBase);                        
                    fixed ( char* chars_ptr = _chars_attr_debug )
                    {
                        var chars_len = System.Text.Encoding.UTF8.GetChars( _AttributeBufferPtrBase, attr_len, chars_ptr, _chars_attr_debug.Length );
                        var s_debug = new string( chars_ptr, 0, chars_len );
                        _sb_attr_debug.Append( s_debug ).Append( '\t' );
                    }
                #endif
                #endregion
            }

            return (marginal);
        }
        private double? BrimMiddleMAT()
        {
            var marginal = default(double?);

            for ( var i = 0; i < _NGramsMiddleLength; i++ )
            {
                var ngram = _NGramsMiddle[ i ];

                _AttributeBufferPtr = ngram.CopyAttributesHeaderChars( _AttributeBufferPtrBase );

                #region [.build attr-values.]
                switch ( ngram.CRFAttributesLength )
                {
                    case 4:
                        AppendAttrValueMiddleMAT( ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueMiddleMAT( ngram.CRFAttribute_1 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueMiddleMAT( ngram.CRFAttribute_2 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueMiddleMAT( ngram.CRFAttribute_3 );
                    break;

                    default:
                    #region
                    {
                        for ( var j = 0; j < ngram.CRFAttributesLength; j++ )
			            {
                            var crfAttr = ngram.CRFAttributes[ j ];
                            AppendAttrValueMiddleMAT( crfAttr ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			            }
			            // Удалить последний '|'
                        _AttributeBufferPtr--;
                    }
                    #endregion
                    break;
                }
                #endregion

                #region [.retrieve-attr-values.]
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _MatThreegram_1 );
                var f1 = default(float);
                #region commented
                /*var attr2 = System.Text.Encoding.UTF8.GetString( _AttributeBuffer, 0, (int) (_AttributeBufferPtr - _AttributeBufferPtrBase) ); //new string( _AttributeBufferPtrBase );                
                if ( _ModelDictionary.TryGetValue( attr2, out f2 ) )
                {
                    if ( marginal.HasValue )
                        marginal *= f2;
                    else
                        marginal = f2;
                }*/
                #endregion
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f1 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f1;
                }

                #if DEBUG
                    var attr_len = (int) (_AttributeBufferPtr - _AttributeBufferPtrBase);
                    fixed ( char* chars_ptr = _chars_attr_debug )
                    {
                        var chars_len = System.Text.Encoding.UTF8.GetChars( _AttributeBufferPtrBase, attr_len, chars_ptr, _chars_attr_debug.Length );
                        var s_debug = new string( chars_ptr, 0, chars_len );
                        _sb_attr_debug.Append( s_debug ).Append( '\t' );
                    }
                #endif
                #endregion
            }

            return (marginal);
        }
        private double? BrimLastMAT()
        {
            var marginal = default(double?);

            for ( var i = 0; i < _NGramsLastLength; i++ )
            {
                var ngram = _NGramsLast[ i ];

                _AttributeBufferPtr = ngram.CopyAttributesHeaderChars( _AttributeBufferPtrBase );

                #region [.build attr-values.]
                switch ( ngram.CRFAttributesLength )
                {
                    case 4:
                        AppendAttrValueLastMAT( ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT( ngram.CRFAttribute_1 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT( ngram.CRFAttribute_2 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT( ngram.CRFAttribute_3 );
                    break;

                    default:
                    #region
                    {
                        for ( var j = 0; j < ngram.CRFAttributesLength; j++ )
			            {
                            var crfAttr = ngram.CRFAttributes[ j ];
                            AppendAttrValueLastMAT( crfAttr ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			            }
			            // Удалить последний '|'
                        _AttributeBufferPtr--;
                    }
                    #endregion
                    break;
                }
                #endregion

                #region [.retrieve-attr-values.]
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _MatThreegram_2 );
                var f2 = default(float);
                #region commented
                /*var attr3 = System.Text.Encoding.UTF8.GetString( _AttributeBuffer, 0, (int) (_AttributeBufferPtr - _AttributeBufferPtrBase) ); //new string( _AttributeBufferPtrBase );                
                if ( _ModelDictionary.TryGetValue( attr3, out f3 ) )
                {
                    if ( marginal.HasValue )
                        marginal *= f3;
                    else
                        marginal = f3;
                }*/
                #endregion
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f2 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f2;
                }

                #if DEBUG
                    var attr_len = (int) (_AttributeBufferPtr - _AttributeBufferPtrBase);
                    fixed ( char* chars_ptr = _chars_attr_debug )
                    {
                        var chars_len = System.Text.Encoding.UTF8.GetChars( _AttributeBufferPtrBase, attr_len, chars_ptr, _chars_attr_debug.Length );
                        var s_debug = new string( chars_ptr, 0, chars_len );
                        _sb_attr_debug.Append( s_debug ).Append( '\t' );
                    }
                #endif
                #endregion
            }

            return (marginal);
        }

        private void AppendAttrValueFirstMAT ( CRFAttribute crfAttribute )
        {
            /* crfAttribute.Position = [-0,1] */

            switch ( crfAttribute.Position )
            {
                case 0:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _MatThreegram_0 );
                break;

                case 1: 
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _MatThreegram_1 );
                break;

                default: throw (new ArgumentException("position: " + crfAttribute.Position));
            }
        }
        private void AppendAttrValueMiddleMAT( CRFAttribute crfAttribute )
        {
            /* crfAttribute.Position = [-1,0,1] */

            switch ( crfAttribute.Position )
            {
                case -1:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _MatThreegram_0 );
                break;

                case  0:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _MatThreegram_1 );
                break;

                case 1:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _MatThreegram_2 );
                break;

                default: throw (new ArgumentException("position: " + crfAttribute.Position));
            }
        }
        private void AppendAttrValueLastMAT  ( CRFAttribute crfAttribute )
        {
            /* crfAttribute.Position = [-1,0] */

            switch ( crfAttribute.Position )
            {
                case -1:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _MatThreegram_1 );
                break;

                case  0:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _MatThreegram_2 );
                break;

                default: throw (new ArgumentException("position: " + crfAttribute.Position));
            }
        }

        unsafe private static byte* CopyToZero( byte* src, byte* dist )
        {
            for ( ; ; dist++, src++ )
            {
                var ch = *src;
                if ( ch == ZERO )
                    return (dist);

                *dist = *src;
            }
        }
        unsafe private static void FillMat5CharsBufferWithZero( byte* ptr, MorphoAmbiguityTuple_t mat )
        {
            *(ptr++) = SEMICOLON;
            *(ptr++) = MA.get_CRF_W_field_value( mat );
            *(ptr++) = MA.get_CRF_A_field_value( mat );
            *(ptr++) = MA.get_CRF_B_field_value( mat );
            *(ptr++) = MA.get_CRF_C_field_value( mat );
            *(ptr++) = MA.get_CRF_D_field_value( mat );
            *(ptr  ) = ZERO;
        }

        private static byte GetAttrValue( int columnIndex, MorphoAmbiguityTuple_t mat )
        {
            switch ( columnIndex )
            {
                //w – part-of-speech
                case 0:
                    return (MA.get_CRF_W_field_value( mat ));

                //a - Person
                case 1:
                    return (MA.get_CRF_A_field_value( mat ));

                //b - Case
                case 2:
                    return (MA.get_CRF_B_field_value( mat ));

                //c - Number
                case 3:
                    return (MA.get_CRF_C_field_value( mat ));

                //d - Gender
                case 4:
                    return (MA.get_CRF_D_field_value( mat ));

                //y – искомое значение
                case 5:
                    return (MA.get_CRF_Y_field_value());

                default: throw (new ArgumentException("columnIndex: " + columnIndex));
            }
        }

        private static void SetBestWordMorphology( MorphoAmbiguityTuple_t max_morphoAmbiguityTuple, WordMorphoAmbiguity_t wma )
        {
            if ( max_morphoAmbiguityTuple != default(MorphoAmbiguityTuple_t) )
            {
                max_morphoAmbiguityTuple.Word.morphology = max_morphoAmbiguityTuple.WordFormMorphology;
                wma.SetWordMorphologyAsDefined();
            }
            else
            {
                wma.SetWordMorphologyAsUndefined();
            }
        }
    }


    //========================= 5-Grams =========================//

    /// <summary>
    /// Конвертор в формат CRF
    /// </summary>
    unsafe internal sealed class MorphoAmbiguityResolver_5g : IDisposable
	{
        #region [.private field's.]
        private const byte ZERO           = (byte) '\0';
        private const byte VERTICAL_SLASH = (byte) '|';
        private const byte SEMICOLON      = (byte) ';';
        private const int ATTRIBUTE_MAX_LENGTH        = 1024; //1KB
        private const int MAT_FIVEGRAMS_SIZE          = 5;
        private const int MAT_FIVEGRAMS_FIRST_INDEX   = 0;
        private const int MAT_FIVEGRAMS_SECOND_INDEX  = 1;
        private const int MAT_FIVEGRAMS_MIDDLE_INDEX  = 2;
        private const int MAT_FIVEGRAMS_PRELAST_INDEX = 3;
        private const int MAT_FIVEGRAMS_LAST_INDEX    = 4;

        private readonly MorphoAmbiguityResolverModel _Model;
        private readonly Dictionary< IntPtr, float >  _ModelDictionaryBytes;
        private readonly CRFNgram[]           _NGramsFirst;
        private readonly int                  _NGramsFirstLength;
        private readonly CRFNgram[]           _NGramsSecond;
        private readonly int                  _NGramsSecondLength;
        private readonly CRFNgram[]           _NGramsMiddle;
        private readonly int                  _NGramsMiddleLength;
        private readonly CRFNgram[]           _NGramsPreLast;
        private readonly int                  _NGramsPreLastLength; 
        private readonly CRFNgram[]           _NGramsLast;
        private readonly int                  _NGramsLastLength;

        //===============================================//
        //private readonly MorphoAmbiguityResolverModel _MorphoAmbiguityModel_3g;
        //private readonly MorphoAmbiguityResolver_3g   _MorphoAmbiguityResolver_3g;
        private readonly CRFNgram[]           _NGramsFirst42;
        private readonly int                  _NGramsFirst42Length;
        private readonly CRFNgram[]           _NGramsLast42;
        private readonly int                  _NGramsLast42Length;

        private readonly CRFNgram[]           _NGramsSecond43;
        private readonly int                  _NGramsSecond43Length;
        //===============================================//

        private readonly GCHandle             _AttributeBufferGCHandle;
        private readonly byte[]               _AttributeBuffer;
        private byte*                         _AttributeBufferPtrBase;
        private byte*                         _AttributeBufferPtr;
        private List< WordMorphoAmbiguity_t > _WordMorphoAmbiguities;
        private int                           _WordMorphoAmbiguitiesCount;
        private WordMorphoAmbiguity_t         _Wma_0, _Wma_1, _Wma_2, _Wma_3, _Wma_4;
        private MorphoAmbiguityTuple_t        _Mat_0, _Mat_1, _Mat_2, _Mat_3, _Mat_4;

        #if DEBUG
            private readonly System.Text.StringBuilder _sb_attr_debug = new System.Text.StringBuilder();
            private readonly char[] _chars_attr_debug = new char[ ATTRIBUTE_MAX_LENGTH * 10 ];
        #endif
        #endregion

        #region [.ctor().]
        public MorphoAmbiguityResolver_5g( MorphoAmbiguityResolverModel model )
		{
            //-1-
            _Model = model;
            _ModelDictionaryBytes = model.DictionaryBytes;

            //-2-
            var crfTemplateFile_5g = LoadTemplate( model.Config.TemplateFilename_5g );
            _NGramsFirst         = crfTemplateFile_5g.GetCRFNgramsWhichCanTemplateBeApplied( MAT_FIVEGRAMS_FIRST_INDEX   /*first-word-(morpho-attr)-index == 0*/  , MAT_FIVEGRAMS_SIZE /*5*/ );
            _NGramsFirstLength   = _NGramsFirst.Length;
            _NGramsSecond        = crfTemplateFile_5g.GetCRFNgramsWhichCanTemplateBeApplied( MAT_FIVEGRAMS_SECOND_INDEX  /*second-word-(morpho-attr)-index == 1*/ , MAT_FIVEGRAMS_SIZE /*5*/ );
            _NGramsSecondLength  = _NGramsSecond.Length;
            _NGramsMiddle        = crfTemplateFile_5g.GetCRFNgramsWhichCanTemplateBeApplied( MAT_FIVEGRAMS_MIDDLE_INDEX  /*middle-word-(morpho-attr)-index == 2*/ , MAT_FIVEGRAMS_SIZE /*5*/ );
            _NGramsMiddleLength  = _NGramsMiddle.Length;
            _NGramsPreLast       = crfTemplateFile_5g.GetCRFNgramsWhichCanTemplateBeApplied( MAT_FIVEGRAMS_PRELAST_INDEX /*prelast-word-(morpho-attr)-index == 3*/, MAT_FIVEGRAMS_SIZE /*5*/ );
            _NGramsPreLastLength = _NGramsPreLast.Length;
            _NGramsLast          = crfTemplateFile_5g.GetCRFNgramsWhichCanTemplateBeApplied( MAT_FIVEGRAMS_LAST_INDEX    /*last-word-(morpho-attr)-index == 4*/   , MAT_FIVEGRAMS_SIZE /*5*/ );
            _NGramsLastLength    = _NGramsLast.Length;

            //-2.1- //sent from 2-word's => [0,1];[-1,0]
            var crfTemplateFile_3g  = LoadTemplate( model.Config.TemplateFilename_3g );
            _NGramsFirst42          = crfTemplateFile_3g.GetCRFNgramsWhichCanTemplateBeApplied( 0, 2 );
            _NGramsFirst42Length    = _NGramsFirst42.Length;
            _NGramsLast42           = crfTemplateFile_3g.GetCRFNgramsWhichCanTemplateBeApplied( 1, 2 );
            _NGramsLast42Length     = _NGramsLast42.Length;
            /*var config = new MorphoAmbiguityResolverConfig()
            {
                ModelFilename    = Path.Combine( Path.GetDirectoryName( model.Config.ModelFilename ), "dsf_pa_(morpho_ambiguity)_3g.txt" ),
                TemplateFilename = Path.Combine( Path.GetDirectoryName( model.Config.ModelFilename ), "templateMorphoAmbiguity_3g.txt"   ),
            };
            _MorphoAmbiguityModel_3g    = new MorphoAmbiguityResolverModel( config );
            _MorphoAmbiguityResolver_3g = new MorphoAmbiguityResolver_3g( _MorphoAmbiguityModel_3g );*/

            //-2.2- //sent from 3-word's => [(0,1,2)];[(-1,0,1)];[(-2,-1,0)]
            _NGramsSecond43       = crfTemplateFile_5g.GetCRFNgramsWhichCanTemplateBeApplied( 1, 3 );
            _NGramsSecond43Length = _NGramsSecond43.Length;

            //-3-
            _AttributeBuffer         = new byte[ ATTRIBUTE_MAX_LENGTH + 1 ];
            _AttributeBufferGCHandle = GCHandle.Alloc( _AttributeBuffer, GCHandleType.Pinned );
            _AttributeBufferPtrBase  = (byte*) _AttributeBufferGCHandle.AddrOfPinnedObject().ToPointer();
		}

        ~MorphoAmbiguityResolver_5g()
        {
            DisposeNativeResources();
        }
        public void Dispose()
        {
            DisposeNativeResources();

            GC.SuppressFinalize( this );
        }
        private void DisposeNativeResources()
        {
            if ( _AttributeBufferPtrBase != null )
            {
                _AttributeBufferGCHandle.Free();
                _AttributeBufferPtrBase = null;
            }

            //_MorphoAmbiguityModel_3g   .Dispose();
            //_MorphoAmbiguityResolver_3g.Dispose();
        }

        /// <summary>
        /// Получить шаблон 
		/// </summary>
		/// <param name="templatePath">путь к файлу шаблона</param>
		/// <returns>Шаблон</returns>
        private static CRFTemplateFile LoadTemplate( string templatePath )
		{
			var result = CRFTemplateFileLoader.Load( templatePath );
			CheckTemplate( result );
			return (result);
		}
		/// <summary>
        /// Проверить правильность шаблона
		/// </summary>
        /// <param name="crfTemplateFile">Шаблон</param>
		private static void CheckTemplate( CRFTemplateFile crfTemplateFile )
		{
			foreach ( CRFNgram ngram in crfTemplateFile.CRFNgrams )
			{
				foreach ( CRFAttribute crfAttribute in ngram.CRFAttributes )
				{
                    switch ( crfAttribute.Position )
                    {
                        case -2:
                        case -1:
                        case  0:
                        case  1: 
                        case  2: 
                            break;
                        default:
					        throw (new Exception("Аттрибут '" + crfAttribute.AttributeName + "' содержащит недопустимое значение индекса морфо-атрибута: '" + crfAttribute.Position + '\''));                            
                    }
				}
			}
		}
        #endregion


        public void Resolve( List< WordMorphoAmbiguity_t > wordMorphoAmbiguities )
        {
            #region [.init.]
            _WordMorphoAmbiguities      = wordMorphoAmbiguities;
            _WordMorphoAmbiguitiesCount = _WordMorphoAmbiguities.Count;
            #if DEBUG
                _sb_attr_debug.Clear();
            #endif
            #endregion

            #region [.special-case-with-0,1,2-WMA.]
            switch ( _WordMorphoAmbiguitiesCount )
            {
                case 0: goto UNINIT;
                case 1: 
                    _WordMorphoAmbiguities[ 0 ].SetWordMorphologyAsUndefined();
                goto UNINIT;

                case 2:
                    Resolve42();
                goto UNINIT;
            }
            #endregion

            #region [.first-wma.]
            _Wma_0 = _WordMorphoAmbiguities[ 0 ];
            _Wma_1 = _WordMorphoAmbiguities[ 1 ];
            _Wma_2 = _WordMorphoAmbiguities[ 2 ];

            var prob = probability_t.Create();

            foreach ( var _ in IteratedOverFirstMAT() )
            {
                //crf-tagging
                var probability = TaggingFirstMAT();
                if ( !probability.HasValue )
                {
                    continue;
                }
                 
                #region [.choose max-probability.]
                if ( prob.max_probability < probability.Value )
                {
                    prob.max_morphoAmbiguityTuple_0 = _Mat_0;
                    prob.max_probability = probability.Value;
                }
                #endregion
            }

            //set best word-morphology
            SetBestWordMorphology( prob.max_morphoAmbiguityTuple_0, _Wma_0 );
            #endregion

            #region [.special-case-with-3,4,5-&-more-WMA.]
            switch ( _WordMorphoAmbiguitiesCount )
            {
                case 3:
                    Resolve43();
                break;

                case 4:
                    _Wma_3 = _WordMorphoAmbiguities[ 3 ];
                    Resolve44();
                break;

                default: //sent from 5-&-more-word's
                    _Wma_3 = _WordMorphoAmbiguities[ 3 ];
                    _Wma_4 = _WordMorphoAmbiguities[ 4 ];
                    Resolve45();
                break;
            }
            #endregion

            #region [.un-init.]
        UNINIT:
            _WordMorphoAmbiguities = null;
            _Mat_0 = _Mat_1 = _Mat_2 = _Mat_3 = _Mat_4 = default(MorphoAmbiguityTuple_t);
            _Wma_0 = _Wma_1 = _Wma_2 = _Wma_3 = _Wma_4 = default(WordMorphoAmbiguity_t);
            #if DEBUG
                var _attr_debug = _sb_attr_debug.ToString();
            #endif
            #endregion            
        }

        private IEnumerable< int > IteratedOverFirstMAT()
        {
            if ( _Wma_0.IsPunctuation() )
            {
                yield break;
            }
            var mats_0 = _Wma_0.MorphoAmbiguityTuples;
            var len_0  = mats_0.Count;

            if ( 1 < len_0 )
            {
                var mats_1 = _Wma_1.MorphoAmbiguityTuples;
                var mats_2 = _Wma_2.MorphoAmbiguityTuples;

                var len_1  = mats_1.Count;
                var len_2  = mats_2.Count;

                for ( var i = 0; i < len_0; i++ )
                {
                    _Mat_0 = mats_0[ i ];

                    for ( var j = 0; j < len_1; j++ )
                    {
                        _Mat_1 = mats_1[ j ];

                        for ( var x = 0; x < len_2; x++ )
                        {
                            _Mat_2 = mats_2[ x ];

                            yield return (0);
                        }
                    }
                }
            }
        }

        private double? TaggingFirstMAT()
        {
            var marginal = default(double?);

            #region [.first.]
                marginal = BrimFirstMAT();

                #region [.BOS.]
                _AttributeBufferPtr = CopyToZero( xlat_Unsafe.Inst._BeginOfSentencePtrBase, _AttributeBufferPtrBase );
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_0 );
                var f1 = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f1 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f1;
                }
                #if DEBUG
                    _sb_attr_debug.Append( xlat_Unsafe.BEGIN_OF_SENTENCE ).Append( '\t' );
                #endif
                #endregion

                #if DEBUG
                    _sb_attr_debug.Append( '\n' );
                #endif
            #endregion

            #if DEBUG
                _sb_attr_debug.Append( '\n' );
            #endif

            return (marginal);
        }

        private double? BrimFirstMAT()
        {
            var marginal = default(double?);

            for ( var i = 0; i < _NGramsFirstLength; i++ )
            {
                var ngram = _NGramsFirst[ i ];

                _AttributeBufferPtr = ngram.CopyAttributesHeaderChars( _AttributeBufferPtrBase );

                #region [.build attr-values.]
                switch ( ngram.CRFAttributesLength )
                {
                    case 6:
                        AppendAttrValueFirstMAT( ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueFirstMAT( ngram.CRFAttribute_1 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueFirstMAT( ngram.CRFAttribute_2 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueFirstMAT( ngram.CRFAttribute_3 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueFirstMAT( ngram.CRFAttribute_4 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueFirstMAT( ngram.CRFAttribute_5 ); 
                    break;

                    default:
                    #region
                    {
                        for ( var j = 0; j < ngram.CRFAttributesLength; j++ )
			            {
                            var crfAttr = ngram.CRFAttributes[ j ];
                            AppendAttrValueFirstMAT( crfAttr ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			            }
			            // Удалить последний '|'
                        _AttributeBufferPtr--;
                    }
                    #endregion
                    break;
                }
                #endregion

                #region [.retrieve-attr-values.]
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_0 );
                var f0 = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f0 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f0;
                }

                #if DEBUG
                    var attr_len = (int) (_AttributeBufferPtr - _AttributeBufferPtrBase);                        
                    fixed ( char* chars_ptr = _chars_attr_debug )
                    {
                        var chars_len = System.Text.Encoding.UTF8.GetChars( _AttributeBufferPtrBase, attr_len, chars_ptr, _chars_attr_debug.Length );
                        var s_debug = new string( chars_ptr, 0, chars_len );
                        _sb_attr_debug.Append( s_debug ).Append( '\t' );
                    }
                #endif
                #endregion
            }

            return (marginal);
        }

        private void AppendAttrValueFirstMAT( CRFAttribute crfAttribute )
        {
            /* crfAttribute.Position = [0,1,2] */

            switch ( crfAttribute.Position )
            {
                case 0:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_0 );
                break;

                case 1: 
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_1 );
                break;

                case 2: 
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_2 );
                break;

                default: throw (new ArgumentException("position: " + crfAttribute.Position));
            }
        }


        #region [.sent from 2-word's.]
        /// <summary>
        /// sent from 2-word's => [(0,1)];[(-1,0)]
        /// </summary>
        private void Resolve42()
        {
            //_MorphoAmbiguityResolver_3g.Resolve( _WordMorphoAmbiguities );
        //return;

            _Wma_0 = _WordMorphoAmbiguities[ 0 ];
            _Wma_1 = _WordMorphoAmbiguities[ 1 ];

            var prob = probability_t.Create();
                    
            foreach ( var _ in IteratedOverMAT42() )
            {
                //crf-tagging
                var probability = TaggingMAT42();
                if ( probability.HasValue )
                {
                    #region [.choose max-probability.]
                    if ( prob.max_probability < probability.Value )
                    {
                        prob.max_morphoAmbiguityTuple_0 = _Mat_0;
                        prob.max_morphoAmbiguityTuple_1 = _Mat_1;
                        prob.max_probability = probability.Value;
                    }
                    #endregion
                }
            }

            //set best word-morphology
            SetBestWordMorphology( prob.max_morphoAmbiguityTuple_0, _Wma_0 );
            SetBestWordMorphology( prob.max_morphoAmbiguityTuple_1, _Wma_1 );
        }

        private IEnumerable< int > IteratedOverMAT42()
        {
            /*if ( _Wma_0.IsPunctuation() )
            {
                yield break;
            }*/
            var mats_0 = _Wma_0.MorphoAmbiguityTuples;
            var len_0  = mats_0.Count;

            var mats_1 = _Wma_1.MorphoAmbiguityTuples;
            var len_1  = mats_1.Count;

            if ( (1 < len_0) || (1 < len_1) )
            {
                for ( var i = 0; i < len_0; i++ )
                {
                    _Mat_0 = mats_0[ i ];

                    for ( var j = 0; j < len_1; j++ )
                    {
                        _Mat_1 = mats_1[ j ];

                        yield return (0);
                    }
                }
            }
        }

        private double? TaggingMAT42()
        {
            var marginal = default(double?);

            #region [.first.]
                marginal = BrimFirstMAT42();

                #region [.BOS.]
                _AttributeBufferPtr = CopyToZero( xlat_Unsafe.Inst._BeginOfSentencePtrBase, _AttributeBufferPtrBase );
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_0 );
                var f1 = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f1 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f1;
                }
                #if DEBUG
                    _sb_attr_debug.Append( xlat_Unsafe.BEGIN_OF_SENTENCE ).Append( '\t' );
                #endif
                #endregion

                #if DEBUG
                    _sb_attr_debug.Append( '\n' );
                #endif
            #endregion

            //_Mat_2 = _Mat_1;
            //_Mat_1 = _Mat_0;

            #region [.last.]
                var f = BrimLastMAT42();
                if ( f.HasValue )
                {
                    marginal = marginal.GetValueOrDefault() + f.Value;
                }

                #region [.EOS.]
                _AttributeBufferPtr = CopyToZero( xlat_Unsafe.Inst._EndOfSentencePtrBase, _AttributeBufferPtrBase );
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_1 );
                var f2 = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f2 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f2;
                }
                #if DEBUG
                    _sb_attr_debug.Append( xlat_Unsafe.END_OF_SENTENCE ).Append( '\t' );
                #endif
                #endregion

                #if DEBUG
                    _sb_attr_debug.Append( '\n' );
                #endif
            #endregion

            //_Mat_0 = _Mat_1;
            //_Mat_1 = _Mat_2;

            #if DEBUG
                _sb_attr_debug.Append( '\n' );
            #endif

            return (marginal);
        }

        private double? BrimFirstMAT42()
        {
            var marginal = default(double?);

            for ( var i = 0; i < _NGramsFirst42Length; i++ )
            {
                var ngram = _NGramsFirst42[ i ];

                _AttributeBufferPtr = ngram.CopyAttributesHeaderChars( _AttributeBufferPtrBase );

                #region [.build attr-values.]
                switch ( ngram.CRFAttributesLength )
                {
                    case 4:
                        AppendAttrValueFirstMAT42( ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueFirstMAT42( ngram.CRFAttribute_1 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueFirstMAT42( ngram.CRFAttribute_2 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueFirstMAT42( ngram.CRFAttribute_3 );
                    break;

                    default:
                    #region
                    {
                        for ( var j = 0; j < ngram.CRFAttributesLength; j++ )
			            {
                            var crfAttr = ngram.CRFAttributes[ j ];
                            AppendAttrValueFirstMAT42( crfAttr ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			            }
			            // Удалить последний '|'
                        _AttributeBufferPtr--;
                    }
                    #endregion
                    break;
                }
                #endregion

                #region [.retrieve-attr-values.]
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_0 );
                var f = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f ) )
                {
                    marginal = marginal.GetValueOrDefault() + f;
                }

                #if DEBUG
                    var attr_len = (int) (_AttributeBufferPtr - _AttributeBufferPtrBase);
                    fixed ( char* chars_ptr = _chars_attr_debug )
                    {
                        var chars_len = System.Text.Encoding.UTF8.GetChars( _AttributeBufferPtrBase, attr_len, chars_ptr, _chars_attr_debug.Length );
                        var s_debug = new string( chars_ptr, 0, chars_len );
                        _sb_attr_debug.Append( s_debug ).Append( '\t' );
                    }
                #endif
                #endregion
            }

            return (marginal);
        }
        private double? BrimLastMAT42()
        {
            var marginal = default(double?);

            for ( var i = 0; i < _NGramsLast42Length; i++ )
            {
                var ngram = _NGramsLast42[ i ];

                _AttributeBufferPtr = ngram.CopyAttributesHeaderChars( _AttributeBufferPtrBase );

                #region [.build attr-values.]
                switch ( ngram.CRFAttributesLength )
                {
                    case 4:
                        AppendAttrValueLastMAT42( ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT42( ngram.CRFAttribute_1 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT42( ngram.CRFAttribute_2 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT42( ngram.CRFAttribute_3 );
                    break;

                    default:
                    #region
                    {
                        for ( var j = 0; j < ngram.CRFAttributesLength; j++ )
			            {
                            var crfAttr = ngram.CRFAttributes[ j ];
                            AppendAttrValueLastMAT42( crfAttr ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			            }
			            // Удалить последний '|'
                        _AttributeBufferPtr--;
                    }
                    #endregion
                    break;
                }
                #endregion

                #region [.retrieve-attr-values.]
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_1 );
                var f = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f ) )
                {
                    marginal = marginal.GetValueOrDefault() + f;
                }

                #if DEBUG
                    var attr_len = (int) (_AttributeBufferPtr - _AttributeBufferPtrBase);
                    fixed ( char* chars_ptr = _chars_attr_debug )
                    {
                        var chars_len = System.Text.Encoding.UTF8.GetChars( _AttributeBufferPtrBase, attr_len, chars_ptr, _chars_attr_debug.Length );
                        var s_debug = new string( chars_ptr, 0, chars_len );
                        _sb_attr_debug.Append( s_debug ).Append( '\t' );
                    }
                #endif
                #endregion
            }

            return (marginal);
        }

        private void AppendAttrValueFirstMAT42( CRFAttribute crfAttribute )
        {
            /* crfAttribute.Position = [0,1] */

            switch ( crfAttribute.Position )
            {
                case 0:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_0 );
                break;

                case 1:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_1 );
                break;

                default: throw (new ArgumentException("position: " + crfAttribute.Position));
            }
        }
        private void AppendAttrValueLastMAT42 ( CRFAttribute crfAttribute )
        {
            /* crfAttribute.Position = [-1,0] */

            switch ( crfAttribute.Position )
            {
                case -1:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_0 );
                break;

                case  0:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_1 );
                break;

                default: throw (new ArgumentException("position: " + crfAttribute.Position));
            }
        }
        #endregion


        #region [.sent from 3-word's.]
        /// <summary>
        /// sent from 3-word's => [(0,1,2)];[(-1,0,1)];[(-2,-1,0)]
        /// </summary>
        private void Resolve43()
        {
            #region [.second/middle-wma.]
            var prob = probability_t.Create();

            foreach ( var _ in IteratedOverSecondMAT43() )
            {
                //crf-tagging
                var probability = TaggingSecondMAT43();
                if ( probability.HasValue )
                {
                    #region [.choose max-probability.]
                    if ( prob.max_probability < probability.Value )
                    {
                        prob.max_morphoAmbiguityTuple_1 = _Mat_1;
                        prob.max_probability = probability.Value;
                    }
                    #endregion
                }
            }

            //set best word-morphology
            SetBestWordMorphology( prob.max_morphoAmbiguityTuple_1, _Wma_1 );
            #endregion

            #region [.last-wma.]
            prob = probability_t.Create();

            foreach ( var _ in IteratedOverLastMAT43() )
            {
                //crf-tagging
                var probability = TaggingLastMAT43();
                if ( probability.HasValue )
                {
                    #region [.choose max-probability.]
                    if ( prob.max_probability < probability.Value )
                    {
                        prob.max_morphoAmbiguityTuple_2 = _Mat_2;
                        prob.max_probability = probability.Value;
                    }
                    #endregion
                }
            }

            //set best word-morphology
            SetBestWordMorphology( prob.max_morphoAmbiguityTuple_2, _Wma_2 );
            #endregion
        }

        private IEnumerable< int > IteratedOverSecondMAT43()
        {
            if ( _Wma_1.IsPunctuation() )
            {
                yield break;
            }
            var mats_1 = _Wma_1.MorphoAmbiguityTuples;
            var len_1  = mats_1.Count;

            if ( 1 < len_1 )
            {
                Debug.Assert( _Wma_0.MorphoAmbiguityTuples.Count == 1, "(_Wma_0.MorphoAmbiguityTuples.Count != 1)" );

                //-must-be-1-/ var mats_0 = _Wma_0.MorphoAmbiguityTuples;
                var mats_2 = _Wma_2.MorphoAmbiguityTuples;

                //-must-be-1-/ var len_0 = mats_0.Count;
                var len_2 = mats_2.Count;

                //-must-be-1-/ for ( var i = 0; i < len_0; i++ )
                //{
                    //-must-be-1-/ _Mat_0 = mats_0[ i ];
                    _Mat_0 = _Wma_0.MorphoAmbiguityTuples[ 0 ];

                    for ( var j = 0; j < len_1; j++ )
                    {
                        _Mat_1 = mats_1[ j ];

                        for ( var x = 0; x < len_2; x++ )
                        {
                            _Mat_2 = mats_2[ x ];

                            yield return (0);
                        }
                    }
                //}
            }
        }
        private IEnumerable< int > IteratedOverLastMAT43()
        {
            if ( _Wma_2.IsPunctuation() )
            {
                yield break;
            }

            var mats_2 = _Wma_2.MorphoAmbiguityTuples;
            var len_2  = mats_2.Count;

            if ( 1 < len_2 )
            {
                Debug.Assert( _Wma_0.MorphoAmbiguityTuples.Count == 1, "(_Wma_0.MorphoAmbiguityTuples.Count != 1)" );
                Debug.Assert( _Wma_1.MorphoAmbiguityTuples.Count == 1, "(_Wma_1.MorphoAmbiguityTuples.Count != 1)" );

                //-must-be-1-/ var mats_0 = _Wma_0.MorphoAmbiguityTuples;
                //-must-be-1-/ var mats_1 = _Wma_1.MorphoAmbiguityTuples;

                //-must-be-1-/ var len_0  = mats_0.Count;
                //-must-be-1-/ var len_1  = mats_1.Count;

                //-must-be-1-/ for ( var i = 0; i < len_0; i++ )
                //{
                    //-must-be-1-/ _Mat_0 = mats_0[ i ];
                    _Mat_0 = _Wma_0.MorphoAmbiguityTuples[ 0 ];

                    //-must-be-1-/ for ( var j = 0; j < len_1; j++ )
                    //{
                        //-must-be-1-/ _Mat_1 = mats_1[ j ];
                        _Mat_1 = _Wma_1.MorphoAmbiguityTuples[ 0 ];

                        for ( var x = 0; x < len_2; x++ )
                        {
                            _Mat_2 = mats_2[ x ];

                            yield return (0);
                        }
                    //}
                //}
            }
        }

        private double? TaggingSecondMAT43()
        {
            var marginal = default(double?);

            #region [.second.]
                marginal = BrimSecondMAT43();

                #if DEBUG
                    _sb_attr_debug.Append( '\n' );
                #endif
            #endregion

            #if DEBUG
                _sb_attr_debug.Append( '\n' );
            #endif

            return (marginal);
        }
        private double? TaggingLastMAT43()
        {
            var marginal = default(double?);

            #region [.last.]
                marginal = BrimLastMAT43();

                #region [.EOS.]
                _AttributeBufferPtr = CopyToZero( xlat_Unsafe.Inst._EndOfSentencePtrBase, _AttributeBufferPtrBase );
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_2 );
                var f2 = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f2 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f2;
                }
                #if DEBUG
                    _sb_attr_debug.Append( xlat_Unsafe.END_OF_SENTENCE ).Append( '\t' );
                #endif
                #endregion

                #if DEBUG
                    _sb_attr_debug.Append( '\n' );
                #endif
            #endregion

            #if DEBUG
                _sb_attr_debug.Append( '\n' );
            #endif

            return (marginal);
        }

        private double? BrimSecondMAT43()
        {
            var marginal = default(double?);

            for ( var i = 0; i < _NGramsSecond43Length; i++ )
            {
                var ngram = _NGramsSecond43[ i ];

                _AttributeBufferPtr = ngram.CopyAttributesHeaderChars( _AttributeBufferPtrBase );

                #region [.build attr-values.]
                switch ( ngram.CRFAttributesLength )
                {
                    case 6:
                        AppendAttrValueSecondMAT43( ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueSecondMAT43( ngram.CRFAttribute_1 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueSecondMAT43( ngram.CRFAttribute_2 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueSecondMAT43( ngram.CRFAttribute_3 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueSecondMAT43( ngram.CRFAttribute_4 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueSecondMAT43( ngram.CRFAttribute_5 ); 
                    break;

                    default:
                    #region
                    {
                        for ( var j = 0; j < ngram.CRFAttributesLength; j++ )
			            {
                            var crfAttr = ngram.CRFAttributes[ j ];
                            AppendAttrValueSecondMAT43( crfAttr ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			            }
			            // Удалить последний '|'
                        _AttributeBufferPtr--;
                    }
                    #endregion
                    break;
                }
                #endregion

                #region [.retrieve-attr-values.]
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_1 );
                var f1 = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f1 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f1;
                }

                #if DEBUG
                    var attr_len = (int) (_AttributeBufferPtr - _AttributeBufferPtrBase);                        
                    fixed ( char* chars_ptr = _chars_attr_debug )
                    {
                        var chars_len = System.Text.Encoding.UTF8.GetChars( _AttributeBufferPtrBase, attr_len, chars_ptr, _chars_attr_debug.Length );
                        var s_debug = new string( chars_ptr, 0, chars_len );
                        _sb_attr_debug.Append( s_debug ).Append( '\t' );
                    }
                #endif
                #endregion
            }

            return (marginal);
        }
        private double? BrimLastMAT43()
        {
            var marginal = default(double?);

            for ( var i = 0; i < _NGramsLastLength; i++ )
            {
                var ngram = _NGramsLast[ i ];

                _AttributeBufferPtr = ngram.CopyAttributesHeaderChars( _AttributeBufferPtrBase );

                #region [.build attr-values.]
                switch ( ngram.CRFAttributesLength )
                {
                    case 6:
                        AppendAttrValueLastMAT43( ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT43( ngram.CRFAttribute_1 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT43( ngram.CRFAttribute_2 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT43( ngram.CRFAttribute_3 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT43( ngram.CRFAttribute_4 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT43( ngram.CRFAttribute_5 ); 
                    break;

                    default:
                    #region
                    {
                        for ( var j = 0; j < ngram.CRFAttributesLength; j++ )
			            {
                            var crfAttr = ngram.CRFAttributes[ j ];
                            AppendAttrValueLastMAT43( crfAttr ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			            }
			            // Удалить последний '|'
                        _AttributeBufferPtr--;
                    }
                    #endregion
                    break;
                }
                #endregion

                #region [.retrieve-attr-values.]
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_2 );
                var f2 = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f2 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f2;
                }

                #if DEBUG
                    var attr_len = (int) (_AttributeBufferPtr - _AttributeBufferPtrBase);
                    fixed ( char* chars_ptr = _chars_attr_debug )
                    {
                        var chars_len = System.Text.Encoding.UTF8.GetChars( _AttributeBufferPtrBase, attr_len, chars_ptr, _chars_attr_debug.Length );
                        var s_debug = new string( chars_ptr, 0, chars_len );
                        _sb_attr_debug.Append( s_debug ).Append( '\t' );
                    }
                #endif
                #endregion
            }

            return (marginal);
        }

        private void AppendAttrValueSecondMAT43( CRFAttribute crfAttribute )
        {
            /* crfAttribute.Position = [-1,0,1] */

            switch ( crfAttribute.Position )
            {
                case -1:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_0 );
                break;

                case 0: 
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_1 );
                break;

                case 1: 
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_2 );
                break;

                default: throw (new ArgumentException("position: " + crfAttribute.Position));
            }
        }
        private void AppendAttrValueLastMAT43  ( CRFAttribute crfAttribute )
        {
            /* crfAttribute.Position = [-2,-1,0] */

            switch ( crfAttribute.Position )
            {
                case -2:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_0 );
                break;

                case -1:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_1 );
                break;

                case  0:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_2 );
                break;

                default: throw (new ArgumentException("position: " + crfAttribute.Position));
            }
        }
        #endregion


        #region [.sent from 4-word's.]
        /// <summary>
        /// sent from 4-word's => [(0,1,2)];[(0,1,2);(-1,0,1)];[(-2,-1,0);(-1,0,1)];[(-2,-1,0)]
        /// </summary>
        private void Resolve44()
        {
            #region [.second-wma.]
            var prob = probability_t.Create();

            foreach ( var _ in IteratedOverSecondMAT44() )
            {
                //crf-tagging
                var probability = TaggingSecondMAT44();
                if ( probability.HasValue )
                {
                    #region [.choose max-probability.]
                    if ( prob.max_probability < probability.Value )
                    {
                        prob.max_morphoAmbiguityTuple_1 = _Mat_1;
                        prob.max_probability = probability.Value;
                    }
                    #endregion
                }
            }

            //set best word-morphology
            SetBestWordMorphology( prob.max_morphoAmbiguityTuple_1, _Wma_1 );
            #endregion

            #region [.prelast/three-wma.]
            prob = probability_t.Create();

            foreach ( var _ in IteratedOverPreLastMAT44() )
            {
                //crf-tagging
                var probability = TaggingPreLastMAT44();
                if ( probability.HasValue )
                {
                    #region [.choose max-probability.]
                    if ( prob.max_probability < probability.Value )
                    {
                        prob.max_morphoAmbiguityTuple_2 = _Mat_2;
                        prob.max_probability = probability.Value;
                    }
                    #endregion
                }
            }

            //set best word-morphology
            SetBestWordMorphology( prob.max_morphoAmbiguityTuple_2, _Wma_2 );
            #endregion

            #region [.last/four-wma.]
            prob = probability_t.Create();

            foreach ( var _ in IteratedOverLastMAT44() )
            {
                //crf-tagging
                var probability = TaggingLastMAT44();
                if ( probability.HasValue )
                {
                    #region [.choose max-probability.]
                    if ( prob.max_probability < probability.Value )
                    {
                        prob.max_morphoAmbiguityTuple_3 = _Mat_3;
                        prob.max_probability = probability.Value;
                    }
                    #endregion
                }
            }

            //set best word-morphology
            SetBestWordMorphology( prob.max_morphoAmbiguityTuple_3, _Wma_3 );
            #endregion
        }

        private IEnumerable< int > IteratedOverSecondMAT44()
        {
            if ( _Wma_1.IsPunctuation() )
            {
                yield break;
            }
            var mats_1 = _Wma_1.MorphoAmbiguityTuples;
            var len_1  = mats_1.Count;

            if ( 1 < len_1 )
            {
                Debug.Assert( _Wma_0.MorphoAmbiguityTuples.Count == 1, "(_Wma_0.MorphoAmbiguityTuples.Count != 1)" );

                //-must-be-1-/ var mats_0 = _Wma_0.MorphoAmbiguityTuples;
                var mats_2 = _Wma_2.MorphoAmbiguityTuples;
                var mats_3 = _Wma_3.MorphoAmbiguityTuples;

                //-must-be-1-/ var len_0 = mats_0.Count;
                var len_2 = mats_2.Count;
                var len_3 = mats_3.Count;

                //-must-be-1-/ for ( var i = 0; i < len_0; i++ )
                //{
                    //-must-be-1-/ _Mat_0 = mats_0[ i ];
                    _Mat_0 = _Wma_0.MorphoAmbiguityTuples[ 0 ];

                    for ( var j = 0; j < len_1; j++ )
                    {
                        _Mat_1 = mats_1[ j ];

                        for ( var x = 0; x < len_2; x++ )
                        {
                            _Mat_2 = mats_2[ x ];

                            for ( var z = 0; z < len_3; z++ )
                            {
                                _Mat_3 = mats_3[ z ];

                                yield return (0);
                            }
                        }
                    }
                //}
            }
        }
        private IEnumerable< int > IteratedOverPreLastMAT44()
        {
            if ( _Wma_2.IsPunctuation() )
            {
                yield break;
            }

            var mats_2 = _Wma_2.MorphoAmbiguityTuples;
            var len_2  = mats_2.Count;

            if ( 1 < len_2 )
            {
                Debug.Assert( _Wma_0.MorphoAmbiguityTuples.Count == 1, "(_Wma_0.MorphoAmbiguityTuples.Count != 1)" );
                Debug.Assert( _Wma_1.MorphoAmbiguityTuples.Count == 1, "(_Wma_1.MorphoAmbiguityTuples.Count != 1)" );

                //-must-be-1-/ var mats_0 = _Wma_0.MorphoAmbiguityTuples;
                //-must-be-1-/ var mats_1 = _Wma_1.MorphoAmbiguityTuples;                
                var mats_3 = _Wma_3.MorphoAmbiguityTuples;

                //-must-be-1-/ var len_0 = mats_0.Count;
                //-must-be-1-/ var len_1  = mats_1.Count;                
                var len_3 = mats_3.Count;

                //-must-be-1-/ for ( var i = 0; i < len_0; i++ )
                //{
                    //-must-be-1-/ _Mat_0 = mats_0[ i ];
                    _Mat_0 = _Wma_0.MorphoAmbiguityTuples[ 0 ];

                    //-must-be-1-/ for ( var j = 0; j < len_1; j++ )
                    //{
                        //-must-be-1-/ _Mat_1 = mats_1[ j ];
                        _Mat_1 = _Wma_1.MorphoAmbiguityTuples[ 0 ];

                        for ( var x = 0; x < len_2; x++ )
                        {
                            _Mat_2 = mats_2[ x ];

                            for ( var z = 0; z < len_3; z++ )
                            {
                                _Mat_3 = mats_3[ z ];

                                yield return (0);
                            }
                        }
                    //}
                //}
            }
        }
        private IEnumerable< int > IteratedOverLastMAT44()
        {
            if ( _Wma_3.IsPunctuation() )
            {
                yield break;
            }

            var mats_3 = _Wma_3.MorphoAmbiguityTuples;
            var len_3  = mats_3.Count;

            if ( 1 < len_3 )
            {
                Debug.Assert( _Wma_2.MorphoAmbiguityTuples.Count == 1, "(_Wma_2.MorphoAmbiguityTuples.Count != 1)" );
                Debug.Assert( _Wma_1.MorphoAmbiguityTuples.Count == 1, "(_Wma_1.MorphoAmbiguityTuples.Count != 1)" );

                //-must-be-1-/ var mats_2 = _Wma_2.MorphoAmbiguityTuples;
                //-must-be-1-/ var mats_1 = _Wma_1.MorphoAmbiguityTuples;                                

                //-must-be-1-/ var len_2 = mats_2.Count;
                //-must-be-1-/ var len_1 = mats_1.Count;

                //-must-be-1-/ for ( var j = 0; j < len_1; j++ )
                //{
                    //-must-be-1-/ _Mat_1 = mats_1[ j ];
                    _Mat_1 = _Wma_1.MorphoAmbiguityTuples[ 0 ];

                    //-must-be-1-/ for ( var x = 0; x < len_2; x++ )
                    //{
                        //-must-be-1-/ _Mat_2 = mats_2[ x ];
                        _Mat_2 = _Wma_2.MorphoAmbiguityTuples[ 0 ];

                        for ( var z = 0; z < len_3; z++ )
                        {
                            _Mat_3 = mats_3[ z ];

                            yield return (0);
                        }
                    //}
                //}
            }
        }

        private double? TaggingSecondMAT44()
        {
            var marginal = default(double?);

            #region [.second.]
                marginal = BrimSecondMAT44();

                #if DEBUG
                    _sb_attr_debug.Append( '\n' );
                #endif
            #endregion

            #if DEBUG
                _sb_attr_debug.Append( '\n' );
            #endif

            return (marginal);
        }
        private double? TaggingPreLastMAT44()
        {
            var marginal = default(double?);

            #region [.pre-last.]
                marginal = BrimPreLastMAT44();

                #if DEBUG
                    _sb_attr_debug.Append( '\n' );
                #endif
            #endregion

            #if DEBUG
                _sb_attr_debug.Append( '\n' );
            #endif

            return (marginal);
        }
        private double? TaggingLastMAT44()
        {
            var marginal = default(double?);

            #region [.last.]
                marginal = BrimLastMAT44();

                #region [.EOS.]
                _AttributeBufferPtr = CopyToZero( xlat_Unsafe.Inst._EndOfSentencePtrBase, _AttributeBufferPtrBase );
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_3 );
                var f3 = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f3 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f3;
                }
                #if DEBUG
                    _sb_attr_debug.Append( xlat_Unsafe.END_OF_SENTENCE ).Append( '\t' );
                #endif
                #endregion

                #if DEBUG
                    _sb_attr_debug.Append( '\n' );
                #endif
            #endregion

            #if DEBUG
                _sb_attr_debug.Append( '\n' );
            #endif

            return (marginal);
        }

        private double? BrimSecondMAT44()
        {
            var marginal = default(double?);

            for ( var i = 0; i < _NGramsSecondLength; i++ )
            {
                var ngram = _NGramsSecond[ i ];

                _AttributeBufferPtr = ngram.CopyAttributesHeaderChars( _AttributeBufferPtrBase );

                #region [.build attr-values.]
                switch ( ngram.CRFAttributesLength )
                {
                    case 6:
                        AppendAttrValueSecondMAT44( ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueSecondMAT44( ngram.CRFAttribute_1 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueSecondMAT44( ngram.CRFAttribute_2 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueSecondMAT44( ngram.CRFAttribute_3 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueSecondMAT44( ngram.CRFAttribute_4 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueSecondMAT44( ngram.CRFAttribute_5 ); 
                    break;

                    default:
                    #region
                    {
                        for ( var j = 0; j < ngram.CRFAttributesLength; j++ )
			            {
                            var crfAttr = ngram.CRFAttributes[ j ];
                            AppendAttrValueSecondMAT44( crfAttr ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			            }
			            // Удалить последний '|'
                        _AttributeBufferPtr--;
                    }
                    #endregion
                    break;
                }
                #endregion

                #region [.retrieve-attr-values.]
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_1 );
                var f1 = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f1 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f1;
                }

                #if DEBUG
                    var attr_len = (int) (_AttributeBufferPtr - _AttributeBufferPtrBase);                        
                    fixed ( char* chars_ptr = _chars_attr_debug )
                    {
                        var chars_len = System.Text.Encoding.UTF8.GetChars( _AttributeBufferPtrBase, attr_len, chars_ptr, _chars_attr_debug.Length );
                        var s_debug = new string( chars_ptr, 0, chars_len );
                        _sb_attr_debug.Append( s_debug ).Append( '\t' );
                    }
                #endif
                #endregion
            }

            return (marginal);
        }
        private double? BrimPreLastMAT44()
        {
            var marginal = default(double?);

            for ( var i = 0; i < _NGramsPreLastLength; i++ )
            {
                var ngram = _NGramsPreLast[ i ];

                _AttributeBufferPtr = ngram.CopyAttributesHeaderChars( _AttributeBufferPtrBase );

                #region [.build attr-values.]
                switch ( ngram.CRFAttributesLength )
                {
                    case 6:
                        AppendAttrValuePreLastMAT44( ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValuePreLastMAT44( ngram.CRFAttribute_1 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValuePreLastMAT44( ngram.CRFAttribute_2 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValuePreLastMAT44( ngram.CRFAttribute_3 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValuePreLastMAT44( ngram.CRFAttribute_4 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValuePreLastMAT44( ngram.CRFAttribute_5 ); 
                    break;

                    default:
                    #region
                    {
                        for ( var j = 0; j < ngram.CRFAttributesLength; j++ )
			            {
                            var crfAttr = ngram.CRFAttributes[ j ];
                            AppendAttrValuePreLastMAT44( crfAttr ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			            }
			            // Удалить последний '|'
                        _AttributeBufferPtr--;
                    }
                    #endregion
                    break;
                }
                #endregion

                #region [.retrieve-attr-values.]
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_2 );
                var f2 = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f2 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f2;
                }

                #if DEBUG
                    var attr_len = (int) (_AttributeBufferPtr - _AttributeBufferPtrBase);                        
                    fixed ( char* chars_ptr = _chars_attr_debug )
                    {
                        var chars_len = System.Text.Encoding.UTF8.GetChars( _AttributeBufferPtrBase, attr_len, chars_ptr, _chars_attr_debug.Length );
                        var s_debug = new string( chars_ptr, 0, chars_len );
                        _sb_attr_debug.Append( s_debug ).Append( '\t' );
                    }
                #endif
                #endregion
            }

            return (marginal);
        }
        private double? BrimLastMAT44()
        {
            var marginal = default(double?);

            for ( var i = 0; i < _NGramsLastLength; i++ )
            {
                var ngram = _NGramsLast[ i ];

                _AttributeBufferPtr = ngram.CopyAttributesHeaderChars( _AttributeBufferPtrBase );

                #region [.build attr-values.]
                switch ( ngram.CRFAttributesLength )
                {
                    case 6:
                        AppendAttrValueLastMAT44( ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT44( ngram.CRFAttribute_1 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT44( ngram.CRFAttribute_2 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT44( ngram.CRFAttribute_3 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT44( ngram.CRFAttribute_4 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT44( ngram.CRFAttribute_5 ); 
                    break;

                    default:
                    #region
                    {
                        for ( var j = 0; j < ngram.CRFAttributesLength; j++ )
			            {
                            var crfAttr = ngram.CRFAttributes[ j ];
                            AppendAttrValueLastMAT44( crfAttr ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			            }
			            // Удалить последний '|'
                        _AttributeBufferPtr--;
                    }
                    #endregion
                    break;
                }
                #endregion

                #region [.retrieve-attr-values.]
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_3 );
                var f3 = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f3 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f3;
                }

                #if DEBUG
                    var attr_len = (int) (_AttributeBufferPtr - _AttributeBufferPtrBase);                        
                    fixed ( char* chars_ptr = _chars_attr_debug )
                    {
                        var chars_len = System.Text.Encoding.UTF8.GetChars( _AttributeBufferPtrBase, attr_len, chars_ptr, _chars_attr_debug.Length );
                        var s_debug = new string( chars_ptr, 0, chars_len );
                        _sb_attr_debug.Append( s_debug ).Append( '\t' );
                    }
                #endif
                #endregion
            }

            return (marginal);
        }

        private void AppendAttrValueSecondMAT44 ( CRFAttribute crfAttribute )
        {
            /* crfAttribute.Position = [-1,0,1,2] */

            switch ( crfAttribute.Position )
            {
                case -1:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_0 );
                break;

                case 0: 
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_1 );
                break;

                case 1: 
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_2 );
                break;

                case 2: 
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_3 );
                break;

                default: throw (new ArgumentException("position: " + crfAttribute.Position));
            }
        }
        private void AppendAttrValuePreLastMAT44( CRFAttribute crfAttribute )
        {
            /* crfAttribute.Position = [-2,-1,0,1] */

            switch ( crfAttribute.Position )
            {
                case -2:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_0 );
                break;

                case -1:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_1 );
                break;

                case  0:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_2 );
                break;

                case  1:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_3 );
                break;

                default: throw (new ArgumentException("position: " + crfAttribute.Position));
            }
        }
        private void AppendAttrValueLastMAT44   ( CRFAttribute crfAttribute )
        {
            /* crfAttribute.Position = [-2,-1,0] */

            switch ( crfAttribute.Position )
            {
                case -2:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_1 );
                break;

                case -1:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_2 );
                break;

                case  0:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_3 );
                break;

                default: throw (new ArgumentException("position: " + crfAttribute.Position));
            }
        }
        #endregion


        #region [.sent from 5-&-more-word's.]
        /// <summary>
        /// sent from 5-&-more-word's => [(0,1,2)];[(0,1,2);(-1,0,1)];[(-2,-1,0);(-1,0,1);(0,1,2)];[(-2,-1,0);(-1,0,1)];[(-2,-1,0)]
        /// </summary>
        private void Resolve45()
        {
            #region [.second-wma.]
            var prob = probability_t.Create();

            foreach ( var _ in IteratedOverSecondMAT45() )
            {
                //crf-tagging
                var probability = TaggingSecondMAT45();
                if ( probability.HasValue )
                {
                    #region [.choose max-probability.]
                    if ( prob.max_probability < probability.Value )
                    {
                        prob.max_morphoAmbiguityTuple_1 = _Mat_1;
                        prob.max_probability = probability.Value;
                    }
                    #endregion
                }
            }

            //set best word-morphology
            SetBestWordMorphology( prob.max_morphoAmbiguityTuple_1, _Wma_1 );
            #endregion

            #region [.middle-wma.]
            foreach ( var wmaIndex in IteratedOverWMA45() )
            {
                prob = probability_t.Create();

                foreach ( var _ in IteratedOverMiddleMAT45() )
                {
                    //crf-tagging
                    var probability = TaggingMiddleMAT45();
                    if ( probability.HasValue )
                    {
                        #region [.choose max-probability.]
                        if ( prob.max_probability < probability.Value )
                        {
                            prob.max_morphoAmbiguityTuple_2 = _Mat_2;
                            prob.max_probability = probability.Value;
                        }
                        #endregion
                    }
                }

                //set best word-morphology
                SetBestWordMorphology( prob.max_morphoAmbiguityTuple_2, _Wma_2 );
            }
            #endregion

            #region [.prelast/three-wma.]
            prob = probability_t.Create();

            foreach ( var _ in IteratedOverPreLastMAT45() )
            {
                //crf-tagging
                var probability = TaggingPreLastMAT45();
                if ( probability.HasValue )
                {
                    #region [.choose max-probability.]
                    if ( prob.max_probability < probability.Value )
                    {
                        prob.max_morphoAmbiguityTuple_3 = _Mat_3;
                        prob.max_probability = probability.Value;
                    }
                    #endregion
                }
            }

            //set best word-morphology
            SetBestWordMorphology( prob.max_morphoAmbiguityTuple_3, _Wma_3 );
            #endregion

            #region [.last/four-wma.]
            prob = probability_t.Create();

            foreach ( var _ in IteratedOverLastMAT45() )
            {
                //crf-tagging
                var probability = TaggingLastMAT45();
                if ( probability.HasValue )
                {
                    #region [.choose max-probability.]
                    if ( prob.max_probability < probability.Value )
                    {
                        prob.max_morphoAmbiguityTuple_4 = _Mat_4;
                        prob.max_probability = probability.Value;
                    }
                    #endregion
                }
            }

            //set best word-morphology
            SetBestWordMorphology( prob.max_morphoAmbiguityTuple_4, _Wma_4 );
            #endregion
        }

        private IEnumerable< int > IteratedOverWMA45()
        {
            yield return (0);

            for ( int i = 5, len = _WordMorphoAmbiguitiesCount; i < len; i++ )
            {
                _Wma_0 = _Wma_1;
                _Wma_1 = _Wma_2;
                _Wma_2 = _Wma_3;
                _Wma_3 = _Wma_4;
                _Wma_4 = _WordMorphoAmbiguities[ i ];

                yield return (i - 4);
            }
        }

        private IEnumerable< int > IteratedOverSecondMAT45()
        {
            if ( _Wma_1.IsPunctuation() )
            {
                yield break;
            }
            var mats_1 = _Wma_1.MorphoAmbiguityTuples;
            var len_1  = mats_1.Count;

            if ( 1 < len_1 )
            {
                Debug.Assert( _Wma_0.MorphoAmbiguityTuples.Count == 1, "(_Wma_0.MorphoAmbiguityTuples.Count != 1)" );

                //-must-be-1-/ var mats_0 = _Wma_0.MorphoAmbiguityTuples;
                var mats_2 = _Wma_2.MorphoAmbiguityTuples;
                var mats_3 = _Wma_3.MorphoAmbiguityTuples;

                //-must-be-1-/ var len_0 = mats_0.Count;
                var len_2 = mats_2.Count;
                var len_3 = mats_3.Count;

                //-must-be-1-/ for ( var i = 0; i < len_0; i++ )
                //{
                //-must-be-1-/ _Mat_0 = mats_0[ i ];
                _Mat_0 = _Wma_0.MorphoAmbiguityTuples[ 0 ];

                for ( var j = 0; j < len_1; j++ )
                {
                    _Mat_1 = mats_1[ j ];

                    for ( var x = 0; x < len_2; x++ )
                    {
                        _Mat_2 = mats_2[ x ];

                        for ( var z = 0; z < len_3; z++ )
                        {
                            _Mat_3 = mats_3[ z ];

                            yield return (0);
                        }
                    }
                }
                //}
            }
        }
        private IEnumerable< int > IteratedOverMiddleMAT45()
        {
            if ( _Wma_2.IsPunctuation() )
            {
                yield break;
            }
            var mats_2 = _Wma_2.MorphoAmbiguityTuples;
            var len_2  = mats_2.Count;

            if ( 1 < len_2 )
            {
                Debug.Assert( _Wma_0.MorphoAmbiguityTuples.Count == 1, "(_Wma_0.MorphoAmbiguityTuples.Count != 1)" );
                Debug.Assert( _Wma_1.MorphoAmbiguityTuples.Count == 1, "(_Wma_1.MorphoAmbiguityTuples.Count != 1)" );

                //-must-be-1-/ var mats_0 = _Wma_0.MorphoAmbiguityTuples;
                //-must-be-1-/ var mats_1 = _Wma_1.MorphoAmbiguityTuples;
                var mats_3 = _Wma_3.MorphoAmbiguityTuples;
                var mats_4 = _Wma_4.MorphoAmbiguityTuples;

                //-must-be-1-/ var len_0 = mats_0.Count;
                //-must-be-1-/ var len_1 = mats_1.Count;
                var len_3 = mats_3.Count;
                var len_4 = mats_4.Count;

                //-must-be-1-/ for ( var i = 0; i < len_0; i++ )
                //{
                    //-must-be-1-/_Mat_0 = mats_0[ i ];
                    _Mat_0 = _Wma_0.MorphoAmbiguityTuples[ 0 ];

                    //-must-be-1-/ for ( var j = 0; j < len_1; j++ )
                    //{
                        //-must-be-1-/ _Mat_1 = mats_1[ j ];
                        _Mat_1 = _Wma_1.MorphoAmbiguityTuples[ 0 ];

                        for ( var x = 0; x < len_2; x++ )
                        {
                            _Mat_2 = mats_2[ x ];

                            for ( var z = 0; z < len_3; z++ )
                            {
                                _Mat_3 = mats_3[ z ];

                                for ( var w = 0; w < len_4; w++ )
                                {
                                    _Mat_4 = mats_4[ w ];

                                    yield return (0);
                                }
                            }
                        }
                    //}
                //}
            }
        }
        private IEnumerable< int > IteratedOverPreLastMAT45()
        {
            if ( _Wma_3.IsPunctuation() )
            {
                yield break;
            }

            var mats_3 = _Wma_3.MorphoAmbiguityTuples;
            var len_3  = mats_3.Count;            
            
            if ( 1 < len_3 )
            {
                Debug.Assert( _Wma_0.MorphoAmbiguityTuples.Count == 1, "(_Wma_0.MorphoAmbiguityTuples.Count != 1)" );
                Debug.Assert( _Wma_1.MorphoAmbiguityTuples.Count == 1, "(_Wma_1.MorphoAmbiguityTuples.Count != 1)" );
                Debug.Assert( _Wma_2.MorphoAmbiguityTuples.Count == 1, "(_Wma_2.MorphoAmbiguityTuples.Count != 1)" );

                //-must-be-1-/ var mats_0 = _Wma_0.MorphoAmbiguityTuples;
                //-must-be-1-/ var mats_1 = _Wma_1.MorphoAmbiguityTuples;
                //-must-be-1-/ var mats_2 = _Wma_2.MorphoAmbiguityTuples;
                var mats_4 = _Wma_4.MorphoAmbiguityTuples;

                //-must-be-1-/ var len_0 = mats_0.Count;
                //-must-be-1-/ var len_1 = mats_1.Count;
                //-must-be-1-/ var len_2 = mats_2.Count;
                var len_4 = mats_4.Count;

                //-must-be-1-/ for ( var i = 0; i < len_0; i++ )
                //{
                    //-must-be-1-/ _Mat_0 = mats_0[ i ];
                    _Mat_0 = _Wma_0.MorphoAmbiguityTuples[ 0 ];

                    //-must-be-1-/ for ( var j = 0; j < len_1; j++ )
                    //{
                        //-must-be-1-/ _Mat_1 = mats_1[ j ];
                        _Mat_1 = _Wma_1.MorphoAmbiguityTuples[ 0 ];

                        //-must-be-1-/ for ( var x = 0; x < len_2; x++ )
                        //{
                            //-must-be-1-/ _Mat_2 = mats_2[ x ];
                            _Mat_2 = _Wma_2.MorphoAmbiguityTuples[ 0 ];

                            for ( var z = 0; z < len_3; z++ )
                            {
                                _Mat_3 = mats_3[ z ];

                                for ( var w = 0; w < len_4; w++ )
                                {
                                    _Mat_4 = mats_4[ w ];

                                    yield return (0);
                                }
                            }
                        //}
                    //}
                //}
            }
        }
        private IEnumerable< int > IteratedOverLastMAT45()
        {
            if ( _Wma_4.IsPunctuation() )
            {
                yield break;
            }

            var mats_4 = _Wma_4.MorphoAmbiguityTuples;
            var len_4  = mats_4.Count;

            if ( 1 < len_4 )
            {
                Debug.Assert( _Wma_3.MorphoAmbiguityTuples.Count == 1, "(_Wma_3.MorphoAmbiguityTuples.Count != 1)" );
                Debug.Assert( _Wma_2.MorphoAmbiguityTuples.Count == 1, "(_Wma_2.MorphoAmbiguityTuples.Count != 1)" );

                //-must-be-1-/ var mats_3 = _Wma_3.MorphoAmbiguityTuples;
                //-must-be-1-/ var mats_2 = _Wma_2.MorphoAmbiguityTuples;                                

                //-must-be-1-/ var len_3 = mats_3.Count;
                //-must-be-1-/ var len_2 = mats_2.Count;

                //-must-be-1-/ for ( var j = 0; j < len_2; j++ )
                //{
                    //-must-be-1-/ _Mat_2 = mats_2[ j ];
                    _Mat_2 = _Wma_2.MorphoAmbiguityTuples[ 0 ];

                    //-must-be-1-/ for ( var x = 0; x < len_3; x++ )
                    //{
                        //-must-be-1-/ _Mat_3 = mats_3[ x ];
                        _Mat_3 = _Wma_3.MorphoAmbiguityTuples[ 0 ];

                        for ( var z = 0; z < len_4; z++ )
                        {
                            _Mat_4 = mats_4[ z ];

                            yield return (0);
                        }
                    //}
                //}
            }
        }

        private double? TaggingSecondMAT45()
        {
            var marginal = default(double?);

            #region [.second.]
                marginal = BrimSecondMAT45();

                #if DEBUG
                    _sb_attr_debug.Append( '\n' );
                #endif
            #endregion

            #if DEBUG
                _sb_attr_debug.Append( '\n' );
            #endif

            return (marginal);
        }
        private double? TaggingMiddleMAT45()
        {
            var marginal = default(double?);

            #region [.second.]
                marginal = BrimMiddleMAT45();

                #if DEBUG
                    _sb_attr_debug.Append( '\n' );
                #endif
            #endregion

            #if DEBUG
                _sb_attr_debug.Append( '\n' );
            #endif

            return (marginal);
        }
        private double? TaggingPreLastMAT45()
        {
            var marginal = default(double?);

            #region [.pre-last.]
                marginal = BrimPreLastMAT45();

                #if DEBUG
                    _sb_attr_debug.Append( '\n' );
                #endif
            #endregion

            #if DEBUG
                _sb_attr_debug.Append( '\n' );
            #endif

            return (marginal);
        }
        private double? TaggingLastMAT45()
        {
            var marginal = default(double?);

            #region [.last.]
                marginal = BrimLastMAT45();

                #region [.EOS.]
                _AttributeBufferPtr = CopyToZero( xlat_Unsafe.Inst._EndOfSentencePtrBase, _AttributeBufferPtrBase );
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_4 );
                var f4 = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f4 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f4;
                }
                #if DEBUG
                    _sb_attr_debug.Append( xlat_Unsafe.END_OF_SENTENCE ).Append( '\t' );
                #endif
                #endregion

                #if DEBUG
                    _sb_attr_debug.Append( '\n' );
                #endif
            #endregion

            #if DEBUG
                _sb_attr_debug.Append( '\n' );
            #endif

            return (marginal);
        }

        private double? BrimSecondMAT45()
        {
            var marginal = default(double?);

            for ( var i = 0; i < _NGramsSecondLength; i++ )
            {
                var ngram = _NGramsSecond[ i ];

                _AttributeBufferPtr = ngram.CopyAttributesHeaderChars( _AttributeBufferPtrBase );

                #region [.build attr-values.]
                switch ( ngram.CRFAttributesLength )
                {
                    case 6:
                        AppendAttrValueSecondMAT45( ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueSecondMAT45( ngram.CRFAttribute_1 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueSecondMAT45( ngram.CRFAttribute_2 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueSecondMAT45( ngram.CRFAttribute_3 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueSecondMAT45( ngram.CRFAttribute_4 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueSecondMAT45( ngram.CRFAttribute_5 ); 
                    break;

                    default:
                    #region
                    {
                        for ( var j = 0; j < ngram.CRFAttributesLength; j++ )
			            {
                            var crfAttr = ngram.CRFAttributes[ j ];
                            AppendAttrValueSecondMAT45( crfAttr ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			            }
			            // Удалить последний '|'
                        _AttributeBufferPtr--;
                    }
                    #endregion
                    break;
                }
                #endregion

                #region [.retrieve-attr-values.]
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_1 );
                var f1 = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f1 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f1;
                }

                #if DEBUG
                    var attr_len = (int) (_AttributeBufferPtr - _AttributeBufferPtrBase);                        
                    fixed ( char* chars_ptr = _chars_attr_debug )
                    {
                        var chars_len = System.Text.Encoding.UTF8.GetChars( _AttributeBufferPtrBase, attr_len, chars_ptr, _chars_attr_debug.Length );
                        var s_debug = new string( chars_ptr, 0, chars_len );
                        _sb_attr_debug.Append( s_debug ).Append( '\t' );
                    }
                #endif
                #endregion
            }

            return (marginal);
        }
        private double? BrimMiddleMAT45()
        {
            var marginal = default(double?);

            for ( var i = 0; i < _NGramsMiddleLength; i++ )
            {
                var ngram = _NGramsMiddle[ i ];

                _AttributeBufferPtr = ngram.CopyAttributesHeaderChars( _AttributeBufferPtrBase );

                #region [.build attr-values.]
                switch ( ngram.CRFAttributesLength )
                {
                    case 6:
                        AppendAttrValueMiddleMAT45( ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueMiddleMAT45( ngram.CRFAttribute_1 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueMiddleMAT45( ngram.CRFAttribute_2 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueMiddleMAT45( ngram.CRFAttribute_3 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueMiddleMAT45( ngram.CRFAttribute_4 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueMiddleMAT45( ngram.CRFAttribute_5 ); 
                    break;

                    default:
                    #region
                    {
                        for ( var j = 0; j < ngram.CRFAttributesLength; j++ )
			            {
                            var crfAttr = ngram.CRFAttributes[ j ];
                            AppendAttrValueMiddleMAT45( crfAttr ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			            }
			            // Удалить последний '|'
                        _AttributeBufferPtr--;
                    }
                    #endregion
                    break;
                }
                #endregion

                #region [.retrieve-attr-values.]
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_2 );
                var f2 = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f2 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f2;
                }

                #if DEBUG
                    var attr_len = (int) (_AttributeBufferPtr - _AttributeBufferPtrBase);                        
                    fixed ( char* chars_ptr = _chars_attr_debug )
                    {
                        var chars_len = System.Text.Encoding.UTF8.GetChars( _AttributeBufferPtrBase, attr_len, chars_ptr, _chars_attr_debug.Length );
                        var s_debug = new string( chars_ptr, 0, chars_len );
                        _sb_attr_debug.Append( s_debug ).Append( '\t' );
                    }
                #endif
                #endregion
            }

            return (marginal);
        }
        private double? BrimPreLastMAT45()
        {
            var marginal = default(double?);

            for ( var i = 0; i < _NGramsPreLastLength; i++ )
            {
                var ngram = _NGramsPreLast[ i ];

                _AttributeBufferPtr = ngram.CopyAttributesHeaderChars( _AttributeBufferPtrBase );

                #region [.build attr-values.]
                switch ( ngram.CRFAttributesLength )
                {
                    case 6:
                        AppendAttrValuePreLastMAT45( ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValuePreLastMAT45( ngram.CRFAttribute_1 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValuePreLastMAT45( ngram.CRFAttribute_2 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValuePreLastMAT45( ngram.CRFAttribute_3 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValuePreLastMAT45( ngram.CRFAttribute_4 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValuePreLastMAT45( ngram.CRFAttribute_5 ); 
                    break;

                    default:
                    #region
                    {
                        for ( var j = 0; j < ngram.CRFAttributesLength; j++ )
			            {
                            var crfAttr = ngram.CRFAttributes[ j ];
                            AppendAttrValuePreLastMAT45( crfAttr ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			            }
			            // Удалить последний '|'
                        _AttributeBufferPtr--;
                    }
                    #endregion
                    break;
                }
                #endregion

                #region [.retrieve-attr-values.]
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_3 );
                var f3 = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f3 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f3;
                }

                #if DEBUG
                    var attr_len = (int) (_AttributeBufferPtr - _AttributeBufferPtrBase);                        
                    fixed ( char* chars_ptr = _chars_attr_debug )
                    {
                        var chars_len = System.Text.Encoding.UTF8.GetChars( _AttributeBufferPtrBase, attr_len, chars_ptr, _chars_attr_debug.Length );
                        var s_debug = new string( chars_ptr, 0, chars_len );
                        _sb_attr_debug.Append( s_debug ).Append( '\t' );
                    }
                #endif
                #endregion
            }

            return (marginal);
        }
        private double? BrimLastMAT45()
        {
            var marginal = default(double?);

            for ( var i = 0; i < _NGramsLastLength; i++ )
            {
                var ngram = _NGramsLast[ i ];

                _AttributeBufferPtr = ngram.CopyAttributesHeaderChars( _AttributeBufferPtrBase );

                #region [.build attr-values.]
                switch ( ngram.CRFAttributesLength )
                {
                    case 6:
                        AppendAttrValueLastMAT45( ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT45( ngram.CRFAttribute_1 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT45( ngram.CRFAttribute_2 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT45( ngram.CRFAttribute_3 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT45( ngram.CRFAttribute_4 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                        AppendAttrValueLastMAT45( ngram.CRFAttribute_5 ); 
                    break;

                    default:
                    #region
                    {
                        for ( var j = 0; j < ngram.CRFAttributesLength; j++ )
			            {
                            var crfAttr = ngram.CRFAttributes[ j ];
                            AppendAttrValueLastMAT45( crfAttr ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			            }
			            // Удалить последний '|'
                        _AttributeBufferPtr--;
                    }
                    #endregion
                    break;
                }
                #endregion

                #region [.retrieve-attr-values.]
                FillMat5CharsBufferWithZero( _AttributeBufferPtr, _Mat_4 );
                var f4 = default(float);
                if ( _ModelDictionaryBytes.TryGetValue( new IntPtr( _AttributeBufferPtrBase ), out f4 ) )
                {
                    marginal = marginal.GetValueOrDefault() + f4;
                }

                #if DEBUG
                    var attr_len = (int) (_AttributeBufferPtr - _AttributeBufferPtrBase);                        
                    fixed ( char* chars_ptr = _chars_attr_debug )
                    {
                        var chars_len = System.Text.Encoding.UTF8.GetChars( _AttributeBufferPtrBase, attr_len, chars_ptr, _chars_attr_debug.Length );
                        var s_debug = new string( chars_ptr, 0, chars_len );
                        _sb_attr_debug.Append( s_debug ).Append( '\t' );
                    }
                #endif
                #endregion
            }

            return (marginal);
        }

        private void AppendAttrValueSecondMAT45 ( CRFAttribute crfAttribute )
        {
            /* crfAttribute.Position = [-1,0,1,2] */

            switch ( crfAttribute.Position )
            {
                case -1:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_0 );
                break;

                case 0: 
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_1 );
                break;

                case 1: 
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_2 );
                break;

                case 2: 
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_3 );
                break;

                default: throw (new ArgumentException("position: " + crfAttribute.Position));
            }
        }
        private void AppendAttrValueMiddleMAT45 ( CRFAttribute crfAttribute )
        {
            /* crfAttribute.Position = [-2,-1,0,1,2] */

            switch ( crfAttribute.Position )
            {
                case -2:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_0 );
                break;

                case -1:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_1 );
                break;

                case  0:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_2 );
                break;

                case  1:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_3 );
                break;

                case  2:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_4 );
                break;

                default: throw (new ArgumentException("position: " + crfAttribute.Position));
            }
        }
        private void AppendAttrValuePreLastMAT45( CRFAttribute crfAttribute )
        {
            /* crfAttribute.Position = [-2,-1,0,1] */

            switch ( crfAttribute.Position )
            {
                case -2:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_1 );
                break;

                case -1:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_2 );
                break;

                case  0:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_3 );
                break;

                case  1:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_4 );
                break;

                default: throw (new ArgumentException("position: " + crfAttribute.Position));
            }
        }
        private void AppendAttrValueLastMAT45   ( CRFAttribute crfAttribute )
        {
            /* crfAttribute.Position = [-2,-1,0] */

            switch ( crfAttribute.Position )
            {
                case -2:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_2 );
                break;

                case -1:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_3 );
                break;

                case  0:
                    *(_AttributeBufferPtr++) = GetAttrValue( crfAttribute.ColumnIndex, _Mat_4 );
                break;

                default: throw (new ArgumentException("position: " + crfAttribute.Position));
            }
        }
        #endregion


        unsafe private static byte* CopyToZero( byte* src, byte* dist )
        {
            for ( ; ; dist++, src++ )
            {
                var ch = *src;
                if ( ch == ZERO )
                    return (dist);

                *dist = *src;
            }
        }
        unsafe private static void FillMat5CharsBufferWithZero( byte* ptr, MorphoAmbiguityTuple_t mat )
        {
            *(ptr++) = SEMICOLON;
            *(ptr++) = MA.get_CRF_W_field_value( mat );
            *(ptr++) = MA.get_CRF_A_field_value( mat );
            *(ptr++) = MA.get_CRF_B_field_value( mat );
            *(ptr++) = MA.get_CRF_C_field_value( mat );
            *(ptr++) = MA.get_CRF_D_field_value( mat );
            *(ptr  ) = ZERO;
        }

        private static byte GetAttrValue( int columnIndex, MorphoAmbiguityTuple_t mat )
        {
            switch ( columnIndex )
            {
                //w – part-of-speech
                case 0:
                    return (MA.get_CRF_W_field_value( mat ));

                //a - Person
                case 1:
                    return (MA.get_CRF_A_field_value( mat ));

                //b - Case
                case 2:
                    return (MA.get_CRF_B_field_value( mat ));

                //c - Number
                case 3:
                    return (MA.get_CRF_C_field_value( mat ));

                //d - Gender
                case 4:
                    return (MA.get_CRF_D_field_value( mat ));

                //y – искомое значение
                case 5:
                    return (MA.get_CRF_Y_field_value());

                default: throw (new ArgumentException("columnIndex: " + columnIndex));
            }
        }

        private static void SetBestWordMorphology( MorphoAmbiguityTuple_t max_morphoAmbiguityTuple, WordMorphoAmbiguity_t wma )
        {
            if ( max_morphoAmbiguityTuple != default(MorphoAmbiguityTuple_t) )
            {
                max_morphoAmbiguityTuple.Word.morphology = max_morphoAmbiguityTuple.WordFormMorphology;
                wma.SetWordMorphologyAsDefined();
            }
            else
            {
                wma.SetWordMorphologyAsUndefined();
            }
        }
    }

}
