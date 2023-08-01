using System;

namespace TH.Hangfire.Oracle
{
    public class OracleDistributedLockException : Exception
    {
        public OracleDistributedLockException(string message) : base(message)
        {
        }
    }
}
