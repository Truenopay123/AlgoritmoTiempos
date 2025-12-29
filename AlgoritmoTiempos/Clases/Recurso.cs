using System;
using System.Collections.Generic;

namespace AlgoritmoTiempos
{
    public class Recurso
    {
        // Atributo almacena el nombre del recurso
        public string Nombre { get; set; }

        // Tope recomendado (configurable por usuario). Por defecto 5h/día.
        public int MaxHorasDiarias { get; set; } = 5;

        // Lista de asignaciones día a día para este recurso
        public List<ActividadAsignada> Actividades { get; set; } = new List<ActividadAsignada>();

        // Calendario de capacidad restante por fecha (solo laborables). Se inicializa en MaxHorasDiarias al pisar una fecha.
        public Dictionary<DateTime, int> CapacidadRestante { get; } = new Dictionary<DateTime, int>();

        // Para imprimir filas por actividad en la tabla, guardamos el orden de alta
        public List<string> OrdenActividades { get; } = new List<string>();

        // Constructor se ejecuta al crear un nuevo recurso.
        public Recurso(string nombre)
        {
            Nombre = nombre;
        }

        // Método sobrescrito devuelve el nombre del recurso cuando se imprime.
        public override string ToString() => Nombre;

        // Obtiene horas ya ocupadas por este recurso en una fecha
        public double HorasOcupadas(DateTime fecha)
        {
            double total = 0;
            foreach (var a in Actividades)
            {
                if (a.Fecha.Date == fecha.Date)
                    total += a.HorasAsignadasEseDia;
            }
            return total;
        }

        // Capacidad restante (inicializa si no existe)
        public int GetRemaining(DateTime fecha)
        {
            var d = fecha.Date;
            if (!CapacidadRestante.ContainsKey(d))
                CapacidadRestante[d] = MaxHorasDiarias;
            return CapacidadRestante[d];
        }

        public void RegistrarAsignacion(DateTime fecha, string nombreActividad, int horas)
        {
            var d = fecha.Date;
            if (!OrdenActividades.Contains(nombreActividad))
                OrdenActividades.Add(nombreActividad);

            Actividades.Add(new ActividadAsignada(d, nombreActividad, horas, this));
            CapacidadRestante[d] = GetRemaining(d) - horas;
            if (CapacidadRestante[d] < 0) CapacidadRestante[d] = 0;
        }
    }
}