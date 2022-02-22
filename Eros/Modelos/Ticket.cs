using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eros.Modelos
{
        public class Ticket
        {
            public int _id { get; set; }
            public int numMesa { get; set; }
            public List<LineaPedido> lineasPedido { get; set; }
            public float costeTotal { get; set; }
            public string fechaYHoraDePago { get; set; }
            public Empleado empleado { get; set; }
            public string metodoDePago { get; set; }


            public override string ToString()
            {
                string cadena = "";
                cadena += "Mesa " + numMesa + Environment.NewLine + Environment.NewLine;
                string producto;
                string cantidad;
                string precio;
                string relleno;
                string linea;
                int cantidadCharsPorLinea = 50;
                int cantidadRelleno;

                foreach (LineaPedido lp in lineasPedido)
                {
                    producto = lp.producto.nombre;
                    cantidad = lp.cantidad > 1 ? " X " + lp.cantidad + " " : " ";
                    precio = String.Format(" {0:0.00}€", lp.costeLinea);
                    cantidadRelleno = cantidadCharsPorLinea - (producto.Length + cantidad.Length + precio.Length);
                    relleno = "";
                    for (int i = 0; i < cantidadRelleno; i++)
                    {
                        relleno += "-";
                    }
                    linea = producto + cantidad + relleno + precio + Environment.NewLine;
                    foreach (LineaExtra le in lp.lineasExtras)
                    {
                        if (le.cantidad > 0)
                        {
                            cantidad = le.cantidad > 1 ? " X " + le.cantidad : "";
                            linea += "  Extra " + le.extra.nombre + cantidad + Environment.NewLine;
                        }

                    }
                    cadena += linea;
                }
                /*cadena += String.Format( Environment.NewLine + "Coste total sin IVA: {0:0.00}€" + Environment.NewLine
                                        + "Coste total: {1:0.00}€" + Environment.NewLine,
                                        costeTotal/1.21,costeTotal);*/
                string aux = "Coste total sin IVA: ";
                precio = String.Format(" {0:0.00}€", costeTotal / 1.21);
                cantidadRelleno = cantidadCharsPorLinea - (aux.Length + precio.Length);
                relleno = "";
                for (int i = 0; i < cantidadRelleno; i++)
                {
                    relleno += "-";
                }
                cadena += Environment.NewLine + aux + relleno + precio + Environment.NewLine;


                aux = "Coste total: ";
                precio = String.Format(" {0:0.00}€", costeTotal);
                cantidadRelleno = cantidadCharsPorLinea - (aux.Length + precio.Length);
                relleno = "";
                for (int i = 0; i < cantidadRelleno; i++)
                {
                    relleno += "-";
                }
                cadena += Environment.NewLine + aux + relleno + precio + Environment.NewLine;

                if (metodoDePago != null)
                {
                    cadena += Environment.NewLine + "Método de pago: " + metodoDePago + Environment.NewLine;
                }
                if (fechaYHoraDePago != null)
                {
                    cadena += "Fecha y hora de pago: " + fechaYHoraDePago + Environment.NewLine;
                }
                if (empleado != null)
                {
                    cadena += "Empleado cobrador: " + empleado.nombre + " " + empleado.apellido;
                }

                return cadena;
            }

            public void CleanLineaPedidoListFromExtras()
            {
                foreach (LineaPedido lp in lineasPedido)
                {
                    for (int i = 0; i < lp.lineasExtras.Count; i++) //No se puede utilizar foreach ya que se altera la lista al borrar un elemento
                    {
                        if (lp.lineasExtras[i].cantidad == 0)
                        {
                            lp.lineasExtras.Remove(lp.lineasExtras[i]);
                            i--;
                        }
                    }
                }
            }
        }

}
