using Microsoft.AspNetCore.Identity;

namespace UserRepositoryEF
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string UserName { get; set; }
        public string Passwrod { get; set; }
    }
}