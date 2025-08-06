using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using PovVoyage.Services.Interfaces;
using PovVoyage.Services.Implementations;
using PovVoyage.Data.Context;
using PovVoyage.Data.Repositories;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Serilog;
using Serilog.Extensions.Hosting;

var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "app-log.txt");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .CreateLogger();
Console.WriteLine($"Log file path: {logFilePath}");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
                 .ReadFrom.Services(services));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddSerilog(); 

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddLogging(logging =>
{
    logging.AddApplicationInsights(
        builder.Configuration["ApplicationInsights:InstrumentationKey"] ?? string.Empty,
        options => { options.TrackExceptionsAsExceptionTelemetry = true; }
    );
});

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// Register data context
builder.Services.AddDbContext<PovVoyageContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

// Register repositories if using repository pattern
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddScoped<IUserInteractionService, UserInteractionService>();

// Application Insights
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["ApplicationInsights:InstrumentationKey"]);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 524288000; // 500MB limit
});
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 524288000; // 500MB limit
});
//builder.Services.AddSwaggerGen();
var app = builder.Build();


// Swagger
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

// Global exception handler
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error != null)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(exceptionHandlerPathFeature.Error,
                "Unhandled Exception: {Message}",
                exceptionHandlerPathFeature.Error.Message);

            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("An unexpected error occurred.");
        }
    });
});

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseAuthorization();
app.UseRouting();
app.MapControllers();

app.UseSerilogRequestLogging();

app.Run();
