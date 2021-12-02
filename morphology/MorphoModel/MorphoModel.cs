using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using lingvo.core;

namespace lingvo.morphology
{
	/// <summary>
    /// Морфо-модель
	/// </summary>
    internal sealed class MorphoModel : MorphoModelBase, IMorphoModel
	{
        /// словарь слов
        private readonly ITreeDictionary _TreeDictionary;

        public MorphoModel( in MorphoModelConfig config ) : base( in config )
        {
            switch ( config.TreeDictionaryType )
            {
                case MorphoModelConfig.TreeDictionaryTypeEnum.Classic:
                    _TreeDictionary = new TreeDictionaryClassic();
                break;

                case MorphoModelConfig.TreeDictionaryTypeEnum.FastMemPlenty:
                    _TreeDictionary = new TreeDictionaryFastMemPlenty();
                break;

                default:
                    throw (new ArgumentException(config.TreeDictionaryType.ToString()));
            }

            Initialization( in config );
        }
        public void Dispose() { }

        #region [.loading model.]
        /// морфо-типы
        private Dictionary< string, MorphoType >       _MorphoTypesDictionary;
        /// список допустимых комбинаций морфо-аттрибутов в группах 
        private MorphoAttributeList                    _MorphoAttributeList;
        private PartOfSpeechList                       _PartOfSpeechList;
        private Dictionary< PartOfSpeechEnum, string > _PartOfSpeechStringDictionary;
        private Action< string, string >               _ModelLoadingErrorCallback;

        /// <summary>
        /// 
        /// </summary>
        private struct MorphoType_pair_t
        {
            public string     Name;
            public MorphoType MorphoType;
        }

		/// инициализация морфо-модели
		/// информация для инициализации морфо-модели
		private void Initialization( in MorphoModelConfig config )
		{
            #region [.init field's.]
            base.Initialization();
            if ( config.ModelLoadingErrorCallback == null )
            {
                _ModelLoadingErrorCallback = (s1, s2) => { };
            }
            else
            {
                _ModelLoadingErrorCallback = config.ModelLoadingErrorCallback;
            }
            _MorphoTypesDictionary = new Dictionary< string, MorphoType >();
            _MorphoAttributeList   = new MorphoAttributeList();
            _PartOfSpeechList      = new PartOfSpeechList();
            _PartOfSpeechStringDictionary = Enum.GetValues( typeof(PartOfSpeechEnum) )
                                            .Cast< PartOfSpeechEnum >()
                                            .ToDictionary( pos => pos, pos => pos.ToString() );
            #endregion

            foreach ( var morphoTypesFilename in config.MorphoTypesFilenames )
            {
                var filename = GetFullFilename( config.BaseDirectory, morphoTypesFilename );
                ReadMorphoTypes( filename );
            }

            foreach ( var properNamesFilename in config.ProperNamesFilenames )
            {
                var filename = GetFullFilename( config.BaseDirectory, properNamesFilename );
                ReadWords( filename, MorphoAttributeEnum.Proper );
            }

            foreach ( var commonFilename in config.CommonFilenames )
            {
                var filename = GetFullFilename( config.BaseDirectory, commonFilename );
                ReadWords( filename, MorphoAttributeEnum.Common );
            }

            #region [.uninit field's.]
            base.UnInitialization();
            _ModelLoadingErrorCallback    = null;
            _MorphoTypesDictionary        = null;
            _MorphoAttributeList          = null;
            _PartOfSpeechList             = null;
            _PartOfSpeechStringDictionary = null;
            #endregion

            GC.Collect( GC.MaxGeneration, GCCollectionMode.Forced );
		}

        /// получение морфотипа по его имени
        private MorphoType GetMorphoTypeByName( string name )
        {
            MorphoType value;
            if ( _MorphoTypesDictionary.TryGetValue( name, out value ) )
                return (value);
            return (null);
        }

        /// сохранение морфотипа
        private void AddMorphoType2Dictionary( MorphoType_pair_t? morphoTypePair )
        {
            if ( _MorphoTypesDictionary.ContainsKey( morphoTypePair.Value.Name ) )
            {
                _ModelLoadingErrorCallback( "Duplicated morpho-type", morphoTypePair.Value.Name ); //throw (new DuplicatedMorphoTypeException());
            }
            else
            {
                _MorphoTypesDictionary.Add( morphoTypePair.Value.Name, morphoTypePair.Value.MorphoType );
            }
        }

