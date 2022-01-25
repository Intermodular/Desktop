using Eros.Modelos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Eros
{
    class ControladorMesas
    {
        public static Mesas GetFromApi(int id)
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/mesa/id/" + id);
            Mesas mesa = JsonConvert.DeserializeObject<Mesas>(respuesta);
            return mesa;
        }

        public static List<Mesas> GetAllFromApi()
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/mesas");
            List<Mesas> listaMesas = JsonConvert.DeserializeObject<List<Mesas>>(respuesta);
            return listaMesas;
        }

        public static string PostToApi(Mesas mesa)
        {
            string jsonPesa = JsonConvert.SerializeObject(mesa);
            StringContent content = new StringContent(jsonPesa, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PostHttp("http://localhost:8080/api/mesa", content);
            return respuesta;
        }

        public static string UpdateInApi(Mesas mesa)
        {
            string jsonMesa = JsonConvert.SerializeObject(mesa);
            StringContent content = new StringContent(jsonMesa, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PutHttp("http://localhost:8080/api/mesa", content);
            return respuesta;
        }

        public static string DeleteFromApi(int id_mesa)
        {
            string respuesta = ControladorApi.DeleteHttp("http://localhost:8080/api/mesa/id/" + id_mesa);
            return respuesta;
        }
    }
}
