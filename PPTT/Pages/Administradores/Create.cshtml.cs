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
            return Page();
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
            List<SelectListItem> servicios = [];

            if (division == "2")
            {
                servicios = [                    
                     new () { Value = "Plomeria", Text = "Plomería" },
                     new () { Value = "Carpinteria", Text = "Carpintería" }
                    ];
            }
            else if (division == "3")
            {
                servicios = [
                    new () { Value = "Telefonia", Text = "Telefonía" },
                    new () { Value = "Redes", Text = "Redes" }
                    ];              
            }

            return new JsonResult(servicios);
        }
    }
}
