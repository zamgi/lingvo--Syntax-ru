using System;

namespace lingvo.morphology
{
    /// <summary>
    /// Исключение при неверном формате словарей
    /// </summary>
    public sealed class MorphoFormatException : Exception
    {
        public MorphoFormatException(): base() { }
    }
}

