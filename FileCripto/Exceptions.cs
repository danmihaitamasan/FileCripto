﻿using System;

namespace FileCrypto
{
    public class NotFoundErrorException : Exception
    {
        public NotFoundErrorException(string message) : base(message)
        {
        }

        public NotFoundErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NotFoundErrorException()
        {
        }
    }

}