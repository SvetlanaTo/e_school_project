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
using System.Web.Http;
using System.Web.Http.Description;

namespace eSchool.Controllers
{
   
    [RoutePrefix("project/subjects")]
    public class SubjectsController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private ISubjectsService subjectsService;

        public SubjectsController(ISubjectsService subjectsService)
        {
            this.subjectsService = subjectsService;
        }


        /*REST endpoint koji vraća sve predmete (listu)
o putanja /project/subjects
•	metoda je otvorena za sve - spisak predmeta tokom skolovanja */

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("", Name = "GetAllSubjects")]
        public IEnumerable<Subject> GetSubjects()
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Student Collection");

            IEnumerable<Subject> subjects = subjectsService.GetAll();

            logger.Info("Success!");
            return subjects;
        }

        /*REST endpoint koji vraća predmet po vrednosti prosleđenog ID-a
o putanja /project/subjects/{id}
o u slučaju da ne postoji predmet sa prosleđenim id-em vratiti grešku
metoda je otvorena za sve - informacije o predmetu tokom skolovanja 
(SubjectID je info u oceni) */

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("{id:int}", Name = "GetSubjectByID")]
        [ResponseType(typeof(Subject))]
        public IHttpActionResult GetSubject(int id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Subject by id: " + id);

            Subject subject = subjectsService.GetByID(id);

            if (subject == null)
            {
                logger.Info("Subject by id " + id + " was not found.");
                return BadRequest("Subject by id " + id + " was not found.");
            }

            logger.Info("Success!");
            return Ok(subject);
        }

        /*REST endpoint za izmenu postojećeg predmeta 
•	putanja project/subjects/{id:int} 
•	metoda je rezervisana za administratora
•	ukoliko je prosleđen ID koji ne pripada nijednom predmetu metoda treba da vrati grešku, 
a u suprotnom vraća podatke predmeta sa izmenjenim vrednostima
*/
        [Authorize(Roles = "admin")]
        [Route("{id:int}", Name = "UpdateSubject")]
        [ResponseType(typeof(Subject))]
        public HttpResponseMessage PutSubject(int id, [FromBody]PutSubjectDTO updated)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Update for Subject Id: " + id);

            if (updated.Id != id)
            {
                logger.Error("Updated Subject id " + updated.Id + " doesn't match the id " + id + " from the request (route).");
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Updated Subject id " + updated.Id + " doesn't match the id " + id + " from the request (route).");
            }

            try
            {
                Subject saved = subjectsService.Update(id, updated);

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

        /*REST endpoint koji omogućava dodavanje novog predmeta
         o putanja /project/subjects
         o metoda je rezervisana samo za administratora
         o metoda treba da vrati dodati predmet */

        [Authorize(Roles = "admin")]
        [Route("", Name = "CreateSubject")]
        [ResponseType(typeof(Subject))]
        public IHttpActionResult PostSubject([FromBody]PostSubjectDTO newSubject)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Subject Insert");

            try
            {
                Subject saved = subjectsService.Create(newSubject);

                if (saved == null)
                {
                    logger.Info("Failed!");
                    return BadRequest("Failed! Something went wrong.");
                }

                logger.Info("Success!");
                return Ok(saved);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return BadRequest("The Subject in already in the system.");
            }
        }


        /*REST endpoint za brisanje predmeta 
o putanja project/subjects/{id:int}
o metoda je rezervisana za administratora
o ukoliko je prosleđen ID koji ne pripada nijednom predmetu metoda treba da vrati grešku
 /*NAPOMENA: BRISANJE PREDMETA JE MOGUCE SAMO 
  * AKO NE POSTOJI OCENA U TRENUTNOJ SKOLSKOJ GODINI IZ OVOG PREDMETA
  * AKO NASTAVNIK NIJE AKTIVNO ANGAZOVAN NA TOM PREDMETU, TJ. AKO JE DateTime StopperWorking<DateTime.UtcNow */

        [Authorize(Roles = "admin")]
        [Route("{id:int}", Name = "DeleteSubject")]
        [ResponseType(typeof(Subject))]
        public HttpResponseMessage DeleteSubject(int id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Subject Remove for Subject Id: " + id);

            try
            {
                Subject subject = subjectsService.Delete(id);

                if (subject == null)
                {
                    logger.Info("The Subject with id: " + id + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The Subject with id: " + id + " was not found.");
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


        /*REST endpoint koji vraća predmete po pocetnom slovu naziva, sortirane u rastucem redosledu naziva
o putanja /project/subjects/by-name-first-letter
* metoda je otvorena za sve korisnike - nastavni program */

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("by-name-first-letter", Name = "GetSortedSubjectsByFirstLetter")]
        public HttpResponseMessage GetSortedSubjectsByFirstLetter([FromUri]string firstLetter)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Subject Collection - ByFirstLetter: " + firstLetter + " - Sorted Asc");

            if (firstLetter.Length != 1)
            {
                logger.Error("You entered " + firstLetter.Length + " letters. Please put exactly one letter for this search.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, "You entered " + firstLetter.Length + " letters. " +
                    "Please put exactly one letter for this search.");
            }

            try
            {
                IEnumerable<Subject> subjects = subjectsService.GetSortedSubjectsByFirstLetter(firstLetter); 
                logger.Info("Success!");
                return Request.CreateResponse(HttpStatusCode.OK, subjects);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*REST endpoint koji vraća predmete prema zadatom razredu (PREDMETI NA GODINI), sortirane u rastucem redosledu naziva
o putanja /project/subjects/by-grade/{grade:int}
*  metoda je otvorena za sve korisnike - nastavni program */

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("by-grade/{grade:int}", Name = "GetSortedSubjectsByGrade")]
        public HttpResponseMessage GetSortedSubjectsByGrade(int grade)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Subject Collection - By Grade: " + grade + " - Sorted Asc");

            if (grade < 1 || grade > 8)
            {
                logger.Error("The grade must be between 1 and 8. The entered value was " + grade);
                return Request.CreateResponse(HttpStatusCode.BadRequest, "The grade must be between 1 and 8. The entered value was " + grade);
            }

            try
            {
                IEnumerable<Subject> subjects = subjectsService.GetSortedSubjectsByGrade(grade);
                logger.Info("Success!");
                return Request.CreateResponse(HttpStatusCode.OK, subjects);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }


    }
}
