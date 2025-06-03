using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolTodoApi.Models;
using SchoolTodoApi.Repositories.Interfaces;

namespace SchoolTodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolsController : ControllerBase
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly IStudentRepository _studentRepository;

        public SchoolsController(ISchoolRepository schoolRepository, IStudentRepository studentRepository)
        {
            _schoolRepository = schoolRepository;
            _studentRepository = studentRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<School>>> Get()
        {
            var schools = await _schoolRepository.GetAllAsync();
            return Ok(schools.ToList());
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetSchool")]
        public async Task<ActionResult<School>> Get(string id)
        {
            var school = await _schoolRepository.GetByIdAsync(id);
            if (school == null)
            {
                return NotFound();
            }
            return school;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<School>> Create(School school)
        {
            await _schoolRepository.CreateAsync(school);
            return CreatedAtRoute("GetSchool", new { id = school.Id }, school);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, School schoolIn)
        {
            var school = await _schoolRepository.GetByIdAsync(id);
            if (school == null)
            {
                return NotFound();
            }

            schoolIn.Id = id;
            await _schoolRepository.UpdateAsync(id, schoolIn);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var school = await _schoolRepository.GetByIdAsync(id);
            if (school == null)
            {
                return NotFound();
            }

            // Delete associated students
            if (school.StudentIds != null && school.StudentIds.Any())
            {
                await _studentRepository.DeleteManyAsync(s => school.StudentIds.Contains(s.Id));
            }

            await _schoolRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}