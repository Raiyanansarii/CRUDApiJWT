using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace CRUDApiJWT.Data
{
    public class DbHelper
    {
        private readonly string _connStr;

        public DbHelper(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection");
        }

        public async Task<int> ExecuteNonQueryAsync(string spName, params NpgsqlParameter[] parameters)
        {
            await using var conn = new NpgsqlConnection(_connStr);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(spName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddRange(parameters);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<T>> ExecuteFunctionAsync<T>(string funcName, Func<IDataReader, T> mapFunc)
        {
            var result = new List<T>();

            await using var conn = new NpgsqlConnection(_connStr);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand($"SELECT * FROM {funcName}()", conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(mapFunc(reader));
            }

            return result;
        }


        public async Task<T?> ExecuteRawItemAsync<T>(string sql, Func<IDataReader, T> mapFunc, params NpgsqlParameter[] parameters)
        {
            await using var conn = new NpgsqlConnection(_connStr);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return mapFunc(reader);
            }

            return default;
        }

        //public async Task<T?> ExecuteItemAsync<T>(string spName, Func<IDataReader, T> mapFunc, params NpgsqlParameter[] parameters)
        //{
        //    await using var conn = new NpgsqlConnection(_connStr);
        //    await conn.OpenAsync();

        //    await using var cmd = new NpgsqlCommand(spName, conn)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };
        //    cmd.Parameters.AddRange(parameters);

        //    await using var reader = await cmd.ExecuteReaderAsync();
        //    if (await reader.ReadAsync())
        //    {
        //        return mapFunc(reader);
        //    }

        //    return default;
        //}

        //public async Task<List<T>> ExecuteListAsync<T>(string spName, Func<IDataReader, T> mapFunc, params NpgsqlParameter[] parameters)
        //{
        //    var results = new List<T>();

        //    await using var conn = new NpgsqlConnection(_connStr);
        //    await conn.OpenAsync();

        //    await using var cmd = new NpgsqlCommand(spName, conn)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };
        //    cmd.Parameters.AddRange(parameters);

        //    await using var reader = await cmd.ExecuteReaderAsync();
        //    while (await reader.ReadAsync())
        //    {
        //        results.Add(mapFunc(reader));
        //    }

        //    return results;
        //}
    }
}
