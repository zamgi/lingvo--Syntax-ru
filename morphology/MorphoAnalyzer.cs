using System.Collections.Generic;

using lingvo.core;

namespace lingvo.morphology
{
    /// <summary>
    /// Морфо-анализатор
	/// </summary>
	public sealed class MorphoAnalyzer
    {
        #region [.private field's.]
        /// морфо-модель
        private readonly IMorphoModel                           _MorphoModel;
        private readonly List< WordFormMorphology_t >           _WordFormMorphologies;
        private readonly List< WordForm_t >                     _WordForms;
        private readonly Dictionary< string, PartOfSpeechEnum > _UniqueWordFormsDictionary;
        #endregion

        public MorphoAnalyzer( IMorphoModel model )
        {
            model.ThrowIfNull( nameof(model) );

            _MorphoModel               = model;
            _WordFormMorphologies      = new List< WordFormMorphology_t >();
            _WordForms                 = new List< WordForm_t >();
            _UniqueWordFormsDictionary = new Dictionary< string, PartOfSpeechEnum >();
        }

		/// получение морфологической информации
		/// words - слова
        public WordMorphology_t GetWordMorphology( string word ) => GetWordMorphology_NoToUpper( StringsHelper.ToUpperInvariant( word ), WordFormMorphologyModeEnum.Default );
        public WordMorphology_t GetWordMorphology( string word, WordFormMorphologyModeEnum wordFormMorphologyMode ) => GetWordMorphology_NoToUpper( StringsHelper.ToUpperInvariant( word ), wordFormMorphologyMode );
        public WordMorphology_t GetWordMorphology_NoToUpper( string wordUpper ) => GetWordMorphology_NoToUpper( wordUpper, WordFormMorphologyModeEnum.Default );
        public WordMorphology_t GetWordMorphology_NoToUpper( string wordUpper, WordFormMorphologyModeEnum wordFormMorphologyMode )
        {
            var wordMorphology = new WordMorphology_t( /*wordUpper*/ );

            if ( _MorphoModel.TryGetWordFormMorphologies( wordUpper, _WordFormMorphologies, wordFormMorphologyMode ) )
			{
                var len = _WordFormMorphologies.Count;
                switch ( len )
                {
                    case 0: break;
                    case 1:
                        wordMorphology.IsSinglePartOfSpeech = true;
                        wordMorphology.PartOfSpeech = _WordFormMorphologies[ 0 ].PartOfSpeech;

                        wordMorphology.WordFormMorphologies = _WordFormMorphologies;
                    break;

                    default:
                        for ( int i = 0; i < len; i++ )
                        {
                            var pos = _WordFormMorphologies[ i ].PartOfSpeech;
                            if ( i == 0 )
                                wordMorphology.IsSinglePartOfSpeech = true;
                            else
                                wordMorphology.IsSinglePartOfSpeech &= (wordMorphology.PartOfSpeech == pos);
                            wordMorphology.PartOfSpeech |= pos;
                        }

                        wordMorphology.WordFormMorphologies = _WordFormMorphologies;
                    break;
                }
			}

            return (wordMorphology);
        }
        unsafe public WordMorphology_t GetWordMorphology_4LastValueUpperInNumeralChain( string wordUpper, WordFormMorphologyModeEnum wordFormMorphologyMode )
        {
            fixed ( char* wordUpper_ptr = wordUpper )
            {
                return (_GetWordMorphology_4LastValueUpperInNumeralChain( wordUpper_ptr, wordFormMorphologyMode ));
            }
        }
        unsafe private WordMorphology_t _GetWordMorphology_4LastValueUpperInNumeralChain( char* wordUpper, WordFormMorphologyModeEnum wordFormMorphologyMode )
        {
            var wordMorphology = new WordMorphology_t();

            if ( _MorphoModel.TryGetWordFormMorphologies( wordUpper, _WordFormMorphologies, wordFormMorphologyMode ) )
			{
                var len = _WordFormMorphologies.Count;
                switch ( len )
                {
                    case 0: break;
                    case 1:
                        wordMorphology.IsSinglePartOfSpeech = true;
                        wordMorphology.PartOfSpeech = _WordFormMorphologies[ 0 ].PartOfSpeech;

                        wordMorphology.WordFormMorphologies = _WordFormMorphologies;
                    break;

                    default:
                        for ( int i = 0; i < len; i++ )
                        {
                            var pos = _WordFormMorphologies[ i ].PartOfSpeech;
                            if ( i == 0 )
                                wordMorphology.IsSinglePartOfSpeech = true;
                            else
                                wordMorphology.IsSinglePartOfSpeech &= (wordMorphology.PartOfSpeech == pos);
                            wordMorphology.PartOfSpeech |= pos;
                        }

                        wordMorphology.WordFormMorphologies = _WordFormMorphologies;
                    break;
                }
			}

            return (wordMorphology);
        }		

