using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HourglassAzure.Data;
using HourglassAzure.Models;

namespace HourglassAzure.Controllers
{
    [Produces("application/json")]
    [Route("api/ApiTramites")]
    public class ApiTramitesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApiTramitesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ApiTramites
        [HttpGet]
        public IEnumerable<Tramite> GetTramite()
        {
            return _context.Tramite;
        }

        //GET: api/ApiTramites/5252
        [ResponseCache(Duration = 1, Location = ResponseCacheLocation.None)]
        [HttpGet("timetask/{tarefa}/{randon}")]
        public String GetTimeTask([FromRoute] int tarefa)
        {
            var TempoTotalTarefa = "";
            foreach (var group in _context.Tramite.Where(x => x.Tarefa == tarefa).GroupBy(x => x.Tarefa))
            {
                TempoTotalTarefa = Math.Round(group.Sum(x => x.HoraFim.Subtract(x.HoraInicio).TotalHours), 2).ToString();
            }

            return TempoTotalTarefa;
        }

        // PUT: api/ApiTramites/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTramite([FromRoute] int id, [FromBody] Tramite tramite)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tramite.TramiteID)
            {
                return BadRequest();
            }

            _context.Entry(tramite).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TramiteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ApiTramites
        [HttpPost]
        public async Task<IActionResult> PostTramite([FromBody] Tramite tramite)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Tramite.Add(tramite);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTramite", new { id = tramite.TramiteID }, tramite);
        }

        // DELETE: api/ApiTramites/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTramite([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tramite = await _context.Tramite.SingleOrDefaultAsync(m => m.TramiteID == id);
            if (tramite == null)
            {
                return NotFound();
            }

            _context.Tramite.Remove(tramite);
            await _context.SaveChangesAsync();

            return Ok(tramite);
        }

        private bool TramiteExists(int id)
        {
            return _context.Tramite.Any(e => e.TramiteID == id);
        }
    }
}