using Microsoft.AspNetCore.Mvc;
using SchoolTodoApi.Models;
using SchoolTodoApi.Repositories.Interfaces;

namespace SchoolTodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;

        public StudentsController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<StudentWithSchoolNameDto>>> Get()
        {
            var students = await _studentRepository.GetWithSchoolNamesAsync();
            return Ok(students);
        }

        [HttpGet("{id}", Name = "GetStudent")]
        public async Task<ActionResult<Student>> Get(string id)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(id);

                if (student == null)
                {
                    return NotFound($"No student found with ID: {id}");
                }

                return Ok(student);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching the student: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Student>> Create(Student student)
        {
            try
            {
                await _studentRepository.CreateAsync(student);
                return CreatedAtRoute("GetStudent", new { id = student.Id }, student);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the student: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Student studentIn)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(id);
                if (student == null)
                {
                    return NotFound($"No student found with ID: {id}");
                }

                studentIn.Id = id;
                await _studentRepository.UpdateAsync(id, studentIn);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the student: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(id);
                if (student == null)
                {
                    return NotFound($"No student found with ID: {id}");
                }

                await _studentRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the student: {ex.Message}");
            }
        }
    }
}