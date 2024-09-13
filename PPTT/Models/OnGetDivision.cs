using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;

public class CreateModel : PageModel
{
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(ILogger<CreateModel> logger)
    {
        _logger = logger;
    }

    public JsonResult OnGetDivision(int division)
    {
        _logger.LogInformation("OnGetDivision handler called with division: {division}", division);

        var servicios = new List<SelectListItem>();
        if (division == 1)
        {
            servicios.Add(new SelectListItem { Value = "1", Text = "Servicio A" });
            servicios.Add(new SelectListItem { Value = "2", Text = "Servicio B" });
        }
        else if (division == 2)
        {
            servicios.Add(new SelectListItem { Value = "3", Text = "Servicio C" });
            servicios.Add(new SelectListItem { Value = "4", Text = "Servicio D" });
        }

        return new JsonResult(servicios);
    }
}
