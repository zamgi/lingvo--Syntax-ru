using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using lingvo.core;
using lingvo.crfsuite;
using lingvo.morphology;
using lingvo.postagger;
using lingvo.tokenizing;

namespace lingvo.syntax
{
    /// <summary>
    /// 
    /// </summary>
    internal static class MA
    {
        public const byte U_BYTE = (byte) 'U';

        public static byte get_Case            ( MorphoAttributeEnum ma )
        {
            //var ma = word.morphology.MorphoAttribute;
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
            if ( (ma & MorphoAttributeEnum.Anycase) == MorphoAttributeEnum.Anycase )
            {
                return ((byte) 'H');
            }

            return (U_BYTE);
        }
        public static byte get_Number          ( MorphoAttributeEnum ma )
        {
            if ( (ma & MorphoAttributeEnum.Singular) == MorphoAttributeEnum.Singular )
            {
                return ((byte) 'S');
            }
            if ( (ma & MorphoAttributeEnum.Plural) == MorphoAttributeEnum.Plural )
            {
                return ((byte) 'P');
            }

            return (U_BYTE);
        }
        public static byte get_Mood            ( MorphoAttributeEnum ma )
        {
            if ( (ma & MorphoAttributeEnum.Imperative) == MorphoAttributeEnum.Imperative )
            {
                return ((byte) 'M');
            }
            if ( (ma & MorphoAttributeEnum.Indicative) == MorphoAttributeEnum.Indicative )
            {
                return ((byte) 'N');
            }
            if ( (ma & MorphoAttributeEnum.Subjunctive) == MorphoAttributeEnum.Subjunctive )
            {
                return ((byte) 'S');
            }
            if ( (ma & MorphoAttributeEnum.Personal) == MorphoAttributeEnum.Personal )
            {
                return ((byte) 'P');
            }
            if ( (ma & MorphoAttributeEnum.Impersonal) == MorphoAttributeEnum.Impersonal )
            {
                return ((byte) 'I');
            }
            if ( (ma & MorphoAttributeEnum.Gerund) == MorphoAttributeEnum.Gerund )
            {
                return ((byte) 'G');
            }
            if ( (ma & MorphoAttributeEnum.Participle) == MorphoAttributeEnum.Participle )
            {
                return ((byte) 'R');
            }

            return (U_BYTE);
        }
        public static byte get_Voice           ( MorphoAttributeEnum ma )
        {
            if ( (ma & MorphoAttributeEnum.Active) == MorphoAttributeEnum.Active )
            {
                return ((byte) 'A');
            }
            if ( (ma & MorphoAttributeEnum.Passive) == MorphoAttributeEnum.Passive )
            {
                return ((byte) 'P');
            }

            return (U_BYTE);
        }
        public static byte get_VerbTransitivity( MorphoAttributeEnum ma )
        {
            if ( (ma & MorphoAttributeEnum.Transitive) == MorphoAttributeEnum.Transitive )
            {
                return ((byte) 'T');
            }
            if ( (ma & MorphoAttributeEnum.Intransitive) == MorphoAttributeEnum.Intransitive )
            {
                return ((byte) 'I');
            }

            return (U_BYTE);
        }
        public static byte get_Form            ( MorphoAttributeEnum ma )
        {
            if ( (ma & MorphoAttributeEnum.Short) == MorphoAttributeEnum.Short )
            {
                return ((byte) 'S');
            }

            return (U_BYTE);
        }
        public static byte get_ConjunctionType ( MorphoAttributeEnum ma )
        {
            if ( (ma & MorphoAttributeEnum.Coordinating) == MorphoAttributeEnum.Coordinating )
            {
                return ((byte) 'C');
            }
            if ( (ma & MorphoAttributeEnum.Subordinating) == MorphoAttributeEnum.Subordinating )
            {
                return ((byte) 'S');
            }

            return (U_BYTE);
        }
        public static byte get_PronounType     ( MorphoAttributeEnum ma )
        {
            if ( (ma & MorphoAttributeEnum.Interrogative) == MorphoAttributeEnum.Interrogative )
            {
                return ((byte) 'I');
            }
            if ( (ma & MorphoAttributeEnum.InterrogativeRelative) == MorphoAttributeEnum.InterrogativeRelative )
            {
                return ((byte) 'R');
            }
            if ( (ma & MorphoAttributeEnum.Negative) == MorphoAttributeEnum.Negative )
            {
                return ((byte) 'N');
            }
            if ( (ma & MorphoAttributeEnum.Reflexive) == MorphoAttributeEnum.Reflexive )
            {
                return ((byte) 'F');
            }
            if ( (ma & MorphoAttributeEnum.Indefinitive1) == MorphoAttributeEnum.Indefinitive1 )
            {
                return ((byte) 'A');
            }
            if ( (ma & MorphoAttributeEnum.Indefinitive2) == MorphoAttributeEnum.Indefinitive2 )
            {
                return ((byte) 'B');
            }
            if ( (ma & MorphoAttributeEnum.Possessive) == MorphoAttributeEnum.Possessive )
            {
                return ((byte) 'P');
            }

            return (U_BYTE);
        }
        public static byte get_NounType        ( MorphoAttributeEnum ma )
        {
            if ( (ma & MorphoAttributeEnum.Proper) == MorphoAttributeEnum.Proper )
            {
                return ((byte) 'P');
            }
            if ( (ma & MorphoAttributeEnum.Common) == MorphoAttributeEnum.Common )
            {
                return ((byte) 'C');
            }

            return (U_BYTE);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ISyntaxScriber : IDisposable
    {
        void Run( IList< word_t > words );
    }


    /// <summary>
    /// Конвертор в формат CRF
    /// </summary>
    unsafe public sealed class SyntaxScriber_NoWords : ISyntaxScriber, IDisposable
	{
        /// <summary>
        /// 
        /// </summary>
        private struct PinnedWord_t
        {
            public MorphoAttributeEnum morphoAttribute;
            public PosTaggerInputType  posTaggerInputType;
            public PosTaggerOutputType posTaggerOutputType;
        }

        #region [.private field's.]
        private const byte VERTICAL_SLASH = (byte) '|';
        private const byte DASH           = (byte) '-';
        private const byte ZERO           = (byte) '\0';
        private const byte O              = (byte) 'O';        

        private const int UTF8_BUFFER_SIZE         = 1024 * 4;             //4KB
        private const int ATTRIBUTE_MAX_LENGTH     = UTF8_BUFFER_SIZE / 4; //1KB
        private const int PINNED_WORDS_BUFFER_SIZE = 100;
        private static readonly char[] ALLOWED_COLUMNNAMES = new[] { 's', 'z', 'y' };

		private readonly CRFTemplateFile _CrfTemplateFile;
        private IntPtr                   _Tagger;
        //private readonly byte[]          _AttributeBuffer;
        private readonly GCHandle        _AttributeBufferGCHandle;
        private byte*                    _AttributeBufferPtrBase;
        private byte*                    _AttributeBufferPtr;
        //private char[]                  _PinnedWordsBuffer;
        private int                      _PinnedWordsBufferSize;
        private GCHandle                 _PinnedWordsBufferGCHandle;
        private PinnedWord_t*            _PinnedWordsBufferPtrBase;
        //private IList< word_t >          _Words;
        #endregion

        #region [.ctor().]
        public SyntaxScriber_NoWords( string modelFilename, string templateFilename )
		{
            _CrfTemplateFile = CRFTemplateFileLoader.Load( templateFilename, ALLOWED_COLUMNNAMES );

            //-0-
            native.load_native_crf_suite();
            var ptr = Marshal.StringToHGlobalAnsi( modelFilename );			
            _Tagger = native.crf_tagger_initialize( ptr );
            Marshal.FreeHGlobal( ptr );

            if ( _Tagger == IntPtr.Zero )
            {
				throw (new InvalidDataException("Не удалось открыть CRF-модель."));
            }

            //-1-
            //_AttributeBuffer = new byte[ ATTRIBUTE_MAX_LENGTH + 1 ];
            var attributeBuffer      = new byte[ ATTRIBUTE_MAX_LENGTH + 1 ];
            _AttributeBufferGCHandle = GCHandle.Alloc( attributeBuffer, GCHandleType.Pinned );
            _AttributeBufferPtrBase  = (byte*) _AttributeBufferGCHandle.AddrOfPinnedObject().ToPointer();

            //-2-
            ReAllocPinnedWordsBuffer( PINNED_WORDS_BUFFER_SIZE );
		}

        private void ReAllocPinnedWordsBuffer( int newBufferSize )
        {
            DisposePinnedWordsBuffer();

            _PinnedWordsBufferSize     = newBufferSize;
            var pinnedWordsBuffer      = new PinnedWord_t[ _PinnedWordsBufferSize ];
            _PinnedWordsBufferGCHandle = GCHandle.Alloc( pinnedWordsBuffer, GCHandleType.Pinned );
            _PinnedWordsBufferPtrBase  = (PinnedWord_t*) _PinnedWordsBufferGCHandle.AddrOfPinnedObject().ToPointer();
        }
        private void DisposePinnedWordsBuffer()
        {
            if ( _PinnedWordsBufferPtrBase != null )
            {
                _PinnedWordsBufferGCHandle.Free();
                _PinnedWordsBufferPtrBase = null;
            }
        }

        ~SyntaxScriber_NoWords()
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
            if ( _Tagger != IntPtr.Zero )
            {
                native.crf_tagger_uninitialize( _Tagger );
                _Tagger = IntPtr.Zero;
            }

            if ( _AttributeBufferPtrBase != null )
            {
                _AttributeBufferGCHandle.Free();
                _AttributeBufferPtrBase = null;
            }

            DisposePinnedWordsBuffer();
        }
        #endregion

        public void Run( IList< word_t > words )
        {
            #region [.init.]
            if ( !Init( words ) )
            {
                return;
            }
            var wordsCount        = words.Count;
            var wordsCount_Minus1 = wordsCount - 1;
            #if DEBUG
                var sb_attr_debug = new StringBuilder();
            #endif
            #endregion

            native.crf_tagger_beginAddItemSequence( _Tagger );

            #region [.put-attr-values-to-crf.]
            for ( int wordIndex = 0; wordIndex < wordsCount; wordIndex++ )
			{
                native.crf_tagger_beginAddItemAttribute( _Tagger );

                #region [.process-crf-attributes-by-word.]
                native.crf_tagger_addItemAttributeNameOnly( _Tagger, xlat_Unsafe.Inst._PosInputtypeOtherPtrBase );
                #if DEBUG
                    sb_attr_debug.Append( PosTaggerInputType.O.ToText() ).Append( '\t' );
                #endif

                var ngrams = _CrfTemplateFile.GetCRFNgramsWhichCanTemplateBeApplied( wordIndex, wordsCount );
                for ( int i = 0, ngramsLength = ngrams.Length; i < ngramsLength; i++ )
                {
                    var ngram = ngrams[ i ];

                    _AttributeBufferPtr = ngram.CopyAttributesHeaderChars( _AttributeBufferPtrBase );

                    #region [.build attr-values.]
                    switch ( ngram.CRFAttributesLength )
                    {
                        case 1:
                        #region
                        {
                            AppendAttrValue( wordIndex, ngram.CRFAttribute_0 );		        
                        }
                        #endregion
                        break;

                        case 2:
                        #region
                        {
                            AppendAttrValue( wordIndex, ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                            AppendAttrValue( wordIndex, ngram.CRFAttribute_1 );
                        }
                        #endregion
                        break;

                        case 3:
                        #region
                        {
                            AppendAttrValue( wordIndex, ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                            AppendAttrValue( wordIndex, ngram.CRFAttribute_1 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                            AppendAttrValue( wordIndex, ngram.CRFAttribute_2 );
                        }
                        #endregion
                        break;

                        default:
                        #region
                        {
                            for ( var j = 0; j < ngram.CRFAttributesLength; j++ )
			                {
                                var crfAttr = ngram.CRFAttributes[ j ];
                                AppendAttrValue( wordIndex, crfAttr ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			                }
			                // Удалить последний '|'
                            _AttributeBufferPtr--;
                        }
                        #endregion
                        break;
                    }
                    #endregion

                    #region [.add-attr-values.]
                    *(_AttributeBufferPtr++) = ZERO;
                    native.crf_tagger_addItemAttributeNameOnly( _Tagger, _AttributeBufferPtrBase );
                    #if DEBUG
                        var attr_len_with_zero = (int) (_AttributeBufferPtr - _AttributeBufferPtrBase);
                        var s_debug = new string( (sbyte*) _AttributeBufferPtrBase, 0, attr_len_with_zero - 1 );
                        sb_attr_debug.Append( s_debug ).Append( '\t' );
                    #endif
                    #endregion
                }

                #region [.BOS-&-EOS.]
			    if ( wordIndex == 0 )
                {
                    native.crf_tagger_addItemAttributeNameOnly( _Tagger, xlat_Unsafe.Inst._BeginOfSentencePtrBase );
                    #if DEBUG
                        sb_attr_debug.Append( xlat_Unsafe.BEGIN_OF_SENTENCE ).Append( '\t' );
                    #endif
                }
                else 
                if ( wordIndex == wordsCount_Minus1 )
                {
                    native.crf_tagger_addItemAttributeNameOnly( _Tagger, xlat_Unsafe.Inst._EndOfSentencePtrBase );
                    #if DEBUG
                        sb_attr_debug.Append( xlat_Unsafe.END_OF_SENTENCE ).Append( '\t' );
                    #endif
                }
                #endregion
                #endregion

                native.crf_tagger_endAddItemAttribute( _Tagger );
                #if DEBUG
                    sb_attr_debug.Append( '\n' );
                #endif
			}
            #endregion

            native.crf_tagger_endAddItemSequence( _Tagger );
            #if DEBUG
                var attr_debug = sb_attr_debug.ToString();
            #endif

            #region [.run-crf-tagging-words.]
            native.crf_tagger_tag( _Tagger );
            #endregion

            #region [.get-crf-tagging-data.]
            for ( uint i = 0, len = native.crf_tagger_getResultLength( _Tagger ); i < len; i++ )
            {
                var ptr = native.crf_tagger_getResultValue( _Tagger, i );
                
                var value = (byte*) ptr.ToPointer();
                words[ (int) i ].syntaxRoleType = SyntaxExtensions.ToSyntaxRoleType( value );
            }
            #endregion

            #region [.un-init.]
            //Uninit();
            #endregion
        }

        private bool Init( IList< word_t > words )
        {
            if ( words.Count == 0 )
            {
                return (false);
            }

            //_Words = words;
            var wordsCount = words.Count;

            if ( _PinnedWordsBufferSize < wordsCount )
            {
                ReAllocPinnedWordsBuffer( wordsCount );
            }
            for ( var i = 0; i < wordsCount; i++ )
            {
                var word = words[ i ];
                PinnedWord_t* pw = _PinnedWordsBufferPtrBase + i;

                pw->posTaggerInputType  = word.posTaggerInputType;
                pw->posTaggerOutputType = word.posTaggerOutputType;
                pw->morphoAttribute     = word.morphology.MorphoAttribute;
            }

            return (true);
        }
        /*private void Uninit()
        {
            //_Words = null;
        }*/

        private void AppendAttrValue( int wordIndex, CRFAttribute crfAttribute )
        {
            /*
            s – часть речи;
            z – морфоатрибуты (для каждой части речи свои значения согласно таблице)
            y – искомое значение.             
            */

            switch ( crfAttribute.AttributeName )
            {
                //s – часть речи;
                case 's':
                #region
                {
                    var index = wordIndex + crfAttribute.Position;
                    *(_AttributeBufferPtr++) = (_PinnedWordsBufferPtrBase + index)->posTaggerOutputType.ToCrfByte();
                }
                #endregion
                break;

                //z – морфоатрибуты (для каждой части речи свои значения согласно таблице)
                case 'z':
                #region
                {
                    var index = wordIndex + crfAttribute.Position;
                    var pw = (_PinnedWordsBufferPtrBase + index);

                    switch ( pw->posTaggerOutputType )
                    {
                        case PosTaggerOutputType.Noun:
                        #region
                        {
                            var ma = pw->morphoAttribute;
                            *(_AttributeBufferPtr++) = MA.get_Case    ( ma );
                            *(_AttributeBufferPtr++) = MA.get_Number  ( ma );
                            *(_AttributeBufferPtr++) = MA.get_NounType( ma );
                        }
                        #endregion
                        break;

                        case PosTaggerOutputType.Verb:
                        case PosTaggerOutputType.Infinitive:
                        case PosTaggerOutputType.AdverbialParticiple:
                        case PosTaggerOutputType.AuxiliaryVerb:
                        case PosTaggerOutputType.Participle:
                        #region
                        {
                            var ma = pw->morphoAttribute;
                            *(_AttributeBufferPtr++) = MA.get_Case            ( ma );
                            *(_AttributeBufferPtr++) = MA.get_Number          ( ma );
                            *(_AttributeBufferPtr++) = MA.get_Mood            ( ma );
                            *(_AttributeBufferPtr++) = MA.get_Voice           ( ma );
                            *(_AttributeBufferPtr++) = MA.get_VerbTransitivity( ma );
                        }
                        #endregion
                        break;

                        case PosTaggerOutputType.Pronoun:
                        case PosTaggerOutputType.AdjectivePronoun:
                        case PosTaggerOutputType.PossessivePronoun:
                        case PosTaggerOutputType.AdverbialPronoun:
                        #region
                        {
                            var ma = pw->morphoAttribute;
                            *(_AttributeBufferPtr++) = MA.get_Case       ( ma );
                            *(_AttributeBufferPtr++) = MA.get_Number     ( ma );
                            *(_AttributeBufferPtr++) = MA.get_Form       ( ma );
                            *(_AttributeBufferPtr++) = MA.get_PronounType( ma );
                        }
                        #endregion
                        break;

                        case PosTaggerOutputType.Numeral:
                        #region
                        {
                            *(_AttributeBufferPtr++) = MA.get_Case( pw->morphoAttribute );
                        }
                        #endregion
                        break;

                        case PosTaggerOutputType.Adjective:
                        #region
                        {
                            var ma = pw->morphoAttribute;
                            *(_AttributeBufferPtr++) = MA.get_Case  ( ma );
                            *(_AttributeBufferPtr++) = MA.get_Number( ma );
                            *(_AttributeBufferPtr++) = MA.get_Form  ( ma );
                        }
                        #endregion
                        break;

                        case PosTaggerOutputType.Preposition:
                        #region
                        {
                            *(_AttributeBufferPtr++) = MA.get_Case( pw->morphoAttribute );
                        }
                        #endregion
                        break;

                        case PosTaggerOutputType.Conjunction:
                        #region
                        {
                            *(_AttributeBufferPtr++) = MA.get_ConjunctionType( pw->morphoAttribute );
                        }
                        #endregion
                        break;

                        default:
                        #region
                        {
                            *(_AttributeBufferPtr++) = MA.U_BYTE;
                        }
                        #endregion
                        break;
                    }
                }
                #endregion
                break;

                //y – искомое значение
                case 'y':
                #region
                {
                    *(_AttributeBufferPtr++) = O; //SINTAXINPUTTYPE_OTHER == "O"
                }
                #endregion
                break;

                #if DEBUG
                default: throw (new InvalidDataException( "Invalid column-name: '" + crfAttribute.AttributeName + "'" ));
                #endif
            }
        }

    }


    /// <summary>
    /// Конвертор в формат CRF
    /// </summary>
    unsafe public sealed class SyntaxScriber_Words : ISyntaxScriber, IDisposable
	{
        /// <summary>
        /// 
        /// </summary>
        private struct PinnedWord_t
        {
            public char*    basePtr;
            public GCHandle gcHandle;

            public MorphoAttributeEnum morphoAttribute;
            public int                 length;
            public PosTaggerInputType  posTaggerInputType;
            public PosTaggerOutputType posTaggerOutputType;            
        }

        #region [.private field's.]
        private const char VERTICAL_SLASH = '|';
        private const char SLASH          = '\\';
        private const char COLON          = ':';
        private const char DASH           = '-';
        private const char ZERO           = '\0';
        private const char O              = 'O';     

        private const int UTF8_BUFFER_SIZE         = 1024 * 100;           //100KB
        private const int ATTRIBUTE_MAX_LENGTH     = UTF8_BUFFER_SIZE / 4; //25KB
        private const int WORD_MAX_LENGTH          = 0x1000;               //4096-chars (4KB) - fusking-enough
        private const int PINNED_WORDS_BUFFER_SIZE = 100;
        private static readonly char[] ALLOWED_COLUMNNAMES = new[] { 'w', 's', 'z', 'y' };

        private static readonly Encoding UTF8_ENCODING = Encoding.UTF8;

		private readonly CRFTemplateFile _CrfTemplateFile;
        private IntPtr                   _Tagger;
        //private readonly byte[]          _UTF8Buffer;
        private readonly GCHandle        _UTF8BufferGCHandle;
        private byte*                    _UTF8BufferPtrBase;
        //private readonly char[]          _AttributeBuffer;
        private readonly GCHandle        _AttributeBufferGCHandle;
        private char*                    _AttributeBufferPtrBase;
        private char*                    _AttributeBufferPtr;
        //private char[]                  _PinnedWordsBuffer;
        private int                      _PinnedWordsBufferSize;
        private GCHandle                 _PinnedWordsBufferGCHandle;
        private PinnedWord_t*            _PinnedWordsBufferPtrBase;
        //private readonly char*           _UIM;
        //private IList< word_t >          _Words;
        //private int                      _WordsCount;
        #endregion

        #region [.ctor().]
        public SyntaxScriber_Words( string modelFilename, string templateFilename )
		{
            _CrfTemplateFile = CRFTemplateFileLoader.Load( templateFilename, ALLOWED_COLUMNNAMES );

            //-0-
            native.load_native_crf_suite();
            var ptr = Marshal.StringToHGlobalAnsi( modelFilename );			
            _Tagger = native.crf_tagger_initialize( ptr );
            Marshal.FreeHGlobal( ptr );

            if ( _Tagger == IntPtr.Zero )
            {
				throw (new InvalidDataException("Не удалось открыть CRF-модель."));
            }

            //-1-
            //_UIM = xlat_Unsafe.Inst._UPPER_INVARIANT_MAP;
            ReAllocPinnedWordsBuffer( PINNED_WORDS_BUFFER_SIZE );

            //-2-
            //_UTF8Buffer      = new byte[ UTF8_BUFFER_SIZE ];
            var utf8Buffer      = new byte[ UTF8_BUFFER_SIZE ];
            _UTF8BufferGCHandle = GCHandle.Alloc( utf8Buffer, GCHandleType.Pinned );
            _UTF8BufferPtrBase  = (byte*) _UTF8BufferGCHandle.AddrOfPinnedObject().ToPointer();

            //-3-
            //_AttributeBuffer = new char[ ATTRIBUTE_MAX_LENGTH + 1 ];
            var attributeBuffer      = new char[ ATTRIBUTE_MAX_LENGTH + 1 ];
            _AttributeBufferGCHandle = GCHandle.Alloc( attributeBuffer, GCHandleType.Pinned );
            _AttributeBufferPtrBase  = (char*) _AttributeBufferGCHandle.AddrOfPinnedObject().ToPointer();
		}

        private void ReAllocPinnedWordsBuffer( int newBufferSize )
        {
            DisposePinnedWordsBuffer();

            _PinnedWordsBufferSize     = newBufferSize;
            var pinnedWordsBuffer      = new PinnedWord_t[ _PinnedWordsBufferSize ];
            _PinnedWordsBufferGCHandle = GCHandle.Alloc( pinnedWordsBuffer, GCHandleType.Pinned );
            _PinnedWordsBufferPtrBase  = (PinnedWord_t*) _PinnedWordsBufferGCHandle.AddrOfPinnedObject().ToPointer();
        }
        private void DisposePinnedWordsBuffer()
        {
            if ( _PinnedWordsBufferPtrBase != null )
            {
                _PinnedWordsBufferGCHandle.Free();
                _PinnedWordsBufferPtrBase = null;
            }
        }

        ~SyntaxScriber_Words()
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
            if ( _Tagger != IntPtr.Zero )
            {
                native.crf_tagger_uninitialize( _Tagger );
                _Tagger = IntPtr.Zero;
            }

            if ( _AttributeBufferPtrBase != null )
            {
                _AttributeBufferGCHandle.Free();
                _AttributeBufferPtrBase = null;
            }

            if ( _UTF8BufferPtrBase != null )
            {
                _UTF8BufferGCHandle.Free();
                _UTF8BufferPtrBase = null;
            }

            DisposePinnedWordsBuffer();
        }
        #endregion

        public void Run( IList< word_t > words )
        {
            #region [.init.]
            if ( !Init( words ) )
            {
                return;
            }
            var wordsCount        = words.Count;
            var wordsCount_Minus1 = wordsCount - 1;
            #if DEBUG
                var sb_attr_debug = new StringBuilder();
            #endif
            #endregion

            native.crf_tagger_beginAddItemSequence( _Tagger );

            #region [.put-attr-values-to-crf.]
            for ( int wordIndex = 0; wordIndex < wordsCount; wordIndex++ )
			{
                native.crf_tagger_beginAddItemAttribute( _Tagger );

                #region [.process-crf-attributes-by-word.]
                native.crf_tagger_addItemAttributeNameOnly( _Tagger, xlat_Unsafe.Inst._PosInputtypeOtherPtrBase );
                #if DEBUG
                    sb_attr_debug.Append( PosTaggerInputType.O.ToText() ).Append( '\t' );
                #endif

                var ngrams = _CrfTemplateFile.GetCRFNgramsWhichCanTemplateBeApplied( wordIndex, wordsCount );
                for ( int i = 0, ngrams_len = ngrams.Length; i < ngrams_len; i++ )
                {
                    var ngram = ngrams[ i ];

                    _AttributeBufferPtr = ngram.CopyAttributesHeaderChars( _AttributeBufferPtrBase );

                    #region [.build attr-values.]
                    switch ( ngram.CRFAttributesLength )
                    {
                        case 1:
                        #region
                        {
                            AppendAttrValue( wordIndex, ngram.CRFAttribute_0 );		        
                        }
                        #endregion
                        break;

                        case 2:
                        #region
                        {
                            AppendAttrValue( wordIndex, ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                            AppendAttrValue( wordIndex, ngram.CRFAttribute_1 );
                        }
                        #endregion
                        break;

                        case 3:
                        #region
                        {
                            AppendAttrValue( wordIndex, ngram.CRFAttribute_0 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                            AppendAttrValue( wordIndex, ngram.CRFAttribute_1 ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
                            AppendAttrValue( wordIndex, ngram.CRFAttribute_2 );
                        }
                        #endregion
                        break;

                        default:
                        #region
                        {
                            for ( var j = 0; j < ngram.CRFAttributesLength; j++ )
			                {
                                var crfAttr = ngram.CRFAttributes[ j ];
                                AppendAttrValue( wordIndex, crfAttr ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			                }
			                // Удалить последний '|'
                            _AttributeBufferPtr--;
                        }
                        #endregion
                        break;
                    }
                    #endregion

                    #region [.add-attr-values.]
                    *(_AttributeBufferPtr++) = ZERO;
                    var attr_len_with_zero = Math.Min( ATTRIBUTE_MAX_LENGTH, (int) (_AttributeBufferPtr - _AttributeBufferPtrBase) );
                    UTF8_ENCODING.GetBytes( _AttributeBufferPtrBase, attr_len_with_zero, _UTF8BufferPtrBase, UTF8_BUFFER_SIZE ); //var bytesWritten = UTF8_ENCODER.GetBytes( attr_ptr, attr_len, utf8buffer, UTF8_BUFFER_SIZE, true ); 
                    native.crf_tagger_addItemAttributeNameOnly( _Tagger, _UTF8BufferPtrBase );
                    #if DEBUG
                        var s_debug = new string( _AttributeBufferPtrBase, 0, attr_len_with_zero - 1 );
                        sb_attr_debug.Append( s_debug ).Append( '\t' );
                    #endif
                    #endregion
                }

                #region [.BOS-&-EOS.]
			    if ( wordIndex == 0 )
                {
                    native.crf_tagger_addItemAttributeNameOnly( _Tagger, xlat_Unsafe.Inst._BeginOfSentencePtrBase );
                    #if DEBUG
                        sb_attr_debug.Append( xlat_Unsafe.BEGIN_OF_SENTENCE ).Append( '\t' );
                    #endif
                }
                else 
                if ( wordIndex == wordsCount_Minus1 )
                {
                    native.crf_tagger_addItemAttributeNameOnly( _Tagger, xlat_Unsafe.Inst._EndOfSentencePtrBase );
                    #if DEBUG
                        sb_attr_debug.Append( xlat_Unsafe.END_OF_SENTENCE ).Append( '\t' );
                    #endif
                }
                #endregion
                #endregion

                native.crf_tagger_endAddItemAttribute( _Tagger );
                #if DEBUG
                    sb_attr_debug.Append( '\n' );
                #endif
			}
            #endregion

            native.crf_tagger_endAddItemSequence( _Tagger );
            #if DEBUG
                var attr_debug = sb_attr_debug.ToString();
            #endif

            #region [.run-crf-tagging-words.]
            native.crf_tagger_tag( _Tagger );
            #endregion

            #region [.get-crf-tagging-data.]
            System.Diagnostics.Debug.Assert( native.crf_tagger_getResultLength( _Tagger ) == wordsCount, "(native.crf_tagger_getResultLength( _Tagger ) != _WordsCount)" );
            for ( var i = 0; i < wordsCount; i++ )
            {
                var ptr = native.crf_tagger_getResultValue( _Tagger, (uint) i );
                
                var value = (byte*) ptr.ToPointer();
                words[ i ].syntaxRoleType = SyntaxExtensions.ToSyntaxRoleType( value );

                //free pinned-gcHandle
                (_PinnedWordsBufferPtrBase + i)->gcHandle.Free();
            }
            #endregion

            #region [.un-init.]
            //Uninit();
            #endregion
        }

        private bool Init( IList< word_t > words )
        {
            if ( words.Count == 0 )
            {
                return (false);
            }

            //_Words = words;
            var wordsCount = words.Count;

            if ( _PinnedWordsBufferSize < wordsCount )
            {
                ReAllocPinnedWordsBuffer( wordsCount );
            }
            for ( var i = 0; i < wordsCount; i++ )
            {
                PinnedWord_t* pw        = _PinnedWordsBufferPtrBase + i;
                var word                = words[ i ];
                var valueUpper          = (word.posTaggerLastValueUpperInNumeralChain == null)
                                          ? word.valueUpper : word.posTaggerLastValueUpperInNumeralChain;
                var gcHandle            = GCHandle.Alloc( valueUpper, GCHandleType.Pinned );
                var basePtr             = (char*) gcHandle.AddrOfPinnedObject().ToPointer();                
                pw->basePtr             = basePtr;
                pw->gcHandle            = gcHandle;
                pw->posTaggerInputType  = word.posTaggerInputType;
                pw->posTaggerOutputType = word.posTaggerOutputType;
                pw->morphoAttribute     = word.morphology.MorphoAttribute;
                pw->length              = valueUpper.Length;
            }

            return (true);
        }
        /*private void Uninit()
        {
            for ( var i = 0; i < _WordsCount; i++ )
            {
                (_PinnedWordsBufferPtrBase + i)->gcHandle.Free();
            }
            //_Words = null;
        }*/

        private void AppendAttrValue( int wordIndex, CRFAttribute crfAttribute )
        {
            /*
            w – слово
            s – часть речи;
            z – морфоатрибуты (для каждой части речи свои значения согласно таблице)
            y – искомое значение.             
            */

            switch ( crfAttribute.AttributeName )
            {
                //w – слово
                case 'w':
                #region
                {
                    /*
                    символы ':' '\'
                    - их комментировать в поле "w", "\:" и "\\"
                    */
                    var index = wordIndex + crfAttribute.Position;
                    var pw = (_PinnedWordsBufferPtrBase + index);
                    //':'
                    if ( pw->posTaggerInputType == PosTaggerInputType.Col )
                    {
                        *(_AttributeBufferPtr++) = SLASH;
                        *(_AttributeBufferPtr++) = COLON;
                    }
                    else
                    {
                        char* _base = pw->basePtr;
                        switch ( *_base )
                        {
                            case SLASH:
                                *(_AttributeBufferPtr++) = SLASH;
                                *(_AttributeBufferPtr++) = SLASH;
                            break;

                            default:                                
                                //---System.Diagnostics.Debug.Assert( word.valueOriginal.Length <= WORD_MAX_LENGTH );
                                //---System.Diagnostics.Debug.Assert( word.length == word.valueOriginal.Length );
                                for ( int i = 0, len = Math.Min( WORD_MAX_LENGTH, pw->length ); i < len; i++ )
                                {
                                    *(_AttributeBufferPtr++) = *(_base + i);
                                }
                                #region commented
                                /*
                                for ( int i = 0; i < WORD_MAX_LENGTH; i++ )
                                {
                                    var ch = *(_base + i);
                                    if ( ch == '\0' )
                                        break;
                                    *(_AttributeBufferPtr++) = ch;
                                }
                                */
                                #endregion
                            break;
                        }
                    }
                }
                #endregion
                break;

                //s – часть речи;
                case 's':
                #region
                {
                    var index = wordIndex + crfAttribute.Position;
                    *(_AttributeBufferPtr++) = (_PinnedWordsBufferPtrBase + index)->posTaggerOutputType.ToCrfChar();
                }
                #endregion
                break;

                //z – морфоатрибуты (для каждой части речи свои значения согласно таблице)
                case 'z':
                #region
                {
                    var index = wordIndex + crfAttribute.Position;
                    var pw = (_PinnedWordsBufferPtrBase + index);

                    switch ( pw->posTaggerOutputType )
                    {
                        case PosTaggerOutputType.Noun:
                        #region
                        {
                            var ma = pw->morphoAttribute;
                            *(_AttributeBufferPtr++) = (char) MA.get_Case    ( ma );
                            *(_AttributeBufferPtr++) = (char) MA.get_Number  ( ma );
                            *(_AttributeBufferPtr++) = (char) MA.get_NounType( ma );
                        }
                        #endregion
                        break;

                        case PosTaggerOutputType.Verb:
                        case PosTaggerOutputType.Infinitive:
                        case PosTaggerOutputType.AdverbialParticiple:
                        case PosTaggerOutputType.AuxiliaryVerb:
                        case PosTaggerOutputType.Participle:
                        #region
                        {
                            var ma = pw->morphoAttribute;
                            *(_AttributeBufferPtr++) = (char) MA.get_Case            ( ma );
                            *(_AttributeBufferPtr++) = (char) MA.get_Number          ( ma );
                            *(_AttributeBufferPtr++) = (char) MA.get_Mood            ( ma );
                            *(_AttributeBufferPtr++) = (char) MA.get_Voice           ( ma );
                            *(_AttributeBufferPtr++) = (char) MA.get_VerbTransitivity( ma );
                        }
                        #endregion
                        break;

                        case PosTaggerOutputType.Pronoun:
                        case PosTaggerOutputType.AdjectivePronoun:
                        case PosTaggerOutputType.PossessivePronoun:
                        case PosTaggerOutputType.AdverbialPronoun:
                        #region
                        {
                            var ma = pw->morphoAttribute;
                            *(_AttributeBufferPtr++) = (char) MA.get_Case       ( ma );
                            *(_AttributeBufferPtr++) = (char) MA.get_Number     ( ma );
                            *(_AttributeBufferPtr++) = (char) MA.get_Form       ( ma );
                            *(_AttributeBufferPtr++) = (char) MA.get_PronounType( ma );
                        }
                        #endregion
                        break;

                        case PosTaggerOutputType.Numeral:
                        #region
                        {
                            *(_AttributeBufferPtr++) = (char) MA.get_Case( pw->morphoAttribute );
                        }
                        #endregion
                        break;

                        case PosTaggerOutputType.Adjective:
                        #region
                        {
                            var ma = pw->morphoAttribute;
                            *(_AttributeBufferPtr++) = (char) MA.get_Case  ( ma );
                            *(_AttributeBufferPtr++) = (char) MA.get_Number( ma );
                            *(_AttributeBufferPtr++) = (char) MA.get_Form  ( ma );
                        }
                        #endregion
                        break;

                        case PosTaggerOutputType.Preposition:
                        #region
                        {
                            *(_AttributeBufferPtr++) = (char) MA.get_Case( pw->morphoAttribute );
                        }
                        #endregion
                        break;

                        case PosTaggerOutputType.Conjunction:
                        #region
                        {
                            *(_AttributeBufferPtr++) = (char) MA.get_ConjunctionType( pw->morphoAttribute );
                        }
                        #endregion
                        break;

                        default:
                        #region
                        {
                            *(_AttributeBufferPtr++) = (char) MA.U_BYTE;
                        }
                        #endregion
                        break;
                    }
                }
                #endregion
                break;

                //y – искомое значение
                case 'y':
                #region
                {
                    *(_AttributeBufferPtr++) = O; //SINTAXINPUTTYPE_OTHER == "O"
                }
                #endregion
                break;

                #if DEBUG
                default: throw (new InvalidDataException( "Invalid column-name: '" + crfAttribute.AttributeName + "'" ));
                #endif
            }
        }

    }
}



