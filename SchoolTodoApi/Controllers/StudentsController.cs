using Microsoft.AspNetCore.Mvc;
using SchoolTodoApi.Models;
using SchoolTodoApi.Services;
using System.Collections.Generic;

namespace SchoolTodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentService _studentService;

        public StudentsController(StudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public ActionResult<List<StudentWithSchoolNameDto>> Get() =>
            _studentService.GetWithSchoolNames();

        [HttpGet("{id}", Name = "GetStudent")]
        public ActionResult<Student> Get(string id)
        {
            var student = _studentService.Get(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        [HttpPost]
        public ActionResult<Student> Create(Student student)
        {
            _studentService.Create(student);

            return CreatedAtRoute("GetStudent", new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
    public IActionResult Update(string id, Student studentIn)
{
    try
    {
        var student = _studentService.Get(id);
        if (student == null)
        {
            return NotFound();
        }

        studentIn.Id = id;
        _studentService.Update(id, studentIn);

        return NoContent();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating student: {ex.Message}");
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var student = _studentService.Get(id);

            if (student == null)
            {
                return NotFound();
            }

            _studentService.Remove(student.Id);

            return NoContent();
        }
    }
}