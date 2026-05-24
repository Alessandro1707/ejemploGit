namespace miweb.Pages
{
    public class Alumno
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        public int Edad { get; set; } 
        public string FechaPago { get; set; } = string.Empty;

        // 📝 NUEVO: Control de asistencia de la semana
        public bool AsistioMartes { get; set; }
        public bool AsistioMiercoles { get; set; }
        public bool AsistioJueves { get; set; }
        public bool AsistioViernes { get; set; }
    }
}