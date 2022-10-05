using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using lingvo.core;
using M = System.Runtime.CompilerServices.MethodImplAttribute;
using O = System.Runtime.CompilerServices.MethodImplOptions;

namespace lingvo.morphology
{
    /// <summary>
    /// Словарь-дерево для слов
    /// </summary>
    unsafe internal sealed class TreeDictionary
    {
        /// <summary>
        /// 
        /// </summary>
        private struct Pair
        {     
            /// <summary>
            /// 
            /// </summary>
            public sealed class EqualityComparer : IEqualityComparer< Pair >
            {
                public static EqualityComparer Inst { [M(O.AggressiveInlining)] get; } = new EqualityComparer();
                private EqualityComparer() { }
                public bool Equals( Pair x, Pair y )
                {
                    if ( x.MorphoAttribute != y.MorphoAttribute )
                        return (false);

                    if( x.BaseMorphoForm.Base != y.BaseMorphoForm.Base )
                        return (false);

                    if ( x.BaseMorphoForm.PartOfSpeech != y.BaseMorphoForm.PartOfSpeech )
                        return (false);

                    if ( x.BaseMorphoForm.MorphoFormEndings[ 0 ] != y.BaseMorphoForm.MorphoFormEndings[ 0 ] )
                        return (false);

                    #region comm.
                    /*
                    var len = x.BaseMorphoForm.MorphoFormEndings.Length;
                    if ( len != y.BaseMorphoForm.MorphoFormEndings.Length )
                        return (false);

                    fixed ( char** x_me = x.BaseMorphoForm.MorphoFormEndings )
                    fixed ( char** y_me = y.BaseMorphoForm.MorphoFormEndings )
                    {
                        for ( var i = 0; i < len; i++ )
                        {
                            if ( *x_me != *y_me )
                                return (false);
                        }
                    }*/
                    #endregion

                    return (true);
                }
                public int GetHashCode( Pair obj ) => obj.GetHashCode();
            }

            public Pair( BaseMorphoFormNative baseMorphoForm, MorphoAttributeEnum morphoAttribute )
            {
                BaseMorphoForm  = baseMorphoForm;
                MorphoAttribute = morphoAttribute;
            }

            public BaseMorphoFormNative BaseMorphoForm;
            public MorphoAttributeEnum MorphoAttribute;

            public override int GetHashCode()
            {
                return ((int) MorphoAttribute ^
                        (int) BaseMorphoForm.Base ^ 
                        (int) BaseMorphoForm.PartOfSpeech ^ 
                        (int) BaseMorphoForm.MorphoFormEndings[ 0 ] 
                       );
            }
#if DEBUG
            public override string ToString() => $"{BaseMorphoForm}, {MorphoAttribute}";
#endif
        }

        /// <summary>
        /// 
        /// </summary>
	    private sealed class PairSet : IEnumerable< Pair >
	    {
            /// <summary>
            /// 
            /// </summary>
		    internal struct Slot
		    {                
                internal int  HashCode;
			    internal int  Next;                
                internal Pair Value;
		    }

            /// <summary>
            /// 
            /// </summary>
            internal struct Enumerator : IEnumerator< Pair >
            {
                private PairSet _Set;
                private Pair    _Current;
                private int     _Index;

                public Pair Current => _Current; 
	            object IEnumerator.Current
	            {
		            get
		            {
                        if ( _Index == 0 || _Index == _Set._Count + 1 ) throw (new InvalidOperationException("InvalidOperation_EnumOpCantHappen"));			            
			            return (_Current);
		            }
	            }

                internal Enumerator( PairSet set )
	            {
		            _Set     = set;
		            _Index   = 0;
		            _Current = default;
	            }
                public void Dispose() { }

	            public bool MoveNext()
	            {
                    while ( _Index < _Set._Count )
		            {
                        _Current = _Set._Slots[ _Index ].Value;
                        _Index++;
                        if ( _Current.BaseMorphoForm != null )
                        {
                            return (true);
                        }
                        #region comm.
                        /*var slot = _Set._Slots[ _Index ];
                        if ( 0 <= slot.hashCode )
                        {
                            _Current = slot.value;
                            _Index++;
                            return (true);
                        }
                        _Index++;
                        */ 
                        #endregion
                    }
                    _Index   = _Set._Count + 1;
		            _Current = default;
		            return (false);
	            }
	            void IEnumerator.Reset()
	            {
		            _Index   = 0;
                    _Current = default;
	            }
            }            

