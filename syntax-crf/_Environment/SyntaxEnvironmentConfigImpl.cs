using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace lingvo.syntax
{
    /// <summary>
    /// 
    /// </summary>
    public class SyntaxEnvironmentConfigImpl : SyntaxEnvironmentConfigBase
    {
        public SyntaxEnvironmentConfigImpl()
        {
            RESOURCES_BASE_DIRECTORY = ConfigurationManager.AppSettings[ "RESOURCES_BASE_DIRECTORY" ];

            URL_DETECTOR_RESOURCES_XML_FILENAME  = ConfigurationManager.AppSettings[ "URL_DETECTOR_RESOURCES_XML_FILENAME" ].GetPath( RESOURCES_BASE_DIRECTORY );
            SENT_SPLITTER_RESOURCES_XML_FILENAME = ConfigurationManager.AppSettings[ "SENT_SPLITTER_RESOURCES_XML_FILENAME" ].GetPath( RESOURCES_BASE_DIRECTORY );
            TOKENIZER_RESOURCES_XML_FILENAME     = ConfigurationManager.AppSettings[ "TOKENIZER_RESOURCES_XML_FILENAME" ].GetPath( RESOURCES_BASE_DIRECTORY );

            POSTAGGER_MODEL_FILENAME             = ConfigurationManager.AppSettings[ "POSTAGGER_MODEL_FILENAME" ].GetPath( RESOURCES_BASE_DIRECTORY );
            POSTAGGER_TEMPLATE_FILENAME          = ConfigurationManager.AppSettings[ "POSTAGGER_TEMPLATE_FILENAME" ].GetPath( RESOURCES_BASE_DIRECTORY );
            POSTAGGER_RESOURCES_XML_FILENAME     = ConfigurationManager.AppSettings[ "POSTAGGER_RESOURCES_XML_FILENAME" ].GetPath( RESOURCES_BASE_DIRECTORY );

            MORPHO_BASE_DIRECTORY        = ConfigurationManager.AppSettings[ "MORPHO_BASE_DIRECTORY" ].GetPath( RESOURCES_BASE_DIRECTORY );
            MORPHO_MORPHOTYPES_FILENAMES = ConfigurationManager.AppSettings[ "MORPHO_MORPHOTYPES_FILENAMES" ].ToFilesArray().Select( fn => fn.GetPath( MORPHO_BASE_DIRECTORY ) ).ToArray();
            MORPHO_PROPERNAMES_FILENAMES = ConfigurationManager.AppSettings[ "MORPHO_PROPERNAMES_FILENAMES" ].ToFilesArray().Select( fn => fn.GetPath( MORPHO_BASE_DIRECTORY ) ).ToArray();
            MORPHO_COMMON_FILENAMES      = ConfigurationManager.AppSettings[ "MORPHO_COMMON_FILENAMES"      ].ToFilesArray().Select( fn => fn.GetPath( MORPHO_BASE_DIRECTORY ) ).ToArray();

            MORPHO_AMBIGUITY_MODEL_FILENAME       = ConfigurationManager.AppSettings[ "MORPHO_AMBIGUITY_MODEL_FILENAME" ].GetPath( MORPHO_BASE_DIRECTORY );
            MORPHO_AMBIGUITY_TEMPLATE_FILENAME_5G = ConfigurationManager.AppSettings[ "MORPHO_AMBIGUITY_TEMPLATE_FILENAME_5G" ].GetPath( MORPHO_BASE_DIRECTORY );
            MORPHO_AMBIGUITY_TEMPLATE_FILENAME_3G = ConfigurationManager.AppSettings[ "MORPHO_AMBIGUITY_TEMPLATE_FILENAME_3G" ].GetPath( MORPHO_BASE_DIRECTORY );

            SYNTAX_MODEL_FILENAME_NO_WORDS_AP = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_FILENAME_NO_WORDS_AP" ].GetPath( RESOURCES_BASE_DIRECTORY );
            SYNTAX_MODEL_FILENAME_NO_WORDS_PA = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_FILENAME_NO_WORDS_PA" ].GetPath( RESOURCES_BASE_DIRECTORY );
            SYNTAX_MODEL_FILENAME_WORDS_AP    = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_FILENAME_WORDS_AP"    ].GetPath( RESOURCES_BASE_DIRECTORY );
            SYNTAX_MODEL_FILENAME_WORDS_PA    = ConfigurationManager.AppSettings[ "SYNTAX_MODEL_FILENAME_WORDS_PA"    ].GetPath( RESOURCES_BASE_DIRECTORY );
            SYNTAX_TEMPLATE_FILENAME_NO_WORDS = ConfigurationManager.AppSettings[ "SYNTAX_TEMPLATE_FILENAME_NO_WORDS" ].GetPath( RESOURCES_BASE_DIRECTORY );
            SYNTAX_TEMPLATE_FILENAME_WORDS    = ConfigurationManager.AppSettings[ "SYNTAX_TEMPLATE_FILENAME_WORDS"    ].GetPath( RESOURCES_BASE_DIRECTORY );
            SYNTAX_MODEL_TYPE                 = Enum.Parse< SyntaxModelTypeEnum   >( ConfigurationManager.AppSettings[ "SYNTAX_MODEL_TYPE"    ], true );
            SYNTAX_MODEL_SUBTYPE              = Enum.Parse<SyntaxModelSubTypeEnum >( ConfigurationManager.AppSettings[ "SYNTAX_MODEL_SUBTYPE" ], true );
        }

        public string RESOURCES_BASE_DIRECTORY { get; }
        public override string MORPHO_BASE_DIRECTORY { get; }

        public override string URL_DETECTOR_RESOURCES_XML_FILENAME  { get; }
        public override string SENT_SPLITTER_RESOURCES_XML_FILENAME { get; }
        public override string TOKENIZER_RESOURCES_XML_FILENAME     { get; }

        public override string POSTAGGER_MODEL_FILENAME             { get; }
        public override string POSTAGGER_TEMPLATE_FILENAME          { get; }
        public override string POSTAGGER_RESOURCES_XML_FILENAME     { get; }

        public override string[] MORPHO_MORPHOTYPES_FILENAMES { get; }
        public override string[] MORPHO_PROPERNAMES_FILENAMES { get; }
        public override string[] MORPHO_COMMON_FILENAMES      { get; }

        public override string MORPHO_AMBIGUITY_MODEL_FILENAME       { get; }
        public override string MORPHO_AMBIGUITY_TEMPLATE_FILENAME_5G { get; }
        public override string MORPHO_AMBIGUITY_TEMPLATE_FILENAME_3G { get; }

        public override string SYNTAX_MODEL_FILENAME_NO_WORDS_AP { get; }
        public override string SYNTAX_MODEL_FILENAME_NO_WORDS_PA { get; }
        public override string SYNTAX_MODEL_FILENAME_WORDS_AP    { get; }
        public override string SYNTAX_MODEL_FILENAME_WORDS_PA    { get; }
        public override string SYNTAX_TEMPLATE_FILENAME_NO_WORDS { get; }
        public override string SYNTAX_TEMPLATE_FILENAME_WORDS    { get; }
        public override SyntaxModelTypeEnum    SYNTAX_MODEL_TYPE    { get; }
        public override SyntaxModelSubTypeEnum SYNTAX_MODEL_SUBTYPE { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class SyntaxEnvironmentConfigExtensions
    {
        public static string GetPath( this string relPath, string basePath ) => Path.Combine( basePath, relPath?.TrimStart( '/', '\\' ) );
        public static string[] ToFilesArray( this string value ) => value.Split( new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries ).Select( f => f.Trim() ).ToArray();
    }
}
