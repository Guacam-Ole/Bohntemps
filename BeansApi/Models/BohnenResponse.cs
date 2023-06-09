namespace Bohntemps.Models
{
    public class BohnenResponse<T>
    {
        public bool Success { get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}