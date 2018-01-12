using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;

namespace lingvo.morphology
{
    /// <summary>
    /// Морфоатрибуты
    /// </summary>
    unsafe internal sealed class MorphoAttributeList_v1 : IDisposable
    {
        /// пара тип атрибута - значение атрибута
        private MorphoAttributePair[] _MorphoAttributePairs;
        private GCHandle              _MorphoAttributePairsGCHandle;
        private MorphoAttributePair*  _MorphoAttributePairsBasePtr;
        private int                   _MorphoAttributePairsLength_Minus1;

	    public MorphoAttributeList_v1()
        {
            #region commented. previous
            /*
            var morphoAttributes = new List< MorphoAttributePair >( 100 );

            /// первое
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Person, MorphoAttributeEnum.First ) );
            /// второе	
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Person, MorphoAttributeEnum.Second ) );
            /// третье
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Person, MorphoAttributeEnum.Third ) );

            /// именительный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Nominative ) );
            /// родительный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Genitive ) );
            /// дательный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Dative ) );
            /// винительный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Accusative ) );
            /// творительный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Instrumental ) );
            /// предложный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Prepositional ) );
            /// местный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Locative ) );
            /// любой
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Anycase ) );

            /// единственное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Number, MorphoAttributeEnum.Singular ) );
            /// множественное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Number, MorphoAttributeEnum.Plural ) );

            /// женский
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Gender, MorphoAttributeEnum.Feminine ) );
            /// мужской
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Gender, MorphoAttributeEnum.Masculine ) );
            /// средний
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Gender, MorphoAttributeEnum.Neuter ) );
            /// общий
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Gender, MorphoAttributeEnum.General ) );

            /// одушевленный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Animateness, MorphoAttributeEnum.Animate ) );
            /// неодушевленный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Animateness, MorphoAttributeEnum.Inanimate ) );

            /// имя собственное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NounType, MorphoAttributeEnum.Proper ) );
            /// имя нарицательное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NounType, MorphoAttributeEnum.Common ) );

            /// будущее
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Tense, MorphoAttributeEnum.Future ) );
            /// прошедшее
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Tense, MorphoAttributeEnum.Past ) );
            /// настоящее
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Tense, MorphoAttributeEnum.Present ) );
            /// будущее в прошедшем
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Tense, MorphoAttributeEnum.FutureInThePast ) );

            /// повелительное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Imperative ) );
            /// изъявительное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Indicative ) );
            /// сослагательное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Subjunctive ) );
            /// личный глагол
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Personal ) );
            /// безличный глагол
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Impersonal ) );
            /// деепричастие
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Gerund ) );
            /// причастие
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Participle ) );

            /// действительный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Voice, MorphoAttributeEnum.Active ) );
            /// страдательный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Voice, MorphoAttributeEnum.Passive ) );

            /// переходный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbTransitivity, MorphoAttributeEnum.Transitive ) );
            /// непереходный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbTransitivity, MorphoAttributeEnum.Intransitive ) );

            /// несовершенная
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbForm, MorphoAttributeEnum.Imperfective ) );
            /// совершенная
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbForm, MorphoAttributeEnum.Perfective ) );
            /// совершенная и несовершенная
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbForm, MorphoAttributeEnum.PerfImPerf ) );

            /// порядковое
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NumeralType, MorphoAttributeEnum.Ordinal ) );
            /// количественное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NumeralType, MorphoAttributeEnum.Cardinal ) );
            /// собирательное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NumeralType, MorphoAttributeEnum.Collective ) );

            /// краткая
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.AdjectForm, MorphoAttributeEnum.Short ) );

            /// сравнительная
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.DegreeOfComparison, MorphoAttributeEnum.Comparative ) );
            /// превосходная
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.DegreeOfComparison, MorphoAttributeEnum.Superlative ) );

            /// сочинительный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.ConjunctionType, MorphoAttributeEnum.Subordinating ) );
            /// подчинительный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.ConjunctionType, MorphoAttributeEnum.Coordinating ) );

            /// вопросительное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Interrogative ) );
            /// относительное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Relative ) );
            /// относительное и вопросительное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.InterrogativeRelative ) );
            /// отрицательное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Negative ) );
            /// возвратное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Reflexive ) );
            /// неопределенное 1
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Indefinitive1 ) );
            /// неопределенное 2
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Indefinitive2 ) );
            /// указательное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Indicative ) );
            /// притяжательное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Possessive ) );
            /// личное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Personal ) );

            /// определенный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.ArticleType, MorphoAttributeEnum.Definite ) );
            /// неопределенный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.ArticleType, MorphoAttributeEnum.Indefinite ) );

            /// инфинитив
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbType, MorphoAttributeEnum.Infinitive ) );
            /// деепричастие
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbType, MorphoAttributeEnum.AdverbialParticiple ) );
            /// вспомогательный глагол
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbType, MorphoAttributeEnum.AuxiliaryVerb ) );
            /// причастие
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbType, MorphoAttributeEnum.Participle ) );

            /// относительное и вопросительное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.AdverbType, MorphoAttributeEnum.InterrogativeRelative ) );
            */
            
            #endregion

            var morphoAttributes = new List< MorphoAttributePair >( 100 );

            /// первое
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Person, MorphoAttributeEnum.First ) );
            /// второе	
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Person, MorphoAttributeEnum.Second ) );
            /// третье
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Person, MorphoAttributeEnum.Third ) );

            /// именительный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Nominative ) );
            /// родительный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Genitive ) );
            /// дательный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Dative ) );
            /// винительный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Accusative ) );
            /// творительный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Instrumental ) );
            /// предложный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Prepositional ) );
            /// местный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Locative ) );
            /// любой
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Anycase ) );

            /// единственное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Number, MorphoAttributeEnum.Singular ) );
            /// множественное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Number, MorphoAttributeEnum.Plural ) );

            /// женский
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Gender, MorphoAttributeEnum.Feminine ) );
            /// мужской
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Gender, MorphoAttributeEnum.Masculine ) );
            /// средний
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Gender, MorphoAttributeEnum.Neuter ) );
            /// общий
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Gender, MorphoAttributeEnum.General ) );

            /// одушевленный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Animateness, MorphoAttributeEnum.Animate ) );
            /// неодушевленный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Animateness, MorphoAttributeEnum.Inanimate ) );

            /// имя собственное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NounType, MorphoAttributeEnum.Proper ) );
            /// имя нарицательное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NounType, MorphoAttributeEnum.Common ) );

            /// будущее
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Tense, MorphoAttributeEnum.Future ) );
            /// прошедшее
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Tense, MorphoAttributeEnum.Past ) );
            /// настоящее
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Tense, MorphoAttributeEnum.Present ) );
            /// будущее в прошедшем
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Tense, MorphoAttributeEnum.FutureInThePast ) );

            /// повелительное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Imperative ) );
            /// изъявительное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Indicative ) );
            /// сослагательное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Subjunctive ) );
            /// личный глагол
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Personal ) );
            /// безличный глагол
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Impersonal ) );
            /// деепричастие
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Gerund ) );
            /// причастие
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Participle ) );

            /// действительный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Voice, MorphoAttributeEnum.Active ) );
            /// страдательный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Voice, MorphoAttributeEnum.Passive ) );

            /// переходный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbTransitivity, MorphoAttributeEnum.Transitive ) );
            /// непереходный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbTransitivity, MorphoAttributeEnum.Intransitive ) );

            /// несовершенная
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbForm, MorphoAttributeEnum.Imperfective ) );
            /// совершенная
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbForm, MorphoAttributeEnum.Perfective ) );
            /// совершенная и несовершенная
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbForm, MorphoAttributeEnum.PerfImPerf ) );

            /// порядковое
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NumeralType, MorphoAttributeEnum.Ordinal ) );
            /// количественное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NumeralType, MorphoAttributeEnum.Cardinal ) );
            /// собирательное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NumeralType, MorphoAttributeEnum.Collective ) );

            /// краткая
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.AdjectForm, MorphoAttributeEnum.Short ) );

            /// сравнительная
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.DegreeOfComparison, MorphoAttributeEnum.Comparative ) );
            /// превосходная
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.DegreeOfComparison, MorphoAttributeEnum.Superlative ) );

            /// сочинительный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.ConjunctionType, MorphoAttributeEnum.Subordinating ) );
            /// подчинительный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.ConjunctionType, MorphoAttributeEnum.Coordinating ) );

            /// вопросительное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Interrogative ) );
            /// относительное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Relative ) );
            /// относительное и вопросительное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.InterrogativeRelative ) );
            /// отрицательное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Negative ) );
            /// возвратное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Reflexive ) );
            /// неопределенное 1
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Indefinitive1 ) );
            /// неопределенное 2
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Indefinitive2 ) );
            /// указательное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Indicative ) );
            /// притяжательное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Possessive ) );
            /// личное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Personal ) );

            /// определенный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.ArticleType, MorphoAttributeEnum.Definite ) );
            /// неопределенный
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.ArticleType, MorphoAttributeEnum.Indefinite ) );

            /// инфинитив
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbType, MorphoAttributeEnum.Infinitive ) );
            /// деепричастие
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbType, MorphoAttributeEnum.AdverbialParticiple ) );
            /// вспомогательный глагол
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbType, MorphoAttributeEnum.AuxiliaryVerb ) );
            /// причастие
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbType, MorphoAttributeEnum.Participle ) );

            /// относительное и вопросительное
            morphoAttributes.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.AdverbType, MorphoAttributeEnum.InterrogativeRelative ) );

            _MorphoAttributePairs = morphoAttributes.ToArray();
            _MorphoAttributePairsLength_Minus1 = _MorphoAttributePairs.Length - 1;
            _MorphoAttributePairsGCHandle = GCHandle.Alloc( _MorphoAttributePairs, GCHandleType.Pinned );
            _MorphoAttributePairsBasePtr  = (MorphoAttributePair*) _MorphoAttributePairsGCHandle.AddrOfPinnedObject().ToPointer();
        }
        ~MorphoAttributeList_v1()
        {
            DisposeNativeResources();
        }

        public void Dispose()
        {
            DisposeNativeResources();

            GC.SuppressFinalize( this );
        }
        private void DisposeNativeResources()
        {
            if ( _MorphoAttributePairsBasePtr != null )
            {
                _MorphoAttributePairsGCHandle.Free();
                _MorphoAttributePairsBasePtr = null;
            }
        }

        /// создание морфоатрибута из строки
        /// attribute - строка со значением атрибута
        /// attributeType - тип атрибута
        public MorphoAttributePair GetMorphoAttributePair( 
            MorphoAttributeGroupEnum morphoAttributeGroup, 
            MorphoAttributeEnum      morphoAttribute )
        {
            //fixed ( MorphoAttributePair* map_ptr = _MorphoAttributePairs )
            {
                for ( int i = _MorphoAttributePairsLength_Minus1; 0 <= i; i-- )
                {
                    //var morphoAttributePair = (map_ptr + i);
                    var morphoAttributePair = (_MorphoAttributePairsBasePtr + i);
                    if ( (morphoAttributeGroup & morphoAttributePair->MorphoAttributeGroup) == morphoAttributeGroup &&
                         (morphoAttribute      & morphoAttributePair->MorphoAttribute     ) == morphoAttribute
                       )
                    {
                        return (*morphoAttributePair);
                    }
                }
            }

            throw (new MorphoFormatException());
        }

        public MorphoAttributePair? TryGetMorphoAttributePair( 
            MorphoAttributeGroupEnum morphoAttributeGroup, 
            MorphoAttributeEnum      morphoAttribute )
        {
            for ( int i = _MorphoAttributePairsLength_Minus1; 0 <= i; i-- )
            {
                var morphoAttributePair = (_MorphoAttributePairsBasePtr + i);
                if ( (morphoAttributeGroup & morphoAttributePair->MorphoAttributeGroup) == morphoAttributeGroup &&
                     (morphoAttribute      & morphoAttributePair->MorphoAttribute     ) == morphoAttribute
                   )
                {
                    return (*morphoAttributePair);
                }
            }

            return (null);
        }
    }


    /// <summary>
    /// Морфоатрибуты
    /// </summary>
    internal sealed class MorphoAttributeList : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
	    private sealed class MorphoAttributePairSet
	    {
            /// <summary>
            /// 
            /// </summary>
		    internal struct Slot
		    {
			    internal int hashCode;            
			    internal int next;
                internal MorphoAttributePair value;
		    }

		    private int[]  _Buckets;
		    private Slot[] _Slots;
		    private int    _Count;
		    private int    _FreeList;

            internal Slot[] Slots
            {
                get { return (_Slots); }
            }
            public int Count
            {
                get { return (_Count); }
            }

		    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		    public MorphoAttributePairSet( int capacity )
		    {
			    _Buckets  = new int[ capacity ];
			    _Slots    = new Slot[ capacity ];
			    _FreeList = -1;
		    }

            public bool Add( MorphoAttributePair value )
            {
                return (Add( ref value ));
            }
            public bool Add( ref MorphoAttributePair value )
		    {
                #region [.find exists.]
                int hash = InternalGetHashCode( ref value );
                for ( int i = _Buckets[ hash % _Buckets.Length ] - 1; 0 <= i; /*i = _Slots[ i ].next*/ )
                {
                    var slot = _Slots[ i ];
                    if ( (slot.hashCode == hash) && IsEquals( ref slot.value, ref value ) )
                    {
                        return (false);
                    }
                    i = slot.next;
                }
                #endregion

                #region [.add new.]
                {
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
                }

                return (true);
                #endregion                
		    }

		    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            public bool Contains( MorphoAttributeGroupEnum morphoAttributeGroup, MorphoAttributeEnum morphoAttribute )
            {
                #region [.find exists.]
                int hash = InternalGetHashCode( morphoAttributeGroup, morphoAttribute );
                for ( int i = _Buckets[ hash % _Buckets.Length ] - 1; 0 <= i; /*i = _Slots[ i ].next*/ )
                {
                    var slot = _Slots[ i ];
                    if ( (slot.hashCode == hash) && IsEquals( ref slot.value, morphoAttributeGroup, morphoAttribute ) )
                    {
                        return (true);
                    }
                    i = slot.next;
                }

                return (false);
                #endregion
            }

            [TargetedPatchingOptOut( "Performance critical to inline this type of method across NGen image boundaries" )]
            public bool TryGetValue( MorphoAttributeGroupEnum morphoAttributeGroup, MorphoAttributeEnum morphoAttribute, ref MorphoAttributePair value )
            {
                #region [.find exists.]
                int hash = InternalGetHashCode( morphoAttributeGroup, morphoAttribute );
                for ( int i = _Buckets[ hash % _Buckets.Length ] - 1; 0 <= i; /*i = _Slots[ i ].next*/ )
                {
                    var slot = _Slots[ i ];
                    if ( (slot.hashCode == hash) && IsEquals( ref slot.value, morphoAttributeGroup, morphoAttribute ) )
                    {
                        value = slot.value;
                        return (true);
                    }
                    i = slot.next;
                }

                return (false);
                #endregion
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

            private static bool IsEquals( ref MorphoAttributePair v1, ref MorphoAttributePair v2 )
            {
                return ( (v1.MorphoAttributeGroup & v2.MorphoAttributeGroup) == v2.MorphoAttributeGroup &&
                         (v1.MorphoAttribute      & v2.MorphoAttribute     ) == v2.MorphoAttribute );
            }
            private static bool IsEquals( ref MorphoAttributePair v1, MorphoAttributeGroupEnum morphoAttributeGroup, MorphoAttributeEnum morphoAttribute )
            {
                return ( (v1.MorphoAttributeGroup & morphoAttributeGroup) == morphoAttributeGroup &&
                         (v1.MorphoAttribute      & morphoAttribute     ) == morphoAttribute );
            }

            private static int InternalGetHashCode( ref MorphoAttributePair value )
		    {
                return ((value.MorphoAttributeGroup.GetHashCode() ^ value.MorphoAttribute.GetHashCode()) & 0x7fffffff);
		    }
            private static int InternalGetHashCode( MorphoAttributeGroupEnum morphoAttributeGroup, MorphoAttributeEnum morphoAttribute )
		    {
                return ((morphoAttributeGroup.GetHashCode() ^ morphoAttribute.GetHashCode()) & 0x7fffffff);
		    }
	    }


        /// пара тип атрибута - значение атрибута
        private MorphoAttributePairSet _Set;
        private MorphoAttributePair    _Pair;

	    public MorphoAttributeList()
        {
            #region commented. previous
            /*
            var _Set = new MorphoAttributePairSet( 100 );

            /// первое
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Person, MorphoAttributeEnum.First ) );
            /// второе	
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Person, MorphoAttributeEnum.Second ) );
            /// третье
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Person, MorphoAttributeEnum.Third ) );

            /// именительный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Nominative ) );
            /// родительный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Genitive ) );
            /// дательный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Dative ) );
            /// винительный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Accusative ) );
            /// творительный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Instrumental ) );
            /// предложный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Prepositional ) );
            /// местный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Locative ) );
            /// любой
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Anycase ) );

            /// единственное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Number, MorphoAttributeEnum.Singular ) );
            /// множественное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Number, MorphoAttributeEnum.Plural ) );

            /// женский
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Gender, MorphoAttributeEnum.Feminine ) );
            /// мужской
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Gender, MorphoAttributeEnum.Masculine ) );
            /// средний
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Gender, MorphoAttributeEnum.Neuter ) );
            /// общий
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Gender, MorphoAttributeEnum.General ) );

            /// одушевленный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Animateness, MorphoAttributeEnum.Animate ) );
            /// неодушевленный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Animateness, MorphoAttributeEnum.Inanimate ) );

            /// имя собственное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NounType, MorphoAttributeEnum.Proper ) );
            /// имя нарицательное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NounType, MorphoAttributeEnum.Common ) );

            /// будущее
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Tense, MorphoAttributeEnum.Future ) );
            /// прошедшее
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Tense, MorphoAttributeEnum.Past ) );
            /// настоящее
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Tense, MorphoAttributeEnum.Present ) );
            /// будущее в прошедшем
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Tense, MorphoAttributeEnum.FutureInThePast ) );

            /// повелительное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Imperative ) );
            /// изъявительное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Indicative ) );
            /// сослагательное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Subjunctive ) );
            /// личный глагол
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Personal ) );
            /// безличный глагол
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Impersonal ) );
            /// деепричастие
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Gerund ) );
            /// причастие
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Participle ) );

            /// действительный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Voice, MorphoAttributeEnum.Active ) );
            /// страдательный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Voice, MorphoAttributeEnum.Passive ) );

            /// переходный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbTransitivity, MorphoAttributeEnum.Transitive ) );
            /// непереходный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbTransitivity, MorphoAttributeEnum.Intransitive ) );

            /// несовершенная
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbForm, MorphoAttributeEnum.Imperfective ) );
            /// совершенная
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbForm, MorphoAttributeEnum.Perfective ) );
            /// совершенная и несовершенная
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbForm, MorphoAttributeEnum.PerfImPerf ) );

            /// порядковое
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NumeralType, MorphoAttributeEnum.Ordinal ) );
            /// количественное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NumeralType, MorphoAttributeEnum.Cardinal ) );
            /// собирательное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NumeralType, MorphoAttributeEnum.Collective ) );

            /// краткая
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.AdjectForm, MorphoAttributeEnum.Short ) );

            /// сравнительная
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.DegreeOfComparison, MorphoAttributeEnum.Comparative ) );
            /// превосходная
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.DegreeOfComparison, MorphoAttributeEnum.Superlative ) );

            /// сочинительный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.ConjunctionType, MorphoAttributeEnum.Subordinating ) );
            /// подчинительный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.ConjunctionType, MorphoAttributeEnum.Coordinating ) );

            /// вопросительное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Interrogative ) );
            /// относительное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Relative ) );
            /// относительное и вопросительное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.InterrogativeRelative ) );
            /// отрицательное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Negative ) );
            /// возвратное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Reflexive ) );
            /// неопределенное 1
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Indefinitive1 ) );
            /// неопределенное 2
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Indefinitive2 ) );
            /// указательное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Indicative ) );
            /// притяжательное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Possessive ) );
            /// личное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Personal ) );

            /// определенный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.ArticleType, MorphoAttributeEnum.Definite ) );
            /// неопределенный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.ArticleType, MorphoAttributeEnum.Indefinite ) );

            /// инфинитив
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbType, MorphoAttributeEnum.Infinitive ) );
            /// деепричастие
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbType, MorphoAttributeEnum.AdverbialParticiple ) );
            /// вспомогательный глагол
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbType, MorphoAttributeEnum.AuxiliaryVerb ) );
            /// причастие
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbType, MorphoAttributeEnum.Participle ) );

            /// относительное и вопросительное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.AdverbType, MorphoAttributeEnum.InterrogativeRelative ) );
            */

            #endregion

            _Set = new MorphoAttributePairSet( 100 );

            /// первое
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Person, MorphoAttributeEnum.First ) );
            /// второе	
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Person, MorphoAttributeEnum.Second ) );
            /// третье
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Person, MorphoAttributeEnum.Third ) );

            /// именительный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Nominative ) );
            /// родительный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Genitive ) );
            /// дательный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Dative ) );
            /// винительный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Accusative ) );
            /// творительный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Instrumental ) );
            /// предложный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Prepositional ) );
            /// местный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Locative ) );
            /// любой
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Case, MorphoAttributeEnum.Anycase ) );

            /// единственное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Number, MorphoAttributeEnum.Singular ) );
            /// множественное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Number, MorphoAttributeEnum.Plural ) );

            /// женский
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Gender, MorphoAttributeEnum.Feminine ) );
            /// мужской
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Gender, MorphoAttributeEnum.Masculine ) );
            /// средний
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Gender, MorphoAttributeEnum.Neuter ) );
            /// общий
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Gender, MorphoAttributeEnum.General ) );

            /// одушевленный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Animateness, MorphoAttributeEnum.Animate ) );
            /// неодушевленный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Animateness, MorphoAttributeEnum.Inanimate ) );

            /// имя собственное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NounType, MorphoAttributeEnum.Proper ) );
            /// имя нарицательное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NounType, MorphoAttributeEnum.Common ) );

            /// будущее
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Tense, MorphoAttributeEnum.Future ) );
            /// прошедшее
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Tense, MorphoAttributeEnum.Past ) );
            /// настоящее
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Tense, MorphoAttributeEnum.Present ) );
            /// будущее в прошедшем
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Tense, MorphoAttributeEnum.FutureInThePast ) );

            /// повелительное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Imperative ) );
            /// изъявительное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Indicative ) );
            /// сослагательное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Subjunctive ) );
            /// личный глагол
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Personal ) );
            /// безличный глагол
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Impersonal ) );
            /// деепричастие
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Gerund ) );
            /// причастие
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Mood, MorphoAttributeEnum.Participle ) );

            /// действительный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Voice, MorphoAttributeEnum.Active ) );
            /// страдательный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.Voice, MorphoAttributeEnum.Passive ) );

            /// переходный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbTransitivity, MorphoAttributeEnum.Transitive ) );
            /// непереходный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbTransitivity, MorphoAttributeEnum.Intransitive ) );

            /// несовершенная
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbForm, MorphoAttributeEnum.Imperfective ) );
            /// совершенная
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbForm, MorphoAttributeEnum.Perfective ) );
            /// совершенная и несовершенная
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbForm, MorphoAttributeEnum.PerfImPerf ) );

            /// порядковое
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NumeralType, MorphoAttributeEnum.Ordinal ) );
            /// количественное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NumeralType, MorphoAttributeEnum.Cardinal ) );
            /// собирательное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.NumeralType, MorphoAttributeEnum.Collective ) );

            /// краткая
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.AdjectForm, MorphoAttributeEnum.Short ) );

            /// сравнительная
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.DegreeOfComparison, MorphoAttributeEnum.Comparative ) );
            /// превосходная
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.DegreeOfComparison, MorphoAttributeEnum.Superlative ) );

            /// сочинительный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.ConjunctionType, MorphoAttributeEnum.Subordinating ) );
            /// подчинительный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.ConjunctionType, MorphoAttributeEnum.Coordinating ) );

            /// вопросительное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Interrogative ) );
            /// относительное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Relative ) );
            /// относительное и вопросительное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.InterrogativeRelative ) );
            /// отрицательное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Negative ) );
            /// возвратное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Reflexive ) );
            /// неопределенное 1
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Indefinitive1 ) );
            /// неопределенное 2
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Indefinitive2 ) );
            /// указательное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Indicative ) );
            /// притяжательное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Possessive ) );
            /// личное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.PronounType, MorphoAttributeEnum.Personal ) );

            /// определенный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.ArticleType, MorphoAttributeEnum.Definite ) );
            /// неопределенный
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.ArticleType, MorphoAttributeEnum.Indefinite ) );

            /// инфинитив
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbType, MorphoAttributeEnum.Infinitive ) );
            /// деепричастие
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbType, MorphoAttributeEnum.AdverbialParticiple ) );
            /// вспомогательный глагол
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbType, MorphoAttributeEnum.AuxiliaryVerb ) );
            /// причастие
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.VerbType, MorphoAttributeEnum.Participle ) );

            /// относительное и вопросительное
            _Set.Add( new MorphoAttributePair( MorphoAttributeGroupEnum.AdverbType, MorphoAttributeEnum.InterrogativeRelative ) );
        }
        public void Dispose()
        {
        }

        /// создание морфоатрибута из строки
        /// attribute - строка со значением атрибута
        /// attributeType - тип атрибута
        public MorphoAttributePair GetMorphoAttributePair( 
            MorphoAttributeGroupEnum morphoAttributeGroup, 
            MorphoAttributeEnum      morphoAttribute )
        {
            if ( _Set.TryGetValue( morphoAttributeGroup, morphoAttribute, ref _Pair ) )
            {
                return (_Pair);
            }

            throw (new MorphoFormatException());
        }

        public MorphoAttributePair GetMorphoAttributePair_2( 
            MorphoAttributeGroupEnum morphoAttributeGroup, 
            MorphoAttributeEnum      morphoAttribute )
        {
            var pair = default(MorphoAttributePair);
            if ( _Set.TryGetValue( morphoAttributeGroup, morphoAttribute, ref pair ) )
            {
                return (pair);
            }

            throw (new MorphoFormatException());
        }

        public MorphoAttributePair? TryGetMorphoAttributePair( 
            MorphoAttributeGroupEnum morphoAttributeGroup, 
            MorphoAttributeEnum      morphoAttribute )
        {
            if ( _Set.TryGetValue( morphoAttributeGroup, morphoAttribute, ref _Pair ) )
            {
                return (_Pair);
            }

            return (null);
        }
    }
}

