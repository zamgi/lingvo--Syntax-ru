using System;

using lingvo.morphology;
using lingvo.postagger;
using lingvo.sentsplitting;
using lingvo.tokenizing;
using TreeDictionaryTypeEnum = lingvo.morphology.MorphoModelConfig.TreeDictionaryTypeEnum;

namespace lingvo.syntax
{
    /// <summary>
    /// 
    /// </summary>
    public enum SyntaxModelSubTypeEnum { AP, PA, }

    /// <summary>
    /// 
    /// </summary>
    public interface ISyntaxEnvironmentConfig
    {
        string URL_DETECTOR_RESOURCES_XML_FILENAME  { get; }
        string SENT_SPLITTER_RESOURCES_XML_FILENAME { get; }        
        string TOKENIZER_RESOURCES_XML_FILENAME     { get; }
        string POSTAGGER_MODEL_FILENAME             { get; }
        string POSTAGGER_TEMPLATE_FILENAME          { get; }
        string POSTAGGER_RESOURCES_XML_FILENAME     { get; }

        string   MORPHO_BASE_DIRECTORY        { get; }
        string[] MORPHO_MORPHOTYPES_FILENAMES { get; }
        string[] MORPHO_PROPERNAMES_FILENAMES { get; }
        string[] MORPHO_COMMON_FILENAMES      { get; }

        string MORPHO_AMBIGUITY_MODEL_FILENAME       { get; }
        string MORPHO_AMBIGUITY_TEMPLATE_FILENAME_5G { get; }
        string MORPHO_AMBIGUITY_TEMPLATE_FILENAME_3G { get; }

        string SYNTAX_MODEL_FILENAME_NO_WORDS_AP { get; }
        string SYNTAX_MODEL_FILENAME_NO_WORDS_PA { get; }
        string SYNTAX_MODEL_FILENAME_WORDS_AP    { get; }
        string SYNTAX_MODEL_FILENAME_WORDS_PA    { get; }
        string SYNTAX_TEMPLATE_FILENAME_NO_WORDS { get; }
        string SYNTAX_TEMPLATE_FILENAME_WORDS    { get; }
        SyntaxModelTypeEnum    SYNTAX_MODEL_TYPE    { get; }
        SyntaxModelSubTypeEnum SYNTAX_MODEL_SUBTYPE { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class SyntaxEnvironmentConfigBase : ISyntaxEnvironmentConfig
    {
        public abstract string TOKENIZER_RESOURCES_XML_FILENAME     { get; }
        public abstract string POSTAGGER_MODEL_FILENAME             { get; }
        public abstract string POSTAGGER_TEMPLATE_FILENAME          { get; }
        public abstract string POSTAGGER_RESOURCES_XML_FILENAME     { get; }
        public abstract string SENT_SPLITTER_RESOURCES_XML_FILENAME { get; }
        public abstract string URL_DETECTOR_RESOURCES_XML_FILENAME  { get; }

        public abstract string   MORPHO_BASE_DIRECTORY         { get; }
        public abstract string[] MORPHO_MORPHOTYPES_FILENAMES  { get; }
        public abstract string[] MORPHO_PROPERNAMES_FILENAMES  { get; }
        public abstract string[] MORPHO_COMMON_FILENAMES       { get; }

        public abstract string MORPHO_AMBIGUITY_MODEL_FILENAME       { get; }
        public abstract string MORPHO_AMBIGUITY_TEMPLATE_FILENAME_5G { get; }
        public abstract string MORPHO_AMBIGUITY_TEMPLATE_FILENAME_3G { get; }

        public abstract string SYNTAX_MODEL_FILENAME_NO_WORDS_AP { get; }
        public abstract string SYNTAX_MODEL_FILENAME_NO_WORDS_PA { get; }
        public abstract string SYNTAX_MODEL_FILENAME_WORDS_AP    { get; }
        public abstract string SYNTAX_MODEL_FILENAME_WORDS_PA    { get; }
        public abstract string SYNTAX_TEMPLATE_FILENAME_NO_WORDS { get; }
        public abstract string SYNTAX_TEMPLATE_FILENAME_WORDS    { get; }
        public abstract SyntaxModelTypeEnum    SYNTAX_MODEL_TYPE    { get; }
        public abstract SyntaxModelSubTypeEnum SYNTAX_MODEL_SUBTYPE { get; }

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
        public (PosTaggerProcessorConfig config, SentSplitterConfig ssc) CreatePosTaggerProcessorConfig( LanguageTypeEnum languageType = LanguageTypeEnum.Ru )
        {
            var sentSplitterConfig = new SentSplitterConfig( SENT_SPLITTER_RESOURCES_XML_FILENAME, URL_DETECTOR_RESOURCES_XML_FILENAME );
            var config = new PosTaggerProcessorConfig( TOKENIZER_RESOURCES_XML_FILENAME, POSTAGGER_RESOURCES_XML_FILENAME, languageType, sentSplitterConfig )
            {
                ModelFilename    = POSTAGGER_MODEL_FILENAME,
                TemplateFilename = POSTAGGER_TEMPLATE_FILENAME,
            };

            return (config, sentSplitterConfig);
        }
        public MorphoModelConfig CreateMorphoModelConfig( Action< string, string > modelLoadingErrorCallback = null ) => new MorphoModelConfig()
        {
            TreeDictionaryType   = TreeDictionaryTypeEnum.Native,
            BaseDirectory        = MORPHO_BASE_DIRECTORY,
            MorphoTypesFilenames = MORPHO_MORPHOTYPES_FILENAMES,
            ProperNamesFilenames = MORPHO_PROPERNAMES_FILENAMES,
            CommonFilenames      = MORPHO_COMMON_FILENAMES,
            ModelLoadingErrorCallback = modelLoadingErrorCallback ?? ((s1, s2) => { })
        };
        public MorphoAmbiguityResolverConfig CreateMorphoAmbiguityConfig() => new MorphoAmbiguityResolverConfig()
        {
            ModelFilename       = MORPHO_AMBIGUITY_MODEL_FILENAME,
            TemplateFilename_5g = MORPHO_AMBIGUITY_TEMPLATE_FILENAME_5G,
            TemplateFilename_3g = MORPHO_AMBIGUITY_TEMPLATE_FILENAME_3G,
        };
        public static MorphoAmbiguityResolverModel CreateMorphoAmbiguityResolverModel( in MorphoAmbiguityResolverConfig config ) => new MorphoAmbiguityResolverModel( config );
    }
}
