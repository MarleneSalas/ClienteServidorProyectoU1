using SnapEventCliente.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace SnapEventCliente.Services
{
    public class AppClient
    {
        public TcpClient client { get; set; } = null!;
        public string NombreUsuario { get; set; } = null!;
        //public event EventHandler<ImagenDTO>? ImagenRecibida;
        
        public void Conectar(IPAddress ip)
        {
            try
            {
                IPEndPoint ipe = new IPEndPoint(ip, 7001);
                client = new();
                client.Connect(ipe);
                NombreUsuario = Environment.UserName;

                var img = new ImagenDTO
                {
                    Usuario = "HELLO"
                };

                EnviarImagen(img);
            }
            catch (Exception ex)
            {

                MessageBox.Show("Se ha producido una excepción: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Desconectar()
        {
            client.Close();
        }

        public void EnviarImagen(ImagenDTO img)
        {
            if(!string.IsNullOrEmpty(img.Imagen)) 
            {
                string json = JsonSerializer.Serialize(img);

                byte[] buffer = Encoding.UTF8.GetBytes(json);
                byte[] lengthBuffer = BitConverter.GetBytes(buffer.Length);

                NetworkStream ns = client.GetStream();
                ns.Write(lengthBuffer);
                ns.Write(buffer, 0, buffer.Length);
                //ns.Flush();
                //var json = JsonSerializer.Serialize(img);
                //byte[] buffer = Encoding.UTF8.GetBytes(json);
                //client.SendBufferSize = buffer.Length;
                //var ns = client.GetStream();
                //ns.Write(buffer,0, buffer.Length);

                //ns.Flush();
            }
        }
    }
}
