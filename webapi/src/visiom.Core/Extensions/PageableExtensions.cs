namespace visiom.Core.Extensions
{
    public static class PageableExtensions
    {
        public static (int Skip, int Take) ToSlice(this IPageable page)
            => ((page.Page - 1) * page.PageSize, page.PageSize);
    }
}
