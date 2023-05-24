using B3DigitalService;
using Blazored.LocalStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredLocalStorage(config =>
    config.JsonSerializerOptions.WriteIndented = true);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHostedService<StartHostedService>();
builder.Services.AddSingleton<IObservableQuotesInfo, ObservableQuotesInfo>();
builder.Services.AddSingleton<ICalculateBestPrice, CalculateBestPrice>();
builder.Services.AddSingleton<IQueueService, QueueService>();
builder.Services.AddSingleton<IWebSocketService, WebSocketService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapControllers();

app.UseRouting();

app.UseAuthorization();

//app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