		    private int[]  _Buckets;
		    private Slot[] _Slots;
		    private int    _Count;
		    private int    _FreeList;
            private Pair.EqualityComparer _Comparer = Pair.EqualityComparer.Inst;

            internal Slot[] Slots { [M(O.AggressiveInlining)] get => _Slots; } 
            public int Count { [M(O.AggressiveInlining)] get => _Count; }
		    public PairSet( int capacity )
		    {
			    _Buckets  = new int [ capacity ];
			    _Slots    = new Slot[ capacity ];
			    _FreeList = -1;
		    }

            [M(O.AggressiveInlining)] public bool TryAddOrGetExists( Pair keyValue, ref Pair existsValue )
		    {
                #region [.find exists.]
                int hash = (keyValue.GetHashCode() & 0x7FFFFFFF);
                for ( int i = _Buckets[ hash % _Buckets.Length ] - 1; 0 <= i; )
                {
                    var slot = _Slots[ i ];
                    if ( (slot.HashCode == hash) && _Comparer.Equals( slot.Value, keyValue ) )
                    {
                        existsValue = slot.Value;
                        return (false);
                    }
                    i = slot.Next;
                }
                #endregion

                #region [.add new.]
                int n1;
                if ( 0 <= _FreeList )
                {
                    n1 = _FreeList;
                    _FreeList = _Slots[ n1 ].Next;
                }
                else
                {
                    if ( _Count == _Slots.Length )
                    {
                        Resize();
                    }
                    n1 = _Count;
                    _Count++;
                }
                int n2 = hash % _Buckets.Length;
                _Slots[ n1 ] = new Slot() 
                {
                    HashCode = hash,
                    Value    = keyValue,
                    Next     = _Buckets[ n2 ] - 1,
                };
                _Buckets[ n2 ] = n1 + 1;

                return (true);
                #endregion
            }
            [M(O.AggressiveInlining)] public void Clear()
            {
                if ( 0 < _Count )
	            {
                    Array.Clear( _Slots,   0, _Count );
                    Array.Clear( _Buckets, 0, _Buckets.Length );
                    _Count    = 0;
                    _FreeList = -1;
	            }                    
            }
		    
            [M(O.AggressiveInlining)] private void Resize()
		    {
                int new_size = checked( _Count * 2 + 1 );
                var buckets = new int[ new_size ];
                var slots   = new Slot[ new_size ];
                Array.Copy( _Slots, 0, slots, 0, _Count );
                for ( int i = 0; i < _Count; i++ )
                {
                    int n = slots[ i ].HashCode % new_size;
                    slots[ i ].Next = buckets[ n ] - 1;
                    buckets[ n ] = i + 1;
                }
                _Buckets = buckets;
                _Slots   = slots;
		    }

            [M(O.AggressiveInlining)] public Enumerator GetEnumerator() => new Enumerator( this );
            IEnumerator< Pair > IEnumerable< Pair >.GetEnumerator() => new Enumerator( this );
            IEnumerator IEnumerable.GetEnumerator() => new Enumerator( this );
	    }

        #region [.temp-buffers (static, because model must loading in single-thread).]
        /*[ThreadStatic]*/ private static PairSet _TempBufferPairs;
        private static Pair[] _TempBufferPairs_ToArrayAndClear()
        {
            var pairs_new = new Pair[ _TempBufferPairs.Count ];
            var it = _TempBufferPairs.GetEnumerator();
            for ( var i = 0; it.MoveNext(); i++ )
            {
                pairs_new[ i ] = it.Current;
            }
            _TempBufferPairs.Clear();
            return (pairs_new);
        }
        private static char* _UPPER_INVARIANT_MAP;
        static TreeDictionary() => _UPPER_INVARIANT_MAP = xlat_Unsafe.Inst._UPPER_INVARIANT_MAP;

        internal static void BeginLoad()
        {
            const int DEFAULT_CAPACITY = 11; //37;

            BaseMorphoFormNative.BeginLoad();
            MorphoTypeNative    .BeginLoad();

            _TempBufferPairs = new PairSet( DEFAULT_CAPACITY ); /*---tempBufferPairs = new Dictionary< pair_t, pair_t >( default(pair_t_IEqualityComparer) );*/
        }
        internal static void EndLoad()
        {
            BaseMorphoFormNative.EndLoad();
            MorphoTypeNative    .EndLoad();
            _TempBufferPairs = null;
        }
        #endregion

        #region [.private field's.]
        /// слот для дочерних слов
        private SortedListCharKey< TreeDictionary > _Slots;
        /// коллекция информаций о формах слова
        private SortedListIntPtrKey< Pair[] > _Endings;
        
