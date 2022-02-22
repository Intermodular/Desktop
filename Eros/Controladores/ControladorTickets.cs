using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eros.Modelos;
using Newtonsoft.Json;
using System.Net.Http;

namespace Eros.Controladores
{
    class ControladorTickets
    {
        public static Ticket GetFromApi(int id)
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/ticket/id/" + id);
            Ticket ticket = JsonConvert.DeserializeObject<Ticket>(respuesta);
            return ticket;
        }

        public static List<Ticket> GetAllFromApi()
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/tickets");
            List<Ticket> listaTickets = JsonConvert.DeserializeObject<List<Ticket>>(respuesta);
            return listaTickets;
        }

        public static string PostToApi(Ticket ticket)
        {
            string jsonTicket = JsonConvert.SerializeObject(ticket);
            StringContent content = new StringContent(jsonTicket, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PostHttp("http://localhost:8080/api/ticket", content);
            return respuesta;
        }

        public static string UpdateInApi(Ticket ticket)
        {
            string jsonTicket = JsonConvert.SerializeObject(ticket);
            StringContent content = new StringContent(jsonTicket, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PutHttp("http://localhost:8080/api/ticket", content);
            return respuesta;
        }

        public static string DeleteFromApi(int id_ticket)
        {
            string respuesta = ControladorApi.DeleteHttp("http://localhost:8080/api/ticket/id/" + id_ticket);
            return respuesta;
        }
    }
}
