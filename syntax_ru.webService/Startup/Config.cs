using System;
using System.Configuration;
using System.Linq;

using captcha;
using lingvo.morphology;
using lingvo.postagger;
using lingvo.sentsplitting;
using lingvo.tokenizing;
using TreeDictionaryTypeEnum = lingvo.morphology.MorphoModelConfig.TreeDictionaryTypeEnum;

namespace lingvo.syntax.webService
{
    /// <summary>
    /// 
    /// </summary>
    public enum SyntaxModelSubTypeEnum { AP, PA, }

    /// <summary>
    /// 
    /// </summary>
    public interface IConfig : IAntiBotConfig
    {
        int CONCURRENT_FACTORY_INSTANCE_COUNT { get; }

        string TOKENIZER_RESOURCES_XML_FILENAME     { get; }
        string POSTAGGER_MODEL_FILENAME             { get; }
        string POSTAGGER_TEMPLATE_FILENAME          { get; }
        string POSTAGGER_RESOURCES_XML_FILENAME     { get; }
        string SENT_SPLITTER_RESOURCES_XML_FILENAME { get; }
        string URL_DETECTOR_RESOURCES_XML_FILENAME  { get; }

        string   MORPHO_BASE_DIRECTORY         { get; }
        string[] MORPHO_MORPHOTYPES_FILENAMES  { get; }
        string[] MORPHO_PROPERNAMES_FILENAMES  { get; }
        string[] MORPHO_COMMON_FILENAMES       { get; }

        string MORPHO_AMBIGUITY_MODEL_FILENAME       { get; }
        string MORPHO_AMBIGUITY_TEMPLATE_FILENAME_5G { get; }
        string MORPHO_AMBIGUITY_TEMPLATE_FILENAME_3G { get; }

