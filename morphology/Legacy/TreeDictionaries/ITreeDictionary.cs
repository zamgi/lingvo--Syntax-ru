using System.Collections.Generic;

namespace lingvo.morphology
{
    /// <summary>
    /// 
    /// </summary>
    internal interface ITreeDictionary
    {
        void AddWord( string word, MorphoType morphoType, MorphoAttributePair? nounType );

        bool GetWordFormMorphologies( string wordUpper, ICollection< WordFormMorphology_t > result, WordFormMorphologyModeEnum wordFormMorphologyMode );
        unsafe bool GetWordFormMorphologies( char*  wordUpper, ICollection< WordFormMorphology_t > result, WordFormMorphologyModeEnum wordFormMorphologyMode );
        bool GetWordForms( string wordUpper, ICollection< WordForm_t > result );
    }
}
