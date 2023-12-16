using FaceRetouching.Server.Entities;
using FaceRetouching.Server.Services;
using Microsoft.EntityFrameworkCore;

using (var db = new Context())
{
	db.Database.Migrate();
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<PluginsService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
