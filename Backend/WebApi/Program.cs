using WebApi.Helpers;
using WebApi.Middlewares;
using WebApi.Seeds;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

// Initialize ClaimHelper with IHttpContextAccessor after the app is built
ClaimHelper.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RefreshTokenCleanupMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Seed data on startup (only in Development to avoid accidental data loss)
    SeedData.Initialize(app.Services);
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
