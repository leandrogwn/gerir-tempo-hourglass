using HourglassAzure.Data;
using HourglassAzure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace HourglassAzure.Controllers
{
    public class PmoTramitesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PmoTramitesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tramites
        [Authorize(Roles = "Pmo/Scrum, Po")]
        public ActionResult Index(string currentFilter, DateTime strt, DateTime end, string searchString, int? page, int lista)
        {
            if (lista == 0)
            {
                string st = DateTime.Now.ToString("dd/MM/yyyy 00:00");
                DateTime stParse = DateTime.ParseExact(st, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                string en = DateTime.Now.ToString("dd/MM/yyyy 23:59");
                DateTime enParse = DateTime.ParseExact(en, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                ViewBag.Strt = stParse;
                ViewBag.End = enParse;


                var tramites =
                    from tr in _context.Tramite
                    orderby tr.TramiteID descending
                    join al in _context.TramiteDePara on tr.TramiteID equals al.IdTramite into alGrupo
                    where tr.HoraInicio >= stParse && tr.HoraFim <= enParse
                    select new GrupoTramite()
                    {
                        Tramite = tr,
                        DePara = (from alte in alGrupo
                                  orderby alte.TramiteDeParaID descending
                                  select alte).ToList()
                    };

                lista = 10;
                int tamanhoPagina = lista;
                ViewBag.Lista = tamanhoPagina;
                int numeroPagina = page ?? 1;

                var ModelTramite = new TramiteViewModel()
                {
                    Itens = new PagedList<GrupoTramite>(tramites.ToList(), numeroPagina, tamanhoPagina)
                };

                return View(ModelTramite);
            }
            else
            {
                ViewBag.Strt = strt;
                ViewBag.End = end;
                ViewBag.Lista = lista;

                var tramites =
                    from tr in _context.Tramite
                    orderby tr.TramiteID descending
                    join al in _context.TramiteDePara on tr.TramiteID equals al.IdTramite into alGrupo
                    where tr.HoraInicio >= strt && tr.HoraFim <= end
                    select new GrupoTramite()
                    {
                        Tramite = tr,
                        DePara = (from alte in alGrupo
                                  orderby alte.TramiteDeParaID descending
                                  select alte).ToList()
                    };

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;

                if (!String.IsNullOrEmpty(searchString))
                {
                    tramites = tramites.Where(s => s.Tramite.Projeto.ToUpper().Contains(searchString.ToUpper())
                        || s.Tramite.Sprint.ToUpper().Contains(searchString.ToUpper())
                        || s.Tramite.Tarefa.ToString().Equals(searchString)
                        || s.Tramite.Pbi.ToString().Equals(searchString)
                        || s.Tramite.Time.ToUpper().Contains(searchString.ToUpper())
                        || s.Tramite.Email.ToUpper().Contains(searchString.ToUpper())
                        || s.Tramite.Titulo.ToUpper().Contains(searchString.ToUpper())).OrderByDescending(o => o.Tramite.TramiteID);
                }

                int tamanhoPagina = lista;
                int numeroPagina = page ?? 1;

                var ModelTramite = new TramiteViewModel()
                {
                    Itens = new PagedList<GrupoTramite>(tramites.ToList(), numeroPagina, tamanhoPagina)
                };

                return View(ModelTramite);
            }
        }

        // GET: Tramites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tramite = await _context.Tramite
                .SingleOrDefaultAsync(m => m.TramiteID == id);
            if (tramite == null)
            {
                return NotFound();
            }

            return View(tramite);
        }

        // GET: Tramites/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tramites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TramiteID,OrigemTramite,Time,Atividade,Retrabalho,Email,Projeto,Pbi,Tarefa,Titulo,HoraInicio,HoraFim,Obs,TramiteEditado,Sprint")] Tramite tramite)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tramite);
                await _context.SaveChangesAsync();

                foreach (var group in _context.Tramite.Where(x => x.Tarefa == tramite.Tarefa).GroupBy(x => x.Tarefa))
                {
                    var qtdTask = group.Count();
                    string tempoTarefa = Math.Round(group.Sum(x => x.HoraFim.Subtract(x.HoraInicio).TotalHours), 2).ToString();
                    string tempoTarefaComPonto = tempoTarefa.Replace(',', '.');
                    string tempoTarefaComVirgula = tempoTarefa.Replace('.', ',');

                    string connectionUrl = "https://linxfarma.visualstudio.com";
                    string projectName = tramite.Projeto;

                    string token = "mtkfsstc4mys6vtd5buyzf6sklrfujhaciqkh2wpke757dggu3cq";

                    var emailTk = User.Identity.Name;

                    var klmop = _context.Users.AsEnumerable().Where(x => x.UserName == emailTk).Select(s => s.PhoneNumber);

                    if (!string.IsNullOrEmpty(string.Join(";", klmop)))
                    {
                        token = string.Join(";", klmop);
                    }

                    string personalAccessToken = token;

                    string basicAuthHeaderValue = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalAccessToken)));

                    Dictionary<string, string> headers = new Dictionary<string, string>() {
                            { "Content-Type", "application/json-patch+json" }
                        };

                    var jsonTempoTarefa = "[{\"op\":\"add\",\"path\":\"/fields/Custom.LinxEsforcoReal\",\"value\":\"" + tempoTarefaComPonto + "\"},{\"op\":\"add\",\"path\":\"/fields/System.History\",\"value\":\"Tramite criado manualmente por PMO/SM, esforço real atualizado para " + tempoTarefaComVirgula + "h.\"}]";

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthHeaderValue);

                        var batchRequest = new StringContent(jsonTempoTarefa, Encoding.UTF8, "application/json-patch+json");
                        var method = new HttpMethod("PATCH");

                        // send the request
                        var request = new HttpRequestMessage(method, connectionUrl + "/_apis/wit/workitems/" + tramite.Tarefa + "?api-version=2.2") { Content = batchRequest };
                        var response = client.SendAsync(request).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            var stringResponse = response.Content.ReadAsStringAsync();
                            WorkItemBatchPostResponse batchResponse = response.Content.ReadAsAsync<WorkItemBatchPostResponse>().Result;
                        }
                        else
                        {
                            // not successful
                        }
                    }

                }

                return RedirectToAction(nameof(Index));
            }
            return View(tramite);
        }

        // GET: Tramites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tramite = await _context.Tramite.SingleOrDefaultAsync(m => m.TramiteID == id);
            if (tramite == null)
            {
                return NotFound();
            }
            return View(tramite);
        }

        // POST: Tramites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TramiteID,OrigemTramite,Time,Atividade,Retrabalho,Email,Projeto,Pbi,Tarefa,Titulo,HoraInicio,HoraFim,Obs,TramiteEditado,Sprint")] Tramite tramite)
        {
            if (id != tramite.TramiteID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tramite);
                    await _context.SaveChangesAsync();
                    foreach (var group in _context.Tramite.Where(x => x.Tarefa == tramite.Tarefa).GroupBy(x => x.Tarefa))
                    {
                        var qtdTask = group.Count();
                        string tempoTarefa = Math.Round(group.Sum(x => x.HoraFim.Subtract(x.HoraInicio).TotalHours), 2).ToString();
                        string tempoTarefaComPonto = tempoTarefa.Replace(',', '.');
                        string tempoTarefaComVirgula = tempoTarefa.Replace('.', ',');

                        string connectionUrl = "https://linxfarma.visualstudio.com";
                        string projectName = tramite.Projeto;

                        string token = "mtkfsstc4mys6vtd5buyzf6sklrfujhaciqkh2wpke757dggu3cq";

                        var emailTk = User.Identity.Name;

                        var klmop = _context.Users.AsEnumerable().Where(x => x.UserName == emailTk).Select(s => s.PhoneNumber);

                        if (!string.IsNullOrEmpty(string.Join(";", klmop)))
                        {
                            token = string.Join(";", klmop);
                        }

                        string personalAccessToken = token;
                        string basicAuthHeaderValue = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalAccessToken)));

                        Dictionary<string, string> headers = new Dictionary<string, string>() {
                            { "Content-Type", "application/json-patch+json" }
                        };

                        var jsonTempoTarefa = "[{\"op\":\"add\",\"path\":\"/fields/Custom.LinxEsforcoReal\",\"value\":\"" + tempoTarefaComPonto + "\"},{\"op\":\"add\",\"path\":\"/fields/System.History\",\"value\":\"Tramite editado por PMO/SM, esforço real atualizado para " + tempoTarefaComVirgula + "h.\"}]";

                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthHeaderValue);

                            var batchRequest = new StringContent(jsonTempoTarefa, Encoding.UTF8, "application/json-patch+json");
                            var method = new HttpMethod("PATCH");

                            // send the request
                            var request = new HttpRequestMessage(method, connectionUrl + "/_apis/wit/workitems/" + tramite.Tarefa + "?api-version=2.2") { Content = batchRequest };
                            var response = client.SendAsync(request).Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var stringResponse = response.Content.ReadAsStringAsync();
                                WorkItemBatchPostResponse batchResponse = response.Content.ReadAsAsync<WorkItemBatchPostResponse>().Result;
                            }
                            else
                            {
                                // not successful
                            }
                        }

                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TramiteExists(tramite.TramiteID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tramite);
        }

        public class BatchRequest
        {
            public string method { get; set; }
            public Dictionary<string, string> headers { get; set; }
            public object[] body { get; set; }
            public string uri { get; set; }
        }

        internal class WorkItemBatchPostResponse
        {
            public int count { get; set; }
            [JsonProperty("value")]
            public List<Value> values { get; set; }

            public class Value
            {
                public int code { get; set; }
                public Dictionary<string, string> headers { get; set; }
                public string body { get; set; }
            }
        }

        // GET: Tramites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tramite = await _context.Tramite
                .SingleOrDefaultAsync(m => m.TramiteID == id);
            if (tramite == null)
            {
                return NotFound();
            }

            return View(tramite);
        }

        // POST: Tramites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tramite = await _context.Tramite.SingleOrDefaultAsync(m => m.TramiteID == id);
            _context.Tramite.Remove(tramite);
            await _context.SaveChangesAsync();


            foreach (var group in _context.Tramite.Where(x => x.Tarefa == tramite.Tarefa).GroupBy(x => x.Tarefa))
            {
                var qtdTask = group.Count();
                string tempoTarefa = Math.Round(group.Sum(x => x.HoraFim.Subtract(x.HoraInicio).TotalHours), 2).ToString();
                string tempoTarefaComPonto = tempoTarefa.Replace(',', '.');
                string tempoTarefaComVirgula = tempoTarefa.Replace('.', ',');

                string connectionUrl = "https://linxfarma.visualstudio.com";
                string projectName = tramite.Projeto;

                string token = "mtkfsstc4mys6vtd5buyzf6sklrfujhaciqkh2wpke757dggu3cq";

                var emailTk = User.Identity.Name;

                var klmop = _context.Users.AsEnumerable().Where(x => x.UserName == emailTk).Select(s => s.PhoneNumber);

                if (!string.IsNullOrEmpty(string.Join(";", klmop)))
                {
                    token = string.Join(";", klmop);
                }

                string personalAccessToken = token;
                string basicAuthHeaderValue = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalAccessToken)));

                Dictionary<string, string> headers = new Dictionary<string, string>() {
                            { "Content-Type", "application/json-patch+json" }
                        };

                var jsonTempoTarefa = "[{\"op\":\"add\",\"path\":\"/fields/Custom.LinxEsforcoReal\",\"value\":\"" + tempoTarefaComPonto + "\"},{\"op\":\"add\",\"path\":\"/fields/System.History\",\"value\":\"Tramite deletado por PMO/SM, esforço real atualizado para " + tempoTarefaComVirgula + "h.\"}]";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthHeaderValue);

                    var batchRequest = new StringContent(jsonTempoTarefa, Encoding.UTF8, "application/json-patch+json");
                    var method = new HttpMethod("PATCH");

                    // send the request
                    var request = new HttpRequestMessage(method, connectionUrl + "/_apis/wit/workitems/" + tramite.Tarefa + "?api-version=2.2") { Content = batchRequest };
                    var response = client.SendAsync(request).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var stringResponse = response.Content.ReadAsStringAsync();
                        WorkItemBatchPostResponse batchResponse = response.Content.ReadAsAsync<WorkItemBatchPostResponse>().Result;
                    }
                    else
                    {
                        // not successful
                    }
                }

            }


            return RedirectToAction(nameof(Index));
        }

        private bool TramiteExists(int id)
        {
            return _context.Tramite.Any(e => e.TramiteID == id);
        }
    }
}
