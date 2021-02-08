using Data.Model.Response;
using System.Collections.Generic;

namespace Api
{
    /// <summary>
    /// Generioc response item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ItemResponseModel<T>: ItemStorageModel<T>, IResponseItem where T: class
    {
        public List<IApiLink> Links { get; set; } = new List<IApiLink>();
        public T Item { get; set; }
    }
}
