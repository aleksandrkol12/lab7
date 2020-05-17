using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Text;
using lab7.JsonModels;

namespace lab7
{
    public class StudentController
    {
        public ApplicationContext db = new ApplicationContext(new DbContextOptions<ApplicationContext>());

        public async Task<string> AddStudentAsync(HttpContext context)
        {
            string response = "error";

            JsonStudent jsonStudent;

            using (StreamReader reader = new StreamReader(context.Request.Body))
            {
                jsonStudent = JsonSerializer.Deserialize<JsonStudent>(await reader.ReadToEndAsync());
            }

            if (jsonStudent.FirstName == null)
                response = "Field 'firstName' is empty";
            else if (jsonStudent.LastName == null)
                response = "Field 'lastName' is empty";
            else if (jsonStudent.GroupId == null)
                response = "Field 'groupId' is empty";
            else
            {
                Group group = await db.Groups.Where(p => p.Id == jsonStudent.GroupId).FirstOrDefaultAsync();

                if (group != null)
                {
                    Student student = new Student() { FirstName = jsonStudent.FirstName, LastName = jsonStudent.LastName, GroupId = jsonStudent.GroupId.Value, Group = group, CreatedAt = DateTime.Now };
                    await db.Students.AddAsync(student);
                    await db.SaveChangesAsync();
                    response = "ok";
                }
                else
                    response = "The group was not found";
            }

            return await Task.FromResult(response);
        }

        public async Task<string> GetStudentsAsync()
        {
            List<Student> students = await db.Students.AsNoTracking().ToListAsync();

            List<JsonStudent> jsonStudents = new List<JsonStudent>(students.Count);

            foreach (Student student in students)
            {
                JsonStudent jsonStudent = new JsonStudent()
                {
                    Id = student.Id,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    GroupId = student.GroupId,
                    CreatedAt = student.CreatedAt,
                    UpdatedAt = student.UpdatedAt
                };

                jsonStudents.Add(jsonStudent);
            }

            string json = JsonSerializer.Serialize<List<JsonStudent>>(jsonStudents);

            return await Task.FromResult(JsonPrettyPrint(json));
        }

        public async Task<string> GetStudentByIdAsync(HttpContext context)
        {
            int id;

            try
            {
                id = Int32.Parse(context.Request.Path.ToString().Substring(context.Request.Path.ToString().LastIndexOf('/') + 1));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(ex.Message);
            }

            Student student = await db.Students.Where(p => p.Id == id).AsNoTracking().FirstOrDefaultAsync();

            if (student == null)
                return await Task.FromResult($"The student with 'Id': {id} was not found");

            JsonStudent jsonStudent = new JsonStudent()
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                GroupId = student.GroupId,
                CreatedAt = student.CreatedAt,
                UpdatedAt = student.UpdatedAt
            };

            string json = JsonSerializer.Serialize<JsonStudent>(jsonStudent);

            return await Task.FromResult(JsonPrettyPrint(json));
        }

        public async Task<string> DeleteStudentByIdAsync(HttpContext context)
        {
            int id;

            try
            {
                id = Int32.Parse(context.Request.Path.ToString().Substring(context.Request.Path.ToString().LastIndexOf('/') + 1));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(ex.Message);
            }

            Student student = await db.Students.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (student == null)
                return await Task.FromResult($"The student with 'Id': {id} was not found");

            db.Students.Remove(student);
            await db.SaveChangesAsync();

            return await Task.FromResult("ok");
        }

        public async Task<string> UpdateStudentByIdAsync(HttpContext context)
        {
            int id;

            try
            {
                id = Int32.Parse(context.Request.Path.ToString().Substring(context.Request.Path.ToString().LastIndexOf('/') + 1));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(ex.Message);
            }

            Student student = await db.Students.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (student == null)
                return await Task.FromResult($"The student with 'Id': {id} was not found");

            JsonStudent jsonStudent;

            using (StreamReader reader = new StreamReader(context.Request.Body))
            {
                jsonStudent = JsonSerializer.Deserialize<JsonStudent>(await reader.ReadToEndAsync());
            }

            if (jsonStudent.FirstName != null)
                student.FirstName = jsonStudent.FirstName;

            if (jsonStudent.LastName != null)
                student.LastName = jsonStudent.LastName;

            if (jsonStudent.GroupId != null)
            {
                Group group = await db.Groups.Where(p => p.Id == jsonStudent.GroupId.Value).FirstOrDefaultAsync();

                if (group == null)
                    return await Task.FromResult($"The group with 'Id': {jsonStudent.GroupId} was not found");

                student.GroupId = group.Id;
                student.Group = group;
            }

            student.UpdatedAt = DateTime.Now;

            db.Students.Update(student);
            await db.SaveChangesAsync();

            return await Task.FromResult("ok");
        }

        public string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            int offset = 0;
            int indentLength = 3;

            foreach (char ch in json)
            {
                switch (ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\'':
                        if (quote) ignore = !ignore;
                        break;
                }

                if (quote)
                    sb.Append(ch);
                else
                {
                    switch (ch)
                    {
                        case '{':
                        case '[':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case '}':
                        case ']':
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', --offset * indentLength));
                            sb.Append(ch);
                            break;
                        case ',':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(ch);
                            sb.Append(' ');
                            break;
                        default:
                            if (ch != ' ') sb.Append(ch);
                            break;
                    }
                }
            }

            return sb.ToString().Trim();
        }
    }
}