        [M(O.AggressiveInlining)] private bool HasEndings() => (_Endings.Tuples != null);
        #endregion

        #region [.ctor().]
        public TreeDictionary() => _Slots.InitArrayAsEmpty();

        internal void Trim()
        {
            if ( HasEndings() )
            {
                _Endings.Trim();
            }
            _Slots.Trim();
            for ( int i = 0, len = _Slots.Count; i < len; i++ )
            {
                _Slots.Tuples[ i ].Value.Trim();
            }
        }
        #endregion

        #region [.Append word.]
        /// добавление слова и всех его форм в словарь
        /// wordBase   - marshaled-слово
        /// morphoType - морфотип
        /// nounType   - тип сущетсвительного
        public void AddWord( char* wordBase, MorphoTypeNative morphoType, in MorphoAttributePair? nounType )
        {
            var baseMorphoForm = new BaseMorphoFormNative( wordBase, morphoType );

            for ( TreeDictionary _this = this, _this_next; ; )
            {
                var first_char = _UPPER_INVARIANT_MAP[ *wordBase ];

                #region [.сохранение характеристик if end-of-word.]
                if ( first_char == '\0' )
	            {
                    // sort & merge after insert - may be faster/better
                    var len = morphoType.MorphoFormEndingUpperAndMorphoAttributes.Length;
                    SortedListIntPtrKey< Pair[] >.Tuple[] tuples;
                    int tuplesOffset;
                    if ( !_this.HasEndings() )
                    {
                        tuplesOffset = 0;
                        tuples       = new SortedListIntPtrKey< Pair[] >.Tuple[ len ];                    
                    }
                    else
                    {
                        tuplesOffset = _this._Endings.Count;
                        tuples       = new SortedListIntPtrKey< Pair[] >.Tuple[ len + tuplesOffset ];

                        Array.Copy( _this._Endings.Tuples, tuples, tuplesOffset );
                        #region comm. prev.
                        //for ( int i = 0; i < tuplesOffset; i++ )
                        //{
                        //    tuples[ i ] = _this._Endings.Array[ i ];
                        //} 
                        #endregion
                    }

                    for ( int i = 0; i < len; i++ )
                    {
                        var p = morphoType.MorphoFormEndingUpperAndMorphoAttributes[ i ];
                        var pairs_current_len = p.MorphoAttributes.Length;
                        var pairs_current     = new Pair[ pairs_current_len ];
                        for ( int j = 0; j < pairs_current_len; j++ )
                        {
                            var ma = MorphoAttributePair.GetMorphoAttribute( morphoType, p.MorphoAttributes[ j ], in nounType );
                            pairs_current[ j ] = new Pair( baseMorphoForm, ma );
                        }
                        tuples[ i + tuplesOffset ] = new SortedListIntPtrKey< Pair[] >.Tuple() { Key = p.EndingUpper, Value = pairs_current };
                    }

                    ShellSortAscending     ( tuples     );
                    MergeSorted            ( ref tuples );
                    _this._Endings.SetArray( tuples     );

                    return;
                }
                #endregion

                if ( !_this._Slots.TryGetValue( first_char, out _this_next ) )
                {
                    /// добавление новой буквы
                    _this_next = new TreeDictionary();
                    _this._Slots.Add( first_char, _this_next );
                }                
                _this = _this_next;
                wordBase++;
            }
        }
        public void AddWord( char* wordBase, MorphoTypeNative morphoType, in MorphoAttributePair nounType )
        {
            var baseMorphoForm = new BaseMorphoFormNative( wordBase, morphoType );

            for ( TreeDictionary _this = this, _this_next; ; )
            {
                var first_char = _UPPER_INVARIANT_MAP[ *wordBase ];

                #region [.сохранение характеристик if end-of-word.]
                if ( first_char == '\0' )
	            {
                    // sort & merge after insert - may be faster/better
                    var len = morphoType.MorphoFormEndingUpperAndMorphoAttributes.Length;
                    SortedListIntPtrKey< Pair[] >.Tuple[] tuples;
                    int tuplesOffset;
                    if ( !_this.HasEndings() )
                    {
                        tuplesOffset = 0;
                        tuples       = new SortedListIntPtrKey< Pair[] >.Tuple[ len ];                    
                    }
                    else
                    {
                        tuplesOffset = _this._Endings.Count;
                        tuples       = new SortedListIntPtrKey< Pair[] >.Tuple[ len + tuplesOffset ];

                        Array.Copy( _this._Endings.Tuples, tuples, tuplesOffset );
                        #region comm. prev.
                        //for ( int i = 0; i < tuplesOffset; i++ )
                        //{
                        //    tuples[ i ] = _this._Endings.Array[ i ];
                        //} 
                        #endregion
                    }

                    for ( int i = 0; i < len; i++ )
                    {
                        var p = morphoType.MorphoFormEndingUpperAndMorphoAttributes[ i ];
                        var pairs_current_len = p.MorphoAttributes.Length;
                        var pairs_current = new Pair[ pairs_current_len ];
                        for ( int j = 0; j < pairs_current_len; j++ )
                        {
                            var ma = MorphoAttributePair.GetMorphoAttribute( morphoType, p.MorphoAttributes[ j ], in nounType );
                            pairs_current[ j ] = new Pair( baseMorphoForm, ma );
                        }
                        tuples[ i + tuplesOffset ] = new SortedListIntPtrKey< Pair[] >.Tuple() { Key = p.EndingUpper, Value = pairs_current };
                    }

                    ShellSortAscending     ( tuples     );
                    MergeSorted            ( ref tuples );
                    _this._Endings.SetArray( tuples     );

                    return;
                }
                #endregion

                if ( !_this._Slots.TryGetValue( first_char, out _this_next ) )
                {
                    /// добавление новой буквы
                    _this_next = new TreeDictionary();
                    _this._Slots.Add( first_char, _this_next );
                }                
                _this = _this_next;
                wordBase++;
            }
        }
        public void AddWord( char* wordBase, MorphoTypeNative morphoType )
        {
            var baseMorphoForm = new BaseMorphoFormNative( wordBase, morphoType );

            for ( TreeDictionary _this = this, _this_next; ; )
            {
                var first_char = _UPPER_INVARIANT_MAP[ *wordBase ];

                #region [.сохранение характеристик if end-of-word.]
                if ( first_char == '\0' )
	            {
                    // sort & merge after insert - may be faster/better
                    var len = morphoType.MorphoFormEndingUpperAndMorphoAttributes.Length;
                    SortedListIntPtrKey< Pair[] >.Tuple[] tuples;
                    int tuplesOffset;
                    if ( !_this.HasEndings() )
                    {
                        tuplesOffset = 0;
                        tuples       = new SortedListIntPtrKey< Pair[] >.Tuple[ len ];                    
                    }
                    else
                    {
                        tuplesOffset = _this._Endings.Count;
                        tuples       = new SortedListIntPtrKey< Pair[] >.Tuple[ len + tuplesOffset ];

                        Array.Copy( _this._Endings.Tuples, tuples, tuplesOffset );
                        #region comm. prev.
                        //for ( int i = 0; i < tuplesOffset; i++ )
                        //{
                        //    tuples[ i ] = _this._Endings.Array[ i ];
                        //} 
                        #endregion
                    }

                    for ( int i = 0; i < len; i++ )
                    {
                        var p = morphoType.MorphoFormEndingUpperAndMorphoAttributes[ i ];
                        var pairs_current_len = p.MorphoAttributes.Length;
                        var pairs_current = new Pair[ pairs_current_len ];
                        for ( int j = 0; j < pairs_current_len; j++ )
                        {
                            //---var ma = MorphoAttributePair.GetMorphoAttribute( morphoType, p.MorphoAttributes[ j ], in nounType );
                            pairs_current[ j ] = new Pair( baseMorphoForm, p.MorphoAttributes[ j ] );
                        }
                        tuples[ i + tuplesOffset ] = new SortedListIntPtrKey< Pair[] >.Tuple() { Key = p.EndingUpper, Value = pairs_current };
                    }

                    ShellSortAscending     ( tuples     );
                    MergeSorted            ( ref tuples );
                    _this._Endings.SetArray( tuples     );

                    return;
                }
                #endregion

                if ( !_this._Slots.TryGetValue( first_char, out _this_next ) )
                {
                    /// добавление новой буквы
                    _this_next = new TreeDictionary();
                    _this._Slots.Add( first_char, _this_next );
                }                
                _this = _this_next;
                wordBase++;
            }
        }

