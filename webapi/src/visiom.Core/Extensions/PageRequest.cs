using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace visiom.Core.Extensions
{
    public class PageRequest : IPageable
    {
        [FromHeader]
        [Range(1, int.MaxValue, MinimumIsExclusive = false)]
        public required int Page { get; set; }

        [FromHeader]
        [Range(1, int.MaxValue, MinimumIsExclusive = false)]
        public required int PageSize { get; set; }
    }
}
