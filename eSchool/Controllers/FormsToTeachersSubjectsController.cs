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
    [RoutePrefix("project/forms-to-teachers-subjects")]
    public class FormsToTeachersSubjectsController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IFormsToTeacherSubjectsService formsToTeacherSubjectsService;
        private FTSToDTO toDTO;

        public FormsToTeachersSubjectsController(IFormsToTeacherSubjectsService formsToTeacherSubjectsService, FTSToDTO toDTO)
        {
            this.formsToTeacherSubjectsService = formsToTeacherSubjectsService;
            this.toDTO = toDTO;
        }

        /*REST endpoint koji vraća sva odeljenja i njihove predmete koje pohadjaju kod nastavnika (listu)
 o putanja /project/forms-to-teachers-subjects
 metoda je rezervisana za administratora */

        [Authorize(Roles = "admin")]
        [Route("", Name = "GetAllFormsToTeachersSubjects")]
        public IEnumerable<FTSDTOForAdmin> GetAllFormsToTeachersSubjects()
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Forms To Teacher-Subject Collection");

            IEnumerable<FTSDTOForAdmin> formsToTeachersSubjects = formsToTeacherSubjectsService.GetAllForAdmin();
            logger.Info("Success!");
            return formsToTeachersSubjects;
        }


        /*REST endpoint koji vraća odredjeno odeljenje koje slusa odredjeni predmet kod odredjenog nastavnik 
         * po vrednosti prosleđenog ID-a
        o putanja /project/forms-to-teachers-subjects/{id}
        o u slučaju da ne postoji vratiti grešku 
        -metoda je otvorena za admina i nastavnika - razlicita dostupnost info*/

        [Authorize(Roles = "admin, teacher")]
        [Route("{id:int}", Name = "GetFormsToTeachersSubjectsById")]
        [ResponseType(typeof(FormToTeacherSubject))]
        public HttpResponseMessage GetFormsToTeachersSubjectsById(int id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting FormToTeacherSubject by id: " + id);

            try
            {
                FormToTeacherSubject formToTeacherSubject = formsToTeacherSubjectsService.GetByID(id);

                if (formToTeacherSubject == null)
                {
                    logger.Info("The formToTeacherSubject with id: " + id + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The formToTeacherSubject with id: " + id + " was not found.");
                }
                if (userRole == "admin")
                {
                    logger.Info("Requesting found FormToTeacherSubject convert for " + userRole + "role.");
                    FTSDTOForAdmin dto = toDTO.ConvertToFTSDTOForAdmin(formToTeacherSubject);
                    if (dto == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dto);
                }
                else //if (userRole == "teacher")
                {
                    logger.Info("Requesting found FormToTeacherSubject convert for " + userRole + "role.");
                    FTSDTOForTeacher dto = toDTO.ConvertToFTSDTOForTeacher(formToTeacherSubject);
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

        /*REST endpoint koji omogućava dodavanje veze izmenju odeljenja i nastavnik-predmet
        * (angazovanog nastavnika na predmetu)
       o putanja /project/forms-to-teachers-subjects/form/{formId:int}/teacher/{teacherId}/subject/{subjectId}
       o metoda je rezervisana samo za administratora
       o metoda treba da vrati dodatu novu kombinaciju */
        [Authorize(Roles = "admin")]
        [Route("form/{formId:int}/teacher/{teacherId}/subject/{subjectId:int}", Name = "AssignTeacherSubjectToForm")]
        [ResponseType(typeof(FormToTeacherSubject))]
        public HttpResponseMessage PostFormToTeacherSubject(int formId, string teacherId, int subjectId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Form TO TeacherSubject Insert for " +
                "Form Id: " + formId + ", Teacher id: " + teacherId + ", Subject Id: " + subjectId);

            try
            {
                FTSDTOForAdmin saved = formsToTeacherSubjectsService.Create(formId, teacherId, subjectId);

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

        /*REST endpoint koji omogućava izmenu veze izmenju odeljenja i nastavnik-predmet
        * (angazovanog nastavnika na predmetu)
       o putanja /project/forms-to-teachers-subjects/{id:int}
       o metoda je rezervisana samo za administratora
       o metoda treba da vrati izmenjenu kombinaciju*/

        [Authorize(Roles = "admin")]
        [Route("{id:int}", Name = "UpdateFormToTEacherSubject")]
        [ResponseType(typeof(FormToTeacherSubject))]
        public HttpResponseMessage PutFormToTeacherSubject(int id, [FromBody]PutFormToTeacherSubjectDTO updated)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Form TO TeacherSubject Update for FTS Id: " + id);

            if (updated.Id != id)
            {
                logger.Error("Updated FormToTeacherSubject id: " + updated.Id + " doesn't match the id: " + id + " from the request (Route).");
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Updated FormToTeacherSubject " +
                    "id: " + updated.Id + " doesn't match id: " + id + " from the request.");
            }

            try
            {
                FTSDTOForAdmin saved = formsToTeacherSubjectsService.Update(id, updated);

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


        /*REST endpoint za brisanje kombinacije odeljenje-nastavnik-predmet 
o putanja /project/forms-to-teachers-subjects/{id:int}
o metoda je rezervisana za administratora
o ukoliko je prosleđen ID koji ne pripada nijednoj kombinaciji metoda treba da vrati grešku

 NAPOMENA: brisanje je moguce samo ako je lista ocena prazna...
 u suprotnom, korisnik se upucuje da arhivira ovu vezu nastavnik-predmet-odeljenje postavljanjem propertija Stopped na trenutni datum
*/

        [Authorize(Roles = "admin")]
        [Route("{id:int}", Name = "DeleteFormToTeacherSubject")]
        [ResponseType(typeof(FormToTeacherSubject))]
        public HttpResponseMessage DeleteFormToTeacherSubject(int id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting FormToTeacherSubject Remove for FTS Id: " + id);

            try
            {
                FormToTeacherSubject deleted = formsToTeacherSubjectsService.Delete(id);

                logger.Info("Success!");
                return Request.CreateResponse(HttpStatusCode.OK, "Success!");
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }

        }

        /*REST endpoint koji omogućava menjanje 
        * datuma kada je nastavnik angazovan da predaje predmet odeljenju (DateTime Started)
        * ili datuma kada je prestao da predaje predmet odeljenju (DateTime? Stopped)
        *u zavisnosti od vrednosti u ruti ("started" or "stopped")
       o putanja /project/forms-to-teachers-subjects/{id:int}/change-date/{date}
       o metoda je rezervisana samo za administratora
       o metoda treba da vrati promenjenu kombinaciju fts */

        [Authorize(Roles = "admin")]
        [Route("{id:int}/change-date/{date}", Name = "PutStartedOrStoppedFTS")]
        [ResponseType(typeof(FormToTeacherSubject))]
        public HttpResponseMessage PutStartedOrStoppedFTS(int id, string date, [FromUri]DateTime updated)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Form TO TeacherSubject Update for FTS Id: " + id);

            try
            {
                FTSDTOForAdmin saved = new FTSDTOForAdmin();

                if (date.ToLower().Equals("started"))
                {
                    logger.Info("Requesting Started Property Update");
                    saved = formsToTeacherSubjectsService.UpdateStarted(id, updated);
                }
                else if (date.ToLower().Equals("stopped"))
                {
                    logger.Info("Requesting StoppedTeaching Property Update");
                    saved = formsToTeacherSubjectsService.UpdateStopped(id, updated);
                }
                else
                {
                    logger.Info("The route value isn't valid.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The route value isn't valid. The correct " +
                        "options are: started or stopped.");
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

        /*REST endpoint koji omogućava prekidanje angazovanja nastavnik-predmet u odeljenju
      * postavlja se trenutni datum za datum kada je nastavnik prestao da predaje taj predmet tom odeljenju
      * (DateTime? Stopped = DateTime.UtcNow))

     o putanja /project/forms-to-teachers-subjects/{id:int}/stopped-to-now
     o metoda je rezervisana samo za administratora
     o metoda treba da vrati listu promenjenih kombinacija ts */

        [Authorize(Roles = "admin")]
        [Route("{id:int}/stopped-to-now", Name = "PutStoppedNowByFTSId")]
        [ResponseType(typeof(FormToTeacherSubject))]
        public HttpResponseMessage PutStoppedNowByFTSId(int id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Stopped Update to DateTime.UtcNow for FTS Id: " + id);

            try
            {
                FTSDTOForAdmin saved = formsToTeacherSubjectsService.UpdateStoppedNowByFTSId(id);

                if (saved == null)
                {
                    logger.Info("Failed!");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Failed!");
                }

                logger.Info("Success! Updated Stopped property to DateTime.UtcNow.");
                return Request.CreateResponse(HttpStatusCode.OK, saved);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }


        /*REST endpoint koji vraća listu odeljenja i predmeta koje predaje ODREDJENI NASTAVNIK
o putanja /project/forms-to-teachers-subjects/by-teacher/{teacherId} 
metoda je rezervisana za administratora */

        [Authorize(Roles = "admin")]
        [Route("by-teacher/{teacherId}", Name = "GetAllFormsToTeachersSubjectsByTeacherId")]
        public IEnumerable<FTSDTOForAdmin> GetAllFormsToTeachersSubjectsByTeacherId(string teacherId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Forms To Teacher-Subject Collection By teacher id: " + teacherId);

            IEnumerable<FTSDTOForAdmin> ftsByTeacherId = formsToTeacherSubjectsService.GetAllByTeacherId(teacherId);
            logger.Info("Success!");
            return ftsByTeacherId;
        }

        /*REST endpoint koji vraća listu kombinacija odeljenje-nastavnik-predmet by teacher-subject id
o putanja /project/forms-to-teachers-subjects/by-teacher-to-subject/{tsId:int} 
metoda je rezervisana za administratora
potrebna za proveru nakon postavljanja StoppedTeaching propertija na DateTime.UtcNow*/

        [Authorize(Roles = "admin")]
        [Route("by-teacher-to-subject/{tsId:int}", Name = "GetAllFormsToTeachersSubjectsByTeacherSubjectId")]
        public IEnumerable<FTSDTOForAdmin> GetAllFormsToTeachersSubjectsByTeacherSubjectId(int tsId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Forms To Teacher-Subject Collection By teacher-subject id: " + tsId);

            IEnumerable<FTSDTOForAdmin> ftsByTSId = formsToTeacherSubjectsService.GetAllByTeacherSubjectId(tsId);
            logger.Info("Success!");
            return ftsByTSId;
        }

        /*REST endpoint koji vraća listu nastavnikovih nedeljnih zaduzenja, IZVESTAJ - nedeljeni fond casova (norma) nastavnika 
         * trenutno zaposlenih nastavnika, pa cak i ako trenutno ne predaju nista
o putanja /project/forms-to-teachers-subjects/teachers-weekly-engagements
metoda je rezervisana za administratora */

        [Authorize(Roles = "admin")]
        [Route("teachers-weekly-engagements", Name = "GetWorkingTeachersWeeklyEngagements")]
        public HttpResponseMessage GetWorkingTeachersWeeklyEngagements()
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Working Teachers Weekly Engagements Report.");

            try
            {
                TeachersWeeklyEngagementsDTO engagementsReport = formsToTeacherSubjectsService.GetWorkingTeachersWeeklyEngagements(userId);
                if (engagementsReport == null)
                {
                    logger.Info("Failed!");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Failed!");
                }
                logger.Info("Success!");
                return Request.CreateResponse(HttpStatusCode.OK, engagementsReport); 
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

    }
}
