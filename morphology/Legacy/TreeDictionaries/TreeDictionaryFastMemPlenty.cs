using System;
using System.Collections.Generic;
using System.Linq;

using lingvo.core;
using M = System.Runtime.CompilerServices.MethodImplAttribute;
using O = System.Runtime.CompilerServices.MethodImplOptions;

namespace lingvo.morphology
{
    /// <summary>
    /// Словарь-дерево для слов
    /// </summary>
    internal sealed class TreeDictionaryFastMemPlenty : ITreeDictionary
    {
        /// <summary>
        /// 
        /// </summary>
        private struct pair_t
        {
            public pair_t( BaseMorphoForm baseMorphoForm, MorphoAttributeEnum morphoAttribute )
            {
                BaseMorphoForm  = baseMorphoForm;
                MorphoAttribute = morphoAttribute;
            }

            public BaseMorphoForm      BaseMorphoForm;
            public MorphoAttributeEnum MorphoAttribute;
        }

	    /// слот для дочерних слов
	    private readonly Dictionary< char, TreeDictionaryFastMemPlenty > _Slots;
	    /// коллекция информаций о формах слова
        private Dictionary< string, pair_t[] > _EndingDictionary;
        public TreeDictionaryFastMemPlenty() => _Slots = new Dictionary< char, TreeDictionaryFastMemPlenty >();

        #region [.append word.]
        /// добавление слова и всех его форм в словарь
	    /// word - слово
	    /// pMorphoType - морфотип
	    /// nounType - тип сущетсвительного
        [M(O.AggressiveInlining)] unsafe public void AddWord( string word, MorphoType morphoType, MorphoAttributePair? nounType )
        {
            if ( morphoType.MorphoForms.Length != 0 )
	        {
                var len   = word.Length - morphoType.MorphoForms[ 0 ].Ending.Length;
                var _base = (0 <= len) ? word.Substring( 0, len ) : word;
                var baseMorphoForm = new BaseMorphoForm( _base, morphoType, nounType );
                var baseUpper      = StringsHelper.ToUpperInvariant( _base );
                fixed ( char* baseUpper_ptr = baseUpper )
                {
                    AddWordPart( baseUpper_ptr, baseMorphoForm );
                }
	        }
        }

	    /// добавление части слова
	    /// wordPart - оставшася часть слова
	    /// pBase - базовая форма
        [M(O.AggressiveInlining)] unsafe private void AddWordPart( char* wordPart, BaseMorphoForm baseMorphoForm )
        {
            var first_char = *wordPart;
            if ( first_char == '\0' ) // сохранение характеристик
	        {
                var tuples = from morphoForm in baseMorphoForm.MorphoForms
                                select 
                                    new 
                                    { 
                                        ending         = morphoForm.EndingUpper, //string.Intern( StringsHelper.ToUpperInvariant( morphoForm.Ending ) ), 
                                        baseMorphoForm = baseMorphoForm,
                                        morphoForm     = morphoForm,
                                    }
                                ;
                var dict = new Dictionary< string, LinkedList< pair_t > >();
                foreach ( var t in tuples )
                {
                    if ( !dict.TryGetValue( t.ending, out var pairs ) )
                    {
                        pairs = new LinkedList< pair_t >();
                        dict.Add( t.ending, pairs );
                    }
                    //var morphoAttribute = ;
                    pairs.AddLast( new pair_t( t.baseMorphoForm, MorphoAttributePair.GetMorphoAttribute( t.baseMorphoForm, t.morphoForm ) ) );
                }

                if ( _EndingDictionary == null ) _EndingDictionary = new Dictionary< string, pair_t[] >();
                foreach ( var p in dict )
                {
                    if ( !_EndingDictionary.TryGetValue( p.Key, out var pairs ) )
                    {
                        _EndingDictionary.Add( p.Key, p.Value.ToArray() );
                    }
                    else
                    {
                        _EndingDictionary[ p.Key ] = pairs.Concat( p.Value ).ToArray();
                    }
                }
	        }
	        else
	        {
                if ( !_Slots.TryGetValue( first_char, out var _next ) )
                {
                    //добавление новой буквы
                    _next = new TreeDictionaryFastMemPlenty();
                    _Slots.Add( first_char, _next );
                }
                _next.AddWordPart( wordPart + 1, baseMorphoForm );
	        }
        }
        #endregion

