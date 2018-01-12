using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;

namespace lingvo.morphology
{
    /*/// <summary>
    /// 
    /// </summary>
	internal sealed class __sorted_list_key_char__< TValue > //: IDictionary< char, TValue >, ICollection< KeyValuePair< char, TValue > >, IEnumerable< KeyValuePair< char, TValue > >
        where TValue : class
	{
		private static readonly char[]   EMPTY_KEYS   = new char  [ 0 ];
        private static readonly TValue[] EMPTY_VALUES = new TValue[ 0 ];

		private char[]   _Keys;
		private TValue[] _Values;
		private ushort   _Size;

		/// <summary>Gets or sets the number of elements that the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> can contain.</summary>
		/// <returns>The number of elements that the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> can contain.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <see cref="P:System.Collections.Generic.SortedListCharKey`2.Capacity" /> is set to a value that is less than <see cref="P:System.Collections.Generic.SortedListCharKey`2.Count" />.</exception>
		/// <exception cref="T:System.OutOfMemoryException">There is not enough memory available on the system.</exception>
		public int Capacity
		{
			get
			{
				return this._Keys.Length;
			}
			private set
			{
                if ( value != this._Keys.Length )
				{
					/*if (value < this._Size)
					{
						throw new ArgumentOutOfRangeException("value");
					}* /
                    if ( value > 0 )
					{
						char[]   destinationArray  = new char  [ value ];
						TValue[] destinationArray2 = new TValue[ value ];
                        if ( this._Size > 0 )
						{
                            Array.Copy( this._Keys  , 0, destinationArray , 0, this._Size );
                            Array.Copy( this._Values, 0, destinationArray2, 0, this._Size );
						}
						this._Keys   = destinationArray;
						this._Values = destinationArray2;
					}
                    else
                    {
					    this._Keys   = EMPTY_KEYS;
					    this._Values = EMPTY_VALUES;
                    }
				}
			}
		}

		/// <summary>Gets the number of key/value pairs contained in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <returns>The number of key/value pairs contained in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</returns>
		public int Count
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get
			{
				return this._Size;
			}
		}

		/// <summary>Gets a collection containing the keys in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <returns>A <see cref="T:System.Collections.Generic.IList`1" /> containing the keys in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</returns>
		public char[] Keys
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get { return (_Keys); }
		}

		/// <summary>Gets a collection containing the values in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <returns>A <see cref="T:System.Collections.Generic.IList`1" /> containing the values in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</returns>
		public TValue[] Values
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get { return (_Values); }
		}

		/// <summary>Gets or sets the value associated with the specified key.</summary>
		/// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a <see cref="T:System.Collections.Generic.KeyNotFoundException" /> and a set operation creates a new element using the specified key.</returns>
		/// <param name="key">The key whose value to get or set.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
		/// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key" /> does not exist in the collection.</exception>
		public TValue this[ char key ]
		{
			get
			{
				int n = this.IndexOfKey(key);
				if (n >= 0)
				{
					return this._Values[n];
				}
				throw (new KeyNotFoundException());
				//return default(TValue);
			}
			set
			{
				int n = Array.BinarySearch< char >( this._Keys, 0, this._Size, key ); //InternalBinarySearch( this.keys, 0, this._size, key );
				if (n >= 0)
				{
					this._Values[n] = value;
					return;
				}
				this.Insert(~n, key, value);
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> class that is empty, has the default initial capacity, and uses the default <see cref="T:System.Collections.Generic.IComparer`1" />.</summary>
		public __sorted_list_key_char__()
		{
			this._Keys   = EMPTY_KEYS;
			this._Values = EMPTY_VALUES;
			this._Size   = 0;
		}
		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> class that is empty, has the specified initial capacity, and uses the default <see cref="T:System.Collections.Generic.IComparer`1" />.</summary>
		/// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> can contain.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="capacity" /> is less than zero.</exception>
		public __sorted_list_key_char__( int capacity )
		{
			if ( capacity < 0 )
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			this._Keys   = new char  [ capacity ];
			this._Values = new TValue[ capacity ];
		}
		/// <summary>Adds an element with the specified key and value into the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="value">The value of the element to add. The value can be null for reference types.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
		/// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</exception>
		public void Add( char key, TValue value )
		{
            int n = Array.BinarySearch< char >( this._Keys, 0, this._Size, key );  //InternalBinarySearch( this.keys, 0, this._size, key );
			if (n >= 0)
			{
				//ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_AddingDuplicate);
                throw (new ArgumentException(n.ToString(), "n"));
			}
			this.Insert(~n, key, value);
		}

		/// <summary>Removes all elements from the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		public void Clear()
		{
			Array.Clear(this._Keys, 0, this._Size);
			Array.Clear(this._Values, 0, this._Size);
			this._Size = 0;
		}
		/// <summary>Determines whether the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> contains a specific key.</summary>
		/// <returns>true if the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> contains an element with the specified key; otherwise, false.</returns>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
		public bool ContainsKey( char key )
		{
			return this.IndexOfKey(key) >= 0;
		}
		/// <summary>Determines whether the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> contains a specific value.</summary>
		/// <returns>true if the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> contains an element with the specified value; otherwise, false.</returns>
		/// <param name="value">The value to locate in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />. The value can be null for reference types.</param>
		public bool ContainsValue( TValue value )
		{
			return this.IndexOfValue(value) >= 0;
		}

		private void EnsureCapacity( int min )
		{
			int n = (this._Keys.Length == 0) ? 4 : (this._Keys.Length * 2);
			if (n > 2146435071)
			{
				n = 2146435071;
			}
			if (n < min)
			{
				n = min;
			}
			this.Capacity = n;
		}

		/// <summary>Searches for the specified key and returns the zero-based index within the entire <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <returns>The zero-based index of <paramref name="key" /> within the entire <see cref="T:System.Collections.Generic.SortedListCharKey`2" />, if found; otherwise, -1.</returns>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
        public int IndexOfKey( char key )
		{
            int n = Array.BinarySearch< char >( this._Keys, 0, this._Size, key ); //InternalBinarySearch( this.keys, 0, this._size, key );
			if (n < 0)
			{
				return -1;
			}
			return n;
		}
		/// <summary>Searches for the specified value and returns the zero-based index of the first occurrence within the entire <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <returns>The zero-based index of the first occurrence of <paramref name="value" /> within the entire <see cref="T:System.Collections.Generic.SortedListCharKey`2" />, if found; otherwise, -1.</returns>
		/// <param name="value">The value to locate in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.  The value can be null for reference types.</param>
		public int IndexOfValue( TValue value )
		{
			return Array.IndexOf<TValue>(this._Values, value, 0, this._Size);
		}
		private void Insert( int index, char key, TValue value )
		{
			if (this._Size == this._Keys.Length)
			{
				this.EnsureCapacity(this._Size + 1);
			}
			if (index < this._Size)
			{
				Array.Copy(this._Keys, index, this._Keys, index + 1, this._Size - index);
				Array.Copy(this._Values, index, this._Values, index + 1, this._Size - index);
			}
			this._Keys[index] = key;
			this._Values[index] = value;
			this._Size++;
		}
		/// <summary>Gets the value associated with the specified key.</summary>
		/// <returns>true if the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> contains an element with the specified key; otherwise, false.</returns>
		/// <param name="key">The key whose value to get.</param>
		/// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
		public bool TryGetValue( char key, out TValue value )
		{
			int n = this.IndexOfKey(key);
			if (n >= 0)
			{
				value = this._Values[n];
				return (true);
			}
			value = default(TValue);
			return (false);
		}
		/// <summary>Removes the element at the specified index of the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than zero.-or-<paramref name="index" /> is equal to or greater than <see cref="P:System.Collections.Generic.SortedListCharKey`2.Count" />.</exception>
		public void RemoveAt( int index )
		{
			if (index < 0 || index >= this._Size)
			{
				throw (new ArgumentOutOfRangeException("index"));
			}
			this._Size--;
			if (index < this._Size)
			{
				Array.Copy(this._Keys, index + 1, this._Keys, index, this._Size - index);
				Array.Copy(this._Values, index + 1, this._Values, index, this._Size - index);
			}
			this._Keys[this._Size] = default(char);
			this._Values[this._Size] = default(TValue);
		}
		/// <summary>Removes the element with the specified key from the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <returns>true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</returns>
		/// <param name="key">The key of the element to remove.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
		public bool Remove( char key )
		{
			int n = this.IndexOfKey(key);
			if (n >= 0)
			{
				this.RemoveAt(n);
			}
			return n >= 0;
		}

		/// <summary>Sets the capacity to the actual number of elements in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />, if that number is less than 90 percent of current capacity.</summary>
		public void TrimExcess()
		{
			int n = (int)((double)this._Keys.Length * 0.9);
			if (this._Size < n)
			{
				this.Capacity = this._Size;
			}
		}
        public void Trim()
        {
            this.Capacity = this._Size;
        }

        private static int InternalBinarySearch( char[] array, int index, int length, char value )
        {
            int i  = index;
            int n1 = index + length - 1;
            while ( i <= n1 )
            {
                int n2 = i + (n1 - i >> 1);
                int n3 = array[ n2 ] - value;
                if ( n3 == 0 )
                {
                    return n2;
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
            return ~i;
        }
	}
    */

