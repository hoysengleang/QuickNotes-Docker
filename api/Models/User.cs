namespace api.Models {
    public class User {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
    }
    public class LoginDto { public string Username { get; set; } }
}