using System.Collections.Generic;

using lingvo.core;

namespace lingvo.morphology
{
    /// <summary>
    /// Морфологическая форма слова
    /// </summary>
    internal sealed class MorphoForm
    {
        private static readonly MorphoAttributePair[] EMPTY = new MorphoAttributePair[ 0 ];

	    /// окончание
        private readonly string _Ending;
        private readonly string _EndingUpper;
	    /// атрибуты
        private readonly MorphoAttributePair[] _MorphoAttributePairs;

        internal MorphoForm( string ending, List< MorphoAttributePair > morphoAttributePair )
	    {
            _Ending      = string.Intern( ending ); //ending; //
            _EndingUpper = string.Intern( StringsHelper.ToUpperInvariant( _Ending ) ); //StringsHelper.ToUpperInvariant( _Ending ); //
            _MorphoAttributePairs = (morphoAttributePair.Count != 0) ? morphoAttributePair.ToArray() : EMPTY;
        }
        /// получение окончания
        public string Ending => _Ending;
        public string EndingUpper => _EndingUpper;
        /// получение атрибутов
        public MorphoAttributePair[] MorphoAttributePairs => _MorphoAttributePairs;

        public override string ToString() => $"[{_Ending}, {{{string.Join( ",", _MorphoAttributePairs )}}}]";
    }
}

