namespace Art_Gallery.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public bool Role { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public void Register(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
