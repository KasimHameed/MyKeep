using Marten;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyKeep.Entities.TodoList;

namespace MyKeep.Pages.Todo;

public class Index : PageModel
{
    [BindProperty(SupportsGet = true)] public Guid Id { get; set; }

    public CheckList CheckList { get; set; } = null!;
    
    public async Task<IActionResult> OnGet([FromServices] IQuerySession session)
    {
        var checklist = await session.LoadAsync<CheckList>(Id);
        if (checklist is null) return NotFound();

        CheckList = checklist;

        return Page();
    }
}