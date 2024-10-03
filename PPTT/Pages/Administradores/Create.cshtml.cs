﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using PPTT.Data;
using PPTT.Models;

namespace PPTT.Pages.Administradores
{
    public class CreateModel : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;

        public CreateModel(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Admin Admin { get; set; } = default!;
        public List<Division> Divisions { get; set; } = new List<Division>(); 
        public List<Servicio> Servicios { get; set; } = new List<Servicio>(); 

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Admin.ID_Rol_Fk = 1;
            Admin.Fecha_Alta = DateTime.Now;
            _context.Usuario.Add(Admin);

            await _context.SaveChangesAsync();
         
            return RedirectToPage("./Index");
        } 
        public async Task<IActionResult> OnGetAsync()
        {
            //int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
            //HttpContext.Session.SetInt32("UserRole", _rol);

            //if (_rol < 2)
            //{
            //    return RedirectToPage("/Index");
            //}
            //else if (_rol > 1)
            //{
            //    return Page();
            //}
            //else
            //{
            //    ModelState.AddModelError(string.Empty, "Rol no reconocido.");
            //    return Page();
            //}
            Divisions = await _context.GetDivisionAsync(); 
            return Page();
        }
                  
        public async Task<JsonResult> OnGetServiciosByDivisionAsync(string division)
        {
            var servicios = await _context.GetServiciosAsync(int.Parse(division));
            return new JsonResult(servicios);
        }
    }
}