		/// чтение файла морфотипов
		/// path - полный путь к файлу
        private void ReadMorphoTypes( string filename )
		{
            var lines = ReadFile( filename );
            var morphoAttributePairs = new List< MorphoAttributePair >();
            var morphoForms          = new List< MorphoForm >();

			var morphoTypePairLast = default(MorphoType_pair_t?);
			foreach ( var line in lines )
			{
                //if ( line == "Pronoun, MORPHO_TYPE:что" )
                  //  System.Diagnostics.Debugger.Break();

                MorphoType_pair_t? _morphoTypePair = CreateMorphoTypePair( line );
                if ( _morphoTypePair != null ) /// новый морфо-тип
				{
                    if ( morphoTypePairLast != null )
                    {
                        morphoTypePairLast.Value.MorphoType.SetMorphoForms( morphoForms );

                        AddMorphoType2Dictionary( morphoTypePairLast );
                    }
                    morphoForms.Clear();
                    
                    morphoTypePairLast = _morphoTypePair;
				}
				else /// словоформа
                if ( morphoTypePairLast != null )
				{
                    try
                    {
                        MorphoForm morphoForm = CreateMorphoForm( morphoTypePairLast.Value.MorphoType, line, morphoAttributePairs );
                        if ( morphoForm != null )
                        {
                            morphoForms.Add( morphoForm );
                            //---morphoTypePairLast.Value.MorphoType.AddMorphoForm( morphoForm );
                        }
                        else
                        {
                            _ModelLoadingErrorCallback( "Wrong line format", line ); //throw (new MorphoFormatException());
                        }
                    }
                    catch ( MorphoFormatException /*ex*/ )
                    {
                        _ModelLoadingErrorCallback( "Wrong line format", line ); 
                    }
				}
				else
                {
                    _ModelLoadingErrorCallback( "Null MorphoType", line ); //throw (new NullPointerException());
                }
			}

            if ( morphoTypePairLast != null )
            {
                morphoTypePairLast.Value.MorphoType.SetMorphoForms( morphoForms );

                AddMorphoType2Dictionary( morphoTypePairLast );
            }
		}

		/// чтение файла со словами
		/// path - полный путь к файлу
		/// nounType - тип существительного
        private void ReadWords( string filename, MorphoAttributeEnum nounType )
		{
            var lines = ReadFile( filename );

			foreach ( var line in lines )
            {
                #region commented
                //try
                //{ 
                #endregion
				var array = line.Split( WORDS_DICTIONARY_SEPARATOR, StringSplitOptions.RemoveEmptyEntries );
                if ( array.Length != 3 )
                {
                    _ModelLoadingErrorCallback( "Wrong line format", line ); //throw (new MorphoFormatException());
                    continue;
                }

                MorphoType morphoType = GetMorphoTypeByName( array[ 1 ] );
				if ( morphoType == null )
                {
                    _ModelLoadingErrorCallback( "Unknown morpho-type", line ); //throw new UnknownMorphoTypeException();
                }
                else
                if ( array[ 2 ] != _PartOfSpeechStringDictionary[ morphoType.PartOfSpeech ] )
                {
                    _ModelLoadingErrorCallback( "Wrong part-of-speech", line ); //throw new WrongPartOfSpeechException();
                }
                else
                {
                    var word = array[ 0 ];

                    /*
                    if ( word == "коем" )
                        System.Diagnostics.Debugger.Break();
                    //*/

                    var _nounType = default(MorphoAttributePair?);
                    if ( (morphoType.MorphoAttributeGroup & MorphoAttributeGroupEnum.NounType) == MorphoAttributeGroupEnum.NounType )
                    {
                        _nounType = _MorphoAttributeList.GetMorphoAttributePair( MorphoAttributeGroupEnum.NounType, nounType );
                    }
                    _TreeDictionary.AddWord( word, morphoType, _nounType );
                }
			}
		}

