using System;
using System.Collections.Generic;

namespace lingvo.morphology
{
    /// <summary>
    /// 
    /// </summary>
	internal struct SortedListCharKey< TValue > where TValue : class
	{
        private const int MAX_CAPACITY_THRESHOLD = 0x7FFFFFFF /*int.MaxValue*/ - 0x400 * 0x400 /*1MB*/; /* => 2146435071 == 0x7fefffff*/

        /// <summary>
        /// 
        /// </summary>
        internal struct Tuple
        {            
            public TValue Value;
            public char   Key;
        }

        private static readonly Tuple[] EMPTY_ARRAY = new Tuple[ 0 ];

        private Tuple[] _Array;
		private ushort  _Size;

		public int Capacity
		{
			get
			{
                return _Array.Length;
			}
			private set
			{
                if ( value != _Array.Length )
				{
                    if ( 0 < value )
					{
                        Tuple[] destinationArray = new Tuple[ value ];
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

		public void InitArrayAsEmpty() => _Array = EMPTY_ARRAY;

		public TValue this[ char key ]
		{
			get
			{
                int n = IndexOfKey( key );
                if ( n >= 0 )
                {
                    return _Array[ n ].Value;
                }
                throw (new KeyNotFoundException());
                //return default;
			}
			set
			{
                int n = InternalBinarySearch( /*_Array,0, _Size,*/ key );
                if ( n >= 0 )
				{
                    _Array[ n ].Value = value;
					return;
				}
                Insert( ~n, key, value );
			}
		}

		public void Add( char key, TValue value )
		{
            int n = InternalBinarySearch( /*_Array,0, _Size,*/ key );
            if ( n >= 0 )
			{
                throw (new ArgumentException( n.ToString(), "n" ));
			}
            Insert( ~n, key, value );
		}
		public void Clear()
		{
            System.Array.Clear( _Array, 0, _Size );
			_Size = 0;
		}
		public bool ContainsKey( char key ) => (IndexOfKey( key ) >= 0);

		private void EnsureCapacity( int min )
		{
            int n;
            switch ( _Array.Length )
            {
                case 0:  n = 1; break;
                case 1:  n = 16; break;
                default: n = _Array.Length * 2; break;
            }
            //int n = (_Array.Length == 0) ? DEFAULT_CAPACITY : (_Array.Length * 2);
            if ( n > MAX_CAPACITY_THRESHOLD )
            {
                n = MAX_CAPACITY_THRESHOLD;
            }
            if ( n < min )
            {
                n = min;
            }
			Capacity = n;
		}

        public int IndexOfKey( char key )
		{
            int n = InternalBinarySearch( /*_Array,0, _Size,*/ key ); //Array.BinarySearch< char >( _Array, 0, _Size, key ); //
            if ( n < 0 )
            {
                return (-1);
            }
			return (n);
		}
		private void Insert( int index, char key, TValue value )
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
		public bool TryGetValue( char key, out TValue value )
		{
            int n = IndexOfKey( key );
            if ( n >= 0 )
			{
                value = _Array[ n ].Value;
				return (true);
			}
			value = default(TValue);
			return (false);
		}
		public void RemoveAt( int index )
		{
            if ( index < 0 || index >= _Size )
			{
				throw (new ArgumentOutOfRangeException("index"));
			}
			_Size--;
            if ( index < _Size )
			{
                System.Array.Copy( _Array, index + 1, _Array, index, _Size - index );
			}
			_Array[ _Size ] = default;
		}
		public bool Remove( char key )
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

        private int InternalBinarySearch( /*tuple_t[] array, int index, int length,*/ char value )
        {
            int i  = /*index*/0;
            int n1 = /*index +*/ _Size - 1;
            while ( i <= n1 )
            {
                int n2 = i + (n1 - i >> 1);
                int n3 = _Array[ n2 ].Key - value;
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
	}

}
