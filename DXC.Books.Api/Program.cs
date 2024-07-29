using DXC.Books.Api.Exceptions;
using DXC.Books.Api.Validators;
using DXC.Books.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DXC.Books.Api;

public class Program
{
    public static void Main(params string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddDbContext<BooksDbContext>(options =>
        {
            options.UseSqlite(builder.Configuration.GetConnectionString("BooksDbContext"));
        });

        builder.Services.AddValidatorsFromAssemblyContaining<DXC.Books.Api.Program>();
        builder.Services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining<DXC.Books.Api.Program>();
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<BooksDbContext>()
                .Database
                .Migrate();
        }

// Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseExceptionHandler();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}