    /*/// <summary>
    /// 
    /// </summary>
	internal sealed class SortedListCharKey< TValue > //: IDictionary< char, TValue >, ICollection< KeyValuePair< char, TValue > >, IEnumerable< KeyValuePair< char, TValue > >
        where TValue : class
	{
        /// <summary>
        /// 
        /// </summary>
        internal struct tuple_t
        {            
            public TValue Value;
            public char   Key;
        }

        private static readonly tuple_t[] EMPTY_ARRAY = new tuple_t[ 0 ];

        private tuple_t[] _Array;
		private ushort    _Size;

		/// <summary>Gets or sets the number of elements that the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> can contain.</summary>
		/// <returns>The number of elements that the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> can contain.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <see cref="P:System.Collections.Generic.SortedListCharKey`2.Capacity" /> is set to a value that is less than <see cref="P:System.Collections.Generic.SortedListCharKey`2.Count" />.</exception>
		/// <exception cref="T:System.OutOfMemoryException">There is not enough memory available on the system.</exception>
		public int Capacity
		{
			get
			{
                return this._Array.Length;
			}
			private set
			{
                if ( value != this._Array.Length )
				{
					/*if (value < this._Size)
					{
						throw new ArgumentOutOfRangeException("value");
					}* /
                    if ( 0 < value )
					{
                        tuple_t[] destinationArray = new tuple_t[ value ];
                        if ( 0 < this._Size )
						{
                            System.Array.Copy( this._Array, 0, destinationArray, 0, this._Size );
						}
                        this._Array = destinationArray;
					}
                    else
                    {
                        this._Array = EMPTY_ARRAY;
                    }
				}
			}
		}

		/// <summary>Gets the number of key/value pairs contained in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <returns>The number of key/value pairs contained in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</returns>
		public int Count
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get
			{
				return this._Size;
			}
		}

		/// <summary>Gets a collection containing the keys in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <returns>A <see cref="T:System.Collections.Generic.IList`1" /> containing the keys in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</returns>
        public tuple_t[] Array
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return (_Array); }
		}

		/* /// <summary>Gets a collection containing the values in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <returns>A <see cref="T:System.Collections.Generic.IList`1" /> containing the values in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</returns>
		public IEnumerable< TValue > Values
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return (_Array.Select( t => t.Value)); }
		}* /

		/// <summary>Gets or sets the value associated with the specified key.</summary>
		/// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a <see cref="T:System.Collections.Generic.KeyNotFoundException" /> and a set operation creates a new element using the specified key.</returns>
		/// <param name="key">The key whose value to get or set.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
		/// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key" /> does not exist in the collection.</exception>
		public TValue this[ char key ]
		{
			get
			{
                int n = this.IndexOfKey( key );
                if ( n >= 0 )
                {
                    return this._Array[ n ].Value;
                }
                throw (new KeyNotFoundException());
                //return default(TValue);
			}
			set
			{
                int n = InternalBinarySearch( this._Array, 0, this._Size, key );  //Array.BinarySearch< char >( this._Array, 0, this._Size, key );  //
                if ( n >= 0 )
				{
                    this._Array[ n ].Value = value;
					return;
				}
                this.Insert( ~n, key, value );
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> class that is empty, has the default initial capacity, and uses the default <see cref="T:System.Collections.Generic.IComparer`1" />.</summary>
        public SortedListCharKey()
		{
			this._Array = EMPTY_ARRAY;
		}
		/* /// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> class that is empty, has the specified initial capacity, and uses the default <see cref="T:System.Collections.Generic.IComparer`1" />.</summary>
		/// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> can contain.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="capacity" /> is less than zero.</exception>
		public SortedListCharKey( int capacity )
		{
			this._Array = new tuple_t[ capacity ];
		}* /
		/// <summary>Adds an element with the specified key and value into the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="value">The value of the element to add. The value can be null for reference types.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
		/// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</exception>
		public void Add( char key, TValue value )
		{
            int n = InternalBinarySearch( this._Array, 0, this._Size, key ); //Array.BinarySearch< char >( this._Array, 0, this._Size, key );  //
            if ( n >= 0 )
			{
				//ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_AddingDuplicate);
                throw (new ArgumentException(n.ToString(), "n"));
			}
            this.Insert( ~n, key, value );
		}

		/// <summary>Removes all elements from the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		public void Clear()
		{
            System.Array.Clear( this._Array, 0, this._Size );
			this._Size = 0;
		}
		/// <summary>Determines whether the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> contains a specific key.</summary>
		/// <returns>true if the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> contains an element with the specified key; otherwise, false.</returns>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
		public bool ContainsKey( char key )
		{
            return this.IndexOfKey( key ) >= 0;
		}

        private const int DEFAULT_CAPACITY = 1 /*4* /;

		private void EnsureCapacity( int min )
		{
            int n;
            switch ( this._Array.Length )
            {
                case 0:  n = 1; break;
                case 1:  n = 16; break;
                default: n = this._Array.Length * 2; break;
            }
            //int n = (this._Array.Length == 0) ? DEFAULT_CAPACITY : (this._Array.Length * 2);
			if (n > 2146435071)
			{
				n = 2146435071;
			}
			if (n < min)
			{
				n = min;
			}
			this.Capacity = n;
		}

		/// <summary>Searches for the specified key and returns the zero-based index within the entire <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <returns>The zero-based index of <paramref name="key" /> within the entire <see cref="T:System.Collections.Generic.SortedListCharKey`2" />, if found; otherwise, -1.</returns>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
        public int IndexOfKey( char key )
		{
            int n = InternalBinarySearch( this._Array, 0, this._Size, key ); //Array.BinarySearch< char >( this._Array, 0, this._Size, key ); //
			if (n < 0)
			{
				return -1;
			}
			return n;
		}
		private void Insert( int index, char key, TValue value )
		{
            if ( this._Size == this._Array.Length )
            {
                this.EnsureCapacity( this._Size + 1 );
            }
            if ( index < this._Size )
            {
                System.Array.Copy( this._Array, index, this._Array, index + 1, this._Size - index );
            }
            this._Array[ index ] = new tuple_t() { Key = key, Value = value };
			this._Size++;
		}
		/// <summary>Gets the value associated with the specified key.</summary>
		/// <returns>true if the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> contains an element with the specified key; otherwise, false.</returns>
		/// <param name="key">The key whose value to get.</param>
		/// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
		public bool TryGetValue( char key, out TValue value )
		{
            int n = this.IndexOfKey( key );
            if ( n >= 0 )
			{
                value = this._Array[ n ].Value;
				return (true);
			}
			value = default(TValue);
			return (false);
		}
		/// <summary>Removes the element at the specified index of the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than zero.-or-<paramref name="index" /> is equal to or greater than <see cref="P:System.Collections.Generic.SortedListCharKey`2.Count" />.</exception>
		public void RemoveAt( int index )
		{
            if ( index < 0 || index >= this._Size )
			{
				throw (new ArgumentOutOfRangeException("index"));
			}
			this._Size--;
            if ( index < this._Size )
			{
                System.Array.Copy( this._Array, index + 1, this._Array, index, this._Size - index );
			}
			this._Array[ this._Size ] = default(tuple_t);
		}
		/// <summary>Removes the element with the specified key from the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <returns>true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</returns>
		/// <param name="key">The key of the element to remove.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
		public bool Remove( char key )
		{
            int n = this.IndexOfKey( key );
            if ( n >= 0 )
            {
                this.RemoveAt( n );
            }
            return n >= 0;
		}

		/// <summary>Sets the capacity to the actual number of elements in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />, if that number is less than 90 percent of current capacity.</summary>
		public void TrimExcess()
		{
            int n = (int) ((double) this._Array.Length * 0.9);
            if ( this._Size < n )
            {
                this.Capacity = this._Size;
            }
		}
        public void Trim()
        {
            this.Capacity = this._Size;
        }

        private static int InternalBinarySearch( tuple_t[] array, int index, int length, char value )
        {
            int i  = index;
            int n1 = index + length - 1;
            while ( i <= n1 )
            {
                int n2 = i + (n1 - i >> 1);
                int n3 = array[ n2 ].Key - value;
                if ( n3 == 0 )
                {
                    return n2;
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
            return ~i;
        }
	}*/

