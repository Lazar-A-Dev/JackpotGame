using Grpc.Net.Client;
using GrpcServer;
using GrpcServer.Data;
using GrpcServer.Models;
using GrpcServer.ServerRmq;
using GrpcServer.Services;
using Jackpot.Domain.Server;
using Jackpot.Infra.RabbitConnection;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data source = GrpcDatabase.db"));

// Add services to the container.
builder.Services.AddGrpc().AddJsonTranscoding();


var app = builder.Build();
//ServerRmq serverRmq = new ServerRmq();

// Configure the HTTP request pipeline.
app.MapGrpcService<ToDoService>();

var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite("Data Source=GrpcDatabase.db")  // SQLite connection string
    .Options;

AppDbContext dbContext = new AppDbContext(dbContextOptions);

// RabbitMQ connection
RabbitConnection rc = new RabbitConnection();
IConnection connection = rc.Connection;
IModel channel = connection.CreateModel();

// Creating ServerRmq object and send it dbContext
ServerRmq server = new ServerRmq(dbContext);
server.TheServerRmq(channel, connection);

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

channel.Close();
connection.Close();
//server.TheServerRmq(channel, connection);