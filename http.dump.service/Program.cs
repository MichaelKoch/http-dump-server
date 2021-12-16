using http.dump.service.middleware;
using http.dump.service.repositories;
DumpRepository.BASE_DIRECTORY = @"c:\temp\dumps";


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton(new DumpRepository());
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseMiddleware<RequestDumpMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
