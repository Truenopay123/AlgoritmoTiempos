using System;
using System.Collections.Generic;
//CalendarioLaboral: sigue controlando sábados/domingos y feriados.
namespace AlgoritmoTiempos
{
    // Controla qué días son laborables y utilidades relacionadas
    public class CalendarioLaboral
    {
        // Conjunto de días no laborables (feriados personalizados).
        // Se guarda solo la parte de fecha (Date) para ignorar la hora.
        private readonly HashSet<DateTime> _noLaborables = new HashSet<DateTime>();

        public void AgregarNoLaborable(DateTime fecha)
        {
            // Agrega un día al listado de feriados/no laborables.
            _noLaborables.Add(fecha.Date);
        }

        public void AgregarNoLaborables(IEnumerable<DateTime> fechas)
        {
            // Agrega múltiples días al listado de feriados/no laborables.
            foreach (var f in fechas)
                _noLaborables.Add(f.Date);
        }

        public bool EsFinDeSemana(DateTime fecha)
        {
            // Valida si la fecha cae en sábado o domingo.
            var d = fecha.DayOfWeek;
            return d == DayOfWeek.Saturday || d == DayOfWeek.Sunday;
        }

        public bool EsNoLaborable(DateTime fecha)
        {
            // Un día no laborable es fin de semana o está en la lista de feriados.
            return EsFinDeSemana(fecha) || _noLaborables.Contains(fecha.Date);
        }

        public bool EsLaborable(DateTime fecha)
        {
            // Un día laborable es aquel que NO está marcado como no laborable.
            return !EsNoLaborable(fecha);
        }

        // Devuelve la misma fecha si ya es laborable; si no, el siguiente día laborable
        public DateTime SiguienteLaborable(DateTime fecha)
        {
            // Avanza desde la fecha dada hasta encontrar un día que no sea fin de semana ni feriado.
            var f = fecha.Date;
            while (EsNoLaborable(f))
                f = f.AddDays(1);
            return f;
        }
    }
}