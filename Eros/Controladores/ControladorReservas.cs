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
    public class ControladorReservas
    {
        public static Reserva GetFromApi(int id)
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/reserva/id/" + id);
            Reserva reserva = JsonConvert.DeserializeObject<Reserva>(respuesta);
            return reserva;
        }

        public static List<Reserva> GetAllFromApi(int anyo, int mes, int dia, int hora, int min)
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/reservas/minuto/" + anyo + "/" + mes + "/" + dia + "/" + hora + "/" + min);
            List<Reserva> listaReserva = JsonConvert.DeserializeObject<List<Reserva>>(respuesta);
            return listaReserva;
        }

        public static List<Reserva> GetAllFromApi(int anyo, int mes, int dia)
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/reservas/dia/" + anyo + "/" + mes + "/" + dia);
            List<Reserva> listaReserva = JsonConvert.DeserializeObject<List<Reserva>>(respuesta);
            return listaReserva;
        }

        public static string PostToApi(Reserva reserva)
        {
            string jsonReserva = JsonConvert.SerializeObject(reserva);
            StringContent content = new StringContent(jsonReserva, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PostHttp("http://localhost:8080/api/reserva", content);
            return respuesta;
        }

        public static string DeleteFromApi(int id_reserva)
        {
            string respuesta = ControladorApi.DeleteHttp("http://localhost:8080/api/reserva/id/" + id_reserva);
            return respuesta;
        }

        /*public static List<Reserva> GetAllFromApi()
        {
            string respuesta = ControladorApi.GetHttp("http://localhost:8080/api/tickets");
            List<Ticket> listaTickets = JsonConvert.DeserializeObject<List<Ticket>>(respuesta);
            return listaTickets;
        }

        

        public static string UpdateInApi(Reserva ticket)
        {
            string jsonTicket = JsonConvert.SerializeObject(ticket);
            StringContent content = new StringContent(jsonTicket, Encoding.UTF8, "application/json");
            string respuesta = ControladorApi.PutHttp("http://localhost:8080/api/ticket", content);
            return respuesta;
        }

        */
    }
}
