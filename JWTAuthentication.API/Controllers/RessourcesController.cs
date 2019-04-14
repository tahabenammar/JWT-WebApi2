using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JWTAuthentication.API.Controllers
{
    public class RessourcesController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok(new { value = "value1" });
        }
    }
}
