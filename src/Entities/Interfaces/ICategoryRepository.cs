using Entities.Enumerations;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Interfaces
{
    public interface ICategoryRepository
    {
        public List<Category> GetCategories();
        public Category GetCategory(int id);
        public void CreateCategory(Category category);
        public void UpdateCategory(Category category);
        public void DeleteCategory(int id);
    }
}
