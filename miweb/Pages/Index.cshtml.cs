using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace miweb.Pages
{
    public class Alumno
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre es demasiado largo")]
        public string Nombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener exactamente 8 caracteres")]
        public string Dni { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El celular es obligatorio")]
        public string Celular { get; set; } = string.Empty;
        
        [Range(18, 99, ErrorMessage = "Debe ser mayor de 18 años")]
        public int Edad { get; set; }
        
        public string EstadoPago { get; set; } = "Deuda Pendiente";
        public string Asistencias { get; set; } = "Sin registros";
    }

    public class NuevoInscrito
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        public int Edad { get; set; }
        public string Fecha { get; set; } = string.Empty;
    }

    public class AcademiaDbContext : DbContext
    {
        public AcademiaDbContext(DbContextOptions<AcademiaDbContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }
        public DbSet<Alumno> Alumnos { get; set; } = null!;
        public DbSet<NuevoInscrito> NuevosInscritos { get; set; } = null!;
    }

    public class IndexModel : PageModel
    {
        // 🟢 Cambiado a la ruta completa para eliminar la línea roja de VS Code
        private readonly miweb.Pages.AcademiaDbContext _context;

        public IndexModel(miweb.Pages.AcademiaDbContext context)
        {
            _context = context;
        }

        // Propiedades de configuración de la Academia
        public string DireccionAcademia { get; set; } = "Complejo Deportivo PJ, Trujillo";
        public int MontoInscripcion { get; set; } = 10;

        // Gestión de alertas del sistema
        [TempData]
        public string MensajeAlerta { get; set; } = string.Empty;
        
        [TempData]
        public string TipoAlerta { get; set; } = "info";

        // Control de Sesión Administrativa
        public static bool EsAdminSesion { get; set; } = false;

        // Propiedades para renderizar la Ficha del Alumno
        public bool AccesoConcedido { get; set; } = false;
        public string AlumnoNombre { get; set; } = string.Empty;
        public string AlumnoDni { get; set; } = string.Empty;
        public string AlumnoCelular { get; set; } = string.Empty;
        public string AlumnoEstadoPago { get; set; } = string.Empty;
        public string AlumnoAsistencias { get; set; } = string.Empty;
        public string WhatsappUrl { get; set; } = string.Empty;

        // Bindeos detallados de los Formularios Públicos
        [BindProperty]
        [Required(ErrorMessage = "Debe ingresar un DNI")]
        public string DniConsulta { get; set; } = string.Empty;

        [BindProperty]
        public string NuevoNombre { get; set; } = string.Empty;

        [BindProperty]
        public string NuevoDni { get; set; } = string.Empty;

        [BindProperty]
        public string NuevoCelular { get; set; } = string.Empty;

        [BindProperty]
        public int NuevoEdad { get; set; }

        // Bindeos detallados del Panel de Administración Manual
        [BindProperty]
        public string AdminManualNombre { get; set; } = string.Empty;

        [BindProperty]
        public string AdminManualDni { get; set; } = string.Empty;

        [BindProperty]
        public string AdminManualCelular { get; set; } = string.Empty;

        [BindProperty]
        public int AdminManualEdad { get; set; }

        [BindProperty]
        public string AdminManualEstadoPago { get; set; } = "Al día";

        // Colecciones en memoria para visualización de Tablas
        public List<Alumno> ListaAlumnos { get; set; } = new List<Alumno>();
        public List<NuevoInscrito> ListaNuevosInscritos { get; set; } = new List<NuevoInscrito>();

        // Método de Carga Inicial de la Página
        public async Task OnGetAsync()
        {
            try 
            {
                ListaAlumnos = await _context.Alumnos.ToListAsync();
                ListaNuevosInscritos = await _context.NuevosInscritos.ToListAsync();
            }
            catch (Exception ex)
            {
                MensajeAlerta = "Error al cargar los datos iniciales del servidor: " + ex.Message;
                TipoAlerta = "danger";
            }
        }

        // Handler Completo para Búsqueda de Alumnos y Acceso Admin
        public async Task<IActionResult> OnPostBuscarAsync()
        {
            if (string.IsNullOrEmpty(DniConsulta))
            {
                MensajeAlerta = "Por favor, introduzca un número de DNI válido.";
                TipoAlerta = "warning";
                ListaAlumnos = await _context.Alumnos.ToListAsync();
                ListaNuevosInscritos = await _context.NuevosInscritos.ToListAsync();
                return Page();
            }

            if (DniConsulta.Trim() == "ADMIN123")
            {
                EsAdminSesion = true;
                MensajeAlerta = "¡Bienvenido de vuelta, Alessandro! Autenticación como administrador correcta.";
                TipoAlerta = "success";
                ListaAlumnos = await _context.Alumnos.ToListAsync();
                ListaNuevosInscritos = await _context.NuevosInscritos.ToListAsync();
                return RedirectToPage();
            }

            var alumno = await _context.Alumnos.FirstOrDefaultAsync(a => a.Dni == DniConsulta.Trim());
            
            if (alumno != null)
            {
                AccesoConcedido = true;
                AlumnoNombre = alumno.Nombre;
                AlumnoDni = alumno.Dni;
                AlumnoCelular = alumno.Celular;
                AlumnoEstadoPago = alumno.EstadoPago;
                AlumnoAsistencias = alumno.Asistencias;
                MensajeAlerta = "Ficha deportiva encontrada correctamente.";
                TipoAlerta = "success";
            }
            else
            {
                AccesoConcedido = false;
                MensajeAlerta = "El DNI ingresado no coincide con ningún alumno registrado en el sistema actual.";
                TipoAlerta = "danger";
            }

            ListaAlumnos = await _context.Alumnos.ToListAsync();
            ListaNuevosInscritos = await _context.NuevosInscritos.ToListAsync();
            return Page();
        }

        // Handler Completo para Auto-Inscripción Externa de Usuarios
        public async Task<IActionResult> OnPostInscribirAsync()
        {
            if (string.IsNullOrEmpty(NuevoNombre) || string.IsNullOrEmpty(NuevoDni) || string.IsNullOrEmpty(NuevoCelular))
            {
                MensajeAlerta = "Todos los campos de inscripción son obligatorios para separar vacante.";
                TipoAlerta = "danger";
                ListaAlumnos = await _context.Alumnos.ToListAsync();
                ListaNuevosInscritos = await _context.NuevosInscritos.ToListAsync();
                return Page();
            }

            if (NuevoEdad < 18)
            {
                MensajeAlerta = "Inscripción denegada de forma automática. La academia solo admite mayores de edad (+18).";
                TipoAlerta = "danger";
                ListaAlumnos = await _context.Alumnos.ToListAsync();
                ListaNuevosInscritos = await _context.NuevosInscritos.ToListAsync();
                return Page();
            }

            var existeEnOficiales = await _context.Alumnos.AnyAsync(a => a.Dni == NuevoDni.Trim());
            if (existeEnOficiales)
            {
                MensajeAlerta = "Este número de DNI ya corresponde a un miembro activo de la Academia PJ.";
                TipoAlerta = "warning";
                ListaAlumnos = await _context.Alumnos.ToListAsync();
                ListaNuevosInscritos = await _context.NuevosInscritos.ToListAsync();
                return Page();
            }

            var nuevoInscrito = new NuevoInscrito
            {
                Nombre = NuevoNombre.Trim(),
                Dni = NuevoDni.Trim(),
                Celular = NuevoCelular.Trim(),
                Edad = NuevoEdad,
                Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
            };

            try
            {
                _context.NuevosInscritos.Add(nuevoInscrito);
                await _context.SaveChangesAsync();
                
                string msgWa = $"Hola, deseo unirme a la Academia PJ. Mis datos:\n⚽ Nombre: {NuevoNombre}\n🪪 DNI: {NuevoDni}\n📱 Celular: {NuevoCelular}";
                WhatsappUrl = $"https://wa.me/51904177349?text={Uri.EscapeDataString(msgWa)}";
                
                MensajeAlerta = "¡Felicidades! Tus datos provisionales han sido guardados. Procede a enviar el WhatsApp.";
                TipoAlerta = "success";
            }
            catch (Exception ex)
            {
                MensajeAlerta = "Ocurrió un error al procesar el registro en SQLite: " + ex.Message;
                TipoAlerta = "danger";
            }

            ListaAlumnos = await _context.Alumnos.ToListAsync();
            ListaNuevosInscritos = await _context.NuevosInscritos.ToListAsync();
            return Page();
        }

        // Handler Completo para Inserción Manual desde el Panel
        public async Task<IActionResult> OnPostAgregarAlumnoManualAsync()
        {
            if (!EsAdminSesion) 
            {
                return RedirectToPage();
            }

            if (string.IsNullOrEmpty(AdminManualNombre) || string.IsNullOrEmpty(AdminManualDni))
            {
                MensajeAlerta = "Los campos de Nombre y DNI son cruciales en la inserción manual.";
                TipoAlerta = "warning";
                return RedirectToPage();
            }

            var existeDni = await _context.Alumnos.AnyAsync(a => a.Dni == AdminManualDni.Trim());
            if (existeDni)
            {
                MensajeAlerta = "Conflicto de duplicidad: El DNI ingresado ya existe en la lista oficial.";
                TipoAlerta = "danger";
                return RedirectToPage();
            }

            var nuevo = new Alumno
            {
                Nombre = AdminManualNombre.Trim(),
                Dni = AdminManualDni.Trim(),
                Celular = AdminManualCelular.Trim(),
                Edad = AdminManualEdad,
                EstadoPago = AdminManualEstadoPago,
                Asistencias = "Sin faltas registradas (Alta manual de administrador)"
            };

            try 
            {
                _context.Alumnos.Add(nuevo);
                await _context.SaveChangesAsync();
                MensajeAlerta = $"El alumno {AdminManualNombre} ha sido registrado de forma exitosa y persistente.";
                TipoAlerta = "success";
            }
            catch (Exception ex)
            {
                MensajeAlerta = "Error crítico de base de datos durante inserción manual: " + ex.Message;
                TipoAlerta = "danger";
            }

            return RedirectToPage();
        }

        // Handler Seguro de Eliminación por ID único (Evita errores 400 de conversión)
        public async Task<IActionResult> OnPostEliminarAlumnoAsync(int id)
        {
            if (!EsAdminSesion) 
            {
                return RedirectToPage();
            }

            try 
            {
                var alumnoARemover = await _context.Alumnos.FindAsync(id);
                
                if (alumnoARemover != null)
                {
                    _context.Alumnos.Remove(alumnoARemover);
                    await _context.SaveChangesAsync();
                    MensajeAlerta = $"El registro del alumno '{alumnoARemover.Nombre}' ha sido purgado del archivo permanentemente.";
                    TipoAlerta = "success";
                }
                else
                {
                    MensajeAlerta = "No se logró ubicar al alumno seleccionado en el archivo SQLite.";
                    TipoAlerta = "danger";
                }
            }
            catch (Exception ex)
            {
                MensajeAlerta = "Error excepcional al procesar la solicitud de borrado: " + ex.Message;
                TipoAlerta = "danger";
            }

            return RedirectToPage();
        }

        // Cierre Seguro de Sesión del Administrador
        public IActionResult OnPostCerrarAdmin()
        {
            EsAdminSesion = false;
            MensajeAlerta = "Sesión de administración finalizada de manera segura.";
            TipoAlerta = "info";
            return RedirectToPage();
        }
    }
}