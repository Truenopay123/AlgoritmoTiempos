using System;
using System.Collections.Generic;
using System.Configuration;

namespace AlgoritmoTiempos
{
    public class Planificador
    {
        public CalendarioLaboral Calendario { get; }

        public Planificador(CalendarioLaboral? calendario = null)
        {
            // Permite inyectar un calendario externo; si no se pasa, se crea uno por defecto.
            // El calendario controla fines de semana y feriados.
            Calendario = calendario ?? new CalendarioLaboral();
        }

        

        // Cargar festivos desde app.config (clave Festivos: "dd/MM/yyyy,dd/MM/yyyy")
        public void CargarFestivosDesdeConfig()
        {
            try
            {
                var val = ConfigurationManager.AppSettings["Festivos"];
                if (string.IsNullOrWhiteSpace(val)) return;
                var partes = val.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var p in partes)
                {
                    var limpio = p.Trim();
                    if (DateTime.TryParseExact(
                        limpio, 
                        "dd/MM/yyyy", 
                        System.Globalization.CultureInfo.InvariantCulture, 
                        System.Globalization.DateTimeStyles.None,
                        out var f))
                    {
                        Calendario.AgregarNoLaborable(f);
                    }    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No se pudieron cargar festivos del config: {ex.Message}");
            }
        }

        // Flujo 2 (solicitado): asignar horas por día durante N días para un recurso, saltando fines de semana/festivos.
        public void AsignarActividadFlujo2(Recurso recurso, string nombreActividad, int dias, int horasPorDia, DateTime fechaInicio)
        {
            try
            {
                if (recurso == null) { Console.WriteLine("Debe seleccionar un usuario antes."); return; }
                if (dias <= 0 || horasPorDia <= 0) { Console.WriteLine("Días y horas por día deben ser > 0."); return; }

                int asignados = 0;
                DateTime cursor = Calendario.SiguienteLaborable(fechaInicio);

                while (asignados < dias)
                {
                    cursor = Calendario.SiguienteLaborable(cursor);
                    int remaining = recurso.GetRemaining(cursor);
                    if (remaining >= horasPorDia)
                    {
                        recurso.RegistrarAsignacion(cursor, nombreActividad, horasPorDia);
                        asignados++;
                        // Avanzar al siguiente día laborable para el próximo bloque
                        cursor = cursor.AddDays(1);
                    }
                    else
                    {
                        // Día lleno, probar siguiente
                        cursor = cursor.AddDays(1);
                    }
                }

                Console.WriteLine($"Actividad '{nombreActividad}' asignada correctamente a {recurso.Nombre}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al asignar actividad: {ex.Message}");
            }
        }

        // Imprimir tabla tipo Excel para un recurso
        public void MostrarTabla(Recurso recurso, DateTime fechaInicio)
        {
            try
            {
                if (recurso == null) { Console.WriteLine("Seleccione un usuario primero."); return; }

                // Determinar rango de fechas: desde inicio hasta el último día asignado o +8 laborables
                DateTime maxFecha = fechaInicio;
                foreach (var a in recurso.Actividades)
                    if (a.Fecha > maxFecha) maxFecha = a.Fecha;

                // Asegurar al menos 8 días laborables
                int laborablesCont = 0;
                DateTime f = fechaInicio;
                while (laborablesCont < 8)
                {
                    f = Calendario.SiguienteLaborable(f);
                    if (f > maxFecha) maxFecha = f;
                    f = f.AddDays(1);
                    laborablesCont++;
                }

                // Construir lista de columnas (solo laborables)
                var columnas = new List<DateTime>();
                var aux = fechaInicio;
                while (aux <= maxFecha.AddDays(10)) // margen
                {
                    aux = Calendario.SiguienteLaborable(aux);
                    if (!columnas.Contains(aux)) columnas.Add(aux);
                    aux = aux.AddDays(1);
                    if (columnas.Count >= 30) break; // evitar tablas enormes
                }

                // Encabezado
                Console.Write("Actividad ".PadRight(20));
                foreach (var c in columnas)
                    Console.Write($"| {c:dd} ");
                Console.WriteLine();

                // Filas por actividad según orden de alta
                foreach (var nombreAct in recurso.OrdenActividades)
                {
                    Console.Write(nombreAct.PadRight(20));
                    foreach (var c in columnas)
                    {
                        int horas = 0;
                        foreach (var a in recurso.Actividades)
                            if (a.Fecha.Date == c.Date && a.NombreActividad == nombreAct)
                                horas += (int)a.HorasAsignadasEseDia;
                        var celda = horas > 0 ? horas.ToString() : " ";
                        Console.Write($"| {celda.PadLeft(2)} ");
                    }
                    Console.WriteLine();
                }

                // Totales por día y validación contra MaxHorasDiarias
                Console.Write("Total por día".PadRight(20));
                foreach (var c in columnas)
                {
                    int total = 0;
                    foreach (var a in recurso.Actividades)
                        if (a.Fecha.Date == c.Date) total += (int)a.HorasAsignadasEseDia;
                    string mark = total > recurso.MaxHorasDiarias ? "*" : "";
                    Console.Write($"| {total.ToString().PadLeft(2)}{mark} ");
                }
                Console.WriteLine();

                Console.WriteLine("(*) Excede el máximo recomendado para el usuario en ese día.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al mostrar tabla: {ex.Message}");
            }
        }
    }
}