using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using TempFileStorage;

namespace TempFileStorage.SqlServer
{
    public class TempFileSqlStorage : ITempFileStorage
    {
        private readonly string _connectionString;

        public TempFileSqlStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<TempFile> StoreFile(string filename, Stream contentStream) => StoreFile(filename, contentStream, TimeSpan.FromMinutes(30));

        public async Task<TempFile> StoreFile(string filename, Stream contentStream, TimeSpan timeout)
        {
            var tempFile = new TempFile
            {
                Filename = filename,
                CacheTimeout = DateTime.Now.Add(timeout)
            };

            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                await using (var cmd = new SqlCommand("INSERT INTO [TempFileStorage] ([Key], Filename, FileSize, CacheTimeout, Content) VALUES (@key, @filename, @fileSize, @cacheTimeout, @content)", connection))
                {
                    cmd.CommandTimeout = 600;

                    cmd.Parameters.Add("@key", SqlDbType.NVarChar).Value = tempFile.Key;
                    cmd.Parameters.Add("@filename", SqlDbType.NVarChar).Value = filename;
                    cmd.Parameters.Add("@fileSize", SqlDbType.BigInt).Value = contentStream.Length;
                    cmd.Parameters.Add("@cacheTimeout", SqlDbType.DateTime).Value = tempFile.CacheTimeout;

                    // Add a parameter which uses the FileStream we just opened
                    // Size is set to -1 to indicate "MAX"
                    cmd.Parameters.Add("@content", SqlDbType.Binary, -1).Value = contentStream;

                    // Send the data to the server asynchronously  
                    await cmd.ExecuteNonQueryAsync();

                    tempFile.FileSize = contentStream.Length;
                }

                await CleanupStorage(connection);
            }

            return tempFile;
        }

        public async Task<bool> ContainsKey(string key)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var cmd = new SqlCommand("SELECT 1 FROM [TempFileStorage] WHERE [Key] = @key AND [CacheTimeout] > @timeout", connection);
            cmd.Parameters.AddWithValue("key", key);
            cmd.Parameters.AddWithValue("timeout", DateTime.Now);

            var count = (int)await cmd.ExecuteScalarAsync();

            await CleanupStorage(connection);

            return count == 1;
        }

        public async Task<TempFile> GetFileInfo(string key)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var cmd = new SqlCommand("SELECT [Filename], [FileSize], [CacheTimeout] FROM [TempFileStorage] WHERE [Key] = @key AND [CacheTimeout] > @timeout", connection);
            cmd.Parameters.AddWithValue("key", key);
            cmd.Parameters.AddWithValue("timeout", DateTime.Now);

            var reader = await cmd.ExecuteReaderAsync();

            TempFile tempFile = null;

            while (await reader.ReadAsync())
            {
                var filename = reader.GetString(0);
                var fileSize = reader.GetInt64(1);
                var cacheTimeout = reader.GetDateTime(2);

                tempFile = new TempFile(key)
                {
                    CacheTimeout = cacheTimeout,
                    Filename = filename,
                    FileSize = fileSize
                };
            }

            await reader.CloseAsync();
            await CleanupStorage(connection);

            return tempFile;
        }

        public async Task<byte[]> Download(string key)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand("SELECT [Content] FROM [TempFileStorage] WHERE [Key] = @key", connection)
            {
                CommandTimeout = 600
            };

            command.Parameters.AddWithValue("key", key);

            byte[] content = null;

            // The reader needs to be executed with the SequentialAccess behavior to enable network streaming  
            // Otherwise ReadAsync will buffer the entire BLOB into memory which can cause scalability issues or even OutOfMemoryExceptions  
            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
            if (await reader.ReadAsync())
            {
                content = (byte[])reader["Content"];
            }

            await reader.CloseAsync();
            await CleanupStorage(connection);

            return content;
        }

        private static async Task CleanupStorage(SqlConnection connection)
        {
            var command = new SqlCommand("DELETE FROM [TempFileStorage] WHERE [CacheTimeout] < @timeout", connection);
            command.Parameters.AddWithValue("timeout", DateTime.Now);

            await command.ExecuteNonQueryAsync();
        }
    }
}
