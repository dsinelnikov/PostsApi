using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostsApi.Services
{
    public interface ISiteSettings
    {
        int PageSize { get; }
    }
}
