using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting Demo Application...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console()
        .ReadFrom.Configuration(ctx.Configuration));

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("EnableCORS", corsPolicyBuilder =>
        {
            corsPolicyBuilder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    builder.Services.AddDbContext<ApplicationDbContext>(opts =>
        opts.UseSqlServer(builder.Configuration["ConnectionStrings:Configuration"],
            options => options.MigrationsAssembly("Avocado")));

    builder.Services.AddServices(builder.Configuration);

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();

    var app = builder.Build();
    
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors("EnableCORS");
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.UseSerilogRequestLogging();
    app.ApplyMigrations();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception at Application");
}

finally
{
    Log.Information("Application Shut down complete");
    Log.CloseAndFlush();
}