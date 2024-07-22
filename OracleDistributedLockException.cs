using System;

namespace Hangfire.Oracle
{
    public class OracleDistributedLockException : Exception
    {
        public OracleDistributedLockException(string message) : base(message)
        {
        }
    }
}
