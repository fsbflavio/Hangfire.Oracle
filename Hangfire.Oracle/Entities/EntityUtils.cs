﻿using System.Data;

using Dapper;

namespace Hangfire.Oracle.Core.Entities
{
    public static class EntityUtils
    {
        public static long GetNextId(this IDbConnection connection)
        {
            return connection.QuerySingle<long>("SELECT MISP.HIBERNATE_SEQUENCE.NEXTVAL FROM dual");
        }
    }
}