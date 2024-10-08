﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PPTT.Data;
using PPTT.Models;

namespace PPTT.Pages.Administradores
{
    public class DetailsModel : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;

        public DetailsModel(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
        }

        public Admin Admin { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
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
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Usuario.FirstOrDefaultAsync(m => m.ID_Usuario_Pk == id);
            if (admin == null)
            {
                return NotFound();
            }
            else
            {
                Admin = admin;
            }
            return Page();
            }

        }
    }
}
