using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HourglassAzure.Models
{
    public class TramiteDePara
    {
        public int TramiteDeParaID { get; set; } 
        public int IdTramite { get; set; } 
        public DateTime DataAlteracao { get; set; }
        public string Email { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFim { get; set; }
        public string Obs { get; set; }
    }
}