		/// получение форм слова
		/// word - слово
		/// pos - часть речи
        public WordForms_t GetWordForms( string word ) => GetWordFormsByPartOfSpeech( word, PartOfSpeechEnum.Other );
        public WordForms_t GetWordForms_NoToUpper( string wordUpper ) => GetWordFormsByPartOfSpeech_NoToUpper( wordUpper, PartOfSpeechEnum.Other );
		public WordForms_t GetWordFormsByPartOfSpeech( string word, PartOfSpeechEnum partOfSpeechFilter )
		{
            var result = new WordForms_t( word );
            var wordUpper = StringsHelper.ToUpperInvariant( word );

            if ( _MorphoModel.TryGetWordForms( wordUpper, _WordForms ) )
            {
                FillUniqueWordFormsDictionary( partOfSpeechFilter );

                #region [.fill word-forms list.]
                _WordForms.Clear();
			    foreach ( var p in _UniqueWordFormsDictionary )
			    {
                    var form         = p.Key;
				    var partOfSpeech = p.Value;

                    var wf = new WordForm_t( form, partOfSpeech );
                    _WordForms.Add( wf );
			    }
                result.Forms = _WordForms;
                #endregion
            }

			return (result);
		}
        public WordForms_t GetWordFormsByPartOfSpeech_NoToUpper( string wordUpper, PartOfSpeechEnum partOfSpeechFilter )
		{
            var result = new WordForms_t( wordUpper );

            if ( _MorphoModel.TryGetWordForms( wordUpper, _WordForms ) )
            {
                FillUniqueWordFormsDictionary( partOfSpeechFilter );

                #region [.fill word-forms list.]
                _WordForms.Clear();
			    foreach ( var p in _UniqueWordFormsDictionary )
			    {
                    var form         = p.Key;
				    var partOfSpeech = p.Value;

                    var wf = new WordForm_t( form, partOfSpeech );
                    _WordForms.Add( wf );
			    }
                result.Forms = _WordForms;
                #endregion
            }

			return (result);
		}

		/// получение уникальных форм
		/// pForms - все формы
		/// uniqueForms [out] - уникальные формы
		/// pos - часть речи
		/// result - общее число уникальных форм
		private void FillUniqueWordFormsDictionary( PartOfSpeechEnum partOfSpeechFilter )
		{
            _UniqueWordFormsDictionary.Clear();

            for ( int  i = 0, len = _WordForms.Count; i < len; i++ )
			{
                var wordForm = _WordForms[ i ];
				PartOfSpeechEnum partOfSpeechForm = wordForm.PartOfSpeech;
                if ( (partOfSpeechForm & partOfSpeechFilter) != partOfSpeechFilter )
                    continue;

				var wordFormString = wordForm.Form;

                if ( _UniqueWordFormsDictionary.TryGetValue( wordFormString, out var partOfSpeechExists ) )
                {
                    if ( (partOfSpeechExists & partOfSpeechForm) != partOfSpeechForm )
                    {
                        partOfSpeechExists |= partOfSpeechForm;
                        _UniqueWordFormsDictionary[ wordFormString ] = partOfSpeechExists;
                    }
                }
                else
                {
                    _UniqueWordFormsDictionary.Add( wordFormString, partOfSpeechForm );
                }
			}
		}
	}
}