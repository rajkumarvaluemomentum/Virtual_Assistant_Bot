using VirtualAssistant.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure port for Render deployment
if (!builder.Environment.IsDevelopment())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Register GitHub service
builder.Services.AddHttpClient<GitHubService>();

var app = builder.Build();

// Use CORS early
app.UseCors("AllowAll");

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// HTTPS only in development
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
