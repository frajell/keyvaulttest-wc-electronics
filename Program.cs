using api.Data;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//https://wc-e-fraje-keyvault.vault.azure.net/secrets/WestCoastSqlConnectionString/b17113e3d1aa4548914f16ce3973a9f6

//Get azure keyvault secret
var secretUri = builder.Configuration.GetSection("KeyVaultSecrets:SqlConnection").Value;

var KeyVaultToken = new AzureServiceTokenProvider().KeyVaultTokenCallback;

var KeyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(KeyVaultToken));

var secret = await KeyVaultClient.GetSecretAsync(secretUri);

builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(secret.Value));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