        private static void ShellSortAscending( SortedListIntPtrKey< Pair[] >.Tuple[] array )
        {
            for ( int arrayLength = array.Length, gap = (arrayLength >> 1); 0 < gap; gap = (gap >> 1) )
            {
                for ( int i = 0, len = arrayLength - gap; i < len; i++ ) //modified insertion sort
                {
                    int j = i + gap;
                    SortedListIntPtrKey< Pair[] >.Tuple tmp = array[ j ];
                    while ( gap <= j /*&& (CompareRoutine( tmp.Key, array[ j - gap ].Key ) < 0)*/ /*(tmp < array[ j - gap ])*/ )
                    {
                        var k = j - gap;
                        var t = array[ k ];
                        if ( 0 <= SortedListIntPtrKey< Pair[] >.CompareRoutine( tmp.Key, t.Key ) )
                            break;
                        array[ j ] = t;
                        j = k;
                    }
                    array[ j ] = tmp;
                }
            }
        }
        private static void MergeSorted( ref SortedListIntPtrKey< Pair[] >.Tuple[] array )
        {
            var emptyCount = 0;
            var i_prev = 0;
            var t_prev = array[ 0 ];
            var arrayLength = array.Length;
            var pair_exists = default(Pair);
            for ( int i = 1; i < arrayLength; i++ )
            {
                var t_curr = array[ i ];
                //comparing native-strings, given that duplicates have the same addresses
                if ( t_prev.Key == t_curr.Key )
                /*full comparing native-strings (same addresses & char-to-char compare)*/
                /*if ( SortedListIntPtrKey< pair_t[] >.CompareRoutine( t_prev.Key, t_curr.Key ) == 0 )*/
                {
                    array[ i ].Key = IntPtr.Zero;
                    emptyCount++;
                      
                    var pairs = t_curr.Value;
                    for ( int j = 0, ln = pairs.Length; j < ln; j++ )
                    {
                        var pair = pairs[ j ];
                        if ( !_TempBufferPairs.TryAddOrGetExists( pair, ref pair_exists ) )
                        {
                            pair_exists.BaseMorphoForm.AppendMorphoFormEndings( pair.BaseMorphoForm );
                        }
                    }

                    pairs = t_prev.Value;
                    for ( int j = 0, ln = pairs.Length; j < ln; j++ )
                    {
                        var pair = pairs[ j ];
                        if ( !_TempBufferPairs.TryAddOrGetExists( pair, ref pair_exists ) )
                        {
                            pair_exists.BaseMorphoForm.AppendMorphoFormEndings( pair.BaseMorphoForm );
                        }
                    }

                    var pairs_new = _TempBufferPairs_ToArrayAndClear();
                    array[ i_prev ].Value = pairs_new;
                }
                else
                {
                    t_prev = t_curr;
                    i_prev = i;
                }
            }

            if ( emptyCount != 0 )
            {
                var array_new = new SortedListIntPtrKey< Pair[] >.Tuple[ arrayLength - emptyCount ];
                for ( int i = 0, j = 0; i < arrayLength; i++ )
                {
                    var t_curr = array[ i ];
                    if ( t_curr.Key != IntPtr.Zero )
                    {
                        array_new[ j++ ] = t_curr;
                    }
                }
                array = array_new;
            }
        }

