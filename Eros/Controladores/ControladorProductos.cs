using Eros.Modelos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Eros.Controladores
{
    class ControladorProductos
    {
        public static Productos GetFromApi(int id)
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/productos/id/" + id);
            Productos product = JsonConvert.DeserializeObject<Productos>(respuesta);
            return product;
        }

        public static List<Productos> GetAllFromApi()
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/productos");
            List<Productos> listaEmpleados = JsonConvert.DeserializeObject<List<Productos>>(respuesta);
            return listaEmpleados;
        }

        public static string PostToApi(Productos product)
        {
            string jsonProducto = JsonConvert.SerializeObject(product);
            StringContent content = new StringContent(jsonProducto, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PostHttp("http://localhost:8080/api/producto", content);
            return respuesta;
        }

        public static string UpdateInApi(Productos product)
        {
            string jsonProducto = JsonConvert.SerializeObject(product);
            StringContent content = new StringContent(jsonProducto, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PutHttp("http://localhost:8080/api/producto", content);
            return respuesta;
        }

        public static string DeleteFromApi(int id_product)
        {
            string respuesta = ControladorApi.DeleteHttp("http://localhost:8080/api/producto/id/" + id_product);
            return respuesta;
        }
    }
}
