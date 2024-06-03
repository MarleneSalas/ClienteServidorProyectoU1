using SnapEventServidor.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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
        //List<TcpClient> clients = new List<TcpClient>();

        public void Iniciar()
        {
            server = new(new IPEndPoint(IPAddress.Any, 7001));
            server.Start();
            new Thread(Escuchar) { IsBackground = true }.Start();
        }

        void Escuchar(object? obj)
        {

            while (server.Server.IsBound)
            {
                var Tcpcliente = server.AcceptTcpClient();
                //clients.Add(Tcpcliente);
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
            var ns = cliente.GetStream();
            byte[] sizeBuffer = new byte[4];
            while (cliente.Connected)
            {
                
                while (cliente.Available == 0)
                {
                    Thread.Sleep(500);
                }
                
                ns.Read(sizeBuffer, 0, sizeBuffer.Length);
                int jsonSize = BitConverter.ToInt32(sizeBuffer, 0);

                byte[] buffer = new byte[jsonSize];
                int bytesRead = 0;

                while(bytesRead < jsonSize)
                {
                    bytesRead += ns.Read(buffer, bytesRead, jsonSize -bytesRead );
                }
                
                string json = Encoding.UTF8.GetString(buffer);

                var msg = JsonSerializer.Deserialize<ImagenDto>(json);

                if (msg != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ImagenRecibido?.Invoke(this, msg);

                    });
                }
            }
        }

        public void Detener()
        {
            if (server != null)
            {
                server.Stop();
            }
        }
    }
}