using System;
using System.Collections.Generic;

using lingvo.core;

namespace lingvo.morphology
{
    /// <summary>
    /// Морфологическая форма слова
    /// </summary>
    internal sealed class MorphoForm
    {
        //private static int _GlobalCount;
        //private static HashSet< string > _GlobalHashsetEnding = new HashSet< string >();
        //private static HashSet< string > _GlobalHashsetEndingUpper = new HashSet< string >();

        private static readonly MorphoAttributePair[] EMPTY = new MorphoAttributePair[ 0 ];

	    /// окончание
        private readonly string _Ending;
        private readonly string _EndingUpper;
	    /// атрибуты
        private readonly MorphoAttributePair[] _MorphoAttributePairs;

        internal MorphoForm( string ending, List< MorphoAttributePair > morphoAttributePair )
	    {
            _Ending               = string.Intern( ending ); //ending; //
            _EndingUpper          = string.Intern( StringsHelper.ToUpperInvariant( _Ending ) ); //StringsHelper.ToUpperInvariant( _Ending ); //
            if ( morphoAttributePair.Count != 0 )
                _MorphoAttributePairs = morphoAttributePair.ToArray();
            else
                _MorphoAttributePairs = EMPTY;


            //_GlobalCount++;
            //_GlobalHashsetEnding.Add( _Ending );
            //_GlobalHashsetEndingUpper.Add( _EndingUpper );
        }
	    /// получение окончания
        public string Ending
        {
            get { return (_Ending); }
        }
        public string EndingUpper
        {
            get { return (_EndingUpper); }
        }
	    /// получение атрибутов
        public MorphoAttributePair[] MorphoAttributePairs
        { 
            get { return (_MorphoAttributePairs); }
        }

        public override string ToString()
        {
            return ('[' + _Ending + ", {" + string.Join( ",", (IEnumerable< MorphoAttributePair >) _MorphoAttributePairs ) + "}]");
        }
    }
}

