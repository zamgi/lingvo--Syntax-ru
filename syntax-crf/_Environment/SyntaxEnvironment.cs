using System;
using System.Diagnostics;
using System.Threading.Tasks;

using lingvo.morphology;
using lingvo.postagger;
using lingvo.sentsplitting;
using lingvo.tokenizing;

namespace lingvo.syntax
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SyntaxEnvironment : IDisposable
    {
        private SyntaxEnvironment() { }
        public void Dispose()
        {
            if ( MorphoModel != null )
            {
                MorphoModel.Dispose();
                MorphoModel = null;
            }

            if ( MorphoAmbiguityResolverModel != null )
            {
                MorphoAmbiguityResolverModel.Dispose();
                MorphoAmbiguityResolverModel = null;
            }

            if ( SentSplitterConfig != null )
            {
                SentSplitterConfig.Dispose();
                SentSplitterConfig = null;
            }
        }

        public  SyntaxProcessorConfig        SyntaxProcessorConfig        { get; private set; }
        public  MorphoAmbiguityResolverModel MorphoAmbiguityResolverModel { get; private set; }
        public  IMorphoModel                 MorphoModel                  { get; private set; }
        private SentSplitterConfig           SentSplitterConfig           { get; set; }

        public SyntaxProcessor CreateSyntaxProcessor() => new SyntaxProcessor( SyntaxProcessorConfig );

        public static SyntaxEnvironment Create( SyntaxEnvironmentConfigBase opts, LanguageTypeEnum languageType = LanguageTypeEnum.Ru, bool print2Console = true )
        {
            var sw = default(Stopwatch);
            if ( print2Console )
            {
                sw = Stopwatch.StartNew();
                Console.Write( "init syntax-environment..." );
            }

            var (posTaggerConfig, ssc) = opts.CreatePosTaggerProcessorConfig( languageType );

            var morphoModelConfig = opts.CreateMorphoModelConfig();
            var morphoModel       = MorphoModelFactory.Create( morphoModelConfig );

            var morphoAmbiguityResolverConfig = opts.CreateMorphoAmbiguityConfig();
            var morphoAmbiguityModel          = SyntaxEnvironmentConfigBase.CreateMorphoAmbiguityResolverModel( morphoAmbiguityResolverConfig );

            var config = opts.CreateSyntaxProcessorConfig( posTaggerConfig, morphoModel, morphoAmbiguityModel );

            var posEnv = new SyntaxEnvironment()
            {
                MorphoAmbiguityResolverModel = morphoAmbiguityModel,
                MorphoModel                  = morphoModel,
                SentSplitterConfig           = ssc,
                SyntaxProcessorConfig        = config,
            };

            if ( print2Console )
            {
                sw.Stop();
                Console.WriteLine( $"end, (elapsed: {sw.Elapsed}).\r\n----------------------------------------------------\r\n" );
            }

            return (posEnv);
        }
        public static async Task< SyntaxEnvironment > CreateAsync( SyntaxEnvironmentConfigBase opts, LanguageTypeEnum languageType = LanguageTypeEnum.Ru, bool print2Console = true )
        {
            var sw = default(Stopwatch);
            if ( print2Console )
            {
                sw = Stopwatch.StartNew();
                Console.Write( "init syntax-environment..." );
            }

            var posTaggerConfig_ssc_task  = Task.Run( () => opts.CreatePosTaggerProcessorConfig( languageType ) );
            var morphoAmbiguityModel_task = Task.Run( () =>
            {
                var morphoAmbiguityResolverConfig = opts.CreateMorphoAmbiguityConfig();
                var morphoAmbiguityModel          = SyntaxEnvironmentConfigBase.CreateMorphoAmbiguityResolverModel( morphoAmbiguityResolverConfig );
                return (morphoAmbiguityModel);
            });
            var morphoModel_task          = Task.Run( () =>
            {
                var morphoModelConfig = opts.CreateMorphoModelConfig();
                var morphoModel       = MorphoModelFactory.Create( morphoModelConfig );
                return (morphoModel);
            });

            await Task.WhenAll( morphoAmbiguityModel_task, morphoModel_task, posTaggerConfig_ssc_task ).ConfigureAwait( false );

            var morphoAmbiguityModel   = morphoAmbiguityModel_task.Result;
            var morphoModel            = morphoModel_task.Result;
            var (posTaggerConfig, ssc) = posTaggerConfig_ssc_task.Result;

            var config = opts.CreateSyntaxProcessorConfig( posTaggerConfig, morphoModel, morphoAmbiguityModel );

            var posEnv = new SyntaxEnvironment()
            {
                MorphoAmbiguityResolverModel = morphoAmbiguityModel,
                MorphoModel                  = morphoModel,
                SentSplitterConfig           = ssc,
                SyntaxProcessorConfig        = config,
            };

            if ( print2Console )
            {
                sw.Stop();
                Console.WriteLine( $"end, (elapsed: {sw.Elapsed}).\r\n----------------------------------------------------\r\n" );
            }

            return (posEnv);
        }
        public static Task< SyntaxEnvironment > CreateAsync( LanguageTypeEnum languageType = LanguageTypeEnum.Ru, bool print2Console = true ) => CreateAsync( new SyntaxEnvironmentConfigImpl(), languageType, print2Console );
    }
}
