using System;

namespace lingvo.morphology
{
    /// Исключение при неверном формате словарей
    public sealed class MorphoFormatException : Exception
    {
        public MorphoFormatException(): base()
        {
        }
    }
}

