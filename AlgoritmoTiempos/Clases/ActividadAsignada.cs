using System;
//ActividadAsignada: nueva clase que registra Fecha, NombreActividad, HorasAsignadasEseDia, Recurso.
namespace AlgoritmoTiempos
{
    // Representa cuántas horas de una actividad se asignaron a un recurso en una fecha específica
    public class ActividadAsignada
    {
        public DateTime Fecha { get; set; }
        public string NombreActividad { get; set; } = string.Empty;
        public double HorasAsignadasEseDia { get; set; }
        public Recurso Recurso { get; set; }

        public ActividadAsignada(DateTime fecha, string nombreActividad, double horas, Recurso recurso)
        {
            Fecha = fecha.Date;
            NombreActividad = nombreActividad;
            HorasAsignadasEseDia = horas;
            Recurso = recurso;
        }

        public override string ToString()
        {
            return $"{Fecha:dd/MM/yyyy} | {NombreActividad} | {HorasAsignadasEseDia}h";
        }
    }
}