using BoyumFoosballStats;
using BoyumFoosballStats.Viewmodel;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Azure;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
});
builder.Services.AddScoped<IScoreInputViewModel, ScoreInputViewModel>();
builder.Services.AddScoped<ITeamScoreViewModel, TeamScoreViewModel>();
builder.Services.AddScoped<IDashboardViewModel, DashboardViewModel>();
builder.Services.AddScoped<ISeasonDashboardViewModel, SeasonDashboardViewModel>();
builder.Services.AddScoped<ITeamStatsViewModel, TeamStatsViewModel>();
builder.Services.AddScoped<IPlayerDashboardViewModel, PlayerDashboardViewModel>();

await builder.Build().RunAsync();
