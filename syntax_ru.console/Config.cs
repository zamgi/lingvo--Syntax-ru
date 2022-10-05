using System;
using System.Configuration;
using System.Linq;
using lingvo.morphology;
using lingvo.postagger;
using lingvo.sentsplitting;
using lingvo.tokenizing;
using static lingvo.morphology.MorphoModelConfig;

namespace lingvo.syntax
{
    /// <summary>
    /// 
    /// </summary>
    internal enum SyntaxModelSubTypeEnum { AP, PA, }

    /// <summary>
    /// 
    /// </summary>
    internal static class Config
    {
        public static readonly string TOKENIZER_RESOURCES_XML_FILENAME     = ConfigurationManager.AppSettings[ "TOKENIZER_RESOURCES_XML_FILENAME" ];
        public static readonly string POSTAGGER_MODEL_FILENAME             = ConfigurationManager.AppSettings[ "POSTAGGER_MODEL_FILENAME" ];
        public static readonly string POSTAGGER_TEMPLATE_FILENAME          = ConfigurationManager.AppSettings[ "POSTAGGER_TEMPLATE_FILENAME" ];
        public static readonly string POSTAGGER_RESOURCES_XML_FILENAME     = ConfigurationManager.AppSettings[ "POSTAGGER_RESOURCES_XML_FILENAME" ];
        public static readonly string SENT_SPLITTER_RESOURCES_XML_FILENAME = ConfigurationManager.AppSettings[ "SENT_SPLITTER_RESOURCES_XML_FILENAME" ];
        public static readonly string URL_DETECTOR_RESOURCES_XML_FILENAME  = ConfigurationManager.AppSettings[ "URL_DETECTOR_RESOURCES_XML_FILENAME" ];

        public static readonly string   MORPHO_BASE_DIRECTORY         = ConfigurationManager.AppSettings[ "MORPHO_BASE_DIRECTORY" ];
        public static readonly string[] MORPHO_MORPHOTYPES_FILENAMES  = ConfigurationManager.AppSettings[ "MORPHO_MORPHOTYPES_FILENAMES" ].ToFilesArray();
        public static readonly string[] MORPHO_PROPERNAMES_FILENAMES  = ConfigurationManager.AppSettings[ "MORPHO_PROPERNAMES_FILENAMES" ].ToFilesArray();
        public static readonly string[] MORPHO_COMMON_FILENAMES       = ConfigurationManager.AppSettings[ "MORPHO_COMMON_FILENAMES" ].ToFilesArray();        

        public static readonly string MORPHO_AMBIGUITY_MODEL_FILENAME       = ConfigurationManager.AppSettings[ "MORPHO_AMBIGUITY_MODEL_FILENAME" ];
        public static readonly string MORPHO_AMBIGUITY_TEMPLATE_FILENAME_5G = ConfigurationManager.AppSettings[ "MORPHO_AMBIGUITY_TEMPLATE_FILENAME_5G" ];
        public static readonly string MORPHO_AMBIGUITY_TEMPLATE_FILENAME_3G = ConfigurationManager.AppSettings[ "MORPHO_AMBIGUITY_TEMPLATE_FILENAME_3G" ];

        public static readonly string SYNTAX_MODEL_FILENAME_NO_WORDS_AP    = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_FILENAME_NO_WORDS_AP" ];
        public static readonly string SYNTAX_MODEL_FILENAME_NO_WORDS_PA    = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_FILENAME_NO_WORDS_PA" ];
        public static readonly string SYNTAX_MODEL_FILENAME_WORDS_AP       = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_FILENAME_WORDS_AP" ];
        public static readonly string SYNTAX_MODEL_FILENAME_WORDS_PA       = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_FILENAME_WORDS_PA" ];
        public static readonly string SYNTAX_TEMPLATE_FILENAME_NO_WORDS    = ConfigurationManager.AppSettings[ "SYNTAX_TEMPLATE_FILENAME_NO_WORDS" ];
        public static readonly string SYNTAX_TEMPLATE_FILENAME_WORDS       = ConfigurationManager.AppSettings[ "SYNTAX_TEMPLATE_FILENAME_WORDS" ];
        public static readonly SyntaxModelTypeEnum    SYNTAX_MODEL_TYPE    = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_TYPE"    ].ToEnum< SyntaxModelTypeEnum >();
        public static readonly SyntaxModelSubTypeEnum SYNTAX_MODEL_SUBTYPE = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_SUBTYPE" ].ToEnum< SyntaxModelSubTypeEnum >();

