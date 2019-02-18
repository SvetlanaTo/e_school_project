using eSchool.Models;
using eSchool.Models.DTOs;
using eSchool.Services;
using eSchool.Support;
using Microsoft.AspNet.Identity.EntityFramework;
using NLog;
using System;
using System.Collections;
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
    [RoutePrefix("project/students")]
    public class StudentsController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IStudentsService studentsService;
        private StudentToStudentDTO toDTO;

        public StudentsController(IStudentsService studentsService, StudentToStudentDTO toDTO)
        {
            this.studentsService = studentsService;
            this.toDTO = toDTO;
        }


        /*REST endpoint koji vraća sve studente (listu korisnika)
o putanja /project/students
*metoda je otvorena za sve, ali je kolicina informacija koja je korisniku na raspolaganju srazmerna njegovoj roli
* Napomena: 
* ukoliko je korisnik student, moze da vidi sve ucenike svog odeljenja (ali ne i sve info)
* ukoliko je korisnik parent, moze da vidi sve ucenike iz svih odeljenja u koja idu njegova deca (ali ne i sve info)*/

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("", Name = "GetAllStudents")]
        public IEnumerable<StudentDTOForStudentAndParent> GetAllStudents()
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Student Collection");

            if (userRole == "admin")
            {
                IEnumerable<StudentDTOForAdmin> users = studentsService.GetAllForAdmin();
                logger.Info("Success!");
                return users;
            }
            else if (userRole == "teacher")
            {
                IEnumerable<StudentDTOForTeacher> users = studentsService.GetAllForTeacher();
                logger.Info("Success!");
                return users;
            }
            //moze da vidi sve studente iz svog razreda
            else if (userRole == "student")
            {
                IEnumerable<StudentDTOForStudentAndParent> users = studentsService.GetAllForStudentFromStudentForm(userId);
                logger.Info("Success!");
                return users;
            }
            //moze da vidi sve studente (ogranicene informacije) onih razreda u koja idu njegova deca
            else //if (userRole == "parent")
            {
                IEnumerable<StudentDTOForStudentAndParent> users = studentsService.GetAllForParentFromStudentsForms(userId);
                logger.Info("Success!");
                return users;
            }
        }


        /*REST endpoint koji vraća studenta po vrednosti prosleđenog ID-a
o putanja /project/students/{id}
*metoda je namenjena administratorima i nastavnicima, koji mogu da vide osnovne podatke o uceniku (ucenicki dosijei) 
* ali i studentu ili roditelju
- ukoliko pretrazuje ucenik ili roditelj za sebe, tj. svoje dete
- ili ukoliko pretrazuje za ucenika iz svog odeljenja ili odeljenja svog deteta
o u slučaju da ne postoji korisnik sa prosleđenim id-em vratiti grešku*/

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("{id}", Name = "GetStudentById")]
        [ResponseType(typeof(Student))]
        public HttpResponseMessage GetStudentById(string id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Student by id: " + id);

            try
            {
                Student student = studentsService.GetByID(id);

                if (student == null)
                {
                    logger.Info("The student with id: " + id + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The student with id: " + id + " was not found.");
                }
                if (userRole == "admin")
                {
                    logger.Info("Requesting found student convert for " + userRole + "role.");
                    StudentDTOForAdmin dto = toDTO.ConvertToStudentDTOForAdmin(student, (List<IdentityUserRole>)student.Roles);
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
                    logger.Info("Requesting found student convert for " + userRole + "role.");
                    StudentDTOForTeacher dto = toDTO.ConvertToStudentDTOForTeacher(student);
                    if (dto == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dto);
                }
                //ukoliko pretrazuje ucenik ili roditelj za sebe, tj. svoje dete
                //ili ukoliko pretrazuje za ucenika iz svog odeljenja ili odeljenja svog deteta
                else if (userId == student.Id || userId == student.Parent.Id || student.Form.Students.Any(x => x.Id == userId) == true
                    || student.Form.Students.Any(x => x.Parent.Id == userId) == true)
                {
                    logger.Info("Requesting found student convert for " + userRole + "role.");
                    StudentDTOForStudentAndParent dto = toDTO.ConvertToStudentDTOForStudentAndParent(student);
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

        /*REST endpoint koji vraća studenta po vrednosti prosleđenog (jedinstvenog) username-a
o putanja /project/students/by-username/{username}
*metoda je namenjena administratorima i nastavnicima, (ucenicki dosijei u skoli) 
* ali i studentu ili roditelju
- ukoliko pretrazuje ucenik ili roditelj za sebe, tj. svoje dete
- ili ukoliko pretrazuje za ucenika iz svog odeljenja ili odeljenja svog deteta
o u slučaju da ne postoji korisnik sa prosleđenim username-om vratiti grešku */

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("by-username/{username}", Name = "GetStudentsByUserName")]
        [ResponseType(typeof(Student))]
        public HttpResponseMessage GetStudentByUserName(string username)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Student by username: " + username);

            try
            {
                Student student = studentsService.GetByUserName(username);

                if (student == null)
                {
                    logger.Info("The student with username: " + username + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The student with username: " + username + " was not found.");
                }
                if (userRole == "admin")
                {
                    logger.Info("Requesting found student convert for " + userRole + "role.");
                    StudentDTOForAdmin dto = toDTO.ConvertToStudentDTOForAdmin(student, (List<IdentityUserRole>)student.Roles);
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
                    logger.Info("Requesting found student convert for " + userRole + "role.");
                    StudentDTOForTeacher dto = toDTO.ConvertToStudentDTOForTeacher(student);
                    if (dto == null)
                    {
                        logger.Info("Failed!");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Something went wrong.");
                    }
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dto);
                }
                //ukoliko pretrazuje ucenik ili roditelj za sebe, tj. svoje dete
                //ili ukoliko pretrazuje za ucenika iz svog odeljenja ili odeljenja svog deteta
                else if (userId == student.Id || userId == student.Parent.Id || student.Form.Students.Any(x => x.Id == userId) == true
                    || student.Form.Students.Any(x => x.Parent.Id == userId) == true)
                {
                    logger.Info("Requesting found student convert for " + userRole + "role.");
                    StudentDTOForStudentAndParent dto = toDTO.ConvertToStudentDTOForStudentAndParent(student);
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


        /*REST endpoint za izmenu postojećeg studenta 
•	putanja project/students/{id} 
•	metoda je rezervisana za administratora
•	ukoliko je prosleđen ID koji ne pripada nijednom student metoda treba da vrati grešku, 
a u suprotnom vraća podatke studenta sa izmenjenim vrednostima
NAPOMENA: metoda ne dozvoljava da se izmeni student, ako se pri izmeni unosi JMBG ili USERNAME koji vec postoji u sistemu
u ovo metodi se ne dozvoljava izmena sifre, role, imagePath, niti roditelja :)
DOZVOLJENA JE IZMENA ODELJENJA - MOZE SE PREBACITI SAMO UKOLIKO JE ODELJENJE KREIRANO OVE SKOLSKE GODINE
•   Kada se podaci uspesno izmene, AUTOMATSKI SE SALJE MAIL studentu i roditelju, CIME SE ISTI INFORMISE DA JE IZVRSENA IZMENA
*/
        [Authorize(Roles = "admin")]
        [Route("{id}", Name = "UpdateStudent")]
        [ResponseType(typeof(Student))]
        public async Task<HttpResponseMessage> PutStudent(string id, [FromBody]PutStudentDTO updated)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Update for Student Id: " + id);

            if (updated.Id != id)
            {
                logger.Error("Updated student id " + updated.Id + " doesn't match the id " + id + " from the request (route).");
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Updated " +
                    "student id " + updated.Id + " doesn't match the id " + id + " from the request (route).");
            }

            try
            {
                StudentDTOForAdmin saved = await studentsService.Update(id, updated);

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

        //JEDINO SE UCENIK MOZE OBRISATI MIMO SVOJIH OCENA !!! OSTALE ENTITETE NE BRISEMO AKO JE ZA NJIH VEZANA OCENA
        /*REST endpoint za brisanje ucenika 
o putanja project/students/{id}
o metoda je rezervisana za administratora
o ukoliko je prosleđen ID koji ne pripada nijednom korisniku metoda treba da vrati grešku
NAPOMENA: ukoliko je ucenik za brisanje jedini ucenik roditelja, onda se automatski brise i roditelj
*/

        [Authorize(Roles = "admin")]
        [Route("{id}", Name = "DeleteStudent")]
        [ResponseType(typeof(Student))]
        public HttpResponseMessage DeleteStudent(string id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Student Remove for Student Id: " + id);

            try
            {
                Student user = studentsService.Delete(id);

                if (user == null)
                {
                    logger.Info("The student with id: " + id + " was not found.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The student with id: " + id + " was not found.");
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

        /*REST endpoint koji omogućava dodavanje slike ucenika
o putanja /project/students/upload-image/{id}
o ukoliko je prosleđen id koji ne pripada nijednom uceniku metoda treba da vrati
grešku, a u suprotnom vraća podatke ucenika sa izmenjenim vrednostima
o metoda je namenjena administratorima */

        //upload i load u db
        [Authorize(Roles = "admin")]
        [Route("upload-image/{id}", Name = "UploadStudentImage")]
        [ResponseType(typeof(Student))]
        public async Task<HttpResponseMessage> PostImageFormData(string id)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Student Image Upload for Student Id: " + id);

            if (!Request.Content.IsMimeMultipartContent())
            {
                logger.Info("Failed! Unsupported Media Type.");
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {

                    logger.Info("Server file path: " + file.LocalFileName);
                    StudentDTOForAdmin student = studentsService.UpdateStudentWithImage(id, file.LocalFileName);

                    if (student == null)
                    {
                        logger.Info("The student with id: " + id + " was not found.");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "The student with id: " + id + " was not found.");
                    }

                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, student);
                }

                logger.Info("Failed.");
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        //dodeljujemo studenta novom roditelju-stratelju
        /*REST endpoint za izmenu roditelja-staratelja studenta
•	putanja project/students/{id}/assign-to-parent/{parentId}
•	metoda je rezervisana za administratora
•	ukoliko je prosleđen ID koji ne pripada nijednom student, niti roditelju, metoda treba da vrati grešku, 
a u suprotnom vraća podatke studenta sa izmenjenim vrednostima
•   Kada se podaci uspesno izmene, AUTOMATSKI SE SALJE MAIL roditelju, cime se informise da mu je dodeljen ucenikov nalog 
*/
        [Authorize(Roles = "admin")]
        [Route("{id}/assign-to-parent/{parentId}", Name = "AssignStudentToNewParent")]
        [ResponseType(typeof(Student))]
        public HttpResponseMessage PutStudentToNewParent(string id, string parentId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Update for Student Id: " + id + ". Assigning new parent id: " + parentId);

            try
            {
                StudentDTOForAdmin saved = studentsService.UpdateStudentParent(id, parentId);

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

        /*REST endpoint koji vraća ucenike dodeljene roditelju po vrednosti prosleđenog ID-a roditelja
o putanja /project/students/assigned-to-parent/{parentId}
*metoda je namenjena administratorima i nastavnicima, koji mogu da vide osnovne podatke o ucenicima (ucenicki dosijei) 
o u slučaju da ne postoji korisnik sa prosleđenim id-em vratiti grešku*/

        [Authorize(Roles = "admin, teacher")]
        [Route("assigned-to-parent/{parentId}", Name = "GetStudentsAssignedToParentId")]
        [ResponseType(typeof(Student))]
        public HttpResponseMessage GetStudentsAssignedToParentId(string parentId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Students Collection Assigned to Parnet by id: " + parentId);

            try
            {
                IEnumerable<Student> parentStudents = studentsService.GetStudentsAssignedToParentId(parentId);

                if (userRole == "admin")
                {
                    logger.Info("Requesting found parent Students collection convert for " + userRole + "role.");
                    IEnumerable<StudentDTOForAdmin> dtos = toDTO.ConvertToStudentDTOListForAdmin((List<Student>)parentStudents);

                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dtos);
                }
                else //if (userRole == "teacher")
                {
                    logger.Info("Requesting found parent Students collection convert for " + userRole + "role.");
                    IEnumerable<StudentDTOForTeacher> dtos = toDTO.ConvertToStudentDTOListForTeacher((List<Student>)parentStudents);

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
        /*REST endpoint za izmenu odeljenja u koje ide student - dodavanje ucenika u odeljenje
•	putanja project/students/{id}/add-to-form/{formId:int}
•	metoda je rezervisana za administratora
•	ukoliko je prosleđen ID koji ne pripada nijednom student, niti odeljenju, metoda treba da vrati grešku, 
a u suprotnom vraća podatke studenta sa izmenjenim vrednostima
•   Kada se podaci uspesno izmene, AUTOMATSKI SE SALJE MAIL roditelju i uceniku, cime se informisu da je nalog izmenjen
*/
        [Authorize(Roles = "admin")]
        [Route("{id}/add-to-form/{formId:int}", Name = "AddStudentToDifferentForm")]
        [ResponseType(typeof(Student))]
        public HttpResponseMessage PutStudentForm(string id, int formId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            logger.Info("UserId: " + userId + ": Requesting Student Update For Student Id: " + id + ", add Student to Form Id: " + formId);

            try
            {
                StudentDTOForAdmin saved = studentsService.UpdateStudentForm(id, formId);

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

        //GET lista ucenika odeljenja (* SORTIRANA DTO LISTA UCENIKA * )
        /*REST endpoint koji vraća ucenike jednog odeljenja po vrednosti prosleđenog ID-a odeljenja
o putanja /project/students/assigned-to-form/{formId}
*metoda je namenjena administratorima, svim nastavnicima (ucenicki dosijei),
* studentima, ukoliko pretrazuje za svoje odeljenja
* roditeljima, ukoliko pretrazuje neko od odeljenja u koja idu njegova deca

*/

        [Authorize(Roles = "admin, teacher, student, parent")]
        [Route("assigned-to-form/{formId:int}", Name = "GetStudentsByFormId")]
        [ResponseType(typeof(Student))]
        public HttpResponseMessage GetStudentsByFormId(int formId)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Student Collection For Form Id: " + formId);

            try
            {
                IEnumerable<Student> students = studentsService.GetStudentsByFormId(formId);

                if (userRole == "admin" || userRole == "teacher" || students.Any(x => x.Id == userId) == true
                        || students.Any(x => x.Parent.Id == userId) == true)
                {
                    IEnumerable<StudentDTOForList> dtos = toDTO.ConvertToStudentDTOListForList((List<Student>)students);
                    logger.Info("Success!");
                    return Request.CreateResponse(HttpStatusCode.OK, dtos);
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

        //LISTA id+ime i prezime
        /*REST endpoint koji vraća ucenike prema zadatom prezimenu, sortirane u rastucem redosledu imena,
o putanja /project/students/by-last-name
* metoda je otvorena za sve korisnike */

        [Authorize(Roles = "admin, teacher, parent, student")]
        [Route("by-last-name", Name = "GetStudentsByLastNameSorted")]
        public HttpResponseMessage GetStudentsByLastNameSorted([FromUri]string lastName)
        {
            string userId = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == "UserId").Value;
            string userRole = ((ClaimsPrincipal)RequestContext.Principal).FindFirst(x => x.Type == ClaimTypes.Role).Value;
            logger.Info("UserRole: " + userRole + ", UserId: " + userId + ": Requesting Student List - " +
                "By Last Name: " + lastName + " - Sorted Asc By Name");

            try
            {
                IEnumerable<FormStudentDTO> students = studentsService.GetStudentsByLastNameSorted(lastName);

                logger.Info("Success!"); 
                return Request.CreateResponse(HttpStatusCode.OK, students);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }



    }
}

