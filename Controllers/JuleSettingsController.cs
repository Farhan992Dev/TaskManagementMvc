using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementMvc.Data;
using TaskManagementMvc.Models;
using TaskManagementMvc.Models.ViewModels;

namespace TaskManagementMvc.Controllers
{
    [Authorize(Policy = Permissions.ManageSystem)]
    public class JuleSettingsController : Controller
    {
        private readonly TaskManagementContext _context;

        public JuleSettingsController(TaskManagementContext context)
        {
            _context = context;
        }

        // GET: JuleSettings
        public async Task<IActionResult> Index()
        {
            var apiKeySetting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == "JuleApiKey");
            var model = new JuleSettingsViewModel
            {
                ApiKey = apiKeySetting?.Value
            };
            return View(model);
        }

        // POST: JuleSettings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(JuleSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var apiKeySetting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == "JuleApiKey");
                if (apiKeySetting == null)
                {
                    apiKeySetting = new Setting { Key = "JuleApiKey" };
                    _context.Settings.Add(apiKeySetting);
                }
                apiKeySetting.Value = model.ApiKey;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Jule settings saved successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error saving Jule settings: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