        /// получение морфологических свойств слова
	    /// word - слово
	    /// result - коллекция информаций о формах слова	    
        [M(O.AggressiveInlining)] unsafe public bool GetWordFormMorphologies( string wordUpper, ICollection< WordFormMorphology_t > result, WordFormMorphologyModeEnum wordFormMorphologyMode )
        {
            result.Clear();
            fixed ( char* word_ptr = wordUpper )
            {
                FillWordFormMorphologies( word_ptr, result, wordFormMorphologyMode );
            }
            return (result.Count != 0);
        }
        [M(O.AggressiveInlining)] unsafe public bool GetWordFormMorphologies( char*  wordUpper, ICollection< WordFormMorphology_t > result, WordFormMorphologyModeEnum wordFormMorphologyMode )
        {
            result.Clear();            
            {
                FillWordFormMorphologies( wordUpper, result, wordFormMorphologyMode );
            }
            return (result.Count != 0);
        }

	    /// получение всех форм слова
	    /// word - слово
	    /// result - коллекция форм слова	    
        [M(O.AggressiveInlining)] unsafe public bool GetWordForms( string wordUpper, ICollection< WordForm_t > result )
        {
            result.Clear();
            fixed ( char* word_ptr = wordUpper )
            {
                FillWordForms( word_ptr, result );
            }
            return (result.Count != 0);
        }

	    /// поиск слова в словаре
	    /// word - слово
	    /// letterIndex - индекс буквы
	    /// result - коллекция форм слова      
        unsafe private void FillWordFormMorphologies( char* word, ICollection< WordFormMorphology_t > result, WordFormMorphologyModeEnum wordFormMorphologyMode )
        {
            FillWordFormMorphologies_Core( word, result, wordFormMorphologyMode );
            var first_char = *word;
            if ( (first_char != '\0') && _Slots.TryGetValue( first_char, out var _next ) )
            {
                _next.FillWordFormMorphologies( word + 1, result, wordFormMorphologyMode );
            }
        }
        unsafe private void FillWordForms( char* word, ICollection< WordForm_t > result )
        {
            FillWordForms_Core( word, result );
            var first_char = *word;
            if ( (first_char != '\0') && _Slots.TryGetValue( first_char, out var _next ) )
            {
                _next.FillWordForms( word + 1, result );
            }
        }

