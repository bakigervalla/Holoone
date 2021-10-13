using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Helpers
{
    public interface IResponse
    {
        HttpStatusCode StatusCode { get; }

        string Content { get; }

        byte[] RawContent { get; }
    }

}
