using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SchoolTodoApi.Models{




public class User{
   [BsonId]
   [BsonRepresentation(BsonType.ObjectId)]
   public string Id{get; set;} =null!;

    [BsonElement("username")]
   public string Username{get; set;} = null!;

     [BsonElement("password")]
   public string Password{get; set;} = null!;

     [BsonElement("role")]
   public string Role {get; set;} = null!; //=Student; //Default

   //   [BsonElement("studentid")]
   public string? Email{get; set;} 
                       
    

}
} 