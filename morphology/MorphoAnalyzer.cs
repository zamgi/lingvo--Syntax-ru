using System;
using System.Collections.Generic;

using lingvo.core;

namespace lingvo.morphology
{
    #region [.commented. declaration move to lingvo.core.dll.]
    /*
	/// <summary>
    /// форма слова
	/// </summary>
	public struct WordForm_t
	{
        public WordForm_t( string form, PartOfSpeechEnum partOfSpeech )
        {
            Form         = form;
            PartOfSpeech = partOfSpeech;
        }

		/// форма
		public string Form;
		/// часть речи
		public PartOfSpeechEnum PartOfSpeech;

        public override string ToString()
        {
            return ('[' + Form + ", " + PartOfSpeech + ']');
        }
	}

	/// <summary>
    /// формы слова
	/// </summary>
	public struct WordForms_t
	{
        private static readonly List< WordForm_t > EMPTY = new List< WordForm_t >( 0 );

        public WordForms_t( string word )
        {
            Word  = word;
            Forms = EMPTY;
        }

		/// исходное слово
		public string Word;
		/// формы слова
		public List< WordForm_t > Forms;

        public bool HasForms
        {
            get { return (Forms != null && Forms.Count != 0); }
        }

        public override string ToString()
        {
            return ('[' + Word + ", {" + string.Join( ",", Forms ) + "}]");
        }
	}

    /// <summary>
    /// морфохарактеристики формы слова
    /// </summary>
    unsafe public struct WordFormMorphology_t
    {
        internal WordFormMorphology_t( BaseMorphoFormNative baseMorphoForm, MorphoAttributeEnum morphoAttribute ) 
            : this()
        {
            _Base           = baseMorphoForm.Base;
            _Ending         = baseMorphoForm.MorphoFormEndings[ 0 ];
            PartOfSpeech    = baseMorphoForm.PartOfSpeech;
            MorphoAttribute = morphoAttribute;
        }
        internal WordFormMorphology_t( BaseMorphoForm baseMorphoForm, MorphoAttributeEnum morphoAttribute )
            : this()
        {
            _NormalForm     = baseMorphoForm.NormalForm;
            PartOfSpeech    = baseMorphoForm.PartOfSpeech;
            MorphoAttribute = morphoAttribute;
        }
        public WordFormMorphology_t( PartOfSpeechEnum partOfSpeech )
            : this()
        {
            PartOfSpeech = partOfSpeech;
        }
        public WordFormMorphology_t( PartOfSpeechEnum partOfSpeech, MorphoAttributeEnum morphoAttribute )
            : this()
        {
            PartOfSpeech    = partOfSpeech;
            MorphoAttribute = morphoAttribute;
        }

        private readonly char* _Base;
        private readonly char* _Ending;

        private string _NormalForm;
        /// нормальная форма
        public string NormalForm
        {
            get
            {
                if ( _NormalForm == null )
                {
                    if ( (IntPtr) _Base != IntPtr.Zero )
                    {
                        _NormalForm = StringsHelper.CreateWordForm( _Base, _Ending );
                    }
                }
                return (_NormalForm);
            }
        }
        /// часть речи
        public readonly PartOfSpeechEnum PartOfSpeech;
        /// морфохарактеристики
        public readonly MorphoAttributeEnum MorphoAttribute;

        public bool IsEmpty()
        {
            return ((MorphoAttribute == MorphoAttributeEnum.__UNDEFINED__) &&
                    (PartOfSpeech == PartOfSpeechEnum.Other)               &&
                    ((_NormalForm == null) && ((IntPtr) _Base == IntPtr.Zero))
                   );
        }
        public bool IsEmptyMorphoAttribute()
        {
            return (MorphoAttribute == MorphoAttributeEnum.__UNDEFINED__);
        }
        public bool IsEmptyNormalForm()
        {
            return ((_NormalForm == null) && ((IntPtr) _Base == IntPtr.Zero));
        }

        public override string ToString()
        {
            return ('[' + NormalForm + ", " + PartOfSpeech + ", " + MorphoAttribute + "]");
        }
    }

	/// <summary>
    /// информация о морфологических свойствах слова
	/// </summary>
	public struct WordMorphology_t
	{
		/// часть речи
		public PartOfSpeechEnum PartOfSpeech;
        public bool             IsSinglePartOfSpeech;
		/// массив морфохарактеристик
		public List< WordFormMorphology_t > WordFormMorphologies;

        public bool HasWordFormMorphologies
        {
            get { return (WordFormMorphologies != null && WordFormMorphologies.Count != 0); }
        }

        public override string ToString()
        {
            return ("[" PartOfSpeech + ", {" + string.Join( ",", WordFormMorphologies ) + "}]");
        }
	}
    */
    #endregion

