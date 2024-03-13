using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HourglassAzure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HourglassAzure.Controllers
{
    public class TramiteDeParaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TramiteDeParaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TramiteDePara
        public ActionResult Index(int idTramite)
        {
            var tramitesDePara = from t in _context.TramiteDePara where t.IdTramite == idTramite orderby t.TramiteDeParaID descending select t;

            return View(tramitesDePara);
        }

        // GET: TramiteDePara/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TramiteDePara/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TramiteDePara/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TramiteDePara/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TramiteDePara/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TramiteDePara/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TramiteDePara/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}