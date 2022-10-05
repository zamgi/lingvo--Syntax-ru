using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using lingvo.core;
using M = System.Runtime.CompilerServices.MethodImplAttribute;
using O = System.Runtime.CompilerServices.MethodImplOptions;

namespace lingvo.morphology
{
    /// <summary>
    /// 
    /// </summary>
    unsafe internal sealed class EnumParser< T > where T : struct
    {
        #region [.commented. slower. WordTrieNode_v1.]
        ///// <summary>
        ///// 
        ///// </summary>
        //private interface IWordTrieNode
        //{
        //    void Add( string s, T t );
        //    /*T? TryGetValue( string s );
        //    unsafe T? TryGetValue( char* ptr, char* endPtr );
        //    unsafe T? TryGetValueUnwrapRecursion( char* ptr, char* endPtr );*/
        //    unsafe bool TryGetValueUnwrapRecursion( char* ptr, char* endPtr, ref T value );

        //    IEnumerable< Tuple< string, T > > GetAll();
        //    int MaxCharsOnLevel();        
        //}


        ///// <summary>
        ///// 
        ///// </summary>
        //private sealed class WordTrieNode_v1 : IWordTrieNode
        //{
        //    private static char* _UPPER_INVARIANT_MAP;

        //    static WordTrieNode_v1()
        //    {
        //        _UPPER_INVARIANT_MAP = xlat_Unsafe.Inst._UPPER_INVARIANT_MAP;
        //    }

        //    private Dictionary< char, WordTrieNode_v1 > _CurrentLevel;
        //    private T? _TValue;

        //    private WordTrieNode_v1()
        //    {
        //        _CurrentLevel = new Dictionary< char, WordTrieNode_v1 >( 20 );
        //    }

        //    public void Add( string s, T t )
        //    {
        //        if ( !string.IsNullOrEmpty( s ) )
        //        {
        //            WordTrieNode_v1 nextLevel;
        //            var ch = _UPPER_INVARIANT_MAP[ s[ 0 ] ];
        //            if ( !_CurrentLevel.TryGetValue( ch, out nextLevel ) )
        //            {
        //                nextLevel = new WordTrieNode_v1();
        //                _CurrentLevel.Add( ch, nextLevel );
        //            }
        //            nextLevel.Add( s.Substring( 1 ), t );
        //        }
        //        else
        //        {
        //            _TValue = t;
        //        }
        //    }
        //    /*public T? TryGetValue( string s )
        //    {
        //        if ( !string.IsNullOrEmpty( s ) )
        //        {
        //            WordTrieNode_v1 _this_next;
        //            //var ch = _UPPER_INVARIANT_MAP[ s[ 0 ] ];
        //            if ( !_CurrentLevel.TryGetValue( _UPPER_INVARIANT_MAP[ s[ 0 ] ], out _this_next ) )
        //            {
        //                return (null);
        //            }
        //            return (_this_next.TryGetValue( s.Substring( 1 ) ));
        //        }
        //        else
        //        {
        //            return (_TValue.Value);
        //        }
        //    }
        //    public T? TryGetValue( char* ptr, char* endPtr )
        //    {
        //        if ( ptr != endPtr )
        //        {
        //            WordTrieNode_v1 _this_next;
        //            //var ch = _UPPER_INVARIANT_MAP[ *ptr ];
        //            if ( !_CurrentLevel.TryGetValue( _UPPER_INVARIANT_MAP[ *ptr ], out _this_next ) )
        //            {
        //                return (null);
        //            }
        //            return (_this_next.TryGetValue( ptr + 1, endPtr ));
        //        }
        //        else
        //        {
        //            return (_TValue.Value);
        //        }
        //    }
        //    public T? TryGetValueUnwrapRecursion( char* ptr, char* endPtr )
        //    {
        //        for ( WordTrieNode_v1 _this = this, _next_this; ; )
        //        {
        //            if ( ptr == endPtr )
        //            {
        //                return (_this._TValue.Value);
        //            }

        //            if ( !_this._CurrentLevel.TryGetValue( _UPPER_INVARIANT_MAP[ *ptr ], out _next_this ) )
        //            {
        //                return (null);
        //            }
        //            _this = _next_this;
        //            ptr++;
        //        }
        //    }*/

