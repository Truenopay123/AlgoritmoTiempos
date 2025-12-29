using System;
using System.Collections.Generic;
using System.Globalization;

namespace AlgoritmoTiempos
{
    static class Program
    {
        static void Main()
        {   //Le estás diciendo a la consola que use la codificación UTF-8 para mostrar texto.
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            // Se crea un objeto planificador, que será el que almacene todas las actividades.
            Planificador planificador = new Planificador();

            Console.WriteLine("=== SISTEMA DE PLANIFICACIÓN DE ACTIVIDADES (Flujo 2) ===\n");

            // Cargar festivos del config
            planificador.CargarFestivosDesdeConfig();

            // Selección de fecha inicio del proyecto (por defecto 19/12/2025)
            DateTime fechaInicioProyectoDefault = new DateTime(2025, 12, 19, 0, 0, 0, DateTimeKind.Local);
            DateTime fechaInicioProyecto;
            while (true)
            {
                Console.Write($"Ingrese la fecha de inicio del proyecto (dd/MM/yyyy) [Enter para {fechaInicioProyectoDefault:dd/MM/yyyy}]: ");
                var entrada = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(entrada))
                {
                    fechaInicioProyecto = fechaInicioProyectoDefault;
                    break;
                }
                if (DateTime.TryParseExact(entrada, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicioProyecto))
                    break;
                Console.WriteLine("Formato inválido. Use dd/MM/yyyy.");
            }
            Console.WriteLine($"Fecha de inicio del proyecto: {fechaInicioProyecto:dd/MM/yyyy}");

            // Memoria de usuarios
            var usuarios = new Dictionary<string, Recurso>(StringComparer.OrdinalIgnoreCase);
            Recurso? usuarioActual = null;

            while (true)
            {
                Console.WriteLine("\nMenú:");
                Console.WriteLine("1. Ingresar/Cambiar usuario y horas máximas");
                Console.WriteLine("2. Agregar actividad (días y horas por día)");
                Console.WriteLine("3. Mostrar tabla de asignaciones (usuario actual)");
                Console.WriteLine("4. Salir");
                Console.Write("Seleccione opción: ");
                var opt = Console.ReadLine();

                if (opt == "1")
                {
                    Console.Write("Nombre de usuario: ");
                    var nombre = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(nombre)) { Console.WriteLine("Nombre inválido."); continue; }
                    int max;
                    while (true)
                    {
                        Console.Write("Horas máximas diarias (recomendadas, p.e. 5): ");
                        var e = Console.ReadLine();
                        if (int.TryParse(e, out max) && max > 0) break;
                        Console.WriteLine("Valor inválido. Ingrese un entero > 0.");
                    }
                    if (!usuarios.TryGetValue(nombre, out usuarioActual))
                    {
                        usuarioActual = new Recurso(nombre) { MaxHorasDiarias = max };
                        usuarios[nombre] = usuarioActual;
                    }
                    else
                    {
                        usuarioActual.MaxHorasDiarias = max;
                    }
                    Console.WriteLine($"Usuario activo: {usuarioActual.Nombre} (Max {usuarioActual.MaxHorasDiarias}h/día)");
                }
                else if (opt == "2")
                {
                    if (usuarioActual == null) { Console.WriteLine("Primero configure un usuario (opción 1)."); continue; }
                    Console.Write("Nombre de la actividad: ");
                    var nomAct = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(nomAct)) { Console.WriteLine("Nombre inválido."); continue; }
                    int dias;
                    while (true)
                    {
                        Console.Write("Número de días: ");
                        var e = Console.ReadLine();
                        if (int.TryParse(e, out dias) && dias > 0) break;
                        Console.WriteLine("Valor inválido. Ingrese un entero > 0.");
                    }
                    int horasPorDia;
                    while (true)
                    {
                        Console.Write("Horas por día: ");
                        var e = Console.ReadLine();
                        if (!int.TryParse(e, out horasPorDia) || horasPorDia <= 0)
                        {
                            Console.WriteLine("Valor inválido. Ingrese un entero > 0.");
                            continue;
                        }
                        if (horasPorDia > usuarioActual.MaxHorasDiarias)
                        {
                            Console.WriteLine($"⚠️ Estás sobrepasando las {usuarioActual.MaxHorasDiarias} horas recomendadas por día para {usuarioActual.Nombre} (solicitadas {horasPorDia}). ¿Deseas continuar de todos modos? (s/n)");
                            var r = Console.ReadLine()?.Trim().ToLower();
                            bool continuar = r == "s" || r == "si" || r == "sí";
                            if (!continuar)
                            {
                                Console.WriteLine("Ok, modifica las horas por día.");
                                continue; // vuelve a pedir horasPorDia
                            }
                        }
                        break;
                    }

                    planificador.AsignarActividadFlujo2(usuarioActual, nomAct!, dias, horasPorDia, fechaInicioProyecto);
                }
                else if (opt == "3")
                {
                    if (usuarioActual == null) { Console.WriteLine("Primero configure un usuario (opción 1)."); continue; }
                    planificador.MostrarTabla(usuarioActual, fechaInicioProyecto);
                }
                else if (opt == "4")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Opción inválida.");
                }
            }
        }
    }
}