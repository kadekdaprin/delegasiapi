using Dapper;
using DelegasiAPI.Models;
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

        public List<SampleModel> Get(string? nama)
        {
            using var conn = new SqlConnection(_connectionString);
            List<SampleModel> result;

            var query = "SELECT * FROM SampleTable";

            Dictionary<string, object> param = new();

            if (!string.IsNullOrEmpty(nama))
            {
                query += " WHERE Name LIKE @nama";

                param.Add("nama", $"%{nama}%");
            }

            result = conn.Query<SampleModel>(query, param).ToList();

            return result;
        }
    }
}
