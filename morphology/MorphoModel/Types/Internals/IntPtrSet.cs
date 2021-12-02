using System;
using System.Collections;
using System.Collections.Generic;

namespace lingvo.morphology
{
    /// <summary>
    /// 
    /// </summary>
	internal sealed class IntPtrSet : IEnumerable< IntPtr >
	{
        /// <summary>
        /// 
        /// </summary>
		internal struct Slot
		{                
            internal int    hashCode;
            internal int    next;
            internal IntPtr value;
		}

        /// <summary>
        /// 
        /// </summary>
        internal struct Enumerator : IEnumerator< IntPtr >
        {
            private IntPtrSet _Set;	            
	        private IntPtr    _Current;
            private int       _Index;

            public IntPtr Current => _Current;
	        object IEnumerator.Current => _Current;

            internal Enumerator( IntPtrSet set )
	        {
		        this._Set     = set;
		        this._Index   = 0;
		        this._Current = default;
	        }
            public void Dispose() { }

	        public bool MoveNext()
	        {
                while ( _Index < _Set._Count )
		        {
                    _Current = _Set._Slots[ _Index ].value;
                    _Index++;
                    if ( _Current != IntPtr.Zero )
                    {                            
                        return (true);
                    }
                }
                _Index = _Set._Count + 1;
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

        internal Slot[] Slots => _Slots;
        public int Count => _Count;
		public IntPtrSet( int capacity )
		{
			_Buckets  = new int [ capacity ];
			_Slots    = new Slot[ capacity ];
			_FreeList = -1;
		}

        public bool Add( IntPtr value )
		{
            #region [.find exists.]
            int hash = (value.GetHashCode() & 0x7FFFFFFF);
            for ( int i = _Buckets[ hash % _Buckets.Length ] - 1; 0 <= i; )
            {
                var slot = _Slots[ i ];
                if ( /*(slot.hashCode == hash) &&*/ (slot.value == value) )
                {
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
                value    = value,
                next     = _Buckets[ n2 ] - 1,
            };
            _Buckets[ n2 ] = n1 + 1;

            return (true);
            #endregion
        }

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

        public Enumerator GetEnumerator() => new Enumerator( this );
        IEnumerator< IntPtr > IEnumerable< IntPtr >.GetEnumerator() => new Enumerator( this );
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator( this );
    }
}
