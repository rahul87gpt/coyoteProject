using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Coyote.Console.Common.Exceptions
{
    [Serializable]
    public sealed class BadRequestException : Exception
    {
        public BadRequestException()
        {
        }
        public BadRequestException(string message)
            : base(message)
        {
        }
        public BadRequestException(string message, string property)
        : base(message)
        {
            if (!string.IsNullOrEmpty(property))
            {
                Data.Add(ErrorMessages.ValidationErrorKey, property);
            }
        }
        public BadRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        [ExcludeFromCodeCoverage]
        private BadRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    [Serializable]
    public sealed class NotFoundException : Exception
    {
        public NotFoundException()
        {
        }
        public NotFoundException(string message)
            : base(message)
        {
        }
        public NotFoundException(string message, string property)
        : base(message)
        {
            if (!string.IsNullOrEmpty(property))
            {
                Data.Add(ErrorMessages.ValidationErrorKey, property);
            }
        }
        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        [ExcludeFromCodeCoverage]
        private NotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public sealed class AlreadyExistsException : Exception
    {
        public AlreadyExistsException()
        {
        }
        public AlreadyExistsException(string message)
            : base(message)
        {
        }
        public AlreadyExistsException(string message, string property)
        : base(message)
        {
            if (!string.IsNullOrEmpty(property))
            {
                Data.Add(ErrorMessages.ValidationErrorKey, property);
            }
        }
        public AlreadyExistsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        [ExcludeFromCodeCoverage]
        private AlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
    [Serializable]
    public sealed class UnauthorizedException : Exception
    {
        public UnauthorizedException()
        {
        }
        public UnauthorizedException(string message)
            : base(message)
        {
        }
        public UnauthorizedException(string message, string property)
        : base(message)
        {
            if (!string.IsNullOrEmpty(property))
            {
                Data.Add(ErrorMessages.ValidationErrorKey, property);
            }
        }
        public UnauthorizedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        [ExcludeFromCodeCoverage]
        private UnauthorizedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
    [Serializable]
    public sealed class NullReferenceCustomException : Exception
    {
        public NullReferenceCustomException()
        {
        }
        public NullReferenceCustomException(string message)
            : base(message)
        {
        }
        public NullReferenceCustomException(string message, string property)
        : base(message)
        {
            if (!string.IsNullOrEmpty(property))
            {
                Data.Add(ErrorMessages.ValidationErrorKey, property);
            }
        }
        public NullReferenceCustomException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        [ExcludeFromCodeCoverage]
        private NullReferenceCustomException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
    [Serializable]
    public sealed class NotDeleteException : Exception
    {
        public NotDeleteException()
        {
        }
        public NotDeleteException(string message)
            : base(message)
        {
        }
        public NotDeleteException(string message, string property)
        : base(message)
        {
            if (!string.IsNullOrEmpty(property))
            {
                Data.Add(ErrorMessages.ValidationErrorKey, property);
            }
        }
        public NotDeleteException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        [ExcludeFromCodeCoverage]
        private NotDeleteException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
    [Serializable]
    public sealed class NotModifiedException : Exception
    {
        public NotModifiedException()
        {
        }
        public NotModifiedException(string message)
            : base(message)
        {
        }
        public NotModifiedException(string message, string property)
        : base(message)
        {
            if (!string.IsNullOrEmpty(property))
            {
                Data.Add(ErrorMessages.ValidationErrorKey, property);
            }
        }
        public NotModifiedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        [ExcludeFromCodeCoverage]
        private NotModifiedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
