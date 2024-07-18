using Blog.Business.Managers;
using Blog.Business.Services;
using Blog.Data.Context;
using Blog.Data.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BlogContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IArticleService, ArticleManager>();
builder.Services.AddScoped<ICommentService, CommentManager>();
builder.Services.AddScoped<ITagService, TagManager>();
builder.Services.AddScoped<IHomeService, HomeManager>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new PathString("/Auth/Login");
        options.LogoutPath = new PathString("/Auth/Logout");
        options.AccessDeniedPath = new PathString("/Auth/AccessDenied");
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "articleDetails",
    pattern: "Article/Details/{id:int}/{title}",
    defaults: new { controller = "Article", action = "Details" });

app.MapDefaultControllerRoute();

app.Run();
