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
    [RoutePrefix("project/forms")]
    public class FormsController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IFormsService formsService;
        private FormToFormDTO toDTO;

        public FormsController(IFormsService formsService, FormToFormDTO toDTO)
        {
            this.formsService = formsService;
            this.toDTO = toDTO;
        }

        /*REST endpoint koji vraća sva odeljenja (listu)
 o putanja /project/forms
 *metoda je otvorena za sve, (pristup informacijama i kolicina je srazmerna roli korisnika)
* Napomena: 
* administratori i nastavnici mogu videti sva odeljenja  
* ukoliko je korisnik student, moze da vidi SVOJE odeljenja (ali ne i sve info o attendingTeacher-u)
* ukoliko je korisnik parent, moze da vidi ODELJENJA SVOJE DECE (ali ne i sve info) */

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("", Name = "GetAllForms")]
        public IEnumerable<FormDTOForStudentAndParents> GetForms()
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Form Collection");

            if (userRole == "admin")
            {
                IEnumerable<FormDTOForAdmin> forms = formsService.GetAllForAdmin();
                logger.Info("Success!");
                return forms;
            }

            else if (userRole == "teacher")
            {
                IEnumerable<FormDTOForTeacher> forms = formsService.GetAllForTeacher();
                logger.Info("Success!");
                return forms;
            }
            //moze da vidi svoje odeljenje
            else if (userRole == "student")
            {
                IEnumerable<FormDTOForStudentAndParents> forms = formsService.GetAllForStudentFromStudentForm(userId);
                logger.Info("Success!");
                return forms;
            }
            //moze da vidi sva odeljenja u koja idu njegova deca
            else //if (userRole == "parent")
            {
                IEnumerable<FormDTOForStudentAndParents> forms = formsService.GetAllForParentFromStudentsForms(userId);
                logger.Info("Success!");
                return forms;
            }
        }

        /*REST endpoint koji vraća odeljenje po vrednosti prosleđenog ID-a (FormId je informacija iz ocene)
o putanja /project/forms/{id:int}
 Napomena: 
* ukoliko je korisnik student, moze da vidi svoje odeljenja (ali ne i sve info o attendingTeacher-u)
* ukoliko je korisnik parent, moze da vidi odeljenja u koja idu njegova deca (ali ne i sve info) 
o u slučaju da ne postoji odeljenje sa prosleđenim id-em vratiti grešku*/

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("{id:int}", Name = "GetFormByID")]
        [ResponseType(typeof(Form))]
        public HttpResponseMessage GetForm(int id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Form by id: " + id);

            try
            {
                Form form = formsService.GetByID(id);

                if (form == null)
                {
                    logger.Info("The form with id: " + id + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The form with id: " + id + " was not found.");
                }
                if (userRole == "admin")
                {
                    logger.Info("Requesting found form convert for " + userRole + "role.");
                    FormDTOForAdmin dto = toDTO.ConvertToFormDTOForAdmin(form);
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
                    logger.Info("Requesting found form convert for " + userRole + "role.");
                    FormDTOForTeacher dto = toDTO.ConvertToFormDTOForTeacher(form);
                    if (dto == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dto);
                }
                else if (form.Students.Any(x => x.Id == userId) == true
                    || form.Students.Any(x => x.Parent.Id == userId) == true)
                {
                    logger.Info("Requesting found form convert for " + userRole + " role.");
                    FormDTOForStudentAndParents dto = toDTO.ConvertToFormDTOForStudentAndParent(form);
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

        //razredni ne sme da ima odeljenje kome je vec razredni, i mora da je trenutno zaposlen
        /*REST endpoint za izmenu postojećeg odeljenja 
•	putanja project/forms/{id:int} 
•	metoda je rezervisana za administratora
•	ukoliko je prosleđen ID koji ne pripada nijednom odeljenju metoda treba da vrati grešku, 
a u suprotnom vraća podatke odeljenja sa izmenjenim vrednostima
metoda dozvoljava promenu razrednog staresine - AttendingTeacher
*/

        [Authorize(Roles = "admin")]
        [Route("{id:int}", Name = "UpdateForm")]
        [ResponseType(typeof(Form))]
        public HttpResponseMessage PutForm(int id, [FromBody]PutFormDTO updated)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Update for Form Id: " + id);

            if (updated.Id != id)
            {
                logger.Error("Updated Form id " + updated.Id + " doesn't match the id " + id + " from the request (route).");
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Updated " +
                    "Form id " + updated.Id + " doesn't match the id " + id + " from the request (route).");
            }

            try
            {
                FormDTOForAdmin saved = formsService.Update(id, updated);

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

        //razredni ne sme da ima odeljenje kome je vec razredni, i mora da je trenutno zaposlen
        /*REST endpoint koji omogućava dodavanje novog odeljenja
       o putanja /project/forms
       o metoda je rezervisana samo za administratora
       o metoda treba da vrati novo odeljenje */

        [Authorize(Roles = "admin")]
        [Route("", Name = "CreateForm")]
        [ResponseType(typeof(Form))]
        public HttpResponseMessage PostForm([FromBody]PostFormDTO newForm)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Form Insert");

            try
            {
                FormDTOForAdmin saved = formsService.Create(newForm);

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

        //razredni ne sme da ima odeljenje kome je vec razredni, i mora da je trenutno zaposlen
        /*REST endpoint koji omogućava izmenu razrednog staresine odeljenja
        o putanja /project/forms/{id:int}/attending-teacher/{teacherId}
        o metoda je rezervisana samo za administratora
        o metoda treba da vrati dodatu promenjeno odeljenje
        Prilikom unosa nove kombinacije postavlja se vrednost polja DateTime Started i StoppedTeaching na utcNow i null */

        [Authorize(Roles = "admin")]
        [Route("{id:int}/attending-teacher/{teacherId}", Name = "UpdateAttentingTeacher")]
        [ResponseType(typeof(Form))]
        public HttpResponseMessage PutChangeAttendingTeacher(int id, string teacherId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Form Update For Form Id: " + id + ", Change Attending Teacher Id to: " + teacherId);

            try
            {
                FormDTOForAdmin saved = formsService.ChangeAttendingTeacher(id, teacherId);

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

        /*REST endpoint koji omogućava dodavanje ucenika u odeljenje (promena ucenikovog odeljenja)
    o putanja /project/forms/{id:int}/students/{studentId}
    o metoda je rezervisana samo za administratora 
    aktivan ucenik ne moze da se prebaci u odeljenje koje nije kreirano za ovu skolsku godinu */

        [Authorize(Roles = "admin")]
        [Route("{id:int}/students/{studentId}", Name = "UpdateFormChangeStudentsForm")]
        [ResponseType(typeof(Form))]
        public HttpResponseMessage PutFormAddStudent(int id, string studentId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Form Update For Form Id: " + id + ", add Student Id: " + studentId);

            try
            {
                Form saved = formsService.AddStudent(id, studentId);

                if (saved == null)
                {
                    logger.Info("Failed!");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Failed!");
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

        //odeljenje se ne moze obrisati ako sadrzi ucenike - moraju se prebaciti u druga odeljenja
        /*REST endpoint za brisanje odeljenja 
o putanja project/forms/{id:int}
o metoda je rezervisana za administratora
o ukoliko je prosleđen ID koji ne pripada nijednom odeljenju metoda treba da vrati grešku */

        [Authorize(Roles = "admin")]
        [Route("{id:int}", Name = "DeleteForm")]
        [ResponseType(typeof(Form))]
        public HttpResponseMessage DeleteForm(int id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Form Remove for Subject Id: " + id);

            try
            {
                Form form = formsService.Delete(id);

                if (form == null)
                {
                    logger.Info("The form with id: " + id + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The form with id: " + id + " was not found.");
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

        //SAMO SPISAK IMENA UCENIKA ODELJENJA - FormIdStudentsDTO
        /*REST endpoint koji vraća SORTIRANI spisak IMENA studenata odredjenog odeljenja
o putanja /project/forms/{id:int}/students
* administratori i nastavnici mogu videti sva odeljenja (UCENICKI DOSIJEI) 
* ukoliko je korisnik student, moze da vidi SVOJE odeljenja (ali ne i sve info o attendingTeacher-u)
* ukoliko je korisnik parent, moze da vidi ODELJENJA SVOJE DECE (ali ne i sve info) */

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("{id:int}/students", Name = "GetSortedStudentsNamesByFormId")]
        public HttpResponseMessage GetSortedStudentsNamesByFormId(int id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Sorted Students Names Collection For Form Id: " + id);

            try
            {
                if (userRole == "admin" || userRole == "teacher")
                {
                    FormIdStudentsDTO form = formsService.GetSortedStudentsNamesByFormId(id);
                    if (form == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Failed!");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, form);
                }
                if (userRole == "student")
                {
                    FormIdStudentsDTO form = formsService.GetSortedStudentsNamesByFormId(id);
                    if (form == null || form.Students.Any(x => x.Id == userId) == false)
                    {
                        logger.Info("Authorisation failure. User " + userId + " is not authorised for this request.");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Access Denied. " +
                            "We’re sorry, but you are not authorized to perform the requested operation.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, form);
                }
                else
                {
                    //parent ulazi, ali bacamo exception ako nije roditelj nekog deteta odeljenja
                    FormIdStudentsDTO form = formsService.GetSortedStudentsNamesByFormIdForParent(id, userId);
                    if (form == null)
                    {
                        logger.Info("Failed.");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Failed.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, form);
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }


        /*REST endpoint koji vraća SORTIRANI spisak odeljenja odredjenog razreda odredjene skolske godine
o putanja /project/forms/by-grade/{grade:int}/by-year-started/{year-int}
*metoda je otvorena za administraotre */

        [Authorize(Roles = "admin")]
        [Route("by-grade/{grade:int}/by-year-started/{year:int}", Name = "GetSortedFormsByGradeByYear")]
        public HttpResponseMessage GetSortedFormsByGradeByYear(int grade, int year)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Sorted Form List For Grade: " + grade + " For Year: " + year);

            try
            {
                IEnumerable<FormDTOForAdmin> forms = formsService.GetSortedFormsByGradeByYear(grade, year);
                logger.Info("Success!");
                return Request.CreateResponse(HttpStatusCode.OK, forms);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        /*REST endpoint koji vraća odeljenje kome je razredni staresina zadati nastavnikov username
o putanja /project/forms/by-attending-teacher/{teacherUserName}
metoda je otvorena za administratore */

        [Authorize(Roles = "admin")]
        [Route("by-attending-teacher/{teacherUserName}", Name = "GetFormByAttendingTeacherUserName")]
        [ResponseType(typeof(Form))]
        public HttpResponseMessage GetFormByAttendingTeacherUserName([FromUri] string teacherUserName)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Form by attending teacher's user name: " + teacherUserName);

            try
            {
                FormDTOForAdmin form = formsService.GetFormByAttendingTeacherLastName(teacherUserName);

                if (form == null)
                {
                    logger.Info("The Form by attending teacher's last name: " + teacherUserName + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The Form by attending teacher's last name: " + teacherUserName + " was not found.");
                }

                logger.Info("Success! Form by id: " + form.Id);
                return Request.CreateResponse(HttpStatusCode.OK, form);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

    }
}