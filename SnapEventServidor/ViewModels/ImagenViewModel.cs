using CommunityToolkit.Mvvm.Input;
using SnapEventServidor.Models.Dtos;
using SnapEventServidor.Services;
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
using System.Drawing;

namespace SnapEventServidor.ViewModels
{
    public class ImagenViewModel : INotifyPropertyChanged
    {
        AppServidor AppServidor { get; set; } = new AppServidor();
        public ICommand IniciarCommand { get; set; }
        public ICommand DetenerCommand { get; set; }
        public ObservableCollection<ImagenDto> Imagenes { get; set; } = new();
        public Dictionary<string, List<string>> Dic { get; set; }
        public string IP { get; set; } = "0.0.0.0";
        public ImagenViewModel()
        {
            var direcciones = Dns.GetHostAddresses
                (Dns.GetHostName());
            if (direcciones != null)
            {
                IP = string.Join(",", direcciones
                    .Where(x => x.AddressFamily ==
                    System.Net.Sockets.AddressFamily
                    .InterNetwork).Select(x => x.ToString()).FirstOrDefault());
            }
            IniciarCommand = new RelayCommand(Iniciar);
            DetenerCommand = new RelayCommand(Detener);
            AppServidor.ImagenRecibido += AppServidor_ImagenRecibido;
            Iniciar();
        }

        private void AppServidor_ImagenRecibido(object? sender, ImagenDto e)
        {
            var imagenbytes = Convert.FromBase64String(e.Imagen);
            using (MemoryStream ms = new MemoryStream(imagenbytes))
            {
                // Crear un objeto de imagen desde el stream
                Image image = Bitmap.FromStream(ms);
                // Guardar la imagen decodificada en un archivo (opcional)
                image.Save($"Images/Client/{e.Usuario}_{Dic[e.Usuario].Count+1}.jpg"); // Cambiar la extensión según el tipo de imagen
            }

        }

        private void Detener()
        {
            Imagenes.Clear();
            AppServidor.Detener();
        }

        private void Iniciar()
        {
            AppServidor.Iniciar();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
 