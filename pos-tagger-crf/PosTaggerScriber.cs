using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using lingvo.core;
using lingvo.crfsuite;
using lingvo.tokenizing;

using M = System.Runtime.CompilerServices.MethodImplAttribute;
using O = System.Runtime.CompilerServices.MethodImplOptions;

namespace lingvo.postagger
{
    /// <summary>
    /// Конвертор в формат CRF
    /// </summary>
    unsafe public sealed class PosTaggerScriber : IDisposable
	{
        /// <summary>
        /// 
        /// </summary>
        private struct PinnedWord_t
        {
            public char*    basePtr;
            public GCHandle gcHandle;

            public int                    length;
            public PosTaggerInputType     posTaggerInputType;
            public PosTaggerExtraWordType posTaggerExtraWordType;            
        }

        #region [.private field's.]
        private const char VERTICAL_SLASH = '|';
        private const char SLASH          = '\\';
        private const char COLON          = ':';
        private const char DASH           = '-';

        private const int UTF8_BUFFER_SIZE         = 1024 * 100;           //100KB
        private const int ATTRIBUTE_MAX_LENGTH     = UTF8_BUFFER_SIZE / 4; //25KB
        private const int WORD_MAX_LENGTH          = 0x1000;               //4096-chars (4KB) - fusking-enough
        private const int PINNED_WORDS_BUFFER_SIZE = 100;
        private static readonly char[] ALLOWED_COLUMNNAMES = new[] { 'w', 'a', 'b', 'c', 'd', 'e', 'z', 'y' };

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
        private readonly List< string >  _Result4ModelBuilder;
        //private int                      _WordsCount;
        //private int                      _WordsCount_Minus1;
        //private List< word_t >           _Words;
        //private readonly char*           _UIM;
        #endregion