        //    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        //    public bool TryGetValueUnwrapRecursion( char* ptr, char* endPtr, ref T value )
        //    {
        //        for ( WordTrieNode_v1 _this = this, _next_this; ; )
        //        {
        //            if ( ptr == endPtr )
        //            {
        //                value = _this._TValue.Value;
        //                return (true);
        //            }

        //            if ( !_this._CurrentLevel.TryGetValue( _UPPER_INVARIANT_MAP[ *ptr ], out _next_this ) )
        //            {
        //                return (false);
        //            }
        //            _this = _next_this;
        //            ptr++;
        //        }
        //    }

        //    private IEnumerable< Tuple< string, T? > > GetAllInternal( Tuple< string, T? > t )
        //    {
        //        if ( _TValue.HasValue )
        //        {
        //            yield return (Tuple.Create( t.Item1, _TValue ));
        //        }

        //        foreach ( var p in _CurrentLevel )
        //        {
        //            var nt = new Tuple< string, T? >( t.Item1 + p.Key, null );

        //            foreach ( var nnt in p.Value.GetAllInternal( nt ) )
        //            {
        //                yield return (nnt);
        //            }
        //        }
        //    }
        //    public IEnumerable< Tuple< string, T > > GetAll()
        //    {
        //        foreach ( var t in GetAllInternal( new Tuple< string, T? >( string.Empty, null ) ) )
        //        {
        //            if ( t.Item2.HasValue )
        //            {
        //                yield return (Tuple.Create( t.Item1, t.Item2.Value ));
        //            }
        //        }
        //    }

        //    public int MaxCharsOnLevel()
        //    {
        //        return (Math.Max( _CurrentLevel.Count, _CurrentLevel.Any() ? _CurrentLevel.Values.Max( nl => nl.MaxCharsOnLevel() ) : 0 ));
        //    }
        //    #if DEBUG
        //    public override string ToString()
        //    {
        //        var sb = new StringBuilder();
        //        foreach ( var t in GetAll() )
        //        {
        //            sb.Append( '\'' ).Append( t.Item1 ).Append( "' => " ).Append( t.Item2 ).Append( "\r\n" );
        //        }
        //        return (sb.ToString());
        //    }
        //    #endif
        //    public static WordTrieNode_v1 Create()
        //    {
        //        var root = new WordTrieNode_v1();

        //        var seq = Enum.GetValues( typeof(T) )
        //                      .Cast< T >()
        //                      .Select( v => new { EnumValue = v, TextValue = v.ToString() } );
        //        foreach ( var a in seq )
        //        {
        //            root.Add( a.TextValue, a.EnumValue );
        //        }
        //        return (root);
        //    }
        //}
        #endregion

        /// <summary>
        /// 
        /// </summary>
	    private sealed class CharKeyedSet< X >
	    {
            /// <summary>
            /// 
            /// </summary>
		    internal struct Slot
		    {
			    internal int  HashCode;			    
			    internal int  Next;
                internal X    Value;
                internal char Key;
		    }

            private const int DEFAULT_CAPACITY = 20;

		    private int[]  _Buckets;
		    private Slot[] _Slots;
		    private int    _Count;
		    private int    _FreeList;

            internal Slot[] Slots { [M(O.AggressiveInlining)] get  => _Slots; }
            public int Count { [M(O.AggressiveInlining)] get => _Count; }
            
            public CharKeyedSet( int? capacity = null )
		    {
			    _Buckets  = new int [ capacity.GetValueOrDefault( DEFAULT_CAPACITY ) ];
			    _Slots    = new Slot[ capacity.GetValueOrDefault( DEFAULT_CAPACITY ) ];
			    _FreeList = -1;
		    }

