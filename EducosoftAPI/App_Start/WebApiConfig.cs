using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding.Binders;
using System.Web.Http.ModelBinding;
using System.Web.Http.Filters;
using EducosoftAPI.Models.User;
using EducosoftAPI.Models.Course;
using EducosoftAPI.Models.Announcement;
using EducosoftAPI.Models.Assessments;
using EducosoftAPI.Models.InternalMail;

namespace EducosoftAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "api/{controller}/{action}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );


            ////For User
            #region
            //Model Binder Configuration for UserCredential
            var provider = new SimpleModelBinderProvider(
            typeof(UserCredential), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, provider);

            //Model Binder Configuration for UserCredentialSurveyFeedback
            var providerForgetPassword = new SimpleModelBinderProvider(
            typeof(UserCredentialSurveyFeedback), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, providerForgetPassword);

            //Model Binder Configuration for UserVerifyLoginCredential
            var providerVerifyLogin = new SimpleModelBinderProvider(
            typeof(UserVerifyLoginCredential), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, providerVerifyLogin);

            //Model Binder Configuration for UserViewCredential
            var providerUserViewCredential = new SimpleModelBinderProvider(
            typeof(UserViewCredential), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, providerUserViewCredential);

            //Model Binder Configuration for UserProfileEditCredential
            var providerProfileEditCredential = new SimpleModelBinderProvider(
            typeof(UserProfileEditCredential), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, providerProfileEditCredential);

            //Model Binder Configuration for UserForgetPasswordCredential
            var providerUserForgetPasswordCredential = new SimpleModelBinderProvider(
            typeof(UserForgetPasswordCredential), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, providerUserForgetPasswordCredential);

            //Model Binder Configuration for UserResetPasswordCredential
            var providerUserResetPasswordCredential = new SimpleModelBinderProvider(
            typeof(UserResetPasswordCredential), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, providerUserResetPasswordCredential);

            //Model Binder Configuration for UserSupportCredential
            var providerUserSupportCredential = new SimpleModelBinderProvider(
            typeof(UserSupportCredential), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, providerUserSupportCredential);

            #endregion

            ////For Course
            #region
            //Model Binder Configuration for CourseCredential
            var providerUserCourse = new SimpleModelBinderProvider(
            typeof(CourseCredential), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, providerUserCourse);

            //Model Binder Configuration for CourseLevelCredential
            var providerCourseLevel = new SimpleModelBinderProvider(
            typeof(CourseLevelCredential), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, providerCourseLevel);

            //Model Binder Configuration for CRLOTimeSpentCredential
            var providerCRLOTimeSpend = new SimpleModelBinderProvider(
            typeof(CRLOTimeSpentCredential), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, providerCRLOTimeSpend);
            
            #endregion

            //For Announcements
            #region
            //Model Binder Configuration for AnnouncementCredential
            var providerAnnouncement = new SimpleModelBinderProvider(
            typeof(AnnouncementCredential), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, providerAnnouncement);
            #endregion

            //For Assessments

            #region
            //Model Binder Configuration for AssessmentCredential
            var providerAssessment = new SimpleModelBinderProvider(
            typeof(AssessmentCredential), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, providerAssessment);

            //Model Binder Configuration for AssessmentCredential
            var providerAssPreReq = new SimpleModelBinderProvider(
            typeof(InitTestPaperCredential), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, providerAssPreReq);

            #endregion

            //For Internal Mail
            #region
            //Model Binder Configuration for InternalMailViewCredential
            var providerInternalMail = new SimpleModelBinderProvider(
            typeof(InternalMailCredential), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, providerInternalMail);

            //Model Binder Configuration for InternalMailViewCredential  
            var providerInternalMailView = new SimpleModelBinderProvider(
            typeof(InternalMailViewCredential), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, providerInternalMailView);

            //Model Binder Configuration for DeleteInternalMailCredential  
            var providerDeleteInternalMail = new SimpleModelBinderProvider(
            typeof(DeleteInternalMailCredential), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, providerDeleteInternalMail);

            //Model Binder Configuration for SendInternalMail  
            var providerSendInternalMail = new SimpleModelBinderProvider(
            typeof(SendInternalMailCredential), new UserCredentialModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, providerSendInternalMail);
            #endregion
        }
    }
}
