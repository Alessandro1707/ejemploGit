using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace miweb.Pages
{
    public class IndexModel : PageModel
    {
        // 🔑 Vinculamos la propiedad para que el HTML la pueda leer sin problemas
        [BindProperty]
        public string DniIngresado { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        // 🚀 Se ejecuta al presionar el botón azul de consulta
        public IActionResult OnPostConsultar()
        {
            // Validamos tu DNI de administrador
            if (DniIngresado == "70961785")
            {
                // Te envía directo a la administración de alumnos
                return RedirectToPage("/admin/PanelAlumnos");
            }

            // Si es otro DNI, por ahora solo refresca el inicio
            return RedirectToPage();
        }
    }
}