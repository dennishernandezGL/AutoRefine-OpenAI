using Autofac.Extensions.DependencyInjection;
using DotNetEnv;
using Services.Helpers;
using Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load("../.env");

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(
    options =>
    {
        options.AddPolicy("AllowAllOrigins",
            builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    });
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Services.AddSingleton<PlaywrightService>();

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

//app.Urls.Add("http://*:80");
app.Run();