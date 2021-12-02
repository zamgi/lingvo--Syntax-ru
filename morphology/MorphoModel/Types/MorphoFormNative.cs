using System;
using System.Collections.Generic;
using System.Linq;

using lingvo.core;

namespace lingvo.morphology
{
    /// <summary>
    /// Морфологическая форма слова
    /// </summary>
    unsafe internal struct /*sealed class*/ MorphoFormNative
    {
        private static readonly MorphoAttributePair[] EMPTY = new MorphoAttributePair[ 0 ];

        internal MorphoFormNative( char* ending, char* endingUpper, List< MorphoAttributePair > morphoAttributePair )
        {
            Ending      = ending;
            EndingUpper = endingUpper;
            if ( morphoAttributePair.Count != 0 )
            {
                MorphoAttributePairs = morphoAttributePair.ToArray();
            }
            else
            {
                MorphoAttributePairs = EMPTY;
            }
        }

        /// окончание
        public readonly char* Ending;
        /// uppercase-окончание
        public readonly char* EndingUpper;
        /// морфо-атрибуты
        public readonly MorphoAttributePair[] MorphoAttributePairs;


        public override string ToString()
        {
            return ("[" + StringsHelper.ToString( Ending ) + ", {" + string.Join( ",", (IEnumerable< MorphoAttributePair >) MorphoAttributePairs ) + "}]");
        }
    }
}