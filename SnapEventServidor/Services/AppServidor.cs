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
        void Escuchar(object? obj)
        {
         
            while (server.Server.IsBound)
            {
                var Tcpcliente=server.AcceptTcpClient();
                clients.Add(Tcpcliente);
                Tcpcliente.ReceiveBufferSize = 500000;
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
                var Fragmentos = new List<byte>();
                byte[] buffer = new byte[4096]; // Buffer de lectura
                int bytesRead = ns.Read(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    // No hay más datos disponibles, salimos del bucle
                    break;
                }

                // Almacenamos los datos leídos en la lista de fragmentos
                Fragmentos.AddRange(buffer.Take(bytesRead));

                // Si no hay más datos disponibles, terminamos la lectura
                if (ns.DataAvailable == false)
                {
                    break;
                }

                //convertimos a una cadena JSON
                string json = Encoding.UTF8.GetString(Fragmentos.ToArray());
                //deserializar JSON
                var foto = JsonSerializer.Deserialize<ImagenDto>(json);
                if( foto != null )
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ImagenRecibido?.Invoke(this, foto);
                    });
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
