using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using BooksApp.Business.Abstract;
using BooksApp.Business.Concrete;
using BooksApp.Data.Abstract;
using BooksApp.Data.Concrete.EfCore;
using BooksApp.Data.Concrete.EfCore.Context;
using BooksApp.Entity.Concrete.Identity;
using BooksApp.MVC.EmailServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BooksAppContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection")));

builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<BooksAppContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit= true;//�ifre i�inde mutlaka rakam olmal�
    options.Password.RequireLowercase= true;//�ifre i�inde mutlaka k���k harf olmal�
    options.Password.RequireUppercase = true;//�ifre i�inde mutlaka b�y�k harf olmal�
    options.Password.RequiredLength= 6; //Uzunlu�u 6 karakter olmal�
    options.Password.RequireNonAlphanumeric= true;//Alfan�meric olmayan karakter bar�nd�rmal�
    //�rnek ge�erli parola: Qwe123.

    options.Lockout.MaxFailedAccessAttempts= 3;//�st �ste izin verilecek hatal� giri� say�s� 3
    options.Lockout.DefaultLockoutTimeSpan= TimeSpan.FromMinutes(5);//Kilitlenmi� hesaba 5 dakika sonra giri� yap�labilsin

    options.User.RequireUniqueEmail= true;//Sistemde daha �nce kay�tl� olmayan bir email adresi ile kay�t olunabilsin
    options.SignIn.RequireConfirmedEmail= false;//Email onay� pasif 
    options.SignIn.RequireConfirmedPhoneNumber= false;//Telefon numaras� onay� pasif
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";//E�er kullan�c� eri�ebilmesi i�in login olmak zorunda oldu�u bir yere istekte bulunursa, bu sayfaya y�nlendirilecek. (account controlleri i�indeki login action�)
    options.LogoutPath= "/account/logout";//Kullan�c� logout oldu�unda bu actiona y�nlendirilecek.
    options.AccessDeniedPath = "/account/accessdenied";//Kullan�c� yetkisi olmayan bir sayfaya istekte bulundu�unda bu actiona y�nlendirilecek.
    options.SlidingExpiration = true;//Cookie ya�am s�resinin her istekte s�f�rlanmas�n� sa�lar. Default olarak ya�am s�resi 20 dk, ama biz bunu ayarlayabiliriz.
    options.ExpireTimeSpan = TimeSpan.FromDays(10);//Ya�am s�resi 10 g�n olacak.
    options.Cookie = new CookieBuilder
    {
        HttpOnly = true,
        SameSite = SameSiteMode.Strict,
        Name = ".BooksApp.Security.Cookie"
    };
});

builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<IBookService, BookManager>();
builder.Services.AddScoped<IAuthorService, AuthorManager>();
builder.Services.AddScoped<IImageService, ImageManager>();
builder.Services.AddScoped<ICartService, CartManager>();
builder.Services.AddScoped<ICartItemService, CartItemManager>();
builder.Services.AddScoped<IOrderService, OrderManager>();

builder.Services.AddScoped<ICategoryRepository, EfCoreCategoryRepository>();
builder.Services.AddScoped<IBookRepository, EfCoreBookRepository>();
builder.Services.AddScoped<IAuthorRepository, EfCoreAuthorRepository>();
builder.Services.AddScoped<IImageRepository, EfCoreImageRepository>();
builder.Services.AddScoped<ICartRepository, EfCoreCartRepository>();
builder.Services.AddScoped<ICartItemRepository, EfCoreCartItemRepository>();
builder.Services.AddScoped<IOrderRepository, EfCoreOrderRepository>();

builder.Services.AddScoped<IEmailSender, SmtpEmailSender>(options=> new SmtpEmailSender (
    builder.Configuration["EmailSender:Host"],
    builder.Configuration.GetValue<int>("EmailSender:Port"),
    builder.Configuration.GetValue<bool>("EmailSender:EnableSSL"),
    builder.Configuration["EmailSender:UserName"],
    builder.Configuration["EmailSender:Password"]
  ));

builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 5;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
});

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();    

app.UseRouting();

app.UseAuthorization();

app.UseNotyf();

app.MapControllerRoute(
    name: "authordetails",
    pattern: "authordetails/{url}",
    defaults: new { controller = "Home", action = "AuthorDetails" }
    );

app.MapControllerRoute(
    name:"bookdetails",
    pattern:"bookdetails/{url}",
    defaults: new { controller="Home", action="BookDetails"}
    );

app.MapControllerRoute(
    name: "categories",
    pattern: "books/{categoryurl?}",
    defaults: new { controller="Home", action="Index" }
    );

app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "admin/{controller=Home}/{action=Index}/{id?}"
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    
app.Run();