    /// <summary>
    /// Морфо-анализатор
	/// </summary>
	public sealed class MorphoAnalyzer
    {
        #region [.private field's.]
        private const int DEFAULT_BUFFER_4_UPPER_SIZE = 256;
        /// морфо-модель
        private readonly IMorphoModel                           _MorphoModel;
        private readonly List< WordFormMorphology_t >           _WordFormMorphologies;
        private readonly List< WordForm_t >                     _WordForms;
        private readonly Dictionary< string, PartOfSpeechEnum > _UniqueWordFormsDictionary;
        private readonly char[]                                 _Buffer4Upper;
        #endregion

        public MorphoAnalyzer( IMorphoModel model )
        {
            model.ThrowIfNull("model");

            _MorphoModel               = model;
            _WordFormMorphologies      = new List< WordFormMorphology_t >();
            _WordForms                 = new List< WordForm_t >();
            _UniqueWordFormsDictionary = new Dictionary< string, PartOfSpeechEnum >();
            _Buffer4Upper              = new char[ DEFAULT_BUFFER_4_UPPER_SIZE ];
        }

		/// получение морфологической информации
		/// words - слова
        public WordMorphology_t GetWordMorphology( string word )
        {
            var wordUpper = StringsHelper.ToUpperInvariant( word );
            
            return (GetWordMorphology_NoToUpper( wordUpper, WordFormMorphologyModeEnum.Default ));
        }
        public WordMorphology_t GetWordMorphology( string word, WordFormMorphologyModeEnum wordFormMorphologyMode )
        {
            var wordUpper = StringsHelper.ToUpperInvariant( word );

            return (GetWordMorphology_NoToUpper( wordUpper, wordFormMorphologyMode ));
        }
        public WordMorphology_t GetWordMorphology_NoToUpper( string wordUpper )
        {
            return (GetWordMorphology_NoToUpper( wordUpper, WordFormMorphologyModeEnum.Default ));
        }
        public WordMorphology_t GetWordMorphology_NoToUpper( string wordUpper, WordFormMorphologyModeEnum wordFormMorphologyMode )
        {
            var wordMorphology = new WordMorphology_t( /*wordUpper*/ );

            if ( _MorphoModel.GetWordFormMorphologies( wordUpper, _WordFormMorphologies, wordFormMorphologyMode ) )
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
        /*unsafe public WordMorphology_t GetWordMorphology_4LastValueOriginalInNumeralChain( char* word, int wordLength, WordFormMorphologyModeEnum wordFormMorphologyMode )
        {
            if ( DEFAULT_BUFFER_4_UPPER_SIZE <= wordLength )
            {
                return (new WordMorphology_t());
            }

            fixed ( char* wordUpper_ptr = _Buffer4Upper )
            {
                StringsHelper.ToUpperInvariant( word, wordUpper_ptr );

                return (_GetWordMorphology_4LastValueUpperInNumeralChain( wordUpper_ptr, wordFormMorphologyMode ));
            }
        }*/
        unsafe private WordMorphology_t _GetWordMorphology_4LastValueUpperInNumeralChain( char* wordUpper, WordFormMorphologyModeEnum wordFormMorphologyMode )
        {
            var wordMorphology = new WordMorphology_t();

            if ( _MorphoModel.GetWordFormMorphologies( wordUpper, _WordFormMorphologies, wordFormMorphologyMode ) )
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
        public WordForms_t GetWordForms( string word )
        {
            return (GetWordFormsByPartOfSpeech( word, PartOfSpeechEnum.Other ));
        }
        public WordForms_t GetWordForms_NoToUpper( string wordUpper )
        {
            return (GetWordFormsByPartOfSpeech_NoToUpper( wordUpper, PartOfSpeechEnum.Other ));
        }
		public WordForms_t GetWordFormsByPartOfSpeech( string word, PartOfSpeechEnum partOfSpeechFilter )
		{
            var result = new WordForms_t( word );
            var wordUpper = StringsHelper.ToUpperInvariant( word );

            if ( _MorphoModel.GetWordForms( wordUpper, _WordForms ) )
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

            if ( _MorphoModel.GetWordForms( wordUpper, _WordForms ) )
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

                PartOfSpeechEnum partOfSpeechExists;
                if ( _UniqueWordFormsDictionary.TryGetValue( wordFormString, out partOfSpeechExists ) )
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