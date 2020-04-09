using System;

namespace PokerClient.Common
{
    public class ValidationException : Exception
    {
        public ValidationException(string message)
            : base(message)
        {
        }

        public static void ThrowIfTrue(bool condition, string errorMessage)
        {
            if (condition)
            {
                throw new ValidationException(errorMessage);
            }
        }
    }
}
