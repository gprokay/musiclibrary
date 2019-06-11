namespace MusicLibrary.Model
{
    public enum OrderDirection
    {
        Ascending = 0,
        Descending = 1
    }

    public class OrderModel<T>
    {
        public T Column { get; set; }

        public OrderDirection Direction { get; set; }
    }

    public class Page<T>
    {
        public int? Skip { get; set; }

        public int? Take { get; set; }

        public OrderModel<T> OrderBy { get; set; }
    }
}