            public bool Add( char key, X value )
		    {
                #region [.find exists.]
                var hash = InternalGetHashCode( key );
                for ( int i = _Buckets[ hash % _Buckets.Length ] - 1; 0 <= i; /*i = _Slots[ i ].next*/ )
                {
                    ref readonly var slot = ref _Slots[ i ];
                    if ( /*(slot.hashCode == hash) &&*/ (slot.Key == key) )
                    {
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
                    Key      = key,
                    Value    = value,
                    Next     = _Buckets[ n2 ] - 1,
                };
                _Buckets[ n2 ] = n1 + 1;

                return (true);
                #endregion
            }
            public bool Contains( char key )
            {
                var hash = InternalGetHashCode( key );
                for ( int i = _Buckets[ hash % _Buckets.Length ] - 1; 0 <= i; /*i = _Slots[ i ].next*/ )
                {
                    ref readonly var slot = ref _Slots[ i ];
                    if ( /*(slot.hashCode == hash) &&*/ (slot.Key == key) )
                    {
                        return (true);
                    }
                    i = slot.Next;
                }
                return (false);
            }
            public bool TryGetValue( char key, ref X value )
            {
                var hash = InternalGetHashCode( key );
                for ( int i = _Buckets[ hash % _Buckets.Length ] - 1; 0 <= i; /*i = _Slots[ i ].next*/ )
                {
                    ref readonly var slot = ref _Slots[ i ];
                    if ( /*(slot.hashCode == hash) &&*/ (slot.Key == key) )
                    {
                        value = slot.Value;
                        return (true);
                    }
                    i = slot.Next;
                }
                return (false);
            }

		    private void Resize()
		    {
                int new_size = checked( _Count * 2 + 1 );
                var buckets = new int [ new_size ];
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

            [M(O.AggressiveInlining)] private static int InternalGetHashCode( char key ) => (key.GetHashCode() & 0x7fffffff);
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class WordTrieNode_v2 //: IWordTrieNode
        {
            private static char* _UPPER_INVARIANT_MAP;
            static WordTrieNode_v2() => _UPPER_INVARIANT_MAP = xlat_Unsafe.Inst._UPPER_INVARIANT_MAP;

            private CharKeyedSet< WordTrieNode_v2 > _CurrentLevel;
            private T? _TValue;
            private WordTrieNode_v2 _this_next;

            private WordTrieNode_v2() => _CurrentLevel = new CharKeyedSet< WordTrieNode_v2 >( 20 );

            public void Add( string s, T t )
            {
                if ( !s.IsNullOrEmpty() )
                {                    
                    var ch = _UPPER_INVARIANT_MAP[ s[ 0 ] ];
                    if ( !_CurrentLevel.TryGetValue( ch, ref _this_next ) )
                    {
                        _this_next = new WordTrieNode_v2();
                        _CurrentLevel.Add( ch, _this_next );
                    }
                    _this_next.Add( s.Substring( 1 ), t );
                }
                else
                {
                    _TValue = t;
                }
            }
            /*public T? TryGetValue( string s )
            {
                if ( !string.IsNullOrEmpty( s ) )
                {
                    //var ch = _UPPER_INVARIANT_MAP[ s[ 0 ] ];
                    if ( !_CurrentLevel.TryGetValue( _UPPER_INVARIANT_MAP[ s[ 0 ] ], ref _this_next ) )
                    {
                        Debug.Assert( !_TValue.HasValue, "_TValue.HasValue" );
                        return (_TValue); //---return (null);
                    }
                    return (_this_next.TryGetValue( s.Substring( 1 ) ));
                }
                else
                {
                    Debug.Assert( _TValue.HasValue, "!_TValue.HasValue" );
                    return (_TValue); //---return (_TValue.Value);
                }
            }
            public T? TryGetValue( char* ptr, char* endPtr )
            {
                if ( ptr != endPtr )
                {
                    //var ch = _UPPER_INVARIANT_MAP[ *ptr ];
                    if ( !_CurrentLevel.TryGetValue( _UPPER_INVARIANT_MAP[ *ptr ], ref _this_next ) )
                    {
                        Debug.Assert( !_TValue.HasValue, "_TValue.HasValue" );
                        return (_TValue); //---return (null);
                    }
                    return (_this_next.TryGetValue( ptr + 1, endPtr ));
                }
                else
                {
                    Debug.Assert( _TValue.HasValue, "!_TValue.HasValue" );
                    return (_TValue); //---return (_TValue.Value);
                }
            }
            public T? TryGetValueUnwrapRecursion( char* ptr, char* endPtr )
            {
                for ( var _this = this; ; )
                {
                    if ( ptr == endPtr )
                    {
                        Debug.Assert( _this._TValue.HasValue, "!_this._TValue.HasValue" );
                        return (_this._TValue); //---return (_this._TValue.Value);
                    }

                    if ( !_this._CurrentLevel.TryGetValue( _UPPER_INVARIANT_MAP[ *ptr ], ref _this_next ) )
                    {
                        Debug.Assert( !_this._TValue.HasValue, "_this._TValue.HasValue" );
                        return (_this._TValue); //---return (null);
                    }
                    _this = _this_next;
                    ptr++;
                }
            }*/

            public bool TryGetValueUnwrapRecursion( char* ptr, char* endPtr, ref T value )
            {
                for ( var _this = this; ; )
                {
                    if ( ptr == endPtr )
                    {
                        value = _this._TValue.Value;
                        return (true);
                    }

                    if ( !_this._CurrentLevel.TryGetValue( _UPPER_INVARIANT_MAP[ *ptr ], ref _this_next ) )
                    {
                        return (false);
                    }
                    _this = _this_next;
                    ptr++;
                }
            }

            private IEnumerable< (string, T?) > GetAllInternal( (string, T?) t )
            {
                if ( _TValue.HasValue )
                {
                    yield return (t.Item1, _TValue);
                }

                for ( var i = 0; i < _CurrentLevel.Count; i++ )
                {
                    var slot = _CurrentLevel.Slots[ i ];
                    var nt   = (t.Item1 + slot.Key, (T?) null);

                    foreach ( var nnt in slot.Value.GetAllInternal( nt ) )
                    {
                        yield return (nnt);
                    }
                }
            }
            public IEnumerable< (string, T) > GetAll()
            {
                foreach ( var t in GetAllInternal( (string.Empty, null) ) )
                {
                    if ( t.Item2.HasValue )
                    {
                        yield return (t.Item1, t.Item2.Value);
                    }
                }
            }

            public int MaxCharsOnLevel() => Math.Max( _CurrentLevel.Count, (0 < _CurrentLevel.Count) ? _CurrentLevel.Slots.Where( sl => sl.Value != null ).Max( sl => sl.Value.MaxCharsOnLevel() ) : 0 );
#if DEBUG
            public override string ToString()
            {
                var sb = new StringBuilder();
                foreach ( var t in GetAll() )
                {
                    sb.Append( '\'' ).Append( t.Item1 ).Append( "' => " ).Append( t.Item2 ).Append( "\r\n" );
                }
                return (sb.ToString());
            }
#endif
            public static WordTrieNode_v2 Create()
            {
                var root = new WordTrieNode_v2();

                var seq = Enum.GetValues( typeof(T) )
                              .Cast< T >()
                              .Select( v => new { EnumValue = v, TextValue = v.ToString() } );
                foreach ( var a in seq )
                {
                    root.Add( a.TextValue, a.EnumValue );
                }
                return (root);
            }
        }


        private WordTrieNode_v2 _FirstWordTrieNode;
        public EnumParser() => _FirstWordTrieNode = WordTrieNode_v2.Create();

        /*public bool TryParse( string s, ref T t )
        {
            var tr = _FirstWordTrieNode.TryGetValue( s );
            if ( tr.HasValue )
            {
                t = tr.Value;
                return (true);
            }
            return (false);
        }
        public bool TryParse( char* ptr, int len, ref T t )
        {
            var tr = _FirstWordTrieNode.TryGetValue( ptr, ptr + len );
            if ( tr.HasValue )
            {
                t = tr.Value;
                return (true);
            }
            return (false);
        }
        public bool TryParseUnwrapRecursion( char* ptr, int len, ref T t )
        {
            var tr = _FirstWordTrieNode.TryGetValueUnwrapRecursion( ptr, ptr + len );
            if ( tr.HasValue )
            {
                t = tr.Value;
                return (true);
            }
            return (false);
        }*/
        [M(O.AggressiveInlining)] public bool TryParse( char* ptr, int len, ref T t ) => _FirstWordTrieNode.TryGetValueUnwrapRecursion( ptr, ptr + len, ref t );

        public IEnumerable< (string, T) > GetAll()
        {
            foreach ( var t in _FirstWordTrieNode.GetAll() )
            {
                yield return (t);
            }
        }
        public override string ToString() => _FirstWordTrieNode.ToString();
    }
}