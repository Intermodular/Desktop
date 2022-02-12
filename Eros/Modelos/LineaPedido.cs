using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eros.Modelos
{
    public class LineaPedido
    {
        public Productos producto { get; set; }
        public int cantidad { get; set; }
        public string anotaciones { get; set; }
        public List<LineaExtra> lineasExtras { get; set; }
        public float costeLinea { get; set; }

        public void CalcularCosteLinea()
        {
            costeLinea = producto.precio * cantidad;
            if (lineasExtras != null)
            {
                foreach (LineaExtra le in lineasExtras)
                {
                    costeLinea += le.extra.precio * le.cantidad * cantidad;
                }
            }

        }

        public void InitializeLineasExtras(List<Tipos> listaTipos)
        {
            lineasExtras = new List<LineaExtra>();
            foreach (Tipos tipos in listaTipos)
            {
                if (tipos.nombre == producto.tipo)
                {
                    foreach (Extras extra in tipos.listaExtras)
                    {
                        LineaExtra le = new LineaExtra();
                        le.extra = extra;
                        le.cantidad = 0;
                        lineasExtras.Add(le);
                    }

                    return;
                }
            }

            lineasExtras = null;
        }
    }
}
