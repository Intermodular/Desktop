using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eros.Modelos
{
    public class Tipos
    {
        public int _id { get; set; }
        public string nombre { get; set; }
        public List<Extras> listaExtras { get; set; }
        public string img { get; set; }
    }
}
