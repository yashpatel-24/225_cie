var builder = WebApplication.CreateBuilder(args);

// Add MVC services to the container
builder.Services.AddControllersWithViews();

// Register IHttpClientFactory for asynchronous API requests
builder.Services.AddHttpClient();

// Register in-memory caching service for 5-minute course list cache
builder.Services.AddMemoryCache();

// Register response caching service
builder.Services.AddResponseCaching();

// Configure in-memory session state
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable Response Caching middleware
app.UseResponseCaching();

// Enable Session middleware
app.UseSession();

app.UseAuthorization();

// Route to CourseController by default
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Course}/{action=Index}/{id?}");

app.Run();