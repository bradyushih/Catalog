using Catalog.Repositories;
using Catalog.Settings;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// MongoClient
// to json
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
var mongoDbSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();

builder.Services.AddSingleton<IMongoClient>(ServiceProvider => {
    return new MongoClient(mongoDbSettings.ConnectionString);
});

builder.Services.AddSingleton<IItemsRepository, MongoDbItemsRepository>();
builder.Services.AddControllers(options => {
    options.SuppressAsyncSuffixInActionNames = false;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddMongoDb(mongoDbSettings.ConnectionString, name: "mongodb", timeout: TimeSpan.FromSeconds(3));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