        private static string ToString( SortedListIntPtrKey< Pair[] >.Tuple[] array )
        {
            var sb = new StringBuilder();
            foreach ( var a in array )
            {
                sb.Append( StringsHelper.ToString( a.Key ) ).Append( Environment.NewLine );
            }
            return (sb.ToString());
        }
        #endregion

        #region [.GetWordFormMorphologies & GetWordForms.]
        /// получение морфологических свойств слова
        /// wordUpper - слово
	    /// result - коллекция информаций о формах слова	    
        public bool TryGetWordFormMorphologies( string wordUpper, ICollection< WordFormMorphology_t > result, WordFormMorphologyModeEnum wordFormMorphologyMode )
        {
            result.Clear();
            fixed ( char* wordUpper_ptr = wordUpper )
            {
                switch ( wordFormMorphologyMode )
                {
                    case WordFormMorphologyModeEnum.Default:
                        FillWordFormMorphologies( wordUpper_ptr, result );
                    break;

                    case WordFormMorphologyModeEnum.FirstStartsWithUpperAfterLowerLetter:
                    {
                        //---FillWordFormMorphologies_FirstStartsWithUpperAfterLowerLetter( wordUpper_ptr, result );

                        FillWordFormMorphologies_StartsWithUpperLetter( wordUpper_ptr, result );
                        if ( result.Count == 0 )
                        {
                            FillWordFormMorphologies_StartsWithLowerLetter( wordUpper_ptr, result );
                        }
                    }
                    break;

                    case WordFormMorphologyModeEnum.FirstStartsWithLowerAfterUpperLetter:
                    {
                        //---FillWordFormMorphologies_FirstStartsWithLowerAfterUpperLetter( wordUpper_ptr, result );

                        FillWordFormMorphologies_StartsWithLowerLetter( wordUpper_ptr, result );
                        if ( result.Count == 0 )
                        {
                            FillWordFormMorphologies_StartsWithUpperLetter( wordUpper_ptr, result );
                        }
                    }
                    break;

                    case WordFormMorphologyModeEnum.StartsWithLowerLetter:
                        FillWordFormMorphologies_StartsWithLowerLetter( wordUpper_ptr, result );
                    break;

                    case WordFormMorphologyModeEnum.StartsWithUpperLetter:
                        FillWordFormMorphologies_StartsWithUpperLetter( wordUpper_ptr, result );
                    break;                    
                }
            }
            return (result.Count != 0);
        }
        public bool TryGetWordFormMorphologies( char* wordUpper, ICollection< WordFormMorphology_t > result, WordFormMorphologyModeEnum wordFormMorphologyMode )
        {
            result.Clear();
            {
                switch ( wordFormMorphologyMode )
                {
                    case WordFormMorphologyModeEnum.Default:
                        FillWordFormMorphologies( wordUpper, result );
                    break;

                    case WordFormMorphologyModeEnum.FirstStartsWithUpperAfterLowerLetter:
                        FillWordFormMorphologies_FirstStartsWithUpperAfterLowerLetter( wordUpper, result );
                    break;

                    case WordFormMorphologyModeEnum.FirstStartsWithLowerAfterUpperLetter:
                        FillWordFormMorphologies_FirstStartsWithLowerAfterUpperLetter( wordUpper, result );
                    break;

                    case WordFormMorphologyModeEnum.StartsWithLowerLetter:
                        FillWordFormMorphologies_StartsWithLowerLetter( wordUpper, result );
                    break;

                    case WordFormMorphologyModeEnum.StartsWithUpperLetter:
                        FillWordFormMorphologies_StartsWithUpperLetter( wordUpper, result );
                    break;                    
                }
            }
            return (result.Count != 0);
        }

