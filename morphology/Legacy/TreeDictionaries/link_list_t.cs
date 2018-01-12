using System;
using System.Collections;
using System.Collections.Generic;

namespace lingvo.morphology
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class link_list_t< T > : IEnumerable< T >
        where T : struct
    {
        public link_list_t()
        {
        }
        public link_list_t( T item )
        {
            _Item = item;
        }

        private T? _Item;
        public T Item
        {
            get { return (_Item.Value); }
        }
        public link_list_t< T > Next
        {
            get;
            private set;
        }

        public link_list_t< T > Add( T item )
        {
            if ( !_Item.HasValue )
            {
                _Item = item;
                return (this);
            }
            else
            {
                for ( var ll = this; ; ll = ll.Next )
                {
                    if ( ll.Next == null )
                    {
                        var next = new link_list_t< T >( item );
                        ll.Next = next;
                        return (next);
                    }
                }
            }
        }
        public void AddRange( link_list_t< T > list )
        {
            if ( list.IsEmpty )
                return;

            if ( !_Item.HasValue )
            {
                _Item = list._Item;
                Next  = list.Next;
            }
            else
            {
                for ( var ll = this; ; ll = ll.Next )
                {
                    if ( ll.Next == null )
                    {
                        ll.Next = list;
                        return;
                    }
                }
            }
        }
        public bool IsEmpty
        {
            get { return (!_Item.HasValue); }
        }
        public int  Count
        {
            get { return (!_Item.HasValue ? 0 : 1); }
        }

        #region [.IEnumerable< T >.]
        public IEnumerator< T > GetEnumerator()
        {
            if ( !_Item.HasValue )
            {
                yield break;
            }
            else
            {
                for ( var ll = this; ll.Next != null; ll = ll.Next )
                {
                    yield return (ll.Item);
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (GetEnumerator());
        }
        #endregion
    }
}
