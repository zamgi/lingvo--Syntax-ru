using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lingvo.morphology
{
    /// <summary>
    /// 
    /// </summary>
    internal interface ITreeDictionary
    {
        void AddWord( string word, MorphoType morphoType, MorphoAttributePair? nounType );

        bool GetWordFormMorphologies( string wordUpper, List< WordFormMorphology_t > result, WordFormMorphologyModeEnum wordFormMorphologyMode );
        unsafe bool GetWordFormMorphologies( char*  wordUpper, List< WordFormMorphology_t > result, WordFormMorphologyModeEnum wordFormMorphologyMode );
        bool GetWordForms( string wordUpper, List< WordForm_t > result );
    }
}
