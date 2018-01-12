using System;
using System.Collections.Generic;

namespace lingvo.morphology
{
    /// <summary>
    /// Базовая форма слова
    /// </summary>
    internal sealed class BaseMorphoForm : IBaseMorphoForm
    {
        //private static int _GlobalCount;
        //private static HashSet< string > _GlobalHashsetBase = new HashSet< string >();
        //private static HashSet< string > _GlobalHashsetNormalForm = new HashSet< string >();

	    /// основа
	    private readonly string _Base;
	    /// нормальная форма
        private readonly string _NormalForm;
	    /// Тип существительного
        private readonly MorphoAttributePair? _NounType;
        private readonly MorphoType _MorphoType;

        public BaseMorphoForm( string _base, MorphoType morphoType, MorphoAttributePair? nounType )
	    {
            _NounType   = nounType;
		    _Base       = _base; //-bad-//string.Intern( _base ); //
            _NormalForm = _Base;
            if ( morphoType.MorphoForms.Length != 0 )
            {
                _NormalForm += morphoType.MorphoForms[ 0 ].Ending;
            }
            _MorphoType = morphoType;


            //_GlobalCount++;
            //_GlobalHashsetBase.Add( _Base );
            //_GlobalHashsetNormalForm.Add( _NormalForm );
	    }

	    /// получение основы
	    public string Base
        { 
            get { return (_Base); }
        }
	    /// полученеи нормальной формы
	    public string NormalForm
        {
            get { return (_NormalForm); }
        }
	    /// получение типа существительного
	    public MorphoAttributePair? NounType 
        { 
            get { return (_NounType); }
        }

        /// получение морфотипа
        public MorphoType MorphoType
        {
            get { return (_MorphoType); }
        }

        public MorphoForm[]             MorphoForms
        {
            get { return (_MorphoType.MorphoForms); }
        }
        public PartOfSpeechEnum         PartOfSpeech
        {
            get { return (_MorphoType.PartOfSpeech); }
        }
        public MorphoAttributeGroupEnum MorphoAttributeGroup
        {
            get { return (_MorphoType.MorphoAttributeGroup); }
        }

        public override string ToString()
        {
            return ('[' + _Base + ", " + _NormalForm + ", " + _NounType + ", " + _MorphoType + ']');
        }
    }
}

