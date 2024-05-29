using MessageExchange.Server.Repositories;
using MessageExchange.Server.SQL;
using MessageExchange.Server.WebSocketsFolder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IRepository, MessageRepository>();
builder.Services.AddScoped<IExchanger, WebSocketExchanger>();
builder.Services.AddSingleton<ISQLExecutor, PostgreSQLExecutor>(provider =>
{
    var connectionString = builder.Configuration.GetSection("SQL").GetConnectionString("DefaultConnection");
    var dbName = builder.Configuration.GetSection("SQL").GetSection("DataBaseName").GetValue<string>("DefaultName");
    var sqlExequtor = new PostgreSQLExecutor(connectionString, dbName);
    sqlExequtor.CreateDatabase();
    sqlExequtor.CreateDefaultTables();
    return sqlExequtor;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
{
    builder.WithOrigins("https://localhost:5173")
           .AllowAnyMethod()
           .AllowAnyHeader();
    builder.WithOrigins("https://localhost:5172")
           .AllowAnyMethod()
           .AllowAnyHeader();
    builder.WithOrigins("https://localhost:5171")
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseWebSockets();
app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();