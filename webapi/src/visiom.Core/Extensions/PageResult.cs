namespace visiom.Core.Extensions
{
    public class PageResult<T>
    {
        public PageResult(IEnumerable<T> items, int count, string nextPageToken = null)
        {
            Items = items;
            Count = count;
            NextPageToken = nextPageToken;
        }

        public int Count { get; set; }
        public IEnumerable<T> Items { get; set; }
        public string NextPageToken { get; set; }
    }
}