	    /// получение всех форм слова
        /// wordUpper - слово
	    /// result - коллекция форм слова	    
        public bool TryGetWordForms( string wordUpper, ICollection< WordForm_t > result )
        {
            result.Clear();
            fixed ( char* word_ptr = wordUpper )
            {
                FillWordForms( word_ptr, result );
            }
            return (result.Count != 0);
        }

	    /// поиск слова в словаре
	    /// word   - слово
	    /// result - коллекция форм слова
        private void FillWordFormMorphologies( char* word, ICollection< WordFormMorphology_t > result )
        {
            var _this = this;
            _this.FillWordFormMorphologies_Core( word, result );            
            for ( var first_char = *word; (first_char != '\0') && _this._Slots.TryGetValue( first_char, out var _this_next ); first_char = *word )
            {
                _this_next.FillWordFormMorphologies_Core( ++word, result );
                _this = _this_next;
            }

            #region comm. prev-recurrent.
            /*
            FillWordFormMorphologies_Core( word, result );
            var first_char = *word;
            if ( (first_char != '\0') && _Slots.TryGetValue( first_char, out var next ) )
            {
                next.FillWordFormMorphologies( word + 1, result );
            }
            //*/
            #endregion
        }
        private void FillWordFormMorphologies_FirstStartsWithUpperAfterLowerLetter( char* word, ICollection< WordFormMorphology_t > result )
        {
            var _this = this;
            _this.FillWordFormMorphologies_FirstStartsWithUpperAfterLowerLetter_Core( word, result );
            for ( var first_char = *word; (first_char != '\0') && _this._Slots.TryGetValue( first_char, out var _this_next ); first_char = *word )
            {
                _this_next.FillWordFormMorphologies_FirstStartsWithUpperAfterLowerLetter_Core( ++word, result );
                _this = _this_next;
            }

            #region comm. prev-recurrent.
            /*
            FillWordFormMorphologies_FirstStartsWithUpperAfterLowerLetter_Core( word, result );
            var first_char = *word;
            if ( (first_char != '\0') && _Slots.TryGetValue( first_char, out var next ) )
            {
                next.FillWordFormMorphologies_FirstStartsWithUpperAfterLowerLetter( word + 1, result );
            }
            //*/
            #endregion
        }
        private void FillWordFormMorphologies_FirstStartsWithLowerAfterUpperLetter( char* word, ICollection< WordFormMorphology_t > result )
        {
            var _this = this;
            _this.FillWordFormMorphologies_FirstStartsWithLowerAfterUpperLetter_Core( word, result );
            for ( var first_char = *word; (first_char != '\0') && _this._Slots.TryGetValue( first_char, out var _this_next ); first_char = *word )
            {
                _this_next.FillWordFormMorphologies_FirstStartsWithLowerAfterUpperLetter_Core( ++word, result );
                _this = _this_next;
            }

            #region comm. prev-recurrent.
            /*
            FillWordFormMorphologies_FirstStartsWithLowerAfterUpperLetter_Core( word, result );
            var first_char = *word;
            if ( (first_char != '\0') && _Slots.TryGetValue( first_char, out var next ) )
            {
                next.FillWordFormMorphologies_FirstStartsWithLowerAfterUpperLetter( word + 1, result );
            }
            //*/
            #endregion
        }
        private void FillWordFormMorphologies_StartsWithLowerLetter( char* word, ICollection< WordFormMorphology_t > result )
        {
            var _this = this;
            _this.FillWordFormMorphologies_StartsWithLowerLetter_Core( word, result );
            for ( var first_char = *word; (first_char != '\0') && _this._Slots.TryGetValue( first_char, out var _this_next ); first_char = *word )
            {
                _this_next.FillWordFormMorphologies_StartsWithLowerLetter_Core( ++word, result );
                _this = _this_next;
            }

            #region comm. prev-recurrent.
            /*
            FillWordFormMorphologies_StartsWithLowerLetter_Core( word, result );
            var first_char = *word;
            if ( (first_char != '\0') && _Slots.TryGetValue( first_char, out var value ) )
            {
                value.FillWordFormMorphologies_StartsWithLowerLetter( word + 1, result );
            }
            //*/
            #endregion
        }
        private void FillWordFormMorphologies_StartsWithUpperLetter( char* word, ICollection< WordFormMorphology_t > result )
        {
            var _this = this;
            _this.FillWordFormMorphologies_StartsWithUpperLetter_Core( word, result );
            for ( var first_char = *word; (first_char != '\0') && _this._Slots.TryGetValue( first_char, out var _this_next ); first_char = *word )
            {
                _this_next.FillWordFormMorphologies_StartsWithUpperLetter_Core( ++word, result );
                _this = _this_next;
            }

            #region comm. prev-recurrent.
            /*
            FillWordFormMorphologies_StartsWithUpperLetter_Core( word, result );
            var first_char = *word;
            if ( (first_char != '\0') && _Slots.TryGetValue( first_char, out var next ) )
            {
                next.FillWordFormMorphologies_StartsWithUpperLetter( word + 1, result );
            }
            //*/
            #endregion
        }
        private void FillWordForms( char* word, ICollection< WordForm_t > result )
        {
            var _this = this;
            _this.FillWordForms_Core( word, result );
            for ( var first_char = *word; (first_char != '\0') && _this._Slots.TryGetValue( first_char, out var _this_next ); first_char = *word )
            {
                _this_next.FillWordForms_Core( ++word, result );
                _this = _this_next;
            }

            #region comm. prev-recurrent.
            /*
            FillWordForms_Core( word, result );
            var first_char = *word;
            if ( (first_char != '\0') && _Slots.TryGetValue( first_char, out var next ) )
            {
                next.FillWordForms( word + 1, result );
            }
            //*/
            #endregion
        }

