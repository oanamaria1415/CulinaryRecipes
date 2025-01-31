﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CulinaryRecipes.Data;
using CulinaryRecipes.Models;
using Microsoft.AspNetCore.Authorization;

namespace CulinaryRecipes.Pages.Recipes
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly CulinaryRecipesContext _context;

        public IndexModel(CulinaryRecipesContext context)
        {
            _context = context;
        }

        public IList<Recipe> Recipe { get; set; } = new List<Recipe>();
        public List<string> ChefList { get; set; } = new List<string>();
        public List<string> CategoryList { get; set; } = new List<string>();

        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string ChefFilter { get; set; } = "All Chefs";

        [BindProperty(SupportsGet = true)]
        public string CategoryFilter { get; set; } = "All Categories";

        public async Task OnGetAsync()
        {
            // Populează listele pentru filtre
            ChefList = await _context.Recipe
                .Select(r => r.ChefName)
                .Distinct()
                .ToListAsync();

            CategoryList = await _context.Recipe
                .Select(r => r.Category)
                .Distinct()
                .ToListAsync();

            // Filtrare și căutare
            var query = _context.Recipe.AsQueryable();

            // Filtrare după bucătar
            if (!string.IsNullOrWhiteSpace(ChefFilter) && ChefFilter != "All Chefs")
            {
                query = query.Where(r => r.ChefName == ChefFilter);
            }

            // Filtrare după categorie
            if (!string.IsNullOrWhiteSpace(CategoryFilter) && CategoryFilter != "All Categories")
            {
                query = query.Where(r => r.Category == CategoryFilter);
            }

            // Bara de căutare (case-insensitive)
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                query = query.Where(r => r.RecipeName.ToLower().Contains(SearchQuery.ToLower()));
            }

            Recipe = await query.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var recipe = await _context.Recipe
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (recipe != null)
            {
                if (recipe.Ingredients != null)
                {
                    _context.Ingredients.RemoveRange(recipe.Ingredients);
                }

                _context.Recipe.Remove(recipe);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Rețeta a fost ștearsă cu succes!";
            }
            else
            {
                TempData["Error"] = "Rețeta nu a fost găsită!";
            }

            return RedirectToPage();
        }
    }
}
