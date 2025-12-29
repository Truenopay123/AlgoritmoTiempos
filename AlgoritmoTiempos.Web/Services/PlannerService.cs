using System;
using System.Collections.Generic;
using System.Linq;
using AlgoritmoTiempos.Web.Models;

namespace AlgoritmoTiempos.Web.Services
{
    public class PlannerService
    {
        private readonly CalendarService _calendar;
        public PlannerService(CalendarService calendar) { _calendar = calendar; }

        // Asigna horas por día, sin fraccionar, saltando no hábiles globales
        public PreviewResultado Preview(List<Persona> personas, List<Actividad> actividades, List<DiaNoHabil> noHabiles, DateTime inicioVisual)
        {
            var resultado = new PreviewResultado();
            var capacidadPorPersonaDia = new Dictionary<(int, DateTime), int>();

            foreach (var p in personas)
            {
                // Pre-cargar capacidad para un rango inicial (30 días)
                var cursor = inicioVisual.Date;
                for (int i = 0; i < 60; i++)
                {
                    var dia = _calendar.SiguienteHabilGlobal(cursor);
                    capacidadPorPersonaDia[(p.Id, dia)] = p.MaxHorasDiarias;
                    cursor = dia.AddDays(1);
                }
            }

            // Aplicar no hábiles por persona
            foreach (var nh in noHabiles)
            {
                if (nh.PersonaId is int pid)
                {
                    var key = (pid, nh.Fecha.Date);
                    if (capacidadPorPersonaDia.ContainsKey(key))
                        capacidadPorPersonaDia[key] = 0;
                }
            }

            // Asignaciones
            foreach (var act in actividades.OrderBy(a => a.Inicio))
            {
                var persona = personas.First(x => x.Id == act.PersonaId);
                var diasNecesarios = DiasHabilEntre(act.Inicio, act.Fin);
                var asignados = 0;
                var cursor = act.Inicio.Date;
                while (asignados < diasNecesarios)
                {
                    cursor = _calendar.SiguienteHabilGlobal(cursor);
                    var key = (persona.Id, cursor);
                    if (!capacidadPorPersonaDia.TryGetValue(key, out var remaining)) remaining = persona.MaxHorasDiarias;

                    if (remaining >= act.HorasPorDia)
                    {
                        capacidadPorPersonaDia[key] = remaining - act.HorasPorDia;
                        resultado.Asignaciones.Add(new AsignacionDia
                        {
                            Fecha = cursor,
                            PersonaId = persona.Id,
                            ActividadId = act.Id,
                            Horas = act.HorasPorDia
                        });
                        asignados++;
                        cursor = cursor.AddDays(1);
                    }
                    else
                    {
                        cursor = cursor.AddDays(1);
                    }
                }
            }

            // Ocupación por día (global, se puede luego por persona)
            var agrupado = resultado.Asignaciones.GroupBy(a => a.Fecha)
                .Select(g => new { Fecha = g.Key, Total = g.Sum(x => x.Horas) }).ToList();
            foreach (var item in agrupado)
            {
                // Máximo nominal 8h global por persona; aquí solo marcamos porcentaje agregado si hubiera una sola persona
                resultado.OcupacionPorDia[item.Fecha] = item.Total; // valor simple (se coloreará en UI por persona)
            }
            resultado.ExcedeCapacidad = resultado.Asignaciones.Any(a => a.Horas > personas.First(p => p.Id == a.PersonaId).MaxHorasDiarias);
            return resultado;
        }

        private int DiasHabilEntre(DateTime inicio, DateTime fin)
        {
            int count = 0;
            var d = inicio.Date;
            while (d <= fin.Date)
            {
                if (!_calendar.EsNoHabilGlobal(d)) count++;
                d = d.AddDays(1);
            }
            return count;
        }
    }
}