        #region [.ctor().]
        private PosTaggerScriber( string modelFilename, string templateFilename )
		{
            _CrfTemplateFile = CRFTemplateFileLoader.Load( templateFilename, ALLOWED_COLUMNNAMES );

            //-0-
            if ( !Native._crf_tagger_initialize_( modelFilename, out _Tagger ) )
            {
				throw (new InvalidDataException( "Failed to open CRF-model." ));
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
        private PosTaggerScriber( string templateFilename )
		{
            _CrfTemplateFile = CRFTemplateFileLoader.Load( templateFilename, ALLOWED_COLUMNNAMES );

            _Result4ModelBuilder = new List< string >();

            //-1-
            //_UIM = xlat_Unsafe.Inst._UPPER_INVARIANT_MAP;
            ReAllocPinnedWordsBuffer( PINNED_WORDS_BUFFER_SIZE );

            //-2-
            //_AttributeBuffer = new char[ ATTRIBUTE_MAX_LENGTH + 1 ];
            var attributeBuffer      = new char[ ATTRIBUTE_MAX_LENGTH + 1 ];
            _AttributeBufferGCHandle = GCHandle.Alloc( attributeBuffer, GCHandleType.Pinned );
            _AttributeBufferPtrBase  = (char*) _AttributeBufferGCHandle.AddrOfPinnedObject().ToPointer();
		}

        public static PosTaggerScriber Create( string modelFilename, string templateFilename ) => new PosTaggerScriber( modelFilename, templateFilename );
        public static PosTaggerScriber Create4ModelBuilder( string templateFilename ) => new PosTaggerScriber( templateFilename );

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

        ~PosTaggerScriber()
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
                Native.crf_tagger_uninitialize( _Tagger );
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

        public void Run( List< word_t > words )
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

            Native.crf_tagger_beginAddItemSequence( _Tagger );

            #region [.put-attr-values-to-crf.]
            for ( var wordIndex = 0; wordIndex < wordsCount; wordIndex++ )
            {
                #region [.commented. debug-assert.]
                /*
                var _w = _Words[ wordIndex ];
                System.Diagnostics.Debug.Assert( _w.valueUpper.ToUpperInvariant() == _w.valueUpper
                                               , "(_w.valueUpper.ToUpperInvariant() != _w.valueUpper) => '" +
                                                 _w.valueOriginal + '\'' );
                
                System.Diagnostics.Debug.Assert( (_w.valueUpper != null) &&
                                                 (_w.valueOriginal.ToUpperInvariant() == _w.valueUpper)
                                               , "(_w.valueUpper == null) || " +
                                                 "(_w.valueOriginal.ToUpperInvariant() != _w.valueUpper) => '" +
                                                 _w.valueOriginal + '\'' );
                */
                #endregion

                Native.crf_tagger_beginAddItemAttribute( _Tagger );

                #region [.process-crf-attributes-by-word.]
                Native.crf_tagger_addItemAttributeNameOnly( _Tagger, xlat_Unsafe.Inst._PosInputtypeOtherPtrBase );
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
                    *(_AttributeBufferPtr++) = '\0';
                    var attr_len_with_zero = Math.Min( ATTRIBUTE_MAX_LENGTH, (int) (_AttributeBufferPtr - _AttributeBufferPtrBase) );
                    UTF8_ENCODING.GetBytes( _AttributeBufferPtrBase, attr_len_with_zero, _UTF8BufferPtrBase, UTF8_BUFFER_SIZE ); //var bytesWritten = UTF8_ENCODER.GetBytes( attr_ptr, attr_len, utf8buffer, UTF8_BUFFER_SIZE, true ); 
                    Native.crf_tagger_addItemAttributeNameOnly( _Tagger, _UTF8BufferPtrBase );
#if DEBUG
                    var s_debug = new string( _AttributeBufferPtrBase, 0, attr_len_with_zero - 1 );
                    sb_attr_debug.Append( s_debug ).Append( '\t' );
#endif
                    #endregion
                }

                #region [.BOS-&-EOS.]
                if ( wordIndex == 0 )
                {
                    Native.crf_tagger_addItemAttributeNameOnly( _Tagger, xlat_Unsafe.Inst._BeginOfSentencePtrBase );
#if DEBUG
                    sb_attr_debug.Append( xlat_Unsafe.BEGIN_OF_SENTENCE ).Append( '\t' );
#endif
                }
                else 
                if ( wordIndex == wordsCount_Minus1 )
                {
                    Native.crf_tagger_addItemAttributeNameOnly( _Tagger, xlat_Unsafe.Inst._EndOfSentencePtrBase );
#if DEBUG
                    sb_attr_debug.Append( xlat_Unsafe.END_OF_SENTENCE ).Append( '\t' );
#endif
                }
                #endregion
                #endregion

                Native.crf_tagger_endAddItemAttribute( _Tagger );
#if DEBUG
                sb_attr_debug.Append( '\n' );
#endif
            }
            #endregion

            Native.crf_tagger_endAddItemSequence( _Tagger );
#if DEBUG
            var attr_debug = sb_attr_debug.ToString();
#endif

            #region [.run-crf-tagging-words.]
            Native.crf_tagger_tag( _Tagger );
            #endregion

            #region [.get-crf-tagging-data.]
            System.Diagnostics.Debug.Assert( Native.crf_tagger_getResultLength( _Tagger ) == wordsCount, "(native.crf_tagger_getResultLength( _Tagger ) != _WordsCount)" );
            for ( var i = 0; i < wordsCount; i++ )
            {
                var ptr = Native.crf_tagger_getResultValue( _Tagger, (uint) i );
                
                var value = (byte*) ptr.ToPointer();
                words[ i ].posTaggerOutputType = PosTaggerExtensions.ToPosTaggerOutputType( value );

                //free pinned-gcHandle
                (_PinnedWordsBufferPtrBase + i)->gcHandle.Free();
            }
            #endregion

            #region [.un-init.]
            //Uninit();
            #endregion
        }

