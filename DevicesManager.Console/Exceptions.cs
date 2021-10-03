using System;
namespace DevicesManager.Console
{
    public class OperationExitException : Exception
    {
        public OperationExitException() { }

        public OperationExitException(string msg) : base(msg)
        {
        }
    }
}
