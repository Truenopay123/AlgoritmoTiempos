using System;
using System.Collections.Generic;

namespace AlgoritmoTiempos.Web.Models
{
    public class Persona
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int MaxHorasDiarias { get; set; } = 8;
    }

    public class Actividad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Cliente { get; set; }
        public int PersonaId { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public int HorasPorDia { get; set; }
    }

    public class DiaNoHabil
    {
        public DateTime Fecha { get; set; }
        public int? PersonaId { get; set; } // null => global
    }

    public class AsignacionDia
    {
        public DateTime Fecha { get; set; }
        public int PersonaId { get; set; }
        public int ActividadId { get; set; }
        public int Horas { get; set; }
    }

    public class PreviewResultado
    {
        public List<AsignacionDia> Asignaciones { get; set; } = new();
        public Dictionary<DateTime, double> OcupacionPorDia { get; set; } = new();
        public bool ExcedeCapacidad { get; set; }
        public string? Mensaje { get; set; }
    }
}