        private bool Init( List< word_t > words )
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
                var word     = words[ i ];
                var gcHandle = GCHandle.Alloc( word.valueUpper, GCHandleType.Pinned );
                var basePtr  = (char*) gcHandle.AddrOfPinnedObject().ToPointer();
                PinnedWord_t* pw = _PinnedWordsBufferPtrBase + i;
                pw->basePtr  = basePtr;
                pw->gcHandle = gcHandle;

                pw->posTaggerInputType     = word.posTaggerInputType;
                pw->posTaggerExtraWordType = word.posTaggerExtraWordType;
                pw->length                 = word.valueUpper.Length;
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

        [M(O.AggressiveInlining)] private void AppendAttrValue( int wordIndex, CRFAttribute attr )
        {
            switch ( attr.AttributeName )
            {
                //w – слово или словосочетание
                case 'w':
                #region
                {
                    /*
                    символы ':' '\'
                    - их комментировать в поле "w", "\:" и "\\"
                    */
                    var index = wordIndex + attr.Position;
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
                                //---System.Diagnostics.Debug.Assert( word.valueOriginal.Length <= WORD_MAX_LENGTH && word.valueUpper.Length <= WORD_MAX_LENGTH );
                                //---System.Diagnostics.Debug.Assert( word.length == word.valueOriginal.Length && word.length == word.valueUpper.Length );
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

                //g\a – первая буква с конца
                case 'a':
                #region
                {
                    var index = wordIndex + attr.Position;
                    var pw = (_PinnedWordsBufferPtrBase + index);

                    if ( pw->posTaggerExtraWordType == PosTaggerExtraWordType.Punctuation )
                    {
                        *(_AttributeBufferPtr++) = DASH;
                        break;
                    }

                    var len = pw->length - 1;
                    *(_AttributeBufferPtr++) = *(pw->basePtr + len);
                }
                #endregion
                break;

                //ng\b – две буква с конца
                case 'b':
                #region
                {
                    var index = wordIndex + attr.Position;
                    var pw = (_PinnedWordsBufferPtrBase + index);

                    if ( pw->posTaggerExtraWordType == PosTaggerExtraWordType.Punctuation )
                    {
                        *(_AttributeBufferPtr++) = DASH;
                        break;
                    }

                    var len = pw->length - 2;
                    if ( 0 <= len )
                    {                        
                        char* _base = pw->basePtr;
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len);
                    }
                    else
                    {
                        *(_AttributeBufferPtr++) = DASH;
                    }
                }
                #endregion
                break;

                //ing\c – три буква с конца
                case 'c':
                #region
                {
                    var index = wordIndex + attr.Position;
                    var pw = (_PinnedWordsBufferPtrBase + index);

                    if ( pw->posTaggerExtraWordType == PosTaggerExtraWordType.Punctuation )
                    {
                        *(_AttributeBufferPtr++) = DASH;
                        break;
                    }

                    var len = pw->length - 3;
                    if ( 0 <= len )
                    {
                        char* _base = pw->basePtr;
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len);
                    }
                    else
                    {
                        *(_AttributeBufferPtr++) = DASH;
                    }
                }
                #endregion
                break;

                //ding\d – четыре буква с конца
                case 'd':
                #region
                {
                    var index = wordIndex + attr.Position;
                    var pw = (_PinnedWordsBufferPtrBase + index);

                    if ( pw->posTaggerExtraWordType == PosTaggerExtraWordType.Punctuation )
                    {
                        *(_AttributeBufferPtr++) = DASH;
                        break;
                    }

                    var len = pw->length - 4;
                    if ( 0 <= len )
                    {
                        char* _base = pw->basePtr;
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len);
                    }
                    else
                    {
                        *(_AttributeBufferPtr++) = DASH;
                    }
                }
                #endregion
                break;

