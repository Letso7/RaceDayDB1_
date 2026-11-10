using Microsoft.EntityFrameworkCore;
using RaceDayDB1.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new() { Title = "RaceDay API - ST10483808 PROG6212 Part2", Version = "v1" });
});
builder.Services.AddDbContext<RaceDayDBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o => {
    o.IdleTimeout = TimeSpan.FromMinutes(30);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();
var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles(); //banner fallback
app.UseSession();
app.UseAuthorization();
app.MapControllers();
app.Run();
public partial class Program { }