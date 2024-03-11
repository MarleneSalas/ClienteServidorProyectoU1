using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;

namespace SnapEventServidor
{
    public partial class MainWindow : Window
    {
        private List<string> imagePaths = new List<string>()
        {
            "image1.jpg",
            "image2.jpg",
            "image3.jpg"
            // Aqui deberia ir el ruteo a las imagenes, pero no se como agregarlo

        };

        private int currentIndex = 0;
        private System.Timers.Timer timer;

        public MainWindow()
        {
            InitializeComponent();
            LoadImage();

            // Inicializa el temporizador
            timer = new System.Timers.Timer();
            timer.Interval = 500; //intervalo en milisegundos
            timer.Elapsed += Timer_Elapsed1; ;
            timer.Start();
        }

        private void Timer_Elapsed1(object? sender, ElapsedEventArgs e)
        {
            currentIndex = (currentIndex + 1) % imagePaths.Count;
            LoadImage();
        }
        private void LoadImage()
        {
            // Carga la imagen actual en el control de imagen
            string imagePath = imagePaths[currentIndex];
            Dispatcher.Invoke(() =>
            {
                imageControl.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(imagePath, UriKind.Relative));
            });
        }
    }
}