    /// <summary>
    /// 
    /// </summary>
	internal struct SortedListCharKey< TValue > //: IDictionary< char, TValue >, ICollection< KeyValuePair< char, TValue > >, IEnumerable< KeyValuePair< char, TValue > >
        where TValue : class
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

		/// <summary>Gets or sets the number of elements that the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> can contain.</summary>
		/// <returns>The number of elements that the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> can contain.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <see cref="P:System.Collections.Generic.SortedListCharKey`2.Capacity" /> is set to a value that is less than <see cref="P:System.Collections.Generic.SortedListCharKey`2.Count" />.</exception>
		/// <exception cref="T:System.OutOfMemoryException">There is not enough memory available on the system.</exception>
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
					/*if (value < _Size)
					{
						throw new ArgumentOutOfRangeException("value");
					}*/
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

		/// <summary>Gets the number of key/value pairs contained in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <returns>The number of key/value pairs contained in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</returns>
		public int Count
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get { return _Size; }
		}

		/// <summary>Gets a collection containing the keys in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <returns>A <see cref="T:System.Collections.Generic.IList`1" /> containing the keys in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</returns>
        public Tuple[] Array
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return (_Array); }
		}
        public void InitArrayAsEmpty()
        {
            _Array = EMPTY_ARRAY;
        }

		/// <summary>Gets or sets the value associated with the specified key.</summary>
		/// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a <see cref="T:System.Collections.Generic.KeyNotFoundException" /> and a set operation creates a new element using the specified key.</returns>
		/// <param name="key">The key whose value to get or set.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
		/// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key" /> does not exist in the collection.</exception>
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
                //return default(TValue);
			}
			set
			{
                int n = InternalBinarySearch( /*_Array,0, _Size,*/ key );  //Array.BinarySearch< char >( _Array, 0, _Size, key );  //
                if ( n >= 0 )
				{
                    _Array[ n ].Value = value;
					return;
				}
                Insert( ~n, key, value );
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> class that is empty, has the default initial capacity, and uses the default <see cref="T:System.Collections.Generic.IComparer`1" />.</summary>
        public static SortedListCharKey< TValue > Create()
		{
            var o = new SortedListCharKey< TValue >()
            {
			    _Array = EMPTY_ARRAY,
                _Size  = 0,
            };
            return (o);
		}		
        public static SortedListCharKey< TValue > Create( int capacity )
		{
            var o = new SortedListCharKey< TValue >()
            {
			    _Array = new Tuple[ capacity ],
                _Size  = 0,
            };
            return (o);
		}		
		/// <summary>Adds an element with the specified key and value into the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="value">The value of the element to add. The value can be null for reference types.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
		/// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</exception>
		public void Add( char key, TValue value )
		{
            int n = InternalBinarySearch( /*_Array,0, _Size,*/ key ); //Array.BinarySearch< char >( _Array, 0, _Size, key );  //
            if ( n >= 0 )
			{
				//ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_AddingDuplicate);
                throw (new ArgumentException( n.ToString(), "n" ));
			}
            Insert( ~n, key, value );
		}

		/// <summary>Removes all elements from the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		public void Clear()
		{
            System.Array.Clear( _Array, 0, _Size );
			_Size = 0;
		}
		/// <summary>Determines whether the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> contains a specific key.</summary>
		/// <returns>true if the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> contains an element with the specified key; otherwise, false.</returns>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
		public bool ContainsKey( char key )
		{
            return (IndexOfKey( key ) >= 0);
		}

        //private const int DEFAULT_CAPACITY = 1 /*4*/;

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

		/// <summary>Searches for the specified key and returns the zero-based index within the entire <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <returns>The zero-based index of <paramref name="key" /> within the entire <see cref="T:System.Collections.Generic.SortedListCharKey`2" />, if found; otherwise, -1.</returns>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
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
		/// <summary>Gets the value associated with the specified key.</summary>
		/// <returns>true if the <see cref="T:System.Collections.Generic.SortedListCharKey`2" /> contains an element with the specified key; otherwise, false.</returns>
		/// <param name="key">The key whose value to get.</param>
		/// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
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
		/// <summary>Removes the element at the specified index of the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than zero.-or-<paramref name="index" /> is equal to or greater than <see cref="P:System.Collections.Generic.SortedListCharKey`2.Count" />.</exception>
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
			_Array[ _Size ] = default(Tuple);
		}
		/// <summary>Removes the element with the specified key from the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</summary>
		/// <returns>true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.SortedListCharKey`2" />.</returns>
		/// <param name="key">The key of the element to remove.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is null.</exception>
		public bool Remove( char key )
		{
            int n = IndexOfKey( key );
            if ( n >= 0 )
            {
                RemoveAt( n );
            }
            return (n >= 0);
		}

		/// <summary>Sets the capacity to the actual number of elements in the <see cref="T:System.Collections.Generic.SortedListCharKey`2" />, if that number is less than 90 percent of current capacity.</summary>
		public void TrimExcess()
		{
            int n = (int) ((double) _Array.Length * 0.9);
            if ( _Size < n )
            {
                Capacity = _Size;
            }
		}
        public void Trim()
        {
            Capacity = _Size;
        }

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
