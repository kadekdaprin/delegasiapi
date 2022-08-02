using Dapper;
using DelegasiAPI.Models;
using DelegasiAPI.Repositories;
using Mahas.Components;
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
        private readonly SampleRepository _sampleRepository;

        public SampleController(IConfiguration configuration, SampleRepository sampleRepository)
        {
            _connectionString = configuration.GetConnectionString("MainConnection");
            _sampleRepository = sampleRepository;
        }

        [HttpGet]
        public ActionResult<List<SampleModel>> Get(string? nama = null)
        {
            var result = _sampleRepository.Get(nama);

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

        [HttpPost]
        public async Task<ActionResult<SampleModel>> PostAsync([FromBody] SampleModel model)
        {
            using var conn = new SqlConnection(_connectionString);

            await conn.OpenAsync();

            using var transaction = conn.BeginTransaction();

            model.Id = await conn.InsertAsync(model, true, transaction);

            await transaction.CommitAsync();

            var uri = Url.Action("Get", new { id = model.Id });

            return Created(uri, model);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> DeleteAsync(int id, SampleModel model)
        {
            using var conn = new SqlConnection(_connectionString);

            await conn.OpenAsync();

            var query = "SELECT TOP 1 * FROM SampleTable WHERE Id = @id";

            var dbModel = conn.QueryFirstOrDefault<SampleModel>(query, new { id });

            if (dbModel == null) return NotFound();

            dbModel.Name = model.Name;

            using var transaction = conn.BeginTransaction();

            await conn.UpdateAsync(dbModel, transaction);

            await transaction.CommitAsync();

            return Ok(dbModel);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);

            await conn.OpenAsync();

            var query = "SELECT TOP 1 * FROM SampleTable WHERE Id = @id";

            var model = conn.QueryFirstOrDefault<SampleModel>(query, new { id });

            if (model == null) return NotFound();

            using var transaction = conn.BeginTransaction();

            await conn.DeleteAsync(model, transaction);

            await transaction.CommitAsync();

            return NoContent();
        }
    }
}