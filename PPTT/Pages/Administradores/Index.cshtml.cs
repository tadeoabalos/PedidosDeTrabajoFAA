using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PPTT.Data;
using PPTT.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Configuration;

namespace PPTT.Pages.Administradores
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly PPTT.Data.DBPPTTContext _context;
        private int datos;
        private IConfiguration? configuration;
        public IndexModel(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
            _configuration = configuration; // Asegúrate de que esto esté presente
        }
        public List<ServicioModel> Items { get; set; } = new List<ServicioModel>();
        public IList<Admin> Admin { get;set; } = default!;
        public class ServicioModel
        {
            public int? ID_Servicio_Fk { get; set; }
        }
        public async Task<IActionResult> OnGetAsync()
        {
            int datos = HttpContext.Session.GetInt32("datos") ?? 0;
            HttpContext.Session.SetInt32("datos", datos);
            if (datos == 0)
            {
                datos = datos + 1;
                HttpContext.Session.SetInt32("datos", datos);
                Console.WriteLine(datos);
                return RedirectToPage("/Administradores/TraerServicio");
            }
            else  
            {
                Admin = await _context.usuario.ToListAsync();
                return Page();
            }
        }
    }
}
