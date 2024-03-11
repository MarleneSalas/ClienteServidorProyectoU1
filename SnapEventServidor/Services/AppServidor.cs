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
                    Thread.Sleep(500);
                }
                byte[] sizeBuffer = new byte[sizeof(int)];
                ns.Read(sizeBuffer,0,sizeBuffer.Length);
                int jsonSize = BitConverter.ToInt32(sizeBuffer,0);  

                }
                Byte[] buffer = new byte[cliente.Available];
                ns.Read(buffer, 0, buffer.Length);
                string json=Encoding.UTF8.GetString(buffer);
                var Imagen = JsonSerializer.Deserialize<ImagenDto>(json);
                if (Imagen != null)
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
