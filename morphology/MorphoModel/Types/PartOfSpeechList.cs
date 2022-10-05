using System.Collections.Generic;

using M = System.Runtime.CompilerServices.MethodImplAttribute;
using O = System.Runtime.CompilerServices.MethodImplOptions;

namespace lingvo.morphology
{
    /// <summary>
    /// части речи
    /// </summary>
    internal sealed class PartOfSpeechList
    {
        /// название части речи - часть речи
        private readonly Dictionary< PartOfSpeechEnum, PartOfSpeechBase > _Dictionary;
	    public PartOfSpeechList()
        {
            _Dictionary = new Dictionary< PartOfSpeechEnum, PartOfSpeechBase >( 20 );

            AddPartOfSpeech( new Noun() );
            AddPartOfSpeech( new Adjective() );
            AddPartOfSpeech( new Pronoun() );
            AddPartOfSpeech( new Numeral() );
            AddPartOfSpeech( new Verb() );
            AddPartOfSpeech( new Adverb() );
            AddPartOfSpeech( new Conjunction() );
            AddPartOfSpeech( new Preposition() );
            AddPartOfSpeech( new Interjection() );
            AddPartOfSpeech( new Particle() );
            AddPartOfSpeech( new Article() );
            AddPartOfSpeech( new Other() );
            AddPartOfSpeech( new Predicate() );
        }

        /// добавление части речи
        [M(O.AggressiveInlining)] private void AddPartOfSpeech( PartOfSpeechBase partOfSpeech ) => _Dictionary.Add( partOfSpeech.PartOfSpeech, partOfSpeech );

	    /// Получение части речи по ее названию
        [M(O.AggressiveInlining)] public PartOfSpeechBase GetPartOfSpeech( PartOfSpeechEnum partOfSpeech ) => _Dictionary.TryGetValue( partOfSpeech, out var value ) ? value : null;
    }
}