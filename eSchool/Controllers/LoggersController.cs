using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace eSchool.Controllers
{
    [RoutePrefix("project/logs")]
    public class LoggersController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        [Authorize(Roles = "admin")]
        [Route("from/{id}/days-ago", Name = "GetLog")]
        public IHttpActionResult GetLog(int id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting log file from " + id + " days ago.");

            try
            {
                string content = string.Empty;
                string path = null;
                if (id == 0)
                {
                    path = "app-log.txt";
                }
                else if (id == 1)
                {
                    path = "archives/app-log.0.txt";
                }
                else if (id == 2)
                {
                    path = "archives/app-log.1.txt";
                }
                else if (id == 3)
                {
                    path = "archives/app-log.2.txt";
                }
                else if (id == 4)
                {
                    path = "archives/app-log.3.txt";
                }
                else if (id == 5)
                {
                    path = "archives/app-log.4.txt";
                }
                else if (id == 6)
                {
                    path = "archives/app-log.5.txt";
                }
                else if (id == 7)
                {
                    path = "archives/app-log.6.txt";
                }
                else
                    return BadRequest("The archive doesn't log files older than 7 days.");

                using (StreamReader stream = new StreamReader(HttpContext.Current.Server.MapPath("~/logs/" + path)))
                {
                    content = stream.ReadToEnd();
                }
                logger.Info("Success!");
                return Ok(content);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return BadRequest("Something went wrong. Couldn't find the file.");
            }

        }

    }
}

