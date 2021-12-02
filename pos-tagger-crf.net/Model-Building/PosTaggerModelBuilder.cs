using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using lingvo.core;
using lingvo.tokenizing;
using lingvo.urls;

namespace lingvo.postagger
{
    /// <summary>
    /// Обработчик именованных сущностей. Обработка с использованием библиотеки CRFSuit 
    /// </summary>
    public sealed class PosTaggerModelBuilder
	{
        private static readonly char[] SPLIT_CHARS = new[] { '\t' };
        private readonly IPosTaggerInputTypeProcessor _PosTaggerInputTypeProcessor;
        private readonly PosTaggerScriber             _PosTaggerScriber;
        private readonly UrlDetector                  _UrlDetector;
        private readonly List< word_t >               _Words;

        public PosTaggerModelBuilder( string templateFilename, LanguageTypeEnum languageType, UrlDetectorConfig urlDetectorConfig )
        {
            templateFilename.ThrowIfNullOrWhiteSpace("templateFilename");
            urlDetectorConfig.ThrowIfNull("urlDetectorConfig");
			
			_PosTaggerScriber  = PosTaggerScriber.Create4ModelBuilder( templateFilename );
            _PosTaggerInputTypeProcessor = CreatePosTaggerInputTypeProcessor( languageType );
            _UrlDetector                 = new UrlDetector( urlDetectorConfig );
            _Words                       = new List< word_t >();
		}

        public delegate void ProcessSentCallbackDelegate( int sentNumber );
        public delegate void ProcessErrorSentCallbackDelegate( string line, int sentNumber );
        public delegate void StartBuildCallbackDelegate();

        unsafe public int CreateCrfInputFormatFile( TextWriter                       textWriter,
                                                    TextReader                       textReader, 
                                                    ProcessSentCallbackDelegate      processSentCallback, 
                                                    int                              sentNumberCallbackStep/*,
                                                    ProcessErrorSentCallbackDelegate processErrorSentCallback*/ )
        {
            var sentNumber = 1;
            var lineNumber = 1;
            for ( ; ; )
            {
                var r = ReadNextSent( textReader, ref lineNumber );
                if ( !r )
                    break;

                if ( 0 < _Words.Count )
                {
                    _PosTaggerScriber.WriteCrfAttributesWords4ModelBuilder( textWriter, _Words );
                }

                if ( (sentNumber % sentNumberCallbackStep) == 0 )
                {
                    processSentCallback( sentNumber );
                }
                sentNumber++;
            }
            if ( (sentNumber % sentNumberCallbackStep) != 0 )
            {
                processSentCallback( sentNumber );
            }

            return (sentNumber);
        }

        unsafe private bool ReadNextSent( TextReader textReader, ref int lineNumber )
        {
            _Words.Clear();

            for ( var line = textReader.ReadLine(); ; line = textReader.ReadLine() )
            {
                if ( line == null )
                    return (false);

                lineNumber++;

                if ( string.IsNullOrWhiteSpace( line ) )
                    break;

                var a = line.Split( SPLIT_CHARS, StringSplitOptions.RemoveEmptyEntries );
                if ( a.Length < 2 )
                    throw (new InvalidDataException("Wrong input data format. APPROXIMITE-LINE-NUMBER: " + lineNumber + ", line-TEXT: '" + line + '\''));

                var v = a[ 0 ].Trim().Replace( 'ё', 'е' ).Replace( 'Ё', 'Е' );
                var p = ToPosTaggerOutputType( a[ 2 ].Trim() );

                //skip url's
                var urls = _UrlDetector.AllocateUrls( v );
                if ( urls.Count != 0 )
                {
                    continue;
                }

                fixed ( char* ptr = v )
                {
                    var word = new word_t()
                    {
                        valueOriginal          = v,
                        valueUpper             = v.ToUpperInvariant(),
                        posTaggerOutputType    = p,
                    };
                    var result = _PosTaggerInputTypeProcessor.GetResult( ptr, v.Length, word ); //v.ToUpperInvariant() );
                    word.posTaggerInputType     = result.posTaggerInputType;
                    word.posTaggerExtraWordType = result.posTaggerExtraWordType;

                    if ( (word.posTaggerExtraWordType == PosTaggerExtraWordType.__DEFAULT__) && (word.posTaggerInputType == PosTaggerInputType.O) )
                    {
                        if ( word.valueOriginal.Contains( ' ' ) )
                        {
                            word.posTaggerInputType = PosTaggerInputType.CompPh;
                        }
                        #region [.if process url's.]
                        /*else
                        {
                            var urls = _UrlDetector.AllocateUrls( v );
                            if ( urls.Count != 0 )
                            {
                                word.posTaggerInputType  = PosTaggerInputType.Url;
                                word.posTaggerOutputType = PosTaggerOutputType.Other;
                            }
                        }*/
                        #endregion
                    }
                
                    _Words.Add( word );
                }
            }

            return (true);
        }

        private static PosTaggerOutputType ToPosTaggerOutputType( string value ) => (PosTaggerOutputType) Enum.Parse( typeof(PosTaggerOutputType), value, true );
        private static IPosTaggerInputTypeProcessor CreatePosTaggerInputTypeProcessor( LanguageTypeEnum languageType )
        {
            switch ( languageType )
            {
                case LanguageTypeEnum.En:
                    return (new PosTaggerInputTypeProcessor_En( new HashSet< string >(), new HashSet< string >() ));
 
                case LanguageTypeEnum.Ru:
                    return (new PosTaggerInputTypeProcessor_Ru( new HashSet< string >(), new HashSet< string >() ));

                default:
                    throw (new ArgumentException( languageType.ToString() ));
            }
        }
    }
}
