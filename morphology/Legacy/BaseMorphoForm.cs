namespace lingvo.morphology
{
    /// <summary>
    /// Базовая форма слова
    /// </summary>
    internal sealed class BaseMorphoForm : IBaseMorphoForm
    {
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
	    }

        /// получение основы
        public string Base => _Base;
        /// полученеи нормальной формы
        public string NormalForm => _NormalForm;
        /// получение типа существительного
        public MorphoAttributePair? NounType => _NounType;

        /// получение морфотипа
        public MorphoType MorphoType => _MorphoType;

        public MorphoForm[] MorphoForms => _MorphoType.MorphoForms;
        public PartOfSpeechEnum PartOfSpeech => _MorphoType.PartOfSpeech;
        public MorphoAttributeGroupEnum MorphoAttributeGroup => _MorphoType.MorphoAttributeGroup;

        public override string ToString() => $"[{_Base}, {_NormalForm}, {_NounType}, {_MorphoType}]";
    }
}

