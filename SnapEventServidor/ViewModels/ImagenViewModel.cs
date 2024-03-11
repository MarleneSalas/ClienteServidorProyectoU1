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
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;
using System.Security.Cryptography.Pkcs;

namespace SnapEventServidor.ViewModels
{
    public partial class ImagenViewModel : ObservableObject
    {
        AppServidor AppServidor { get; set; } = new AppServidor();
        public AppServidor server { get; set; } = new();
        public ObservableCollection<ImagenDto> Imagenes { get; set; } = new();
       public List<string> FotosEliminadas { get; set; } = new();
        public Dictionary<string, List<string>> Dic { get; set; }
        public string IP { get; set; } = "0.0.0.0";
        [ObservableProperty]
        public ImagenDto foto=new();
        [ObservableProperty]
        public int _imagenReciente;
        [ObservableProperty]
        public bool iniciarServer = false;

        private readonly string _carpeta = $"{AppDomain.CurrentDomain.BaseDirectory}Imagenes";
        [RelayCommand]
        private void Iniciar()
        {
            server.Iniciar();
            IniciarServer = true;
        }
        [RelayCommand]
        public void Detener()
        {
            server.Detener();
            IniciarServer = false;
        }


        public ImagenViewModel()
        {
            //generar ip
            var direcciones = Dns.GetHostAddresses
                (Dns.GetHostName());
            
            if (direcciones != null)
            {
                IP = string.Join(",", direcciones
                    .Where(x => x.AddressFamily ==
                    System.Net.Sockets.AddressFamily
                    .InterNetwork).Select(x => x.ToString()).FirstOrDefault());
            }
            //-------------------------------
            AppServidor.ImagenRecibido += AppServidor_ImagenRecibido;
            cargarArchivo();
            Iniciar();
        }

        private void cargarArchivo()
        {
            throw new NotImplementedException();
        }

        private void AppServidor_ImagenRecibido(object? sender, ImagenDto e)
        {
           if(e.Estado == "**ELIMINAR")
            {
                var fotoEliminar= Imagenes.Where(x=>x.Id.ToLower()==
                e.Id.ToLower()).FirstOrDefault();
                if(fotoEliminar != null)
                {
                    FotosEliminadas.Add(fotoEliminar.Imagen);
                    Imagenes.Remove(fotoEliminar);
                }
                var fp = Imagenes.FirstOrDefault();
                if (fp != null)
                {
                    ImagenReciente=Imagenes.IndexOf(fp);
                }
                    GuardarArchivo();
            }
           if(!string.IsNullOrWhiteSpace(e.Imagen))
            {
                Imagenes.Add(e);
                ImagenReciente = Imagenes.IndexOf(e);
                GuardarArchivo();
            }

        }
        string DecodificarImagen(ImagenDto imagen)
        {
            byte[] imageBytes = Convert.FromBase64String(imagen.Imagen);
            if (!Directory.Exists(_carpeta))
            {
                Directory.CreateDirectory(_carpeta);
            }
            var totalFoto=Imagenes.Where(x=>x.Usuario.ToLower()==
            imagen.Usuario.ToLower()).Count();

            string rutaImagen = $"{_carpeta}/{imagen.Id}.jpg";
            using (FileStream fs = new FileStream(rutaImagen, FileMode.Create))
            {
                fs.Write(imageBytes, 0, imageBytes.Length);
                fs.Close();
            }
            return rutaImagen;
        }
        string nombreJson = "Fotos.json";
        public void GuardarArchivo()
        {
                var json = JsonSerializer.Serialize(Imagenes);
                File.WriteAllText(nombreJson, json);

        }
        public void CargarArchivo()
        {
            if (File.Exists(nombreJson))
            {
                var json= File.ReadAllText(nombreJson);
                var datos = JsonSerializer.Deserialize<ObservableCollection<ImagenDto>?>(json);
                if(datos != null )
                {
                    Imagenes = datos;
                }
                else
                {
                    Imagenes = new ObservableCollection<ImagenDto>();
                }
            }
        }
      

    }
}
 