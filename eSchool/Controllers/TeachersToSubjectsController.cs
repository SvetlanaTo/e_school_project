using eSchool.Models;
using eSchool.Models.DTOs;
using eSchool.Services;
using eSchool.Support;
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
    [RoutePrefix("project/teachers-to-subjects")]
    public class TeachersToSubjectsController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private ITeachersToSubjectsService teachersToSubjectsService;
        private ITeachersService teachersService;
        private TeacherSubjectToDTO toDTO;

        public TeachersToSubjectsController(ITeachersToSubjectsService teachersToSubjectsService, ITeachersService teachersService, TeacherSubjectToDTO toDTO)
        {
            this.teachersToSubjectsService = teachersToSubjectsService;
            this.teachersService = teachersService;
            this.toDTO = toDTO;
        }

        
        /*REST endpoint koji vraća sve nastavnike i njihove predmete (listu)
 o putanja /project/teachers-to-subjects
 metoda je otvorena za sve (korisnici mogu da vide ceo kolektiv)
* ali je kolicina informacija koja im je na raspolaganju srazmerna njihovoj roli
             */
        [Authorize(Roles = "admin, teacher, parent, student")]
        [Route("", Name = "GetAllTeachersToSubjects")]
        public IEnumerable<TeacherToSubjectDTOForStudentAndParent> GetTeachersToSubjects()
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Teacher To Subject Collection");


            IEnumerable<TeacherToSubject> teachersToSubjects = teachersToSubjectsService.GetAll();

            if (userRole == "admin")
            {
                IList<TeacherToSubjectDTOForAdmin> dtos = toDTO.ConvertToTSDTOListForAdmin((List<TeacherToSubject>)teachersToSubjects);
                logger.Info("Success!");
                return dtos;
            }

            if (userRole == "teacher")
            {
                IList<TeacherToSubjectDTOForTeacher> dtos = toDTO.ConvertToTSDTOListForTeacher((List<TeacherToSubject>)teachersToSubjects);
                logger.Info("Success!");
                return dtos;
            }
            else //(isStudent == true || isParent == true)
            {
                IList<TeacherToSubjectDTOForStudentAndParent> dtos = toDTO.ConvertToTSDTOListForStudentAndParent((List<TeacherToSubject>)teachersToSubjects);
                logger.Info("Success!");
                return dtos;
            }

        }

        /*REST endpoint koji vraća odrenjenu kombinaciju nastavnik predmet po vrednosti prosleđenog ID-a
o putanja /project/teachers-to-subjects/{id}
 metoda je otvorena za sve (korisnici mogu da vide ceo kolektiv)
* ali je kolicina informacija koja im je na raspolaganju srazmerna njihovoj roli
o u slučaju da ne postoji da odredjeni nastavnik predaje odredjeni predmet pod prosleđenim id-em vratiti grešku*/

        [Authorize(Roles = "admin, teacher, parent, student")]
        [Route("{id:int}", Name = "GetTeacherToSubject")]
        [ResponseType(typeof(TeacherToSubject))]
        public HttpResponseMessage GetTeacherToSubject(int id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Teacher To Subject by id: " + id);

            try
            {
                TeacherToSubject ts = teachersToSubjectsService.GetByID(id);
                if (ts == null)
                {
                    logger.Info("The teacher with id: " + id + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The TeacherToSubject with id: " + id + " was not found.");
                }
                if (userRole == "admin")
                {
                    logger.Info("Requesting found TeacherToSubject convert for " + userRole + "role.");
                    TeacherToSubjectDTOForAdmin dto = toDTO.ConvertToTeacherToSubjectDTOForAdmin(ts);
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
                    logger.Info("Requesting found TeacherToSubject convert for " + userRole + "role.");
                    TeacherToSubjectDTOForTeacher dto = toDTO.ConvertToTeacherToSubjectDTOForTeacher(ts);
                    if (dto == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dto);
                }
                else //if (userRole == "student" || userRole == "parent")
                {
                    logger.Info("Requesting found TeacherToSubject convert for " + userRole + "role.");
                    TeacherToSubjectDTOForStudentAndParent dto = toDTO.ConvertToTeacherToSubjectDTOForStudentAndParent(ts);
                    if (dto == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dto);
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*REST endpoint koji omogućava dodavanje nove veze izmenju nastavnika i predmeta
         * (angazovanje nastavnika na predmetu)
        o putanja /project/teachers-to-subjects/teacher/{teacherId}/subject/{subjectId:int} 
        o metoda je rezervisana samo za administratora
        o metoda treba da vrati dodatu novu kombinaciju
        Prilikom unosa nove kombinacije postavlja se vrednost polja DateTime Started na utcNow i StoppedTeaching  i null */

        [Authorize(Roles = "admin")]
        [Route("teachers/{teacherId}/subjects/{subjectId:int}", Name = "AssignTeacherToSubject")]
        [ResponseType(typeof(TeacherToSubject))]
        public HttpResponseMessage PostTeacherToSubject(string teacherId, int subjectId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Teacher To Subject Insert - Teacher Id: " + teacherId + ", " +
                "Subject Id:" + subjectId);

            try
            {
                TeacherToSubjectDTOForAdmin saved = teachersToSubjectsService.Create(teacherId, subjectId);

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
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }


        /*REST endpoint koji omogućava menjanje 
         * datuma kada je nastavnik angazovan na predmetu (DateTime StartedTeaching)
         * ili datuma kada je prestao da predaje taj predmet (DateTime? StoppedTeaching)
         *u zavisnosti od vrednosti u ruti ("started" or "stopped")
        o putanja /project/teachers-to-subjects/{id:int}/change-date/{date}
        o metoda je rezervisana samo za administratora
        o metoda treba da vrati promenjenu kombinaciju ts */

        [Authorize(Roles = "admin")]
        [Route("{id:int}/change-date/{date}", Name = "PutStartedOrStoppedTeaching")]
        [ResponseType(typeof(TeacherToSubject))]
        public HttpResponseMessage PutStartedOrStoppedTeaching(int id, string date, [FromUri]DateTime updated)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Teacher To Subject Update");

            try
            {
                TeacherToSubjectDTOForAdmin saved = new TeacherToSubjectDTOForAdmin();

                if (date.ToLower().Equals("started"))
                {
                    logger.Info("Requesting StartedTeaching Property Update");
                    saved = teachersToSubjectsService.UpdateStartedTeaching(id, updated);
                }
                else if (date.ToLower().Equals("stopped"))
                {
                    logger.Info("Requesting StoppedTeaching Property Update");
                    saved = teachersToSubjectsService.UpdateStoppedTeaching(id, updated);
                }
                else
                {
                    logger.Info("The route value: " + date + " isn't valid. The correct options are: started or stopped.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The route value: " + date + " isn't valid. " +
                        "The correct options are: started or stopped.");
                }

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
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*REST endpoint koji omogućava postavljenje propertija StoppedTeaching na trnutni datum za datu teacher-subject kombinaciju
       o putanja /project/teachers-to-subjects/{id:int}/stopped-teaching-now
       o metoda je rezervisana samo za administratora
       o metoda treba da vrati promenjenu kombinaciju ts
       
            - PUTANJA IZ DELETE TS */

        [Authorize(Roles = "admin")]
        [Route("{id:int}/stopped-teaching-now", Name = "PutStoppedTeachingNowByTSId")]
        [ResponseType(typeof(TeacherToSubject))]
        public HttpResponseMessage PutStoppedTeachingNowByTSId(int id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Teacher To Subject Update - Stopped Teaching for TeacherToSubject Id: " + id);

            try
            {
                TeacherToSubjectDTOForAdmin saved = teachersToSubjectsService.PutStoppedTeachingNowByTSId(id);

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
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }


        /*REST endpoint koji vraća sve predmete koje predaje jedan nastavnik (prema teacher Id)
o putanja /project/teachers-to-subjects/teachers/{teacherId}
metoda je otvorena za sve korisnike, samo je kolicina informacija srazmerna tipu korisnika */

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("teacher-id/{teacherId}/subject-list", Name = "GetSubjectDTOListByTeacherId")]
        public HttpResponseMessage GetSubjectDTOListByTeacherId(string teacherId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Subject Collection By Teacher Id" + teacherId);

            try
            {
                TeacherTeacherSubjectDTOItems subjects = teachersToSubjectsService.GetSubjectDTOListByTeacherId(teacherId);

                if (subjects == null)
                {
                    logger.Info("Failed!");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Failed!");
                }

                logger.Info("Success!");
                return Request.CreateResponse(HttpStatusCode.OK, subjects); 
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        //drugi nacin
        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("teachers/{teacherId}/subjects", Name = "GetSubjectsByTeacherId")]
        public HttpResponseMessage GetSubjectsByTeacherId(string teacherId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Subject Collection By Teacher Id" + teacherId);

            try
            {
                IEnumerable<TeacherToSubject> tsByTeacherId = teachersToSubjectsService.GetSubjectsByTeacherId(teacherId);

                if (userRole == "admin")
                {
                    IList<TeacherToSubjectDTOForAdmin> dtos = toDTO.ConvertToTSDTOListForAdmin((List<TeacherToSubject>)tsByTeacherId);
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dtos);
                }
                else if (userRole == "teacher")
                {
                    IList<TeacherToSubjectDTOForTeacher> dtos = toDTO.ConvertToTSDTOListForTeacher((List<TeacherToSubject>)tsByTeacherId);
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dtos);
                }
                else
                {
                    IList<TeacherToSubjectDTOForStudentAndParent> dtos = toDTO.ConvertToTSDTOListForStudentAndParent((List<TeacherToSubject>)tsByTeacherId);
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

        //lista nastavnika koji predaju odredjeni predmet SubjectTeacherSubjectDTOItems i lista TeacherSubjectDTOItemForSubject

        /*REST endpoint koji vraća sve nastavnike koji predaju jedan predmet, prema subject Id . Sortirani prema prezimenu i imenu
 o putanja /project/teachers-to-subjects/subjects/{subjectId:int}/teachers
 metoda je otvorena za sve korisnike, samo je kolicina informacija srazmerna tipu korisnika */

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("subject-id/{subjectId:int}/teacher-list", Name = "GetTeacherDTOListBySubjectId")]
        public HttpResponseMessage GetTeacherDTOListBySubjectId(int subjectId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Subject Collection By Subject Id" + subjectId);

            try
            {
                SubjectTeacherSubjectDTOItems teachers = teachersToSubjectsService.GetTeacherDTOListBySubjectId(subjectId);  

                if (teachers == null)
                {
                    logger.Info("Failed!");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Failed!");
                }

                logger.Info("Success!");
                return Request.CreateResponse(HttpStatusCode.OK, teachers);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        //drugi nacin
        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("subjects/{subjectId:int}/teachers", Name = "GetTeachersBySubjectId")]
        public HttpResponseMessage GetTeachersBySubjectId(int subjectId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Subject Collection By Subject Id" + subjectId);

            try
            {
                IEnumerable<TeacherToSubject> tsBySubjectId = teachersToSubjectsService.GetTeachersBySubjectId(subjectId);

                if (userRole == "admin")
                {
                    IList<TeacherToSubjectDTOForAdmin> dtos = toDTO.ConvertToTSDTOListForAdmin((List<TeacherToSubject>)tsBySubjectId);
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dtos);
                }
                else if (userRole == "teacher")
                {
                    IList<TeacherToSubjectDTOForTeacher> dtos = toDTO.ConvertToTSDTOListForTeacher((List<TeacherToSubject>)tsBySubjectId);
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dtos);
                }
                else
                {
                    IList<TeacherToSubjectDTOForStudentAndParent> dtos = toDTO.ConvertToTSDTOListForStudentAndParent((List<TeacherToSubject>)tsBySubjectId);
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


        /*REST endpoint koji omogućava prekidanje angazovanja jednog nastavnika na SVIM predmetima
         * za nastavnika (by id) za sve predmete se postavlja trenutni datum za StoppedTeaching properti 
         *(datuma kada je prestao da predaje taj predmet (DateTime? StoppedTeaching = UtsNow))

        o putanja /project/teachers-to-subjects/by-teacher/{teacherId}/stopped-teaching-to-now
        o metoda je rezervisana samo za administratora
        o metoda treba da vrati listu promenjenih kombinacija ts 
        
NAPOMENA: AUTOMATSKI SE POSTAVLJA TRENUTNI DATUM ZA PROPERTY STOPPED NA SVE TORKE U TABELI FTS GDE NASTAVNIK PREDAJE TAJ PREMET,
tj. ako nastavnik prestaje da predaje taj predmet, prestaje da ga predaje automatski svakom odeljenju */

        [Authorize(Roles = "admin")]
        [Route("by-teacher/{teacherId}/stopped-teaching-to-now", Name = "PutStoppedTeachingNowForAllSubjectsByTeacherId")]
        [ResponseType(typeof(TeacherToSubject))]
        public HttpResponseMessage PutStoppedTeachingNowByTeacherId(string teacherId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting StoppedTeaching Update to DateTime.UtcNow For Teacher with Id: " + teacherId);

            try
            {
                IEnumerable<TeacherToSubject> saved = teachersToSubjectsService.UpdateStoppedTeachingToNowForAllSubjectsByTeacherId(teacherId);
                IEnumerable<TeacherToSubjectDTOForAdmin> dtos = toDTO.ConvertToTSDTOListForAdmin((List<TeacherToSubject>)saved);

                logger.Info("Success! Updated StoppedTeaching property to DateTime.UtcNow.");
                return Request.CreateResponse(HttpStatusCode.OK, dtos);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*REST endpoint koji omogućava prekidanje angazovanja svih nastavnika na jednom predmetu
        * za predmet (by id) za sve nastavnike se postavlja trenutni datum za StoppedTeaching properti 
        *(datuma kada je prestao da predaje taj predmet (DateTime? StoppedTeaching = UtcNow))

       o putanja /project/teachers-to-subjects/by-subject/{subjectId:int}/stopped-teaching-to-now
       o metoda je rezervisana samo za administratora
       o metoda treba da vrati listu promenjenih kombinacija ts */

        [Authorize(Roles = "admin")]
        [Route("by-subject/{subjectId:int}/stopped-teaching-to-now", Name = "PutStoppedTeachingNowForAllTeachersBySubjectId")]
        [ResponseType(typeof(TeacherToSubject))]
        public HttpResponseMessage PutStoppedTeachingNowForAllTeachersBySubjectId(int subjectId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting StoppedTeaching Update to DateTime.UtcNow For Subject with Id: " + subjectId);

            try
            {
                IEnumerable<TeacherToSubject> saved = teachersToSubjectsService.UpdateStoppedTeachingToNowForAllTeachersBySubjectId(subjectId);
                IEnumerable<TeacherToSubjectDTOForAdmin> dtos = toDTO.ConvertToTSDTOListForAdmin((List<TeacherToSubject>)saved);

                logger.Info("Success! Updated StoppedTeaching property to DateTime.UtcNow.");
                return Request.CreateResponse(HttpStatusCode.OK, dtos);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*REST endpoint za brisanje kombinacije nastavnik-predmet 
o putanja /project/teachers-to-subjects/{id:int}
o metoda je rezervisana za administratora
o ukoliko je prosleđen ID koji ne pripada nijednoj kombinaciji metoda treba da vrati grešku */

        [Authorize(Roles = "admin")]
        [Route("{id:int}", Name = "DeleteTeacherToSubject")]
        [ResponseType(typeof(TeacherToSubject))]
        public HttpResponseMessage DeleteTeacherToSubject(int id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Remove For Teacher-Subject with Id: " + id);

            try
            {
                TeacherToSubject teacherToSubject = teachersToSubjectsService.Delete(id);

                if (teacherToSubject == null)
                {
                    logger.Info("TeacherToSubjectject with id " + id + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "TeacherToSubjectject with id " + id + " was not found.");
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

        /*REST endpoint koji vraća ZA subject Id, u zavisnosti od vrednosti u ruti ("active" ili "inactive")
         * sve nastavnike koji su i dalje angazovani na predmetu
         * ILI sve nastavnike koji su prestali da predaju taj predmet       
o putanja /project/teachers-to-subjects/subjects/{subjectId:int}/{active}
*/

        [Authorize(Roles = "admin")]
        [Route("subjects/{subjectId:int}/teachers/{active}", Name = "GetActiveOrInactiveTeachersBySubjectId")]
        public HttpResponseMessage GetActiveOrInactiveTeachersBySubjectId(int subjectId, string active)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting " + active + " Teacher Collection By Subject Id: " + subjectId);

            if (!active.ToLower().Equals("active") && !active.ToLower().Equals("inactive"))
            {
                logger.Error("The route value: " + active + " isn't valid. The options are: active or inactive");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The route value: " + active + " isn't valid. " +
                    "The options are: active or inactive");
            }

            try
            {
                IEnumerable<TeacherToSubject> teachersBySubjectId = teachersToSubjectsService.GetActiveOrInactiveTeachersBySubjectId(subjectId, active);
                IEnumerable<TeacherToSubjectDTOForAdmin> dtos = toDTO.ConvertToTSDTOListForAdmin((List<TeacherToSubject>)teachersBySubjectId);
                logger.Info("Success!");
                return Request.CreateResponse(HttpStatusCode.OK, dtos);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*REST endpoint koji vraća ZA teacher Id, u zavisnosti od vrednosti u ruti ("active" ili "inactive")
        * sve nastavnike koji su i dalje angazovani na predmetu
        * ILI sve nastavnike koji su prestali da predaju taj predmet       
o putanja /project/teachers-to-subjects/subjects/{subjectId:int}/{active}
*/

        [Authorize(Roles = "admin")]
        [Route("{active}/teachers/{teacherId}/subjects", Name = "GetSubjectsByActiveOrInactiveTeacherId")]
        public HttpResponseMessage GetSubjectsByActiveOrInactiveTeacherId(string teacherId, string active)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting " + active + " Subject Collection By Teacher Id: " + teacherId);

            if (!active.ToLower().Equals("active") && !active.ToLower().Equals("inactive"))
            {
                logger.Error("The route value: " + active + " isn't valid. The options are: active or inactive");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The route value: " + active + " isn't valid. " +
                    "The options are: active or inactive");
            }

            try
            {
                IEnumerable<TeacherToSubject> subjectsByTeacherId = teachersToSubjectsService.GetSubjectsByActiveOrInactiveTeacherId(teacherId, active);
                IEnumerable<TeacherToSubjectDTOForAdmin> dtos = toDTO.ConvertToTSDTOListForAdmin((List<TeacherToSubject>)subjectsByTeacherId);
                logger.Info("Success!");
                return Request.CreateResponse(HttpStatusCode.OK, dtos);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*REST endpoint koji vraća nastavnike koji aktivno predaju predmete ZA ODREDJENI RAZRED (GODINU)
       * sve nastavnike koji su i angazovani na predmetima za odredjeni razred  (ne odeljenje, vec godinu)   
o putanja /project/teachers-to-subjects/by-grade/teachers/for-grade/{grade:int}
metoda je otvorena za sve, samo je kolicina informacija o nastavnicima srazmerna tipu ulogovanog korisnika
*/

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("teachers/for-grade/{grade:int}", Name = "GetActiveTeachersForGradeNumber")]
        public HttpResponseMessage GetActiveTeachersForGrade(int grade)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Teacher To Subject Collection For" +
                " Grade: " + grade);

            if (grade < 1 || grade > 8)
            {
                logger.Error("The route value: " + grade + " isn't valid. The grade must be between 1 and 8.");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The route value: " + grade + " isn't valid. " +
                   "The grade must be between 1 and 8.");
            }

            try
            {
                IEnumerable<TeacherToSubject> teachersToSubjects = teachersToSubjectsService.GetActiveTeachersForGrade(grade);

                if (userRole == "admin")
                {
                    IList<TeacherToSubjectDTOForAdmin> dtos = toDTO.ConvertToTSDTOListForAdmin((List<TeacherToSubject>)teachersToSubjects);
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dtos);
                }

                if (userRole == "teacher")
                {
                    IList<TeacherToSubjectDTOForTeacher> dtos = toDTO.ConvertToTSDTOListForTeacher((List<TeacherToSubject>)teachersToSubjects);
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dtos);
                }
                else //(isStudent == true || isParent == true)
                {
                    IList<TeacherToSubjectDTOForStudentAndParent> dtos = toDTO.ConvertToTSDTOListForStudentAndParent((List<TeacherToSubject>)teachersToSubjects);
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

        /*REST endpoint koji vraća nastavnike koji su predavali odredjeni predmet u zadatom periodu
       * sve nastavnike koji su i angazovani na trazenom predmetu za odredjeni period     
o putanja /project/teachers-to-subjects/teachers/for-subject/{subjectId:int}/findByDate/{startDate}/and/{endDate}
metoda je otvorena za sve, samo je kolicina informacija o nastavnicima srazmerna tipu ulogovanog korisnika
*/
        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("teachers/for-subject/{subjectId:int}/findByDate/{startDate}/and/{endDate}", Name = "GetTeachersForSubjectByDate")]
        public HttpResponseMessage GetTeachersForSubjectByDate(int subjectId, DateTime startDate, DateTime endDate)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Teacher To Subject Collection For" +
                    " Subject: " + subjectId + " By DateTime: " + startDate + " - " + endDate);

            if (startDate > endDate)
            {
                logger.Error("The route value isn't valid. The startDate must be before endDate.");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The route value isn't valid. " +
                   "The startDate must be before endDate.");
            }

            try
            {
                IEnumerable<TeacherToSubject> teachersToSubjects = teachersToSubjectsService.GetTeachersForSubjectByDate(subjectId, startDate, endDate);

                if (userRole == "admin")
                {
                    IList<TeacherToSubjectDTOForAdmin> dtos = toDTO.ConvertToTSDTOListForAdmin((List<TeacherToSubject>)teachersToSubjects);
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dtos);
                }

                if (userRole == "teacher")
                {
                    IList<TeacherToSubjectDTOForTeacher> dtos = toDTO.ConvertToTSDTOListForTeacher((List<TeacherToSubject>)teachersToSubjects);
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dtos);
                }
                else //(isStudent == true || isParent == true)
                {
                    IList<TeacherToSubjectDTOForStudentAndParent> dtos = toDTO.ConvertToTSDTOListForStudentAndParent((List<TeacherToSubject>)teachersToSubjects);
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
    }
}
