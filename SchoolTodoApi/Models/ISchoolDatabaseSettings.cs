namespace SchoolTodoApi.Models
{
    public interface ISchoolDatabaseSettings
    {
         string UsersCollectionName { get; set; } 
        string StudentsCollectionName { get; set; }
        string SchoolsCollectionName { get; set; }
        string TodoItemsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}