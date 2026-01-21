using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AlgoritmoTiempos.Web.Models;
using AlgoritmoTiempos.Web.Services;

namespace AlgoritmoTiempos.Web.Controllers
{
    public class PlanningController : Controller
    {
        private readonly PlannerService _planner;
        public PlanningController(PlannerService planner)
        {
            _planner = planner;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult MockData()
        {
            var personas = new List<Persona>
            {
                new Persona{ Id=1, Nombre="Julián", MaxHorasDiarias=8 },
                new Persona{ Id=2, Nombre="Lupita", MaxHorasDiarias=8 }
            };
            var actividades = new List<Actividad>
            {
                new Actividad{ Id=1, Nombre="Actividad 1", PersonaId=1, Inicio=new DateTime(2025,12,19,0,0,0,DateTimeKind.Unspecified), Fin=new DateTime(2025,12,24,0,0,0,DateTimeKind.Unspecified), HorasPorDia=2 },
                new Actividad{ Id=2, Nombre="Actividad 2", PersonaId=1, Inicio=new DateTime(2025,12,19,0,0,0,DateTimeKind.Unspecified), Fin=new DateTime(2025,12,19,0,0,0,DateTimeKind.Unspecified), HorasPorDia=3 },
                new Actividad{ Id=3, Nombre="Actividad 3", PersonaId=1, Inicio=new DateTime(2025,12,22,0,0,0,DateTimeKind.Unspecified), Fin=new DateTime(2025,12,24,0,0,0,DateTimeKind.Unspecified), HorasPorDia=2 },
                new Actividad{ Id=4, Nombre="Actividad 4", PersonaId=1, Inicio=new DateTime(2025,12,22,0,0,0,DateTimeKind.Unspecified), Fin=new DateTime(2025,12,22,0,0,0,DateTimeKind.Unspecified), HorasPorDia=3 },
                new Actividad{ Id=5, Nombre="Actividad 5", PersonaId=1, Inicio=new DateTime(2025,12,22,0,0,0,DateTimeKind.Unspecified), Fin=new DateTime(2025,12,24,0,0,0,DateTimeKind.Unspecified), HorasPorDia=1 },
                new Actividad{ Id=6, Nombre="Actividad 6", PersonaId=2, Inicio=new DateTime(2025,12,22,0,0,0,DateTimeKind.Unspecified), Fin=new DateTime(2025,12,26,0,0,0,DateTimeKind.Unspecified), HorasPorDia=1 },
                new Actividad{ Id=7, Nombre="Actividad 7", PersonaId=2, Inicio=new DateTime(2025,12,22,0,0,0,DateTimeKind.Unspecified), Fin=new DateTime(2025,12,23,0,0,0,DateTimeKind.Unspecified), HorasPorDia=2 }
            };
            var noHabiles = new List<DiaNoHabil>();
            var inicio = new DateTime(2025,12,19,0,0,0,DateTimeKind.Unspecified);
            var preview = _planner.Preview(personas, actividades, noHabiles, inicio);
            return Json(new { personas, actividades, preview });
        }
    }
}
