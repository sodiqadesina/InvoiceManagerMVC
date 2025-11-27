using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();

var connectstr = builder.Configuration.GetConnectionString("CourseConnectionString");

builder.Services.AddDbContext<CustomerDbContext>(option => option.UseSqlServer(connectstr));


// dependacy injection so we can access the current Httpcontext ouside the controller 
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// dependacy injection AddScope for cus and Invoice service 

builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<IInvoiceService, InvoiceService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
