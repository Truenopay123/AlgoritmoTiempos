using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace AlgoritmoTiempos.Web.Services
{
    public class CalendarService
    {
        private readonly HashSet<DateTime> _festivos = new();
        public CalendarService(IConfiguration config)
        {
            var list = config.GetSection("Festivos").Get<string[]>() ?? Array.Empty<string>();
            foreach (var s in list)
            {
                if (DateTime.TryParseExact(s, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var f))
                    _festivos.Add(f.Date);
            }
        }
        public bool EsFinDeSemana(DateTime f) => f.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        public bool EsNoHabilGlobal(DateTime f) => EsFinDeSemana(f) || _festivos.Contains(f.Date);
        public DateTime SiguienteHabilGlobal(DateTime f)
        {
            var d = f.Date;
            while (EsNoHabilGlobal(d)) d = d.AddDays(1);
            return d;
        }
    }
}
