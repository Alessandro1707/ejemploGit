using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace miweb.Pages;

public class IndexModel : PageModel
{
    private static readonly string DbPath = "academia.db";
    private static readonly string ConnectionString = $"Data Source={DbPath}";

    public List<(string Nombre, string Dni, string Celular, int Edad, string EstadoPago, string Asistencias)> ListaAlumnos { get; set; } = new();
    public List<(string Nombre, string Dni, string Celular, int Edad, string Fecha)> ListaNuevosInscritos { get; set; } = new();

    [BindProperty]
    public string DniConsulta { get; set; } = "";

    [BindProperty]
    public string NuevoNombre { get; set; } = "";
    [BindProperty]
    public string NuevoDni { get; set; } = "";
    [BindProperty]
    public int NuevoEdad { get; set; }
    [BindProperty]
    public string NuevoCelular { get; set; } = "";

    [BindProperty]
    public string AdminManualNombre { get; set; } = "";
    [BindProperty]
    public string AdminManualDni { get; set; } = "";
    [BindProperty]
    public string AdminManualCelular { get; set; } = "";
    [BindProperty]
    public int AdminManualEdad { get; set; }
    [BindProperty]
    public string AdminManualEstadoPago { get; set; } = "Al día";

    public int MontoInscripcion => 109;
    public bool AccesoConcedido { get; set; } = false;
    public static bool EsAdminSesion { get; set; } = false; 
    public string MensajeAlerta { get; set; } = "";
    public string TipoAlerta { get; set; } = "success";
    
    public string AlumnoNombre { get; set; } = "";
    public string AlumnoDni { get; set; } = "";
    public string AlumnoCelular { get; set; } = "";
    public int AlumnoEdad { get; set; }
    public string AlumnoEstadoPago { get; set; } = "";
    public string AlumnoAsistencias { get; set; } = "";
    public string WhatsappUrl { get; set; } = "";
    
    public string DireccionAcademia => "El descanso del guerrero - Mochica";

    public void OnGet()
    {
        AccesoConcedido = false;
        InicializarBaseDeDatos();
        if (EsAdminSesion)
        {
            CargarDatosAdmin();
        }
    }

