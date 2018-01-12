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
    /// Морфотип
    /// </summary>
    unsafe internal sealed class MorphoTypeNative
    {
        /// <summary>
        /// 
        /// </summary>
        internal struct MorphoFormEndingUpperAndMorphoAttribute
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            public MorphoFormEndingUpperAndMorphoAttribute( IntPtr endingUpper, MorphoAttributeEnum[] morphoAttributes )
            {
                EndingUpper      = endingUpper;
                MorphoAttributes = morphoAttributes;
            }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            public MorphoFormEndingUpperAndMorphoAttribute( IntPtr endingUpper, LinkedList< MorphoAttributeEnum > morphoAttributes )
            {
                EndingUpper      = endingUpper;
                MorphoAttributes = new MorphoAttributeEnum[ morphoAttributes.Count ];
                morphoAttributes.CopyTo( MorphoAttributes, 0 );
            }

            public IntPtr                EndingUpper;
            public MorphoAttributeEnum[] MorphoAttributes;

            public override string ToString()
            {
                return ("[" + StringsHelper.ToString( EndingUpper ) + ", {" + string.Join( ",", MorphoAttributes ) + "}]");
            }
        }

        /// <summary>
        /// 
        /// </summary>
	    private sealed class Set< TValue > : IEnumerable< KeyValuePair< IntPtr, TValue > >
            where TValue : class
	    {
            /// <summary>
            /// 
            /// </summary>
		    internal struct Slot
		    {                
                internal int    hashCode;
			    internal int    next;
                internal IntPtr key;
                internal TValue value;
		    }

            /// <summary>
            /// 
            /// </summary>
            internal struct Enumerator : IEnumerator< KeyValuePair< IntPtr, TValue > >
            {
                private Set< TValue > _Set;                
                private int           _Index;
                private KeyValuePair< IntPtr, TValue > _Current;

                public IntPtr Current_IntPtr
                {
                    [TargetedPatchingOptOut( "Performance critical to inline this type of method across NGen image boundaries" )]
                    get { return _Current.Key; }
                }
                public TValue Current_Value
                {
                    [TargetedPatchingOptOut( "Performance critical to inline this type of method across NGen image boundaries" )]
                    get { return _Current.Value; }
                }
                public KeyValuePair< IntPtr, TValue > Current
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

                internal Enumerator( Set< TValue > set )
	            {
		            this._Set     = set;
		            this._Index   = 0;
		            this._Current = default(KeyValuePair< IntPtr, TValue >);
	            }
                public void Dispose()
                {
                }

	            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
	            public bool MoveNext()
	            {
                    while ( _Index < _Set._Count )
		            {
                        var slot = _Set._Slots[ _Index ];
                        _Index++;
                        if ( slot.value != null )
                        {
                            _Current = new KeyValuePair< IntPtr, TValue >( slot.key, slot.value );
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
		            _Current = default(KeyValuePair< IntPtr, TValue >);
		            return (false);
	            }

	            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
	            void IEnumerator.Reset()
	            {
		            _Index   = 0;
                    _Current = default(KeyValuePair< IntPtr, TValue >);
	            }
            }            

		    private int[]  _Buckets;
		    private Slot[] _Slots;
		    private int    _Count;
		    private int    _FreeList;

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
		    public Set( int capacity )
		    {
			    _Buckets  = new int [ capacity ];
			    _Slots    = new Slot[ capacity ];
			    _FreeList = -1;
		    }

            /// <summary>
            /// try add not-exists-item & return-(true), else get exists-item to 'existsValue' & return-(false)
            /// </summary>
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            public bool Add( IntPtr key, TValue value )
		    {
                #region [.find exists.]
                int hash = (key.GetHashCode() & 0x7FFFFFFF);
                for ( int i = _Buckets[ hash % _Buckets.Length ] - 1; 0 <= i; )
                {
                    var slot = _Slots[ i ];
                    if ( /*(slot.hashCode == hash) &&*/ (slot.key == key) )
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
                    key      = key,
                    value    = value,
                    next     = _Buckets[ n2 ] - 1,
                };
                _Buckets[ n2 ] = n1 + 1;

                return (true);
                #endregion
            }

            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            public bool TryGetValue( IntPtr key, ref TValue existsValue )
            {
                #region [.find exists.]
                int hash = (key.GetHashCode() & 0x7fffffff);
                for ( int i = _Buckets[ hash % _Buckets.Length ] - 1; 0 <= i; )
                {
                    var slot = _Slots[ i ];
                    if ( /*(slot.hashCode == hash) &&*/ (slot.key == key) )
                    {
                        existsValue = slot.value;
                        return (true);
                    }
                    i = slot.next;
                }

                return (false);
                #endregion
            }

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
                int[]  buckets = new int [ n1 ];
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
            public Set< TValue >.Enumerator GetEnumerator()
            {
                return (new Set< TValue >.Enumerator( this ));
            }

            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            IEnumerator< KeyValuePair< IntPtr, TValue > > IEnumerable< KeyValuePair< IntPtr, TValue > >.GetEnumerator()
            {
                return (new Set< TValue >.Enumerator( this ));
            }

            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return (new Set< TValue >.Enumerator( this ));
            }
	    }

        //private static readonly MorphoFormNative[] EMPTY_MORPHOFORM = new MorphoFormNative[ 0 ];
        private static readonly char*[]                                   EMPTY_ENDINGS = new char*[ 0 ];
        private static readonly MorphoFormEndingUpperAndMorphoAttribute[] EMPTY_MFUEMA  = new MorphoFormEndingUpperAndMorphoAttribute[ 0 ];

        #region [.temp-buffers (static, because model loading in single-thread).]
        /// temp (static, because model loading in single-thread)        
        private static IntPtrSet tempBufferHS; /*---private static HashSet< IntPtr > tempBufferHS;*/
        /// temp (static, because model loading in single-thread)
        private static Set< LinkedList< MorphoAttributeEnum > > tempBufferDict; /*---private static Dictionary< IntPtr, LinkedList< MorphoAttributeEnum > > tempBufferDict;*/
        /// temp (static, because model loading in single-thread)
        private static Stack< LinkedList< MorphoAttributeEnum > > tempBufferLinkedLists;

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        private static LinkedList< MorphoAttributeEnum > PopLinkedList()
        {
            if ( tempBufferLinkedLists.Count != 0 )
            {
                return (tempBufferLinkedLists.Pop());
            }
            else
            {
                var pairs = new LinkedList< MorphoAttributeEnum >();
                tempBufferLinkedLists.Push( pairs );
                return (pairs);
            }               
        }
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        private static void PushLinkedList( LinkedList< MorphoAttributeEnum > pairs )
        {
            pairs.Clear();
            tempBufferLinkedLists.Push( pairs );
        }

        internal static void BeginLoad()
        {
            const int DEFAULT_CAPACITY = 107;

            tempBufferHS   = new IntPtrSet( DEFAULT_CAPACITY ); /*---tempBufferHS = new HashSet< IntPtr >();*/
            tempBufferDict = new Set< LinkedList< MorphoAttributeEnum > >( DEFAULT_CAPACITY ); /*---tempBufferDict = new Dictionary< IntPtr, LinkedList< MorphoAttributeEnum > >( DEFAULT_CAPACITY );*/
            tempBufferLinkedLists = new Stack< LinkedList< MorphoAttributeEnum > >( DEFAULT_CAPACITY );
            for ( var i = 0; i < DEFAULT_CAPACITY; i++ )
            {
                tempBufferLinkedLists.Push( new LinkedList< MorphoAttributeEnum >() );
            }
        }
        internal static void EndLoad()
        {
            tempBufferHS   = null;
            tempBufferDict = null;
            tempBufferLinkedLists = null;
        }
        #endregion

        /// группа морфо-атрибутов
        private MorphoAttributeGroupEnum _MorphoAttributeGroup;
        /// часть речи
        private PartOfSpeechEnum         _PartOfSpeech;
        /// морфо-формы
        /*private MorphoFormNative[] _MorphoForms;*/

        /// окончания морфо-форм
        private char*[] _MorphoFormEndings;
        /// кортежи uppercase-окончаний морфо-форма и морфо-атрибутов
        private MorphoFormEndingUpperAndMorphoAttribute[] _MorphoFormEndingUpperAndMorphoAttributes;

        internal MorphoTypeNative( PartOfSpeechBase partOfSpeechBase )
        {
            //_MorphoForms          = EMPTY_MORPHOFORM;
            _MorphoAttributeGroup = partOfSpeechBase.MorphoAttributeGroup;
            _PartOfSpeech         = partOfSpeechBase.PartOfSpeech;
            _MorphoFormEndings    = EMPTY_ENDINGS;
            _MorphoFormEndingUpperAndMorphoAttributes = EMPTY_MFUEMA;
        }

        internal void SetMorphoForms( List< MorphoFormNative > morphoForms )
        {
            if ( morphoForms.Count != 0 )
            {
                //---_MorphoForms = morphoForms.ToArray();

                LinkedList< MorphoAttributeEnum > morphoAttributes = null;
                for ( int i = 0, len = morphoForms.Count; i < len; i++ )
                {
                    var morphoForm = morphoForms[ i ];

                    #region [.окончания морфо-форм.]
                    tempBufferHS.Add( (IntPtr) morphoForm.Ending );
                    #endregion

                    #region [.MorphoFormEndingUpper-&-MorphoAttribute.]
                    var endingUpperPtr = (IntPtr) morphoForm.EndingUpper;
                    if ( !tempBufferDict.TryGetValue( endingUpperPtr, ref morphoAttributes ) )
                    {
                        morphoAttributes = PopLinkedList();
                        tempBufferDict.Add( endingUpperPtr, morphoAttributes );
                    }
                    var morphoAttribute = MorphoAttributePair.GetMorphoAttribute( this, morphoForm );
                    morphoAttributes.AddLast( morphoAttribute );
                    #endregion
                }

                #region [.окончания морфо-форм.]
                _MorphoFormEndings = new char*[ tempBufferHS.Count ];
                fixed ( char** morphoFormEndingsBase = _MorphoFormEndings )
                {
                    var it = tempBufferHS.GetEnumerator();
                    for ( var i = 0; it.MoveNext(); i++ )
                    {
                        *(morphoFormEndingsBase + i) = (char*) it.Current;
                    }

                    #region commented. foreach.
                    /*var i = 0;
                    foreach ( var intptr in tempBufferHS )
                    {
                        *(morphoFormEndingsBase + i++) = (char*) intptr;
                    }*/
                    #endregion
                }
                tempBufferHS.Clear();
                #endregion

                #region [.MorphoFormEndingUpper-&-MorphoAttribute.]
                _MorphoFormEndingUpperAndMorphoAttributes = new MorphoFormEndingUpperAndMorphoAttribute[ tempBufferDict.Count ];

                var it2 = tempBufferDict.GetEnumerator();
                for ( var i = 0; it2.MoveNext(); i++ )
                {
                    _MorphoFormEndingUpperAndMorphoAttributes[ i ] = new MorphoFormEndingUpperAndMorphoAttribute(
                                                                            it2.Current_IntPtr, it2.Current_Value );
                    PushLinkedList( it2.Current_Value );
                }
                #region commented
                /*
                var k = 0;
                foreach ( var p in tempBufferDict )
                {
                    _MorphoFormEndingUpperAndMorphoAttributes[ k++ ] = new MorphoFormEndingUpperAndMorphoAttribute( p.Key, p.Value.ToArray() );
                    PushLinkedList( p.Value );
                }
                */
                #endregion

                tempBufferDict.Clear();
                #endregion
            }
            else
            {
                //_MorphoForms = EMPTY_MORPHOFORM;
                _MorphoFormEndings = EMPTY_ENDINGS;
                _MorphoFormEndingUpperAndMorphoAttributes = EMPTY_MFUEMA;
            }
        }

        /// группа морфо-атрибутов
        public MorphoAttributeGroupEnum MorphoAttributeGroup
        {
            get { return (_MorphoAttributeGroup); }
        }
        /// часть речи
        public PartOfSpeechEnum PartOfSpeech
        {
            get { return (_PartOfSpeech); }
        }

        /// морфо-формы
        /*public MorphoFormNative[] MorphoForms
        {
            get { return (_MorphoForms); }
        }*/

        public bool  HasMorphoForms
        {
            get { return (MorphoFormEndings.Length != 0/*MorphoForms.Length != 0*/); }
        }
        public char* FirstEnding
        {
            get { return (MorphoFormEndings[ 0 ]/*_MorphoForms[ 0 ].Ending*/); }
        }

        /// кортежи uppercase-окончаний морфо-форма и морфо-атрибутов
        public MorphoFormEndingUpperAndMorphoAttribute[] MorphoFormEndingUpperAndMorphoAttributes
        {
            get { return (_MorphoFormEndingUpperAndMorphoAttributes); }
        }
        /// окончания морфо-форм
        public char*[] MorphoFormEndings
        {
            get { return (_MorphoFormEndings); }
        }

        public override string ToString()
        {
            return ("[" + PartOfSpeech + ", " + MorphoAttributeGroup + ", {" + 
                    /*string.Join( ",", (IEnumerable< MorphoFormNative >) MorphoForms )*/
                    string.Join( ",", MorphoFormEndingUpperAndMorphoAttributes ) + "}]");
        }
    }
}