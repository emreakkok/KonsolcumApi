﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Exceptions
{
    public class UserCreateFailedException : Exception
    {
        public UserCreateFailedException(): base("Kullanıcı oluştururken beklenmeyen bir hatayla karşılaşıldı")
        {
        }

        public UserCreateFailedException(string? message) : base(message)
        {
        }

        public UserCreateFailedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
