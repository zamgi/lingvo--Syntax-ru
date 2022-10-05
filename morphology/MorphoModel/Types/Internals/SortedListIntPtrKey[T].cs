using System;
using System.Collections.Generic;

using lingvo.core;
using M = System.Runtime.CompilerServices.MethodImplAttribute;
using O = System.Runtime.CompilerServices.MethodImplOptions;

namespace lingvo.morphology
{
    /// <summary>
    /// 
    /// </summary>
	internal struct SortedListIntPtrKey< T > where T : class
	{
        private const int MAX_CAPACITY_THRESHOLD = 0x7FFFFFFF /*int.MaxValue*/ - 0x400 * 0x400 /*1MB*/; /* => 2146435071 == 0x7fefffff*/

        /// <summary>
        /// 
        /// </summary>
        internal struct Tuple
        {            
            public T      Value;
            public IntPtr Key;
#if DEBUG
            public override string ToString()
            {
                string s;
                if ( Key != IntPtr.Zero )
                {
                    s = StringsHelper.ToString( Key );
                    if ( s.IsNullOrEmpty() )
                        s = "_";
                    s = '\'' + s + '\'';
                }
                else
                {
                    s = "NULL";
                }
                var v = (Value is ICollection< T > col) ? col.Count.ToString() : Value.ToString();
                return (s + ", " + v);
            }
#endif
        }

        private static readonly Tuple[] EMPTY_ARRAY = new Tuple[ 0 ];

        private Tuple[] _Tuples;
		private ushort  _Count;

		public int Count { [M(O.AggressiveInlining)] get => _Count; }
        public Tuple[] Tuples { [M(O.AggressiveInlining)] get => _Tuples; }
		public int Capacity
		{
			get => _Tuples.Length;
			private set
			{
                if ( value != _Tuples.Length )
				{
                    if ( 0 < value )
					{
                        var destinationArray = new Tuple[ value ];
                        if ( 0 < _Count )
						{
                            Array.Copy( _Tuples, 0, destinationArray, 0, _Count );
						}
                        _Tuples = destinationArray;
					}
                    else
                    {
                        _Tuples = EMPTY_ARRAY;
                    }
				}
			}
		}

		public T this[ IntPtr key ]
		{
			get
			{
                var i = IndexOfKey( key );
                if ( 0 <= i )
                {
                    return _Tuples[ i ].Value;
                }

                throw (new KeyNotFoundException());
                //return default(TValue);
			}
			set
			{
                var i = InternalBinarySearch( /*_Array, 0, _Size,*/ key ); //Array.BinarySearch< IntPtr >( _Keys, 0, _Size, key ); //
                if ( 0 <= i )
                {
                    _Tuples[ i ].Value = value;
                }
                else
                {
                    Insert( ~i, key, value );
                }                
			}
		}
        public void Add( IntPtr key, T value )
		{
            var i = InternalBinarySearch( /*_Array, 0, _Size,*/ key ); //Array.BinarySearch< IntPtr >( _Keys, 0, _Size, key ); //
            if ( 0 <= i )
			{
                throw (new ArgumentException( i.ToString(), "n" ));
			}
            Insert( ~i, key, value );
		}
		public void Clear()
		{
            Array.Clear( _Tuples, 0, _Count );
			_Count = 0;
		}
		public bool ContainsKey( IntPtr key ) => (IndexOfKey( key ) >= 0);

		private void EnsureCapacity( int min )
		{
            int n = _Tuples.Length * 3;
            //int n = (_Array.Length == 0) ? 4 : (_Array.Length * 2);            
            if ( MAX_CAPACITY_THRESHOLD < n )
            {
                n = MAX_CAPACITY_THRESHOLD;
            }
            if ( n < min )
            {
                n = min;
            }
			Capacity = n;
		}

        public int IndexOfKey( IntPtr key )
		{
            var i = InternalBinarySearch( /*_Array, 0, _Size,*/ key ); //Array.BinarySearch< IntPtr >( _Keys, 0, _Size, key ); //
            if ( i < 0 )
            {
                return (-1);
            }
			return (i);
		}
        public int IndexOfKeyCore( IntPtr key ) => InternalBinarySearch( /*_Array, 0, _Size,*/ key );
	 	public /*private*/ void Insert( int index, IntPtr key, T value )
		{
            if ( _Count == _Tuples.Length )
            {
                EnsureCapacity( _Count + 1 );
            }
            if ( index < _Count )
            {
                Array.Copy( _Tuples, index, _Tuples, index + 1, _Count - index );
            }
            _Tuples[ index ] = new Tuple() { Key = key, Value = value };
            _Count++;
		}
        public T GetValue( int index ) => _Tuples[ index ].Value;
        public void SetValue( int index, T value ) => _Tuples[ index ].Value = value;
		[M(O.AggressiveInlining)] public bool TryGetValue( IntPtr key, out T value )
		{
            var i = IndexOfKey( key );
            if ( 0 <= i )
			{
                value = _Tuples[ i ].Value;
				return (true);
			}
			value = default;
			return (false);
		}
		public void RemoveAt( int index )
		{
            if ( index < 0 || _Count <= index ) throw (new ArgumentOutOfRangeException( nameof(index) ));
            
			_Count--;
            if ( index < _Count )
            {
                Array.Copy( _Tuples, index + 1, _Tuples, index, _Count - index );
            }
            _Tuples[ _Count ] = default;
		}
		public bool Remove( IntPtr key )
		{
            var i = IndexOfKey( key );
            if ( 0 <= i )
            {
                RemoveAt( i );
            }
            return (0 <= i);
		}

		public void TrimExcess()
		{
            var n = (int) ((double) _Tuples.Length * 0.9);
            if ( _Count < n )
            {
                Capacity = _Count;
            }
		}
        public void Trim() => Capacity = _Count;

        public void SetArray( Tuple[] array )
        {
            _Tuples = array;
            _Count  = (ushort) array.Length;
        }

        [M(O.AggressiveInlining)] private int InternalBinarySearch( /*tuple_t[] array, int index, int length,*/ IntPtr value )
        {
            int i  = /*index*/ 0;
            int n1 = /*index +*/ _Count - 1;
            while ( i <= n1 )
            {
                int n2 = i + ((n1 - i) >> 1);
                int n3 = CompareRoutine( _Tuples[ n2 ].Key, value );
                if ( n3 == 0 )
                {
                    return (n2);
                }
                if ( n3 < 0 )
                {
                    i = n2 + 1;
                }
                else
                {
                    n1 = n2 - 1;
                }
            }
            return (~i);
        }
        [M(O.AggressiveInlining)] unsafe internal static int CompareRoutine( IntPtr x, IntPtr y )
        {
            if ( x == y )
                return (0);

            for ( char* x_ptr = (char*) x, 
                        y_ptr = (char*) y; ; x_ptr++, y_ptr++ )
            {
                int d = *x_ptr - *y_ptr;
                if ( d != 0 )
                    return (d);
                if ( *x_ptr == '\0' )
                    return (0);
            }
        }
	}
}
