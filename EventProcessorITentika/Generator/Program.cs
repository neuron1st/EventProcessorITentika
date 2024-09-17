using Generator.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<EventGeneratorService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<EventGeneratorService>());
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
app.MapControllers();

app.Run();
