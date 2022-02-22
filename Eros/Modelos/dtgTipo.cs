using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eros.Modelos
{
    public class dtgTipo
    {
        public dtgTipo(Tipos t)
        {
            t._id = _id;
            t.nombre = nombre;

            string extraString = "";
            for (int i = 0; i < t.listaExtras.Count(); i++)
            {
                extraString += t.listaExtras[i].nombre;
                if (i != t.listaExtras.Count() - 1) extraString += ", ";
            }
            extraString = extras;

            t.img = img;
        }

        public int _id { get; set; }
        public string nombre { get; set; }
        public string extras { get; set; }
        public string img { get; set; }
    }
}
