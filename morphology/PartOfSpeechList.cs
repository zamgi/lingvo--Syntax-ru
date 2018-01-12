using System;
using System.Collections.Generic;

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
        private void AddPartOfSpeech( PartOfSpeechBase partOfSpeech )
        {
            _Dictionary.Add( partOfSpeech.PartOfSpeech, partOfSpeech );
        }

	    /// Получение части речи по ее названию
        public PartOfSpeechBase GetPartOfSpeech( PartOfSpeechEnum partOfSpeech )
        {
	        PartOfSpeechBase value;
            if ( _Dictionary.TryGetValue( partOfSpeech, out value ) )
            {
                return (value);
            }
	        return (null);
        }
    }
}