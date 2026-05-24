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

// ⚽ INYECTAMOS LOS ALUMNOS DIRECTAMENTE A LA BASE DE DATOS
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<miweb.Pages.ApplicationDbContext>();
    
    // Asegura que la base de datos esté creada correctamente
    context.Database.EnsureCreated();

    // Si la tabla está vacía, registramos al plantel automático
    if (!context.Alumnos.Any())
    {
        context.Alumnos.AddRange(
            new miweb.Pages.Alumno { NombreCompleto = "Alessandro Lozano (Admin)", Dni = "70961785", Celular = "900000000", Edad = 20, FechaPago = "24-05" },
            new miweb.Pages.Alumno { NombreCompleto = "Bryan Jesus Salvador Bejarano", Dni = "73351373", Celular = "954621912", Edad = 20, FechaPago = "" },
            new miweb.Pages.Alumno { NombreCompleto = "Leandro Jair Rodríguez Chávez", Dni = "71243492", Celular = "916703961", Edad = 20, FechaPago = "" },
            new miweb.Pages.Alumno { NombreCompleto = "Wilson Rojas Rojas", Dni = "71143044", Celular = "922333444", Edad = 20, FechaPago = "" },
            new miweb.Pages.Alumno { NombreCompleto = "Kevin Harold Villena Santos", Dni = "72791117", Celular = "955666777", Edad = 20, FechaPago = "" },
            new miweb.Pages.Alumno { NombreCompleto = "Heyner Melon Carvajal", Dni = "75351486", Celular = "988777666", Edad = 20, FechaPago = "" }
        );
        context.SaveChanges();
    }
}

app.Run();