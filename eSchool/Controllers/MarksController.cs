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
using System.Web.Http;
using System.Web.Http.Description;

namespace eSchool.Controllers
{
    [RoutePrefix("project/marks")]
    public class MarksController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IMarksService marksService;
        private MarkToMarkDTO toDTO;

        public MarksController(IMarksService marksService, MarkToMarkDTO toDTO)
        {
            this.marksService = marksService;
            this.toDTO = toDTO;
        }

        /*REST endpoint koji vraća sve ocene (listu)
 o putanja /project/marks
 metoda je namenjena administratorima  */

        [Authorize(Roles = "admin")]
        [Route("")]
        public IEnumerable<MarkDTO> GetMarks()
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Mark Collection.");

            //IEnumerable<MarkDTO> marks = marksService.GetAllDTOs();
            IEnumerable<MarkDTO> marks = marksService.GetAllDTOsFromService();
            logger.Info("Success!");
            return marks;
        }

        /*REST endpoint koji vraća ocenu po vrednosti prosleđenog ID-a
o putanja /project/marks/{id}
o u slučaju da ne postoji ocena sa prosleđenim id-em vratiti grešku
metoda je namenjena svima, s tim da
*administator moze da vidi sve ocene by id
* nastavnik moze da vidi markById ako ju je on dao
* roditelj moze da vidi markById ako je to ocena njegovog deteta
* ucenik moze da vidi markById ako je to njegova ocena
 */

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("{id:int}")]
        [ResponseType(typeof(Mark))]
        public HttpResponseMessage GetMark(int id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Mark by id: " + id);

            try
            {
                //MarkDTO mark = marksService.GetDTOByID(id);
                MarkDTO mark = marksService.GetByIDDTOFromService(id);

                if (mark == null)
                {
                    logger.Info("The mark with id: " + id + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The mark with id: " + id + " was not found.");
                }

                if (userRole == "admin" || userId == mark.TeacherID || userId == mark.StudentID || userId == mark.ParentID)
                {
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, mark);
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



        /*REST endpoint koji omogućava upisivanje ocene 

      o putanja /project/marks/teacher/{teacherId}/subject/{subjectId:int}/form/{formId:int}/student/{studentId}
      o metoda je rezervisana za administratora i 
      nastavnika ukoliko upisuje ocenu iz svog predmeta studentu kojem taj predmet predaje
      o metoda treba da vrati KREIRANU OCENU i AUTOMATSKI SE SALJE MAIL RODITELJU I UCENIKU */

        [Authorize(Roles = "admin, teacher")]
        [Route("teacher/{teacherId}/subject/{subjectId:int}/form/{formId:int}/student/{studentId}", Name = "AddMark")]
        [ResponseType(typeof(Mark))]
        public HttpResponseMessage PostMark([FromBody]PostMarkDTO postDTO, string teacherId, int subjectId, int formId, string studentId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Mark Insert - " +
                "TeacherId: " + teacherId + ", SubjectId: " + subjectId + ", FormId: " + formId + ", StudentId: " + studentId);

            if (userRole == "admin" || userId == teacherId)
            {
                try
                {
                    MarkDTO saved = marksService.Create(postDTO, teacherId, subjectId, formId, studentId);

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
            logger.Info("Authorisation failure. User is not authorised for this request.");
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access Denied. " +
                "We’re sorry, but you are not authorized to perform the requested operation.");
        }

        /*REST endpoint koji omogućava IZMENU ocene 

    o putanja /project/marks/{id:int}/by-teacher/{teacherId}
    o metoda je rezervisana samo za administratora i nastavnika koji je ocenu kreirao
    *DOZVOLJENA JE IZMENA SAMO VREDNOSTI OCENE. IZMENA OSTALIH OBELEZJA, PODRAZUMEVA KREIRANJE NOVE OCENE.
    o metoda treba da vrati IZMENJENU OCENU, 
    automatski se salje mail roditelju i studentu, koji se obavestavaju o napravljenoj izmeni */

        [Authorize(Roles = "admin, teacher")]
        [Route("{id:int}/by-teacher/{teacherId}", Name = "UpdateMark")]
        [ResponseType(typeof(Mark))]
        public HttpResponseMessage PutMark(int id, string teacherId, [FromUri]int value)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Mark Update - mark id" +
                ": " + id + " to mark value: " + value);

            if (value < 1 || value > 5)
            {
                logger.Info("The value you entered (" + value + ") is not valid. Mark value must be between 1 and 5.");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The value you " +
                    "entered (" + value + ") is not valid. Mark value must be between 1 and 5.");
            }

            if (userRole == "admin" || userId == teacherId)
            {
                try
                {
                    MarkDTO saved = marksService.Update(id, teacherId, value);

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
            logger.Info("Authorisation failure. User is not authorised for this request.");
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access Denied. " +
                "We’re sorry, but you are not authorized to perform the requested operation.");
        }


        /*REST endpoint koji omogućava BRISANJE ocene  

    o putanja /project/marks/{id:int}/by-teacher/{teacherId}
    o metoda je rezervisana samo za administratora i nastavnika koji je ocenu kreirao */

        [Authorize(Roles = "admin, teacher")]
        [Route("{id:int}/by-teacher/{teacherId}", Name = "DeleteMark")]
        [ResponseType(typeof(Mark))]
        public HttpResponseMessage DeleteMark(int id, string teacherId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Mark Remove - mark id: " + id);

            if (userRole == "admin" || userId == teacherId)
            {
                try
                {
                    Mark deleted = marksService.Delete(id, teacherId);

                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, "Success!");
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
                }
            }
            logger.Info("Authorisation failure. User is not authorised for this request.");
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access Denied. " +
                "We’re sorry, but you are not authorized to perform the requested operation.");
        }


        /*REST endpoint koji vraća sve ocene jednog studenta, prema studentId
o putanja /project/marks/by-student/{studentId} 
 metoda je namenjena:
*administatoru - moze da vidi sve ocene za svakog studenta
* roditelj moze da vidi sve ocene ako trazi za svoje dete (by studentId)
* ucenik moze da vidi sve ocene, ako trazi za sebe (by studentId)
o metoda treba da vrati listu svih studentovih ocena */

        [Authorize(Roles = "admin, student, parent, teacher")]
        [Route("by-student/{studentId}", Name = "GetMarksByStudentId")]
        public HttpResponseMessage GetMarksByStudentId(string studentId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Marks By Student Id: " + studentId);

            try
            {
                if (userRole == "admin" || userRole == "student" || userRole == "parent")
                {
                    IEnumerable<MarkDTO> marks = marksService.GetMarksDTOByStudentId(studentId); 
                    //admin, student za sebe, roditelj za svoje dete
                    if (userRole == "admin" || userId == studentId || marks.Any(x => x.ParentID == userId) == true)
                    {
                        logger.Info("Success!");
                        return Request.CreateResponse(HttpStatusCode.OK, marks);
                    }
                }
                //nastavnik moze da vidi samo one ocene koje je on dao za trazenog studenta
                if (userRole == "teacher")
                {
                    IEnumerable<MarkDTO> marks = marksService.GetMarksDTOByStudentIdForTeacher(studentId, userId); 

                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, marks);
                }

                logger.Info("Authorisation failure. User is not authorised for this request.");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access Denied. We’re sorry, " +
                    "but you are not authorized to perform the requested operation.");
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }

        }


        /*REST endpoint koji vraća sve ocene jednog studenta iz odredjenog predmeta, (by studentId, subject Id)
o putanja /project/marks/by-student/{studentId}/for-subject/{subjectId:int} 
 metoda je namenjena svima, s tim da
*administator moze da vidi sve ocene 
* nastavnik moze da vidi ocene zadatog ucenika iz zadatog predmeta ako je on dao te ocene iz tog predmeta tom studentu
* roditelj - ako pretrazuje za svoje dete
* ucenik - ako pretrazuje za sebe

o metoda treba da vrati listu svih studentovih ocena iz jednog predmeta */

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("by-student/{studentId}/for-subject/{subjectId:int}", Name = "GetMarksByStudentIdForSubjectId")]
        public HttpResponseMessage GetMarksByStudentIdForSubjectId(string studentId, int subjectId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Marks " +
                "By Student Id: " + studentId + " For Subject Id: " + subjectId);

            try
            {
                IEnumerable<MarkDTO> marks = marksService.GetMarksByStudentIdForSubjectId(studentId, subjectId);

                if (userRole == "admin" || userId == studentId || marks.Any(x => x.ParentID == userId) == true
                    || marks.Any(x => x.TeacherID == userId) == true)
                {
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, marks);
                }

                logger.Info("Authorisation failure. User is not authorised for this request.");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access Denied. We’re sorry, " +
                    "but you are not authorized to perform the requested operation.");
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }

        }


        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("by-student/{studentId}/for-subject/{subjectId:int}/values-list", Name = "GetMarkValuesListByStudentIdForSubjectId")]
        public HttpResponseMessage GetMarkValuesListByStudentIdForSubjectId(string studentId, int subjectId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Marks " +
                "By Student Id: " + studentId + " For Subject Id: " + subjectId);

            try
            {             
               MarkValuesListDTO marks = marksService.GetMarkValuesListByStudentIdForSubjectId(studentId, subjectId);

                if (userRole == "admin" || userId == studentId || marks.ParentID == userId || marks.TeacherID == userId)
                {
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, marks);
                }

                logger.Info("Authorisation failure. User is not authorised for this request.");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access Denied. We’re sorry, " +
                    "but you are not authorized to perform the requested operation.");
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }

        }


        /*REST endpoint koji vraća sve ocene koje je dao jedan nastavnik, prema teacherId
o putanja /project/marks/by-teacher/{teacherId} 
metoda je namenjena:
*administatoru - moze da vidi sve ocene za svakog studenta
*nastavnik - moze da vidi sve ocene koje je on dao
o metoda treba da vrati listu svih studentovih ocena
*/

        [Authorize(Roles = "admin, teacher")]
        [Route("by-teacher/{teacherId}", Name = "GetMarksByTeacherId")]
        public HttpResponseMessage GetMarksByTeacherId(string teacherId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Marks By Teacher Id: " + teacherId);

            try
            {
                IEnumerable<MarkDTO> marks = marksService.GetMarksDTOByTeacherId(teacherId);
               
                if (userRole == "admin" || marks.Any(x => x.TeacherID == userId) == true)
                {
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, marks);
                }

                logger.Info("Authorisation failure. User is not authorised for this request.");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access Denied. We’re sorry, " +
                    "but you are not authorized to perform the requested operation.");
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }

        }

        /*REST endpoint koji vraća sve ocene jednog studenta od odredjenog nastavnika, (by studentId, teacher Id)
o putanja /project/marks/by-student/{studentId}/from-teacher/{teacherId} 
metoda je namenjena svima, s tim da
*administator moze da vidi sve ocene 
* nastavnik moze da vidi ocene zadatog ucenika ako pretrazuje za svoj predmet
* roditelj - ako pretrazuje za svoje dete
* ucenik - ako pretrazuje za sebe
*/

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("by-student/{studentId}/from-teacher/{teacherId}", Name = "GetMarksByStudentIdFromTeacherId")]
        public HttpResponseMessage GetMarksByStudentIdFromTeacherId(string studentId, string teacherId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Marks " +
                "By Student Id: " + studentId + " From Teacher Id: " + teacherId);

            try
            {             
                MarkValuesListDTO marks = marksService.GetMarkValuesListByStudentIdFromTeacherId(studentId, teacherId);

                if (userRole == "admin" || userId == studentId || marks.ParentID == userId || marks.TeacherID == userId)
                {
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, marks);
                }

                logger.Info("Authorisation failure. User is not authorised for this request.");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access Denied. We’re sorry, " +
                    "but you are not authorized to perform the requested operation.");
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }

        }


        /*REST endpoint koji vraća sve ocene jednog odeljenja od odredjenog nastavnika i odredjenog predmeta - SORTIRANI DNEVNIK
         * (by formId, from teacher Id, for subjectId)
o putanja /project/marks/by-form/{formId:int}/from-teacher/{teacherId}/for-subject/{teacherId} 
metoda je namenjena administratoru i nastavnicima
*administator moze da vidi sve ocene 
* nastavnik moze da vidi ocene zadatog odeljenja ako pretrazuje za svoj predmet*/

        [Authorize(Roles = "admin, teacher")]
        [Route("by-form/{formId:int}/from-teacher/{teacherId}/for-subject/{subjectId}", Name = "GetMarksByFormIdFromTeacherIdForSubjectId")]
        public HttpResponseMessage GetMarksByFormIdFromTeacherIdForSubjectId(int formId, string teacherId, int subjectId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Marks " +
                "By Form Id: " + formId + " From Teacher Id: " + teacherId + " For Subject Id: " + subjectId + "" +
                "Sorted by Last, than First name.");

            try
            {            
                IEnumerable<MarkValuesListDTO> marks = marksService.GetMarksByFormIdFromTeacherIdForSubjectId(formId, teacherId, subjectId);

                if (userRole == "admin" || userId == teacherId)
                {
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, marks);
                }

                logger.Info("Authorisation failure. User is not authorised for this request.");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access Denied. We’re sorry, " +
                    "but you are not authorized to perform the requested operation.");
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }

        }

        //CEO DNEVNIK ZA ADMINA I RAZREDNOG  
        /*REST endpoint koji vraća sve ocene jednog odeljenja - SORTIRANI DNEVNIK - ZA RAZREDNOG STARESINU 
         * (by formId, from teacher Id, for subjectId)
o putanja /project/marks/by-form/{formId:int}/from-teacher/{teacherId}/for-subject/{teacherId} 
metoda je namenjena administratoru i razrednom staresini */

        [Authorize(Roles = "admin, teacher")]
        [Route("by-form/{formId:int}", Name = "GetMarksByFormIdForAttendingTeacher")]
        public HttpResponseMessage GetMarksByFormIdForAttendingTeacher(int formId)
        { 
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Marks By Form Id: " + formId + " For Attending Teacher");

            try
            {
                Mark mark = marksService.GetFirstMarkByFormIdForAttendingTeacherValidation(formId);
                if (mark == null)
                {
                    logger.Info("Failed!");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Failed!");
                } 
                if (userRole == "admin" || mark.Student.Form.AttendingTeacher.Id == userId)
                {
                    IEnumerable<MarkValuesListDTO> dtos = marksService.ConvertToMarkValuesListDTOList(formId);
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dtos);
                }

                logger.Info("Authorisation failure. User is not authorised for this request.");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access Denied. We’re sorry, " +
                    "but you are not authorized to perform the requested operation.");
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }

        }

        /*REST endpoint koji vraća sve ocene jednog studenta - REPORT CARD 
         * 
o putanja /project/marks/by-student/{studentId}/report-card
metoda je namenjena administratorima
studentu - ukoliko pretrazuje za sebe
roditelju - ukoliko pretrazuje za svoju decu */

        [Authorize(Roles = "admin, student, parent")]
        [Route("by-student/{studentId}/report-card", Name = "GetReportCardForStudentId")]
        public HttpResponseMessage GetReportCardForStudentId(string studentId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Report Card For Student Id: " + studentId);

            try
            {
                ReportCardDTO reportCard = marksService.GetReportCardForStudentId(studentId);
                if (reportCard == null)
                {
                    logger.Info("Failed!");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Failed!");
                }
                if (userRole == "admin" || reportCard.StudentId == userId
                    || reportCard.Parent.Substring(0, 3).Equals(userId) )
                {
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, reportCard); 
                }

                logger.Info("Authorisation failure. User is not authorised for this request.");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access Denied. We’re sorry, " +
                    "but you are not authorized to perform the requested operation.");
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }

        }

    }
}
