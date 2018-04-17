using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostsApi.Services
{
    public class SiteSettings : ISiteSettings
    {
        public int PageSize => 10;
    }
}
