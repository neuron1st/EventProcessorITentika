using Context;
using Microsoft.EntityFrameworkCore;
using Processor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<ProcessorDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("ProcessorDb")));
builder.Services.AddSingleton<EventProcessorService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<EventProcessorService>());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
