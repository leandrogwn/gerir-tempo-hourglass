using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HourglassAzure.Models
{
    public class Tramite
    {
        public int TramiteID { get; set; }
        public Boolean OrigemTramite { get; set; }
        public Boolean TramiteEditado { get; set; }
        public string Time { get; set; }
        public string Atividade { get; set; }
        public string Retrabalho { get; set; }
        public string Email { get; set; }
        public string Projeto { get; set; }
        public string Sprint { get; set; }
        public int Pbi { get; set; }
        public int Tarefa { get; set; }
        public string Titulo { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFim { get; set; }
        public string Obs { get; set; }
    }
}
