using System;
using System.Collections.Generic;

using lingvo.core;
using lingvo.morphology;
using lingvo.tokenizing;

namespace lingvo.postagger
{
    /// <summary>
    /// Обработчик именованных сущностей. Обработка с использованием библиотеки CRFSuit 
    /// </summary>
    public sealed class PosTaggerProcessor : IDisposable
	{
        #region [.private field's.]
        private const int DEFAULT_WORDSLIST_CAPACITY = 1000;
        private readonly Tokenizer                    _Tokenizer;
        private readonly List< word_t >               _Words;
        private readonly PosTaggerScriber             _PosTaggerScriber;
        private readonly PosTaggerPreMerging          _PosTaggerPreMerging;
        private readonly PosTaggerMorphoAnalyzer      _PosTaggerMorphoAnalyzer;
        private Tokenizer.ProcessSentCallbackDelegate _ProcessSentCallback_1_Delegate;
        private Tokenizer.ProcessSentCallbackDelegate _ProcessSentCallback_2_Delegate;
        private Tokenizer.ProcessSentCallbackDelegate _OuterProcessSentCallback_Delegate;
        #endregion

        #region [.ctor().]
        public PosTaggerProcessor( PosTaggerProcessorConfig     config, 
                                   IMorphoModel                 morphoModel,
                                   MorphoAmbiguityResolverModel morphoAmbiguityModel )
		{
            CheckConfig( config, morphoModel, morphoAmbiguityModel );
						
            _Tokenizer               = new Tokenizer( config.TokenizerConfig );
            _Words                   = new List< word_t >( DEFAULT_WORDSLIST_CAPACITY );
            _PosTaggerScriber        = PosTaggerScriber.Create( config.ModelFilename, config.TemplateFilename );
            _PosTaggerPreMerging     = new PosTaggerPreMerging( config.Model );
            _PosTaggerMorphoAnalyzer = new PosTaggerMorphoAnalyzer( morphoModel, morphoAmbiguityModel );
            _ProcessSentCallback_1_Delegate = new Tokenizer.ProcessSentCallbackDelegate( ProcessSentCallback_1 );
            _ProcessSentCallback_2_Delegate = new Tokenizer.ProcessSentCallbackDelegate( ProcessSentCallback_2 );
        }

        public void Dispose()
        {
            _Tokenizer              .Dispose();
            _PosTaggerScriber       .Dispose();
            _PosTaggerMorphoAnalyzer.Dispose();
        }

        private static void CheckConfig( PosTaggerProcessorConfig config, IMorphoModel morphoModel, MorphoAmbiguityResolverConfig morphoAmbiguityConfig )
		{
            morphoModel.ThrowIfNull( "morphoModel" );

			config                 .ThrowIfNull( "config" );
            config.Model           .ThrowIfNull( "Model" );
            config.TokenizerConfig .ThrowIfNull( "TokenizerConfig" );
			config.ModelFilename   .ThrowIfNullOrWhiteSpace( "ModelFilename" );
			config.TemplateFilename.ThrowIfNullOrWhiteSpace( "TemplateFilename" );

            morphoAmbiguityConfig                    .ThrowIfNull( "morphoAmbiguityConfig" );
            morphoAmbiguityConfig.ModelFilename      .ThrowIfNullOrWhiteSpace( "morphoAmbiguityConfig.ModelFilename" );
            morphoAmbiguityConfig.TemplateFilename_5g.ThrowIfNullOrWhiteSpace( "morphoAmbiguityConfig.TemplateFilename_5g" );
		}
        private static void CheckConfig( PosTaggerProcessorConfig config, IMorphoModel morphoModel, MorphoAmbiguityResolverModel morphoAmbiguityModel )
		{
            morphoModel.ThrowIfNull( "morphoModel" );

			config                 .ThrowIfNull( "config" );
            config.Model           .ThrowIfNull( "Model" );
            config.TokenizerConfig .ThrowIfNull( "TokenizerConfig" );
			config.ModelFilename   .ThrowIfNullOrWhiteSpace( "ModelFilename" );
			config.TemplateFilename.ThrowIfNullOrWhiteSpace( "TemplateFilename" );

            morphoAmbiguityModel.ThrowIfNull( "morphoAmbiguityModel" );
		}
        #endregion

        public List< word_t > Run( string text, bool splitBySmiles )
        {
            _Words.Clear();

            _Tokenizer.Run( text, splitBySmiles, _ProcessSentCallback_1_Delegate );

            return (_Words);
        }
        private void ProcessSentCallback_1( List< word_t > words )
        {
            //-merge-phrases-abbreviations-numbers-
            _PosTaggerPreMerging.Run( words );

            //directly pos-tagging
            _PosTaggerScriber.Run( words );

            //morpho-analyze            
            #if DEBUG
            _PosTaggerMorphoAnalyzer.Run( words, true );
            #else
            _PosTaggerMorphoAnalyzer.Run( words );
            #endif

            _Words.AddRange( words );
        }

        public void Run( string text, bool splitBySmiles, Tokenizer.ProcessSentCallbackDelegate processSentCallback )
        {
            _OuterProcessSentCallback_Delegate = processSentCallback;

            _Tokenizer.Run( text, splitBySmiles, _ProcessSentCallback_2_Delegate );

            _OuterProcessSentCallback_Delegate = null;
        }
        private void ProcessSentCallback_2( List< word_t > words )
        {
            //-merge-phrases-abbreviations-numbers-
            _PosTaggerPreMerging.Run( words );

            //directly pos-tagging
            _PosTaggerScriber.Run( words );

            //morpho-analyze
            #if DEBUG
            _PosTaggerMorphoAnalyzer.Run( words, true );
            #else
            _PosTaggerMorphoAnalyzer.Run( words );
            #endif

            _OuterProcessSentCallback_Delegate( words );
        }

        public List< word_t[] > Run_Debug( string text
            , bool splitBySmiles
            , bool mergeChains
            , bool processMorphology
            , bool applyMorphoAmbiguityPreProcess )
        {
            var wordsBySents = new List< word_t[] >();

            _Tokenizer.Run( text, splitBySmiles, (words) =>
            {
                if ( mergeChains )
                {
                    //-merge-phrases-abbreviations-numbers-
                    _PosTaggerPreMerging.Run( words );
                }

                //directly pos-tagging
                _PosTaggerScriber.Run( words );

                if ( processMorphology )
                {
                    //morpho-analyze
                    #if DEBUG
                    _PosTaggerMorphoAnalyzer.Run( words, applyMorphoAmbiguityPreProcess );
                    #else
                    _PosTaggerMorphoAnalyzer.Run( words );
                    #endif                    
                }

                wordsBySents.Add( words.ToArray() );
            });

            return (wordsBySents);
        }
    }
}
