using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

using Dapper;

using Hangfire.Logging;

namespace TH.Hangfire.Oracle
{
    public static class OracleObjectsInstaller
    {
        private static readonly ILog Log = LogProvider.GetLogger(typeof(OracleStorage));
        public static void Install(IDbConnection connection, string schemaName)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            var tablesExist = TablesExists(connection, schemaName);
            var indexExists = IndexExists(connection, schemaName);
            if (tablesExist && indexExists)
            {
                Log.Info("DB tables and indexes already exist. Exit install");
                return;
            }
            if (!tablesExist)
            {
                Log.Info("Start installing Hangfire SQL objects...");

                var script = GetStringResource("Hangfire.Oracle.Install.sql");

                var sqlCommands = script.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                sqlCommands.ToList().ForEach(s => connection.Execute(s));

                Log.Info("Hangfire SQL objects installed.");
            }

            if (!indexExists)
            {
                Log.Info("Start installing Hangfire Indexes...");

                var script = GetStringResource("Hangfire.Oracle.InstallIndexes.sql");

                var sqlCommands = script.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                sqlCommands.ToList().ForEach(s => connection.Execute(s));

                Log.Info("Hangfire Indexes installed.");
            }
        }

        private static bool TablesExists(IDbConnection connection, string schemaName)
        {
            string tableExistsQuery;

            if (!string.IsNullOrEmpty(schemaName))
            {
                tableExistsQuery = $@"SELECT TABLE_NAME FROM all_tables WHERE OWNER = '{schemaName}' AND TABLE_NAME LIKE 'HF_%'";
            }
            else
            {
                tableExistsQuery = @"SELECT TABLE_NAME FROM all_tables WHERE TABLE_NAME LIKE 'HF_%'";
            }

            return connection.ExecuteScalar<string>(tableExistsQuery) != null;
        }

        private static bool IndexExists(IDbConnection connection, string schemaName)
        {
            string indexExistsQuery;

            if (!string.IsNullOrEmpty(schemaName))
            {
                indexExistsQuery = $@"SELECT index_name FROM all_indexes WHERE OWNER = '{schemaName}' AND INDEX_NAME = 'HF_JOB_STATEID'";
            }
            else
            {
                indexExistsQuery = @"SELECT index_name FROM all_indexes WHERE INDEX_NAME = 'HF_JOB_STATEID'";
            }

            return connection.ExecuteScalar<string>(indexExistsQuery) != null;
        }

        private static string GetStringResource(string resourceName)
        {
#if NET45
            var assembly = typeof(OracleObjectsInstaller).Assembly;
#else
            var assembly = typeof(OracleObjectsInstaller).GetTypeInfo().Assembly;
#endif

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Requested resource `{resourceName}` was not found in the assembly `{assembly}`.");
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
