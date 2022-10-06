using System.Collections.Generic;

namespace Data.Model.Response
{
    public interface IApiLinks
    {
        IEnumerable<IApiLink> Links { get; set; }
    }
}
