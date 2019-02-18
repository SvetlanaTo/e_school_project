using eSchool.Models;
using eSchool.Models.DTOs;
using eSchool.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace eSchool.Controllers
{
    [RoutePrefix("project/account")]
    public class AccountController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IUsersService usersService;

        public AccountController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        /*● REST endpoint za pretragu korisnika po unetom JMBGu
        o putanja project/account/jmbg/{jmbg}
        o metoda treba da vrati korisnika, ukoliko postoji, ili obavestenje da korisnik nije u sistemu */
        [Authorize(Roles = "admin")]
        [Route("jmbg/{jmbg}", Name = "GetAccountByJmbg")]
        public HttpResponseMessage GetAccountByJmbg(string jmbg)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting User Account By Jmbg: " + jmbg);

            if (jmbg.Length != 13)
            {
                logger.Info("The JMBG must be 13 characters long.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, "The JMBG must be 13 characters long. " +
                    "You entered " + jmbg.Length + " characters.");
            }
            try
            {
                ApplicationUser user = usersService.GetByJmbg(jmbg);  

                if (user == null)
                {
                    logger.Info("The user is not in the system.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The user is not in the system.");
                }

                logger.Info("Success!"); 
                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*● REST endpoint za registrovanje (dodavanje) novog administratora
        o putanja project/account/register-admin
        *ne saljemo podatke za logovanje na mejl, posto moze da pristupi ocenama, samo info da je kreiran nalog 
        */

        [Authorize(Roles = "admin")]
        [Route("register-admin", Name = "RegisterAdmin")]
        public async Task<HttpResponseMessage> RegisterAdmin(RegisterAdminDTO adminDTO)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting New Admin Insert");           

            try
            {
                var result = await usersService.RegisterAdmin(adminDTO);

                if (result == null)
                {
                    logger.Info("Failed!");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Failed!");
                }

                logger.Info("Admin registered successfully!");
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*● REST endpoint za registrovanje (dodavanje) novog ucenika i novog roditelja zajedno
       o putanja project/account/register-student-and-parent
       ucenika i roditelja obavestavamo mejlom 
       da je za njih napravljen nalog, da je roditelju dodeljen ucenik i saljemo podatke za logovanje 
       */

        [Authorize(Roles = "admin")]
        [Route("register-student-and-parent", Name = "RegisterStudentAndParent")]
        public async Task<HttpResponseMessage> RegisterStudentAndParent(RegisterStudentDTO studentDTO)  
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting New Student and New Parent Insert");

            try
                {
                var result = await usersService.RegisterStudentAndParent(studentDTO);   

                if (result == null)
                {
                    logger.Info(ModelState);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong");
                    //return BadRequest(ModelState);
                }

                logger.Info("Student (with parent) registered successfully!");
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*● REST endpoint za registrovanje (dodavanje) novog ucenika
         * -salje se ParentJmbg 
      o putanja project/account/register-student
      */
        [Authorize(Roles = "admin")]
        [Route("register-student", Name = "RegisterStudent")]
        public async Task<HttpResponseMessage> RegisterStudent(RegisterStudentDTOAlone studentDTOalone)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting New Student Insert");
       
            try
            {
                IdentityResult result = await usersService.RegisterStudent(studentDTOalone);    

                if (result == null)
                {
                    logger.Info(ModelState);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong");
                }

                logger.Info("Student (with parent) registered successfully!");
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }


        //OVA METODA SE NE KORISTI. U NASOJ APLIKACIJI RODITELJ MORA DA IMA UCENIKA, KREIRA SE S PRVIM svojim ucenikom 
        //[Authorize(Roles = "admin")]
        //[Route("register-parent")]
        //public async Task<IHttpActionResult> RegisterParent(RegisterParentDTO parentDTO)
        //{
        //    string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
        //    logger.Info("UserId: " + userId + ": Requesting New Parent Insert");

        //    var result = await usersService.RegisterParent(parentDTO);

        //    if (result == null)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    return Ok(result);
        //}


        /*● REST endpoint za registrovanje (dodavanje) novog nastavnika
 o putanja project/account/register-teacher
 * saljemo samo info da je kreiran nalog */

        [Authorize(Roles = "admin")]
        [Route("register-teacher", Name = "RegisterTeacher")]
        public async Task<HttpResponseMessage> RegisterTeacher(RegisterTeacherDTO teacherDTO)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting New Teacher Insert");
         
            try
            {
                var result = await usersService.RegisterTeacher(teacherDTO);

                if (result == null)
                {
                    logger.Error(ModelState);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Failed.");
                }

                logger.Info("Teacher registered successfully!");
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /*	REST endpoint koji omogućava izmenu vrednosti atributa lozinka postojećeg korisnika
o putanja  /project/account/change-password/{id}
o ukoliko je prosleđen id koji ne pripada nijednom korisniku metoda treba da vrati grešku
o metodi se prosleđuju stara i nova lozinka, a da bi vrednost atributa lozinka mogla da bude zamenjena novom, 
neophodno je da se vrednost stare lozinke korisnika poklapa sa vrednošću prosleđene stare lozinke */

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("change-password/{id}", Name = "UpdatePasswordFromBody")]
        public async Task<HttpResponseMessage> ChangePassword(string id, [FromBody]ChangePassDTO updated)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Password Update for user id " + id);

            if (updated.Id != id)
            {
                logger.Error("Updated user id " + updated.Id + " doesn't match the id " + id + " from the request (route).");
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Updated " +
                    "user id " + updated.Id + " doesn't match the id " + id + " from the request (route).");
            }

            try
            {
                IdentityResult result = await usersService.ChangePasswordFromBodyAsync(id, updated);

                if (result == null)
                {
                    logger.Error(ModelState);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong");
                }

                logger.Info("Password changed successfully!");
                return Request.CreateResponse(HttpStatusCode.OK, "Password changed successfully!");
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*	REST endpoint koji omogućava izmenu vrednosti atributa lozinka postojećeg korisnika
o putanja  /project/account/change-password/{id}
o ukoliko je prosleđen id koji ne pripada nijednom korisniku metoda treba da vrati grešku
o metodi se prosleđuju stara i nova lozinka, a da bi vrednost atributa lozinka mogla da bude zamenjena sa novom vrednošću, 
neophodno je da se vrednost stare lozinke korisnika poklapa sa vrednošću prosleđene stare lozinke */

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("change-password/{id}", Name = "UpdatePasswordFromUri")]
        public async Task<HttpResponseMessage> ChangePasswordFromUri(string id, [FromUri]string oldPassword, [FromUri]string newPassword)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Password Update for user id " + id);

            try
            {
                IdentityResult result = await usersService.ChangePasswordFromUriAsync(id, oldPassword, newPassword);   

                if (result == null)
                {
                    logger.Error(ModelState);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong");
                }

                logger.Info("Password changed successfully!");
                return Request.CreateResponse(HttpStatusCode.OK, "Password changed successfully!");
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

    }
}

