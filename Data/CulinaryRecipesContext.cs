using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CulinaryRecipes.Models;

namespace CulinaryRecipes.Data
{
    public class CulinaryRecipesContext : DbContext
    {
        public CulinaryRecipesContext (DbContextOptions<CulinaryRecipesContext> options)
            : base(options)
        {
        }

        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<CulinaryRecipes.Models.Recipe> Recipe { get; set; } = default!;
    }
}
