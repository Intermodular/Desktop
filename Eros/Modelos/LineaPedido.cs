using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eros.Modelos
{
    class LineaPedido
    {
        public Productos producto { get; set; }
        public int cantidad { get; set; }
        public string anotaciones { get; set; }
        public float costeLinea { get; set; }

        public void CalcularCosteLinea()
        {
            costeLinea = producto.precio * cantidad;
        }
    }
}
