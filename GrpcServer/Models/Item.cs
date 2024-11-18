namespace GrpcServer.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int JackpotValue { get; set; }
        public string? Time { get; set; }
    }
}
