using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace miweb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Alumno NuevoAlumno { get; set; } = new Alumno();

        [TempData]
        public string? ErrorDni { get; set; }

        public void OnGet() { }

        // 🔍 CONSULTAR FICHA / ACCESO ADMIN
        public async Task<IActionResult> OnPostConsultarAsync(string dniIngresado)
        {
            if (dniIngresado == "70961785")
            {
                return RedirectToPage("/admin/PanelAlumnos");
            }

            var alumno = await _context.Alumnos.FirstOrDefaultAsync(a => a.Dni == dniIngresado);
            if (alumno == null)
            {
                ErrorDni = "El DNI ingresado no coincide con ningún alumno registrado en el sistema actual.";
                return Page();
            }

            return RedirectToPage("/Index"); 
        }

        // 📲 REGISTRAR EN BD Y REDIRECCIONAR A WHATSAPP
        public async Task<IActionResult> OnPostRegistrarAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.Alumnos.Add(NuevoAlumno);
            await _context.SaveChangesAsync();

            // ⚠️ PON AQUÍ EL CELULAR REAL DE LA ACADEMIA (Ej: "51987654321")
            string telefonoAcademia = "51904177349"; 
            
            string mensaje = $"Hola, quiero confirmar mi inscripción:\n\n" +
                             $"*Nombre:* {NuevoAlumno.NombreCompleto}\n" +
                             $"*DNI:* {NuevoAlumno.Dni}\n" +
                             $"*Celular:* {NuevoAlumno.Celular}\n" +
                             $"*Edad:* {NuevoAlumno.Edad} años\n" +
                             $"*Costo:* S/ 109.00";

            string urlWhatsapp = $"https://api.whatsapp.com/send?phone={telefonoAcademia}&text={Uri.EscapeDataString(mensaje)}";
            return Redirect(urlWhatsapp);
        }
    }
}