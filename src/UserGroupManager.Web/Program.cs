using Blazored.LocalStorage;
using UserGroupManager.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredLocalStorage(); // This requires the Blazored.LocalStorage package

builder.Services.AddScoped(frontEnd => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7090/")
}
);

builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// This policy is used by the UI to show/hide elements.
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("CanManageUsers", policy =>
        policy.RequireClaim("permission", "Manage Users"));

    options.AddPolicy("CanManageGroups", policy =>
        policy.RequireClaim("permission", "Manage Groups"));

    options.AddPolicy("CanViewUsers", policy =>
        policy.RequireClaim("permission", "View Users"));
});

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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
