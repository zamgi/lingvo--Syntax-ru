using System;

using M = System.Runtime.CompilerServices.MethodImplAttribute;
using O = System.Runtime.CompilerServices.MethodImplOptions;

namespace lingvo.morphology
{
    /// <summary>
    /// Морфоатрибуты
    /// </summary>
    internal sealed class MorphoAttributeList
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
                internal int HashCode;
                internal int Next;
                internal MorphoAttributePair Value;
            }

            private int[] _Buckets;
            private Slot[] _Slots;
            private int _Count;
            private int _FreeList;

            internal Slot[] Slots => _Slots;
            public int Count => _Count;

            public MorphoAttributePairSet( int capacity )
            {
                _Buckets = new int[ capacity ];
                _Slots = new Slot[ capacity ];
                _FreeList = -1;
            }

            public bool Add( MorphoAttributePair value ) => Add( in value );
            public bool Add( in MorphoAttributePair value )
            {
                #region [.find exists.]
                int hash = InternalGetHashCode( in value );
                for ( int i = _Buckets[ hash % _Buckets.Length ] - 1; 0 <= i; /*i = _Slots[ i ].next*/ )
                {
                    var slot = _Slots[ i ];
                    if ( (slot.HashCode == hash) && IsEquals( in slot.Value, in value ) )
                    {
                        return (false);
                    }
                    i = slot.Next;
                }
                #endregion

                #region [.add new.]
                {
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
                        Value = value,
                        Next = _Buckets[ n2 ] - 1,
                    };
                    _Buckets[ n2 ] = n1 + 1;
                }

                return (true);
                #endregion
            }
            public bool Contains( MorphoAttributeGroupEnum mag, MorphoAttributeEnum ma )
            {
                #region [.find exists.]
                int hash = InternalGetHashCode( mag, ma );
                for ( int i = _Buckets[ hash % _Buckets.Length ] - 1; 0 <= i; /*i = _Slots[ i ].next*/ )
                {
                    var slot = _Slots[ i ];
                    if ( (slot.HashCode == hash) && IsEquals( in slot.Value, mag, ma ) )
                    {
                        return (true);
                    }
                    i = slot.Next;
                }

                return (false);
                #endregion
            }
            [M( O.AggressiveInlining )]
            public bool TryGetValue( MorphoAttributeGroupEnum mag, MorphoAttributeEnum ma, ref MorphoAttributePair value )
            {
                #region [.find exists.]
                int hash = InternalGetHashCode( mag, ma );
                for ( int i = _Buckets[ hash % _Buckets.Length ] - 1; 0 <= i; /*i = _Slots[ i ].next*/ )
                {
                    var slot = _Slots[ i ];
                    if ( (slot.HashCode == hash) && IsEquals( in slot.Value, mag, ma ) )
                    {
                        value = slot.Value;
                        return (true);
                    }
                    i = slot.Next;
                }

                return (false);
                #endregion
            }

            private void Resize()
            {
                int n1 = checked(_Count * 2 + 1);
                int[] buckets = new int[ n1 ];
                Slot[] slots = new Slot[ n1 ];
                Array.Copy( _Slots, 0, slots, 0, _Count );
                for ( int i = 0; i < _Count; i++ )
                {
                    int n2 = slots[ i ].HashCode % n1;
                    slots[ i ].Next = buckets[ n2 ] - 1;
                    buckets[ n2 ] = i + 1;
                }
                _Buckets = buckets;
                _Slots = slots;
            }

            [M( O.AggressiveInlining )]
            private static bool IsEquals( in MorphoAttributePair v1, in MorphoAttributePair v2 ) => ((v1.MorphoAttributeGroup & v2.MorphoAttributeGroup) == v2.MorphoAttributeGroup &&
                                                                                                     (v1.MorphoAttribute & v2.MorphoAttribute) == v2.MorphoAttribute);
            [M( O.AggressiveInlining )]
            private static bool IsEquals( in MorphoAttributePair v1, MorphoAttributeGroupEnum morphoAttributeGroup, MorphoAttributeEnum morphoAttribute )
                => ((v1.MorphoAttributeGroup & morphoAttributeGroup) == morphoAttributeGroup && (v1.MorphoAttribute & morphoAttribute) == morphoAttribute);

            [M( O.AggressiveInlining )] private static int InternalGetHashCode( in MorphoAttributePair value ) => ((value.MorphoAttributeGroup.GetHashCode() ^ value.MorphoAttribute.GetHashCode()) & 0x7fffffff);
            [M( O.AggressiveInlining )] private static int InternalGetHashCode( MorphoAttributeGroupEnum mag, MorphoAttributeEnum ma ) => ((mag.GetHashCode() ^ ma.GetHashCode()) & 0x7fffffff);
        }

        /// пара тип атрибута - значение атрибута
        private MorphoAttributePairSet _Set;
        //private MorphoAttributePair    _Pair;

        public MorphoAttributeList()
        {
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

        private MorphoAttributePair _Pair;
        [M(O.AggressiveInlining)] public MorphoAttributePair GetMorphoAttributePair( MorphoAttributeGroupEnum mag, MorphoAttributeEnum ma )
        {
            if ( _Set.TryGetValue( mag, ma, ref _Pair ) )
            {
                return (_Pair);
            }

            throw (new MorphoFormatException());
        }

        [M( O.AggressiveInlining )] public bool TryGetMorphoAttributePair( MorphoAttributeGroupEnum mag, MorphoAttributeEnum ma, ref MorphoAttributePair map ) => _Set.TryGetValue( mag, ma, ref map );
        [M( O.AggressiveInlining )] public MorphoAttributePair? TryGetMorphoAttributePair( MorphoAttributeGroupEnum mag, MorphoAttributeEnum ma )
        {
            MorphoAttributePair map = default;
            if ( _Set.TryGetValue( mag, ma, ref map ) )
            {
                return (map);
            }
            return (null);
        }
    }
}