                //nding\e – пять букв с конца
                case 'e':
                #region
                {
                    var index = wordIndex + attr.Position;
                    var pw = (_PinnedWordsBufferPtrBase + index);

                    if ( pw->posTaggerExtraWordType == PosTaggerExtraWordType.Punctuation )
                    {
                        *(_AttributeBufferPtr++) = DASH;
                        break;
                    }

                    var len = pw->length - 5;
                    if ( 0 <= len )
                    {
                        char* _base = pw->basePtr;
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len);
                    }
                    else
                    {
                        *(_AttributeBufferPtr++) = DASH;
                    }
                }
                #endregion
                break;

                //atr\z – список атрибутов (дан ниже)
                case 'z':
                #region
                {
                    var index = wordIndex + attr.Position;
                    *(_AttributeBufferPtr++) = (_PinnedWordsBufferPtrBase + index)->posTaggerInputType.ToCrfChar();
                }
                #endregion
                break;

                //y – искомое значение
                case 'y':
                #region
                {
                    *(_AttributeBufferPtr++) = 'O'; //POSINPUTTYPE_OTHER == "O"
                }
                #endregion
                break;

#if DEBUG
                default: throw (new InvalidDataException( "Invalid column-name: '" + attr.AttributeName + "'" ));
#endif
            }
        }
        /*private void AppendAttrValue__previous( int wordIndex, CRFAttribute crfAttribute )
        {
            switch ( crfAttribute.AttributeName )
            {
                //w – слово или словосочетание
                case 'w':
                #region
                {
                    /*
                    символы ':' '\'
                    - их комментировать в поле "w", "\:" и "\\"
                    * /
                    var index = wordIndex + crfAttribute.Position;
                    var word = _Words[ index ];
                    //':'
                    if ( word.posTaggerInputType == PosTaggerInputType.Col )
                    {
                        *(_AttributeBufferPtr++) = SLASH;
                        *(_AttributeBufferPtr++) = COLON;
                    }
                    else
                    {
                        char* _base = (_PinnedWordsBufferPtrBase + index)->basePtr;
                        switch ( *_base )
                        {
                            case SLASH:
                                *(_AttributeBufferPtr++) = SLASH;
                                *(_AttributeBufferPtr++) = SLASH;
                            break;

                            default:
                                System.Diagnostics.Debug.Assert( word.valueOriginal.Length <= WORD_MAX_LENGTH && word.valueUpper.Length <= WORD_MAX_LENGTH );
                                //---System.Diagnostics.Debug.Assert( word.length == word.valueOriginal.Length && word.length == word.valueUpper.Length );
                                for ( int i = 0; i < WORD_MAX_LENGTH; i++ )
                                {
                                    var ch = *(_base + i);
                                    if ( ch == '\0' )
                                        break;
                                    *(_AttributeBufferPtr++) = ch;
                                }
                            break;
                        }
                    }
                }
                #endregion
                break;

                //g\a – первая буква с конца
                case 'a':
                #region
                {
                    var index = wordIndex + crfAttribute.Position;
                    var word = _Words[ index ];

                    if ( word.posTaggerExtraWordType == PosTaggerExtraWordType.Punctuation )
                    {
                        *(_AttributeBufferPtr++) = DASH;
                        break;
                    }

                    var len = word.valueUpper.Length - 1;
                    *(_AttributeBufferPtr++) = *((_PinnedWordsBufferPtrBase + index)->basePtr + len);
                }
                #endregion
                break;

                //ng\b – две буква с конца
                case 'b':
                #region
                {
                    var index = wordIndex + crfAttribute.Position;
                    var word = _Words[ index ];

                    if ( word.posTaggerExtraWordType == PosTaggerExtraWordType.Punctuation )
                    {
                        *(_AttributeBufferPtr++) = DASH;
                        break;
                    }
                    
                    var len = word.valueUpper.Length - 2;
                    if ( 0 <= len )
                    {                        
                        char* _base = (_PinnedWordsBufferPtrBase + index)->basePtr;
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len);
                    }
                    else
                    {
                        *(_AttributeBufferPtr++) = DASH;
                    }
                }
                #endregion
                break;

                //ing\c – три буква с конца
                case 'c':
                #region
                {
                    var index = wordIndex + crfAttribute.Position;
                    var word = _Words[ index ];

                    if ( word.posTaggerExtraWordType == PosTaggerExtraWordType.Punctuation )
                    {
                        *(_AttributeBufferPtr++) = DASH;
                        break;
                    }

                    var len = word.valueUpper.Length - 3;
                    if ( 0 <= len )
                    {
                        char* _base = (_PinnedWordsBufferPtrBase + index)->basePtr;
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len);
                    }
                    else
                    {
                        *(_AttributeBufferPtr++) = DASH;
                    }
                }
                #endregion
                break;

                //ding\d – четыре буква с конца
                case 'd':
                #region
                {
                    var index = wordIndex + crfAttribute.Position;
                    var word = _Words[ index ];

                    if ( word.posTaggerExtraWordType == PosTaggerExtraWordType.Punctuation )
                    {
                        *(_AttributeBufferPtr++) = DASH;
                        break;
                    }

                    var len = word.valueUpper.Length - 4;
                    if ( 0 <= len )
                    {
                        char* _base = (_PinnedWordsBufferPtrBase + index)->basePtr;
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len);
                    }
                    else
                    {
                        *(_AttributeBufferPtr++) = DASH;
                    }
                }
                #endregion
                break;

                //nding\e – пять букв с конца
                case 'e':
                #region
                {
                    var index = wordIndex + crfAttribute.Position;
                    var word = _Words[ index ];

                    if ( word.posTaggerExtraWordType == PosTaggerExtraWordType.Punctuation )
                    {
                        *(_AttributeBufferPtr++) = DASH;
                        break;
                    }

                    var len = word.valueUpper.Length - 5;
                    if ( 0 <= len )
                    {
                        char* _base = (_PinnedWordsBufferPtrBase + index)->basePtr;
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len++);
                        *(_AttributeBufferPtr++) = *(_base + len);
                    }
                    else
                    {
                        *(_AttributeBufferPtr++) = DASH;
                    }
                }
                #endregion
                break;

                //atr\z – список атрибутов (дан ниже)
                case 'z':
                #region
                {
                    var index = wordIndex + crfAttribute.Position;
                    *(_AttributeBufferPtr++) = _Words[ index ].posTaggerInputType.ToCrfChar();
                }
                #endregion
                break;

                //y – искомое значение
                case 'y':
                #region
                {
                    *(_AttributeBufferPtr++) = 'O'; //POSINPUTTYPE_OTHER == "O"
                }
                #endregion
                break;

                //default: throw (new ArgumentException("AttributeName: " + crfAttribute.AttributeName));
            }
        }*/
        /*private void AppendAttrValue__previous__previous( int wordIndex, CRFAttribute crfAttribute )
        {
            switch ( crfAttribute.AttributeName )
            {
                //w – слово или словосочетание
                case 'w':
                #region
                {
                    /*
                    символы ':' '\'
                    - их комментировать в поле "w", "\:" и "\\"
                    * /
                    var index = wordIndex + crfAttribute.Position;
                    var word = _Words[ index ];
                    //':'
                    if ( word.posTaggerInputType == PosTaggerInputType.Col )
                    {
                        *(_AttributeBufferPtr++) = SLASH;
                        *(_AttributeBufferPtr++) = COLON;
                    }
                    else
                    {
                        fixed ( char* _base = word.valueOriginal )
                        {
                            switch ( *_base )
                            {
                                case SLASH:
                                    *(_AttributeBufferPtr++) = SLASH;
                                    *(_AttributeBufferPtr++) = SLASH;
                                break;

                                default:                                
                                    System.Diagnostics.Debug.Assert( word.valueOriginal.Length <= WORD_MAX_LENGTH );
                                    //---System.Diagnostics.Debug.Assert( word.length == word.valueOriginal.Length );
                                    for ( int i = 0; i < WORD_MAX_LENGTH; i++ )
                                    {
                                        var ch = *(_base + i);
                                        if ( ch == '\0' )
                                            break;
                                        *(_AttributeBufferPtr++) = *(_UIM + ch);
                                    }
                                break;
                            }
                        }
                    }
                }
                #endregion
                break;

                //g\a – первая буква с конца
                case 'a':
                #region
                {
                    var index = wordIndex + crfAttribute.Position;
                    var w = _Words[ index ];

                    if ( w.posTaggerExtraWordType == PosTaggerExtraWordType.Punctuation )
                    {
                        *(_AttributeBufferPtr++) = DASH;
                        break;
                    }

                    var len = w.valueOriginal.Length - 1;
                    fixed ( char* _base = w.valueOriginal )
                    {
                        *(_AttributeBufferPtr++) = *(_UIM + *(_base + len));
                    }
                }
                #endregion
                break;

                //ng\b – две буква с конца
                case 'b':
                #region
                {
                    var index = wordIndex + crfAttribute.Position;
                    var w = _Words[ index ];

                    if ( w.posTaggerExtraWordType == PosTaggerExtraWordType.Punctuation )
                    {
                        *(_AttributeBufferPtr++) = DASH;
                        break;
                    }
                    
                    var len = w.valueOriginal.Length - 2;
                    if ( 0 <= len )
                    {                        
                        fixed ( char* _base = w.valueOriginal )
                        {
                            *(_AttributeBufferPtr++) = *(_UIM + *(_base + len++));
                            *(_AttributeBufferPtr++) = *(_UIM + *(_base + len));
                        }
                    }
                    else
                    {
                        *(_AttributeBufferPtr++) = DASH;
                    }
                }
                #endregion
                break;

                //ing\c – три буква с конца
                case 'c':
                #region
                {
                    var index = wordIndex + crfAttribute.Position;
                    var w = _Words[ index ];

                    if ( w.posTaggerExtraWordType == PosTaggerExtraWordType.Punctuation )
                    {
                        *(_AttributeBufferPtr++) = DASH;
                        break;
                    }

                    var len = w.valueOriginal.Length - 3;
                    if ( 0 <= len )
                    {
                        fixed ( char* _base = w.valueOriginal )
                        {
                            *(_AttributeBufferPtr++) = *(_UIM + *(_base + len++));
                            *(_AttributeBufferPtr++) = *(_UIM + *(_base + len++));
                            *(_AttributeBufferPtr++) = *(_UIM + *(_base + len));
                        }
                    }
                    else
                    {
                        *(_AttributeBufferPtr++) = DASH;
                    }
                }
                #endregion
                break;

                //ding\d – четыре буква с конца
                case 'd':
                #region
                {
                    var index = wordIndex + crfAttribute.Position;
                    var w = _Words[ index ];

                    if ( w.posTaggerExtraWordType == PosTaggerExtraWordType.Punctuation )
                    {
                        *(_AttributeBufferPtr++) = DASH;
                        break;
                    }

                    var len = w.valueOriginal.Length - 4;
                    if ( 0 <= len )
                    {
                        fixed ( char* _base = w.valueOriginal )
                        {
                            *(_AttributeBufferPtr++) = *(_UIM + *(_base + len++));
                            *(_AttributeBufferPtr++) = *(_UIM + *(_base + len++));
                            *(_AttributeBufferPtr++) = *(_UIM + *(_base + len++));
                            *(_AttributeBufferPtr++) = *(_UIM + *(_base + len));
                        }
                    }
                    else
                    {
                        *(_AttributeBufferPtr++) = DASH;
                    }
                }
                #endregion
                break;

                //nding\e – пять букв с конца
                case 'e':
                #region
                {
                    var index = wordIndex + crfAttribute.Position;
                    var w = _Words[ index ];

                    if ( w.posTaggerExtraWordType == PosTaggerExtraWordType.Punctuation )
                    {
                        *(_AttributeBufferPtr++) = DASH;
                        break;
                    }

                    var len = w.valueOriginal.Length - 5;
                    if ( 0 <= len )
                    {
                        fixed ( char* _base = w.valueOriginal )
                        {
                            *(_AttributeBufferPtr++) = *(_UIM + *(_base + len++));
                            *(_AttributeBufferPtr++) = *(_UIM + *(_base + len++));
                            *(_AttributeBufferPtr++) = *(_UIM + *(_base + len++));
                            *(_AttributeBufferPtr++) = *(_UIM + *(_base + len++));
                            *(_AttributeBufferPtr++) = *(_UIM + *(_base + len));
                        }
                    }
                    else
                    {
                        *(_AttributeBufferPtr++) = DASH;
                    }
                }
                #endregion
                break;

                //atr\z – список атрибутов (дан ниже)
                case 'z':
                #region
                {
                    var index = wordIndex + crfAttribute.Position;
                    *(_AttributeBufferPtr++) = _Words[ index ].posTaggerInputType.ToCrfChar();
                }
                #endregion
                break;

                //y – искомое значение
                case 'y':
                #region
                {
                    *(_AttributeBufferPtr++) = 'O'; //POSINPUTTYPE_OTHER == "O"
                }
                #endregion
                break;

                default: throw (new ArgumentException("AttributeName: " + crfAttribute.AttributeName));
            }
        }*/

        #region [.model-builder.]
        private void Uninit4ModelBuilder( int wordsCount )
        {
            for ( var i = 0; i < wordsCount; i++ )
            {
                (_PinnedWordsBufferPtrBase + i)->gcHandle.Free();
            }
            //_Words = null;
        }

        public void WriteCrfAttributesWords4ModelBuilder( TextWriter textWriter, List< word_t > buildModel_words )
        {
            #region [.init.]
            if ( !Init( buildModel_words ) )
            {
                return;
            }
            var wordsCount = buildModel_words.Count;
            #endregion

            #region [.write-crf-attributes-words.]
            for ( int wordIndex = 0; wordIndex < wordsCount; wordIndex++ )
			{
                var attrs = GetPosTaggerAttributes4ModelBuilder( 
                    wordIndex, wordsCount, buildModel_words[ wordIndex ].posTaggerOutputType );

                for ( int i = 0, attrsLength = attrs.Count; i < attrsLength; i++ )
                {
                    textWriter.Write( attrs[ i ] );
                    textWriter.Write( '\t' );
                }
                textWriter.Write( '\n' );
			}
            textWriter.Write( '\n' );
            #endregion

            #region [.un-init.]
            Uninit4ModelBuilder( wordsCount );
            //Uninit();
            #endregion
        }

        unsafe private List< string > GetPosTaggerAttributes4ModelBuilder( 
            int wordIndex, int wordsCount, PosTaggerOutputType posTaggerOutputType )
        {
            var wordsCount_Minus1 = wordsCount - 1;

            _Result4ModelBuilder.Clear();

            _Result4ModelBuilder.Add( /*_Words[ wordIndex ].*/posTaggerOutputType.ToCrfChar().ToString() );

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
                            AppendAttrValue( wordIndex, ngram.CRFAttributes[ j ] ); *(_AttributeBufferPtr++) = VERTICAL_SLASH;
			            }
			            // Удалить последний '|'
                        _AttributeBufferPtr--;
                    }
                    #endregion
                    break;
                }
                #endregion

                var crfValue = new string( _AttributeBufferPtrBase, 0, (int) (_AttributeBufferPtr - _AttributeBufferPtrBase) );
                _Result4ModelBuilder.Add( crfValue );                    
            }

			if ( wordIndex == 0 )
            {
                _Result4ModelBuilder.Add( xlat_Unsafe.BEGIN_OF_SENTENCE );
            }
            else if ( wordIndex == wordsCount_Minus1 )
            {
                _Result4ModelBuilder.Add( xlat_Unsafe.END_OF_SENTENCE );
            }

            return (_Result4ModelBuilder);
        }
        #endregion
    }
}
