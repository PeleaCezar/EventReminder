using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services
            .AddHttpContextAccessor();
            //.AddInfrastructure(builder.Configuration)
            //.AddPersistence(builder.Configuration)
            //.AddBackgroundTasks(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();