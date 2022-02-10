using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eros.Modelos
{
    public class Nominas
    {
        public int _id { get; set; }
        public int idEmpleado { get; set; }
        public int horas { get; set; }
        public float euros{ get; set; }
        public string fechaInicio { get; set; }
        public string fechaFinal { get; set; }
    }
}