        string SYNTAX_MODEL_FILENAME_NO_WORDS_AP    { get; }
        string SYNTAX_MODEL_FILENAME_NO_WORDS_PA    { get; }
        string SYNTAX_MODEL_FILENAME_WORDS_AP       { get; }
        string SYNTAX_MODEL_FILENAME_WORDS_PA       { get; }
        string SYNTAX_TEMPLATE_FILENAME_NO_WORDS    { get; }
        string SYNTAX_TEMPLATE_FILENAME_WORDS       { get; }
        SyntaxModelTypeEnum    SYNTAX_MODEL_TYPE    { get; }
        SyntaxModelSubTypeEnum SYNTAX_MODEL_SUBTYPE { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    internal sealed class Config : IConfig, IAntiBotConfig
    {
        public Config() { }

        public int? SameIpBannedIntervalInSeconds  { get; } = int.TryParse( ConfigurationManager.AppSettings[ "SAME_IP_BANNED_INTERVAL_IN_SECONDS"  ], out var i ) ? i : null;
        public int? SameIpIntervalRequestInSeconds { get; } = int.TryParse( ConfigurationManager.AppSettings[ "SAME_IP_INTERVAL_REQUEST_IN_SECONDS" ], out var i ) ? i : null;
        public int? SameIpMaxRequestInInterval     { get; } = int.TryParse( ConfigurationManager.AppSettings[ "SAME_IP_MAX_REQUEST_IN_INTERVAL"     ], out var i ) ? i : null;
        public string CaptchaPageTitle => "Определение синтаксических ролей слов в предложении";


        public int CONCURRENT_FACTORY_INSTANCE_COUNT { get; } = int.Parse( ConfigurationManager.AppSettings[ "CONCURRENT_FACTORY_INSTANCE_COUNT" ] );
        //public int MAX_INPUTTEXT_LENGTH { get; } = ConfigurationManager.AppSettings[ "MAX_INPUTTEXT_LENGTH" ].ToInt32();


        public string TOKENIZER_RESOURCES_XML_FILENAME     { get; } = ConfigurationManager.AppSettings[ "TOKENIZER_RESOURCES_XML_FILENAME" ];
        public string POSTAGGER_MODEL_FILENAME             { get; } = ConfigurationManager.AppSettings[ "POSTAGGER_MODEL_FILENAME" ];
        public string POSTAGGER_TEMPLATE_FILENAME          { get; } = ConfigurationManager.AppSettings[ "POSTAGGER_TEMPLATE_FILENAME" ];
        public string POSTAGGER_RESOURCES_XML_FILENAME     { get; } = ConfigurationManager.AppSettings[ "POSTAGGER_RESOURCES_XML_FILENAME" ];
        public string SENT_SPLITTER_RESOURCES_XML_FILENAME { get; } = ConfigurationManager.AppSettings[ "SENT_SPLITTER_RESOURCES_XML_FILENAME" ];
        public string URL_DETECTOR_RESOURCES_XML_FILENAME  { get; } = ConfigurationManager.AppSettings[ "URL_DETECTOR_RESOURCES_XML_FILENAME" ];

        public string   MORPHO_BASE_DIRECTORY         { get; } = ConfigurationManager.AppSettings[ "MORPHO_BASE_DIRECTORY" ];
        public string[] MORPHO_MORPHOTYPES_FILENAMES  { get; } = ConfigurationManager.AppSettings[ "MORPHO_MORPHOTYPES_FILENAMES" ].ToFilesArray();
        public string[] MORPHO_PROPERNAMES_FILENAMES  { get; } = ConfigurationManager.AppSettings[ "MORPHO_PROPERNAMES_FILENAMES" ].ToFilesArray();
        public string[] MORPHO_COMMON_FILENAMES       { get; } = ConfigurationManager.AppSettings[ "MORPHO_COMMON_FILENAMES" ].ToFilesArray();        

        public string MORPHO_AMBIGUITY_MODEL_FILENAME       { get; } = ConfigurationManager.AppSettings[ "MORPHO_AMBIGUITY_MODEL_FILENAME" ];
        public string MORPHO_AMBIGUITY_TEMPLATE_FILENAME_5G { get; } = ConfigurationManager.AppSettings[ "MORPHO_AMBIGUITY_TEMPLATE_FILENAME_5G" ];
        public string MORPHO_AMBIGUITY_TEMPLATE_FILENAME_3G { get; } = ConfigurationManager.AppSettings[ "MORPHO_AMBIGUITY_TEMPLATE_FILENAME_3G" ];

        public string SYNTAX_MODEL_FILENAME_NO_WORDS_AP    { get; } = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_FILENAME_NO_WORDS_AP" ];
        public string SYNTAX_MODEL_FILENAME_NO_WORDS_PA    { get; } = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_FILENAME_NO_WORDS_PA" ];
        public string SYNTAX_MODEL_FILENAME_WORDS_AP       { get; } = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_FILENAME_WORDS_AP" ];
        public string SYNTAX_MODEL_FILENAME_WORDS_PA       { get; } = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_FILENAME_WORDS_PA" ];
        public string SYNTAX_TEMPLATE_FILENAME_NO_WORDS    { get; } = ConfigurationManager.AppSettings[ "SYNTAX_TEMPLATE_FILENAME_NO_WORDS" ];
        public string SYNTAX_TEMPLATE_FILENAME_WORDS       { get; } = ConfigurationManager.AppSettings[ "SYNTAX_TEMPLATE_FILENAME_WORDS" ];
        public SyntaxModelTypeEnum    SYNTAX_MODEL_TYPE    { get; } = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_TYPE"    ].ToEnum< SyntaxModelTypeEnum >();
        public SyntaxModelSubTypeEnum SYNTAX_MODEL_SUBTYPE { get; } = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_SUBTYPE" ].ToEnum< SyntaxModelSubTypeEnum >();



        public string GetSyntaxModelFilename( SyntaxModelTypeEnum syntaxModelType, SyntaxModelSubTypeEnum syntaxModelsubType )
        {
            switch ( syntaxModelType )
            {
                case SyntaxModelTypeEnum.No_Words:
                    switch ( syntaxModelsubType )
                    {
                        case SyntaxModelSubTypeEnum.AP: return (SYNTAX_MODEL_FILENAME_NO_WORDS_AP);
                        case SyntaxModelSubTypeEnum.PA: return (SYNTAX_MODEL_FILENAME_NO_WORDS_PA);
                        default:  throw (new ArgumentException(syntaxModelsubType.ToString()));
                    }

                case SyntaxModelTypeEnum.Words:
                    switch ( syntaxModelsubType )
                    {
                        case SyntaxModelSubTypeEnum.AP: return (SYNTAX_MODEL_FILENAME_WORDS_AP);
                        case SyntaxModelSubTypeEnum.PA: return (SYNTAX_MODEL_FILENAME_WORDS_PA);
                        default:  throw (new ArgumentException(syntaxModelsubType.ToString()));
                    }

                default: 
                    throw (new ArgumentException(syntaxModelType.ToString()));
            }
        }
        public string GetSyntaxTemplateFilename( SyntaxModelTypeEnum syntaxModelType )
        {
            switch ( syntaxModelType )
            {
                case SyntaxModelTypeEnum.No_Words: return (SYNTAX_TEMPLATE_FILENAME_NO_WORDS);
                case SyntaxModelTypeEnum.Words:    return (SYNTAX_TEMPLATE_FILENAME_WORDS);
                default: throw (new ArgumentException(syntaxModelType.ToString()));
            }
        }
        public SyntaxProcessorConfig CreateSyntaxProcessorConfig( PosTaggerProcessorConfig posTaggerConfig, IMorphoModel morphoModel, MorphoAmbiguityResolverModel morphoAmbiguityModel )
            => new SyntaxProcessorConfig( posTaggerConfig, morphoModel, morphoAmbiguityModel )
            {
                ModelType        = SYNTAX_MODEL_TYPE,
                ModelFilename    = GetSyntaxModelFilename   ( SYNTAX_MODEL_TYPE, SYNTAX_MODEL_SUBTYPE ),
                TemplateFilename = GetSyntaxTemplateFilename( SYNTAX_MODEL_TYPE ),
            };
        public PosTaggerProcessorConfig CreatePosTaggerProcessorConfig()
        {
            var sentSplitterConfig = new SentSplitterConfig( SENT_SPLITTER_RESOURCES_XML_FILENAME, URL_DETECTOR_RESOURCES_XML_FILENAME );
            var config = new PosTaggerProcessorConfig( TOKENIZER_RESOURCES_XML_FILENAME, POSTAGGER_RESOURCES_XML_FILENAME, LanguageTypeEnum.Ru, sentSplitterConfig )
            {
                ModelFilename    = POSTAGGER_MODEL_FILENAME,
                TemplateFilename = POSTAGGER_TEMPLATE_FILENAME,
            };

            return (config);
        }
        public MorphoModelConfig CreateMorphoModelConfig()
        {
            //_ModelLoadingErrors.Clear();

            var config = new MorphoModelConfig()
            {
                TreeDictionaryType   = TreeDictionaryTypeEnum.Native,
                BaseDirectory        = MORPHO_BASE_DIRECTORY,
                MorphoTypesFilenames = MORPHO_MORPHOTYPES_FILENAMES,
                ProperNamesFilenames = MORPHO_PROPERNAMES_FILENAMES,
                CommonFilenames      = MORPHO_COMMON_FILENAMES,
                ModelLoadingErrorCallback = (s1, s2) =>
                {
                    //_ModelLoadingErrors.Append( s1 ).Append( " - " ).Append( s2 ).Append( "<br/>" );
                    /*
#if DEBUG
                    //Debug.WriteLine( s1 + " - " + s2 );
                    Console.WriteLine( s1 + " - " + s2 );
#endif
                    */
                }
            };

            return (config);
        }
        public MorphoAmbiguityResolverConfig CreateMorphoAmbiguityConfig() => new MorphoAmbiguityResolverConfig()
        {
            ModelFilename       = MORPHO_AMBIGUITY_MODEL_FILENAME,
            TemplateFilename_5g = MORPHO_AMBIGUITY_TEMPLATE_FILENAME_5G,
            TemplateFilename_3g = MORPHO_AMBIGUITY_TEMPLATE_FILENAME_3G,
        };
        public static MorphoAmbiguityResolverModel  CreateMorphoAmbiguityResolverModel( in MorphoAmbiguityResolverConfig config ) => new MorphoAmbiguityResolverModel( in config );
        
        //private static SyntaxProcessorConfig CreateSyntaxProcessorConfig()
        //{
        //    var posTaggerConfig = CreatePosTaggerProcessorConfig();

        //    var morphoModelConfig = CreateMorphoModelConfig();
        //    var morphoModel       = MorphoModelFactory.Create( in morphoModelConfig );

        //    var morphoAmbiguityResolverConfig = CreateMorphoAmbiguityConfig();
        //    var morphoAmbiguityModel          = CreateMorphoAmbiguityResolverModel( in morphoAmbiguityResolverConfig );

        //    var config = CreateSyntaxProcessorConfig( posTaggerConfig, morphoModel, morphoAmbiguityModel );
        //    return (config);
        //}
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class ConfigExtensions
    {
        public static string[] ToFilesArray( this string value ) => value.Split( new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries ).Select( f => f.Trim() ).ToArray();

        public static T ToEnum< T >( this string value ) where T : struct => (T) Enum.Parse( typeof(T), value, true );
        public static int ToInt32( this string value ) => int.Parse( value );
    }
}
