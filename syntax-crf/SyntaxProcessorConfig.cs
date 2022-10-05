using lingvo.morphology;
using lingvo.postagger;

namespace lingvo.syntax
{
    /// <summary>
    /// 
    /// </summary>
    public enum SyntaxModelTypeEnum : byte { No_Words, Words, }

    /// <summary>
    /// 
    /// </summary>
    public struct SyntaxProcessorConfig
    {
        public SyntaxProcessorConfig( PosTaggerProcessorConfig     config, 
                                      IMorphoModel                 morphoModel,
                                      MorphoAmbiguityResolverModel morphoAmbiguityModel ) : this()
        {
            PosTaggerProcessorConfig = config;
            MorphoModel              = morphoModel;
            MorphoAmbiguityModel     = morphoAmbiguityModel;
        }

        public PosTaggerProcessorConfig     PosTaggerProcessorConfig { get; }
        public IMorphoModel                 MorphoModel              { get; set; }
        public MorphoAmbiguityResolverModel MorphoAmbiguityModel     { get; set; }
        public SyntaxModelTypeEnum          ModelType                { get; set; }
        public string                       ModelFilename            { get; set; }
        public string                       TemplateFilename         { get; set; }
    }
}
