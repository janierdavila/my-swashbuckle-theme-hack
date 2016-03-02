using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace WebApplication3.Controllers
{
    /// <summary>
    /// Default Enpoint to test the API
    /// </summary>
    public class DefaultController : ApiController
    {
        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <remarks>Every value given as an array of strings</remarks>
        [ResponseType(typeof(List<string>))]
        public IHttpActionResult GetValues()
        {
            var a = new string[] { "janier", "davila"};
            return Ok(a);
        }

        /// <summary>
        /// Post the values.
        /// </summary>
        /// <remarks>Every value given as an array of strings</remarks>
        [ResponseType(typeof(List<string>))]
        public IHttpActionResult PostValues([FromBody] List<int> ids)
        {
            var a = new string[] { "janier", "davila" };
            return Ok(a);
        }
    }
}
