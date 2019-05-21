using System.Collections.Generic;

namespace ApplicationPlanner.Transcripts.Web.Models
{
    public class ItemsCountModel<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int Count { get; set; }
    }
}
