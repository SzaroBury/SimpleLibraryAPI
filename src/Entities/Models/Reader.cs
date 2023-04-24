using Entities.Enumerations;

namespace Entities.Models
{
    public class Reader
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public string FullName => string.Format("{0} {1}", FirstName, LastName);
    }
}
