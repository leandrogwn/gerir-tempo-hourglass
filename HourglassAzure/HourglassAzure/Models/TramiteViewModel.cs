using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace HourglassAzure.Models
{
    public class TramiteViewModel
    {
        public TramiteViewModel(IPagedList<object> pagedList)
        {
        }
        public TramiteViewModel()
        {
        }

        public IPagedList<GrupoTramite> Itens { get; set; }

    }

    public class GrupoTramite
    {
        public Tramite Tramite { get; set; }
        public List<TramiteDePara> DePara { get; set; }
    }
}
