using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapEventServidor.Models.Dtos
{
    public class ImagenDto
    {
        public string Id { get; set; } = null!;
        public string Imagen { get; set; } = null!;
        public string Usuario { get; set; } = null!;
        public string Estado { get; set; }=null!;

    }
}
