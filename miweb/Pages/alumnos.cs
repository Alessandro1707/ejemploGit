namespace miweb.Pages
{
    public class Alumno
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        public int Edad { get; set; } 
        
        // 📅 NUEVO: Guarda la fecha del último pago (Ej: "14-05")
        public string FechaPago { get; set; } = string.Empty;
    }
}