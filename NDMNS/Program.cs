var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<HeaderHandler>();

builder
    .Services.AddHttpClient(
        "ApiClient",
        client =>
        {
            // client.BaseAddress = new Uri("https://localhost:44389/");
            client.Timeout = TimeSpan.FromMinutes(30);
        }
    )
    .AddHttpMessageHandler<HeaderHandler>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.MaxValue;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddMemoryCache();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Auth}/{action=Index}/{id?}");

app.Run();
