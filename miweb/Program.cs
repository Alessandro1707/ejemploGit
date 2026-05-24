using Microsoft.EntityFrameworkCore;
using miweb.Pages;

var builder = WebApplication.CreateBuilder(args);

// Agrega los servicios para las Páginas Razor
builder.Services.AddRazorPages();

// Configura la conexión a tu base de datos academia.db
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=academia.db"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// 🛠️ ASEGURA QUE TU DNI ADMIN SIEMPRE EXISTA AL INICIAR
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated(); // Crea la BD si no existe

        if (!context.Alumnos.Any(a => a.Dni == "70961785"))
        {
            context.Alumnos.Add(new Alumno
            {
                NombreCompleto = "Alessandro Lozano (Admin)",
                Dni = "70961785",
                Celular = "900000000",
                Edad = 20
            });
            context.SaveChanges();
        }
    }
    catch (Exception) { }
}

app.MapRazorPages();
app.Run();