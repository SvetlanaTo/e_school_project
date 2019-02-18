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
    [RoutePrefix("project/teachers")]
    public class TeachersController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private ITeachersService teachersService;
        private TeacherToTeacherDTO toDTO;

        public TeachersController(ITeachersService teachersService, TeacherToTeacherDTO toDTO)
        {
            this.teachersService = teachersService;
            this.toDTO = toDTO;
        }

        /*REST endpoint koji vraća sve nastavnike (listu korisnika)
o putanja /project/teachers
*metoda je otvorena za sve
* nastavnici, studenti i roditelji mogu da vide samo TRENUTNO ZAPOSLENE NASTAVNIKE (ceo kolektiv)
* ali je kolicina informacija koja je na raspolaganju srazmerna roli
*/

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("", Name = "GetAllTeachers")]
        public IEnumerable<TeacherDTOForStudentAndParent> GetAllTeachers()
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Student Collection");

            if (userRole == "admin")
            {
                IEnumerable<TeacherDTOForAdmin> users = teachersService.GetAllForAdmin();
                logger.Info("Success!");
                return users;
            }
            else if (userRole == "teacher")
            {
                IEnumerable<TeacherDTOForTeacher> users = teachersService.GetAllForTeacher();
                logger.Info("Success!");
                return users;
            }
            else //if (userRole == "student" || userRole == "parent")
            {
                IEnumerable<TeacherDTOForStudentAndParent> users = teachersService.GetAllForStudentAndParent();
                logger.Info("Success!");
                return users;
            }
        }

        /*REST endpoint koji vraća nastavnika po vrednosti prosleđenog ID-a
     o putanja /project/teachers/{id}
     o u slučaju da ne postoji korisnik sa prosleđenim id-em vratiti grešku
     •	metoda je otvorena za sve
     nastavnici, studenti i roditelji mogu da vide smo TRENUTNO ZAPOSLENE NASTAVNIKE (ceo kolektiv), 
     ali je kolicina dostupnih informacija srazmerna roli 
     - studenti i roditelji ce kroz ocenu dobijati id-eve nastavnika, te koristiti ovu pretragu za dodatne informacije*/

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("{id}", Name = "GetTeacherById")]
        [ResponseType(typeof(Teacher))]
        public HttpResponseMessage GetTeacherById(string id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Teacher by id: " + id);

            try
            {
                Teacher teacher = teachersService.GetById(id);
                if (teacher == null)
                {
                    logger.Info("The teacher with id: " + id + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The teacher with id: " + id + " was not found.");
                }
                if (userRole == "admin")
                {
                    logger.Info("Requesting found teacher convert for " + userRole + "role.");
                    TeacherDTOForAdmin dto = toDTO.ConvertToTeacherDTOForAdmin(teacher, (List<IdentityUserRole>)teacher.Roles);
                    if (dto == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dto);
                }
                else if (userRole == "teacher" && teacher.IsStillWorking == true)
                {
                    logger.Info("Requesting found teacher convert for " + userRole + "role.");
                    TeacherDTOForTeacher dto = toDTO.ConvertToTeacherDTOForTeacher(teacher);
                    if (dto == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dto);
                }
                else if (teacher.IsStillWorking == true && (userRole == "student" || userRole == "parent"))
                {
                    logger.Info("Requesting found teacher convert for " + userRole + "role.");
                    TeacherDTOForStudentAndParent dto = toDTO.ConvertToTeacherDTOForStudentAndParent(teacher);
                    if (dto == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dto);
                }
                else //zbog provere teacher.IsStillWorking
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
        /*REST endpoint koji vraća nastavnika po vrednosti prosleđenog username-a 
              o putanja /project/teachers/by-username/{username}
           o u slučaju da ne postoji korisnik sa prosleđenim username-om vratiti grešku
           metoda je otvorena za sve role, 
            nastavnici, studenti i roditelji mogu da vide smo TRENUTNO ZAPOSLENE NASTAVNIKE (ceo kolektiv),
            samo je kolicina informacija koja se vidi, srazmerna tipu role*/

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("by-username/{username}", Name = "GetTeacherByUserName")]
        [ResponseType(typeof(Teacher))]
        public HttpResponseMessage GetTeacherByUserName(string username)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Teacher by username: " + username);

            try
            {
                Teacher teacher = teachersService.GetByUserName(username);

                if (teacher == null)
                {
                    logger.Info("The teacher with username: " + username + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The teacher with username: " + username + " was not found.");
                }
                if (userRole == "admin")
                {
                    logger.Info("Requesting found teacher convert for " + userRole + "role.");
                    TeacherDTOForAdmin dto = toDTO.ConvertToTeacherDTOForAdmin(teacher, (List<IdentityUserRole>)teacher.Roles);
                    if (dto == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dto);
                }
                else if (userRole == "teacher" && teacher.IsStillWorking == true)
                {
                    logger.Info("Requesting found teacher convert for " + userRole + "role.");
                    TeacherDTOForTeacher dto = toDTO.ConvertToTeacherDTOForTeacher(teacher);
                    if (dto == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dto);
                }
                else if (teacher.IsStillWorking == true && (userRole == "student" || userRole == "parent"))
                {
                    logger.Info("Requesting found teacher convert for " + userRole + "role.");
                    TeacherDTOForStudentAndParent dto = toDTO.ConvertToTeacherDTOForStudentAndParent(teacher);
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

        /*REST endpoint za izmenu postojećeg nastavnika 
•	putanja project/teachers/{id} 
•	metoda je rezervisana za administratora
•	ukoliko je prosleđen ID koji ne pripada nijednom nastavniku metoda treba da vrati grešku, 
a u suprotnom vraća podatke korisnika sa izmenjenim vrednostima
•	NAPOMENA: u okviru ove metode ne menja se vrednost atributa Role, Password, niti FormAttending
•	Kada se podaci uspesno izmene, AUTOMATSKI SE SALJE MAIL NASTAVNIKU, CIME SE ISTI INFORMISE O IZMEDJENIM VREDNOSTIMA*/

        [Authorize(Roles = "admin")]
        [Route("{id}", Name = "UpdateTeacher")]
        [ResponseType(typeof(Teacher))]
        public async Task<HttpResponseMessage> PutTeacher(string id, [FromBody]PutTeacherDTO updated)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Update for Teacher Id: " + id);

            if (updated.Id != id)
            {
                logger.Error("Updated Teacher id " + updated.Id + " doesn't match the id " + id + " from the request (route).");
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Updated " +
                    "Teacher id " + updated.Id + " doesn't match the id " + id + " from the request (route).");
            }

            try
            {
                TeacherDTOForAdmin saved = await teachersService.Update(id, updated);

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

        //nastavnik ne moze da se obrise ako je vezan za ocenu, predlaze se prekid radnog odnosa
        //a za to je potrebno odeljenje povezati sa drugim razrednim staresinom i sva angazovanja na predmetu i po odeljenjima stopirati (datetimeNow)
        /*REST endpoint za brisanje postojećeg nastavnika 
o putanja project/teachers/{id}
o metoda je rezervisana za administratora
o ukoliko je prosleđen ID koji ne pripada nijednom korisniku metoda treba da vrati grešku.
NAPOMENA: BRISANJE NASTAVNIKA JE MOGUCE SAMO AKO 
NIJE DAO OCENU, NIJE AKTIVNO ANGAZOVAN NA PREDMETU,  I AKO NIJE RAZREDNI STARESINA 
u suprotnom, neophodno je razresiti ga datih veza. */

        [Authorize(Roles = "admin")]
        [Route("{id}", Name = "DeleteTeacher")]
        [ResponseType(typeof(Teacher))]
        public HttpResponseMessage DeleteTeacher(string id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Teacher Remove for Teacher Id: " + id);

            try
            {
                Teacher user = teachersService.Delete(id);

                if (user == null)
                {
                    logger.Info("The Teacher with id: " + id + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The Teacher with id: " + id + " was not found.");
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

        /*REST endpoint koji vraća nastavnike prema zadatom prezimenu koji su trenutno zaposleni, sortirane u rastucem redosledu imena,
 o putanja /project/teachers/by-last-name
 * metoda je otvorena za sve korisnike, samo je dostupnost informacija srazmerna tipu korisnika */

        [Authorize(Roles = "admin, teacher, parent, student")]
        [Route("by-last-name", Name = "GetTeachersByLastNameSorted")]
        public HttpResponseMessage GetWorkingTeachersByLastNameSorted([FromUri]string lastName)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Teacher Collection - " +
                "By Last Name: " + lastName + " - Sorted Asc By Name");

            try
            {
                IList<Teacher> teachers = teachersService.GetWorkingTeachersByLastNameSorted(lastName);
                if (teachers.Count == 0)
                {
                    logger.Info("Teachers by last name: " + lastName + " were not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Teachers by last name: " + lastName + ", were not found.");
                }
                if (userRole == "admin")
                {
                    IEnumerable<TeacherDTOForAdmin> dtos = toDTO.ConvertToTeacherDTOListForAdmin((List<Teacher>)teachers);

                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dtos);
                }
                else if (userRole == "teacher")
                {
                    IEnumerable<TeacherDTOForTeacher> dtos = toDTO.ConvertToTeacherDTOListForTeacher((List<Teacher>)teachers);

                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dtos);
                }
                else
                {
                    IEnumerable<TeacherDTOForStudentAndParent> dtos = toDTO.ConvertToTeacherDTOListForStudentAndParent((List<Teacher>)teachers);

                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dtos);
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*REST endpoint koji omogućava prekidanje radnog ugovora jednog nastavnika 
        * cime se postavlja trenutni datum za propertije 
        * StoppedTeaching ZA SVE PREDMETE (TeacherToSubject) i Stopped ZA SVA ODELJENJA (FormToTeacherSubject)         
*I SALJE OBAVESTENJE DA JE NEOPHODNO POSTAVITI NOVOG RAZREDNOG ODELJENJU, ukoliko je teacher id attending teacher

      o putanja /project/teachers/{teacherId}/stopped-working
      o metoda je rezervisana samo za administratora
      o metoda treba da vrati promenjenog nastavnika */

        [HttpPut]
        [Authorize(Roles = "admin")]
        [Route("{id}/stopped-working", Name = "StoppedWorkingAndEndedAllEngagementsInTSAndFTSByTeacherId")]
        [ResponseType(typeof(TeacherToSubject))]
        public HttpResponseMessage StoppedWorkingAndEndedAllEngagementsInTSAndFTSByTeacherId(string id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting IsStillWorking Update to False For Teacher with Id: " + id + " and" +
                " updating All engagements in TeacherToSubject and FormToTeacherSubject Tables.");

            try
            {
                TeacherDTOForAdmin saved = teachersService.StoppedWorkingAndEndedAllEngagementsInTSAndFTSByTeacherId(id);

                if (saved == null)
                {
                    logger.Info("The Teacher with id: " + id + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The Teacher with id: " + id + " was not found.");
                }

                logger.Info("Success!");
                return Request.CreateResponse(HttpStatusCode.OK, saved);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        //ispis je StudentTeacherDTOItems = student podaci + lista njegovih teachera

        /*REST endpoint koji vraća nastavnike koji TRENUTNO AKTIVNO predaju odredjenom studentu, sortirane u rastucem redosledu imena,
 o putanja /project/teachers/by-student-username/{studentUserName}
 * metoda je otvorena za sve korisnike
 * admin i nastavnici mogu da vide sve, dok student ukoliko pretrazuje za sebe, a roditelj za svoje dete */

        [Authorize(Roles = "admin, teacher, parent, student")]
        [Route("by-student-username/{studentUserName}", Name = "GetTeachersByStudentUserName")]
        public HttpResponseMessage GetTeachersByStudentUserName([FromUri]string studentUserName)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Teacher Collection - " +
                "By student User Name: " + studentUserName + " - Sorted Asc By Name");

            try
            {
                if (userRole == "admin" || userRole == "teacher")
                {
                    StudentTeacherDTOItems teachers = teachersService.GetTeachersByStudentUserName(studentUserName);
                    if (teachers == null)
                    {
                        logger.Info("Teachers by student User Name: " + studentUserName + " were not found.");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Teachers by student User Name: " + studentUserName + " were not found.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, teachers);
                }
                else if (userRole == "student")
                {
                    StudentTeacherDTOItems teachers = teachersService.GetTeachersByStudentUserName(studentUserName);
                    if (teachers == null || teachers.Id != userId)
                    {
                        logger.Info("Authorisation failure. User " + userId + " is not authorised for this request.");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access Denied. " +
                            "We’re sorry, but you are not authorized to perform the requested operation.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, teachers);
                }
                else
                {
                    StudentTeacherDTOItems teachers = teachersService.GetTeachersByStudentUserNameForParent(studentUserName, userId);
                    if (teachers == null)
                    {
                        logger.Info("Failed.");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Failed.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, teachers);
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        //.Substring(0, 3).ToLower().Equals(startsWith.ToLower())
        /*REST endpoint koji vraća zaposlene nastavnike po vrednosti prosledjena pocetna tri slova ime ili prezimena
    o putanja /project/teachers/starts-with
    •	metoda je otvorena za sve
    nastavnici, studenti i roditelji mogu da vide smo TRENUTNO ZAPOSLENE NASTAVNIKE (ceo kolektiv), 
*/

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("starts-with", Name = "GetWorkingTeachersByFirstThreeLetters")]
        [ResponseType(typeof(Teacher))]
        public HttpResponseMessage GetWorkingTeachersByFirstThreeLetters([FromUri]string startsWith)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Teacher ByFirstThreeLetters: " + startsWith);

            if (startsWith.Length != 3)
            {
                logger.Error("You entered " + startsWith.Length + " letters. Please put exactly three letters for this search.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, "You entered " + startsWith.Length + " letters. " +
                    "Please put exactly three letters for this search.");
            }
            try
            {
                IEnumerable<TeacherDTOItem> teachers = teachersService.GetWorkingTeachersByFirstThreeLetters(startsWith);
                logger.Info("Success!");
                return Request.CreateResponse(HttpStatusCode.OK, teachers);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }


    }
}


