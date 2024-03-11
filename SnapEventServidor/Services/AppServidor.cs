using SnapEventServidor.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace SnapEventServidor.Services
{
    public class AppServidor
    {
        TcpListener server = null!;
        public event EventHandler<ImagenDto>? ImagenRecibido;
        List<TcpClient> clients = new List<TcpClient>();

        public void Iniciar()
        {
            server= new(new IPEndPoint(IPAddress.Any, 7001));
            server.Start();
            new Thread(Escuchar) { IsBackground = true }.Start();
        }
        void Escuchar()
        {
            while (server.Server.IsBound)
            {
                var Tcpcliente=server.AcceptTcpClient();
                clients.Add(Tcpcliente);
                Thread t = new(() =>
                {
                    RecibirImagenes(Tcpcliente);
                });
                t.IsBackground = true;
                t.Start();
            }
            
        }
        void RecibirImagenes(TcpClient cliente)
        {
            while (cliente.Connected)
            {
                var ns=cliente.GetStream();
                while (cliente.Available == 0)
                {
                    Thread.Sleep(500);

                }
                Byte[] buffer = new byte[cliente.Available];
                ns.Read(buffer, 0, buffer.Length);
                string json=Encoding.UTF8.GetString(buffer);
                var Imagen = JsonSerializer.Deserialize<ImagenDto>(json);
                if (Imagen != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    ImagenRecibido?.Invoke(this, Imagen));
                }

            }
            clients.Remove(cliente);
        }
        public void Detener()
        {
            if (server != null)
            {
                server.Stop();
                foreach(var item in clients)
                {
                    item.Close();
                }
            }
        }
    }
}
