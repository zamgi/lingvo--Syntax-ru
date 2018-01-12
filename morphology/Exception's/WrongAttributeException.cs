using System;

namespace lingvo.morphology
{
    /// Исключение при неправильном атрибуте
    public sealed class WrongAttributeException : Exception
    {
        public WrongAttributeException(): base()
        {
        }
    }
}

