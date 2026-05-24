using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace miweb.Pages.admin
{
    public class PanelAlumnosModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PanelAlumnosModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Alumno> ListaAlumnos { get; set; } = new();

        [BindProperty]
        public Alumno AlumnoInput { get; set; } = new();

        public async Task OnGetAsync()
        {
            ListaAlumnos = await _context.Alumnos.ToListAsync();
        }

        // ❌ ACCIÓN: ELIMINAR UN ALUMNO
        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            var alumno = await _context.Alumnos.FindAsync(id);
            if (alumno != null)
            {
                _context.Alumnos.Remove(alumno);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        // ➕ ACCIÓN: AGREGAR UN ALUMNO DESDE EL PANEL
        public async Task<IActionResult> OnPostAgregarAsync()
        {
            if (!ModelState.IsValid)
            {
                ListaAlumnos = await _context.Alumnos.ToListAsync();
                return Page();
            }

            _context.Alumnos.Add(AlumnoInput);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}