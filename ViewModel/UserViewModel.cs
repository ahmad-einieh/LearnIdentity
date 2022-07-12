namespace WebApplication1.ViewModel
{
    public class UserViewModel
    {
        public  string Id { get; set; }
        public string name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }   
        public IEnumerable<string> Roles { get; set; }
    }
}
