using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PostsApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Ping")]
    public class PingController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return "Alive";
        }
    }
}