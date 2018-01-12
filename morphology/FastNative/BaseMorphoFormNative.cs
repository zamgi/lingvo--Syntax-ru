using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;
using lingvo.core;

namespace lingvo.morphology
{
    /// <summary>
    /// Базовая форма слова
    /// </summary>
    unsafe internal sealed class /*struct*/ BaseMorphoFormNative : IBaseMorphoFormNative
    {
        #region [.temp-buffers (static, because model loading in single-thread).]
        //private static int _GlobalCount;
        //private static HashSet< string > _GlobalHashsetBase = new HashSet< string >();
        //private static HashSet< string > _GlobalHashsetNormalForm = new HashSet< string >();

        private static IntPtrSet tempBufferHS; /*---private static HashSet< IntPtr > tempBufferHS;*/

        internal static void BeginLoad()
        {
            const int DEFAULT_CAPACITY = 107; //197;

            tempBufferHS = new IntPtrSet( DEFAULT_CAPACITY ); /*---tempBufferHS = new HashSet< IntPtr >();*/
        }
        internal static void EndLoad()
        {
            tempBufferHS = null;
        }
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

            #region commented
            /*
            for ( int i = 0, len = morphoType.MorphoForms.Length; i < len; i++ )
            {
                tempBufferHS.Add( (IntPtr) morphoType.MorphoForms[ i ].Ending );
            }
            _MorphoFormEndings = new char*[ tempBufferHS.Count ];
            fixed ( char** morphoFormEndingsBase = _MorphoFormEndings )
            {
                var morphoFormEndings_ptr = morphoFormEndingsBase;
                foreach ( var intptr in tempBufferHS )
                {
                    *(morphoFormEndings_ptr++) = (char*) intptr;
                }
            }
            tempBufferHS.Clear();
            */
            #endregion

            //_GlobalCount++;
            //_GlobalHashsetBase.Add( _Base );
            //_GlobalHashsetNormalForm.Add( _NormalForm );
	    }

        /*public void SetMorphoFormEndings( IEnumerable< BaseMorphoFormNative > baseMorphoForms )
        {
            //if ( baseMorphoForms.Length <= 1 )
            if ( baseMorphoForms.ElementAtOrDefault( 1 ) == null )
                return;

            foreach ( var bmf in baseMorphoForms )
            {
                fixed ( char** morphoFormEndingsBase = bmf._MorphoFormEndings )
                {
                    for ( int i = 0, len = bmf._MorphoFormEndings.Length; i < len; i++ )
                    {
                        tempBufferHS.Add( new IntPtr( *(morphoFormEndingsBase + i) ) );
                    }
                }
            }
            _MorphoFormEndings = new char*[ tempBufferHS.Count ];
            var j = 0;
            fixed ( char** morphoFormEndingsBase = _MorphoFormEndings )
            {
                foreach ( var intptr in tempBufferHS )
                {
                    *(morphoFormEndingsBase + j++) = (char*) intptr;
                }
            }
            tempBufferHS.Clear();
        }*/
        public void AppendMorphoFormEndings( BaseMorphoFormNative other )
        {
            Debug.Assert( _MorphoFormEndings[ 0 ] == other._MorphoFormEndings[ 0 ]
                , "_MorphoFormEndings[ 0 ] != baseMorphoForms._MorphoFormEndings[ 0 ]" );

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
                    tempBufferHS.Add( new IntPtr( *(morphoFormEndingsBase + i) ) );
                }
            }
            //store curreent count in 'tempBufferHS'
            var count = tempBufferHS.Count;
            fixed ( char** morphoFormEndingsBase = second )
            {
                for ( int i = 0, len = second.Length; i < len; i++ )
                {
                    tempBufferHS.Add( new IntPtr( *(morphoFormEndingsBase + i) ) );
                }
            }
            //if count of 'tempBufferHS' not changed, then [_MorphoFormEndings] & [other._MorphoFormEndings] are equals
            if ( count != tempBufferHS.Count )
            {
                _MorphoFormEndings = new char*[ tempBufferHS.Count ];
                fixed ( char** morphoFormEndingsBase = _MorphoFormEndings )
                {
                    var it = tempBufferHS.GetEnumerator();
                    for ( var i = 0; it.MoveNext(); i++ )
                    {
                        *(morphoFormEndingsBase + i) = (char*) it.Current;
                    }

                    #region commented. foreach.
                    /*var i = 0;
                    foreach ( var intptr in tempBufferHS )
                    {
                        *(morphoFormEndingsBase + i++) = (char*) intptr;
                    }*/
                    #endregion
                }
            }
            tempBufferHS.Clear();
        }

        /// получение основы
        public char*   Base
        { 
            get { return (_Base); }
        }
        /// окончания морфо-форм
        public char*[] MorphoFormEndings
        {
            get { return (_MorphoFormEndings); }
        }
        /// часть речи
        public PartOfSpeechEnum PartOfSpeech
        {
            get { return (_PartOfSpeech); }
        }

        /// получение нормальной формы
        public string GetNormalForm()
        {
            return (StringsHelper.CreateWordForm( _Base, _MorphoFormEndings[ 0 ] ));
        }

        public override string ToString()
        {
            const string format = "[base: '{0}', normal-form: '{1}', pos: '{2}', {{morpho-form-endings: '{3}'}}]";

            var sb = new StringBuilder();
            foreach ( var morphoFormEnding in MorphoFormEndings )
            {
                if ( sb.Length != 0 )
                    sb.Append( ", " );
                sb.Append( StringsHelper.ToString( morphoFormEnding ) );
            }

            var _base = StringsHelper.ToString( Base );
            /*
            var normalForm = (_MorphoFormEndings.Length != 0)
                             ? StringsHelper.CreateWordForm( _Base, _MorphoFormEndings[ 0 ] )
                             : _base;
            */
            var normalForm = GetNormalForm();

            return (string.Format( format, _base, normalForm, PartOfSpeech, sb.ToString() ));
        }
    }
}