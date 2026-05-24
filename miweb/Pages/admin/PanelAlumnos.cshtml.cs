using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using miweb.Pages;

namespace miweb.Pages.admin
{
    public class PanelAlumnosModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PanelAlumnosModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Alumno> ListaAlumnos { get; set; } = new List<Alumno>();

        [BindProperty]
        public Alumno AlumnoInput { get; set; } = new Alumno();

        public async Task OnGetAsync()
        {
            ListaAlumnos = await _context.Alumnos.ToListAsync();
        }

        // ❌ ACCIÓN: ELIMINAR ALUMNO
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

        // ➕ ACCIÓN: AGREGAR NUEVO ALUMNO
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

        // 📝 ACCIÓN: EDITAR / ACTUALIZAR ALUMNO EXISTENTE
        public async Task<IActionResult> OnPostEditarAsync()
        {
            if (!ModelState.IsValid)
            {
                ListaAlumnos = await _context.Alumnos.ToListAsync();
                return Page();
            }

            var alumnoDb = await _context.Alumnos.FindAsync(AlumnoInput.Id);
            if (alumnoDb != null)
            {
                alumnoDb.NombreCompleto = AlumnoInput.NombreCompleto;
                alumnoDb.Dni = AlumnoInput.Dni;
                alumnoDb.Celular = AlumnoInput.Celular;
                alumnoDb.Edad = AlumnoInput.Edad;
                alumnoDb.FechaPago = AlumnoInput.FechaPago; // Actualiza la fecha de pago

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        // 💡 FUNCIÓN UTILITARIA: Calcula el próximo mes basándose en "DD-MM"
        public string CalcularProximoPago(string fechaActual)
        {
            if (string.IsNullOrEmpty(fechaActual) || !fechaActual.Contains("-"))
                return "Sin registrar";

            try
            {
                var partes = fechaActual.Split('-');
                int dia = int.Parse(partes[0]);
                int mes = int.Parse(partes[1]);

                // Sumamos un mes
                mes++;
                if (mes > 12) { mes = 1; }

                // Retorna el formato limpio con ceros a la izquierda si es necesario
                return $"{dia:D2}-{mes:D2}";
            }
            catch
            {
                return "Formato inválido (usar DD-MM)";
            }
        }
    }
}