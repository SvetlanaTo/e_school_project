using eSchool.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSchool.Models
{
    public class Init : DropCreateDatabaseAlways<AuthContext>
    // DropCreateDatabaseIfModelChanges<AuthContext>
    {
        protected override void Seed(AuthContext context)
        {
            IList<Form> forms = new List<Form>();
            IList<Subject> subjects = new List<Subject>();        
            IList<TeacherToSubject> teachersToSubjects = new List<TeacherToSubject>();
            IList<FormToTeacherSubject> formsToTeacherSubjects = new List<FormToTeacherSubject>();
            IList<Mark> marks = new List<Mark>();

            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            roleManager.Create(new IdentityRole() { Id = "4", Name = "admin" });
            roleManager.Create(new IdentityRole() { Id = "1", Name = "student" });
            roleManager.Create(new IdentityRole() { Id = "2", Name = "parent" });
            roleManager.Create(new IdentityRole() { Id = "3", Name = "teacher" });


            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            Admin admin1 = new Admin
            {
                Id = "401",
                FirstName = "Admin",
                LastName = "Admin",
                Email = "svetlana.topalov@gmail.com",
                EmailConfirmed = true,
                UserName = "Owner",
                PhoneNumber = "+111111111111",
                PhoneNumberConfirmed = true,
                PasswordHash = userManager.PasswordHasher.HashPassword("Secret1"), 
                //LockoutEnabled = true,
                //SecurityStamp = Guid.NewGuid().ToString("D"),
                Jmbg = "1111111111111",
                ShortName = "owner"
            };

            userManager.Create(admin1, "Secret1"); 
            userManager.AddToRole(admin1.Id, "admin");

            //kreiranje nastavnika
            Teacher CreateTeacher(string fn, string ln, string email, string un, string pn, string jmbg, Genders g, bool working)
            {
                Teacher t = new Teacher
                {
                    FirstName = fn,
                    LastName = ln,
                    Email = email,
                    EmailConfirmed = true,
                    UserName = un,
                    PhoneNumber = pn,
                    PhoneNumberConfirmed = true,
                    Jmbg = jmbg,
                    Gender = g,
                    IsStillWorking = working 
                };
                return t;
            }

            Teacher t1 = CreateTeacher("Milica", "Andric", "svetlana.topalov@gmail.com", "teacher1", "+11111111111", "1111111111112", Genders.FEMALE, false);
            t1.Id = "301";
            userManager.Create(t1, "Secret1");
            userManager.AddToRole(t1.Id, "teacher");


            Teacher t2 = CreateTeacher("Jagoda", "Topalov", "svetlana.topalov@gmail.com", "teacher2", "+11111111111", "1111111111113", Genders.FEMALE, true); 
            t2.Id = "302";
            userManager.Create(t2, "Secret1");
            userManager.AddToRole(t2.Id, "teacher");

            Teacher t3 = CreateTeacher("Marija", "Dora", "svetlana.topalov@gmail.com", "teacher3", "+11111111111", "1111111111114", Genders.FEMALE, true);
            t3.Id = "303";
            userManager.Create(t3, "Secret1");
            userManager.AddToRole(t3.Id, "teacher");


            Teacher t4 = CreateTeacher("Manda", "Topalov", "svetlana.topalov@gmail.com", "teacher4", "+11111111111", "1111111111115", Genders.FEMALE, true);
            t4.Id = "304";
            userManager.Create(t4, "Secret1");
            userManager.AddToRole(t4.Id, "teacher");

            Teacher t5 = CreateTeacher("Tamara", "Ilic", "svetlana.topalov@gmail.com", "teacher5", "+11111111111", "1111111111116", Genders.FEMALE, true);
            t5.Id = "305";
            userManager.Create(t5, "Secret1");
            userManager.AddToRole(t5.Id, "teacher");

            Teacher t6 = CreateTeacher("Dobrila", "Ivanovic", "svetlana.topalov@gmail.com", "teacher6", "+11111111111", "1111111111117", Genders.FEMALE, true);
            t6.Id = "306";
            userManager.Create(t6, "Secret1");
            userManager.AddToRole(t6.Id, "teacher");

            Teacher t7 = CreateTeacher("Nikola", "Kis", "svetlana.topalov@gmail.com", "teacher7", "+11111111111", "1111111111118", Genders.MALE, true);
            t7.Id = "307";
            userManager.Create(t7, "Secret1");
            userManager.AddToRole(t7.Id, "teacher");

            Teacher t8 = CreateTeacher("Bojan", "Kovac", "svetlana.topalov@gmail.com", "teacher8", "+11111111111", "1111111111119", Genders.MALE, true);
            t8.Id = "308";
            userManager.Create(t8, "Secret1");
            userManager.AddToRole(t8.Id, "teacher");

            Teacher t9 = CreateTeacher("Oliver", "Nikolic", "svetlana.topalov@gmail.com", "teacher9", "+11111111111", "1111111111110", Genders.FEMALE, true);
            t9.Id = "309";
            userManager.Create(t9, "Secret1");
            userManager.AddToRole(t9.Id, "teacher");
             
            Teacher t10 = CreateTeacher("Nikola", "Kovac", "svetlana.topalov@gmail.com", "teacher10", "+11111111111", "1111111111120", Genders.MALE, true);
            t10.Id = "310";
            userManager.Create(t10, "Secret1");
            userManager.AddToRole(t10.Id, "teacher");

            Teacher t11 = CreateTeacher("Bojan", "Kovac", "svetlana.topalov@gmail.com", "teacher11", "+11111111111", "1111111111121", Genders.MALE, true);
            t11.Id = "311";
            userManager.Create(t11, "Secret1");
            userManager.AddToRole(t11.Id, "teacher");

            Teacher t12 = CreateTeacher("Jovana", "Bojanic", "svetlana.topalov@gmail.com", "teacher12", "+11111111111", "1111111111122", Genders.FEMALE, true);
            t12.Id = "312";
            userManager.Create(t12, "Secret1");
            userManager.AddToRole(t12.Id, "teacher");        

            Teacher t13 = CreateTeacher("Dragan", "Jovic", "svetlana.topalov@gmail.com", "teacher13", "+11111111111", "1111111111123", Genders.MALE, false);
            t13.Id = "313";
            userManager.Create(t13, "Secret1");
            userManager.AddToRole(t13.Id, "teacher");
           

            Teacher t14 = CreateTeacher("Milica", "Jovic", "svetlana.topalov@gmail.com", "teacher14", "+11111111111", "1111111111124", Genders.FEMALE, false);
            t14.Id = "314";
            userManager.Create(t14, "Secret1");
            userManager.AddToRole(t14.Id, "teacher");
           

            //kreiranje predmeta
            Subject CreateSubject(string n, int g, int num)
            {
                Subject sub = new Subject
                {
                    Name = n,
                    Grade = g,
                    NumberOfClassesPerWeek = num
                };
                return sub;
            }

           
            Subject sub101 = CreateSubject("Naziv predmeta za brisanje", 1, 5);
            sub101.Id = 101;
            subjects.Add(sub101);

            //kreiranje predmeta petog RAZREDA
            Subject sub11 = CreateSubject("Engleski jezik i knjizevnost", 5, 2);
            sub11.Id = 1;
            subjects.Add(sub11);
            Subject sub12 = CreateSubject("Muzicka kultura", 5, 2);
            sub12.Id = 2;
            subjects.Add(sub12);
            Subject sub13 = CreateSubject("Matematika", 5, 4);
            sub13.Id = 3;
            subjects.Add(sub13);
            Subject sub14 = CreateSubject("Likovna kultura", 5, 2);
            sub14.Id = 4;
            subjects.Add(sub14);
            Subject sub15 = CreateSubject("Fizicka kultura", 5, 2);
            sub15.Id = 5;
            subjects.Add(sub15);
            Subject sub16 = CreateSubject("Srpski jezik i knjizevnost", 5, 5);
            sub16.Id = 6;
            subjects.Add(sub16);
            Subject sub17 = CreateSubject("Istorija", 5, 1);
            sub17.Id = 7;
            subjects.Add(sub17);
            Subject sub18 = CreateSubject("Geografija", 5, 1);
            sub18.Id = 8;
            subjects.Add(sub18);
            Subject sub19 = CreateSubject("Biologija", 5, 2);
            sub19.Id = 9;
            subjects.Add(sub19);
            Subject sub20 = CreateSubject("Tehnicko i informatika", 5, 2);
            sub20.Id = 10;
            subjects.Add(sub20);

            //kreiranje predmeta sestog RAZREDA
            Subject sub21 = CreateSubject("Engleski jezik i knjizevnost", 6, 2);
            sub21.Id = 11;
            subjects.Add(sub21);
            Subject sub22 = CreateSubject("Muzicka kultura", 6, 1);
            sub22.Id = 12;
            subjects.Add(sub22);
            Subject sub23 = CreateSubject("Matematika", 6, 4);
            sub23.Id = 13;
            subjects.Add(sub23);
            Subject sub24 = CreateSubject("Likovna kultura", 6, 1);
            sub24.Id = 14;
            subjects.Add(sub24);
            Subject sub25 = CreateSubject("Fizicka kultura", 6, 2);
            sub25.Id = 15;
            subjects.Add(sub25);
            Subject sub26 = CreateSubject("Srpski jezik i knjizevnost", 6, 4);
            sub26.Id = 16;
            subjects.Add(sub26);
            Subject sub27 = CreateSubject("Istorija", 6, 2);
            sub27.Id = 17;
            subjects.Add(sub27);
            Subject sub28 = CreateSubject("Geografija", 6, 2);
            sub28.Id = 18;
            subjects.Add(sub28);
            Subject sub29 = CreateSubject("Biologija", 6, 2);
            sub29.Id = 19;
            subjects.Add(sub29);
            Subject sub30 = CreateSubject("Tehnicko i informatika", 6, 2);
            sub30.Id = 20;
            subjects.Add(sub30);
            Subject sub31 = CreateSubject("Fizika", 6, 2); 
            sub31.Id = 21;
            subjects.Add(sub31);

            //slobodan predmet za CREATE TS I CREATE FTS
            Subject sub32 = CreateSubject("Izbodni predmet", 6, 1);
            sub32.Id = 22;
            subjects.Add(sub32);

            //slobodan predmet za CREATE TS I CREATE FTS
            Subject sub33 = CreateSubject("Izbodni predmet", 5, 1);
            sub33.Id = 23;
            subjects.Add(sub33);

            context.Subjects.AddRange(subjects);

            //kreiranje tabele nastavnik-predmet 
            TeacherToSubject CreateTS(Teacher t, Subject sub, DateTime started, DateTime? stopped)
            {
                TeacherToSubject ts = new TeacherToSubject
                {
                    Teacher = t,
                    Subject = sub,
                    StartedTeaching = started,
                    StoppedTeaching = stopped
                };

                return ts;
            }

            TeacherToSubject ts1 = CreateTS(t5, sub14, new DateTime(2000, 9, 1), new DateTime(2010, 6, 30));
            ts1.Id = 1;           
            t5.TeachersSubjects.Add(ts1); 
            sub14.SubjectsTeachers.Add(ts1);
            teachersToSubjects.Add(ts1);

            //isti nastavnik predaje dva predmeta 
            TeacherToSubject ts2 = CreateTS(t2, sub11, new DateTime(2010, 9, 1), null);
            ts2.Id = 2;            
            t2.TeachersSubjects.Add(ts2);       
            sub11.SubjectsTeachers.Add(ts2);     
            teachersToSubjects.Add(ts2);

            TeacherToSubject ts3 = CreateTS(t2, sub21, new DateTime(2010, 9, 1), null);
            ts3.Id = 3;           
            t2.TeachersSubjects.Add(ts3);
            sub21.SubjectsTeachers.Add(ts3);
            teachersToSubjects.Add(ts3);

           
            TeacherToSubject ts4 = CreateTS(t3, sub12, new DateTime(2011, 9, 1), null);
            ts4.Id = 4;          
            t3.TeachersSubjects.Add(ts4);
            sub12.SubjectsTeachers.Add(ts4);
            teachersToSubjects.Add(ts4);

            TeacherToSubject ts5 = CreateTS(t3, sub22, new DateTime(2012, 9, 1), null);
            ts5.Id = 5;           
            t3.TeachersSubjects.Add(ts5);
            sub22.SubjectsTeachers.Add(ts5);
            teachersToSubjects.Add(ts5);

            
            TeacherToSubject ts6 = CreateTS(t4, sub13, new DateTime(2012, 9, 1), null);
            ts6.Id = 6;           
            t4.TeachersSubjects.Add(ts6);
            sub13.SubjectsTeachers.Add(ts6);
            teachersToSubjects.Add(ts6);

            TeacherToSubject ts7 = CreateTS(t4, sub23, new DateTime(2012, 9, 1), null);
            ts7.Id = 7;            
            t4.TeachersSubjects.Add(ts7);
            sub23.SubjectsTeachers.Add(ts7);
            teachersToSubjects.Add(ts7);

            
            TeacherToSubject ts8 = CreateTS(t6, sub14, new DateTime(2013, 9, 1), null);
            ts8.Id = 8;          
            t6.TeachersSubjects.Add(ts8);
            sub14.SubjectsTeachers.Add(ts8);
            teachersToSubjects.Add(ts8);

            TeacherToSubject ts9 = CreateTS(t6, sub24, new DateTime(2016, 9, 1), null);
            ts9.Id = 9;          
            t6.TeachersSubjects.Add(ts9);
            sub24.SubjectsTeachers.Add(ts9);
            teachersToSubjects.Add(ts9);

           
            TeacherToSubject ts10 = CreateTS(t7, sub15, new DateTime(2010, 9, 1), null);
            ts10.Id = 10;           
            t7.TeachersSubjects.Add(ts10);
            sub15.SubjectsTeachers.Add(ts10);
            teachersToSubjects.Add(ts10);

            TeacherToSubject ts11 = CreateTS(t7, sub25, new DateTime(2011, 9, 1), null);
            ts11.Id = 11;           
            t7.TeachersSubjects.Add(ts11);
            sub25.SubjectsTeachers.Add(ts11);
            teachersToSubjects.Add(ts11);

            
            TeacherToSubject ts12 = CreateTS(t8, sub16, new DateTime(2010, 9, 1), null);
            ts12.Id = 12;
            t8.TeachersSubjects.Add(ts12);
            sub16.SubjectsTeachers.Add(ts12);
            teachersToSubjects.Add(ts12);

            TeacherToSubject ts13 = CreateTS(t8, sub26, new DateTime(2014, 9, 1), null);
            ts13.Id = 13;
            t8.TeachersSubjects.Add(ts13);
            sub26.SubjectsTeachers.Add(ts13);
            teachersToSubjects.Add(ts13);

           
            TeacherToSubject ts14 = CreateTS(t9, sub17, new DateTime(2010, 9, 1), null);
            ts14.Id = 14;
            t9.TeachersSubjects.Add(ts14);
            sub17.SubjectsTeachers.Add(ts14);
            teachersToSubjects.Add(ts14);

            TeacherToSubject ts15 = CreateTS(t9, sub27, new DateTime(2015, 9, 1), null);
            ts15.Id = 15;
            t9.TeachersSubjects.Add(ts15);
            sub27.SubjectsTeachers.Add(ts15);
            teachersToSubjects.Add(ts15);

            
            TeacherToSubject ts16 = CreateTS(t10, sub18, new DateTime(2010, 9, 1), null);
            ts16.Id = 16;
            t10.TeachersSubjects.Add(ts16);
            sub18.SubjectsTeachers.Add(ts16);
            teachersToSubjects.Add(ts16);

            TeacherToSubject ts17 = CreateTS(t10, sub28, new DateTime(2012, 9, 1), null);
            ts17.Id = 17; 
            t10.TeachersSubjects.Add(ts17);
            sub28.SubjectsTeachers.Add(ts17);
            teachersToSubjects.Add(ts17);

           
            TeacherToSubject ts18 = CreateTS(t11, sub19, new DateTime(2011, 9, 1), null);
            ts18.Id = 18;
            t11.TeachersSubjects.Add(ts18);
            sub19.SubjectsTeachers.Add(ts18);
            teachersToSubjects.Add(ts18);

            TeacherToSubject ts19 = CreateTS(t11, sub29, new DateTime(2010, 9, 1), null);
            ts19.Id = 19;
            t11.TeachersSubjects.Add(ts19);
            sub29.SubjectsTeachers.Add(ts19);
            teachersToSubjects.Add(ts19);

           
            TeacherToSubject ts20 = CreateTS(t12, sub20, new DateTime(2012, 9, 1), null);
            ts20.Id = 20;
            t12.TeachersSubjects.Add(ts20);
            sub20.SubjectsTeachers.Add(ts20);
            teachersToSubjects.Add(ts20);

            TeacherToSubject ts21 = CreateTS(t12, sub30, new DateTime(2011, 9, 1), null);
            ts21.Id = 21;
            t12.TeachersSubjects.Add(ts21);
            sub30.SubjectsTeachers.Add(ts21);
            teachersToSubjects.Add(ts21);

           
            TeacherToSubject ts22 = CreateTS(t4, sub31, new DateTime(2015, 9, 1), null);
            ts22.Id = 22;
            t4.TeachersSubjects.Add(ts22);
            sub31.SubjectsTeachers.Add(ts22);
            teachersToSubjects.Add(ts22);

           
            TeacherToSubject ts23 = CreateTS(t8, sub11, new DateTime(2015, 9, 1), new DateTime(2016, 9, 1));
            ts23.Id = 23;
            t8.TeachersSubjects.Add(ts23);
            sub11.SubjectsTeachers.Add(ts23);
            teachersToSubjects.Add(ts23);

            
            TeacherToSubject ts24 = CreateTS(t9, sub11, new DateTime(2014, 9, 1), new DateTime(2015, 9, 1));
            ts24.Id = 23;
            t9.TeachersSubjects.Add(ts24);
            sub11.SubjectsTeachers.Add(ts24);
            teachersToSubjects.Add(ts24);

            context.TeachersToSubjects.AddRange(teachersToSubjects);

            //kreiranje odeljenja
            Form CreateForm(int grade, string tag, DateTime started, Teacher t)
            {
                Form f = new Form
                {
                    Grade = grade,
                    Tag = tag,
                    Started = started,
                    AttendingTeacher = t
                };
                return f;
            }


            Form f1 = CreateForm(8, "1", new DateTime(2017, 9, 1), t2);
            f1.Id = 1;
            t2.FormAttending = f1;

            forms.Add(f1);
                        
            Form f2 = CreateForm(5, "1", new DateTime(2018, 9, 1), t3);
            f2.Id = 2;
            t3.FormAttending = f2;
            forms.Add(f2);

            Form f3 = CreateForm(5, "2", new DateTime(2018, 9, 1), t4);
            f3.Id = 3;
            t4.FormAttending = f3;
            forms.Add(f3);


            Form f4 = CreateForm(5, "3", new DateTime(2018, 9, 1), t5);
            f4.Id = 4;
            t5.FormAttending = f4;
            forms.Add(f4);
 
            Form f5 = CreateForm(6, "1", new DateTime(2018, 9, 1), t6);
            f5.Id = 5;
            t6.FormAttending = f5;
            forms.Add(f5);

            Form f6 = CreateForm(6, "2", new DateTime(2018, 9, 1), t7);
            f6.Id = 6;
            t7.FormAttending = f6;
            forms.Add(f6);


            Form f7 = CreateForm(6, "3", new DateTime(2018, 9, 1), t8);
            f7.Id = 7;
            t8.FormAttending = f7;
            forms.Add(f7);

            Form f8 = CreateForm(5, "1", new DateTime(2017, 9, 1), t9);
            f8.Id = 8;
            t9.FormAttending = f8;

            forms.Add(f8);

            context.Forms.AddRange(forms);


            //kreiranje roditelja
            Parent CreateParent(string fn, string ln, string email, string un, string pn, string jmbg, string mp)
            {
                Parent p = new Parent
                {
                    FirstName = fn,
                    LastName = ln,
                    Email = email,
                    EmailConfirmed = true,
                    UserName = un,
                    PhoneNumber = pn,
                    PhoneNumberConfirmed = true,
                    Jmbg = jmbg,
                    MobilePhone = mp
                };

                return p;
            }

            //kreiranje ucenika
            Student CreateStudent(string fn, string ln, string email, string un, string pn, string jmbg, DateTime dob, string ip, bool isActive, Parent p, Form f)
            {
                Student s = new Student
                {
                    FirstName = fn,
                    LastName = ln,
                    Email = email,
                    EmailConfirmed = true,
                    UserName = un,
                    PhoneNumber = pn,
                    PhoneNumberConfirmed = true,
                    Jmbg = jmbg,
                    DayOfBirth = dob,
                    ImagePath = ip,
                    IsActive = isActive,
                    Parent = p, 
                    Form = f 
                };
                return s;
            }

           
            Parent p1 = CreateParent("Milana", "Vulin", "svetlana.topalov@gmail.com", "parent1", "+11111111111", "1111111111125", "1111111111");
            p1.Id = "201";
            userManager.Create(p1, "Secret1");
            userManager.AddToRole(p1.Id, "parent");

            //p1, f1
            Student s1 = CreateStudent("Vanja", "Vulin", "svetlana.topalov@gmail.com", "student1", "+111111111111", "1111111111142", new DateTime(2005, 7, 26), null, true, p1, f1);
            s1.Id = "101";
            f1.Students.Add(s1);
            userManager.Create(s1, "Secret1");
            userManager.AddToRole(s1.Id, "student");

            //kreiranje roditelja sa troje dece u istom odeljenju s2, s3, s4 - f2
            Parent p2 = CreateParent("Ana", "Topalov", "svetlana.topalov@gmail.com", "parent2", "+11111111111", "1111111111126", "1111111111");
            p2.Id = "202";
            userManager.Create(p2, "Secret1");
            userManager.AddToRole(p2.Id, "parent");

            //p2, f2
            Student s2 = CreateStudent("Nevena", "Topalov", "svetlana.topalov@gmail.com", "student2", "+111111111111", "1111111111143", new DateTime(2007, 7, 26), "D:\\1-Project\\Project\\eSchool\\App_Data\\BodyPart_9c5e9467-9fbc-47f6-a592-67c0561c7411", true, p2, f2);
            s2.Id = "102";
            f2.Students.Add(s2);
            userManager.Create(s2, "Secret1");
            userManager.AddToRole(s2.Id, "student");

            //p2, f2
            Student s3 = CreateStudent("Visnja", "Topalov", "svetlana.topalov@gmail.com", "student3", "+111111111111", "1111111111144", new DateTime(2007, 7, 26), "D:\\1-Project\\Project\\eSchool\\App_Data\\BodyPart_9c5e9467-9fbc-47f6-a592-67c0561c7411", true, p2, f2);
            s3.Id = "103";
            f2.Students.Add(s3);
            userManager.Create(s3, "Secret1");
            userManager.AddToRole(s3.Id, "student");

            //p2, f2
            Student s4 = CreateStudent("Suncica", "Topalov", "svetlana.topalov@gmail.com", "student4", "+111111111111", "1111111111145", new DateTime(2007, 7, 26), "D:\\1-Project\\Project\\eSchool\\App_Data\\BodyPart_9c5e9467-9fbc-47f6-a592-67c0561c7411", true, p2, f2);
            s4.Id = "104";
            f2.Students.Add(s4);
            userManager.Create(s4, "Secret1");
            userManager.AddToRole(s4.Id, "student");

            //kreiranje roditelja sa dvoje dece u razlicitim odeljenjima s5-f2 , s6-f5
            Parent p3 = CreateParent("Ivan", "Zelenkovic", "svetlana.topalov@gmail.com", "parent3", "+11111111111", "1111111111127", "1111111111");
            p3.Id = "203";
            userManager.Create(p3, "Secret1");
            userManager.AddToRole(p3.Id, "parent");

            //p3, f2
            Student s5 = CreateStudent("Luciana", "Zelenkovic", "svetlana.topalov@gmail.com", "student5", "+111111111111", "1111111111146", new DateTime(2007, 5, 24), "D:\\1-Project\\Project\\eSchool\\App_Data\\BodyPart_9c5e9467-9fbc-47f6-a592-67c0561c7411", true, p3, f2);
            s5.Id = "105";
            f2.Students.Add(s5);
            userManager.Create(s5, "Secret1");
            userManager.AddToRole(s5.Id, "student");

            //p3, f5
            Student s6 = CreateStudent("Iva", "Zelenkovic", "svetlana.topalov@gmail.com", "student6", "+111111111111", "1111111111147", new DateTime(2006, 1, 27), "D:\\1 - Project\\Project\\eSchool\\App_Data\\BodyPart_9c5e9467 - 9fbc - 47f6 - a592 - 67c0561c7411", true, p3, f5);
            s6.Id = "106";
            f5.Students.Add(s6);
            userManager.Create(s6, "Secret1");
            userManager.AddToRole(s6.Id, "student");

           
            Parent p4 = CreateParent("Natasa", "Stojkovic", "svetlana.topalov@gmail.com", "parent4", "+11111111111", "1111111111128", "1111111111");
            p4.Id = "204";
            userManager.Create(p4, "Secret1");
            userManager.AddToRole(p4.Id, "parent");

            //p4,f2
            Student s7 = CreateStudent("Emina", "Stojkovic", "svetlana.topalov@gmail.com", "student7", "+111111111111", "1111111111148", new DateTime(2007, 9, 1), "D:\\1-Project\\Project\\eSchool\\App_Data\\BodyPart_9c5e9467-9fbc-47f6-a592-67c0561c7411", true, p4, f2);
            s7.Id = "107";
            f2.Students.Add(s7);
            userManager.Create(s7, "Secret1");
            userManager.AddToRole(s7.Id, "student");

            //p4,f5
            Student s8 = CreateStudent("Kristijan", "Stojkovic", "svetlana.topalov@gmail.com", "student8", "+111111111111", "1111111111149", new DateTime(2006, 2, 14), "D:\\1-Project\\Project\\eSchool\\App_Data\\BodyPart_9c5e9467-9fbc-47f6-a592-67c0561c7411", true, p4, f5);
            s8.Id = "108";
            f5.Students.Add(s8);
            userManager.Create(s8, "Secret1");
            userManager.AddToRole(s8.Id, "student");

            //s9,f3
            Parent p5 = CreateParent("Jelena", "Rudic", "svetlana.topalov@gmail.com", "parent5", "+11111111111", "1111111111129", "1111111111");
            p5.Id = "205";
            userManager.Create(p5, "Secret1");
            userManager.AddToRole(p5.Id, "parent");

            //p5,f3
            Student s9 = CreateStudent("Dominik", "Rudic", "svetlana.topalov@gmail.com", "student9", "+111111111111", "1111111111150", new DateTime(2007, 2, 14), null, true, p5, f3);
            s9.Id = "109";
            f3.Students.Add(s9); 
            userManager.Create(s9, "Secret1");
            userManager.AddToRole(s9.Id, "student");

            //s10,f3
            Parent p6 = CreateParent("Milos", "Nenadic", "svetlana.topalov@gmail.com", "parent6", "+11111111111", "1111111111130", "1111111111");
            p6.Id = "206";
            userManager.Create(p6, "Secret1");
            userManager.AddToRole(p6.Id, "parent");

            //p6,f3
            Student s10 = CreateStudent("Boris", "Nenadic", "svetlana.topalov@gmail.com", "student10", "+111111111111", "1111111111151", new DateTime(2007, 2, 14), null, true, p6, f3);
            s10.Id = "110";
            f3.Students.Add(s10);
            userManager.Create(s10, "Secret1");
            userManager.AddToRole(s10.Id, "student");

            //s11,f3
            Parent p7 = CreateParent("Dusan", "Lukic", "svetlana.topalov@gmail.com", "parent7", "+11111111111", "1111111111131", "1111111111");
            p7.Id = "207";
            userManager.Create(p7, "Secret1");
            userManager.AddToRole(p7.Id, "parent");

            //p7,f3
            Student s11 = CreateStudent("Tomislav", "Lukic", "svetlana.topalov@gmail.com", "student11", "+111111111111", "1111111111152", new DateTime(2007, 2, 14), null, true, p7, f3);
            s11.Id = "111";
            f3.Students.Add(s11);
            userManager.Create(s11, "Secret1");
            userManager.AddToRole(s11.Id, "student");

            Parent p8 = CreateParent("Slavko", "Malic", "svetlana.topalov@gmail.com", "parent8", "+11111111111", "1111111111132", "1111111111");
            p8.Id = "208";
            userManager.Create(p8, "Secret1");
            userManager.AddToRole(p8.Id, "parent");

            //p8,f3
            Student s12 = CreateStudent("Ana", "Malic", "svetlana.topalov@gmail.com", "student12", "+111111111111", "1111111111153", new DateTime(2007, 2, 14), null, true, p8, f3);
            s12.Id = "112";
            f3.Students.Add(s12);
            userManager.Create(s12, "Secret1");
            userManager.AddToRole(s12.Id, "student");

            Parent p9 = CreateParent("Marko", "Marijanovic", "svetlana.topalov@gmail.com", "parent9", "+11111111111", "1111111111133", "1111111111");
            p9.Id = "209";
            userManager.Create(p9, "Secret1");
            userManager.AddToRole(p9.Id, "parent");

            //p9,f4
            Student s13 = CreateStudent("Tamara", "Marijanovic", "svetlana.topalov@gmail.com", "student13", "+111111111111", "1111111111154", new DateTime(2007, 2, 14), null, true, p9, f4);
            s13.Id = "113";
            f4.Students.Add(s13);
            userManager.Create(s13, "Secret1");
            userManager.AddToRole(s13.Id, "student");

            Parent p10 = CreateParent("Mina", "Lukac", "svetlana.topalov@gmail.com", "parent10", "+11111111111", "1111111111134", "1111111111");
            p10.Id = "210"; 
            userManager.Create(p10, "Secret1");
            userManager.AddToRole(p10.Id, "parent");

            //p10,f4
            Student s14 = CreateStudent("Emilija", "Lukac", "svetlana.topalov@gmail.com", "student14", "+111111111111", "1111111111155", new DateTime(2007, 2, 14), null, true, p10, f4);
            s14.Id = "114";
            f4.Students.Add(s14);
            userManager.Create(s14, "Secret1");
            userManager.AddToRole(s14.Id, "student");

            //p10,f4
            Student s15 = CreateStudent("Roberta", "Lukac", "svetlana.topalov@gmail.com", "student15", "+111111111111", "1111111111156", new DateTime(2007, 2, 14), null, true, p10, f4);
            s15.Id = "115";
            f4.Students.Add(s15);
            userManager.Create(s15, "Secret1");
            userManager.AddToRole(s15.Id, "student");

            Parent p11 = CreateParent("Olja", "Jovicin", "svetlana.topalov@gmail.com", "parent11", "+11111111111", "1111111111132", "1111111111");
            p11.Id = "211";
            userManager.Create(p11, "Secret1");
            userManager.AddToRole(p11.Id, "parent");

            //p11,f4
            Student s16 = CreateStudent("Ivona", "Jovicin", "svetlana.topalov@gmail.com", "student16", "+111111111111", "1111111111157", new DateTime(2007, 2, 14), null, true, p11, f4);
            s16.Id = "116";
            f4.Students.Add(s16);
            userManager.Create(s16, "Secret1");
            userManager.AddToRole(s16.Id, "student");

            Parent p12 = CreateParent("Sonja", "Gracanin", "svetlana.topalov@gmail.com", "parent12", "+11111111111", "1111111111133", "1111111111");
            p12.Id = "212";
            userManager.Create(p12, "Secret1");
            userManager.AddToRole(p12.Id, "parent");

            //p12,f5
            Student s17 = CreateStudent("Sanja", "Gracanin", "svetlana.topalov@gmail.com", "student17", "+111111111111", "1111111111158", new DateTime(2006, 2, 14), null, true, p12, f5);
            s17.Id = "117";
            f5.Students.Add(s17);
            userManager.Create(s17, "Secret1");
            userManager.AddToRole(s17.Id, "student");

            //p12,f5
            Student s18 = CreateStudent("Vanja", "Gracanin", "svetlana.topalov@gmail.com", "student18", "+111111111111", "1111111111159", new DateTime(2006, 2, 14), null, true, p12, f5);
            s18.Id = "118";
            f5.Students.Add(s18);
            userManager.Create(s18, "Secret1");
            userManager.AddToRole(s18.Id, "student");

            Parent p13 = CreateParent("Nevena", "Dudas", "svetlana.topalov@gmail.com", "parent13", "+11111111111", "1111111111134", "1111111111");
            p13.Id = "213";
            userManager.Create(p13, "Secret1");
            userManager.AddToRole(p13.Id, "parent");

            //p13,f6
            Student s19 = CreateStudent("Natasa", "Dudas", "svetlana.topalov@gmail.com", "student19", "+111111111111", "1111111111160", new DateTime(2006, 2, 14), null, true, p13, f6);
            s19.Id = "119";
            f6.Students.Add(s19);
            userManager.Create(s19, "Secret1");
            userManager.AddToRole(s19.Id, "student");

            Parent p14 = CreateParent("Stefan", "Cizmar", "svetlana.topalov@gmail.com", "parent14", "+11111111111", "1111111111135", "1111111111");
            p14.Id = "214";
            userManager.Create(p14, "Secret1");
            userManager.AddToRole(p14.Id, "parent");

            //p14,f6
            Student s20 = CreateStudent("Sasa", "Cizmar", "svetlana.topalov@gmail.com", "student20", "+111111111111", "1111111111161", new DateTime(2006, 2, 14), null, true, p14, f6);
            s20.Id = "120";
            f6.Students.Add(s20);
            userManager.Create(s20, "Secret1");
            userManager.AddToRole(s20.Id, "student");

            Parent p15 = CreateParent("Ana", "Bagic", "svetlana.topalov@gmail.com", "parent15", "+11111111111", "1111111111136", "1111111111");
            p15.Id = "215";
            userManager.Create(p15, "Secret1");
            userManager.AddToRole(p15.Id, "parent");

            //p15,f6
            Student s21 = CreateStudent("Vladimir", "Bagic", "svetlana.topalov@gmail.com", "student21", "+111111111111", "1111111111162", new DateTime(2006, 2, 14), null, true, p15, f6);
            s21.Id = "121";
            f6.Students.Add(s21);
            userManager.Create(s21, "Secret1");
            userManager.AddToRole(s21.Id, "student");

            Parent p16 = CreateParent("Milica", "Andric", "svetlana.topalov@gmail.com", "parent16", "+11111111111", "1111111111137", "1111111111");
            p16.Id = "216";
            userManager.Create(p16, "Secret1");
            userManager.AddToRole(p16.Id, "parent");

            //p16,f6
            Student s22 = CreateStudent("Aca", "Andric", "svetlana.topalov@gmail.com", "student22", "+111111111111", "1111111111163", new DateTime(2006, 2, 14), null, true, p16, f6);
            s22.Id = "122";
            f6.Students.Add(s22);
            userManager.Create(s22, "Secret1");
            userManager.AddToRole(s22.Id, "student");

            Parent p17 = CreateParent("Nika", "Colic", "svetlana.topalov@gmail.com", "parent17", "+11111111111", "1111111111138", "1111111111");
            p17.Id = "217";
            userManager.Create(p17, "Secret1");
            userManager.AddToRole(p17.Id, "parent");

            //p17,f7
            Student s23 = CreateStudent("Simonida", "Colic", "svetlana.topalov@gmail.com", "student23", "+111111111111", "1111111111164", new DateTime(2006, 2, 14), null, true, p17, f7);
            s23.Id = "123";
            f7.Students.Add(s23);
            userManager.Create(s23, "Secret1");
            userManager.AddToRole(s23.Id, "student");

            Parent p18 = CreateParent("Sava", "Gligorin", "svetlana.topalov@gmail.com", "parent18", "+11111111111", "1111111111139", "1111111111");
            p18.Id = "218";
            userManager.Create(p18, "Secret1");
            userManager.AddToRole(p18.Id, "parent");

            //p18,f7
            Student s24 = CreateStudent("Isidora", "Gligorin", "svetlana.topalov@gmail.com", "student24", "+111111111111", "1111111111165", new DateTime(2006, 2, 14), null, true, p18, f7);
            s24.Id = "124";
            f7.Students.Add(s24);
            userManager.Create(s24, "Secret1");
            userManager.AddToRole(s24.Id, "student");

            //p18,f7
            Student s25 = CreateStudent("Maksim", "Gligorin", "svetlana.topalov@gmail.com", "student25", "+111111111111", "1111111111166", new DateTime(2006, 2, 14), null, true, p18, f7);
            s25.Id = "125";
            f7.Students.Add(s25);
            userManager.Create(s25, "Secret1");
            userManager.AddToRole(s25.Id, "student");

            Parent p19 = CreateParent("Martina", "Hudak", "svetlana.topalov@gmail.com", "parent19", "+11111111111", "1111111111140", "1111111111");
            p19.Id = "219";
            userManager.Create(p19, "Secret1");
            userManager.AddToRole(p19.Id, "parent");

            //p19,f7
            Student s26 = CreateStudent("Lazar", "Hudak", "svetlana.topalov@gmail.com", "student26", "+111111111111", "1111111111167", new DateTime(2006, 2, 14), null, true, p19, f7);
            s26.Id = "126";
            f7.Students.Add(s26);
            userManager.Create(s26, "Secret1");
            userManager.AddToRole(s26.Id, "student");

            Parent p20 = CreateParent("Milica", "Jeftic", "svetlana.topalov@gmail.com", "parent20", "+11111111111", "1111111111141", "1111111111");
            p20.Id = "220";
            userManager.Create(p20, "Secret1");
            userManager.AddToRole(p20.Id, "parent");

            //p20,f7
            Student s27 = CreateStudent("Marija", "Jeftic", "svetlana.topalov@gmail.com", "student27", "+111111111111", "1111111111168", new DateTime(2006, 2, 14), null, false, p20, f7);
            s27.Id = "127";
            f7.Students.Add(s27);
            userManager.Create(s27, "Secret1");
            userManager.AddToRole(s27.Id, "student");

            //p20,f7
            Student s28 = CreateStudent("Ivana", "Jeftic", "svetlana.topalov@gmail.com", "student28", "+111111111111", "1111111111169", new DateTime(2006, 2, 14), null, false, p20, f7);
            s28.Id = "128";
            f7.Students.Add(s28);
            userManager.Create(s28, "Secret1");
            userManager.AddToRole(s28.Id, "student");


            //kreiranje tabele odeljenje-nastavnik-predmet 
            FormToTeacherSubject CreateFTS(Form f, TeacherToSubject ts, DateTime started, DateTime? stopped)
            {
                FormToTeacherSubject created = new FormToTeacherSubject
                {
                    Form = f,
                    TeacherToSubject = ts,  
                    Started = started,
                    Stopped = stopped
                };
                return created; 
            }

            FormToTeacherSubject fts1 = CreateFTS(f2, ts1, new DateTime(2000, 9, 1), new DateTime(2010, 6, 30));
            fts1.Id = 1;
            f2.FormsTeachersToSubjects.Add(fts1);
            ts1.TeacherSubjectForms.Add(fts1);
            formsToTeacherSubjects.Add(fts1);
            
            FormToTeacherSubject fts2 = CreateFTS(f2, ts2, new DateTime(2018, 9, 1), null);
            fts2.Id = 2;           
            f2.FormsTeachersToSubjects.Add(fts2);
            ts2.TeacherSubjectForms.Add(fts2);
            formsToTeacherSubjects.Add(fts2);

            FormToTeacherSubject fts3 = CreateFTS(f2, ts4, new DateTime(2018, 9, 1), null);
            fts3.Id = 3;          
            f2.FormsTeachersToSubjects.Add(fts3);
            ts4.TeacherSubjectForms.Add(fts3);
            formsToTeacherSubjects.Add(fts3);

            FormToTeacherSubject fts4 = CreateFTS(f2, ts6, new DateTime(2018, 9, 1), null);
            fts4.Id = 4;          
            f2.FormsTeachersToSubjects.Add(fts4);
            ts6.TeacherSubjectForms.Add(fts4);
            formsToTeacherSubjects.Add(fts4);

            FormToTeacherSubject fts5 = CreateFTS(f2, ts8, new DateTime(2018, 9, 1), null);
            fts5.Id = 5;            
            f2.FormsTeachersToSubjects.Add(fts5);
            ts8.TeacherSubjectForms.Add(fts5);
            formsToTeacherSubjects.Add(fts5);

            FormToTeacherSubject fts6 = CreateFTS(f2, ts10, new DateTime(2018, 9, 1), null);
            fts6.Id = 6;            
            f2.FormsTeachersToSubjects.Add(fts6);
            ts10.TeacherSubjectForms.Add(fts6);
            formsToTeacherSubjects.Add(fts6);

            FormToTeacherSubject fts7 = CreateFTS(f2, ts12, new DateTime(2018, 9, 1), null);
            fts7.Id = 7;
            f2.FormsTeachersToSubjects.Add(fts7);
            ts12.TeacherSubjectForms.Add(fts7);
            formsToTeacherSubjects.Add(fts7);

            FormToTeacherSubject fts8 = CreateFTS(f2, ts14, new DateTime(2018, 9, 1), null);
            fts8.Id = 8;
            f2.FormsTeachersToSubjects.Add(fts8);
            ts14.TeacherSubjectForms.Add(fts8);
            formsToTeacherSubjects.Add(fts8);

            FormToTeacherSubject fts9 = CreateFTS(f2, ts16, new DateTime(2018, 9, 1), null);
            fts9.Id = 9;
            f2.FormsTeachersToSubjects.Add(fts9);
            ts16.TeacherSubjectForms.Add(fts9);
            formsToTeacherSubjects.Add(fts9);

            FormToTeacherSubject fts10 = CreateFTS(f2, ts18, new DateTime(2018, 9, 1), null);
            fts10.Id = 10;
            f2.FormsTeachersToSubjects.Add(fts10);
            ts18.TeacherSubjectForms.Add(fts10);
            formsToTeacherSubjects.Add(fts10);

            FormToTeacherSubject fts11 = CreateFTS(f2, ts20, new DateTime(2018, 9, 1), null);
            fts11.Id = 11;
            f2.FormsTeachersToSubjects.Add(fts11);
            ts20.TeacherSubjectForms.Add(fts11);
            formsToTeacherSubjects.Add(fts11);

            FormToTeacherSubject fts12 = CreateFTS(f3, ts2, new DateTime(2018, 9, 1), null);
            fts12.Id = 12;
            f3.FormsTeachersToSubjects.Add(fts12);
            ts2.TeacherSubjectForms.Add(fts12);
            formsToTeacherSubjects.Add(fts12);

            FormToTeacherSubject fts13 = CreateFTS(f3, ts4, new DateTime(2018, 9, 1), null);
            fts13.Id = 13;
            f3.FormsTeachersToSubjects.Add(fts13);
            ts4.TeacherSubjectForms.Add(fts13);
            formsToTeacherSubjects.Add(fts13);

            FormToTeacherSubject fts14 = CreateFTS(f3, ts6, new DateTime(2018, 9, 1), null);
            fts14.Id = 14;
            f3.FormsTeachersToSubjects.Add(fts14);
            ts6.TeacherSubjectForms.Add(fts14);
            formsToTeacherSubjects.Add(fts14);

            FormToTeacherSubject fts15 = CreateFTS(f3, ts8, new DateTime(2018, 9, 1), null);
            fts15.Id = 15;
            f3.FormsTeachersToSubjects.Add(fts15);
            ts8.TeacherSubjectForms.Add(fts15);
            formsToTeacherSubjects.Add(fts15);
           
            FormToTeacherSubject fts16 = CreateFTS(f3, ts10, new DateTime(2018, 9, 1), null);
            fts16.Id = 16;
            f3.FormsTeachersToSubjects.Add(fts16);
            ts10.TeacherSubjectForms.Add(fts16);
            formsToTeacherSubjects.Add(fts16);

            FormToTeacherSubject fts17 = CreateFTS(f3, ts12, new DateTime(2018, 9, 1), null);
            fts17.Id = 17;
            f3.FormsTeachersToSubjects.Add(fts17);
            ts12.TeacherSubjectForms.Add(fts17);
            formsToTeacherSubjects.Add(fts17);

            FormToTeacherSubject fts18 = CreateFTS(f3, ts14, new DateTime(2018, 9, 1), null);
            fts18.Id = 18;
            f3.FormsTeachersToSubjects.Add(fts18);
            ts14.TeacherSubjectForms.Add(fts18);
            formsToTeacherSubjects.Add(fts18);

            FormToTeacherSubject fts19 = CreateFTS(f3, ts16, new DateTime(2018, 9, 1), null);
            fts19.Id = 19;
            f3.FormsTeachersToSubjects.Add(fts19);
            ts16.TeacherSubjectForms.Add(fts19);
            formsToTeacherSubjects.Add(fts19);

            FormToTeacherSubject fts20 = CreateFTS(f3, ts18, new DateTime(2018, 9, 1), null);
            fts20.Id = 20;
            f3.FormsTeachersToSubjects.Add(fts20);
            ts18.TeacherSubjectForms.Add(fts20);
            formsToTeacherSubjects.Add(fts20);

            FormToTeacherSubject fts21 = CreateFTS(f3, ts20, new DateTime(2018, 9, 1), null);
            fts21.Id = 21;
            f3.FormsTeachersToSubjects.Add(fts21);
            ts20.TeacherSubjectForms.Add(fts21);
            formsToTeacherSubjects.Add(fts21);

            FormToTeacherSubject fts22 = CreateFTS(f4, ts2, new DateTime(2018, 9, 1), null);
            fts22.Id = 22;
            f4.FormsTeachersToSubjects.Add(fts22);
            ts2.TeacherSubjectForms.Add(fts22);
            formsToTeacherSubjects.Add(fts22);

            FormToTeacherSubject fts23 = CreateFTS(f4, ts4, new DateTime(2018, 9, 1), null);
            fts23.Id = 23;
            f4.FormsTeachersToSubjects.Add(fts23);
            ts4.TeacherSubjectForms.Add(fts23);
            formsToTeacherSubjects.Add(fts23);

            FormToTeacherSubject fts24 = CreateFTS(f4, ts6, new DateTime(2018, 9, 1), null);
            fts24.Id = 24;
            f4.FormsTeachersToSubjects.Add(fts24);
            ts6.TeacherSubjectForms.Add(fts24);
            formsToTeacherSubjects.Add(fts24);

            FormToTeacherSubject fts25 = CreateFTS(f4, ts8, new DateTime(2018, 9, 1), null);
            fts25.Id = 25;
            f4.FormsTeachersToSubjects.Add(fts25);
            ts8.TeacherSubjectForms.Add(fts25);
            formsToTeacherSubjects.Add(fts25);

            FormToTeacherSubject fts26 = CreateFTS(f4, ts10, new DateTime(2018, 9, 1), null);
            fts26.Id = 26;
            f4.FormsTeachersToSubjects.Add(fts26);
            ts10.TeacherSubjectForms.Add(fts26);
            formsToTeacherSubjects.Add(fts26);

            FormToTeacherSubject fts27 = CreateFTS(f4, ts12, new DateTime(2018, 9, 1), null);
            fts27.Id = 27;
            f4.FormsTeachersToSubjects.Add(fts27);
            ts12.TeacherSubjectForms.Add(fts27);
            formsToTeacherSubjects.Add(fts27);

            FormToTeacherSubject fts28 = CreateFTS(f4, ts14, new DateTime(2018, 9, 1), null);
            fts28.Id = 28;
            f4.FormsTeachersToSubjects.Add(fts28);
            ts14.TeacherSubjectForms.Add(fts28);
            formsToTeacherSubjects.Add(fts28);

            FormToTeacherSubject fts29 = CreateFTS(f4, ts16, new DateTime(2018, 9, 1), null);
            fts29.Id = 29; 
            f4.FormsTeachersToSubjects.Add(fts29);
            ts16.TeacherSubjectForms.Add(fts29);
            formsToTeacherSubjects.Add(fts29);

            FormToTeacherSubject fts30 = CreateFTS(f4, ts18, new DateTime(2018, 9, 1), null);
            fts30.Id = 30;
            f4.FormsTeachersToSubjects.Add(fts30);
            ts18.TeacherSubjectForms.Add(fts30);
            formsToTeacherSubjects.Add(fts30);

            FormToTeacherSubject fts31 = CreateFTS(f4, ts20, new DateTime(2018, 9, 1), null);
            fts31.Id = 31;
            f4.FormsTeachersToSubjects.Add(fts31);
            ts20.TeacherSubjectForms.Add(fts31);
            formsToTeacherSubjects.Add(fts31);

            FormToTeacherSubject fts32 = CreateFTS(f5, ts3, new DateTime(2018, 9, 1), null);
            fts32.Id = 32;           
            f5.FormsTeachersToSubjects.Add(fts32);
            ts3.TeacherSubjectForms.Add(fts32);
            formsToTeacherSubjects.Add(fts32);

            FormToTeacherSubject fts33 = CreateFTS(f5, ts5, new DateTime(2018, 9, 1), null);
            fts33.Id = 33;           
            f5.FormsTeachersToSubjects.Add(fts33);
            ts5.TeacherSubjectForms.Add(fts33);
            formsToTeacherSubjects.Add(fts33);

            FormToTeacherSubject fts34 = CreateFTS(f5, ts7, new DateTime(2018, 9, 1), null);
            fts34.Id = 34;           
            f5.FormsTeachersToSubjects.Add(fts34);
            ts7.TeacherSubjectForms.Add(fts34);
            formsToTeacherSubjects.Add(fts34); 

            FormToTeacherSubject fts35 = CreateFTS(f5, ts9, new DateTime(2018, 9, 1), null);
            fts35.Id = 35;            
            f5.FormsTeachersToSubjects.Add(fts35);
            ts9.TeacherSubjectForms.Add(fts35);
            formsToTeacherSubjects.Add(fts35); 

            FormToTeacherSubject fts36 = CreateFTS(f5, ts11, new DateTime(2018, 9, 1), null);
            fts36.Id = 36;           
            f5.FormsTeachersToSubjects.Add(fts36);
            ts11.TeacherSubjectForms.Add(fts36);
            formsToTeacherSubjects.Add(fts36);

            FormToTeacherSubject fts37 = CreateFTS(f5, ts13, new DateTime(2018, 9, 1), null);
            fts37.Id = 37;
            f5.FormsTeachersToSubjects.Add(fts37);
            ts13.TeacherSubjectForms.Add(fts37);
            formsToTeacherSubjects.Add(fts37);

            FormToTeacherSubject fts38 = CreateFTS(f5, ts15, new DateTime(2018, 9, 1), null);
            fts38.Id = 38;
            f5.FormsTeachersToSubjects.Add(fts38);
            ts15.TeacherSubjectForms.Add(fts38);
            formsToTeacherSubjects.Add(fts38);

            FormToTeacherSubject fts39 = CreateFTS(f5, ts17, new DateTime(2018, 9, 1), null);
            fts39.Id = 39;
            f5.FormsTeachersToSubjects.Add(fts39);
            ts17.TeacherSubjectForms.Add(fts39);
            formsToTeacherSubjects.Add(fts39);

            FormToTeacherSubject fts40 = CreateFTS(f5, ts19, new DateTime(2018, 9, 1), null);
            fts40.Id = 40;
            f5.FormsTeachersToSubjects.Add(fts40);
            ts19.TeacherSubjectForms.Add(fts40);
            formsToTeacherSubjects.Add(fts40);

            FormToTeacherSubject fts41 = CreateFTS(f5, ts21, new DateTime(2018, 9, 1), null);
            fts41.Id = 41;
            f5.FormsTeachersToSubjects.Add(fts41);
            ts21.TeacherSubjectForms.Add(fts41);
            formsToTeacherSubjects.Add(fts41);
       
            FormToTeacherSubject fts42 = CreateFTS(f5, ts22, new DateTime(2018, 9, 1), null);
            fts42.Id = 42;
            f5.FormsTeachersToSubjects.Add(fts42);
            ts22.TeacherSubjectForms.Add(fts42);
            formsToTeacherSubjects.Add(fts42);
   
            FormToTeacherSubject fts43 = CreateFTS(f6, ts3, new DateTime(2018, 9, 1), null);
            fts43.Id = 43;
            f6.FormsTeachersToSubjects.Add(fts43);
            ts3.TeacherSubjectForms.Add(fts43);
            formsToTeacherSubjects.Add(fts43);

            FormToTeacherSubject fts44 = CreateFTS(f6, ts5, new DateTime(2018, 9, 1), null);
            fts44.Id = 44;
            f6.FormsTeachersToSubjects.Add(fts44);
            ts5.TeacherSubjectForms.Add(fts44);
            formsToTeacherSubjects.Add(fts44);

            FormToTeacherSubject fts45 = CreateFTS(f6, ts7, new DateTime(2018, 9, 1), null);
            fts45.Id = 45;
            f6.FormsTeachersToSubjects.Add(fts45);
            ts7.TeacherSubjectForms.Add(fts45);
            formsToTeacherSubjects.Add(fts45);

            FormToTeacherSubject fts46 = CreateFTS(f6, ts9, new DateTime(2018, 9, 1), null);
            fts46.Id = 46;
            f6.FormsTeachersToSubjects.Add(fts46);
            ts9.TeacherSubjectForms.Add(fts46);
            formsToTeacherSubjects.Add(fts46);

            FormToTeacherSubject fts47 = CreateFTS(f6, ts11, new DateTime(2018, 9, 1), null);
            fts47.Id = 47;
            f6.FormsTeachersToSubjects.Add(fts47);
            ts11.TeacherSubjectForms.Add(fts47);
            formsToTeacherSubjects.Add(fts47);

            FormToTeacherSubject fts48 = CreateFTS(f6, ts13, new DateTime(2018, 9, 1), null);
            fts48.Id = 48;
            f6.FormsTeachersToSubjects.Add(fts48);
            ts13.TeacherSubjectForms.Add(fts48);
            formsToTeacherSubjects.Add(fts48);

            FormToTeacherSubject fts49 = CreateFTS(f6, ts15, new DateTime(2018, 9, 1), null);
            fts49.Id = 49;
            f6.FormsTeachersToSubjects.Add(fts49);
            ts15.TeacherSubjectForms.Add(fts49);
            formsToTeacherSubjects.Add(fts49);

            FormToTeacherSubject fts50 = CreateFTS(f6, ts17, new DateTime(2018, 9, 1), null);
            fts50.Id = 50;
            f6.FormsTeachersToSubjects.Add(fts50);
            ts17.TeacherSubjectForms.Add(fts50);
            formsToTeacherSubjects.Add(fts50); 

            FormToTeacherSubject fts51 = CreateFTS(f6, ts19, new DateTime(2018, 9, 1), null);
            fts51.Id = 51;
            f6.FormsTeachersToSubjects.Add(fts51);
            ts19.TeacherSubjectForms.Add(fts51);
            formsToTeacherSubjects.Add(fts51);

            FormToTeacherSubject fts52 = CreateFTS(f6, ts21, new DateTime(2018, 9, 1), null);
            fts52.Id = 52;
            f6.FormsTeachersToSubjects.Add(fts52);
            ts21.TeacherSubjectForms.Add(fts52);
            formsToTeacherSubjects.Add(fts52);

            FormToTeacherSubject fts53 = CreateFTS(f6, ts22, new DateTime(2018, 9, 1), null);
            fts53.Id = 53;
            f6.FormsTeachersToSubjects.Add(fts53);
            ts22.TeacherSubjectForms.Add(fts53);
            formsToTeacherSubjects.Add(fts53);

            FormToTeacherSubject fts54 = CreateFTS(f8, ts22, new DateTime(2017, 9, 1), new DateTime(2017, 6, 30));
            fts54.Id = 54;
            f8.FormsTeachersToSubjects.Add(fts54); 
            ts22.TeacherSubjectForms.Add(fts54);
            formsToTeacherSubjects.Add(fts54);

            context.FormsToTeacherSubjects.AddRange(formsToTeacherSubjects);

            //kreiranje ocena
            Mark CreateMark(int mv, Semesters sem, DateTime created, FormToTeacherSubject fts, Student s)
            {
                Mark m = new Mark
                {
                    MarkValue = mv,
                    Semester = sem,
                    Created = created,
                    FormToTeacherSubject = fts,
                    Student = s
                };
                return m;
            }

            Mark m1 = CreateMark(5, Semesters.FIRST_SEMESTER, new DateTime(2018, 9, 1), fts2, s2);
            m1.Id = 1;
           
            s2.Marks.Add(m1);
            fts2.Marks.Add(m1);
            marks.Add(m1);

            Mark m2 = CreateMark(5, Semesters.FIRST_SEMESTER, new DateTime(2018, 9, 15), fts2, s2);
            m2.Id = 2;
            
            s2.Marks.Add(m2);
            fts2.Marks.Add(m2);
            marks.Add(m2);

            Mark m3 = CreateMark(5, Semesters.FIRST_SEMESTER, new DateTime(2018, 9, 30), fts2, s2);
            m2.Id = 3;
            
            s2.Marks.Add(m3);
            fts2.Marks.Add(m3);
            marks.Add(m3);

            Mark m4 = CreateMark(5, Semesters.FIRST_SEMESTER, new DateTime(2018, 9, 30), fts2, s2);
            m4.Id = 4;
           
            s2.Marks.Add(m4);
            fts2.Marks.Add(m4);
            marks.Add(m4);

            Mark m5 = CreateMark(5, Semesters.FIRST_SEMESTER, new DateTime(2018, 9, 30), fts2, s2);
            m5.Id = 5;         
            s2.Marks.Add(m5);
            fts2.Marks.Add(m5);
            marks.Add(m5);

           
            for (int i = 1; i <= 3; i++)
            { 
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts3, 
                    Student = s2
                };
                s2.Marks.Add(m);
                fts3.Marks.Add(m);
                marks.Add(m); 
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts4,
                    Student = s2
                };
                s2.Marks.Add(m);
                fts4.Marks.Add(m);
                marks.Add(m); 
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts5,
                    Student = s2
                };
                s2.Marks.Add(m);
                fts5.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts6,
                    Student = s2
                };
                s2.Marks.Add(m);
                fts6.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts7,
                    Student = s2
                };
                s2.Marks.Add(m);
                fts7.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts8,
                    Student = s2
                };
                s2.Marks.Add(m);
                fts8.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts9,
                    Student = s2
                };
                s2.Marks.Add(m);
                fts9.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts10,
                    Student = s2
                };
                s2.Marks.Add(m);
                fts10.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts11,
                    Student = s2
                };
                s2.Marks.Add(m);
                fts11.Marks.Add(m);
                marks.Add(m);
            }

            //ocene s3 
            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts2,
                    Student = s3
                };
                s3.Marks.Add(m);
                fts2.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts3,
                    Student = s3
                };
                s3.Marks.Add(m);
                fts3.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts4,
                    Student = s3
                };
                s3.Marks.Add(m);
                fts4.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts5,
                    Student = s3
                };
                s3.Marks.Add(m);
                fts5.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 3,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts6,
                    Student = s3
                };
                s3.Marks.Add(m);
                fts6.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts7,
                    Student = s3
                };
                s3.Marks.Add(m);
                fts7.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts8,
                    Student = s3
                };
                s3.Marks.Add(m);
                fts8.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts9,
                    Student = s3
                };
                s3.Marks.Add(m);
                fts9.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts10,
                    Student = s3
                };
                s3.Marks.Add(m);
                fts10.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts11,
                    Student = s3
                };
                s3.Marks.Add(m);
                fts11.Marks.Add(m);
                marks.Add(m);
            }

            //ocene s4 
            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts2,
                    Student = s4
                };
                s4.Marks.Add(m);
                fts2.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts3,
                    Student = s4
                };
                s4.Marks.Add(m);
                fts3.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 3,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts4,
                    Student = s4
                };
                s4.Marks.Add(m);
                fts4.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts5,
                    Student = s4
                };
                s4.Marks.Add(m);
                fts5.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts6,
                    Student = s4
                };
                s4.Marks.Add(m);
                fts6.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts7,
                    Student = s4
                };
                s4.Marks.Add(m);
                fts7.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts8,
                    Student = s4
                };
                s4.Marks.Add(m);
                fts8.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts9,
                    Student = s4
                };
                s4.Marks.Add(m);
                fts9.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts10,
                    Student = s4
                };
                s4.Marks.Add(m);
                fts10.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts11,
                    Student = s4
                };
                s4.Marks.Add(m);
                fts11.Marks.Add(m);
                marks.Add(m);
            }

            //ocene s5 
            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts2,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts2.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts3,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts3.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts4,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts4.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts5,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts5.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts6,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts6.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts7,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts7.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts8,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts8.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts9,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts9.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts10,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts10.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts11,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts11.Marks.Add(m);
                marks.Add(m);
            }

            //ocene s7
            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts2,
                    Student = s7
                };
                s7.Marks.Add(m);
                fts2.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts3,
                    Student = s7
                };
                s7.Marks.Add(m);
                fts3.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts4,
                    Student = s7
                };
                s7.Marks.Add(m);
                fts4.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts5,
                    Student = s7
                };
                s7.Marks.Add(m);
                fts5.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts6,
                    Student = s7
                };
                s7.Marks.Add(m);
                fts6.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts7,
                    Student = s7
                };
                s7.Marks.Add(m);
                fts7.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts8,
                    Student = s7
                };
                s7.Marks.Add(m);
                fts8.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts9,
                    Student = s7
                };
                s7.Marks.Add(m);
                fts9.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts10,
                    Student = s7
                };
                s7.Marks.Add(m);
                fts10.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts11,
                    Student = s7
                };
                s7.Marks.Add(m);
                fts11.Marks.Add(m);
                marks.Add(m);
            }

            //ocene s9 f3
            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts12,
                    Student = s9
                };
                s9.Marks.Add(m);
                fts12.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts13,
                    Student = s9
                };
                s9.Marks.Add(m);
                fts13.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts14,
                    Student = s9
                };
                s9.Marks.Add(m);
                fts14.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts15,
                    Student = s9
                };
                s9.Marks.Add(m);
                fts15.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 3,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts16,
                    Student = s9
                };
                s9.Marks.Add(m);
                fts16.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts17,
                    Student = s9
                };
                s9.Marks.Add(m);
                fts17.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts18,
                    Student = s9
                };
                s9.Marks.Add(m);
                fts18.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts19,
                    Student = s9
                };
                s9.Marks.Add(m);
                fts19.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts20,
                    Student = s9
                };
                s9.Marks.Add(m);
                fts20.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts21,
                    Student = s9
                };
                s9.Marks.Add(m);
                fts21.Marks.Add(m);
                marks.Add(m);
            }

            //ocene s13 f4
            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts22,
                    Student = s13
                };
                s13.Marks.Add(m);
                fts22.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts23,
                    Student = s13
                };
                s13.Marks.Add(m);
                fts23.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts24,
                    Student = s13
                };
                s13.Marks.Add(m);
                fts24.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts25,
                    Student = s13
                };
                s13.Marks.Add(m);
                fts25.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 3,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts26,
                    Student = s13
                };
                s13.Marks.Add(m);
                fts26.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts27,
                    Student = s13
                };
                s13.Marks.Add(m);
                fts27.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts28,
                    Student = s13
                };
                s13.Marks.Add(m);
                fts28.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts29,
                    Student = s13
                };
                s13.Marks.Add(m);
                fts29.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts30,
                    Student = s13
                };
                s13.Marks.Add(m);
                fts30.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.FIRST_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts31,
                    Student = s13
                };
                s13.Marks.Add(m);
                fts31.Marks.Add(m);
                marks.Add(m); 
            }

            //ocene s2 drugi semestr
            for (int i = 1; i <= 2; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts2,
                    Student = s2
                };
                s2.Marks.Add(m);
                fts2.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 2; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts3,
                    Student = s2
                };
                s2.Marks.Add(m);
                fts3.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 2; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts4,
                    Student = s2
                };
                s2.Marks.Add(m);
                fts4.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 2; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts5,
                    Student = s2
                };
                s2.Marks.Add(m);
                fts5.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 2; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts6,
                    Student = s2
                };
                s2.Marks.Add(m);
                fts6.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 2; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts7,
                    Student = s2
                };
                s2.Marks.Add(m);
                fts7.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts8,
                    Student = s2
                };
                s2.Marks.Add(m);
                fts8.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 1; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts9,
                    Student = s2
                };
                s2.Marks.Add(m);
                fts9.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 2; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts10,
                    Student = s2
                };
                s2.Marks.Add(m);
                fts10.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 1; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts11,
                    Student = s2
                };
                s2.Marks.Add(m); 
                fts11.Marks.Add(m);
                marks.Add(m);
            }

            //ocene student5 2.semestar
            for (int i = 1; i <= 2; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts2,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts2.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 2; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts3,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts3.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 2; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts4,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts4.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 2; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts5,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts5.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 2; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts6,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts6.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 2; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts7,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts7.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 3; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts8,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts8.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 1; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts9,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts9.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 2; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 5,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts10,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts10.Marks.Add(m);
                marks.Add(m);
            }

            for (int i = 1; i <= 1; i++)
            {
                Mark m = new Mark
                {
                    MarkValue = 4,
                    Semester = Semesters.SECOND_SEMESTER,
                    Created = DateTime.UtcNow,
                    FormToTeacherSubject = fts11,
                    Student = s5
                };
                s5.Marks.Add(m);
                fts11.Marks.Add(m);
                marks.Add(m);
            }

            context.Marks.AddRange(marks); 
         
            context.SaveChanges();

        }

    }
}