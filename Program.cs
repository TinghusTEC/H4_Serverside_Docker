using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApp.Components;
using TodoApp.Components.Account;
using TodoApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var identityDbConnectionString = builder.Configuration.GetConnectionString("DockerIdendityDb") ?? throw new InvalidOperationException("Connection string 'DockerIdendityDb' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(identityDbConnectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var TodoconnectionString = builder.Configuration.GetConnectionString("DockerTodoDb") ?? throw new InvalidOperationException("Connection string 'DockerTodoDb' not found.");
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlite(TodoconnectionString));


builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequiredLength = 8;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAuthenticatedUser", policy =>
    {
        policy.RequireAuthenticatedUser();
    })
    .AddPolicy("RequireAdmin", inner =>
    {
        inner.RequireRole("Admin");
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
