using System.Collections.Generic;

namespace MusicLibrary.Model
{
    public class ListResult<T>
    {
        public List<T> Items { get; set; }

        public int Count { get; set; }
    }
}
