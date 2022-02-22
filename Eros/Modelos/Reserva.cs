using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eros.Modelos
{
    public class Reserva
    {
        public int _id { get; set; }
        public string nombre { get; set; }
        public int idMesa { get; set; }
        public int anyo { get; set; }
        public int mes { get; set; }
        public int dia { get; set; }
        public int hora { get; set; }
        public int minuto { get; set; }
    }
}