        /// поиск слова в слоте
        /// wordPart - оставшаяся часть слова
        /// result   - коллекция форм слова
        private void FillWordFormMorphologies_Core( char* wordPart, ICollection< WordFormMorphology_t > result )
        {
            if ( HasEndings() && _Endings.TryGetValue( (IntPtr) wordPart, out var pairs ) )
            {
                for ( int i = 0, len = pairs.Length; i < len; i++ )
                {
                    var p = pairs[ i ];
                    var wfmi = new WordFormMorphology_t( p.BaseMorphoForm, p.MorphoAttribute );
                    result.Add( wfmi ); 
                }         
            }
        }
        private void FillWordFormMorphologies_FirstStartsWithUpperAfterLowerLetter_Core( char* wordPart, ICollection< WordFormMorphology_t > result )
        {
            if ( HasEndings() && _Endings.TryGetValue( (IntPtr) wordPart, out var pairs ) )
            {
                var findWithUpper = false;
                var len = pairs.Length;
                for ( var i = 0; i < len; i++ )
                {
                    var p = pairs[ i ];
                    var baseMorphoForm = p.BaseMorphoForm;
                    var first_char = *baseMorphoForm.Base;
                    if ( (first_char != '\0') && _UPPER_INVARIANT_MAP[ first_char ] != first_char )
                    {
                        continue;
                    }

                    findWithUpper = true;
                    var wfmi = new WordFormMorphology_t( baseMorphoForm, p.MorphoAttribute );
                    result.Add( wfmi );
                }

                if ( !findWithUpper )
                {
                    for ( var i = 0; i < len; i++ )
                    {
                        var p = pairs[ i ];
                        var wfmi = new WordFormMorphology_t( p.BaseMorphoForm, p.MorphoAttribute );
                        result.Add( wfmi ); 
                    }
                }
            }
        }
        private void FillWordFormMorphologies_FirstStartsWithLowerAfterUpperLetter_Core( char* wordPart, ICollection< WordFormMorphology_t > result )
        {
            if ( HasEndings() && _Endings.TryGetValue( (IntPtr) wordPart, out var pairs ) )
            {
                var findWithLower = false;
                var len = pairs.Length;
                for ( var i = 0; i < len; i++ )
                {
                    var p = pairs[ i ];
                    var baseMorphoForm = p.BaseMorphoForm;
                    var first_char = *baseMorphoForm.Base;
                    if ( (first_char != '\0') && _UPPER_INVARIANT_MAP[ first_char ] == first_char )
                    {
                        continue;
                    }

                    findWithLower = true;
                    var wfmi = new WordFormMorphology_t( baseMorphoForm, p.MorphoAttribute );
                    result.Add( wfmi );
                }

                if ( !findWithLower )
                {
                    for ( var i = 0; i < len; i++ )
                    {
                        var p = pairs[ i ];
                        var wfmi = new WordFormMorphology_t( p.BaseMorphoForm, p.MorphoAttribute );
                        result.Add( wfmi ); 
                    }
                }
            }
        }
        private void FillWordFormMorphologies_StartsWithLowerLetter_Core( char* wordPart, ICollection< WordFormMorphology_t > result )
        {
            if ( HasEndings() && _Endings.TryGetValue( (IntPtr) wordPart, out var pairs ) )
            {
                for ( int i = 0, len = pairs.Length; i < len; i++ )
                {
                    var p = pairs[ i ];
                    var baseMorphoForm = p.BaseMorphoForm;
                    var first_char = *baseMorphoForm.Base;
                    if ( (first_char != '\0') && _UPPER_INVARIANT_MAP[ first_char ] == first_char )
                    {
                        continue;
                    }

                    var wfmi = new WordFormMorphology_t( baseMorphoForm, p.MorphoAttribute );
                    result.Add( wfmi ); 
                }
            }
        }
        private void FillWordFormMorphologies_StartsWithUpperLetter_Core( char* wordPart, ICollection< WordFormMorphology_t > result )
        {
            if ( HasEndings() && _Endings.TryGetValue( (IntPtr) wordPart, out var pairs ) )
            {
                for ( int i = 0, len = pairs.Length; i < len; i++ )
                {
                    var p = pairs[ i ];
                    var baseMorphoForm = p.BaseMorphoForm;
                    var first_char = *baseMorphoForm.Base;
                    if ( (first_char != '\0') && _UPPER_INVARIANT_MAP[ first_char ] != first_char )
                    {
                        continue;
                    }

                    var wfmi = new WordFormMorphology_t( baseMorphoForm, p.MorphoAttribute );
                    result.Add( wfmi ); 
                }
            }
        }
        private void FillWordForms_Core( char* wordPart, ICollection< WordForm_t > result )
        {
            if ( HasEndings() && _Endings.TryGetValue( (IntPtr) wordPart, out var pairs ) )
            {
                for ( int i = 0, len = pairs.Length; i < len; i++ )
                {
                    var p = pairs[ i ];
                    var partOfSpeech = p.BaseMorphoForm.PartOfSpeech;
                    var _base        = p.BaseMorphoForm.Base;
                    fixed ( char** morphoFormsEnding = p.BaseMorphoForm.MorphoFormEndings )
                    {
                        for ( int j = 0, mf_len = p.BaseMorphoForm.MorphoFormEndings.Length; j < mf_len; j++ )
                        {
                            /// получение словоформы
                            var wordForm = StringsHelper.CreateWordForm( _base, *(morphoFormsEnding + j) );

                            var wf = new WordForm_t( wordForm, partOfSpeech );
                            result.Add( wf );
                        }
                    }
                }
            }
        }
        #endregion
    }
}