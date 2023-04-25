namespace StudentEnrolmentManagement
{
    using DAL;
    using DAL.Models;
    using DALayer;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using System;
    using System.Security.Claims;
    using System.Text;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddTransient(typeof(BookStoreContext));
            services.AddTransient(typeof(StudentDataService));
            services.AddTransient(typeof(BookDataService));
            services.AddTransient(typeof(UserDataService));
            services.AddTransient(typeof(InvoiceDataService));
            services.AddTransient(typeof(UniversityDataService));
            services.AddTransient(typeof(BookAllocationDataService));
            services.AddTransient(typeof(AuthenticationDataService));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "myapp", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "jwt",
                    // Name="www-authenticate",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                {
                    new OpenApiSecurityScheme
                    {
                      Reference=new OpenApiReference
                    {
                      Type=ReferenceType.SecurityScheme,
                         Id="Bearer"
                     }
                },new String[]{ }
            }
        });

            });


            services.AddAuthentication(c =>
                {
                    c.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    c.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    c.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;



                }).AddJwtBearer(c =>
                {
                    c.SaveToken = true;

                    c.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidAudience = Configuration["JwtSettings:Issuer"],
                        ValidIssuer = Configuration["JwtSettings:Issuer"],
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Key"]))
                    };
                });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(sw => sw.SwaggerEndpoint("/swagger/v1/swagger.json", "StudentEnrolmentManagement"));
            app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=StudentWebApi}/{action=ShowAllStudents}/{id?}");
            });
        }
    }
}
