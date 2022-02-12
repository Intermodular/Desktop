using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eros.Modelos
{
    public class Pedidos
    {
        public int _id { get; set; }
        public int idMesa { get; set; }
        public List<LineaPedido> lineasPedido { get; set; }
    }
}
