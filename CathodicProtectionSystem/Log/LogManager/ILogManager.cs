using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.Log
{
    public interface ILogManager
    {
        void Info(string message);
        void Error(string message);
        void FatalException(string message, Exception exception);
    }
}
