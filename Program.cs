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
 
// Add CORS policy
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
builder.Services.AddScoped<GitHubService>();

// Register Knowledge Source service
builder.Services.AddScoped<KnowledgeSourceService>();
 
var app = builder.Build();
 
// Use CORS
app.UseCors("AllowAll");
 
// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI();
 
// Only use HTTPS redirection in development
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
 
app.UseAuthorization();
 
// Add a simple root endpoint
app.MapGet("/", () => "Virtual Assistant API is running! Visit /swagger for API documentation.");
 
app.MapControllers();
 
app.Run();
