
using System.Collections;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class PagedOutputModel<T> where T : IList
    {
        /// <summary>
        /// To support serialization default required.
        /// </summary>
        public PagedOutputModel()
        {

        }
        public T Data { get; }
        public long TotalCount { get; set; }
        public PagedOutputModel(T data, long totalCount)
        {
            Data = data;
            TotalCount = totalCount;
        }
    }
}
