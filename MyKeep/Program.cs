using Marten;
using Marten.Events.Projections;
using MyKeep.Entities;
using MyKeep.Entities.TodoList;
using Oakton;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);

var isProduction = builder.Environment.IsProduction();

builder.Host.UseWolverine(opts =>
{
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.Policies.UseDurableLocalQueues();

    opts.Durability.Mode = isProduction ? DurabilityMode.Balanced : DurabilityMode.Solo;
});

builder.Host.ApplyOaktonExtensions();

builder.Services.AddMarten(opts =>
    {
        opts.Connection(builder.Configuration.GetConnectionString("Marten")
                        ?? throw new InvalidOperationException("Could not get Marten connection string"));
        opts.Events.MetadataConfig.HeadersEnabled = true;
        opts.DisableNpgsqlLogging = true;
        opts.UseSystemTextJsonForSerialization();


    })
    .ApplyAllDatabaseChangesOnStartup()
    .IntegrateWithWolverine()
    .EventForwardingToWolverine()
    .UseLightweightSessions();

builder.Services.AddEntities();
// Add services to the container.
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();

var app = builder.Build();

app.UsePathBase("/MyKeep");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapWolverineEndpoints();

await app.RunOaktonCommands(args);