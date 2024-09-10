using Job_Rentabilitätsrechner.Interfaces;
using Job_Rentabilitätsrechner.Pages;
using Job_Rentabilitätsrechner.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ICommuteCostCalculationService, CommuteCostCalculationService>();
builder.Services.AddScoped<IWearAndTearCalculator, WearAndTearCalculationService>();
builder.Services.AddScoped<INetSalaryCalculationService, NetSalaryCalculationService>();
builder.Services.AddScoped<IGeocodingService, GeocodingService>();
builder.Services.AddScoped<IDistanceService, DistanceService>();
builder.Services.AddScoped<IFuelConsumptionAdjustment, FuelConsumptionAdjustmentService>();

var app = builder.Build();

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

app.MapControllers();

app.MapRazorPages();

app.Run();