		/// создание морфоформы из строки
        private MorphoForm CreateMorphoForm( MorphoType morphoType, string line, List< MorphoAttributePair > morphoAttributePairs )
		{
			int index = line.IndexOf( ':' );
			if (index < 0)
				throw (new MorphoFormatException());

            var ending = StringsHelper.ToLowerInvariant( line.Substring( 0, index ).Trim() );
			if (ending == EMPTY_ENDING)
				ending = string.Empty;

            morphoAttributePairs.Clear();
			var attributes = line.Substring( index + 1 ).Split( MORPHO_ATTRIBUTE_SEPARATOR, StringSplitOptions.RemoveEmptyEntries );
			foreach ( var attribute in attributes )
			{
				var attr = attribute.Trim();
				if ( !string.IsNullOrEmpty( attr ) )
				{
                    var morphoAttribute = default(MorphoAttributeEnum);
                    if ( Enum.TryParse( attr, true, out morphoAttribute ) )
                    {
                        //---morphoAttributePairs.Add( _MorphoAttributeList.GetMorphoAttributePair( morphoType.MorphoAttributeGroup, morphoAttribute ) );                        

                        var map = _MorphoAttributeList.TryGetMorphoAttributePair( morphoType.MorphoAttributeGroup, morphoAttribute );
                        if ( map.HasValue )
                        {
                            morphoAttributePairs.Add( map.Value );
                        }
#if DEBUG
                        //TOO MANY ERRORS AFTER last (2016.12.28) getting morpho-dcitionaries from 'lingvo-[ilook]'
                        else
                        {
                            _ModelLoadingErrorCallback( "Error in morpho-attribute: '" + attr + '\'', line );
                        }
#endif
                    }
                    else
                    {
                        _ModelLoadingErrorCallback( "Unknown morpho-attribute: '" + attr + '\'', line );
                    }
				}
			}
            var morphoForm = new MorphoForm( ending, morphoAttributePairs );
            return (morphoForm);
        }

		/// создание морфотипа из строки
        private MorphoType_pair_t? CreateMorphoTypePair( string line )
		{
            var m = MORPHOTYPE_PREFIX_REGEXP.Match( line );
            if ( m == null || m.Groups.Count < 3 )
            {
                return (null);
            }

            string prefix = m.Groups[ 1 ].Value;
            string pos    = m.Groups[ 2 ].Value;
            string name   = line.Substring( prefix.Length );

            var partOfSpeech = default(PartOfSpeechEnum);
            if ( Enum.TryParse( pos, true, out partOfSpeech ) )
            {
                var morphoType = new MorphoType( _PartOfSpeechList.GetPartOfSpeech( partOfSpeech ) );
                var morphoTypePair = new MorphoType_pair_t()
                {
                    Name       = name,
                    MorphoType = morphoType,
                };
                return (morphoTypePair);
            }
            else
            {
                _ModelLoadingErrorCallback( "Unknown part-of-speech: '" + pos + '\'', line );
            }
            return (null);

            #region commented
            //int index = MorphoTypePrefixRegExp.indexIn( str );
            //if (index != 0)
            //    return (null);

            //string prefix       = MorphoTypePrefixRegExp.cap(1);
            //string partOfSpeech = MorphoTypePrefixRegExp.cap(2);
            //string name = str.Substring( prefix.Length );
            //return (new CMorphoType( name, CPartOfSpeech.Create( partOfSpeech ) )); 
            #endregion
		}
        #endregion

        #region [.IMorphoModel.]
        public bool TryGetWordFormMorphologies( string wordUpper, ICollection< WordFormMorphology_t > result, WordFormMorphologyModeEnum wordFormMorphologyMode )
        {
            return (_TreeDictionary.GetWordFormMorphologies( wordUpper, result, wordFormMorphologyMode ));
        }
        unsafe public bool TryGetWordFormMorphologies( char* wordUpper, ICollection< WordFormMorphology_t > result, WordFormMorphologyModeEnum wordFormMorphologyMode )
        {
            return (_TreeDictionary.GetWordFormMorphologies( wordUpper, result, wordFormMorphologyMode ));
        }
        public bool TryGetWordForms( string wordUpper, ICollection< WordForm_t > result )
        {
            return (_TreeDictionary.GetWordForms( wordUpper, result ));
        }
        #endregion
    }
}