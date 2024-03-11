using Microsoft.Win32;
using SnapEventCliente.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnapEventCliente.Views
{
    /// <summary>
    /// Lógica de interacción para SnapEventView.xaml
    /// </summary>
    public partial class SnapEventView : UserControl
    {
        public SnapEventView()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            SnapEventViewModel datacontext = this.DataContext as SnapEventViewModel;

            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Archivos de imagen JPEG|*.jpg;*.jpeg|Todos los archivos|*.*";

            bool? resultado = openFileDialog.ShowDialog();
            string rutaImagen;
            if (resultado == true)
            {
                 rutaImagen = openFileDialog.FileName;

                try
                {
                    BitmapImage imagen = new BitmapImage(new Uri(rutaImagen));
                    imgImagenSeleccionada.Source = imagen;
                    datacontext.ImagenPath = rutaImagen;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar la imagen: " + ex.Message);
                }
            }
            wrpBotones.Visibility = Visibility.Visible;
            
            if (imgImagenSeleccionada.Source != null)
            {
                datacontext.Actualizar(null);
                datacontext.EnviarImagenCommand.Execute(this);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            imgImagenSeleccionada.Source = null;
            wrpBotones.Visibility = Visibility.Collapsed;
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
