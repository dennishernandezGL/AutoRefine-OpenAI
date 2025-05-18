using Autofac.Extensions.DependencyInjection;
using DotNetEnv;
using Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load(".env");

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Services.AddSingleton<GithubService>()
    ;
/*builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Register services with Autofac
    containerBuilder.RegisterType<GreetingService>().As<IGreetingService>();
});*/

// Inject configuration file
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
// Register services here (DI container)
//builder.Services.AddScoped<IGreetingService, GreetingService>();
builder.Services.AddControllers();

// (Optional) Add Swagger
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Urls.Add("http://*:80");
app.Run();