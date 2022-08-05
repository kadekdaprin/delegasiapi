using Dapper;
using DelegasiAPI.Exceptions;
using DelegasiAPI.Models;
using Mahas.Components;
using System.Data.SqlClient;

namespace DelegasiAPI.Repositories
{
    public class SampleRepository: BaseRepository
    {
        public SampleRepository(IConfiguration configuration) : base(configuration, "MainConnection")
        {
        }

        public async Task<PageResult<SampleModel>> GetAsync(string? nama, PageFilter filter)
        {
            Dictionary<string, object> param = new();
            List<string> where = new();

            if (!string.IsNullOrEmpty(nama))
            {
                where.Add("Name LIKE @nama");
                param.Add("nama", $"%{nama}%");
            }

            var query = $"SELECT * FROM SampleTable {ToWhere(where)}";

            var result = await GetPaginationAsync<SampleModel>(query, "Name", OrderByType.ASC, filter, param);

            return result;
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

            var query = "SELECT TOP 1 * FROM SampleTable WHERE Id = @id";

            var dbModel = conn.QueryFirstOrDefault<SampleModel>(query, new { model.Id });

            if (dbModel == null) throw new DefaultException("Data tidak ditemukan", model);

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

            if (model == null) throw new DefaultException("Data tidak ditemukan", model);

            using var transaction = conn.BeginTransaction();

            await conn.DeleteAsync(model, transaction);

            await transaction.CommitAsync();
        }
    }
}
