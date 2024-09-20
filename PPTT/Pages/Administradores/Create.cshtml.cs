using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IActionResult OnGet()
        {
            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
            HttpContext.Session.SetInt32("UserRole", _rol);

            if (_rol < 2)
            {
                return RedirectToPage("/Index");
            }
            else if (_rol > 1)
            {
                return Page();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Rol no reconocido.");
                return Page();
            }
        }

        [BindProperty]
        public Admin Admin { get; set; } = default!;        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Usuarios.Add(Admin);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");

        }

        public JsonResult OnGetServiciosByDivision(string division)
        {            
            List<SelectListItem> servicios = new List<SelectListItem>();

            if (division == "2")
            {
                servicios = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Plomeria", Text = "Plomería" },
                    new SelectListItem { Value = "Carpinteria", Text = "Carpintería" }
                };
            }
            else if (division == "3")
            {
                servicios = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Telefonia", Text = "Telefonía" },
                    new SelectListItem { Value = "Redes", Text = "Redes" }
                };
            }

            return new JsonResult(servicios);
        }
    }
}
