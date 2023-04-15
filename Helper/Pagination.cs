using System.Collections.Generic;

namespace MarketPlace.API.Helper
{
    public class Pagination<T> where T : class
    {
        public int Count { get; set; }

        public List<T> Data { get; set; }

        public Pagination(int count, List<T> data)
        {
            this.Data = data;
            this.Count = count;
        }
    }

}