	    /// поиск слова в слоте
	    /// wordPart - оставшаяся часть слова
	    /// pSlot - слот
	    /// letterIndex - индекс буквы
        unsafe private void FillWordFormMorphologies_Core( char* wordPart, ICollection< WordFormMorphology_t > result, WordFormMorphologyModeEnum wordFormMorphologyMode )
        {
            if ( _EndingDictionary == null )
                return;

            var ending = new string( wordPart );
            if ( _EndingDictionary.TryGetValue( ending, out var pairs ) )
            {
                switch ( wordFormMorphologyMode )
                {
                    case WordFormMorphologyModeEnum.Default:
                        foreach ( var p in pairs )
                        {
                            var wfmi = new WordFormMorphology_t( p.BaseMorphoForm, p.MorphoAttribute );
                            result.Add( wfmi ); 
                        }
                    break;

                    case WordFormMorphologyModeEnum.StartsWithLowerLetter:
                        foreach ( var p in pairs )
                        {
                            var baseMorphoForm = p.BaseMorphoForm;
                            fixed ( char* normalForm_ptr = baseMorphoForm.NormalForm )
                            {
                                var first_char = *normalForm_ptr;
                                if ( (first_char != '\0') && *(xlat_Unsafe.Inst._UPPER_INVARIANT_MAP + first_char) == first_char )
                                {
                                    continue;
                                }
                            }

                            var wfmi = new WordFormMorphology_t( baseMorphoForm, p.MorphoAttribute );
                            result.Add( wfmi ); 
                        }
                    break;

                    case WordFormMorphologyModeEnum.StartsWithUpperLetter:
                        foreach ( var p in pairs )
                        {
                            var baseMorphoForm = p.BaseMorphoForm;
                            fixed ( char* normalForm_ptr = baseMorphoForm.NormalForm )
                            {
                                var first_char = *normalForm_ptr;
                                if ( (first_char != '\0') && *(xlat_Unsafe.Inst._UPPER_INVARIANT_MAP + first_char) != first_char )
                                {
                                    continue;
                                }
                            }

                            var wfmi = new WordFormMorphology_t( baseMorphoForm, p.MorphoAttribute );
                            result.Add( wfmi ); 
                        }
                    break;

                    case WordFormMorphologyModeEnum.FirstStartsWithUpperAfterLowerLetter:
                    #region
                        throw new NotImplementedException();

                        /*
                        var findWithUpper = false;
                        foreach ( var p in pairs )
                        {
                            var baseMorphoForm = p.BaseMorphoForm;
                            fixed ( char* normalForm_ptr = baseMorphoForm.NormalForm )
                            {
                                var first_char = *normalForm_ptr;
                                if ( (first_char != '\0') && *(xlat_Unsafe.Inst._UPPER_INVARIANT_MAP + first_char) != first_char )
                                {
                                    continue;
                                }
                            }

                            findWithUpper = true;
                            var wfmi = new WordFormMorphology_t( baseMorphoForm, p.MorphoAttribute );
                            result.Add( wfmi ); 
                        }
                        if ( !findWithUpper )
                        {
                            goto case WordFormMorphologyModeEnum.Default;
                        }
                        */
                    #endregion
                    break;

                    case WordFormMorphologyModeEnum.FirstStartsWithLowerAfterUpperLetter:
                    #region
                        throw new NotImplementedException();

                        /*
                        var findWithLower = false;
                        foreach ( var p in pairs )
                        {
                            var baseMorphoForm = p.BaseMorphoForm;
                            fixed ( char* normalForm_ptr = baseMorphoForm.NormalForm )
                            {
                                var first_char = *normalForm_ptr;
                                if ( (first_char != '\0') && *(xlat_Unsafe.Inst._UPPER_INVARIANT_MAP + first_char) == first_char )
                                {
                                    continue;
                                }
                            }

                            findWithLower = true;
                            var wfmi = new WordFormMorphology_t( baseMorphoForm, p.MorphoAttribute );
                            result.Add( wfmi );
                        }
                        if ( !findWithLower )
                        {
                            goto case WordFormMorphologyModeEnum.Default;
                        }
                        */
                    #endregion
                    break;
                }
            }
        }
        unsafe private void FillWordForms_Core( char* wordPart, ICollection< WordForm_t > result )
        {
            if ( _EndingDictionary == null )
                return;

            var ending = new string( wordPart );
            if ( _EndingDictionary.TryGetValue( ending, out var pairs ) )
            {
                foreach ( var p in pairs )
                {
                    var partOfSpeech = p.BaseMorphoForm.PartOfSpeech;
                    var _base        = p.BaseMorphoForm.Base;
                    foreach ( var morphoForm in p.BaseMorphoForm.MorphoForms )
                    {
                        // получение словоформы
                        var wordForm = _base + morphoForm.Ending;

                        var wf = new WordForm_t( wordForm, partOfSpeech );
                        result.Add( wf ); 
                    }
                }
            }
        }

    }
}