using System;

using lingvo.postagger;
using lingvo.morphology;

namespace lingvo.syntax
{
    /// <summary>
    /// 
    /// </summary>
    public enum SyntaxModelTypeEnum : byte { No_Words, Words, }

    /// <summary>
    /// 
    /// </summary>
    public sealed class SyntaxProcessorConfig
    {
        public SyntaxProcessorConfig( PosTaggerProcessorConfig     config, 
                                      IMorphoModel                 morphoModel,
                                      MorphoAmbiguityResolverModel morphoAmbiguityModel )
        {
            PosTaggerProcessorConfig = config;
            MorphoModel              = morphoModel;
            MorphoAmbiguityModel     = morphoAmbiguityModel;
        }

        public PosTaggerProcessorConfig     PosTaggerProcessorConfig
        {
            get;
            private set;
        }
        public IMorphoModel                 MorphoModel
        {
            get;
            set;
        }
        public MorphoAmbiguityResolverModel MorphoAmbiguityModel
        {
            get;
            set;
        }
        public SyntaxModelTypeEnum          ModelType
        {
            get;
            set;
        }
        public string                       ModelFilename
        {
            get;
            set;
        }
        public string                       TemplateFilename
        {
            get;
            set;
        }
    }
}
