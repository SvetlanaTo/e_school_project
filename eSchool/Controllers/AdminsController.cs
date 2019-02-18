using eSchool.Models;
using eSchool.Models.DTOs;
using eSchool.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace eSchool.Controllers
{
    [RoutePrefix("project/admins")]
    public class AdminsController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IAdminsService adminsService;

        public AdminsController(IAdminsService adminsService)
        {
            this.adminsService = adminsService;
        }

        /*REST endpoint koji vraća sve administratore (listu korisnika)
o putanja /project/admins*/

        [Authorize(Roles = "admin")]
        [Route("")]
        public IEnumerable<Admin> GetAllAdmins()
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Admin Collection");

            IEnumerable<Admin> users = adminsService.GetAll();

            return users;
        }

        /*REST endpoint koji vraća administratora po vrednosti prosleđenog ID-a
o putanja /project/admin/{id}
o u slučaju da ne postoji korisnik sa prosleđenim id-em vratiti grešku*/

        [Authorize(Roles = "admin")]
        [Route("{id}")]
        [ResponseType(typeof(Admin))]
        public IHttpActionResult GetAdmin(string id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Admin By ID " + id);

            Admin user = adminsService.GetByID(id);

            if (user == null)
            {
                return BadRequest("Admin by id - not found.");
            }

            return Ok(user);
        }

        /*REST endpoint za izmenu postojećeg admina 
•	putanja project/admins/{id} 
•	metoda je rezervisana za administratora
•	ukoliko je prosleđen ID koji ne pripada nijednom nastavniku metoda treba da vrati grešku, 
a u suprotnom vraća podatke korisnika sa izmenjenim vrednostima
•	NAPOMENA: u okviru ove metode ne menja se vrednost atributa Role, Password
•	Kada se podaci uspesno izmene, AUTOMATSKI SE SALJE MAIL ADMINU, CIME SE ISTI INFORMISA DA JE IZVRSENA IZMENA*/

        [Authorize(Roles = "admin")]
        [Route("{id}", Name = "UpdateAdmin")] 
        [ResponseType(typeof(Admin))]
        public async Task<HttpResponseMessage> PutAdmin(string id, [FromBody]PutAdminDTO updated)    
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserId: " + userId + ": Requesting Update for Admin Id: " + id);
          
            if (updated.Id != id)
            {
                logger.Error("Updated Admin id " + updated.Id + " doesn't match the id " + id + " from the request (route).");
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Updated " +
                    "Admin id " + updated.Id + " doesn't match the id " + id + " from the request (route).");
            }

            try
            {
                Admin saved = await adminsService.Update(id, updated); 

                if (saved == null)
                {
                    logger.Info("Failed!");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Failed!");
                }

                logger.Info("Success!");
                return Request.CreateResponse(HttpStatusCode.OK, saved);

            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*REST endpoint za brisanje administratora 
o putanja project/admin/{id}
o metoda je rezervisana za administratora
o ukoliko je prosleđen ID koji ne pripada nijednom adminu metoda treba da vrati grešku
NAPOMENA: BRISANJE ADMINISTRATORA JE MOGUCE SAMO AKO NIJE JEDINI ADMINISTRATOR U SISTEMU I AKO ON NIJE ULOGOVANI KORISNIK KOJI TO CINI
*/

        [Authorize(Roles = "admin")]
        [Route("{id}", Name = "DeleteAdmin")]
        [ResponseType(typeof(Admin))]
        public HttpResponseMessage DeleteAdmin(string id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Subject Remove for Subject Id: " + id);

            if (userId == id)
            {
                logger.Info("You cannot delete your own account!");
                return Request.CreateResponse(HttpStatusCode.BadRequest, "You cannot delete your own account!");
            }
            try
            {
                Admin admin = adminsService.Delete(id, userId); 

                if (admin == null)
                {
                    logger.Info("The admin with id: " + id + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The admin with id: " + id + " was not found.");
                }
                logger.Info("Success!");
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*REST endpoint koji vraća admina po vrednosti prosleđenog username-a 
             o putanja /project/admins/by-username/{username}
          o u slučaju da ne postoji korisnik sa prosleđenim username-om vratiti grešku
          metoda je rezervisana za administratore */

        [Authorize(Roles = "admin")]
        [Route("by-username/{username}", Name = "GetAdminByUserName")]
        [ResponseType(typeof(Admin))]
        public HttpResponseMessage GetAdminByUserName(string username)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserId: " + userId + ": Requesting admin by username: " + username);

            try 
            {
                Admin admin = adminsService.GetByUserName(username); 

                if (admin == null)
                {
                    logger.Info("The admin with username: " + username + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The admin with username: " + username + " was not found.");
                }

                logger.Info("Success!");
                return Request.CreateResponse(HttpStatusCode.OK, admin);
            }

            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*REST endpoint koji vraća admina po vrednosti prosleđenog jmbg-a 
            o putanja /project/admins/by-jmbg/{jmbg}
         o u slučaju da ne postoji korisnik sa prosleđenim jmbg-om vratiti grešku
         metoda je rezervisana za administratore */

        [Authorize(Roles = "admin")]
        [Route("by-jmbg/{jmbg}", Name = "GetAdminByJmbg")]
        [ResponseType(typeof(Admin))]
        public HttpResponseMessage GetAdminByJmbg(string jmbg)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserId: " + userId + ": Requesting admin by username: " + jmbg);

            try
            {
                Admin admin = adminsService.GetByUserName(jmbg);

                if (admin == null)
                {
                    logger.Info("The admin with username: " + jmbg + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The admin with username: " + jmbg + " was not found.");
                }

                logger.Info("Success!");
                return Request.CreateResponse(HttpStatusCode.OK, admin);
            }

            catch (Exception e) 
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e); 
            }
        }
    }
}
