namespace ConsumerListApp.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        public bool IsAdmin = false;

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int PhoneNumber { get; set; }

        public string Passwords { get; set; }

        public string UserName { get; set; }
    }
}
