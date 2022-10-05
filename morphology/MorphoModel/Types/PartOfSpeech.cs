using M = System.Runtime.CompilerServices.MethodImplAttribute;
using O = System.Runtime.CompilerServices.MethodImplOptions;

namespace lingvo.morphology
{
    /// Часть речи
    public abstract class PartOfSpeechBase
    {
        /// морфоатрибуты
        protected MorphoAttributeGroupEnum _MorphoAttributeGroup;

	    protected PartOfSpeechBase()
        {
            //_MorphoAttributeGroup = MorphoAttributeGroupEnum.__UNDEFINED__;
        }

	    /// получение названия части речи
        public abstract PartOfSpeechEnum PartOfSpeech { [M(O.AggressiveInlining)] get; }

        /// получение типов атрибутов
        public MorphoAttributeGroupEnum MorphoAttributeGroup { [M(O.AggressiveInlining)] get => _MorphoAttributeGroup; }

        public override string ToString() => $"[{PartOfSpeech}, {MorphoAttributeGroup}]";
    }

    /// Существительное
    internal class Noun : PartOfSpeechBase
    {
        public Noun()
	    {
            _MorphoAttributeGroup |= MorphoAttributeGroupEnum.Person      |
                                     MorphoAttributeGroupEnum.Case        |
                                     MorphoAttributeGroupEnum.Number      |
                                     MorphoAttributeGroupEnum.Gender      |
                                     MorphoAttributeGroupEnum.Animateness |
                                     MorphoAttributeGroupEnum.NounType;
	    }

	    public override PartOfSpeechEnum PartOfSpeech => PartOfSpeechEnum.Noun;
    }

    /// Прилагательное
    internal sealed class Adjective : PartOfSpeechBase
    {
        public Adjective()
	    {
            _MorphoAttributeGroup |= MorphoAttributeGroupEnum.Person |
                                     MorphoAttributeGroupEnum.Case |
                                     MorphoAttributeGroupEnum.Number |
                                     MorphoAttributeGroupEnum.Gender |
                                     MorphoAttributeGroupEnum.Animateness |
                                     MorphoAttributeGroupEnum.AdjectForm | 
                                     MorphoAttributeGroupEnum.DegreeOfComparison;
	    }

        public override PartOfSpeechEnum PartOfSpeech => PartOfSpeechEnum.Adjective;
    }

    /// Местоимение
    internal sealed class Pronoun : PartOfSpeechBase
    {
        public Pronoun()
        {
            _MorphoAttributeGroup |= MorphoAttributeGroupEnum.Person |
                                     MorphoAttributeGroupEnum.Case |
                                     MorphoAttributeGroupEnum.Number |
                                     MorphoAttributeGroupEnum.Gender |
                                     MorphoAttributeGroupEnum.Animateness |
                                     MorphoAttributeGroupEnum.NounType |
                                     MorphoAttributeGroupEnum.AdjectForm |
                                     MorphoAttributeGroupEnum.PronounType;
        }

        public override PartOfSpeechEnum PartOfSpeech => PartOfSpeechEnum.Pronoun;
    }

    /// Числительное
    internal sealed class Numeral : PartOfSpeechBase
    {
        public Numeral()
	    {
            _MorphoAttributeGroup |= MorphoAttributeGroupEnum.Case |
                                     MorphoAttributeGroupEnum.Number |
                                     MorphoAttributeGroupEnum.Gender |
                                     MorphoAttributeGroupEnum.Animateness |
                                     MorphoAttributeGroupEnum.NumeralType;
	    }

        public override PartOfSpeechEnum PartOfSpeech => PartOfSpeechEnum.Numeral;
    }

    internal abstract class Verbs : PartOfSpeechBase
    {
	    protected Verbs()
	    {
            _MorphoAttributeGroup |= MorphoAttributeGroupEnum.Person |
                                     MorphoAttributeGroupEnum.Case |
                                     MorphoAttributeGroupEnum.Number |
                                     MorphoAttributeGroupEnum.Gender |
                                     MorphoAttributeGroupEnum.Animateness |
                                     MorphoAttributeGroupEnum.Tense |
                                     MorphoAttributeGroupEnum.Mood |
                                     MorphoAttributeGroupEnum.Voice |
                                     MorphoAttributeGroupEnum.VerbTransitivity |
                                     MorphoAttributeGroupEnum.VerbForm |
                                     MorphoAttributeGroupEnum.AdjectForm |
                                     MorphoAttributeGroupEnum.VerbType;
	    }
    }

    /// Глагол
    internal sealed class Verb : Verbs
    {
        public Verb() { }

        public override PartOfSpeechEnum PartOfSpeech => PartOfSpeechEnum.Verb;
    }

    /// Наречие
    internal sealed class Adverb : PartOfSpeechBase
    {
        public Adverb()
	    {
            _MorphoAttributeGroup |= MorphoAttributeGroupEnum.DegreeOfComparison |
                                     MorphoAttributeGroupEnum.AdverbType;
	    }

        public override PartOfSpeechEnum PartOfSpeech => PartOfSpeechEnum.Adverb;
    }

    /// Союз
    internal sealed class Conjunction : PartOfSpeechBase
    {
	    public Conjunction() => _MorphoAttributeGroup |= MorphoAttributeGroupEnum.ConjunctionType;

        public override PartOfSpeechEnum PartOfSpeech => PartOfSpeechEnum.Conjunction;
    }

    /// Предлог
    internal sealed class Preposition : PartOfSpeechBase
    {
        public Preposition() => _MorphoAttributeGroup |= MorphoAttributeGroupEnum.Case;

        public override PartOfSpeechEnum PartOfSpeech => PartOfSpeechEnum.Preposition;
    }

    /// Междометие
    internal sealed class Interjection : PartOfSpeechBase
    {
        public Interjection() { }

        public override PartOfSpeechEnum PartOfSpeech => PartOfSpeechEnum.Interjection;
    }

    /// Частица
    internal sealed class Particle : PartOfSpeechBase
    {
        public Particle() { }

        public override PartOfSpeechEnum PartOfSpeech => PartOfSpeechEnum.Particle;
    }

    /// Артикль
    internal sealed class Article : PartOfSpeechBase
    {
        public Article() => _MorphoAttributeGroup |= MorphoAttributeGroupEnum.ArticleType;

        public override PartOfSpeechEnum PartOfSpeech => PartOfSpeechEnum.Article;
    }

    /// Иная часть речи
    internal sealed class Other : PartOfSpeechBase
    {
        public Other() { }

        public override PartOfSpeechEnum PartOfSpeech => PartOfSpeechEnum.Other;
    }

    /// Предикатив
    internal sealed class Predicate : Verbs
    {
        public Predicate() { }

        public override PartOfSpeechEnum PartOfSpeech => PartOfSpeechEnum.Predicate;
    }
}