    public void OnPostBuscar()
    {
        InicializarBaseDeDatos();

        if (DniConsulta == "70961785")
        {
            EsAdminSesion = true;
            AccesoConcedido = false;
            TipoAlerta = "success";
            MensajeAlerta = "👑 ¡Acceso Concedido, Administrador! Desplegando panel de control.";
            CargarDatosAdmin();
            return;
        }

        EsAdminSesion = false; 

        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Nombre, Celular, Edad, EstadoPago, Asistencias FROM Alumnos WHERE Dni = $dni";
            command.Parameters.AddWithValue("$dni", DniConsulta);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    AlumnoNombre = reader.GetString(0);
                    AlumnoCelular = reader.GetString(1);
                    AlumnoEdad = reader.GetInt32(2);
                    AlumnoEstadoPago = reader.GetString(3);
                    AlumnoAsistencias = reader.GetString(4);
                    AlumnoDni = DniConsulta;
                    
                    AccesoConcedido = true;
                    TipoAlerta = "success";
                    MensajeAlerta = $"¡Bienvenido, {AlumnoNombre}! Ficha deportiva cargada.";
                }
                else
                {
                    AccesoConcedido = false;
                    TipoAlerta = "danger";
                    MensajeAlerta = "El DNI ingresado no se encuentra registrado en la academia.";
                }
            }
        }
    }

    public void OnPostInscribir()
    {
        EsAdminSesion = false;
        InicializarBaseDeDatos();

        if (NuevoEdad < 18)
        {
            TipoAlerta = "danger";
            MensajeAlerta = "Inscripción rechazada. La academia es para mayores de 18 años.";
            return;
        }

        if (ExisteDni(NuevoDni))
        {
            TipoAlerta = "danger";
            MensajeAlerta = "Este DNI ya figura registrado en la academia.";
            return;
        }

        string fechaActual = DateTime.Now.ToString("dd/MM hh:mm tt");

        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO NuevosInscritos (Nombre, Dni, Celular, Edad, Fecha) VALUES ($nombre, $dni, $celular, $edad, $fecha)";
            command.Parameters.AddWithValue("$nombre", NuevoNombre);
            command.Parameters.AddWithValue("$dni", NuevoDni);
            command.Parameters.AddWithValue("$celular", NuevoCelular);
            command.Parameters.AddWithValue("$edad", NuevoEdad);
            command.Parameters.AddWithValue("$fecha", fechaActual);
            command.ExecuteNonQuery();
        }

        string numeroAcademia = "51904177349"; 
        string mensajeTexto = $"⚽ *NUEVA INSCRIPCIÓN - ACADEMIA PJ* ⚽\n\n" +
                              $"¡Hola! Me acabo de registrar desde la web para unirme a la academia:\n\n" +
                              $"👤 *Nombre:* {NuevoNombre}\n" +
                              $"🪪 *DNI:* {NuevoDni}\n" +
                              $"🎂 *Edad:* {NuevoEdad} años\n" +
                              $"📱 *Celular:* {NuevoCelular}\n" +
                              $"💰 *Monto:* S/ {MontoInscripcion} (Fijo)\n" +
                              $"📍 *Sede:* {DireccionAcademia}";

        string mensajeCodificado = Uri.EscapeDataString(mensajeTexto);
        WhatsappUrl = $"https://api.whatsapp.com/send?phone={numeroAcademia}&text={mensajeCodificado}";
        
        TipoAlerta = "success";
        MensajeAlerta = "¡Ficha de inscripción generada con éxito!";
    }

    public void OnPostAgregarAlumnoManual()
    {
        InicializarBaseDeDatos();

        if (ExisteDni(AdminManualDni))
        {
            TipoAlerta = "danger";
            MensajeAlerta = $"Error: El DNI {AdminManualDni} ya está registrado.";
            CargarDatosAdmin();
            return;
        }

        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Alumnos (Dni, Nombre, Celular, Edad, EstadoPago, Asistencias) VALUES ($dni, $nombre, $celular, $edad, $estado, $asistencias)";
            command.Parameters.AddWithValue("$dni", AdminManualDni);
            command.Parameters.AddWithValue("$nombre", AdminManualNombre);
            command.Parameters.AddWithValue("$celular", AdminManualCelular);
            command.Parameters.AddWithValue("$edad", AdminManualEdad);
            command.Parameters.AddWithValue("$estado", AdminManualEstadoPago);
            command.Parameters.AddWithValue("$asistencias", "Sin asistencias marcadas");
            command.ExecuteNonQuery();
        }
        
        TipoAlerta = "success";
        MensajeAlerta = $"⚽ ¡Registrado! {AdminManualNombre} ya puede consultar su estado con su DNI.";
        CargarDatosAdmin();
    }

    public void OnPostCerrarAdmin()
    {
        EsAdminSesion = false;
        AccesoConcedido = false;
    }

    private void InicializarBaseDeDatos()
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Alumnos (
                    Dni TEXT PRIMARY KEY,
                    Nombre TEXT,
                    Celular TEXT,
                    Edad INTEGER,
                    EstadoPago TEXT,
                    Asistencias TEXT
                );";
            command.ExecuteNonQuery();

            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS NuevosInscritos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nombre TEXT,
                    Dni TEXT,
                    Celular TEXT,
                    Edad INTEGER,
                    Fecha TEXT
                );";
            command.ExecuteNonQuery();

            command.CommandText = "SELECT COUNT(*) FROM Alumnos";
            long count = (long)(command.ExecuteScalar() ?? 0);
            if (count == 0)
            {
                command.CommandText = @"
                    INSERT INTO Alumnos (Dni, Nombre, Celular, Edad, EstadoPago, Asistencias) VALUES 
                    ('74589632', 'Alessandro Lozano', '987654321', 21, 'Al día', 'Mar (Asistió) - Jue (Asistió)'),
                    ('45127896', 'Carlos Mendoza', '912345678', 25, 'Deuda Pendiente', 'Mié (Faltó) - Vie (Asistió)');";
                command.ExecuteNonQuery();
            }
        }
    }

    private bool ExisteDni(string dni)
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Alumnos WHERE Dni = $dni";
            command.Parameters.AddWithValue("$dni", dni);
            long count = (long)(command.ExecuteScalar() ?? 0);
            return count > 0;
        }
    }

    private void CargarDatosAdmin()
    {
        ListaAlumnos.Clear();
        ListaNuevosInscritos.Clear();

        using (var connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();
            
            var cmdAlumnos = connection.CreateCommand();
            cmdAlumnos.CommandText = "SELECT Nombre, Dni, Celular, Edad, EstadoPago, Asistencias FROM Alumnos";
            using (var reader = cmdAlumnos.ExecuteReader())
            {
                while (reader.Read())
                {
                    ListaAlumnos.Add((
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetInt32(3),
                        reader.GetString(4),
                        reader.GetString(5)
                    ));
                }
            }

            var cmdNuevos = connection.CreateCommand();
            cmdNuevos.CommandText = "SELECT Nombre, Dni, Celular, Edad, Fecha FROM NuevosInscritos ORDER BY Id DESC";
            using (var reader = cmdNuevos.ExecuteReader())
            {
                while (reader.Read())
                {
                    ListaNuevosInscritos.Add((
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetInt32(3),
                        reader.GetString(4)
                    ));
                }
            }
        }
    }
}