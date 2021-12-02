using System;
using System.Collections.Generic;

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
            public T Value;
            public IntPtr Key;
#if DEBUG
            public override string ToString()
            {
                var s = default(string);
                if ( Key != IntPtr.Zero )
                {
                    s = lingvo.core.StringsHelper.ToString( Key );
                    if ( string.IsNullOrEmpty( s ) )
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

        private Tuple[] _Array;
		private ushort  _Size;

		public int Capacity
		{
			get => _Array.Length;
			private set
			{
                if ( value != _Array.Length )
				{
                    if ( 0 < value )
					{
                        var destinationArray = new Tuple[ value ];
                        if ( 0 < _Size )
						{
                            System.Array.Copy( _Array, 0, destinationArray, 0, _Size );
						}
                        _Array = destinationArray;
					}
                    else
                    {
                        _Array = EMPTY_ARRAY;
                    }
				}
			}
		}

		public int Count => _Size;
        public Tuple[] Array => _Array;

		public T this[ IntPtr key ]
		{
			get
			{
                int n = IndexOfKey( key );
                if ( n >= 0 )
                {
                    return _Array[ n ].Value;
                }
                throw (new KeyNotFoundException());
                //return default(TValue);
			}
			set
			{
                int n = InternalBinarySearch( /*_Array, 0, _Size,*/ key ); //Array.BinarySearch< IntPtr >( _Keys, 0, _Size, key ); //
                if ( n >= 0 )
                {
                    _Array[ n ].Value = value;
                    return;
                }
                Insert( ~n, key, value );
			}
		}

        public void Add( IntPtr key, T value )
		{
            int n = InternalBinarySearch( /*_Array, 0, _Size,*/ key ); //Array.BinarySearch< IntPtr >( _Keys, 0, _Size, key ); //
            if ( n >= 0 )
			{
                throw (new ArgumentException(n.ToString(), "n"));
			}
            Insert( ~n, key, value );
		}

		public void Clear()
		{
            System.Array.Clear( _Array, 0, _Size );
			_Size = 0;
		}
		public bool ContainsKey( IntPtr key ) => (IndexOfKey( key ) >= 0);

		private void EnsureCapacity( int min )
		{
            int n = _Array.Length * 3;
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
            int n = InternalBinarySearch( /*_Array, 0, _Size,*/ key ); //Array.BinarySearch< IntPtr >( _Keys, 0, _Size, key ); //
            if ( n < 0 )
            {
                return (-1);
            }
			return (n);
		}
        public int IndexOfKeyCore( IntPtr key ) => InternalBinarySearch( /*_Array, 0, _Size,*/ key );
	 	public /*private*/ void Insert( int index, IntPtr key, T value )
		{
            if ( _Size == _Array.Length )
            {
                EnsureCapacity( _Size + 1 );
            }
            if ( index < _Size )
            {
                System.Array.Copy( _Array, index, _Array, index + 1, _Size - index );
            }
            _Array[ index ] = new Tuple() { Key = key, Value = value };
            _Size++;
		}
        public T GetValue( int index ) => _Array[ index ].Value;
        public void SetValue( int index, T value ) => _Array[ index ].Value = value;
		public bool TryGetValue( IntPtr key, out T value )
		{
            int n = IndexOfKey( key );
            if ( n >= 0 )
			{
                value = _Array[ n ].Value;
				return (true);
			}
			value = default;
			return (false);
		}
		public void RemoveAt( int index )
		{
            if ( index < 0 || _Size <= index )
            {
                throw (new ArgumentOutOfRangeException( "index" ));
            }
			_Size--;
            if ( index < _Size )
            {
                System.Array.Copy( _Array, index + 1, _Array, index, _Size - index );
            }
            _Array[ _Size ] = default;
		}
		public bool Remove( IntPtr key )
		{
            int n = IndexOfKey( key );
            if ( n >= 0 )
            {
                RemoveAt( n );
            }
            return (n >= 0);
		}

		public void TrimExcess()
		{
            int n = (int) ((double) _Array.Length * 0.9);
            if ( _Size < n )
            {
                Capacity = _Size;
            }
		}
        public void Trim() => Capacity = _Size;

        public void SetArray( Tuple[] array )
        {
            _Array = array;
            _Size  = (ushort) array.Length;
        }

        private int InternalBinarySearch( /*tuple_t[] array, int index, int length,*/ IntPtr value )
        {
            int i  = /*index*/ 0;
            int n1 = /*index +*/ _Size - 1;
            while ( i <= n1 )
            {
                int n2 = i + ((n1 - i) >> 1);
                int n3 = CompareRoutine( _Array[ n2 ].Key, value );
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
        unsafe internal static int CompareRoutine( IntPtr x, IntPtr y )
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
