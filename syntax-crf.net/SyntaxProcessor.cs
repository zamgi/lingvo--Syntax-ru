using System;
using System.Collections.Generic;

using lingvo.core;
using lingvo.crfsuite;
using lingvo.postagger;
using lingvo.tokenizing;

namespace lingvo.syntax
{
    /// <summary>
    /// Обработчик именованных сущностей. Обработка с использованием библиотеки CRFSuit 
    /// </summary>
    public sealed class SyntaxProcessor : IDisposable
	{
        #region [.private field's.]
        private const int DEFAULT_WORDSLIST_CAPACITY = 1000;
        private readonly List< word_t >               _Words;

        private readonly PosTaggerProcessor           _PosTaggerProcessor;
        private readonly ISyntaxScriber               _SyntaxScriber;
        private Tokenizer.ProcessSentCallbackDelegate _ProcessSentCallback;        
        #endregion

        #region [.ctor().]
        public SyntaxProcessor( SyntaxProcessorConfig config )
		{
            CheckConfig( config );

            _Words              = new List< word_t >( DEFAULT_WORDSLIST_CAPACITY );
            _PosTaggerProcessor = new PosTaggerProcessor( config.PosTaggerProcessorConfig,
                                                          config.MorphoModel,
                                                          config.MorphoAmbiguityModel
                                                        );

            switch ( config.ModelType )
            {
                case SyntaxModelTypeEnum.No_Words:
                    _SyntaxScriber = new SyntaxScriber_NoWords( config.ModelFilename, config.TemplateFilename );
                break;

                case SyntaxModelTypeEnum.Words:
                    _SyntaxScriber = new SyntaxScriber_Words( config.ModelFilename, config.TemplateFilename );
                break;
            }
            ModelType = config.ModelType;
		}

        public void Dispose()
        {
            _PosTaggerProcessor.Dispose();
            _SyntaxScriber     .Dispose();
        }

        private static void CheckConfig( SyntaxProcessorConfig config )
		{
			config                         .ThrowIfNull( "config" );
            config.PosTaggerProcessorConfig.ThrowIfNull( "PosTaggerProcessorConfig" );
            config.MorphoModel             .ThrowIfNull( "MorphoModel" );
            config.MorphoAmbiguityModel    .ThrowIfNull( "MorphoAmbiguityModel" );
			config.ModelFilename           .ThrowIfNullOrWhiteSpace( "ModelFilename" );
			config.TemplateFilename        .ThrowIfNullOrWhiteSpace( "TemplateFilename" );
		}
        #endregion

        /*public List< word_t > Run( string text, bool splitBySmiles )
        {
            var words = _PosTaggerProcessor.Run( text, splitBySmiles );

            _SyntaxScriber.Run( words );

            return (words);
        }*/

        public SyntaxModelTypeEnum ModelType
        {
            get;
            private set;
        }

        public List< word_t > Run( string text, bool splitBySmiles )
        {
            _Words.Clear();

            _PosTaggerProcessor.Run( text, splitBySmiles, ProcessSentCallback );

            return (_Words);
        }
        private void ProcessSentCallback( List< word_t > words )
        {
            _SyntaxScriber.Run( words );

            _Words.AddRange( words );
        }

        public void Run( string text, bool splitBySmiles, Tokenizer.ProcessSentCallbackDelegate processSentCallback )
        {
            _ProcessSentCallback = processSentCallback;

            _PosTaggerProcessor.Run( text, splitBySmiles, ProcessSentCallback_Callback );

            _ProcessSentCallback = null;
        }
        private void ProcessSentCallback_Callback( List< word_t > words )
        {
            _SyntaxScriber.Run( words );

            _ProcessSentCallback( words );
        }

        public List< word_t[] > Run_Debug( string text, bool splitBySmiles )
        {
            var wordsBySents = _PosTaggerProcessor.Run_Debug( text, splitBySmiles, true, true, true );

            foreach ( var words in wordsBySents )
            {
                _SyntaxScriber.Run( words );
            }

            return (wordsBySents);
        }
    }
}
