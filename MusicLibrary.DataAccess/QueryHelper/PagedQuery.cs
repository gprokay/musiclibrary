namespace MusicLibrary.DataAccess.QueryHelper
{
    public class PagedQuery<TColumn>
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public OrderBy<TColumn> OrderBy { get; set; }
    }

    public enum OrderByDirection
    {
        Ascending = 0,
        Descending = 1
    }

    public class OrderBy<TColumn>
    {
        public TColumn Column { get; set; }
        public OrderByDirection Direction { get; set; }
    }
}