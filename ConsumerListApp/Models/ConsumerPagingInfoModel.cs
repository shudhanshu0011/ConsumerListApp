using X.PagedList;

namespace ConsumerListApp.Models
{
    public class ConsumerPagingInfoModel
    {
        public int? pageSize;
        public int sortBy;
        public string Search;
        public bool isAsc { get; set; }
        public StaticPagedList<ApiResModel> Consumers { get; set; }
    }
}
