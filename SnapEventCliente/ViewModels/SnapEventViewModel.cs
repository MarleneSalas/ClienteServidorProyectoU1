using CommunityToolkit.Mvvm.Input;
using SnapEventCliente.Models.DTOs;
using SnapEventCliente.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SnapEventCliente.ViewModels
{
    public class SnapEventViewModel : INotifyPropertyChanged
    {
        AppClient client = new();
        public bool Conectado { get { return client.client == null ? false : client.client.Connected; } }
        public string IP { get; set; } = "";
        public ICommand ConectarCommand { get; set; }
        public ICommand DesconectarCommand { get; set; }
        public ICommand EliminarImagenCommand { get; set; }
        public ICommand EnviarImagenCommand { get; set; }
        public ObservableCollection<ImagenDTO> ImagenesUsuario { get; set; } = new();
        public ObservableCollection<string> ListaImagenes { get; set; } = new();
        public string ImagenPath { get; set; }


        public SnapEventViewModel()
        {
            ConectarCommand = new RelayCommand(Conectar);
            DesconectarCommand = new RelayCommand(Desconectar);
            EnviarImagenCommand = new RelayCommand(EnviarImagen);
            EliminarImagenCommand = new RelayCommand(EliminarImagen);
        }

        private void EliminarImagen()
        {
            if (ListaImagenes.Contains(ImagenPath))
            {
                byte[] image = File.ReadAllBytes(ImagenPath);

                ImagenDTO foto = new ImagenDTO()
                {
                    Usuario = Environment.UserName,
                    Imagen = Convert.ToBase64String(image)
                };

                client.EnviarImagen(foto);
                ListaImagenes.Remove(ImagenPath);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        private void Desconectar()
        {
            IP = "0.0.0.0";
            client.Desconectar();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Conectado)));
        }

        private void Conectar()
        {
            IPAddress.TryParse(IP, out IPAddress? ipAdress);
            if (ipAdress != null)
            {
                client.Conectar(ipAdress);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Conectado)));
            }
        }

        private void EnviarImagen()
        {
            if (File.Exists(ImagenPath))
            {
                byte[] buffer = File.ReadAllBytes(ImagenPath);
                string imagenBase64 = Convert.ToBase64String(buffer);
                client.EnviarImagen(
                    new ImagenDTO
                    {
                        Usuario = Environment.UserName,
                        Imagen = imagenBase64
                    });
                ListaImagenes.Add(ImagenPath);
                
            }
        }
        public void Actualizar(string? text)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(text));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
