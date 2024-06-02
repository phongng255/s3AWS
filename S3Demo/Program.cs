using Amazon.Runtime;
using Amazon.S3;
using Autofac;
using Autofac.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var accessKey = builder.Configuration.GetSection("AWS:Credential:AccessKey").Value;
var secretKey = builder.Configuration.GetSection("AWS:Credential:SecretKey").Value;
var serviceUrl = builder.Configuration.GetSection("AWS:ServiceURL").Value;

builder.Host
.UseServiceProviderFactory(new AutofacServiceProviderFactory())
.ConfigureContainer<ContainerBuilder>(b => 
{
    b.Register(context =>
    {
        var credentials = new BasicAWSCredentials(accessKey, secretKey);
        var config = new AmazonS3Config
        {
            ServiceURL = serviceUrl,
            ForcePathStyle = true
        };
        return new AmazonS3Client(credentials, config);
    }).As<IAmazonS3>().SingleInstance();
});

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
