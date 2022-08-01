using Dapper;
using DelegasiAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DelegasiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly string _connectionString;

        public SampleController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MainConnection");
        }

        [HttpGet]
        public ActionResult<List<SampleModel>> Get(string? nama = null)
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

            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<SampleModel> Get(int? id = null)
        {
            using var conn = new SqlConnection(_connectionString);

            var query = "SELECT TOP 1 * FROM SampleTable WHERE Id = @id";

            var result = conn.QueryFirstOrDefault<SampleModel>(query, new { id });

            if (result == null) return NotFound();

            return Ok(result);
        }
    }
}