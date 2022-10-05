using System;
using System.Diagnostics;
using System.Text;

using lingvo.core;
using M = System.Runtime.CompilerServices.MethodImplAttribute;
using O = System.Runtime.CompilerServices.MethodImplOptions;

namespace lingvo.morphology
{
    /// <summary>
    /// Базовая форма слова
    /// </summary>
    unsafe internal sealed class BaseMorphoFormNative : IBaseMorphoFormNative
    {
        #region [.temp-buffers (static, because model loading in single-thread).]
        private static IntPtrSet _TempBufferHS;

        internal static void BeginLoad()
        {
            const int DEFAULT_CAPACITY = 107; //197;

            _TempBufferHS = new IntPtrSet( DEFAULT_CAPACITY );
        }
        internal static void EndLoad() => _TempBufferHS = null;
        #endregion

	    /// основа
        private char*            _Base;
        /// окончания морфо-форм
        private char*[]          _MorphoFormEndings;
        /// часть речи
        private PartOfSpeechEnum _PartOfSpeech;
        
        public BaseMorphoFormNative( char* _base, MorphoTypeNative morphoType )
	    {
		    _Base         = _base;
            _PartOfSpeech = morphoType.PartOfSpeech;

            Debug.Assert( morphoType.HasMorphoForms, "morphoType.MorphoForms.Length <= 0" );

            _MorphoFormEndings = morphoType.MorphoFormEndings;
	    }

        public void AppendMorphoFormEndings( BaseMorphoFormNative other )
        {
            Debug.Assert( _MorphoFormEndings[ 0 ] == other._MorphoFormEndings[ 0 ], "_MorphoFormEndings[ 0 ] != baseMorphoForms._MorphoFormEndings[ 0 ]" );

            //select longest array of morpho-form-endings
            char*[] first, second;
            if ( _MorphoFormEndings.Length < other._MorphoFormEndings.Length )
            {
                first  = other._MorphoFormEndings;
                second = _MorphoFormEndings;
            }
            else
            {
                first  = _MorphoFormEndings;
                second = other._MorphoFormEndings;
            }

            fixed ( char** morphoFormEndingsBase = first )
            {
                for ( int i = 0, len = first.Length; i < len; i++ )
                {
                    _TempBufferHS.Add( new IntPtr( *(morphoFormEndingsBase + i) ) );
                }
            }
            //store curreent count in 'tempBufferHS'
            var count = _TempBufferHS.Count;
            fixed ( char** morphoFormEndingsBase = second )
            {
                for ( int i = 0, len = second.Length; i < len; i++ )
                {
                    _TempBufferHS.Add( new IntPtr( *(morphoFormEndingsBase + i) ) );
                }
            }
            //if count of 'tempBufferHS' not changed, then [_MorphoFormEndings] & [other._MorphoFormEndings] are equals
            if ( count != _TempBufferHS.Count )
            {
                _MorphoFormEndings = new char*[ _TempBufferHS.Count ];
                fixed ( char** morphoFormEndingsBase = _MorphoFormEndings )
                {
                    var it = _TempBufferHS.GetEnumerator();
                    for ( var i = 0; it.MoveNext(); i++ )
                    {
                        *(morphoFormEndingsBase + i) = (char*) it.Current;
                    }
                }
            }
            _TempBufferHS.Clear();
        }

        /// получение основы
        public char* Base { [M(O.AggressiveInlining)] get => _Base; }
        /// окончания морфо-форм
        public char*[] MorphoFormEndings { [M(O.AggressiveInlining)] get => _MorphoFormEndings; }
        /// часть речи
        public PartOfSpeechEnum PartOfSpeech { [M(O.AggressiveInlining)] get => _PartOfSpeech; }

        /// получение нормальной формы
        [M(O.AggressiveInlining)] public string GetNormalForm() => StringsHelper.CreateWordForm( _Base, _MorphoFormEndings[ 0 ] );

        public override string ToString()
        {
            const string FORMAT = "[base: '{0}', normal-form: '{1}', pos: '{2}', {{morpho-form-endings: '{3}'}}]";

            var sb = new StringBuilder();
            foreach ( var morphoFormEnding in MorphoFormEndings )
            {
                if ( sb.Length != 0 ) sb.Append( ", " );
                sb.Append( StringsHelper.ToString( morphoFormEnding ) );
            }

            var _base = StringsHelper.ToString( Base );
            var normalForm = GetNormalForm();

            return (string.Format( FORMAT, _base, normalForm, PartOfSpeech, sb.ToString() ));
        }
    }
}