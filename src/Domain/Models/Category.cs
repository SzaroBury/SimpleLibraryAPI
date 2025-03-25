using System.ComponentModel.DataAnnotations;

namespace SimpleLibrary.Domain.Models;

public  class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required] public required string Name { get; set; }
    public string Tags { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Guid? ParentCategoryId { get; set; }
    public virtual Category? ParentCategory { get; set; }

    public virtual ICollection<Book> Books { get; set; } = [];
    public virtual ICollection<Category> Subcategories { get; set; } = [];
}