using BoyumFoosballStats;
using BoyumFoosballStats.Viewmodel;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IScoreInputViewModel, ScoreInputViewModel>();
builder.Services.AddScoped<ITeamScoreViewModel, TeamScoreViewModel>();
await builder.Build().RunAsync();
