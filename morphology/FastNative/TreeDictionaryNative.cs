using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using lingvo.core;
using lingvo.morphology;

namespace lingvo.morphology
{
    /// <summary>
    /// Словарь-дерево для слов
    /// </summary>
    unsafe internal sealed class TreeDictionaryNative
    {
        /// <summary>
        /// 
        /// </summary>
        private struct pair
        {
            public pair( BaseMorphoFormNative baseMorphoForm, MorphoAttributeEnum morphoAttribute )
            {
                BaseMorphoForm  = baseMorphoForm;
                MorphoAttribute = morphoAttribute;
            }

            public BaseMorphoFormNative BaseMorphoForm;
            public MorphoAttributeEnum  MorphoAttribute;

            public override int GetHashCode()
            {
                return ((int) MorphoAttribute ^ //MorphoAttribute.GetHashCode() ^ 
                        (int) BaseMorphoForm.Base ^ //((IntPtr) BaseMorphoForm.Base).GetHashCode() ^                         
                        (int) BaseMorphoForm.PartOfSpeech ^ //BaseMorphoForm.PartOfSpeech.GetHashCode() ^
                        (int) BaseMorphoForm.MorphoFormEndings[ 0 ] //((IntPtr) BaseMorphoForm.MorphoFormEndings[ 0 ]).GetHashCode() 
                       );
            }
#if DEBUG
            public override string ToString()
            {
                return (BaseMorphoForm + ", " + MorphoAttribute);
            }
#endif
        }
        /// <summary>
        /// 
        /// </summary>
        private struct pair_IEqualityComparer : IEqualityComparer< pair >
        {
            #region [.IEqualityComparer< pair_t >.]
            public bool Equals( pair x, pair y )
            {
                if ( x.MorphoAttribute != y.MorphoAttribute )
                    return (false);

                if( x.BaseMorphoForm.Base != y.BaseMorphoForm.Base )
                    return (false);

                if ( x.BaseMorphoForm.PartOfSpeech != y.BaseMorphoForm.PartOfSpeech )
                    return (false);

                if ( x.BaseMorphoForm.MorphoFormEndings[ 0 ] != y.BaseMorphoForm.MorphoFormEndings[ 0 ] )
                    return (false);

                #region commented
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
            public int GetHashCode( pair obj )
            {
                return (obj.GetHashCode());
            }
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
	    private sealed class PairSet : IEnumerable< pair >
	    {
            /// <summary>
            /// 
            /// </summary>
		    internal struct Slot
		    {                
                internal int  hashCode;
			    internal int  next;                
                internal pair value;
		    }

            /// <summary>
            /// 
            /// </summary>
            internal struct Enumerator : IEnumerator< pair >
            {
                private PairSet _Set;
                private pair  _Current;
                private int     _Index;

                public pair Current
	            {
		            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		            get { return _Current; }
	            }
	            object IEnumerator.Current
	            {
		            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		            get
		            {
                        if ( _Index == 0 || _Index == _Set._Count + 1 )
			            {
				            throw (new InvalidOperationException("InvalidOperation_EnumOpCantHappen"));
			            }
			            return (_Current);
		            }
	            }

                internal Enumerator( PairSet set )
	            {
		            this._Set     = set;
		            this._Index   = 0;
		            this._Current = default(pair);
	            }
                public void Dispose()
                {
                }

	            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
	            public bool MoveNext()
	            {
                    while ( _Index < _Set._Count )
		            {
                        _Current = _Set._Slots[ _Index ].value;
                        _Index++;
                        if ( _Current.BaseMorphoForm != null )
                        {                            
                            return (true);
                        }
                        /*var slot = _Set._Slots[ _Index ];
                        if ( 0 <= slot.hashCode )
			            {
                            _Current = slot.value;
				            _Index++;
				            return (true);
			            }
			            _Index++;
                        */
                    }
                    _Index = _Set._Count + 1;
		            _Current = default(pair);
		            return (false);
	            }

	            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
	            void IEnumerator.Reset()
	            {
		            _Index   = 0;
                    _Current = default(pair);
	            }
            }            

		    private int[]  _Buckets;
		    private Slot[] _Slots;
		    private int    _Count;
		    private int    _FreeList;
            private pair_IEqualityComparer _Comparer;

            internal Slot[] Slots
            {
                [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
                get { return (_Slots); }
            }
            public int Count
            {
                [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
                get { return (_Count); }
            }

		    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		    public PairSet( int capacity )
		    {
			    _Buckets  = new int [ capacity ];
			    _Slots    = new Slot[ capacity ];
			    _FreeList = -1;
		    }

            /// <summary>
            /// try add not-exists-item & return-(true), else get exists-item to 'existsValue' & return-(false)
            /// </summary>
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            public bool TryAddOrGetExists( pair keyValue, ref pair existsValue )
		    {
                #region [.find exists.]
                int hash = (keyValue.GetHashCode() & 0x7FFFFFFF);
                for ( int i = _Buckets[ hash % _Buckets.Length ] - 1; 0 <= i; )
                {
                    var slot = _Slots[ i ];
                    if ( (slot.hashCode == hash) && _Comparer.Equals( slot.value, keyValue ) )
                    {
                        existsValue = slot.value;
                        return (false);
                    }
                    i = slot.next;
                }
                #endregion

                #region [.add new.]
                int n1;
                if ( 0 <= _FreeList )
                {
                    n1 = _FreeList;
                    _FreeList = _Slots[ n1 ].next;
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
                    hashCode = hash,
                    value    = keyValue,
                    next     = _Buckets[ n2 ] - 1,
                };
                _Buckets[ n2 ] = n1 + 1;

                return (true);
                #endregion
            }

            /*[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            public bool TryGetValue( pair_t keyValue, ref pair_t existsValue )
            {
                #region [.find exists.]
                int hash = (keyValue.GetHashCode() & 0x7fffffff);
                for ( int i = _Buckets[ hash % _Buckets.Length ] - 1; 0 <= i; )
                {
                    var slot = _Slots[ i ];
                    if ( (slot.hashCode == hash) && _Comparer.Equals( slot.value, keyValue ) )
                    {
                        existsValue = slot.value;
                        return (true);
                    }
                    i = slot.next;
                }

                return (false);
                #endregion
            }*/

            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            public void Clear()
            {
                if ( 0 < _Count )
	            {
                    Array.Clear( _Slots,   0, _Count );
                    Array.Clear( _Buckets, 0, _Buckets.Length );
                    _Count    = 0;
                    _FreeList = -1;
	            }                    
            }

            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		    private void Resize()
		    {
                int n1 = checked( _Count * 2 + 1 );
                int[]  buckets = new int[ n1 ];
                Slot[] slots   = new Slot[ n1 ];
                Array.Copy( _Slots, 0, slots, 0, _Count );
                for ( int i = 0; i < _Count; i++ )
                {
                    int n2 = slots[ i ].hashCode % n1;
                    slots[ i ].next = buckets[ n2 ] - 1;
                    buckets[ n2 ] = i + 1;
                }
                _Buckets = buckets;
                _Slots   = slots;
		    }

            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            public PairSet.Enumerator GetEnumerator()
            {
                return (new PairSet.Enumerator( this ));
            }

            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            IEnumerator< pair > IEnumerable< pair >.GetEnumerator()
            {
                return (new PairSet.Enumerator( this ));
            }

            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return (new PairSet.Enumerator( this ));
            }
	    }

        #region [.temp-buffers (static, because model must loading in single-thread).]
        private static PairSet tempBufferPairs; /*---private static Dictionary< pair_t, pair_t > tempBufferPairs;*/
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        private static pair[] tempBufferPairs_ToArrayAndClear()
        {
            var pairs_new = new pair[ tempBufferPairs.Count ];
            var it = tempBufferPairs.GetEnumerator();
            for ( var i = 0; it.MoveNext(); i++ )
            {
                pairs_new[ i ] = it.Current;
            }
            tempBufferPairs.Clear();
            return (pairs_new);
        }
        private static char* _UPPER_INVARIANT_MAP;

        static TreeDictionaryNative()
        {
            _UPPER_INVARIANT_MAP = xlat_Unsafe.Inst._UPPER_INVARIANT_MAP;
        }

        internal static void BeginLoad()
        {
            const int DEFAULT_CAPACITY = 11; //37;

            BaseMorphoFormNative.BeginLoad();
            MorphoTypeNative    .BeginLoad();

            tempBufferPairs = new PairSet( DEFAULT_CAPACITY ); /*---tempBufferPairs = new Dictionary< pair_t, pair_t >( default(pair_t_IEqualityComparer) );*/
        }
        internal static void EndLoad()
        {
            BaseMorphoFormNative.EndLoad();
            MorphoTypeNative    .EndLoad();
            tempBufferPairs = null;
        }
        #endregion

        #region [.private field's.]
        /// слот для дочерних слов
        private SortedListCharKey< TreeDictionaryNative > _Slots;
        /// коллекция информаций о формах слова
        private SortedListIntPtrKey< pair[] > _Endings;

        private bool HasEndings()
        {
            return (_Endings.Array != null);
        } 
        #endregion

        #region [.ctor().]
        public TreeDictionaryNative()
        {
            _Slots.InitArrayAsEmpty(); //---_Slots = sorted_list_key_char_t< TreeDictionaryNative >.Create( /*DEFAULT_SLOTS_CAPACITY*/ );
        }
        #endregion

        internal void Trim()
        {
            if ( HasEndings() )
            {
                _Endings.Trim();
            }
            _Slots.Trim();
            for ( int i = 0, len = _Slots.Count; i < len; i++ )
            {
                _Slots.Array[ i ].Value.Trim();
            }
        }

        #region [.Append word.]
        /// добавление слова и всех его форм в словарь
        /// wordBase   - marshaled-слово
        /// morphoType - морфотип
        /// nounType   - тип сущетсвительного
        public void AddWord( char* wordBase, MorphoTypeNative morphoType, ref MorphoAttributePair? nounType )
        {
            var baseMorphoForm = new BaseMorphoFormNative( wordBase, morphoType );

            for ( TreeDictionaryNative _this = this, _this_next; ; )
            {
                var first_char = _UPPER_INVARIANT_MAP[ *wordBase ];

                #region [.сохранение характеристик if end-of-word.]
                if ( first_char == '\0' )
	            {
                    // sort & merge after insert - may be faster/better (30.04.2014)!!!!
                    //*
                    var len = morphoType.MorphoFormEndingUpperAndMorphoAttributes.Length;
                    SortedListIntPtrKey< pair[] >.Tuple[] tuples;
                    int tuplesOffset;
                    if ( !_this.HasEndings() )
                    {
                        tuplesOffset = 0;
                        tuples = new SortedListIntPtrKey< pair[] >.Tuple[ len ];                    
                    }
                    else
                    {
                        tuplesOffset = _this._Endings.Count;
                        tuples = new SortedListIntPtrKey< pair[] >.Tuple[ len + tuplesOffset ];

                        for ( int i = 0; i < tuplesOffset; i++ )
                        {
                            tuples[ i ] = _this._Endings.Array[ i ];
                        }
                    }

                    for ( int i = 0; i < len; i++ )
                    {
                        var p = morphoType.MorphoFormEndingUpperAndMorphoAttributes[ i ];
                        var pairs_current_len = p.MorphoAttributes.Length;
                        var pairs_current = new pair[ pairs_current_len ];
                        for ( int j = 0; j < pairs_current_len; j++ )
                        {
                            var ma = MorphoAttributePair.GetMorphoAttribute( morphoType, p.MorphoAttributes[ j ], ref nounType );
                            pairs_current[ j ] = new pair( baseMorphoForm, ma );
                        }
                        tuples[ i + tuplesOffset ] = new SortedListIntPtrKey< pair[] >.Tuple() { Key = p.EndingUpper, Value = pairs_current };
                    }

                    ShellSortAscending     ( tuples     );
                    MergeSorted            ( ref tuples );
                    _this._Endings.SetArray( tuples     );
                    //*/
                    return;
                }
                #endregion

                if ( !_this._Slots.TryGetValue( first_char, out _this_next ) )
                {
                    /// добавление новой буквы
                    _this_next = new TreeDictionaryNative();
                    _this._Slots.Add( first_char, _this_next );
                }                
                _this = _this_next;
                wordBase++;
            }
        }

        #region [.commented. previous.]
        ///// <summary>
        ///// 
        ///// </summary>
        //private struct addword_info_t
        //{
        //    public char*                wordPart;
        //    public BaseMorphoFormNative baseMorphoForm;
        //    public MorphoTypeNative     morphoType;
        //    public MorphoAttributePair? nounType;
        //}

        ///// добавление слова и всех его форм в словарь
        ///// basePtr    - marshaled-слово
        ///// _base      - слово
        ///// morphoType - морфотип
        ///// nounType   - тип сущетсвительного
        //public void ____AddWord____( char* basePtr, MorphoTypeNative morphoType, ref MorphoAttributePair? nounType )
        //{
        //    var baseMorphoForm = new BaseMorphoFormNative( basePtr, morphoType );

        //    var aw = new addword_info_t()
        //    {
        //        wordPart       = basePtr,
        //        baseMorphoForm = baseMorphoForm,
        //        morphoType     = morphoType,
        //        nounType       = nounType,
        //    };

        //    AddWordPart( ref aw );
        //}

        ///// добавление части слова
        ///// wordPart - оставшася часть слова
        ///// baseMorphoForm - базовая форма
        //private void AddWordPart( ref addword_info_t aw )
        //{
        //    var first_char = _UPPER_INVARIANT_MAP[ *aw.wordPart ];
        //    if ( first_char == '\0' )
        //    /// сохранение характеристик
        //    {
        //        // sort & merge after insert - may be faster/better (30.04.2014)!!!!
        //        //*
        //        var len = aw.morphoType.MorphoFormEndingUpperAndMorphoAttributes.Length;
        //        var tuples = default(SortedListIntPtrKey< pair_t[] >.tuple_t[]);
        //        var tuplesOffset = default(int);
        //        if ( !HasEndings() )
        //        {
        //            tuplesOffset = 0;
        //            tuples = new SortedListIntPtrKey< pair_t[] >.tuple_t[ len ];                    
        //        }
        //        else
        //        {
        //            tuplesOffset = _Endings.Count;
        //            tuples = new SortedListIntPtrKey< pair_t[] >.tuple_t[ len + tuplesOffset ];

        //            for ( int i = 0; i < tuplesOffset; i++ )
        //            {
        //                tuples[ i ] = _Endings.Array[ i ];
        //            }
        //        }

        //        for ( int i = 0; i < len; i++ )
        //        {
        //            var p = aw.morphoType.MorphoFormEndingUpperAndMorphoAttributes[ i ];
        //            var pairs_current_len = p.MorphoAttributes.Length;
        //            var pairs_current = new pair_t[ pairs_current_len ];
        //            for ( int j = 0; j < pairs_current_len; j++ )
        //            {
        //                var ma = MorphoAttributePair.GetMorphoAttribute( aw.morphoType, p.MorphoAttributes[ j ], ref aw.nounType );
        //                pairs_current[ j ] = new pair_t( aw.baseMorphoForm, ma );
        //            }
        //            tuples[ i + tuplesOffset ] = new SortedListIntPtrKey< pair_t[] >.tuple_t() { Key = p.EndingUpper, Value = pairs_current };
        //        }

        //        ShellSortAscending( tuples     );
        //        MergeSorted       ( ref tuples );
        //        _Endings.SetArray ( tuples     );
        //        //*/

        //        #region previous-1. commented
        //        /*
        //        if ( !HasEndings() )
        //        {
        //            _Endings = new SortedListIntPtrKey< pair_t[] >( aw.morphoType.MorphoFormEndingUpperAndMorphoAttributes.Length );
        //        }

        //        for ( int i = 0, len = aw.morphoType.MorphoFormEndingUpperAndMorphoAttributes.Length; i < len; i++ )
        //        {
        //            var p = aw.morphoType.MorphoFormEndingUpperAndMorphoAttributes[ i ];
        //            #region commented
        //            /*
        //            var pairs_current = from morphoAttribute in p.MorphoAttributes
        //                                let ma = MorphoAttributePair.GetMorphoAttribute( morphoType, morphoAttribute, nounType )
        //                                select
        //                                    new pair_t( baseMorphoForm, ma );
        //            * /
        //            #endregion
        //            var pairs_current_len = p.MorphoAttributes.Length;

        //            var index = _Endings.IndexOfKeyCore( p.EndingUpper );
        //            if ( index < 0 )
        //            {                        
        //                var pairs_current = new pair_t[ pairs_current_len ];
        //                for ( int j = 0; j < pairs_current_len; j++ )
        //                {
        //                    var ma = MorphoAttributePair.GetMorphoAttribute( aw.morphoType, p.MorphoAttributes[ j ], aw.nounType );
        //                    pairs_current[ j ] = new pair_t( aw.baseMorphoForm, ma );
        //                }
        //                _Endings.Insert( ~index, p.EndingUpper, pairs_current );
        //            }
        //            else
        //            {
        //                pair_t pair_exists;
        //                for ( int j = 0; j < pairs_current_len; j++ )
        //                {
        //                    var ma = MorphoAttributePair.GetMorphoAttribute( aw.morphoType, p.MorphoAttributes[ j ], aw.nounType );
        //                    var pair = new pair_t( aw.baseMorphoForm, ma );

        //                    if ( !tempBufferPairs.TryGetValue( pair, out pair_exists ) )
        //                    {
        //                        tempBufferPairs.Add( pair, pair );
        //                    }
        //                    else
        //                    {
        //                        pair_exists.BaseMorphoForm.AppendMorphoFormEndings( pair.BaseMorphoForm );
        //                    }
        //                }

        //                var pairs = _Endings.GetValue( index );
        //                for ( int j = 0, pairs_len = pairs.Length; j < pairs_len; j++ )
        //                {
        //                    var pair = pairs[ j ];
        //                    if ( !tempBufferPairs.TryGetValue( pair, out pair_exists ) )
        //                    {
        //                        tempBufferPairs.Add( pair, pair );
        //                    }
        //                    else
        //                    {
        //                        pair_exists.BaseMorphoForm.AppendMorphoFormEndings( pair.BaseMorphoForm );
        //                    }
        //                }
        //                pairs = null;

        //                var pairs_new = tempBufferPairs_ToArrayAndClear(); //var pairs_new = tempBufferPairs.Keys.ToArray();
        //                _Endings.SetValue( index, pairs_new );                        
        //            }
        //        }
        //        */
        //        #endregion

        //        #region previous-2. commented
        //        /*
        //        foreach ( var morphoForm in morphoType.MorphoForms )
        //        {
        //            var endingUpper_ptr = new IntPtr( morphoForm.EndingUpper );
        //            LinkedList< pair_t > pairs;
        //            if ( !tempBufferDict.TryGetValue( endingUpper_ptr, out pairs ) )
        //            {
        //                pairs = PopLinkedList();
        //                tempBufferDict.Add( endingUpper_ptr, pairs );
        //            }
        //            var morphoAttribute = MorphoAttributePair.GetMorphoAttribute( morphoType, morphoForm, nounType );
        //            pairs.AddLast( new pair_t( baseMorphoForm, morphoAttribute ) );
        //        }

        //        if ( _Endings == null )
        //        {
        //            _Endings = new SortedListIntPtrKey< pair_t[] >( tempBufferDict.Count );
        //        }
        //        foreach ( var p in tempBufferDict )
        //        {
        //            var index = _Endings.IndexOfKeyCore( p.Key );
        //            if ( index < 0 )
        //            {
        //                _Endings.Insert( ~index, p.Key, p.Value.ToArray() );
        //            }
        //            else
        //            {
        //                var pairs = _Endings.GetValue( index );

        //                #region version.1
        //                //*
        //                pair_t pair_exists;
        //                foreach ( var pair in pairs.Concat( p.Value ) )
        //                {
        //                    if ( !tempBufferPairs.TryGetValue( pair, out pair_exists ) )
        //                    {
        //                        tempBufferPairs.Add( pair, pair );
        //                    }
        //                    else
        //                    {
        //                        pair_exists.BaseMorphoForm.AppendMorphoFormEndings( pair.BaseMorphoForm );
        //                    }
        //                }
        //                var pairs_new = tempBufferPairs.Keys.ToArray();
        //                _Endings.SetValue( index, pairs_new );
        //                tempBufferPairs.Clear();
        //                //* /
        //                #endregion

        //                #region version.2
        //                /*
        //                var groups = pairs.Concat( p.Value ).GroupBy( _p => _p, pair_t_IEqualityComparer.Inst );

        //                var pairs_new = groups.Select( _g => 
        //                                { 
        //                                    _g.Key.BaseMorphoForm.SetMorphoFormEndings( _g.Select( _p => _p.BaseMorphoForm ) ); 
        //                                    return (_g.Key); 
        //                                })
        //                                .ToArray();
        //                _Endings.SetValue( index, pairs_new );
        //                //* /
        //                #endregion
        //            }

        //            PushLinkedList( p.Value );
        //        }
        //        tempBufferDict.Clear();
        //        */
        //        #endregion

        //        #region direct. commented
        //        /*
        //        if ( _Endings == null )
        //        {
        //            _Endings = new SortedListIntPtrKey< pair_t[] >( DEFAULT_ENDINGS_CAPACITY );
        //        }
        //        foreach ( var morphoForm in morphoType.MorphoForms )
        //        {
        //            var morphoAttribute = MorphoAttributePair.GetMorphoAttribute( baseMorphoForm, morphoType, morphoForm, nounType );
        //            var pair = new pair_t( baseMorphoForm, morphoAttribute );

        //            pair_t[] pairs;
        //            var endingUpper_ptr = new IntPtr( morphoForm.EndingUpper );
        //            if ( !_Endings.TryGetValue( endingUpper_ptr, out pairs ) )
        //            {
        //                _Endings.Add( endingUpper_ptr, new[] { pair } );
        //            }
        //            else
        //            {
        //                _Endings[ endingUpper_ptr ] = pairs.Concat( Enumerable.Repeat( pair, 1 ) ).ToArray();
        //            }
        //        }
        //        */
        //        #endregion
        //    }
        //    else
        //    {
        //        TreeDictionaryNative value;
        //        if ( !_Slots.TryGetValue( first_char, out value ) )
        //        {
        //            /// добавление новой буквы
        //            value = new TreeDictionaryNative();
        //            _Slots.Add( first_char, value );
        //        }
        //        aw.wordPart++;
        //        value.AddWordPart( ref aw );
        //    }
        //} 
        #endregion

        private static void ShellSortAscending( SortedListIntPtrKey< pair[] >.Tuple[] array )
        {
            for ( int arrayLength = array.Length, gap = (arrayLength >> 1); 0 < gap; gap = (gap >> 1) )
            {
                for ( int i = 0, len = arrayLength - gap; i < len; i++ ) //modified insertion sort
                {
                    int j = i + gap;
                    SortedListIntPtrKey< pair[] >.Tuple tmp = array[ j ];
                    while ( gap <= j /*&& (CompareRoutine( tmp.Key, array[ j - gap ].Key ) < 0)*/ /*(tmp < array[ j - gap ])*/ )
                    {
                        var k = j - gap;
                        var t = array[ k ];
                        if ( 0 <= SortedListIntPtrKey< pair[] >.CompareRoutine( tmp.Key, t.Key ) )
                            break;
                        array[ j ] = t;
                        j = k;
                    }
                    array[ j ] = tmp;
                }
            }
        }
        private static void MergeSorted( ref SortedListIntPtrKey< pair[] >.Tuple[] array )
        {
            var emptyCount = 0;
            var i_prev = 0;
            var t_prev = array[ 0 ];
            var arrayLength = array.Length;
            var pair_exists = default(pair);
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
                        if ( !tempBufferPairs.TryAddOrGetExists( pair, ref pair_exists ) )
                        {
                            pair_exists.BaseMorphoForm.AppendMorphoFormEndings( pair.BaseMorphoForm );
                        }
                    }

                    pairs = t_prev.Value;
                    for ( int j = 0, ln = pairs.Length; j < ln; j++ )
                    {
                        var pair = pairs[ j ];
                        if ( !tempBufferPairs.TryAddOrGetExists( pair, ref pair_exists ) )
                        {
                            pair_exists.BaseMorphoForm.AppendMorphoFormEndings( pair.BaseMorphoForm );
                        }
                    }

                    var pairs_new = tempBufferPairs_ToArrayAndClear();
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
                var array_new = new SortedListIntPtrKey< pair[] >.Tuple[ arrayLength - emptyCount ];
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

        private static string ToString( SortedListIntPtrKey< pair[] >.Tuple[] array )
        {
            var sb = new System.Text.StringBuilder();
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
        public bool GetWordFormMorphologies( string wordUpper, List< WordFormMorphology_t > result, WordFormMorphologyModeEnum wordFormMorphologyMode )
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
        public bool GetWordFormMorphologies( char* wordUpper, List< WordFormMorphology_t > result, WordFormMorphologyModeEnum wordFormMorphologyMode )
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
        public bool GetWordForms( string wordUpper, List< WordForm_t > result )
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
        private void FillWordFormMorphologies( char* word, List< WordFormMorphology_t > result )
        {
            FillWordFormMorphologies_Core( word, result );
            var first_char = *word;
            if ( first_char != '\0' )
            {
                TreeDictionaryNative value;
                if ( _Slots.TryGetValue( first_char, out value ) )
                {
                    value.FillWordFormMorphologies( word + 1, result );
                }
            }
        }
        private void FillWordFormMorphologies_FirstStartsWithUpperAfterLowerLetter
                                                    ( char* word, List< WordFormMorphology_t > result )
        {
            FillWordFormMorphologies_FirstStartsWithUpperAfterLowerLetter_Core( word, result );
            var first_char = *word;
            if ( first_char != '\0' )
            {
                TreeDictionaryNative value;
                if ( _Slots.TryGetValue( first_char, out value ) )
                {
                    value.FillWordFormMorphologies_FirstStartsWithUpperAfterLowerLetter( word + 1, result );
                }
            }
        }
        private void FillWordFormMorphologies_FirstStartsWithLowerAfterUpperLetter
                                                    ( char* word, List< WordFormMorphology_t > result )
        {
            FillWordFormMorphologies_FirstStartsWithLowerAfterUpperLetter_Core( word, result );
            var first_char = *word;
            if ( first_char != '\0' )
            {
                TreeDictionaryNative value;
                if ( _Slots.TryGetValue( first_char, out value ) )
                {
                    value.FillWordFormMorphologies_FirstStartsWithLowerAfterUpperLetter( word + 1, result );
                }
            }
        }
        private void FillWordFormMorphologies_StartsWithLowerLetter
                                                    ( char* word, List< WordFormMorphology_t > result )
        {
            FillWordFormMorphologies_StartsWithLowerLetter_Core( word, result );
            var first_char = *word;
            if ( first_char != '\0' )
            {
                TreeDictionaryNative value;
                if ( _Slots.TryGetValue( first_char, out value ) )
                {
                    value.FillWordFormMorphologies_StartsWithLowerLetter( word + 1, result );
                }
            }
        }
        private void FillWordFormMorphologies_StartsWithUpperLetter
                                                    ( char* word, List< WordFormMorphology_t > result )
        {
            FillWordFormMorphologies_StartsWithUpperLetter_Core( word, result );
            var first_char = *word;
            if ( first_char != '\0' )
            {
                TreeDictionaryNative value;
                if ( _Slots.TryGetValue( first_char, out value ) )
                {
                    value.FillWordFormMorphologies_StartsWithUpperLetter( word + 1, result );
                }
            }
        }
        private void FillWordForms( char* word, List< WordForm_t > result )
        {
            FillWordForms_Core( word, result );
            var first_char = *word;
            if ( first_char != '\0' )
            {
                TreeDictionaryNative value;
                if ( _Slots.TryGetValue( first_char, out value ) )
                {
                    value.FillWordForms( word + 1, result );
                }
            }
        }

	    /// поиск слова в слоте
	    /// wordPart - оставшаяся часть слова
        /// result   - коллекция форм слова
        private void FillWordFormMorphologies_Core( char* wordPart, List< WordFormMorphology_t > result )
        {
            if ( !HasEndings() )
                return;

            pair[] pairs;
            if ( _Endings.TryGetValue( (IntPtr) wordPart, out pairs ) )
            {
                for ( int i = 0, len = pairs.Length; i < len; i++ )
                {
                    var p = pairs[ i ];
                    var wfmi = new WordFormMorphology_t( p.BaseMorphoForm, p.MorphoAttribute );
                    result.Add( wfmi ); 
                }         
            }
        }
        private void FillWordFormMorphologies_FirstStartsWithUpperAfterLowerLetter_Core
                                                         ( char* wordPart, List< WordFormMorphology_t > result )
        {
            if ( !HasEndings() )
                return;

            pair[] pairs;
            if ( _Endings.TryGetValue( (IntPtr) wordPart, out pairs ) )
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
        private void FillWordFormMorphologies_FirstStartsWithLowerAfterUpperLetter_Core
                                                         ( char* wordPart, List< WordFormMorphology_t > result )
        {
            if ( !HasEndings() )
                return;

            pair[] pairs;
            if ( _Endings.TryGetValue( (IntPtr) wordPart, out pairs ) )
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
        private void FillWordFormMorphologies_StartsWithLowerLetter_Core
                                                         ( char* wordPart, List< WordFormMorphology_t > result )
        {
            if ( !HasEndings() )
                return;

            pair[] pairs;
            if ( _Endings.TryGetValue( (IntPtr) wordPart, out pairs ) )
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
        private void FillWordFormMorphologies_StartsWithUpperLetter_Core
                                                         ( char* wordPart, List< WordFormMorphology_t > result )
        {
            if ( !HasEndings() )
                return;

            pair[] pairs;
            if ( _Endings.TryGetValue( (IntPtr) wordPart, out pairs ) )
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
        private void FillWordForms_Core( char* wordPart, List< WordForm_t > result )
        {
            if ( !HasEndings() )
                return;

            pair[] pairs;
            if ( _Endings.TryGetValue( (IntPtr) wordPart, out pairs ) )
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