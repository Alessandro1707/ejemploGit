using miweb.Pages;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// 🟢 ¡ESTO ERA LO QUE FALTABA! Registramos la base de datos de manera estable para Render y Local
builder.Services.AddDbContext<AcademiaDbContext>(options =>
    options.UseSqlite($"Data Source={System.IO.Path.Combine(AppContext.BaseDirectory, "academia.db")}"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Importante para leer correctamente las imágenes y estilos css
app.UseStaticFiles(); 

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();