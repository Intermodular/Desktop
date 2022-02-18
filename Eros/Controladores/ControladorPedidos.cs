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
    class ControladorPedidos
    {
        public static Pedidos GetFromApi(int id)
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/pedido/id/" + id);
            Pedidos pedido = JsonConvert.DeserializeObject<Pedidos>(respuesta);
            return pedido;
        }

        public static List<Pedidos> GetAllFromApi()
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/pedidos");
            List<Pedidos> listaPedidos = JsonConvert.DeserializeObject<List<Pedidos>>(respuesta);
            return listaPedidos;
        }

        public static Pedidos GetFromApiByIdMesa(int idMesa)
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/pedido/idMesa/" + idMesa);
            if (respuesta == "Not Found")
            {
                return null;
            }
            Pedidos pedido = JsonConvert.DeserializeObject<Pedidos>(respuesta);
            return pedido;
        }

        public static string PostToApi(Pedidos pedido)
        {
            string jsonPedidos = JsonConvert.SerializeObject(pedido);
            StringContent content = new StringContent(jsonPedidos, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PostHttp("http://localhost:8080/api/pedido", content);
            return respuesta;
        }

        public static string UpdateInApi(Pedidos pedido)
        {
            string jsonPedidos = JsonConvert.SerializeObject(pedido);
            StringContent content = new StringContent(jsonPedidos, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PutHttp("http://localhost:8080/api/pedido", content);
            return respuesta;
        }

        public static string DeleteFromApi(Pedidos pedido)
        {
            string respuesta = ControladorApi.DeleteHttp("http://localhost:8080/api/pedido/id/" + pedido._id + "/idMesa/" + pedido.idMesa);
            return respuesta;
        }
    }
}
