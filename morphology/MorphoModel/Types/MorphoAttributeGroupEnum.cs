using System;

namespace lingvo.morphology
{
    /// <summary>
    /// Тип/группа морфоатрибута
    /// </summary>
    [Flags] public enum MorphoAttributeGroupEnum : uint
    {
        __UNDEFINED__      = 0x0U,

        /// Лицо
        Person             = 0x1U,
        /// Падеж
        Case               = (1U << 1),
        /// Число
        Number             = (1U << 2),
        /// Род
        Gender             = (1U << 3),
        /// Одушевленность
        Animateness        = (1U << 4),
        /// Тип существительного
        NounType           = (1U << 5),
        /// Время
        Tense              = (1U << 6),
        /// Наклонение
        Mood               = (1U << 7),
        /// Залог
        Voice              = (1U << 8),
        /// Переходность глагола
        VerbTransitivity   = (1U << 9),
        /// Форма глагола
        VerbForm           = (1U << 10),
        /// Тип числительного
        NumeralType        = (1U << 11),
        /// Форма прилагательного
        AdjectForm         = (1U << 12),
        /// Степень сравнения
        DegreeOfComparison = (1U << 13),
        /// Тип союза
        ConjunctionType    = (1U << 14),
        /// Тип местоимения
        PronounType        = (1U << 15),
        /// Тип артикля
        ArticleType        = (1U << 16),
        /// Тип глагола
        VerbType           = (1U << 17),
        /// Тип наречия
        AdverbType         = (1U << 18),
    }
}

