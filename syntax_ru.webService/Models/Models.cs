using System;
using System.Collections.Generic;
using System.Linq;

using lingvo.morphology;
using lingvo.postagger;
using lingvo.tokenizing;
using JP = System.Text.Json.Serialization.JsonPropertyNameAttribute;

namespace lingvo.syntax.webService
{
    /// <summary>
    /// 
    /// </summary>
    public struct InitParamsVM
    {
        public string Text          { get; set; }
        public bool   SplitBySmiles { get; set; }
#if DEBUG
        public override string ToString() => Text;
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    internal readonly struct ResultVM
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly struct morpho_info
        {
            public morpho_info( in WordFormMorphology_t morphology )
            {
                normalForm      = morphology.NormalForm;
                partOfSpeech    = morphology.PartOfSpeech.ToString();
                morphoAttribute = !morphology.IsEmptyMorphoAttribute() ? morphology.MorphoAttribute.ToString() : "-";
            }
            [JP("nf")]  public string normalForm      { get; init; }
            [JP("pos")] public string partOfSpeech    { get; init; }
            [JP("ma")]  public string morphoAttribute { get; init; }
        }
        /// <summary>
        /// 
        /// </summary>
        public readonly struct word_info
        {
            [JP("i")]      public int          startIndex          { get; init; }
            [JP("l")]      public int          length              { get; init; }
            //[JP("v")]      public string       value             { get; init; }
            [JP("p")]      public bool         isPunctuation       { get; init; }
            [JP("pos")]    public string       posTaggerOutputType { get; init; }
            [JP("stx")]    public string       syntaxRoleType      { get; init; }
            [JP("morpho")] public morpho_info? morpho              { get; init; }
        }

        public ResultVM( in InitParamsVM m, Exception ex ) : this() => (init_params, exception_message) = (m, ex.Message);
        public ResultVM( in InitParamsVM m, List< word_t[] > sents ) : this()
        {
            init_params = m;
            sentInfos = new List< word_info[] >( sents.Count );

            foreach ( var words_by_sent in sents )
            {
                var words = (from word in words_by_sent
                                select
                                    new word_info()
                                    {
                                        startIndex          = word.startIndex,
                                        length              = word.length,
                                        /*value               = word.valueOriginal,*/
                                        posTaggerOutputType = word.posTaggerOutputType.ToString(),
                                        isPunctuation       = (word.posTaggerExtraWordType == PosTaggerExtraWordType.Punctuation),
                                        syntaxRoleType      = word.syntaxRoleType.ToText(),
                                        morpho              = !word.morphology.IsEmpty() ? new morpho_info( word.morphology ) : null,
                                    }
                            ).ToArray();
                sentInfos.Add( words );
            }
        }

        [JP("ip")   ] public InitParamsVM        init_params       { get; }
        [JP("sents")] public List< word_info[] > sentInfos         { get; }
        [JP("err") ] public string               exception_message { get; }
    }
}
