using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eros.Modelos
{
    class Pedidos
    {
        public int _id { get; set; }
        public int idMesa { get; set; }
        public List<String> productos { get; set; }
        public string fecha{ get; set; }
        public string horaPago { get; set; }
        public bool pagado { get; set; }
        public float importeTotal { get; set; }
    }
}
