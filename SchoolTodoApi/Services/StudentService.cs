using MongoDB.Driver;
using SchoolTodoApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace SchoolTodoApi.Services
{
    public class StudentService
    {
        private readonly IMongoCollection<Student> _students;
          private readonly IMongoCollection<School> _schools;

        public StudentService(ISchoolDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _students = database.GetCollection<Student>(settings.StudentsCollectionName);
            _schools = database.GetCollection<School>(settings.SchoolsCollectionName);
        }

        public List<Student> Get() =>
            _students.Find(student => true).ToList();

        public Student Get(string id) =>
            _students.Find(student => student.Id == id).FirstOrDefault();


        public List<StudentWithSchoolNameDto> GetWithSchoolNames()
    {
        var students = _students.Find(student => true).ToList();
        var schools = _schools.Find(s => true).ToList();

        var result = students.Select(s =>
        {
            var schoolName = schools.FirstOrDefault(sc => sc.Id == s.SchoolId)?.Name;
            return new StudentWithSchoolNameDto
            {
                Id = s.Id,
                Name = s.Name,
                Age = s.Age,
                Grade = s.Grade,
                SchoolId = s.SchoolId,
                SchoolName = schoolName
            };
        }).ToList();

        return result;
    }
        public Student Create(Student student)
        {
            _students.InsertOne(student);
            return student;
        }

        public void Update(string id, Student studentIn) =>
            _students.ReplaceOne(student => student.Id == id, studentIn);

        public void Remove(Student studentIn) =>
            _students.DeleteOne(student => student.Id == studentIn.Id);

        public void Remove(string id) =>
            _students.DeleteOne(student => student.Id == id);
    }
}