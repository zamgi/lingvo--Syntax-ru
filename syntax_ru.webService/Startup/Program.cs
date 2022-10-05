using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;

using captcha;
using lingvo.morphology;
using lingvo.postagger;

namespace lingvo.syntax.webService
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Program
    {
        public const string SERVICE_NAME = "syntax_ru.webService";

        /// <summary>
        /// 
        /// </summary>
        private sealed class environment : IDisposable
        {
            private IMorphoModel _MorphoModel;
            private MorphoAmbiguityResolverModel _MorphoAmbiguityModel;
            private SyntaxProcessorConfig _SyntaxProcessorConfig;
            public void Dispose()
            {
                _MorphoModel.Dispose();
                _MorphoAmbiguityModel.Dispose();
            }

            public ref readonly SyntaxProcessorConfig SyntaxProcessorConfig => ref _SyntaxProcessorConfig;
            public static environment Create( Config opts, bool print2Console = true )
            {
                var sw = default( Stopwatch );
                if ( print2Console )
                {
                    sw = Stopwatch.StartNew();
                    Console.Write( "init syntax-environment..." );
                }

                var posTaggerConfig = opts.CreatePosTaggerProcessorConfig();

                var morphoModelConfig = opts.CreateMorphoModelConfig();
                var morphoModel       = MorphoModelFactory.Create( morphoModelConfig );

                var morphoAmbiguityResolverConfig = opts.CreateMorphoAmbiguityConfig();
                var morphoAmbiguityModel          = Config.CreateMorphoAmbiguityResolverModel( morphoAmbiguityResolverConfig );

                var syntaxProcessorConfig = opts.CreateSyntaxProcessorConfig( posTaggerConfig, morphoModel, morphoAmbiguityModel );

                if ( print2Console )
                {
                    sw.Stop();
                    Console.WriteLine( $"end, (elapsed: {sw.Elapsed}).\r\n----------------------------------------------------\r\n" );
                }

                return (new environment() { _MorphoModel = morphoModel, _MorphoAmbiguityModel = morphoAmbiguityModel, _SyntaxProcessorConfig = syntaxProcessorConfig });
            }
        }

        private static async Task Main( string[] args )
        {
            var hostApplicationLifetime = default(IHostApplicationLifetime);
            var logger                  = default(ILogger);
            try
            {
                //---------------------------------------------------------------//
                var opts = new Config();
                using var env = environment.Create( opts );

                using var concurrentFactory = new ConcurrentFactory( env.SyntaxProcessorConfig, opts );
                //---------------------------------------------------------------//

                var host = Host.CreateDefaultBuilder( args )
                               .ConfigureLogging( loggingBuilder => loggingBuilder.ClearProviders().AddDebug().AddConsole().AddEventSourceLogger()
                                                              .AddEventLog( new EventLogSettings() { LogName = SERVICE_NAME, SourceName = SERVICE_NAME } ) )
                               //---.UseWindowsService()
                               .ConfigureServices( (hostContext, services) => services.AddSingleton< IConfig >( opts ).AddSingleton< IAntiBotConfig >( opts ).AddSingleton( concurrentFactory ) )
                               .ConfigureWebHostDefaults( webBuilder => webBuilder.UseStartup< Startup >() )
                               .Build();
                hostApplicationLifetime = host.Services.GetService< IHostApplicationLifetime >();
                logger                  = host.Services.GetService< ILoggerFactory >()?.CreateLogger( SERVICE_NAME );
                await host.RunAsync();
            }
            catch ( OperationCanceledException ex ) when ((hostApplicationLifetime?.ApplicationStopping.IsCancellationRequested).GetValueOrDefault())
            {
                Debug.WriteLine( ex ); //suppress
            }
            catch ( Exception ex ) when (logger != null)
            {
                logger.LogCritical( ex, "Global exception handler" );
            }
        }
    }
}
