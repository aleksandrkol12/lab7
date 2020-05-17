using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace lab7
{
    public class Startup
    {
        private GroupController groupController = new GroupController();
        private StudentController studentController = new StudentController();

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapGet("/groups", async context =>
                {
                    await context.Response.WriteAsync(await groupController.GetGroupsAsync());
                });

                endpoints.MapPost("/groups", async context =>
                {
                    await context.Response.WriteAsync(await groupController.AddGroupAsync(context));
                });

                endpoints.MapDelete("/groups/{id}", async context =>
                {
                    await context.Response.WriteAsync(await groupController.DeleteGroupAsync(context));
                });

                endpoints.MapPost("/students", async context =>
                {
                    await context.Response.WriteAsync(await studentController.AddStudentAsync(context));
                });

                endpoints.MapGet("/students", async context =>
                {
                    await context.Response.WriteAsync(await studentController.GetStudentsAsync());
                });

                endpoints.MapGet("/students/{id}", async context =>
                {
                    await context.Response.WriteAsync(await studentController.GetStudentByIdAsync(context));
                });

                endpoints.MapDelete("/students/{id}", async context =>
                {
                    await context.Response.WriteAsync(await studentController.DeleteStudentByIdAsync(context));
                });

                endpoints.MapPut("/students/{id}", async context =>
                {
                    await context.Response.WriteAsync(await studentController.UpdateStudentByIdAsync(context));
                });
            });
        }
    }
}