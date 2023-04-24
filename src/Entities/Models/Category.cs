using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public  class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; } = "";
        public string Description { get; set; } = "";
        public int? ParentCategoryId { get; set; }
    }
}
