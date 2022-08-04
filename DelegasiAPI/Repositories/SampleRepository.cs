using Dapper;
using DelegasiAPI.Models;
using Mahas.Components;
using System.Data.SqlClient;

namespace DelegasiAPI.Repositories
{
    public class SampleRepository
    {
        private readonly string _connectionString;

        public SampleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MainConnection");
        }

        public async Task<List<SampleModel>> GetAsync(string? nama)
        {
            using var conn = new SqlConnection(_connectionString);

            var query = "SELECT * FROM SampleTable";

            Dictionary<string, object> param = new();

            if (!string.IsNullOrEmpty(nama))
            {
                query += " WHERE Name LIKE @nama";

                param.Add("nama", $"%{nama}%");
            }

            var result = await conn.QueryAsync<SampleModel>(query, param);

            return result.ToList();
        }

        public async Task<SampleModel> GetAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);

            var query = "SELECT TOP 1 * FROM SampleTable WHERE Id = @id";

            var result = await conn.QueryFirstOrDefaultAsync<SampleModel>(query, new { id });

            return result;
        }

        public async Task<SampleModel> InsertAsync(SampleModel model)
        {
            using var conn = new SqlConnection(_connectionString);

            await conn.OpenAsync();

            using var transaction = conn.BeginTransaction();

            model.Id = await conn.InsertAsync(model, true, transaction);

            await transaction.CommitAsync();

            return model;
        }

        public async Task<SampleModel> UpdateAsync(SampleModel model)
        {
            using var conn = new SqlConnection(_connectionString);

            await conn.OpenAsync();

            using var transaction = conn.BeginTransaction();

            await conn.UpdateAsync(model, transaction);

            await transaction.CommitAsync();

            return model;
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);

            await conn.OpenAsync();

            var query = "SELECT TOP 1 * FROM SampleTable WHERE Id = @id";

            var model = conn.QueryFirstOrDefault<SampleModel>(query, new { id });

            using var transaction = conn.BeginTransaction();

            await conn.DeleteAsync(model, transaction);

            await transaction.CommitAsync();
        }
    }
}
