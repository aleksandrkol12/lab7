using Microsoft.AspNetCore.Http;
using System;
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
    public class GroupController
    {
        public ApplicationContext db = new ApplicationContext(new DbContextOptions<ApplicationContext>());

        public async Task<string> GetGroupsAsync()
        {
            List<Group> groups = await db.Groups
                .Include(p => p.Students)
                .AsNoTracking()
                .ToListAsync();

            List<JsonGroup> jsonGroups = new List<JsonGroup>();

            foreach (Group group in groups)
            {
                JsonGroup jsonGroup = new JsonGroup()
                {
                    Id = group.Id,
                    Name = group.Name,
                    Students = new List<JsonStudent>(group.Students.Count)
                };

                foreach (Student student in group.Students)
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

                    jsonGroup.Students.Add(jsonStudent);
                }

                jsonGroups.Add(jsonGroup);
            }

            string json = JsonSerializer.Serialize<List<JsonGroup>>(jsonGroups);

            return await Task.FromResult(JsonPrettyPrint(json));
        }

        public async Task<string> AddGroupAsync(HttpContext context)
        {
            JsonGroup jsonGroup;

            string response = "error";

            using (StreamReader reader = new StreamReader(context.Request.Body))
            {
                jsonGroup = JsonSerializer.Deserialize<JsonGroup>(await reader.ReadToEndAsync());
            }

            if (jsonGroup.Name == null)
                return await Task.FromResult(response);

            Group group = await db.Groups.Where(p => p.Name == jsonGroup.Name).FirstOrDefaultAsync();

            if (group == null)
            {
                group = new Group();
                group.Name = jsonGroup.Name;
                await db.Groups.AddAsync(group);
                await db.SaveChangesAsync();
                response = "ok";
            }
            else
                response = "Group is exists";

            return await Task.FromResult(response);
        }

        public async Task<string> DeleteGroupAsync(HttpContext context)
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

            string response = "error";

            Group group = await db.Groups.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (group == null)
                response = "Group is not found";
            else
            {
                db.Groups.Remove(group);
                await db.SaveChangesAsync();
                response = "ok";
            }

            return await Task.FromResult(response);
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