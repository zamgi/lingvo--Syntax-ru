using System;
using System.Configuration;
using System.Linq;

using lingvo.syntax;

namespace lingvo
{
    /*
    /// <summary>
    /// 
    /// </summary>
    internal static class Config
    {
        public static readonly string URL_DETECTOR_RESOURCES_XML_FILENAME  = ConfigurationManager.AppSettings[ "URL_DETECTOR_RESOURCES_XML_FILENAME" ];
        public static readonly string SENT_SPLITTER_RESOURCES_XML_FILENAME = ConfigurationManager.AppSettings[ "SENT_SPLITTER_RESOURCES_XML_FILENAME" ];
        public static readonly string TOKENIZER_RESOURCES_XML_FILENAME     = ConfigurationManager.AppSettings[ "TOKENIZER_RESOURCES_XML_FILENAME" ];
        public static readonly string POSTAGGER_MODEL_FILENAME             = ConfigurationManager.AppSettings[ "POSTAGGER_MODEL_FILENAME" ];
        public static readonly string POSTAGGER_TEMPLATE_FILENAME          = ConfigurationManager.AppSettings[ "POSTAGGER_TEMPLATE_FILENAME" ];
        public static readonly string POSTAGGER_RESOURCES_XML_FILENAME     = ConfigurationManager.AppSettings[ "POSTAGGER_RESOURCES_XML_FILENAME" ];

        public static readonly string   MORPHO_BASE_DIRECTORY        = ConfigurationManager.AppSettings[ "MORPHO_BASE_DIRECTORY" ];
        public static readonly string[] MORPHO_MORPHOTYPES_FILENAMES = ConfigurationManager.AppSettings[ "MORPHO_MORPHOTYPES_FILENAMES" ].ToFilesArray();
        public static readonly string[] MORPHO_PROPERNAMES_FILENAMES = ConfigurationManager.AppSettings[ "MORPHO_PROPERNAMES_FILENAMES" ].ToFilesArray();
        public static readonly string[] MORPHO_COMMON_FILENAMES      = ConfigurationManager.AppSettings[ "MORPHO_COMMON_FILENAMES"      ].ToFilesArray();
        private static string[] ToFilesArray( this string value )
        {
            var array = value.Split( new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries )
                             .Select( f => f.Trim() )
                             .ToArray();
            return (array);
        }

        public static readonly string MORPHO_AMBIGUITY_MODEL_FILENAME       = ConfigurationManager.AppSettings[ "MORPHO_AMBIGUITY_MODEL_FILENAME" ];
        public static readonly string MORPHO_AMBIGUITY_TEMPLATE_FILENAME_5G = ConfigurationManager.AppSettings[ "MORPHO_AMBIGUITY_TEMPLATE_FILENAME_5G" ];
        public static readonly string MORPHO_AMBIGUITY_TEMPLATE_FILENAME_3G = ConfigurationManager.AppSettings[ "MORPHO_AMBIGUITY_TEMPLATE_FILENAME_3G" ];

        public static readonly int MAX_INPUTTEXT_LENGTH                = ConfigurationManager.AppSettings[ "MAX_INPUTTEXT_LENGTH"                ].ToInt32();
        public static readonly int CONCURRENT_FACTORY_INSTANCE_COUNT   = ConfigurationManager.AppSettings[ "CONCURRENT_FACTORY_INSTANCE_COUNT"   ].ToInt32();
        public static readonly int SAME_IP_INTERVAL_REQUEST_IN_SECONDS = ConfigurationManager.AppSettings[ "SAME_IP_INTERVAL_REQUEST_IN_SECONDS" ].ToInt32();
        public static readonly int SAME_IP_MAX_REQUEST_IN_INTERVAL     = ConfigurationManager.AppSettings[ "SAME_IP_MAX_REQUEST_IN_INTERVAL"     ].ToInt32();        
        public static readonly int SAME_IP_BANNED_INTERVAL_IN_SECONDS  = ConfigurationManager.AppSettings[ "SAME_IP_BANNED_INTERVAL_IN_SECONDS"  ].ToInt32();
    }
    */

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

        public static readonly int MAX_INPUTTEXT_LENGTH                = ConfigurationManager.AppSettings[ "MAX_INPUTTEXT_LENGTH"                ].ToInt32();
        public static readonly int CONCURRENT_FACTORY_INSTANCE_COUNT   = ConfigurationManager.AppSettings[ "CONCURRENT_FACTORY_INSTANCE_COUNT"   ].ToInt32();
        public static readonly int SAME_IP_INTERVAL_REQUEST_IN_SECONDS = ConfigurationManager.AppSettings[ "SAME_IP_INTERVAL_REQUEST_IN_SECONDS" ].ToInt32();
        public static readonly int SAME_IP_MAX_REQUEST_IN_INTERVAL     = ConfigurationManager.AppSettings[ "SAME_IP_MAX_REQUEST_IN_INTERVAL"     ].ToInt32();        
        public static readonly int SAME_IP_BANNED_INTERVAL_IN_SECONDS  = ConfigurationManager.AppSettings[ "SAME_IP_BANNED_INTERVAL_IN_SECONDS"  ].ToInt32();

        public static readonly string SYNTAX_MODEL_FILENAME_NO_WORDS_AP    = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_FILENAME_NO_WORDS_AP" ];
        public static readonly string SYNTAX_MODEL_FILENAME_NO_WORDS_PA    = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_FILENAME_NO_WORDS_PA" ];
        public static readonly string SYNTAX_MODEL_FILENAME_WORDS_AP       = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_FILENAME_WORDS_AP" ];
        public static readonly string SYNTAX_MODEL_FILENAME_WORDS_PA       = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_FILENAME_WORDS_PA" ];
        public static readonly string SYNTAX_TEMPLATE_FILENAME_NO_WORDS    = ConfigurationManager.AppSettings[ "SYNTAX_TEMPLATE_FILENAME_NO_WORDS" ];
        public static readonly string SYNTAX_TEMPLATE_FILENAME_WORDS       = ConfigurationManager.AppSettings[ "SYNTAX_TEMPLATE_FILENAME_WORDS" ];
        public static readonly SyntaxModelTypeEnum    SYNTAX_MODEL_TYPE    = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_TYPE"    ].ToEnum< SyntaxModelTypeEnum >();
        public static readonly SyntaxModelSubTypeEnum SYNTAX_MODEL_SUBTYPE = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_SUBTYPE" ].ToEnum< SyntaxModelSubTypeEnum >();

        private static string[] ToFilesArray( this string value )
        {
            var array = value.Split( new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries )
                             .Select( f => f.Trim() )
                             .ToArray();
            return (array);
        }
    }
}
