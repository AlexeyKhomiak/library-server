using library;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddHttpLogging(option =>
{
    option.LoggingFields = HttpLoggingFields.RequestMethod | HttpLoggingFields.RequestHeaders |
                            HttpLoggingFields.RequestQuery | HttpLoggingFields.RequestBody;

});

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
            new BadRequestObjectResult(context.ModelState)
            {
                ContentTypes =
                {
                    Application.Json
                }
            };
    })
    .AddXmlSerializerFormatters();

builder.Services.AddDbContext<LibraryContext>(option => option.UseInMemoryDatabase("LibraryDb"));

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers();


var app = builder.Build();

app.UseCors(options =>
    options.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader()
);

app.UseHttpLogging();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStatusCodePages();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
