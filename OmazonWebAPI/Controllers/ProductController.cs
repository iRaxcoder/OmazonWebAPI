using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OmazonWebAPI.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmazonWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        
        [HttpPost]
        [Route("SendProviderRequest")]
        public IActionResult DoRequest(String serviceResponse)
        {
            var request  = JsonConvert.DeserializeObject<ServiceResponse>(serviceResponse);

            
        }

        
    }
}
