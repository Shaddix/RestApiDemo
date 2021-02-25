using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Bogus.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RestApiDemo.App.Features.Comments;
using RestApiDemo.App.Features.Topics;
using RestApiDemo.App.Features.Users;
using RestApiDemo.Domain;
using RestApiDemo.Persistence;
using RestApiDemo.WebApi.Middleware;
using RestApiDemo.WebApi.NSwag;
using RestApiDemo.WebApi.PatchRequests;

namespace RestApiDemo.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<TopicService>()
                .AddScoped<UserService>()
                .AddScoped<CommentService>();

            ConfigureDatabase(services);

            services.AddControllers();
            services.AddPatchRequest();


            services.AddSwagger(Configuration.GetSection("Swagger").Get<SwaggerOptions>());
            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo {Title = "RestApiDemo.App", Version = "v1"});
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            RunMigration(app.ApplicationServices);

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseSwagger(Configuration.GetSection("Swagger").Get<SwaggerOptions>());
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }


        protected virtual void ConfigureDatabase(IServiceCollection services)
        {
            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<MainDbContext>(
                    opt => opt.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                        builder => builder.EnableRetryOnFailure()),
                    contextLifetime: ServiceLifetime.Scoped,
                    optionsLifetime: ServiceLifetime.Singleton);
        }


        protected virtual void RunMigration(IServiceProvider container)
        {
            using var scope = container.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MainDbContext>();
            context.Database.Migrate();
            GenerateData(context).GetAwaiter().GetResult();
        }

        private async Task GenerateData(MainDbContext dbContext)
        {
            if (dbContext.Users.Count() < 10)
            {
                var userFaker = new Faker<User>()
                        .RuleFor(p => p.FirstName, f => f.Name.FirstName())
                        .RuleFor(p => p.LastName, f => f.Name.LastName())
                        .RuleFor(p => p.BirthDate, f => f.Person.DateOfBirth)
                        .RuleFor(p => p.ProfileImage, f => f.Image.PicsumUrl())
                    ;
                dbContext.Users.AddRange(userFaker.Generate(10));

                await dbContext.SaveChangesAsync();
            }

            if (dbContext.Topics.Count() < 100)
            {
                var users = await dbContext.Users.Select(x => x.Id).ToListAsync();
                var topicFaker = new Faker<Topic>()
                        .RuleFor(p => p.Text, f => f.Lorem.Paragraphs())
                        .RuleFor(p => p.Date, f => f.Date.Past())
                        .RuleFor(p => p.Category, f => f.Commerce.Categories(1).First())
                        .RuleFor(p => p.IsDraft, f => f.PickRandom(true, false))
                        .RuleFor(p => p.AuthorId, f => f.PickRandom(users))
                    ;
                var commentFaker = new Faker<Comment>()
                        .RuleFor(p => p.AuthorId, f => f.PickRandom(users))
                        .RuleFor(p => p.Text, f => f.Lorem.Sentences())
                        .RuleFor(p => p.Date, f => f.Date.Past())
                    ;
                for (int i = 0; i < 100; i++)
                {
                    var topic = topicFaker.Generate();
                    dbContext.Topics.Add(topic);

                    topic.Comments.AddRange(commentFaker.GenerateBetween(0, 36));
                }


                await dbContext.SaveChangesAsync();
            }
        }
    }
}