using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;
using System.Web.Http.Controllers;
using Newtonsoft.Json;
using EducosoftAPI.Models.Course;
using EducosoftAPI.Models.User;
using EducosoftAPI.Models.Announcement;
using EducosoftAPI.Models.Assessments;
using EducosoftAPI.Models.InternalMail;

/// <summary>
/// Summary description for UserCredentialModelBinder
/// </summary>
public class UserCredentialModelBinder : IModelBinder
{
    public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
    {
        try
        {
            bool flag = false;
            //if (bindingContext.ModelType != typeof(UserCredential))
            //{
            //    flag = false;
            //}

            //For User
            #region
            if (bindingContext.ModelType == typeof(UserCredential) || bindingContext.ModelType == typeof(UserCredentialSurveyFeedback)
                || bindingContext.ModelType == typeof(UserVerifyLoginCredential) || bindingContext.ModelType == typeof(UserViewCredential)
                || bindingContext.ModelType == typeof(UserProfileEditCredential) || bindingContext.ModelType == typeof(UserForgetPasswordCredential)
                || bindingContext.ModelType == typeof(UserResetPasswordCredential) || bindingContext.ModelType == typeof(UserSupportCredential))
            {
                if (bindingContext.ModelType == typeof(UserCredential))
                {
                    UserCredential result = JsonConvert.DeserializeObject<UserCredential>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }
                else if (bindingContext.ModelType == typeof(UserCredentialSurveyFeedback))
                {
                    UserCredentialSurveyFeedback result = JsonConvert.DeserializeObject<UserCredentialSurveyFeedback>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }
                else if (bindingContext.ModelType == typeof(UserVerifyLoginCredential))
                {
                    UserVerifyLoginCredential result = JsonConvert.DeserializeObject<UserVerifyLoginCredential>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }
                else if (bindingContext.ModelType == typeof(UserViewCredential))
                {
                    UserViewCredential result = JsonConvert.DeserializeObject<UserViewCredential>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }
                else if (bindingContext.ModelType == typeof(UserProfileEditCredential))
                {
                    UserProfileEditCredential result = JsonConvert.DeserializeObject<UserProfileEditCredential>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }
                else if (bindingContext.ModelType == typeof(UserForgetPasswordCredential))
                {
                    UserForgetPasswordCredential result = JsonConvert.DeserializeObject<UserForgetPasswordCredential>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }
                else if (bindingContext.ModelType == typeof(UserResetPasswordCredential))
                {
                    UserResetPasswordCredential result = JsonConvert.DeserializeObject<UserResetPasswordCredential>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }
                else if (bindingContext.ModelType == typeof(UserSupportCredential))
                {
                    UserSupportCredential result = JsonConvert.DeserializeObject<UserSupportCredential>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }

            }
            #endregion

            //For Course
            #region
            else if (bindingContext.ModelType == typeof(CourseCredential) || bindingContext.ModelType == typeof(CourseLevelCredential) || bindingContext.ModelType == typeof(CRLOTimeSpentCredential))
            {
                if (bindingContext.ModelType == typeof(CourseCredential))
                {
                    CourseCredential result = JsonConvert.DeserializeObject<CourseCredential>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }
                else if (bindingContext.ModelType == typeof(CourseLevelCredential))
                {
                    CourseLevelCredential result = JsonConvert.DeserializeObject<CourseLevelCredential>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }
                else if (bindingContext.ModelType == typeof(CRLOTimeSpentCredential))
                {
                    CRLOTimeSpentCredential result = JsonConvert.DeserializeObject<CRLOTimeSpentCredential>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }

            }
            #endregion
            #region
            //For Announcements
            else if (bindingContext.ModelType == typeof(AnnouncementCredential))
            {
                if (bindingContext.ModelType == typeof(AnnouncementCredential))
                {
                    AnnouncementCredential result = JsonConvert.DeserializeObject<AnnouncementCredential>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }
            }
            #endregion
            #region
            //For Assessments
            else if (bindingContext.ModelType == typeof(AssessmentCredential) || bindingContext.ModelType == typeof(InitTestPaperCredential))
            {
                if (bindingContext.ModelType == typeof(AssessmentCredential))
                {
                    AssessmentCredential result = JsonConvert.DeserializeObject<AssessmentCredential>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }
                else if (bindingContext.ModelType == typeof(InitTestPaperCredential))
                {
                    InitTestPaperCredential result = JsonConvert.DeserializeObject<InitTestPaperCredential>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }
            }
            #endregion
            #region
            //For Internal Mail
            else if (bindingContext.ModelType == typeof(InternalMailCredential) || bindingContext.ModelType == typeof(InternalMailViewCredential) || bindingContext.ModelType == typeof(DeleteInternalMailCredential)|| bindingContext.ModelType == typeof(SendInternalMailCredential))
            {
                if (bindingContext.ModelType == typeof(InternalMailCredential))
                {
                    InternalMailCredential result = JsonConvert.DeserializeObject<InternalMailCredential>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }
                else if (bindingContext.ModelType == typeof(InternalMailViewCredential))
                {
                    InternalMailViewCredential result = JsonConvert.DeserializeObject<InternalMailViewCredential>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }
                else if (bindingContext.ModelType == typeof(DeleteInternalMailCredential))
                {
                    DeleteInternalMailCredential result = JsonConvert.DeserializeObject<DeleteInternalMailCredential>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }
                else if (bindingContext.ModelType == typeof(SendInternalMailCredential))
                {
                    SendInternalMailCredential result = JsonConvert.DeserializeObject<SendInternalMailCredential>
                       (actionContext.Request.Content.ReadAsStringAsync().Result);
                    bindingContext.Model = result;
                    flag = true;
                }
            }

            #endregion

            //For Else region
            #region
            else
            {
                flag = false;
            }
            #endregion

            return flag;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}