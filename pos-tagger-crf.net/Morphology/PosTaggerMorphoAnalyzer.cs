using System;
using System.Collections.Generic;
using System.Diagnostics;

using lingvo.core;
using lingvo.morphology;
using lingvo.ner;
using lingvo.tokenizing;
using MorphoAmbiguityResolver = lingvo.postagger.MorphoAmbiguityResolver_5g;

namespace lingvo.postagger
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class /*struct*/ MorphoAmbiguityTuple_t
    {
        /// <summary>
        /// 
        /// </summary>
        public enum PunctuationTypeEnum : byte
        {
            __NonPunctuation__,

            PunctuationQuote,
            PunctuationBracket,
            Punctuation,
        }
        
        unsafe public MorphoAmbiguityTuple_t( 
            word_t               word, 
            WordFormMorphology_t wordFormMorphology, 
            PunctuationTypeEnum  punctuationType )
        {
            Word               = word;
            WordFormMorphology = wordFormMorphology;
            PunctuationType    = punctuationType;
        }

        unsafe public static PunctuationTypeEnum GetPunctuationType( word_t word )
        {
            if ( word.posTaggerOutputType == PosTaggerOutputType.Punctuation )
            {
                if ( word.nerInputType == NerInputType.Q )
                {
                    return (PunctuationTypeEnum.PunctuationQuote);
                }
                else
                {
                    fixed ( char* _base = word.valueOriginal )
                    {
                        var ct = *(xlat_Unsafe.Inst._CHARTYPE_MAP + *_base);
                        if ( (ct & CharType.IsQuote) == CharType.IsQuote )
                        {
                            word.nerInputType = NerInputType.Q;
                            return (PunctuationTypeEnum.PunctuationQuote);
                        }
                        else
                        if ( (ct & CharType.IsBracket) == CharType.IsBracket )
                        {
                            return (PunctuationTypeEnum.PunctuationBracket);
                        }
                        else
                        {
                            return (PunctuationTypeEnum.Punctuation);
                        }
                    }
                }
            }
            else
            {
                return (PunctuationTypeEnum.__NonPunctuation__);
            }
        }

        public /*readonly*/ word_t               Word;
        public /*readonly*/ WordFormMorphology_t WordFormMorphology;
        public /*readonly*/ PunctuationTypeEnum  PunctuationType;

        public override string ToString()
        {
            return (((PunctuationType != PunctuationTypeEnum.__NonPunctuation__) ? ("{" + PunctuationType + "}, ") : string.Empty) + 
                    "Word: " + Word + ", WordFormMorphology: " + WordFormMorphology);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal sealed class /*struct*/ WordMorphoAmbiguity_t
    {        
        public WordMorphoAmbiguity_t( 
            word_t                                      word, 
            MorphoAmbiguityTuple_t.PunctuationTypeEnum  punctuationType, 
            List< MorphoAmbiguityTuple_t >              morphoAmbiguityTuples )
        {
            Word                  = word;
            PunctuationType       = punctuationType;
            MorphoAmbiguityTuples = morphoAmbiguityTuples;
        }

        public readonly word_t                                     Word;
        public readonly MorphoAmbiguityTuple_t.PunctuationTypeEnum PunctuationType;
        public readonly List< MorphoAmbiguityTuple_t >             MorphoAmbiguityTuples;

        public bool IsPunctuation()
        {
            return (PunctuationType != MorphoAmbiguityTuple_t.PunctuationTypeEnum.__NonPunctuation__);
        }
        public void SetWordMorphologyAsUndefined()
        {
            var wma = MorphoAmbiguityTuples[ 0 ];

            if ( !wma.WordFormMorphology.IsEmpty() )
            {
                Word.morphology = wma.WordFormMorphology;
            }
            else
            {
                var partOfSpeech = PosTaggerMorphoAnalyzer.ToPartOfSpeech( Word.posTaggerOutputType ).GetValueOrDefault();
                Word.morphology = new WordFormMorphology_t( partOfSpeech );
            }

            MorphoAmbiguityTuples.Clear();
            MorphoAmbiguityTuples.Add( new MorphoAmbiguityTuple_t( Word, Word.morphology, wma.PunctuationType ) );
        }
        public void SetWordMorphologyAsDefined()
        {
            //if ( MorphoAmbiguityTuples.Count != 0 )
            //{
            var punctuationType = MorphoAmbiguityTuples[ 0 ].PunctuationType;
            MorphoAmbiguityTuples.Clear();
            MorphoAmbiguityTuples.Add( new MorphoAmbiguityTuple_t( Word, Word.morphology, punctuationType ) );
            //}
        }

        public override string ToString()
        {
            return (((PunctuationType != MorphoAmbiguityTuple_t.PunctuationTypeEnum.__NonPunctuation__) ? ("{" + PunctuationType + "}, ") : string.Empty) + 
                    "MorphoAmbiguityTuples: " + MorphoAmbiguityTuples.Count + ", Word: " + Word
                   );
        }
    }
    /// <summary>
    /// 
    /// </summary>
    unsafe internal struct WordMorphoAmbiguityFactory
    {
        private const int DEFAULT_WORD_COUNT               = 100;
        private const int DEFAULT_WORDFORMMORPHOLOGY_COUNT = 5;

        private readonly List< List< MorphoAmbiguityTuple_t > > _MorphoAmbiguityTuples_Buffer;

        public WordMorphoAmbiguityFactory( object _dummy_ )
        {
            _MorphoAmbiguityTuples_Buffer = new List< List< MorphoAmbiguityTuple_t > >( DEFAULT_WORD_COUNT );
            for ( var i = 0; i < _MorphoAmbiguityTuples_Buffer.Capacity; i++ )
            {
                _MorphoAmbiguityTuples_Buffer.Add( new List< MorphoAmbiguityTuple_t >( DEFAULT_WORDFORMMORPHOLOGY_COUNT ) ); 
            }
        }

        unsafe public WordMorphoAmbiguity_t Create( word_t word, int wordIdex )
        {
            while ( _MorphoAmbiguityTuples_Buffer.Count <= wordIdex )
            {
                _MorphoAmbiguityTuples_Buffer.Add( new List< MorphoAmbiguityTuple_t >( DEFAULT_WORDFORMMORPHOLOGY_COUNT ) );
            }

            var punctuationType = MorphoAmbiguityTuple_t.GetPunctuationType( word );
            var buffer = _MorphoAmbiguityTuples_Buffer[ wordIdex ];
            buffer.Clear();
            buffer.Add( new MorphoAmbiguityTuple_t( word, new WordFormMorphology_t(), punctuationType ) );

            return (new WordMorphoAmbiguity_t( word, punctuationType, buffer ));
        }
        unsafe public WordMorphoAmbiguity_t Create( word_t word, int wordIdex, WordFormMorphology_t[] wordFormMorphologies )
        {
            while ( _MorphoAmbiguityTuples_Buffer.Count <= wordIdex )
            {
                _MorphoAmbiguityTuples_Buffer.Add( new List< MorphoAmbiguityTuple_t >( DEFAULT_WORDFORMMORPHOLOGY_COUNT ) );
            }

            var punctuationType = MorphoAmbiguityTuple_t.GetPunctuationType( word );
            var buffer = _MorphoAmbiguityTuples_Buffer[ wordIdex ];
            buffer.Clear();
            for ( int i = 0, len = wordFormMorphologies.Length; i < len; i++ )
            {
                buffer.Add( new MorphoAmbiguityTuple_t( word, wordFormMorphologies[ i ], punctuationType ) );
            }
            return (new WordMorphoAmbiguity_t( word, punctuationType, buffer ));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    unsafe public sealed class PosTaggerMorphoAnalyzer : IDisposable
    {
        #region [.private field's.]
        private readonly MorphoAnalyzer                   _MorphoAnalyzer;
        private readonly IMorphoModel                     _MorphoModel;
        private readonly MorphoAmbiguityPreProcessor      _MorphoAmbiguityPreProcessor;
        private readonly MorphoAmbiguityResolver          _MorphoAmbiguityResolver;        
        private readonly List< WordFormMorphology_t  >    _wordFormMorphologies_Buffer;
        private readonly WordMorphoAmbiguityFactory       _WordMorphoAmbiguityFactory;
        private readonly List< WordMorphoAmbiguity_t >    _WordMorphoAmbiguities;
        private readonly CharType*                        _CTM;
        #endregion

        #region [.ctor().]
        public PosTaggerMorphoAnalyzer( IMorphoModel morphoModel
            , MorphoAmbiguityResolverModel morphoAmbiguityModel )
        {
            _MorphoModel                 = morphoModel;
            _MorphoAnalyzer              = new MorphoAnalyzer( _MorphoModel );
            _MorphoAmbiguityPreProcessor = new MorphoAmbiguityPreProcessor();
            _MorphoAmbiguityResolver     = new MorphoAmbiguityResolver( morphoAmbiguityModel );
            _wordFormMorphologies_Buffer = new List< WordFormMorphology_t >();
            _WordMorphoAmbiguityFactory  = new WordMorphoAmbiguityFactory( null );
            _WordMorphoAmbiguities       = new List< WordMorphoAmbiguity_t >();
            _CTM                         = xlat_Unsafe.Inst._CHARTYPE_MAP;
        }

        public void Dispose()
        {
            //---!!!---_MorphoModel.Dispose();
            _MorphoAmbiguityResolver.Dispose();
        }
        #endregion

        unsafe public void Run( List< word_t > words
#if DEBUG
            , bool applyMorphoAmbiguityPreProcess
#endif
            )
        {
            //-apply morpho-analysis-//
            var wordMorphology = default(WordMorphology_t);
            for ( int i = 0, wordsLength = words.Count; i < wordsLength; i++ )
            {
                var word = words[ i ];

                #region [.sikp something & make Morpho-Analyze.]
                if ( word.posTaggerExtraWordType != PosTaggerExtraWordType.__DEFAULT__ )
                {
                    _WordMorphoAmbiguities.Add( _WordMorphoAmbiguityFactory.Create( word, i ) );
                    continue;
                }

                switch ( word.posTaggerInputType )
                {
                    case PosTaggerInputType.O:      // = "O"; // Другой
                    case PosTaggerInputType.AllLat: // - только латиница: нет строчных и точек;
                    case PosTaggerInputType.FstC:   // - первая заглавная, не содержит пробелов;
                    #region
                    {
                        if ( word.valueUpper == null )
                        {
                            _WordMorphoAmbiguities.Add( _WordMorphoAmbiguityFactory.Create( word, i ) );
                            continue;
                        }

                    #if DEBUG
                        var wordFirstCharIsUpper = (*(_CTM + word.valueOriginal[ 0 ]) & CharType.IsUpper) == CharType.IsUpper;
                        Debug.Assert( wordFirstCharIsUpper == word.posTaggerFirstCharIsUpper, "(wordFirstCharIsUpper != word.posTaggerFirstCharIsUpper)" );
                    #endif

                        var mode = GetWordFormMorphologyMode( word, i );
                        wordMorphology = _MorphoAnalyzer.GetWordMorphology_NoToUpper( word.valueUpper, mode );

                        //---!!!---DONT-FREE-USED-IN-FURTHER---// word.valueUpper = null; //free resource
                    }
                    #endregion
                    break;

                    case PosTaggerInputType.Num:    // – содержит хотя бы одну цифру и не содержит букв;
                    #region
                    {
                        //---Debug.Assert( word.valueUpper == null, "word.valueUpper != null" );---//
                        if ( word.posTaggerLastValueUpperInNumeralChain == null )
                        {
                            _WordMorphoAmbiguities.Add( _WordMorphoAmbiguityFactory.Create( word, i ) );
                            continue;
                        }

                        var mode = GetWordFormMorphologyMode( word, i );
                        wordMorphology = _MorphoAnalyzer.GetWordMorphology_4LastValueUpperInNumeralChain( 
                                                    word.posTaggerLastValueUpperInNumeralChain, mode );

                        #region [.commented.]
                        /*
                        fixed ( char* ptr = word.posTaggerLastValueOriginalInNumeralChain )
                        {
                            //-get-first-is-upper-from-valueOriginal-//
                            //---var wordFirstCharIsUpper = (*(_CTM + *ptr) & CharType.IsUpper) == CharType.IsUpper;
                            //---var wordFormMorphologyMode = GetWordFormMorphologyMode( word, wordFirstCharIsUpper, i );
                            var mode = GetWordFormMorphologyMode( word, i );
                            var length = word.posTaggerLastValueOriginalInNumeralChain.Length;
                            wordMorphology = _MorphoAnalyzer.GetWordMorphology_4LastValueOriginalInNumeralChain( ptr, length, mode );
                        }
                        */
                        #endregion

                        //---!!!---DONT-FREE-USED-IN-FURTHER---// word.posTaggerLastValueOriginalInNumeralChain = null; //free resource
                    }
                    #endregion
                    break;

                    default:
                        #region [.who's skip.]
                        //CompPh – составные (имеющие хотя бы один пробел);
                        //Col    – двоеточие.
                        //Com    – запятая;
                        //Dush   – тире;
                        //OneCP  – первая заглавная с точкой;
                        #endregion
                        _WordMorphoAmbiguities.Add( _WordMorphoAmbiguityFactory.Create( word, i ) );
                    continue;
                }
                #endregion

                #region [.post-process Morpho-Analyze-result.]
                if ( wordMorphology.HasWordFormMorphologies )
                {
                    var wfms = default(WordFormMorphology_t[]);
                    //Если данное слово имеет только одну часть речи, прописанную в морфословаре, то использовать ее вместо определённой с помощью PoS-tagger.
                    if ( wordMorphology.IsSinglePartOfSpeech )
                    {
                        CorrectPosTaggerOutputType( word, wordMorphology.PartOfSpeech );
                        wfms = wordMorphology.WordFormMorphologies.ToArray();
                        _WordMorphoAmbiguities.Add( _WordMorphoAmbiguityFactory.Create( word, i, wfms ) );
                        #if DEBUG
                            word.morphologies = wfms;
                        #endif
                    }
                    else
                    {
                        #region [.clause #1.]
                        //для данного слова в морфословаре определено несколько частей речи. 
                        //ищем среди морфоинформации по слову морфоинформацию по части речи от pos-tagger'а, если она есть - берем её

                        //вот эта хуйня из-за двойной трансляции 
                        //{PosTaggerOutputType::AdjectivePronoun => PartOfSpeechEnum::Adjective, PartOfSpeechEnum::Pronoun} 
                        // & 
                        //{PosTaggerOutputType::AdverbialPronoun => PartOfSpeechEnum::Adverb, PartOfSpeechEnum::Pronoun}
                        var partOfSpeech = default(PartOfSpeechEnum?);
                        switch ( word.posTaggerOutputType )
                        {
                            case PosTaggerOutputType.AdjectivePronoun:
                            #region
                            {
                                wfms = TryGetByPosTaggerOutputType( ref wordMorphology, PartOfSpeechEnum.Adjective, PartOfSpeechEnum.Pronoun );
                                if ( wfms != null )
                                {
                                    _WordMorphoAmbiguities.Add( _WordMorphoAmbiguityFactory.Create( word, i, wfms ) );
                                    #if DEBUG
                                        word.morphologies = wfms;
                                    #endif
                                    continue;
                                }
                            }
                            #endregion
                            break;

                            case PosTaggerOutputType.AdverbialPronoun:
                            #region
                            {
                                wfms = TryGetByPosTaggerOutputType( ref wordMorphology, PartOfSpeechEnum.Adverb, PartOfSpeechEnum.Pronoun );
                                if ( wfms != null )
                                {
                                    _WordMorphoAmbiguities.Add( _WordMorphoAmbiguityFactory.Create( word, i, wfms ) );
                                    #if DEBUG
                                        word.morphologies = wfms;
                                    #endif
                                    continue;
                                } 
                            }
                            #endregion
                            break;

                            default:
                            #region
                            {
                                partOfSpeech = ToPartOfSpeech( word.posTaggerOutputType );
                                if ( partOfSpeech.HasValue )
                                {
                                    wfms = TryGetByPosTaggerOutputType( ref wordMorphology, partOfSpeech.Value );
                                    if ( wfms != null )
                                    {
                                        _WordMorphoAmbiguities.Add( _WordMorphoAmbiguityFactory.Create( word, i, wfms ) );
                                        #if DEBUG
                                            word.morphologies = wfms;
                                        #endif
                                        continue;
                                    }
                                }
                            }
                            #endregion
                            break;
                        }
                        #endregion

                        #region [.clause #2.]
                        //При этом  для данного слова в морфословаре определено несколько частей речи.  
                        //В данном случае в первую очередь м.б. соответствия (слева выход PoS-tagger, справа морфословарь, последовательность пунктов неважна):
                        switch ( word.posTaggerOutputType )
                        {
                            #region [.commented. previous.]
                            /*
                            /*2.1. AdjectivePronoun = Adjective* /
                            case PosTaggerOutputType.AdjectivePronoun:
                                wfms = TryGetByPosTaggerOutputType( ref wordMorphology, PartOfSpeechEnum.Adjective );                                    
                                #if DEBUG
                                    word.morphologies = wfms;
                                #endif
                            break;

                            /*2.2. AdverbialPronoun = Adverb* /
                            case PosTaggerOutputType.AdverbialPronoun:
                                wfms = TryGetByPosTaggerOutputType( ref wordMorphology, PartOfSpeechEnum.Adverb );
                                #if DEBUG
                                    word.morphologies = wfms;
                                #endif
                            break;
                            */
                            #endregion

                            /*2.3. Participle = Adjective */
                            case PosTaggerOutputType.Participle:
                                wfms = TryGetByPosTaggerOutputType( ref wordMorphology, PartOfSpeechEnum.Adjective );
                                #if DEBUG
                                    word.morphologies = wfms;
                                #endif
                            break;

                            default:
                            #region [.clause #3.]
                            {
                                if ( partOfSpeech.HasValue ) 
                                {
                                    switch ( partOfSpeech.Value )
                                    {
                                        /*3.1. Pronoun = Noun*/
                                        case PartOfSpeechEnum.Pronoun:
                                            wfms = TryGetByPosTaggerOutputType( ref wordMorphology, PartOfSpeechEnum.Noun );
                                            #if DEBUG
                                                word.morphologies = wfms;
                                            #endif
                                        break;

                                        /*3.2. Noun = Pronoun */
                                        case PartOfSpeechEnum.Noun:
                                            wfms = TryGetByPosTaggerOutputType( ref wordMorphology, PartOfSpeechEnum.Pronoun );
                                            #if DEBUG
                                                word.morphologies = wfms;
                                            #endif
                                        break;

                                        /*3.3. Conjunction = Particle */
                                        case PartOfSpeechEnum.Conjunction:
                                            wfms = TryGetByPosTaggerOutputType( ref wordMorphology, PartOfSpeechEnum.Particle );
                                            #if DEBUG
                                                word.morphologies = wfms;
                                            #endif
                                        break;

                                        /*3.4. Particle = Conjunction*/
                                        case PartOfSpeechEnum.Particle:
                                            wfms = TryGetByPosTaggerOutputType( ref wordMorphology, PartOfSpeechEnum.Conjunction );
                                            #if DEBUG
                                                word.morphologies = wfms;
                                            #endif
                                        break;

                                        /*3.5. Numeral = Noun, Adjective */
                                        case PartOfSpeechEnum.Numeral:
                                            wfms = TryGetByPosTaggerOutputType( ref wordMorphology, PartOfSpeechEnum.Noun, PartOfSpeechEnum.Adjective );
                                            #if DEBUG
                                                word.morphologies = wfms;
                                            #endif
                                        break;

                                        /*3.6. Adjective = Verb, Adverb*/
                                        case PartOfSpeechEnum.Adjective:
                                            wfms = TryGetByPosTaggerOutputType( ref wordMorphology, PartOfSpeechEnum.Verb, PartOfSpeechEnum.Adverb );
                                            #if DEBUG
                                                word.morphologies = wfms;
                                            #endif
                                        break;

                                        /*3.7. Adverb = Adjective */
                                        case PartOfSpeechEnum.Adverb:
                                            wfms = TryGetByPosTaggerOutputType( ref wordMorphology, PartOfSpeechEnum.Adjective );
                                            #if DEBUG
                                                word.morphologies = wfms;
                                            #endif
                                        break;
                                    }
                                }
                            }
                            #endregion
                            break;
                        }

                        /*Если таковых соответствий не нашлось, то берется первая из выдачи морфословаря часть речи.*/
                        if ( wfms == null )
                        {
                            var _partOfSpeech = wordMorphology.WordFormMorphologies[ 0 ].PartOfSpeech;
                            word.posTaggerOutputType = ToPosTaggerOutputType( _partOfSpeech );
                            wfms = TryGetByPosTaggerOutputType( ref wordMorphology, _partOfSpeech );
                            #if DEBUG
                                word.morphologies = wfms;
                            #endif
                        }

                        _WordMorphoAmbiguities.Add( _WordMorphoAmbiguityFactory.Create( word, i, wfms ) );

                        #endregion
                    }
                }
                else
                {
                    _WordMorphoAmbiguities.Add( _WordMorphoAmbiguityFactory.Create( word, i ) );
                }
                #endregion
            }

            //-pre-process morpho-ambiguity-//
#if DEBUG
            if ( applyMorphoAmbiguityPreProcess )
            {
#endif
            _MorphoAmbiguityPreProcessor.Run( _WordMorphoAmbiguities );
#if DEBUG
            }
#endif
            //-resolve morpho-ambiguity-//
            _MorphoAmbiguityResolver.Resolve( _WordMorphoAmbiguities );

            //-clear-//
            _WordMorphoAmbiguities.Clear();
        }

        private WordFormMorphology_t[] TryGetByPosTaggerOutputType( ref WordMorphology_t wordMorphology
            , PartOfSpeechEnum filterPartOfSpeech )
        {
            if ( (wordMorphology.PartOfSpeech & filterPartOfSpeech) == filterPartOfSpeech )
            {
                var morphologies = TryGetByPosTaggerOutputType( wordMorphology.WordFormMorphologies, filterPartOfSpeech );
                return (morphologies);
            }

            return (null);
        }
        private WordFormMorphology_t[] TryGetByPosTaggerOutputType( ref WordMorphology_t wordMorphology
            , PartOfSpeechEnum filterPartOfSpeech_1, PartOfSpeechEnum filterPartOfSpeech_2 )
        {
            if ( (wordMorphology.PartOfSpeech & filterPartOfSpeech_1) == filterPartOfSpeech_1 )
            {
                var morphologies = TryGetByPosTaggerOutputType( wordMorphology.WordFormMorphologies, filterPartOfSpeech_1 );
                if ( morphologies != null )
                {
                    return (morphologies);
                }
            }

            if ( (wordMorphology.PartOfSpeech & filterPartOfSpeech_2) == filterPartOfSpeech_2 )
            {
                var morphologies = TryGetByPosTaggerOutputType( wordMorphology.WordFormMorphologies, filterPartOfSpeech_2 );
                //if ( morphologies != null )
                //{
                return (morphologies);
                //}
            }

            return (null);
        }
        private WordFormMorphology_t[] TryGetByPosTaggerOutputType( List< WordFormMorphology_t > wordFormMorphologies
            , PartOfSpeechEnum filterPartOfSpeech )
        {
            _wordFormMorphologies_Buffer.Clear();

            for ( int i = 0, len = wordFormMorphologies.Count; i < len; i++ )
            {
                var wordFormMorphology = wordFormMorphologies[ i ];
                if ( (wordFormMorphology.PartOfSpeech & filterPartOfSpeech) == filterPartOfSpeech )
                {
                    //if ( FilterByLetterCase( ref wordFormMorphology, _WordFirstCharIsUpper ) )
                    //{
                    _wordFormMorphologies_Buffer.Add( wordFormMorphology );
                    //}
                }
            }

            if ( _wordFormMorphologies_Buffer.Count != 0 )
            {
                var result = _wordFormMorphologies_Buffer.ToArray(); //FilterByLetterCase( _FilteredWordFormMorphologies ); //
                return (result);
            }
            return (null);
        }

        #region [.table {PartOfSpeechEnum} <=> {PosTaggerOutputType}.]
        /*
        морфоанализатор::{PartOfSpeechEnum}  PoS-tagger::{PosTaggerOutputType}
        PartOfSpeechEnum.Adjective	    PosTaggerOutputType.Adjective
                                        PosTaggerOutputType.AdjectivePronoun
        PartOfSpeechEnum.Adverb	        PosTaggerOutputType.Adverb
                                        PosTaggerOutputType.AdverbialPronoun
        PartOfSpeechEnum.Article	    PosTaggerOutputType.Article
        PartOfSpeechEnum.Conjunction	PosTaggerOutputType.Conjunction
        PartOfSpeechEnum.Interjection	PosTaggerOutputType.Interjection
        PartOfSpeechEnum.Noun	        PosTaggerOutputType.Noun
        PartOfSpeechEnum.Numeral	    PosTaggerOutputType.Numeral
        PartOfSpeechEnum.Other	        PosTaggerOutputType.Other
        PartOfSpeechEnum.Particle	    PosTaggerOutputType.Particle
        PartOfSpeechEnum.Predicate	    PosTaggerOutputType.Predicate
        PartOfSpeechEnum.Preposition	PosTaggerOutputType.Preposition
        PartOfSpeechEnum.Pronoun	    PosTaggerOutputType.Pronoun 
	                                    PosTaggerOutputType.PossessivePronoun
	                                    PosTaggerOutputType.AdjectivePronoun  
	                                    PosTaggerOutputType.AdverbialPronoun
        PartOfSpeechEnum.Verb	        PosTaggerOutputType.Verb
	                                    PosTaggerOutputType.Infinitive
	                                    PosTaggerOutputType.AdverbialParticiple
	                                    PosTaggerOutputType.AuxiliaryVerb
	                                    PosTaggerOutputType.Participle
        -	                            PosTaggerOutputType.Punctuation
        */
        #endregion

        private  static void CorrectPosTaggerOutputType( word_t word, PartOfSpeechEnum singlePartOfSpeech )
        {
            switch ( singlePartOfSpeech )
            {
                case PartOfSpeechEnum.Adjective: 
                    switch ( word.posTaggerOutputType )
                    {
                        case PosTaggerOutputType.Adjective       :
                        case PosTaggerOutputType.AdjectivePronoun:
                            break;
                        default:
                            word.posTaggerOutputType = PosTaggerOutputType.Adjective;
                        break;
                    }                        
                break;
                case PartOfSpeechEnum.Adverb:
                    switch ( word.posTaggerOutputType )
                    {
                        case PosTaggerOutputType.Adverb          :
                        case PosTaggerOutputType.AdverbialPronoun:
                            break;
                        default:
                            word.posTaggerOutputType = PosTaggerOutputType.Adverb;
                        break;
                    }
                break;
                case PartOfSpeechEnum.Article     : word.posTaggerOutputType = PosTaggerOutputType.Article;      break;
                case PartOfSpeechEnum.Conjunction : word.posTaggerOutputType = PosTaggerOutputType.Conjunction;  break;
                case PartOfSpeechEnum.Interjection: word.posTaggerOutputType = PosTaggerOutputType.Interjection; break;
                case PartOfSpeechEnum.Noun        : word.posTaggerOutputType = PosTaggerOutputType.Noun;         break;
                case PartOfSpeechEnum.Numeral     : word.posTaggerOutputType = PosTaggerOutputType.Numeral;      break;
                case PartOfSpeechEnum.Other       : word.posTaggerOutputType = PosTaggerOutputType.Other;        break;
                case PartOfSpeechEnum.Particle    : word.posTaggerOutputType = PosTaggerOutputType.Particle;     break;
                case PartOfSpeechEnum.Predicate   : word.posTaggerOutputType = PosTaggerOutputType.Predicate;    break;
                case PartOfSpeechEnum.Preposition : word.posTaggerOutputType = PosTaggerOutputType.Preposition;  break;
                case PartOfSpeechEnum.Pronoun     :
                    switch ( word.posTaggerOutputType )
                    {
                        case PosTaggerOutputType.Pronoun          :
                        case PosTaggerOutputType.PossessivePronoun:
                        case PosTaggerOutputType.AdjectivePronoun :
                        case PosTaggerOutputType.AdverbialPronoun : 
                            break;
                        default:
                            word.posTaggerOutputType = PosTaggerOutputType.Pronoun; 
                        break;
                    }
                break;
                case PartOfSpeechEnum.Verb        : 
                    switch ( word.posTaggerOutputType )
                    {
                        case PosTaggerOutputType.Verb               :
                        case PosTaggerOutputType.Infinitive         :
                        case PosTaggerOutputType.AdverbialParticiple:
                        case PosTaggerOutputType.AuxiliaryVerb      :
                        case PosTaggerOutputType.Participle         :
                            break;
                        default:
                            word.posTaggerOutputType = PosTaggerOutputType.Verb; 
                        break;
                    }
                break;

                default:
                    throw (new ArgumentException(singlePartOfSpeech.ToString()));
            }
        }
        private  static PosTaggerOutputType ToPosTaggerOutputType( PartOfSpeechEnum singlePartOfSpeech )
        {
            switch ( singlePartOfSpeech )
            {
                case PartOfSpeechEnum.Adjective   : return (PosTaggerOutputType.Adjective);
                case PartOfSpeechEnum.Adverb      : return (PosTaggerOutputType.Adverb);
                case PartOfSpeechEnum.Article     : return (PosTaggerOutputType.Article);
                case PartOfSpeechEnum.Conjunction : return (PosTaggerOutputType.Conjunction);
                case PartOfSpeechEnum.Interjection: return (PosTaggerOutputType.Interjection);
                case PartOfSpeechEnum.Noun        : return (PosTaggerOutputType.Noun);
                case PartOfSpeechEnum.Numeral     : return (PosTaggerOutputType.Numeral);
                case PartOfSpeechEnum.Other       : return (PosTaggerOutputType.Other);
                case PartOfSpeechEnum.Particle    : return (PosTaggerOutputType.Particle);
                case PartOfSpeechEnum.Predicate   : return (PosTaggerOutputType.Predicate);
                case PartOfSpeechEnum.Preposition : return (PosTaggerOutputType.Preposition);
                case PartOfSpeechEnum.Pronoun     : return (PosTaggerOutputType.Pronoun);
                case PartOfSpeechEnum.Verb        : return (PosTaggerOutputType.Verb);

                default:
                    throw (new ArgumentException(singlePartOfSpeech.ToString()));
            }
        }
        internal static PartOfSpeechEnum?   ToPartOfSpeech       ( PosTaggerOutputType posTaggerOutputType )
        {
            switch ( posTaggerOutputType )
            {
                case PosTaggerOutputType.Adjective        :
                /*case PosTaggerOutputType.AdjectivePronoun :*/ return PartOfSpeechEnum.Adjective;
                case PosTaggerOutputType.Adverb           : 
                /*case PosTaggerOutputType.AdverbialPronoun :*/ return PartOfSpeechEnum.Adverb;
                case PosTaggerOutputType.Article          : return PartOfSpeechEnum.Article;
                case PosTaggerOutputType.Conjunction      : return PartOfSpeechEnum.Conjunction;
                case PosTaggerOutputType.Interjection     : return PartOfSpeechEnum.Interjection;
                case PosTaggerOutputType.Noun             : return PartOfSpeechEnum.Noun;
                case PosTaggerOutputType.Numeral          : return PartOfSpeechEnum.Numeral;
                case PosTaggerOutputType.Other            : return PartOfSpeechEnum.Other;
                case PosTaggerOutputType.Particle         : return PartOfSpeechEnum.Particle;
                case PosTaggerOutputType.Predicate        : return PartOfSpeechEnum.Predicate;
                case PosTaggerOutputType.Preposition      : return PartOfSpeechEnum.Preposition;

                case PosTaggerOutputType.Pronoun          : 
                case PosTaggerOutputType.PossessivePronoun:
                case PosTaggerOutputType.AdjectivePronoun :
                case PosTaggerOutputType.AdverbialPronoun : return PartOfSpeechEnum.Pronoun;

                case PosTaggerOutputType.Verb               : 
                case PosTaggerOutputType.Infinitive         :
                case PosTaggerOutputType.AdverbialParticiple:
                case PosTaggerOutputType.AuxiliaryVerb      :
                case PosTaggerOutputType.Participle         : return PartOfSpeechEnum.Verb;

                //default:
                    //throw (new ArgumentException(posTaggerOutputType.ToString()));
            }

            return (null);
        }

        #region [.description.]
        /*
        это на этапе морфо+теггер до снятия неоднозначности

        В случае наличия нескольких вариантов нормализации с разным положением регистра отбор кандидата  производить следующим образом:
        - если слово написано с [_не_заглавной_] буквы и это часть речи      NOUN; ADJECTIVE; ADVERB , то отбирать [_все_] варианты;
        - если слово написано с [_не_заглавной_] буквы и это часть речи _не_ NOUN; ADJECTIVE; ADVERB , то отбирать варианты с [_не_заглавной_] буквы;
        - если слово написано с [_Заглавной_]    буквы и это {_первое_} слово в предложении, то отбирать [_все_] варианты;
        - если слово написано с [_Заглавной_]    буквы и это часть речи      NOUN; ADJECTIVE; ADVERB и {_не_первое_} слово в предложении, то отбирать с [_заглавной_] буквы;
        - если слово написано с [_Заглавной_]    буквы и это часть речи _не_ NOUN; ADJECTIVE; ADVERB и {_не_первое_} слово в предложении, то отбирать [_все_] варианты;
        */
        #endregion        
        private static WordFormMorphologyModeEnum GetWordFormMorphologyMode( word_t word, int wordindex )
        {
            if ( wordindex == 0 )
            {
                return (WordFormMorphologyModeEnum.Default);
            }

            if ( word.posTaggerFirstCharIsUpper )
            {
                switch ( word.posTaggerOutputType )
                {
                    case PosTaggerOutputType.Noun:
                    case PosTaggerOutputType.Adjective:
                    case PosTaggerOutputType.Adverb:
                        return (WordFormMorphologyModeEnum.FirstStartsWithUpperAfterLowerLetter);

                    default:
                        return (WordFormMorphologyModeEnum.Default);
                }
            }
            else
            {
                return (WordFormMorphologyModeEnum.FirstStartsWithLowerAfterUpperLetter);
            }
        }  
        /*private static WordFormMorphologyModeEnum GetWordFormMorphologyMode( word_t word, bool wordFirstCharIsUpper, int wordindex )
        {
            if ( wordindex == 0 )
            {
                return (WordFormMorphologyModeEnum.Default);
            }

            if ( wordFirstCharIsUpper )
            {
                switch ( word.posTaggerOutputType )
                {
                    case PosTaggerOutputType.Noun:
                    case PosTaggerOutputType.Adjective:
                    case PosTaggerOutputType.Adverb:
                        return (WordFormMorphologyModeEnum.FirstStartsWithUpperAfterLowerLetter);

                    default:
                        return (WordFormMorphologyModeEnum.Default);
                }
            }
            else
            {
                return (WordFormMorphologyModeEnum.FirstStartsWithLowerAfterUpperLetter);
            }
        }        */
    }
}
