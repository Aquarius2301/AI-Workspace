using AIWorkspace.Api.Extensions;
using AIWorkspace.Api.Helpers;
using AIWorkspace.Api.Middlewares;
using AIWorkspace.Api.Seeds;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddExtensions(builder.Configuration, builder.Environment);

var app = builder.Build();

ClaimHelper.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    if (args.Contains("--seed"))
    {
        await SeedData.Initialize(app.Services);
    }
}

app.UseAuthentication();

app.UseAuthorization();

// ActiveTrackingMiddleware must be placed
// after UseAuthorization to have access to the authenticated user
app.UseMiddleware<ActiveTrackingMiddleware>();

app.MapControllers();

await app.RunAsync();
