using System;

namespace lingvo.morphology
{
    /// <summary>
    /// Исключение при неправильном атрибуте
    /// </summary>
    public sealed class WrongAttributeException : Exception
    {
        public WrongAttributeException(): base() { }
    }
}

