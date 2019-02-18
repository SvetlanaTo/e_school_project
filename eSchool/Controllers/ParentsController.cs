using eSchool.Models;
using eSchool.Models.DTOs;
using eSchool.Services;
using eSchool.Support;
using Microsoft.AspNet.Identity.EntityFramework;
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
    [RoutePrefix("project/parents")]
    public class ParentsController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IParentsService parentsService;
        private ParentToParentDTO toDTO;

        public ParentsController(IParentsService parentsService, ParentToParentDTO toDTO)
        {
            this.parentsService = parentsService;
            this.toDTO = toDTO;
        }

        /*● REST endpoint za pretragu roditelja po unetom JMBGu
       o putanja project/parents/jmbg/{jmbg}
       o metoda treba da vrati roditelja, ukoliko postoji, 
       ili obavestenje da postoji korisnik ali nije roditelj
       ili obavestenje da korisnik nije u sistemu */

        [Authorize(Roles = "admin")]
        [Route("jmbg/{jmbg}", Name = "GetAParentByJmbg")]
        public HttpResponseMessage GetAParentByJmbg(string jmbg)
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
                Parent user = parentsService.GetByJmbg(jmbg);  

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


        /*REST endpoint koji vraća sve roditelje (listu korisnika)
o putanja /project/parents
* Napomena: 
* ukoliko je korisnik student, moze da vidi sve roditelje ucenika svog odeljenja - smanjena dostupnost informacija
* ukoliko je korisnik parent, moze da vidi sve roditelje ucenika svih odeljenja u koja idu njegova deca */

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("", Name = "GetAllParents")]
        public IEnumerable<ParentDTOForStudentAndParents> GetAllParents()
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Parent Collection");

            if (userRole == "admin")
            {
                IEnumerable<ParentDTOForAdmin> users = parentsService.GetAllForAdmin(); 
                logger.Info("Success!");
                return users;
            }
            else if (userRole == "teacher")
            {
                IEnumerable<ParentDTOForTeacher> users = parentsService.GetAllForTeacher();
                logger.Info("Success!");
                return users;
            }
            //moze da vidi sve roditelje ucenika svog razreda
            else if (userRole == "student")
            {
                IEnumerable<ParentDTOForStudentAndParents> users = parentsService.GetAllForStudentFromStudentForm(userId);
                logger.Info("Success!");
                return users;
            }
            //moze da vidi sve roditelje ucenika onih razreda u koja idu njegova deca
            else //if (userRole == "parent")
            {
                IEnumerable<ParentDTOForStudentAndParents> users = parentsService.GetAllForParentFromStudentsForms(userId);
                logger.Info("Success!");
                return users;
            }
        }

        /*REST endpoint koji vraća roditelja po vrednosti prosleđenog ID-a
o putanja /project/parents/{id}
*metoda je namenjena administratorima i nastavnicima, koji mogu da vide osnovne podatke o roditelju uceniku (ucenicki dosijei) 
* ali i studentu ili roditelju
- ukoliko pretrazuje ucenik ili roditelj za sebe, tj. svog roditelja
- ili ukoliko pretrazuje za roditelja ucenika iz svog odeljenja ili odeljenja svog deteta
o u slučaju da ne postoji korisnik sa prosleđenim id-em vratiti grešku*/

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("{id}", Name = "GetParentById")]
        [ResponseType(typeof(Parent))]
        public HttpResponseMessage GetParentById(string id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Parnet by id: " + id);

            try
            {
                Parent parent = parentsService.GetByID(id);

                if (parent == null)
                {
                    logger.Info("The parent with id: " + id + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The parent with id: " + id + " was not found.");
                }
                if (userRole == "admin")
                {
                    logger.Info("Requesting found parent convert for " + userRole + "role.");
                    ParentDTOForAdmin dto = toDTO.ConvertToParentDTOForAdmin(parent, (List<IdentityUserRole>)parent.Roles); 
                    if (dto == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dto);
                }
                else if (userRole == "teacher")
                {
                    logger.Info("Requesting found parent convert for " + userRole + "role.");
                    ParentDTOForTeacher dto = toDTO.ConvertToParentDTOForTeacher(parent); 
                    if (dto == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dto);
                }
                else if (userId == parent.Id
                    || parent.Students.Any(x => x.Id == userId) == true
                   || parent.Students.Any(x => x.Form.Students.Any(y => y.Id == userId)) == true
                   || parent.Students.Any(x => x.Form.Students.Any(y => y.Parent.Id == userId)) == true)
                {
                    logger.Info("Requesting found parent convert for " + userRole + " role.");
                    ParentDTOForStudentAndParents dto = toDTO.ConvertToParentDTOForStudentAndParent(parent);
                    if (dto == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dto);
                }
                else
                {
                    logger.Info("Authorisation failure. User " + userId + " is not authorised for this request.");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access Denied. " +
                        "We’re sorry, but you are not authorized to perform the requested operation.");
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*REST endpoint koji vraća roditelja po vrednosti prosleđenog username-a
o putanja /project/parents/by-username/{username}
*metoda je namenjena administratorima i nastavnicima, koji mogu da vide podatke o roditelju uceniku (ucenicki dosijei) 
* ali i studentu ili roditelju
- ukoliko pretrazuje roditelj za sebe ili ucenik za svog roditelja
- ili ukoliko pretrazuje za roditelja ucenika iz svog odeljenja ili odeljenja svog deteta
o u slučaju da ne postoji korisnik sa prosleđenim id-em vratiti grešku*/

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("by-username/{username}", Name = "GetParentByUserName")]
        [ResponseType(typeof(Parent))]
        public HttpResponseMessage GetParentByUserName(string username)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Parnet by username: " + username);

            try
            {
                Parent parent = parentsService.GetByUserName(username); 

                if (parent == null)
                {
                    logger.Info("The parent with username: " + username + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The parent with username: " + username + " was not found.");
                }
                if (userRole == "admin")
                {
                    logger.Info("Requesting found parent convert for " + userRole + "role.");
                    ParentDTOForAdmin dto = toDTO.ConvertToParentDTOForAdmin(parent, (List<IdentityUserRole>)parent.Roles);
                    if (dto == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dto);
                }
                else if (userRole == "teacher")
                {
                    logger.Info("Requesting found parent convert for " + userRole + "role.");
                    ParentDTOForTeacher dto = toDTO.ConvertToParentDTOForTeacher(parent);
                    if (dto == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dto);
                }
                else if (userId == parent.Id
                    || parent.Students.Any(x => x.Id == userId) == true
                   || parent.Students.Any(x => x.Form.Students.Any(y => y.Id == userId)) == true
                   || parent.Students.Any(x => x.Form.Students.Any(y => y.Parent.Id == userId)) == true)
                {
                    logger.Info("Requesting found parent convert for " + userRole + "role.");
                    ParentDTOForStudentAndParents dto = toDTO.ConvertToParentDTOForStudentAndParent(parent); 
                    if (dto == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dto);
                }
                else
                {
                    logger.Info("Authorisation failure. User " + userId + " is not authorised for this request.");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access Denied. " +
                        "We’re sorry, but you are not authorized to perform the requested operation.");
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*REST endpoint za izmenu postojećeg roditelja 
•	putanja project/parents/{id} 
•	metoda je rezervisana za administratora
•	ukoliko je prosleđen ID koji ne pripada nijednom roditelju metoda treba da vrati grešku, 
a u suprotnom vraća podatke roditelja sa izmenjenim vrednostima
NAPOMENA: metoda vraca gresku ukoliko se pri izmeni unosi JMBG ili USERNAME koji vec postoji u sistemu
u ovoj metodi se ne dozvoljava izmena sifre, niti role 
*/
        [Authorize(Roles = "admin")]
        [Route("{id}", Name = "UpdateParent")]
        [ResponseType(typeof(Parent))] 
        public async Task<HttpResponseMessage> PutParent(string id, [FromBody]PutParentDTO updated) 
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Update for Parent Id: " + id);

          
            if (updated.Id != id)
            {
                logger.Error("Updated parent id " + updated.Id + " doesn't match the id " + id + " from the request (route).");
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Updated " +
                    "parent id " + updated.Id + " doesn't match the id " + id + " from the request (route).");
            }

            try
            {
                ParentDTOForAdmin saved = await parentsService.Update(id, updated);  

                if (saved == null)
                {
                    logger.Info("Failed!");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Failed! Something went wrong.");
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

        /*REST endpoint za brisanje roditelja 
o putanja project/parents/{id}
o metoda je rezervisana za administratora
o ukoliko je prosleđen ID koji ne pripada nijednom korisniku metoda treba da vrati grešku.
NAPOMENA: BRISANJE RODITELJA JE MOGUCE SAMO AKO SE PRETHODNO UCENIK KOJI MU JE DODELJEN, DODELI NEKOM DRUGOM STARATELJU. 
nakon cega se automatski roditelj (bez deteta) brise */

        [Authorize(Roles = "admin")] 
        [Route("{id}", Name = "DeleteParent")]
        [ResponseType(typeof(Parent))]
        public HttpResponseMessage DeleteParent(string id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Parent Remove for Parent Id: " + id);

            try
            {
                Parent user = parentsService.Delete(id);

                if (user == null)
                {
                    logger.Info("The Parent with id: " + id + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The Parent with id: " + id + " was not found.");
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

    }
}
