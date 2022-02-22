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
        public string nombreEmpleado { get; set; }
        public string apellidoEmpleado { get; set; }
        public string dniEmpleado { get; set; }
        public string direccionEmpleado { get; set; }
        public int horasCorrientes { get; set; }
        public float precioHoraCorriente { get; set; }
        public int horasExtras { get; set; }
        public float precioHoraExtra { get; set; }
        public string fechaInicio { get; set; }
        public string fechaFinal { get; set; }
        public float remuneracionTotal { get; set; }
    }
}
