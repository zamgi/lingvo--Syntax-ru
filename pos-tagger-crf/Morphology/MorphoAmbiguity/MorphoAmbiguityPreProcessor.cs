using System.Collections.Generic;
using System.Diagnostics;

using lingvo.morphology;
using M = System.Runtime.CompilerServices.MethodImplAttribute;
using O = System.Runtime.CompilerServices.MethodImplOptions;

namespace lingvo.postagger
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class MorphoAmbiguityPreProcessor
    {
        #region [.statics.]
        private const MorphoAttributeEnum MorphoAttributeAllCases = 
            MorphoAttributeEnum.Nominative    |
            MorphoAttributeEnum.Genitive      |
            MorphoAttributeEnum.Dative        |
            MorphoAttributeEnum.Accusative    |
            MorphoAttributeEnum.Instrumental  |
            MorphoAttributeEnum.Prepositional |
            MorphoAttributeEnum.Locative      |
            MorphoAttributeEnum.Anycase;
        private const MorphoAttributeEnum MorphoAttributeAllNumber =
            MorphoAttributeEnum.Singular |
            MorphoAttributeEnum.Plural;
        private const MorphoAttributeEnum MorphoAttributeAllGender =
            MorphoAttributeEnum.Feminine  |
            MorphoAttributeEnum.Masculine |
            MorphoAttributeEnum.Neuter    |
            MorphoAttributeEnum.General;

        [M(O.AggressiveInlining)] private static bool IsCaseAnycase( MorphoAttributeEnum morphoAttribute ) => ((morphoAttribute & MorphoAttributeEnum.Anycase) == MorphoAttributeEnum.Anycase);
        [M(O.AggressiveInlining)] private static bool IsGenderGeneral( MorphoAttributeEnum morphoAttribute ) => ((morphoAttribute & MorphoAttributeEnum.General) == MorphoAttributeEnum.General);
        #endregion

        #region [.ctor().]
        private /*readonly*/ List< MorphoAmbiguityTuple_t > _Mats_0;
        private /*readonly*/ List< MorphoAmbiguityTuple_t > _Mats_1;

        private /*readonly*/ HashSet< MorphoAmbiguityTuple_t > _Mats_0_stepII;
        private /*readonly*/ HashSet< MorphoAmbiguityTuple_t > _Mats_1_stepII;
        private /*readonly*/ HashSet< MorphoAmbiguityTuple_t > _Mats_2_stepII;

        public MorphoAmbiguityPreProcessor()
        {
            _Mats_0 = new List< MorphoAmbiguityTuple_t >();
            _Mats_1 = new List< MorphoAmbiguityTuple_t >();

            _Mats_0_stepII = new HashSet< MorphoAmbiguityTuple_t >();
            _Mats_1_stepII = new HashSet< MorphoAmbiguityTuple_t >();
            _Mats_2_stepII = new HashSet< MorphoAmbiguityTuple_t >();
        }
        #endregion

        #region [.description.]
        /*
        [-Case_1-]
        Сочетания вида:
        1.
         1.1
        - Preposition + PossessivePronoun + Noun
        - Preposition + AdjectivePronoun  + Noun
        - Preposition + Adjective         + Noun
ПРИМЕЧАНИЕ. a) Для цепочек:
- Preposition + PossessivePronoun + Noun
- Preposition + AdjectivePronoun  + Noun
Если первое слагаемое не согласуется со вторым, то первое слагаемое согласовать с третьим, а второе доопределять моделью.
Примеры: в его фильме;
b) Для цепочки:
- Preposition + Adjective + Noun
у второго (или третьего) слагаемого есть падеж Anycase (или General) или Unknown (отсутствует), то заменяем это значение на падежное согласование первого с третьим (или первого со вторым соответственно).
Примеры: в последнем интервью; в постмодернистских очках; по водному поло; с эксклюзивным товаром; на собственном Ferrari         
        
         1.2
        - Preposition + Noun
        - Preposition + Pronoun
        - Preposition + Numeral
        - Preposition + PossessivePronoun
        - Preposition + Adjective
        - Preposition + AdjectivePronoun
            - идущие подряд, всегда согласуются по падежу (case). 
              Т.образом, можно свести к минимуму кол-во вариаций, выбирая только те, у которых падежи совпадают.
ПРИМЕЧАНИЕ. a) Для цепочек:
- Preposition + Noun
- Preposition + Pronoun
- Preposition + Numeral
- Preposition + Adjective
Если во втором слагаемом  есть падеж Anycase (или General) или Unknown (отсутствует), то заменяем это значение падеж первого слагаемого.
Примеры: в интервью; на iPhone; на 23;         

        [-Case_2-]
        Сочетания вида:
        2.
        - PossessivePronoun + Noun 
        - Adjective         + Noun
        - Adjective         + Adjective
        - AdjectivePronoun  + Noun
        - AdjectivePronoun  + Pronoun
        - AdjectivePronoun  + AdjectivePronoun
        - AdjectivePronoun  + Adjective
        - Numeral           + Noun
            - идущие подряд, всегда согласуются по падежу (case), числу (number) и роду (gender). 
            Т.образом, можно свести к минимуму кол-во вариаций, выбирая только те, 
            1) у которых падежи совпадают (case); 
            2) если вариаций более одной, то проверять число (number); 
            3) если вариаций более одной, то проверять род (gender). 
            Предобработку проводить циклично до тех пор, пока ни один из вариантов не сработает.
ПРИМЕЧАНИЕ. a) Для цепочек:
- Adjective + Noun
- Adjective + Adjective
- Numeral   + Noun
у первого (или второго) слагаемого есть падеж Anycase или Unknown (отсутствует), то заменяем это значение на падеж второго (или соответственно первого).
Примеры: последнее интервью; новое Ferrari; последний эксклюзивный         
        */

        /*
        - Preposition + PossessivePronoun + Noun
        По его данным, на Крещение в Москве подготовят  хуйню
        Указ о ее создании был подписан   хуй знает кем
        
        - Preposition + AdjectivePronoun + Noun
        чтобы ему дали	дать домой хотя бы на несколько дней
        В эти минуты твориться какая-то хуйня
        
        - Preposition + Adjective + Noun
        Чудеса начинаются почти сразу же со случайных прохожих



        - Preposition + Noun
        Вася пришел к бабушке

        - Preposition + PossessivePronoun
        Вася пришел к своим друзьям        

        - Preposition + Pronoun
        Вася пришел к нему

        - Preposition + Numeral
        Вася пришел к одному из друзей
        :
        - Adjective + Noun
        Вася вылил вкусного чая

        - AdjectivePronoun + Noun
        Все люди хотят в магазин

        - AdjectivePronoun + Pronoun
        Все мы работали хорошо 

        - PossessivePronoun + Adjective
        Вася позвал вашего старого друга

        - Adjective + Adjective
        московской районной больнице требуется кровь

        - Numeral + Noun
        одному Васе тут не справиться       
        */
        #endregion

        public void Run( List< WordMorphoAmbiguity_t > wordMorphoAmbiguities )
        {
            var len = wordMorphoAmbiguities.Count;
            #region [.len < 2.]
            if ( len < 2 )
            {
                return;
            }
            #endregion

            #region [.len == 2.]
            var wma_0 = wordMorphoAmbiguities[ 0 ];
            var wma_1 = wordMorphoAmbiguities[ 1 ];
            if ( len == 2 )
            {
                Run( wma_0, wma_1 );

                return;
            }
            #endregion

            #region [.2 < len.]
            var wma_2 = wordMorphoAmbiguities[ 2 ];
            for ( var wasModify = false; ; wasModify = false )
            {
                for ( var i = 3; ; i++ )
                {
                    #region [.switch by {posTaggerOutputType} Case_1__1 & Case_1__2 & Case_2.]
                    switch ( wma_0.Word.posTaggerOutputType )
                    {
                        #region [.Case_1__1 & Case_1__2.]
                        //Preposition: {+ PossessivePronoun, + AdjectivePronoun, + Adjective} + Noun
                        //Preposition: {+ Noun, + Pronoun, + Numeral, + PossessivePronoun, + Adjective, + AdjectivePronoun}
                        case PosTaggerOutputType.Preposition:
                            var Case_1__1_wasMatch = false;

                            #region [.Case_1__1.]
                            if ( wma_2.Word.posTaggerOutputType == PosTaggerOutputType.Noun )
                            {
                                switch ( wma_1.Word.posTaggerOutputType )
                                {
                                    case PosTaggerOutputType.PossessivePronoun:
                                    case PosTaggerOutputType.AdjectivePronoun:
                                    case PosTaggerOutputType.Adjective:
                                        Case_1__1_wasMatch = Case_1__1( wma_0, wma_1, wma_2 );
                                    break;
                                }
                            }
                            #endregion

                            #region [.Case_1__2.]
                            if ( !Case_1__1_wasMatch )
                            {
                                switch ( wma_1.Word.posTaggerOutputType )
                                {
                                    case PosTaggerOutputType.Noun:
                                    case PosTaggerOutputType.Pronoun:
                                    case PosTaggerOutputType.Numeral:
                                    case PosTaggerOutputType.PossessivePronoun:
                                    case PosTaggerOutputType.Adjective:
                                    case PosTaggerOutputType.AdjectivePronoun:
                                        Case_1__2( wma_0, wma_1 );
                                    break;
                                }
                            }
                            #endregion
                        break;
                        #endregion

                        #region [.Case_2.]
                        //PossessivePronoun + Noun 
                        case PosTaggerOutputType.PossessivePronoun:
                            #region
                            if ( wma_1.Word.posTaggerOutputType == PosTaggerOutputType.Pronoun )
                            {
                                wasModify |= Case_2( wma_0, wma_1 );
                            }
                            #endregion
                        break;

                        //Adjective: {+ Noun, + Adjective}
                        case PosTaggerOutputType.Adjective:
                            #region
                            switch ( wma_1.Word.posTaggerOutputType )
                            {
                                case PosTaggerOutputType.Noun:
                                case PosTaggerOutputType.Adjective:
                                    wasModify |= Case_2( wma_0, wma_1 );
                                break;
                            }
                            #endregion
                        break;

                        //AdjectivePronoun: {+ Noun, + Pronoun, + AdjectivePronoun, + Adjective}
                        case PosTaggerOutputType.AdjectivePronoun:
                            #region
                            switch ( wma_1.Word.posTaggerOutputType )
                            {
                                case PosTaggerOutputType.Noun:
                                case PosTaggerOutputType.Pronoun:
                                case PosTaggerOutputType.AdjectivePronoun:
                                case PosTaggerOutputType.Adjective:
                                    wasModify |= Case_2( wma_0, wma_1 );
                                break;
                            }
                            #endregion
                        break;

                        //Numeral + Noun
                        case PosTaggerOutputType.Numeral:
                            #region
                            if ( wma_1.Word.posTaggerOutputType == PosTaggerOutputType.Noun )
                            {
                                wasModify |= Case_2( wma_0, wma_1 );
                            }
                            #endregion
                        break;
                        #endregion
                    }
                    #endregion

                    if ( len <= i )
                    {
                        break;
                    }

                    wma_0 = wma_1;
                    wma_1 = wma_2;
                    wma_2 = wordMorphoAmbiguities[ i ];
                }

                if ( !wasModify )
                {
                    break;
                }
            }

            Run( wma_1, wma_2 );

            #endregion

            #region comm.
            /*
            #region [.Case_1__1.]
            var wma_0 = wordMorphoAmbiguities[ 0 ];
            var wma_1 = wordMorphoAmbiguities[ 1 ];
            if ( 3 <= len )
            {
                var wma_2 = wordMorphoAmbiguities[ 2 ];
                for ( var i = 3; ; i++ )
                {
                    #region [.switch by {posTaggerOutputType}.]
                    switch ( wma_0.Word.posTaggerOutputType )
                    {
                        //Preposition: {+ PossessivePronoun, + AdjectivePronoun, + Adjective} + Noun
                        case PosTaggerOutputType.Preposition:
                            #region
                            if ( wma_2.Word.posTaggerOutputType == PosTaggerOutputType.Noun )
                            {
                                switch ( wma_1.Word.posTaggerOutputType )
                                {
                                    case PosTaggerOutputType.PossessivePronoun:
                                    case PosTaggerOutputType.AdjectivePronoun:
                                    case PosTaggerOutputType.Adjective:
                                        Case_1__1( wma_0, wma_1, wma_2 );
                                    break;
                                }
                            }
                            #endregion
                        break;
                    }
                    #endregion

                    if ( len <= i )
                    {
                        break;
                    }

                    wma_0 = wma_1;
                    wma_1 = wma_2;
                    wma_2 = wordMorphoAmbiguities[ i ];
                }

                //reset to begin for Case_1.2
                wma_0 = wordMorphoAmbiguities[ 0 ];
                wma_1 = wordMorphoAmbiguities[ 1 ];
            }
            #endregion

            #region [.Case_1__2.]
            for ( var i = 2; ; i++ )
            {
                #region [.switch by {posTaggerOutputType}.]
                switch ( wma_0.Word.posTaggerOutputType )
                {
                    //Preposition: {+ Noun, + Pronoun, + Numeral, + PossessivePronoun, + Adjective, + AdjectivePronoun}
                    case PosTaggerOutputType.Preposition:
                        #region
                        switch ( wma_1.Word.posTaggerOutputType )
                        {
                            case PosTaggerOutputType.Noun:
                            case PosTaggerOutputType.Pronoun:
                            case PosTaggerOutputType.Numeral:
                            case PosTaggerOutputType.PossessivePronoun:
                            case PosTaggerOutputType.Adjective:
                            case PosTaggerOutputType.AdjectivePronoun:
                                Case_1__2( wma_0, wma_1 );
                            break;
                        }
                        #endregion
                    break;
                }
                #endregion

                if ( len <= i )
                {
                    break;
                }

                wma_0 = wma_1;
                wma_1 = wordMorphoAmbiguities[ i ];
            }
            #endregion

            #region [.Case_2.]
            wma_0 = wordMorphoAmbiguities[ 0 ];
            wma_1 = wordMorphoAmbiguities[ 1 ];
            for ( var wasModify = false; ; wasModify = false )
            {
                for ( var i = 2; ; i++ )
                {
                    #region [.switch by {posTaggerOutputType}.]
                    switch ( wma_0.Word.posTaggerOutputType )
                    {
                        //PossessivePronoun + Noun 
                        case PosTaggerOutputType.PossessivePronoun:
                            #region
                            if ( wma_1.Word.posTaggerOutputType == PosTaggerOutputType.Pronoun )
                            {
                                wasModify |= Case_2( wma_0, wma_1 );
                            }
                            #endregion
                        break;

                        //Adjective: {+ Noun, + Adjective}
                        case PosTaggerOutputType.Adjective:
                            #region
                            switch ( wma_1.Word.posTaggerOutputType )
                            {
                                case PosTaggerOutputType.Noun:
                                case PosTaggerOutputType.Adjective:
                                    wasModify |= Case_2( wma_0, wma_1 );
                                break;
                            }
                            #endregion
                        break;

                        //AdjectivePronoun: {+ Noun, + Pronoun, + AdjectivePronoun, + Adjective}
                        case PosTaggerOutputType.AdjectivePronoun:
                            #region
                            switch ( wma_1.Word.posTaggerOutputType )
                            {
                                case PosTaggerOutputType.Noun:
                                case PosTaggerOutputType.Pronoun:
                                case PosTaggerOutputType.AdjectivePronoun:
                                case PosTaggerOutputType.Adjective:
                                    wasModify |= Case_2( wma_0, wma_1 );
                                break;
                            }
                            #endregion
                        break;

                        //Numeral + Noun
                        case PosTaggerOutputType.Numeral:
                            #region
                            if ( wma_1.Word.posTaggerOutputType == PosTaggerOutputType.Noun )
                            {
                                wasModify |= Case_2( wma_0, wma_1 );
                            }
                            #endregion
                        break;
                    }
                    #endregion

                    if ( len <= i )
                    {
                        break;
                    }

                    wma_0 = wma_1;
                    wma_1 = wordMorphoAmbiguities[ i ];
                }

                if ( !wasModify )
                {
                    break;
                }
            }
            #endregion
            */
            #endregion
        }

        private void Run( WordMorphoAmbiguity_t wma_0, WordMorphoAmbiguity_t wma_1 )
        {
            switch ( wma_0.Word.posTaggerOutputType )
            {
                #region [.Case_1__2.]
                //Preposition: {+ Noun, + Pronoun, + Numeral, + PossessivePronoun, + Adjective, + AdjectivePronoun}
                case PosTaggerOutputType.Preposition:
                    #region
                    switch ( wma_1.Word.posTaggerOutputType )
                    {
                        case PosTaggerOutputType.Noun:
                        case PosTaggerOutputType.Pronoun:
                        case PosTaggerOutputType.Numeral:
                        case PosTaggerOutputType.PossessivePronoun:
                        case PosTaggerOutputType.Adjective:
                        case PosTaggerOutputType.AdjectivePronoun:
                            Case_1__2( wma_0, wma_1 );
                        break;
                    }
                    #endregion
                break;
                #endregion

                #region [.Case_2.]
                //PossessivePronoun + Noun 
                case PosTaggerOutputType.PossessivePronoun:
                    #region
                    if ( wma_1.Word.posTaggerOutputType == PosTaggerOutputType.Pronoun )
                    {
                        Case_2( wma_0, wma_1 );
                    }
                    #endregion
                break;

                //Adjective: {+ Noun, + Adjective}
                case PosTaggerOutputType.Adjective:
                    #region
                    switch ( wma_1.Word.posTaggerOutputType )
                    {
                        case PosTaggerOutputType.Noun:
                        case PosTaggerOutputType.Adjective:
                            Case_2( wma_0, wma_1 );
                        break;
                    }
                    #endregion
                break;

                //AdjectivePronoun: {+ Noun, + Pronoun, + AdjectivePronoun, + Adjective}
                case PosTaggerOutputType.AdjectivePronoun:
                    #region
                    switch ( wma_1.Word.posTaggerOutputType )
                    {
                        case PosTaggerOutputType.Noun:
                        case PosTaggerOutputType.Pronoun:
                        case PosTaggerOutputType.AdjectivePronoun:
                        case PosTaggerOutputType.Adjective:
                            Case_2( wma_0, wma_1 );
                        break;
                    }
                    #endregion
                break;

                //Numeral + Noun
                case PosTaggerOutputType.Numeral:
                    #region
                    if ( wma_1.Word.posTaggerOutputType == PosTaggerOutputType.Noun )
                    {
                        Case_2( wma_0, wma_1 );
                    }
                    #endregion
                break;
                #endregion
            }
        }

        private bool Case_1__1( WordMorphoAmbiguity_t wma_0, WordMorphoAmbiguity_t wma_1, WordMorphoAmbiguity_t wma_2 )
        {
            var len_0 = wma_0.MorphoAmbiguityTuples.Count;
            var len_1 = wma_1.MorphoAmbiguityTuples.Count;

            if ( len_0 == 1 )
            {
                #region [.1 == mats_0 & 1 < mats_1.]
                if ( 1 < len_1 )
                {
                    var ma_case_0 = (wma_0.MorphoAmbiguityTuples[ 0 ].WordFormMorphology.MorphoAttribute & MorphoAttributeAllCases);
                    if ( !IsCaseAnycase( ma_case_0 ) )
                    {
                        for ( var j = 0; j < len_1; j++ )
                        {
                            var mat_1 = wma_1.MorphoAmbiguityTuples[ j ];
                            var ma_1  = mat_1.WordFormMorphology.MorphoAttribute;
                            if ( ma_case_0 == (ma_1 & MorphoAttributeAllCases) )
                            {
                                _Mats_1.Add( mat_1 );
                            }
                        }

                        if ( _Mats_1.Count != 0 )
                        {
                            var wasModify = (_Mats_1.Count < len_1);
                            if ( wasModify )
                            {
                                wasModify = Case_1__1_stepII_1( _Mats_1, wma_2 );
                                if ( wasModify )
                                {
                                    wma_1.MorphoAmbiguityTuples.Clear();
                                    wma_1.MorphoAmbiguityTuples.AddRange( _Mats_1 );
                                }
                            }
                            _Mats_1.Clear();

                            return (wasModify);
                        }
                    }
                }
                #endregion
            }
            else if ( len_1 == 1 )
            {
                #region [.1 < mats_0 & 1 == mats_1.]
                var ma_case_1 = (wma_1.MorphoAmbiguityTuples[ 0 ].WordFormMorphology.MorphoAttribute & MorphoAttributeAllCases);
                if ( !IsCaseAnycase( ma_case_1 ) )
                {
                    for ( var i = 0; i < len_0; i++ )
                    {
                        var mat_0 = wma_0.MorphoAmbiguityTuples[ i ];
                        var ma_0  = mat_0.WordFormMorphology.MorphoAttribute;
                        if ( ma_case_1 == (ma_0 & MorphoAttributeAllCases) )
                        {
                            _Mats_0.Add( mat_0 );
                        }
                    }

                    if ( _Mats_0.Count != 0 )
                    {
                        var wasModify = (_Mats_0.Count < len_0);
                        if ( wasModify )
                        {
                            wasModify = Case_1__1_stepII_1( _Mats_0, wma_2 );
                            if ( wasModify )
                            {
                                wma_0.MorphoAmbiguityTuples.Clear();
                                wma_0.MorphoAmbiguityTuples.AddRange( _Mats_0 );
                            }
                        }
                        _Mats_0.Clear();

                        return (wasModify);
                    }
                }
                #endregion
            }
            else
            {
                #region [.1 < mats_0 & 1 < mats_1.]
                for ( var i = 0; i < len_0; i++ )
                {
                    var mat_0 = wma_0.MorphoAmbiguityTuples[ i ];
                    var ma_case_0 = (mat_0.WordFormMorphology.MorphoAttribute & MorphoAttributeAllCases);
                    if ( !IsCaseAnycase( ma_case_0 ) )
                    {
                        for ( var j = 0; j < len_1; j++ )
                        {
                            var mat_1 = wma_1.MorphoAmbiguityTuples[ j ];
                            var ma_1  = mat_1.WordFormMorphology.MorphoAttribute;
                            if ( ma_case_0 == (ma_1 & MorphoAttributeAllCases) )
                            {
                                _Mats_0.AddIfNotExists( mat_0 );
                                _Mats_1.AddIfNotExists( mat_1 );
                            }
                        }
                    }
                }

                if ( _Mats_0.Count != 0 /*&& _Mats_1.Count != 0*/ )
                {
                    var wasModify = ((_Mats_0.Count < len_0) || (_Mats_1.Count < len_1));
                    if ( wasModify )
                    {
                        wasModify = Case_1__1_stepII_2( wma_2 );
                        if ( wasModify )
                        {
                            wma_0.MorphoAmbiguityTuples.Clear();
                            wma_0.MorphoAmbiguityTuples.AddRange( _Mats_0 );

                            wma_1.MorphoAmbiguityTuples.Clear();
                            wma_1.MorphoAmbiguityTuples.AddRange( _Mats_1 );                        
                        }
                    }
                    _Mats_0.Clear();
                    _Mats_1.Clear();

                    return (wasModify);
                }
                #endregion
            }

            return (false);
        }
        private bool Case_1__1_stepII_1( List< MorphoAmbiguityTuple_t > mats, WordMorphoAmbiguity_t wma_2 )
        {
            var len_0 = mats.Count;
            var len_2 = wma_2.MorphoAmbiguityTuples.Count;

            if ( len_0 == 1 )
            {
                #region [.1 == mats & 1 < wma.MorphoAmbiguityTuples.]
                if ( 1 < len_2 )
                {
                    var ma_case_0 = (mats[ 0 ].WordFormMorphology.MorphoAttribute & MorphoAttributeAllCases);
                    if ( !IsCaseAnycase( ma_case_0 ) )
                    {
                        for ( var j = 0; j < len_2; j++ )
                        {
                            var mat_2 = wma_2.MorphoAmbiguityTuples[ j ];
                            var ma_2  = mat_2.WordFormMorphology.MorphoAttribute;
                            if ( ma_case_0 == (ma_2 & MorphoAttributeAllCases) )
                            {
                                _Mats_2_stepII.Add( mat_2 );
                            }
                        }

                        if ( _Mats_2_stepII.Count != 0 )
                        {
                            var wasModify = (_Mats_2_stepII.Count < len_2);
                            if ( wasModify )
                            {
                                wma_2.MorphoAmbiguityTuples.Clear();
                                wma_2.MorphoAmbiguityTuples.AddRange( _Mats_2_stepII );
                            }
                            _Mats_2_stepII.Clear();

                            return (wasModify);
                        }
                    }
                }
                #endregion
            }
            else if ( len_2 == 1 )
            {
                #region [.1 < mats & 1 == wma.MorphoAmbiguityTuples.]
                var ma_case_2 = (wma_2.MorphoAmbiguityTuples[ 0 ].WordFormMorphology.MorphoAttribute & MorphoAttributeAllCases);
                if ( !IsCaseAnycase( ma_case_2 ) )
                {
                    for ( var i = 0; i < len_0; i++ )
                    {
                        var mat_0 = mats[ i ];
                        var ma_0  = mat_0.WordFormMorphology.MorphoAttribute;
                        if ( ma_case_2 == (ma_0 & MorphoAttributeAllCases) )
                        {
                            _Mats_0_stepII.Add( mat_0 );
                        }
                    }

                    if ( _Mats_0_stepII.Count != 0 )
                    {
                        var wasModify = (_Mats_0_stepII.Count < len_0);
                        if ( wasModify )
                        {
                            mats.Clear();
                            mats.AddRange( _Mats_0_stepII );                                
                        }
                        _Mats_0_stepII.Clear();

                        return (wasModify);
                    }
                }
                #endregion
            }
            else
            {
                #region [.1 < mats & 1 < wma.MorphoAmbiguityTuples.]
                for ( var i = 0; i < len_0; i++ )
                {
                    var mat_0 = mats[ i ];
                    var ma_case_0 = (mat_0.WordFormMorphology.MorphoAttribute & MorphoAttributeAllCases);
                    if ( !IsCaseAnycase( ma_case_0 ) )
                    {
                        for ( var j = 0; j < len_2; j++ )
                        {
                            var mat_2 = wma_2.MorphoAmbiguityTuples[ j ];
                            var ma_2  = mat_2.WordFormMorphology.MorphoAttribute;
                            if ( ma_case_0 == (ma_2 & MorphoAttributeAllCases) )
                            {
                                _Mats_0_stepII.Add( mat_0 );
                                _Mats_2_stepII.Add( mat_2 );
                            }
                        }
                    }
                }

                if ( _Mats_0_stepII.Count != 0 /*&& _Mats_1__2.Count != 0*/ )
                {
                    var wasModify = ((_Mats_0_stepII.Count < len_0) || (_Mats_2_stepII.Count < len_2));
                    if ( wasModify )
                    {
                        mats.Clear();
                        mats.AddRange( _Mats_0_stepII );                        

                        wma_2.MorphoAmbiguityTuples.Clear();
                        wma_2.MorphoAmbiguityTuples.AddRange( _Mats_2_stepII );                        
                    }
                    _Mats_0_stepII.Clear();
                    _Mats_2_stepII.Clear();

                    return (wasModify);
                }
                #endregion
            }

            return (false);
        }
        private bool Case_1__1_stepII_2( WordMorphoAmbiguity_t wma_2 )
        {
            Debug.Assert( _Mats_0.Count == _Mats_1.Count, "_Mats_0.Count != _Mats_1.Count" );

            var len_0 = _Mats_0.Count /*|| _Mats_1.Count*/;
            var len_2 = wma_2.MorphoAmbiguityTuples.Count;

            if ( len_0 == 1 )
            {
                #region [.1 == _Mats_0 & 1 < wma.MorphoAmbiguityTuples.]
                if ( 1 < len_2 )
                {
                    var ma_case_0 = (_Mats_0[ 0 ].WordFormMorphology.MorphoAttribute & MorphoAttributeAllCases);
                    if ( !IsCaseAnycase( ma_case_0 ) )
                    {
                        for ( var j = 0; j < len_2; j++ )
                        {
                            var mat_2 = wma_2.MorphoAmbiguityTuples[ j ];
                            var ma_2  = mat_2.WordFormMorphology.MorphoAttribute;
                            if ( ma_case_0 == (ma_2 & MorphoAttributeAllCases) )
                            {
                                _Mats_2_stepII.Add( mat_2 );
                            }
                        }

                        if ( _Mats_2_stepII.Count != 0 )
                        {
                            var wasModify = (_Mats_2_stepII.Count < len_2);
                            if ( wasModify )
                            {
                                wma_2.MorphoAmbiguityTuples.Clear();
                                wma_2.MorphoAmbiguityTuples.AddRange( _Mats_2_stepII );
                            }
                            _Mats_2_stepII.Clear();

                            return (wasModify);
                        }
                    }
                }
                #endregion
            }
            else if ( len_2 == 1 )
            {
                #region [.1 < _Mats_0 & 1 == wma.MorphoAmbiguityTuples.]
                var ma_case_2 = (wma_2.MorphoAmbiguityTuples[ 0 ].WordFormMorphology.MorphoAttribute & MorphoAttributeAllCases);
                if ( !IsCaseAnycase( ma_case_2 ) )
                {
                    for ( var i = 0; i < len_0; i++ )
                    {
                        var mat_0 = _Mats_0[ i ];
                        var ma_0  = mat_0.WordFormMorphology.MorphoAttribute;
                        if ( ma_case_2 == (ma_0 & MorphoAttributeAllCases) )
                        {
                            _Mats_0_stepII.Add( mat_0 );
                            var mat_1 = _Mats_1[ i ];
                            _Mats_1_stepII.Add( mat_1 );
                        }
                    }

                    if ( _Mats_0_stepII.Count != 0 )
                    {
                        var wasModify = (_Mats_0_stepII.Count < len_0);
                        if ( wasModify )
                        {
                            _Mats_0.Clear();
                            _Mats_0.AddRange( _Mats_0_stepII );

                            _Mats_1.Clear();
                            _Mats_1.AddRange( _Mats_1_stepII );
                        }
                        _Mats_0_stepII.Clear();
                        _Mats_1_stepII.Clear();

                        return (wasModify);
                    }
                }
                #endregion
            }
            else
            {
                #region [.1 < _Mats_0 & 1 < wma.MorphoAmbiguityTuples.]
                for ( var i = 0; i < len_0; i++ )
                {
                    var mat_0 = _Mats_0[ i ];
                    var ma_case_0 = (mat_0.WordFormMorphology.MorphoAttribute & MorphoAttributeAllCases);
                    if ( !IsCaseAnycase( ma_case_0 ) )
                    {
                        for ( var j = 0; j < len_2; j++ )
                        {
                            var mat_2 = wma_2.MorphoAmbiguityTuples[ j ];
                            var ma_2  = mat_2.WordFormMorphology.MorphoAttribute;
                            if ( ma_case_0 == (ma_2 & MorphoAttributeAllCases) )
                            {
                                _Mats_0_stepII.Add( mat_0 );
                                var mat_1 = _Mats_1[ i ];
                                _Mats_1_stepII.Add( mat_1 );
                                _Mats_2_stepII.Add( mat_2 );
                            }
                        }
                    }
                }

                if ( _Mats_0_stepII.Count != 0 /*&& _Mats_2_stepII.Count != 0*/ )
                {
                    var wasModify = ((_Mats_0_stepII.Count < len_0) || (_Mats_2_stepII.Count < len_2));
                    if ( wasModify )
                    {
                        _Mats_0.Clear();
                        _Mats_0.AddRange( _Mats_0_stepII );

                        _Mats_1.Clear();
                        _Mats_1.AddRange( _Mats_1_stepII );  

                        wma_2.MorphoAmbiguityTuples.Clear();
                        wma_2.MorphoAmbiguityTuples.AddRange( _Mats_2_stepII );                        
                    }
                    _Mats_0_stepII.Clear();
                    _Mats_1_stepII.Clear();
                    _Mats_2_stepII.Clear();

                    return (wasModify);
                }
                #endregion
            }

            return (false);
        }

        private void Case_1__2( WordMorphoAmbiguity_t wma_0, WordMorphoAmbiguity_t wma_1 )
        {
            var len_0 = wma_0.MorphoAmbiguityTuples.Count;
            var len_1 = wma_1.MorphoAmbiguityTuples.Count;

            if ( len_0 == 1 )
            {
                #region [.1 == mats_0 & 1 < mats_1.]
                if ( 1 < len_1 )
                {
                    var ma_case_0 = (wma_0.MorphoAmbiguityTuples[ 0 ].WordFormMorphology.MorphoAttribute & MorphoAttributeAllCases);
                    if ( !IsCaseAnycase( ma_case_0 ) )
                    {
                        for ( var j = 0; j < len_1; j++ )
                        {
                            var mat_1 = wma_1.MorphoAmbiguityTuples[ j ];
                            var ma_1  = mat_1.WordFormMorphology.MorphoAttribute;
                            if ( ma_case_0 == (ma_1 & MorphoAttributeAllCases) )
                            {
                                _Mats_1.Add( mat_1 );
                            }
                        }

                        if ( _Mats_1.Count != 0 )
                        {
                            var wasModify = (_Mats_1.Count < len_1);
                            if ( wasModify )
                            {
                                wma_1.MorphoAmbiguityTuples.Clear();
                                wma_1.MorphoAmbiguityTuples.AddRange( _Mats_1 );                                
                            }
                            _Mats_1.Clear();
                        }
                    }
                }
                #endregion
            }
            else if ( len_1 == 1 )
            {
                #region [.1 < mats_0 & 1 == mats_1.]
                    var ma_case_1 = (wma_1.MorphoAmbiguityTuples[ 0 ].WordFormMorphology.MorphoAttribute & MorphoAttributeAllCases);
                    if ( !IsCaseAnycase( ma_case_1 ) )
                    {
                        for ( var i = 0; i < len_0; i++ )
                        {
                            var mat_0 = wma_0.MorphoAmbiguityTuples[ i ];
                            var ma_0  = mat_0.WordFormMorphology.MorphoAttribute;
                            if ( ma_case_1 == (ma_0 & MorphoAttributeAllCases) )
                            {
                                _Mats_0.Add( mat_0 );
                            }
                        }

                        if ( _Mats_0.Count != 0 )
                        {
                            var wasModify = (_Mats_0.Count < len_0);
                            if ( wasModify )
                            {
                                wma_0.MorphoAmbiguityTuples.Clear();
                                wma_0.MorphoAmbiguityTuples.AddRange( _Mats_0 );                                
                            }
                            _Mats_0.Clear();
                        }
                    }
                #endregion
            }
            else
            {
                #region [.1 < mats_0 & 1 < mats_1.]
                for ( var i = 0; i < len_0; i++ )
                {
                    var mat_0 = wma_0.MorphoAmbiguityTuples[ i ];
                    var ma_case_0 = (mat_0.WordFormMorphology.MorphoAttribute & MorphoAttributeAllCases);
                    if ( !IsCaseAnycase( ma_case_0 ) )
                    {
                        for ( var j = 0; j < len_1; j++ )
                        {
                            var mat_1 = wma_1.MorphoAmbiguityTuples[ j ];
                            var ma_1  = mat_1.WordFormMorphology.MorphoAttribute;
                            if ( ma_case_0 == (ma_1 & MorphoAttributeAllCases) )
                            {
                                _Mats_0.AddIfNotExists( mat_0 );
                                _Mats_1.AddIfNotExists( mat_1 );
                            }
                        }
                    }
                }

                if ( _Mats_0.Count != 0 /*&& _Mats_1.Count != 0*/ )
                {
                    var wasModify = ((_Mats_0.Count < len_0) || (_Mats_1.Count < len_1));
                    if ( wasModify )
                    {
                        wma_0.MorphoAmbiguityTuples.Clear();
                        wma_0.MorphoAmbiguityTuples.AddRange( _Mats_0 );                        

                        wma_1.MorphoAmbiguityTuples.Clear();
                        wma_1.MorphoAmbiguityTuples.AddRange( _Mats_1 );                        
                    }
                    _Mats_0.Clear();
                    _Mats_1.Clear();
                }
                #endregion
            }
        }

        private bool Case_2( WordMorphoAmbiguity_t wma_0, WordMorphoAmbiguity_t wma_1 )
        {
            var len_0 = wma_0.MorphoAmbiguityTuples.Count;
            var len_1 = wma_1.MorphoAmbiguityTuples.Count;

            if ( len_0 == 1 )
            {
                #region [.1 == mats_0 & 1 < mats_1.]
                if ( 1 < len_1 )
                {
                    var ma_0      = wma_0.MorphoAmbiguityTuples[ 0 ].WordFormMorphology.MorphoAttribute;
                    var ma_case_0 = (ma_0 & MorphoAttributeAllCases);
                    if ( !IsCaseAnycase( ma_case_0 ) )
                    {
                        for ( var j = 0; j < len_1; j++ )
                        {
                            var mat_1 = wma_1.MorphoAmbiguityTuples[ j ];
                            if ( ma_case_0 == (mat_1.WordFormMorphology.MorphoAttribute & MorphoAttributeAllCases) )
                            {
                                _Mats_1.Add( mat_1 );
                            }
                        }

                        if ( _Mats_1.Count != 0 )
                        {
                            if ( Case_2_TryFilterByMask( MorphoAttributeAllNumber, ma_0, _Mats_1 ) )
                            {
                                if ( !IsGenderGeneral( ma_0 ) )
                                {
                                    Case_2_TryFilterByMask( MorphoAttributeAllGender, ma_0, _Mats_1 );
                                }
                            }

                            var wasModify = (_Mats_1.Count < wma_1.MorphoAmbiguityTuples.Count);
                            if ( wasModify )
                            {
                                wma_1.MorphoAmbiguityTuples.Clear();
                                wma_1.MorphoAmbiguityTuples.AddRange( _Mats_1 );                                
                            }
                            _Mats_1.Clear();
                            return (wasModify);
                        }
                    }
                }
                #endregion
            }
            else if ( len_1 == 1 )
            {
                #region [.1 < mats_0 & 1 == mats_1.]
                    var ma_1      = wma_1.MorphoAmbiguityTuples[ 0 ].WordFormMorphology.MorphoAttribute;
                    var ma_case_1 = (ma_1 & MorphoAttributeAllCases);
                    if ( !IsCaseAnycase( ma_case_1 ) )
                    {
                        for ( var i = 0; i < len_0; i++ )
                        {
                            var mat_0 = wma_0.MorphoAmbiguityTuples[ i ];
                            if ( ma_case_1 == (mat_0.WordFormMorphology.MorphoAttribute & MorphoAttributeAllCases) )
                            {
                                _Mats_0.Add( mat_0 );
                            }
                        }

                        if ( _Mats_0.Count != 0 )
                        {
                            if ( Case_2_TryFilterByMask( MorphoAttributeAllNumber, ma_1, _Mats_0 ) )
                            {
                                if ( !IsGenderGeneral( ma_1 ) )
                                {
                                    Case_2_TryFilterByMask( MorphoAttributeAllGender, ma_1, _Mats_0 );
                                }
                            }

                            var wasModify = (_Mats_0.Count < wma_0.MorphoAmbiguityTuples.Count);
                            if ( wasModify )
                            {
                                wma_0.MorphoAmbiguityTuples.Clear();
                                wma_0.MorphoAmbiguityTuples.AddRange( _Mats_0 );
                            }
                            _Mats_0.Clear();
                            return (wasModify);
                        }
                    }
                #endregion
            }
            else
            {
                #region [.1 < mats_0 & 1 < mats_1.]
                for ( var i = 0; i < len_0; i++ )
                {
                    var mat_0     = wma_0.MorphoAmbiguityTuples[ i ];
                    var ma_0      = mat_0.WordFormMorphology.MorphoAttribute;
                    var ma_case_0 = (ma_0 & MorphoAttributeAllCases);
                    if ( !IsCaseAnycase( ma_case_0 ) )
                    {
                        for ( var j = 0; j < len_1; j++ )
                        {
                            var mat_1 = wma_1.MorphoAmbiguityTuples[ j ];
                            if ( ma_case_0 == (mat_1.WordFormMorphology.MorphoAttribute & MorphoAttributeAllCases) )
                            {
                                _Mats_0.AddIfNotExists( mat_0 );
                                _Mats_1.AddIfNotExists( mat_1 );
                            }
                        }
                    }
                }

                if ( _Mats_0.Count != 0 /*&& _Mats_1.Count != 0*/ )
                {
                    if ( Case_2_TryFilterByNumber( _Mats_0, _Mats_1 ) )
                    {
                        Case_2_TryFilterByGender( _Mats_0, _Mats_1 );
                    }

                    var wasModify = ((_Mats_0.Count < wma_0.MorphoAmbiguityTuples.Count) ||
                                     (_Mats_1.Count < wma_1.MorphoAmbiguityTuples.Count)
                                    );
                    if ( wasModify )
                    {
                        wma_0.MorphoAmbiguityTuples.Clear();
                        wma_0.MorphoAmbiguityTuples.AddRange( _Mats_0 );                        

                        wma_1.MorphoAmbiguityTuples.Clear();
                        wma_1.MorphoAmbiguityTuples.AddRange( _Mats_1 );                        
                    }
                    _Mats_1.Clear();
                    _Mats_0.Clear();
                    return (wasModify);
                }
                #endregion
            }

            return (false);
        }
        private static bool Case_2_TryFilterByMask( MorphoAttributeEnum mask, MorphoAttributeEnum ma, List< MorphoAmbiguityTuple_t > mats )
        {
            var len = mats.Count - 1;
            if ( 0 < len )
            {
                ma = ma & mask;
                for ( var i = len; 0 <= i; i-- )
                {
                    var mat = mats[ i ];
                    if ( ma == (mat.WordFormMorphology.MorphoAttribute & mask) )
                    {
                        for ( i = len; 0 <= i; i-- )
                        {
                            mat = mats[ i ];
                            if ( ma != (mat.WordFormMorphology.MorphoAttribute & mask) )
                            {
                                mats.RemoveAt( i );
                            }
                        }
                        return (true);
                    }
                }
            }
            return (false);
        }
        private static bool Case_2_TryFilterByNumber( List< MorphoAmbiguityTuple_t > mats_0, List< MorphoAmbiguityTuple_t > mats_1 )
        {
            var len = mats_0.Count;
            if ( 1 < len || 1 < mats_1.Count )
            {
                var wasModify = false;
                for ( var i = 0; i < len; i++ )
                {
                    wasModify |= Case_2_TryFilterByMask( MorphoAttributeAllNumber, mats_0[ i ].WordFormMorphology.MorphoAttribute, mats_1 );
                }
                return (wasModify);
            }
            return (false);
        }
        private static bool Case_2_TryFilterByGender( List< MorphoAmbiguityTuple_t > mats_0, List< MorphoAmbiguityTuple_t > mats_1 )
        {
            var len = mats_0.Count;
            if ( 1 < len || 1 < mats_1.Count )
            {
                var wasModify = false;
                for ( var i = 0; i < len; i++ )
                {
                    var ma = mats_0[ i ].WordFormMorphology.MorphoAttribute;
                    if ( !IsGenderGeneral( ma ) )
                    {
                        wasModify |= Case_2_TryFilterByMask( MorphoAttributeAllGender, ma, mats_1 );
                    }
                }
                return (wasModify);
            }
            return (false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class MorphoAmbiguityPreProcessorExt
    {
        [M(O.AggressiveInlining)] public static void AddIfNotExists( this List< MorphoAmbiguityTuple_t > mats, MorphoAmbiguityTuple_t mat )
        {
            for ( int i = 0, len = mats.Count; i < len; i++ )
            {
                if ( Equals( mats[ i ], mat ) )
                {
                    return;
                }
            }

            mats.Add( mat );
        }

        [M(O.AggressiveInlining)] private static bool Equals( in MorphoAmbiguityTuple_t x, in MorphoAmbiguityTuple_t y )
        {
            if ( !WordFormMorphology_t.Equals( in x.WordFormMorphology, in y.WordFormMorphology ) )
                return (false);

            if ( string.CompareOrdinal( x.Word.valueUpper, y.Word.valueUpper ) != 0 )
                return (false);

            return (x.PunctuationType == y.PunctuationType);
        }
    }
}
