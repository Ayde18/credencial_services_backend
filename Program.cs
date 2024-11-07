using System.Text;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5006"); // Cambia el puerto aquí

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("*") // Reemplaza con la URL de BlazorWasm
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
var app = builder.Build();

app.UseCors(); // Habilita CORS

var credenciales = new List<Credencial>();
const string correctKey = "123456789";

app.MapPost("/api/authenticate", ([FromBody] string key) => {
    if (key == correctKey) {
        return Results.Ok(true);
    }

    return Results.Ok(false);
});

app.MapPost("/api/credenciales", (Credencial credencial) => {
    credenciales.Add(credencial);

    return Results.Ok();
});

app.MapGet("/api/credenciales", () => {
    return Results.Ok(credenciales);
});

app.MapGet("/api/credenciales/{servicio}", (string servicio) => {
    var credencial = credenciales.FirstOrDefault(c => c.servicio == servicio);
    if (credencial == null) {
        return Results.NotFound();
    }
    return Results.Ok(credencial);
});

app.MapDelete("/api/credenciales/{servicio}", (string servicio) => {
    var credencial = credenciales.FirstOrDefault(c => c.servicio == servicio);
    if (credencial == null) {
        return Results.NotFound();
    }
    credenciales.Remove(credencial);
    return Results.NoContent();
});

app.Run();

public class Credencial
{
    public string servicio { get; set; } = string.Empty;
    public string usuario { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
}
