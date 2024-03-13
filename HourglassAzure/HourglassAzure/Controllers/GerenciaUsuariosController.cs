using HourglassAzure.Data;
using HourglassAzure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Linq;

namespace HourglassAzure.Controllers
{
    public class GerenciaUsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        public GerenciaUsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }
        public ActionResult UsuariosComPerfils()
        {
                                        
           
            //var usuariosComPerfis = (from user in _context.Users
            //                         select new
            //                         {
            //                             UserId = user.Id,
            //                             Username = user.UserName,
            //                             Email = user.Email,

            //                             // select Name from dbo.AspNetRoles where Id in (select RoleId from dbo.AspNetUserRoles where UserId = 'ca767d94-804b-485a-bf4b-7a9f5e681e47');


            //                             //RoleNames = (from userRole in user.Roles
            //                             //             join role in _context.Roles on userRole.RoleId
            //                             //             equals role.Id
            //                             //             select role.Name).ToList()

            //                            //RoleNames = (from userRole in user.Roles
            //                            //            join role in _context.Roles on userRole.RoleId
            //                            //            equals role.Id
            //                            //            select role.Name).ToList()
            //                         }).ToList().Select(p => new UsersInRoleViewModel()
            //                         {
            //                             UserId = p.UserId,
            //                             Username = p.Username,
            //                             Email = p.Email,
            //                             Role = string.Join(",", p.RoleNames)
            //                         });


            return View();
        }
    }
}