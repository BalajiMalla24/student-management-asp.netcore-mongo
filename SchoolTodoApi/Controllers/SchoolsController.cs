using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolTodoApi.Models;
using SchoolTodoApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolTodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolsController : ControllerBase
    {
        private readonly SchoolService _schoolService;

        public SchoolsController(SchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        [Authorize] 
        [HttpGet]
        public async Task<ActionResult<List<School>>> Get() =>
            await _schoolService.GetAsync();

        [Authorize] 
        [HttpGet("{id}", Name = "GetSchool")]
        public async Task<ActionResult<School>> Get(string id)
        {
            var school = await _schoolService.GetAsync(id);

            if (school == null)
            {
                return NotFound();
            }

            return school;
        }

        //Only allow admin to create schools
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<School>> Create(School school)
        {
            await _schoolService.CreateAsync(school);

            return CreatedAtRoute("GetSchool", new { id = school.Id }, school);
        }

        //Only allow admin to edit schools
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, School schoolIn)
        {
            var school = await _schoolService.GetAsync(id);

            if (school == null)
            {
                return NotFound();
            }
            schoolIn.Id = id;

            await _schoolService.UpdateAsync(id, schoolIn);

            return NoContent();
        }
     
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var school = await _schoolService.GetAsync(id);

            if (school == null)
            {
                return NotFound();
            }

            await _schoolService.RemoveAsync(id);

            return NoContent();
        }
    }
}