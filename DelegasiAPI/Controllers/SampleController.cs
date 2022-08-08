using Dapper;
using DelegasiAPI.Exceptions;
using DelegasiAPI.Helpers;
using DelegasiAPI.Models;
using DelegasiAPI.Models.Validators;
using DelegasiAPI.Repositories;
using Mahas.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DelegasiAPI.Controllers
{
    [ApiKey]
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly SampleRepository _sampleRepository;

        public SampleController(SampleRepository sampleRepository)
        {
            _sampleRepository = sampleRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<SampleModel>>> GetAsync([FromQuery]PageFilter filter, string? nama = null)
        {
            throw new DefaultException("Text error log");

            var result = await _sampleRepository.GetAsync(nama, filter);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SampleModel>> GetAsync(int id)
        {
            var result = await _sampleRepository.GetAsync(id);

            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<SampleModel>> PostAsync([FromBody] SampleModel model)
        {
            var validationResult = new SampleModelValidator().Validate(model);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return BadRequest(errors);
            }

            var result = await _sampleRepository.InsertAsync(model);

            var uri = Url.Action("Get", new { id = result.Id });

            return Created(uri, result);
        }

        [HttpPut]
        public async Task<ActionResult> PutAsync(SampleModel model)
        {
            var validationResult = new SampleModelValidator().Validate(model);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return BadRequest(errors);
            }

            var result = await _sampleRepository.UpdateAsync(model);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await _sampleRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}