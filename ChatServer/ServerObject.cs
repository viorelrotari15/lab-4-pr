using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ChatServer
{
    public class ServerObject
    {
        static TcpListener tcpListener; 
        List<ClientObject> clients = new List<ClientObject>(); // Lista de conexiuni la server

        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            // primim dupa ID conexiunea inchisa
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            // sau stergem din lista
            if (client != null)
                clients.Remove(client);
        }
        // citirea conexiunile de intrare
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Serverul este initializat asteptati conexiunea");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // Translarea mesajelor de intrare a clientilor cnectati
        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id) // verificam daca Id-ul clientului care a trimis este egal cu ID-ul clientului care primesje mesaj
                {
                    clients[i].Stream.Write(data, 0, data.Length); //Transmitem datele
                }
            }
        }
        // inchidem toti clientii
        protected internal void Disconnect()
        {
            tcpListener.Stop(); //orim serverul

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //remove pentru clientul conectat
            }
            Environment.Exit(0); //Terminam Task-ul
        }

    }
}
