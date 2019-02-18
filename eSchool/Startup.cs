using eSchool.Infrastructure;
using eSchool.Models;
using eSchool.Providers;
using eSchool.Repositories;
using eSchool.Services;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using Unity;
using Unity.Lifetime;
using Unity.WebApi;

[assembly: OwinStartup(typeof(eSchool.Startup))]
namespace eSchool
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = SetupUnity();
            ConfigureOAuth(app, container);

            HttpConfiguration config = new HttpConfiguration();
            config.DependencyResolver = new UnityDependencyResolver(container);
            config.Formatters.JsonFormatter.SerializerSettings.DateFormatString = "dd.MM.yyyy";

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            WebApiConfig.Register(config);
            app.UseWebApi(config);

        }

        public void ConfigureOAuth(IAppBuilder app, UnityContainer container)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/project/login"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new SimpleAuthorizationServerProvider(container)
            };

            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }

        private UnityContainer SetupUnity()
        {
            var container = new UnityContainer();

            container.RegisterType<DbContext, AuthContext>(new HierarchicalLifetimeManager());
            container.RegisterType<IAuthRepository, AuthRepository>();
            container.RegisterType<IUnitOfWork, UnitOfWork>();

            container.RegisterType<IGenericRepository<ApplicationUser>, GenericRepository<ApplicationUser>>();
            container.RegisterType<IGenericRepository<Admin>, GenericRepository<Admin>>();

            //container.RegisterType<IGenericRepository<Student>, GenericRepository<Student>>();
            container.RegisterType<IStudentsRepository, StudentsRepository>();

            //container.RegisterType<IGenericRepository<Parent>, GenericRepository<Parent>>();
            container.RegisterType<IParentsRepository, ParentsRepository>(); 

            //container.RegisterType<IGenericRepository<Teacher>, GenericRepository<Teacher>>();
            container.RegisterType<ITeachersRepository, TeachersRepository>();

            //container.RegisterType<IGenericRepository<Form>, GenericRepository<Form>>();
            container.RegisterType<IFormsRepository, FormsRepository>();

            //container.RegisterType<IGenericRepository<Subject>, GenericRepository<Subject>>();
            container.RegisterType<ISubjectsRepository, SubjectsRepository>();

            //container.RegisterType<IGenericRepository<TeacherToSubject>, GenericRepository<TeacherToSubject>>();
            container.RegisterType<ITeachersToSubjectsRepository, TeachersToSubjectsRepository>(); 

            //container.RegisterType<IGenericRepository<FormToTeacherSubject>, GenericRepository<FormToTeacherSubject>>();
            container.RegisterType<IFormsToTeacherSubjectsRepository, FormsToTeacherSubjectsRepository>();

            //container.RegisterType<IGenericRepository<Mark>, GenericRepository<Mark>>();
            container.RegisterType<IMarksRepository, MarksRepository>(); 


            //Services
            //container.RegisterType<IRolesService, RolesService>();
            container.RegisterType<IUsersService, UsersService>();

            container.RegisterType<IAdminsService, AdminsService>();
            container.RegisterType<IStudentsService, StudentsService>();
            container.RegisterType<IParentsService, ParentsService>();
            container.RegisterType<ITeachersService, TeachersService>();

            container.RegisterType<IFormsService, FormsService>();
            container.RegisterType<ISubjectsService, SubjectsService>();

            container.RegisterType<ITeachersToSubjectsService, TeachersToSubjectsService>();
            container.RegisterType<IFormsToTeacherSubjectsService, FormsToTeacherSubjectsService>();

            container.RegisterType<IMarksService, MarksService>();

            container.RegisterType<IEmailsService, EmailsService>(); 


            return container;
        }

    }

}
