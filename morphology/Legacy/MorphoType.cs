using System;
using System.Collections.Generic;
using System.Linq;

namespace lingvo.morphology
{
    /// <summary>
    /// Морфотип
    /// </summary>
    internal sealed class MorphoType
    {
        private static readonly MorphoForm[] EMPTY = new MorphoForm[ 0 ];

        private readonly MorphoAttributeGroupEnum _MorphoAttributeGroup;
        private readonly PartOfSpeechEnum         _PartOfSpeech;
	    /// формы
        private MorphoForm[] _MorphoForms;
	    /// самое длинное окончание
	    private int _MaxEndingLength;

        internal MorphoType( PartOfSpeechBase partOfSpeechBase )
        {
            _MorphoForms          = EMPTY;
            _MorphoAttributeGroup = partOfSpeechBase.MorphoAttributeGroup;
            _PartOfSpeech         = partOfSpeechBase.PartOfSpeech;
        }

	    /// добавление морфоформы
        /*internal void AddMorphoForm( MorphoForm morphoForm )
        {
            if ( morphoForm != null )
            {
                var len = _MorphoForms.Length;
                Array.Resize( ref _MorphoForms, len + 1 );
                _MorphoForms[ len ] = morphoForm;

                int endingLength = morphoForm.Ending.Length;
                if ( _MaxEndingLength < endingLength )
                {
                    _MaxEndingLength = endingLength;
                }
            }
            else
            {
                throw (new NullPointerException());
            }
        }*/
        internal void SetMorphoForms( List< MorphoForm > morphoForms )
        {
            _MorphoForms = morphoForms.ToArray();
            if ( morphoForms.Count != 0 )
            {
                _MaxEndingLength = _MorphoForms.Max( morphoForm => morphoForm.Ending.Length );
            }
            else
            {
                _MaxEndingLength = 0;
            }
        }

	    /// получение типов атрибутов
        public MorphoAttributeGroupEnum MorphoAttributeGroup
        {
            get { return (_MorphoAttributeGroup); }
        }
        public PartOfSpeechEnum         PartOfSpeech
        {
            get { return (_PartOfSpeech); }
        }
	    /// получение форм
        public MorphoForm[] MorphoForms
        {
            get { return (_MorphoForms); }
        }
	    /// получение длины самого длинного окончания
        public int MaxEndingLength
        {
            get { return (_MaxEndingLength); }
        }

        public override string ToString()
        {
            return ("[" + PartOfSpeech + ", " + MorphoAttributeGroup + ", {" + string.Join( ",", (IEnumerable< MorphoForm >) MorphoForms ) + "}]");
        }
    }
}