        //public static readonly int MAX_INPUTTEXT_LENGTH                = ConfigurationManager.AppSettings[ "MAX_INPUTTEXT_LENGTH"                ].ToInt32();
        //public static readonly int CONCURRENT_FACTORY_INSTANCE_COUNT   = ConfigurationManager.AppSettings[ "CONCURRENT_FACTORY_INSTANCE_COUNT"   ].ToInt32();
        //public static readonly int SAME_IP_INTERVAL_REQUEST_IN_SECONDS = ConfigurationManager.AppSettings[ "SAME_IP_INTERVAL_REQUEST_IN_SECONDS" ].ToInt32();
        //public static readonly int SAME_IP_MAX_REQUEST_IN_INTERVAL     = ConfigurationManager.AppSettings[ "SAME_IP_MAX_REQUEST_IN_INTERVAL"     ].ToInt32();        
        //public static readonly int SAME_IP_BANNED_INTERVAL_IN_SECONDS  = ConfigurationManager.AppSettings[ "SAME_IP_BANNED_INTERVAL_IN_SECONDS"  ].ToInt32();

        private static string[] ToFilesArray( this string value )
        {
            var array = value.Split( new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries )
                             .Select( f => f.Trim() )
                             .ToArray();
            return (array);
        }

        private static bool Try2Bool( this string value, bool defaultValue )
        {
            if ( value != null )
            {
                if ( bool.TryParse( value, out var result ) )
                {
                    return (result);
                }
            }
            return (defaultValue);
        }

        private static T ToEnum< T >( this string value ) where T : struct => (T) Enum.Parse( typeof(T), value, true );
        private static int ToInt32( this string value ) => int.Parse( value );




        public static string GetSyntaxModelFilename( SyntaxModelTypeEnum syntaxModelType, SyntaxModelSubTypeEnum syntaxModelsubType )
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
        public static string GetSyntaxTemplateFilename( SyntaxModelTypeEnum syntaxModelType )
        {
            switch ( syntaxModelType )
            {
                case SyntaxModelTypeEnum.No_Words: return (SYNTAX_TEMPLATE_FILENAME_NO_WORDS);
                case SyntaxModelTypeEnum.Words:    return (SYNTAX_TEMPLATE_FILENAME_WORDS);
                default: throw (new ArgumentException(syntaxModelType.ToString()));
            }
        }
        public static SyntaxProcessorConfig CreateSyntaxProcessorConfig( PosTaggerProcessorConfig posTaggerConfig, IMorphoModel morphoModel, MorphoAmbiguityResolverModel morphoAmbiguityModel )
            => new SyntaxProcessorConfig( posTaggerConfig, morphoModel, morphoAmbiguityModel )
            {
                ModelType        = SYNTAX_MODEL_TYPE,
                ModelFilename    = GetSyntaxModelFilename   ( SYNTAX_MODEL_TYPE, SYNTAX_MODEL_SUBTYPE ),
                TemplateFilename = GetSyntaxTemplateFilename( SYNTAX_MODEL_TYPE ),
            };
        public static PosTaggerProcessorConfig CreatePosTaggerProcessorConfig()
        {
            var sentSplitterConfig = new SentSplitterConfig( SENT_SPLITTER_RESOURCES_XML_FILENAME, URL_DETECTOR_RESOURCES_XML_FILENAME );
            var config = new PosTaggerProcessorConfig( TOKENIZER_RESOURCES_XML_FILENAME, POSTAGGER_RESOURCES_XML_FILENAME, LanguageTypeEnum.Ru, sentSplitterConfig )
            {
                ModelFilename    = POSTAGGER_MODEL_FILENAME,
                TemplateFilename = POSTAGGER_TEMPLATE_FILENAME,
            };

            return (config);
        }
        public static MorphoModelConfig CreateMorphoModelConfig()
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
        public static MorphoAmbiguityResolverConfig CreateMorphoAmbiguityConfig() => new MorphoAmbiguityResolverConfig()
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
}
