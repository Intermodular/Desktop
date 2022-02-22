using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eros.Modelos
{
    public class dtgProducto
    {
        public dtgProducto(Productos p)
        {
            p._id = _id;
            p.nombre = nombre;
            p.tipo = tipo;

            string ingreString = "";
            for (int i = 0; i < p.ingredientes.Count(); i++)
            {
                ingreString += p.ingredientes[i];
                if (i != p.ingredientes.Count() - 1) ingreString += ", ";
            }
            ingreString = ingredientes;

            p.precio = precio;

            string espeString = "";
            for (int i = 0; i < p.especificaciones.Count(); i++)
            {
                espeString += p.especificaciones[i];
                if (i != p.especificaciones.Count() - 1) espeString += ", ";
            }
            espeString = ingredientes;

            p.imagen = imagen;
            p.stock = stock;
        }

        public int _id { get; set; }
        public string nombre { get; set; }
        public string tipo { get; set; }
        public string ingredientes { get; set; }
        public float precio { get; set; }
        public string especificaciones { get; set; }
        public string imagen { get; set; }
        public int stock { get; set; }
    }
}
