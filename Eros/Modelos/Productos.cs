using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eros.Modelos
{
    public class Productos
    {
        public int _id { get; set; }
        public string nombre { get; set; }
        public string tipo { get; set; }
        public List<String> ingredientes { get; set; }
        public float precio { get; set; }
        public List<String> especificaciones { get; set; }
        public string imagen { get; set; }
        public int stock { get; set; }
    }
}
