using System;

namespace SchoolTodoApi.Models
{
    // Remove the duplicate interface definition from this file
    // and keep only the class implementation
    public class SchoolDatabaseSettings : ISchoolDatabaseSettings
    {
        public string UsersCollectionName { get; set; } = null!;
        public string StudentsCollectionName { get; set; } = string.Empty;
        public string SchoolsCollectionName { get; set; } = string.Empty;
        public string TodoItemsCollectionName { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}