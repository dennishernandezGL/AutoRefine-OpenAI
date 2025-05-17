using Autofac.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
// Add Autofac as the DI container
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
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
    //app.MapOpenApi();
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();