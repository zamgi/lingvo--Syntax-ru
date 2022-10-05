using System;
using System.Collections.Generic;

using M = System.Runtime.CompilerServices.MethodImplAttribute;
using O = System.Runtime.CompilerServices.MethodImplOptions;

namespace lingvo.morphology
{
    /// <summary>
    /// 
    /// </summary>
	internal struct SortedListCharKey< T > where T : class
	{
        private const int MAX_CAPACITY_THRESHOLD = 0x7FFFFFFF /*int.MaxValue*/ - 0x400 * 0x400 /*1MB*/; /* => 2146435071 == 0x7fefffff*/

        /// <summary>
        /// 
        /// </summary>
        internal struct Tuple
        {            
            public T    Value;
            public char Key;
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

		[M(O.AggressiveInlining)] public void InitArrayAsEmpty() => _Tuples = EMPTY_ARRAY;

		public T this[ char key ]
		{
			get
			{
                var i = IndexOfKey( key );
                if ( 0 <= i )
                {
                    return _Tuples[ i ].Value;
                }

                throw (new KeyNotFoundException());
                //return default;
			}
			set
			{
                var i = InternalBinarySearch( /*_Array,0, _Size,*/ key );
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

		public void Add( char key, T value )
		{
            var i = InternalBinarySearch( /*_Array,0, _Size,*/ key );
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
		public bool ContainsKey( char key ) => (IndexOfKey( key ) >= 0);

		private void EnsureCapacity( int min )
		{
            int n;
            switch ( _Tuples.Length )
            {
                case 0:  n = 1; break;
                case 1:  n = 16; break;
                default: n = _Tuples.Length * 2; break;
            }
            //int n = (_Array.Length == 0) ? DEFAULT_CAPACITY : (_Array.Length * 2);
            if ( MAX_CAPACITY_THRESHOLD < n ) n = MAX_CAPACITY_THRESHOLD;            
            if ( n < min ) n = min;
			Capacity = n;
		}

        public int IndexOfKey( char key )
		{
            var i = InternalBinarySearch( /*_Array,0, _Size,*/ key ); //Array.BinarySearch< char >( _Array, 0, _Size, key ); //
			return ((i < 0) ? -1 : i);
		}
		private void Insert( int index, char key, T value )
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
		public bool TryGetValue( char key, out T value )
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
            if ( index < 0 || index >= _Count ) throw (new ArgumentOutOfRangeException( nameof(index) ));
			
			_Count--;
            if ( index < _Count )
			{
                Array.Copy( _Tuples, index + 1, _Tuples, index, _Count - index );
			}
			_Tuples[ _Count ] = default;
		}
		public bool Remove( char key )
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
            int n = (int) ((double) _Tuples.Length * 0.9);
            if ( _Count < n )
            {
                Capacity = _Count;
            }
		}
        public void Trim() => Capacity = _Count;

        [M(O.AggressiveInlining)] private int InternalBinarySearch( /*tuple_t[] array, int index, int length,*/ char value )
        {
            int i  = /*index*/0;
            int n1 = /*index +*/ _Count - 1;
            while ( i <= n1 )
            {
                int n2 = i + (n1 - i >> 1);
                int n3 = _Tuples[ n2 ].Key - value;
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
