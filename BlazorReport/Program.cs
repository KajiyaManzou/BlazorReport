using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorReport;
using BlazorReport.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// BootstrapBlazorサービスの追加
builder.Services.AddBootstrapBlazor();

// 社員データサービスの登録
builder.Services.AddScoped<EmployeeService>();

// Excelエクスポートサービスの登録
builder.Services.AddScoped<ExcelExportService>();

await builder.Build().RunAsync();
