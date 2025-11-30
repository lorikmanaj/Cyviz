namespace Cyviz.Core.Application.Models.Pagination
{
    public class KeysetPageResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public string? NextCursor { get; set; }
    }
}
