using System.Collections.Generic;

namespace Data.Model.Response
{
    public interface IApiLinks
    {
        List<IApiLink> Links { get; set; }
    }
}
