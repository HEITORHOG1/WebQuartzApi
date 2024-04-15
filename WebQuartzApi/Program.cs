using Quartz;
using WebQuartzApi;
using WebQuartzApi.EndPoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConfigureQuartz(builder.Configuration);
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
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
app.MapJobEndpoints();
app.Run();