using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace EducosoftAPI.Areas.HelpPage
{
    /// <summary>
    /// Use this class to customize the Help Page.
    /// For example you can set a custom <see cref="System.Web.Http.Description.IDocumentationProvider"/> to supply the documentation
    /// or you can provide the samples for the requests/responses.
    /// </summary>
    public static class HelpPageConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //// Uncomment the following to use the documentation from XML documentation file.
            config.SetDocumentationProvider(new XmlDocumentationProvider(HttpContext.Current.Server.MapPath("~/App_Data/XmlDocument.xml")));

            //// Uncomment the following to use "sample string" as the sample for all actions that have string as the body parameter or return type.
            //// Also, the string arrays will be used for IEnumerable<string>. The sample objects will be serialized into different media type 
            //// formats by the available formatters.
            //config.SetSampleObjects(new Dictionary<Type, object>
            //{
            //    {typeof(string), "sample string"},
            //    {typeof(IEnumerable<string>), new string[]{"sample 1", "sample 2"}}
            //});

            //// Uncomment the following to use "[0]=foo&[1]=bar" directly as the sample for all actions that support form URL encoded format
            //// and have IEnumerable<string> as the body parameter or return type.
            //config.SetSampleForType("[0]=foo&[1]=bar", new MediaTypeHeaderValue("application/x-www-form-urlencoded"), typeof(IEnumerable<string>));

            //// Uncomment the following to use "1234" directly as the request sample for media type "text/plain" on the controller named "Values"
            //// and action named "Put".
            //config.SetSampleRequest("1234", new MediaTypeHeaderValue("text/plain"), "Values", "Put");

            //// Uncomment the following to use the image on "../images/aspNetHome.png" directly as the response sample for media type "image/png"
            //// on the controller named "Values" and action named "Get" with parameter "id".
            //config.SetSampleResponse(new ImageSample("../images/aspNetHome.png"), new MediaTypeHeaderValue("image/png"), "Values", "Get", "id");

            //// Uncomment the following to correct the sample request when the action expects an HttpRequestMessage with ObjectContent<string>.
            //// The sample will be generated as if the controller named "Values" and action named "Get" were having string as the body parameter.
            //config.SetActualRequestType(typeof(string), "Values", "Get");

            //// Uncomment the following to correct the sample response when the action returns an HttpResponseMessage with ObjectContent<string>.
            //// The sample will be generated as if the controller named "Values" and action named "Post" were returning a string.
            //config.SetActualResponseType(typeof(string), "Values", "Post");

            //For UserSurveyFeedback
            #region
            string sampleRequestBodySaveSurveyUser = "{\"Name\":\"Name of user\",\"Email\":\"emailid of user\",\"MobileNo\":\"Mobile no of user\"}";
            string sampleResponseBodySaveSurveyUser = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"User successfully registered/updated\",\n\"response\": {\n \"SurveyUserId\":\"18\"\n  }\n} \n\nResponse Type: Json  Error \n\n{\n \"status\": 0,\n \"message\": \"Error message regarding the transaction failure\"\n}";
            config.SetSampleRequest(sampleRequestBodySaveSurveyUser, new MediaTypeHeaderValue("application/json"), "SurveyUser", "SaveSurveyUser");
            config.SetSampleResponse(sampleResponseBodySaveSurveyUser, new MediaTypeHeaderValue("application/json"), "SurveyUser", "SaveSurveyUser");

            string sampleRequestBodySaveSurveyFeedback = "{\"Email\":\"emailid of user\",\"Feedback\":\"Feedback-string\"}";
            sampleRequestBodySaveSurveyFeedback += "\n\n Note: Format of 'Feedback-string' :" +
                                                   "\n Ques1IdÅOptIdÅCommentÇQues2IdÅOptIdÅCommentÇQues3IdÅOptIdÅCommentÇQues4IdÅOptIdÅCommentÇQues5IdÅOptIdÅCommentÇQues6IdÅOptIdÅComment"+
                                                   "\n\n Ç -> will be used as Question separator [CHAR(199)]" +
                                                   "\n Å -> will be used as Question option and comment separator [CHAR(198)]" +
                                                   "\n All the six QuesId are required in the feedback string." +
                                                   "\n Max size of OptId is 5 char and Max size of Comment is 500 character";
            string sampleResponseBodySaveSurveyFeedback = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"User feedback saved/updated successfully\",\n\"response\": {\n \"SurveyFeedbackId\":\"13\", \"SurveyUserId\":\"18\"\n  }\n} \n\nResponse Type: Json  Error \n\n{\n \"status\": 0,\n \"message\": \"Error message regarding the transaction failure\"\n}";
            config.SetSampleRequest(sampleRequestBodySaveSurveyFeedback, new MediaTypeHeaderValue("application/json"), "SurveyUser", "SaveSurveyFeedback");
            config.SetSampleResponse(sampleResponseBodySaveSurveyFeedback, new MediaTypeHeaderValue("application/json"), "SurveyUser", "SaveSurveyFeedback");
            #endregion

            //User
            #region
            string sampleRequestBodyVerifyLogin = "{\"Email\":\"Login email id\",\"Password\":\"Password of user\"} \n";
            string sampleResponseBodyVerifyLogin = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"User is successfully verified\",\n\"country\":\"US\",\"response\":{\n \"User_FirstName\":\"Aapi\", \"User_LastName\":\"TestUser\", \"User_Email\":\"aapitestuser@gmail.com\", \"UserId\":\"267022\"\n  }\n} \n\nResponse Type: Json  Error \n\n{\n \"status\": 0,\n \"message\": \"User does not exist / Error message regarding the transaction failure\"\n}";
            config.SetSampleRequest(sampleRequestBodyVerifyLogin, new MediaTypeHeaderValue("application/json"), "User", "VerifyLogin");
            config.SetSampleResponse(sampleResponseBodyVerifyLogin, new MediaTypeHeaderValue("application/json"), "User", "VerifyLogin");

            string sampleRequestBodyViewUser = "{\"UserId\":\"267023\"} \n";
            string sampleResponseBodyViewUser = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"User is successfully verified\",\n\"response\":{\n\"address\":\"\", \"address2\":\"\", \"cityName\":\"\", \"countryId\":\"61\", \"countryName\":\"USA\", \"emailId\":\"a_testapi01@gmail.com\", \"firstName\":\"A\", \"imageName\":\"Koala.jpg\", \"imagePath\":\"http://localhost//Educo-dev/EducoContentFiles/UserImages/267023/\", \"language\":\"English\", \"lastName\":\"TestApi01\", \"mobile\":\"\", \"phone\":\"\", \"salutation\":\"MR\", \"stateId\":\"136\", \"stateName\":\"Alabama\", \"studentId\":\"\", \"timeZone\":\"(UTC-05:00) Eastern Time (US & Canada)\", \"timeZoneId\":\"35\", \"zipcode\":\"\"  \n  }\n} \n\nResponse Type: Json  Error \n\n{\n \"status\": 0,\n \"message\": \"User does not exist / Error message regarding the transaction failure\"\n}";
            config.SetSampleRequest(sampleRequestBodyViewUser, new MediaTypeHeaderValue("application/json"), "User", "ViewUser");
            config.SetSampleResponse(sampleResponseBodyViewUser, new MediaTypeHeaderValue("application/json"), "User", "ViewUser");

            string sampleRequestBodyEditUser = "{\"UserId\":\"257560\",[at least one Optional Field is needed to EditUser]... }";
            sampleRequestBodyEditUser += "\n\n Notes";
            sampleRequestBodyEditUser += "\n\n 1) Optional Fields are {\"Salutation\":\"Mr\",\"FirstName\":\"A1\",\"LastName\":\"104\",\"CountryId\":\"2\",\"StateId\":\"5\", \"Address1\":\"addr1\",\"Address2\":\"addr2\",\"City\":\"Georgia\",\"Phone\":\"\", \"Mobile\":\"\",\"strImage\":\"\",\"strImageType\":\"\",\"ZipCode\":\"\" }";
            sampleRequestBodyEditUser += "\n\n 2) dropdown values of 'Salutation' :Mr.,Mrs.,Ms.,Dr.,Prof.";
            sampleRequestBodyEditUser += "\n\n 3) 'strImage' is in Base64String format and 'strImageType' is the extension of image i.e, jpg, png etc";
            string sampleResponseBodyEditUser = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"User has updated successfully\", \n  }\n} \n\nResponse Type: Json  Error \n\n{\n \"status\": 0,\n \"message\": \"User does not exist / Error message regarding the transaction failure\"\n}";

            config.SetSampleRequest(sampleRequestBodyEditUser, new MediaTypeHeaderValue("application/json"), "User", "EditUser");
            config.SetSampleResponse(sampleResponseBodyEditUser, new MediaTypeHeaderValue("application/json"), "User", "EditUser");

            string sampleRequestBodyForgetPassword = "{\"Email\":\"a_testapi01@gmail.com\"} \n";
            string sampleResponseBodyForgetPassword = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"Your password has been sent to your email id entered.\" \n } \n\nResponse Type: Json  Error \n\n{\n \"status\": 0,\n \"message\": \"User does not exist / Error message regarding the transaction failure\"\n}";
            config.SetSampleRequest(sampleRequestBodyForgetPassword, new MediaTypeHeaderValue("application/json"), "User", "ForgetPassword");
            config.SetSampleResponse(sampleResponseBodyForgetPassword, new MediaTypeHeaderValue("application/json"), "User", "ForgetPassword");

            string sampleRequestBodyResetPassword = "{\"UserId\":\"267023\",\"NewPassword\":\"welcome\",\"OldPassword\":\"welcome\"} \n";
            string sampleResponseBodyResetPassword = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"Your password has been changed successfully.\"\n  }\n} \n\nResponse Type: Json  Error \n\n{\n \"status\": 0,\n \"message\": \"User does not exist / Error message regarding the transaction failure\"\n}";
            config.SetSampleRequest(sampleRequestBodyResetPassword, new MediaTypeHeaderValue("application/json"), "User", "ResetPassword");
            config.SetSampleResponse(sampleResponseBodyResetPassword, new MediaTypeHeaderValue("application/json"), "User", "ResetPassword");

            string sampleRequestBodySupport = "{\"UserId\":\"257560\",\"SectionId\":\"29582\",\"UserType\":\"ST\",\"PhoneNumber\":\"9050009724\", \"AttachmentFile\":\"\",\"TellAboutText\":\"Helo Text\",\"Date\":\"24/05/2021\",\"Time\":\"10:10:10 AM\"} \n";
            sampleRequestBodySupport += "\n\n Notes";
            sampleRequestBodySupport += "\n\n 1) Optional Fields are {\"AttachmentFile\":\"\",\"Date\":\"24/05/2021\",\"Time\":\"10:10:10 AM\" }";
            sampleRequestBodySupport += "\n\n 2) AttachmentFile' is in Base64String format";
            string sampleResponseBodySupport = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"Your support email has been successfully sent.\"\n \"response\":{\n \"UserId\":\"257560\" }\n} \n\nResponse Type: Json  Error \n\n{\n \"status\": 0,\n \"message\": \"User does not exist / Error message regarding the transaction failure\"\n}";
            config.SetSampleRequest(sampleRequestBodySupport, new MediaTypeHeaderValue("application/json"), "User", "Support");
            config.SetSampleResponse(sampleResponseBodySupport, new MediaTypeHeaderValue("application/json"), "User", "Support");

            //Country list
            string sampleResponseBodyCountrylist = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"Country list are successfully verified\",\n \"response\":{\n \"country_Name\":\"USA\",\"countryId\":\"2\" \n  }\n} \n\nResponse Type: Json  Error \n\n{\n \"status\": 0,\n \"message\": \"Country list does not exist / Error message regarding the transaction failure\"\n}";
            config.SetSampleResponse(sampleResponseBodyCountrylist, new MediaTypeHeaderValue("application/json"), "User", "GetCountryList");

            //State list
            string sampleResponseBodyStatelist = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"State list are successfully verified\",\n \"response\":{\n \"stateId\":\"40\",\"state_CountryId\":\"2\",\"state_Name\":\"Alabama\" \n  }\n} \n\nResponse Type: Json  Error \n\n{\n \"status\": 0,\n \"message\": \"CountryId does not exist / Error message regarding the transaction failure\"\n}";
            config.SetSampleRequest("CountryId=5", new MediaTypeHeaderValue("text/plain"), "User", "GetStateList");
            
            config.SetSampleResponse(sampleResponseBodyStatelist, new MediaTypeHeaderValue("application/json"), "User", "GetStateList");
            
            #endregion
            //For Course
            //     api/Course/Courses
            #region
            string sampleRequestBodyGetCourses = "{\"UserID\":\"257560\",\"UserType\":\"ST\"} \n";
            string sampleResponseBodyGetCourses = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"Succesfully retrieved user course information\",\n\"response\": {\n \"Course_Name\":\"Beginning Algebra\", \"Course_ISDev\":\"0\", \"CourseId\":\"452\", \"Section_Name\":\"Math 105 Demo\", \"SectionId\":\"29582\", \"Term_Name\":\"Fall 2012\", \"TermId\":\"1752\"\n  }\n} \n\nResponse Type: Json  Error \n\n{\n \"status\": 0,\n \"message\": \"User does not have access to section/No data found/Error while retrieving course information\"\n}";
            config.SetSampleRequest(sampleRequestBodyGetCourses, new MediaTypeHeaderValue("application/json"), "Course", "GetCourseList");
            config.SetSampleResponse(sampleResponseBodyGetCourses, new MediaTypeHeaderValue("application/json"), "Course", "GetCourseList");

            //     api/Course/CourseLevelInfo
            string sampleRequestBodyCourseLevelInfo = "{\"UserId\":\"257560\",\"CourseId\":\"452\",\"SectionId\":\"29582\",\"CRLevelId\":\"3612\"}\n";
            sampleRequestBodyCourseLevelInfo += "\n\n Note:'CRLevelId': default value is '-1' ";
            
            string sampleResponseBodyCourseLevelInfo = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"Succesfully retrieved course level information\",\n\"response\": {\n \"BlockedLevel\":\"0\", \"CR_LSchedule_FromDate\":\"3/16/2020\", \"CR_LSchedule_ToDate\":\"3/17/2020\", \"CRLevel_Name\":\"Tutorial: Important Terms and Symbols- Audio\", \"CRLevel_ParentId\":\"3612\", \"CRLevelId\":\"14703\", \"Depth\":\"4\", \"HASNOLEVEL\":\"0\", \"HASNOLO\":\"0\", \"ISLO\":\"1\", \"LOID\":\"12954\", \"LOType\":\"5\", \"SRLNo\":\"01\", \"TimeSpend\":\"169\"\n  }\n} \n\nResponse Type: Json  Error \n\n{\n \"status\": 0,\n \"message\": \"User does not have access to section/No data found/Error while retrieving course level information\"\n}";
            config.SetSampleRequest(sampleRequestBodyCourseLevelInfo, new MediaTypeHeaderValue("application/json"), "Course", "GetCourseLevelInfo");
            config.SetSampleResponse(sampleResponseBodyCourseLevelInfo, new MediaTypeHeaderValue("application/json"), "Course", "GetCourseLevelInfo");

            //     api/Course/GetCourseSelectedLevelInfo
            string sampleRequestBodyGetCourseSelectedLevelInfo = "{\"UserId\":\"257560\",\"CourseId\":\"452\",\"SectionId\":\"29582\",\"CRLevelId\":\"14703\"}\n";
            string sampleResponseBodyGetCourseSelectedLevelInfo = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"Succesfully retrieved course level information\",\n\"response\": {\n \"CRLevel_Name\":\"Tutorial: Important Terms and Symbols- Audio\", \"CRLevel_ParentId\":\"3612\", \"CRLevelId\":\"14703\",  \"Depth\":\"4\", \"FilePath\"=\"http://localhost//Content/Course//Beginning Algebra/12954/57C32EAA-D73D-4BF4-A17B-252A337D2EB1.swf\", \"HASNOLEVEL\":\"0\", \"HASNOLO\":\"0\", \"ISLO\":\"1\", \"LevelHierarchy\":\"Beginning Algebra > 1. Review of Pre-Algebra > 1.1 Review of Integers11\", \"LOID\":\"12954\", \"LOType\":\"5\", \"SRLNo\":\"01\", \"TimeSpend\":\"169\" \n  }\n} \n\nResponse Type: Json  Error \n\n{\n \"status\": 0,\n \"message\": \"User does not have access to section/No data found/Error while retrieving course level information\"\n}";
            config.SetSampleRequest(sampleRequestBodyGetCourseSelectedLevelInfo, new MediaTypeHeaderValue("application/json"), "Course", "GetCourseSelectedLevelInfo");
            config.SetSampleResponse(sampleResponseBodyGetCourseSelectedLevelInfo, new MediaTypeHeaderValue("application/json"), "Course", "GetCourseSelectedLevelInfo");

            //     api/Course/SaveCRLOTimeSpent
            string sampleRequestBodySaveCRLevelTimeSpent  = "{\"UserId\":\"267023\",\"SectionId\":\"32166\",\"CourseId\":\"451\",\"CRLOId\":\"11980\",\"TimeSpent\":\"133\"} \n";
            sampleRequestBodySaveCRLevelTimeSpent += "\n\n Notes: 'TimeSpent' value should be in seconds.";
            string sampleResponseBodySaveCRLevelTimeSpent = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"Time spent has been successfully Updated.\" \n} \n\nResponse Type: Json  Error \n\n{\n \"status\": 0,\n \"message\": \"Error occured while saving time spent.\"\n}";
            config.SetSampleRequest(sampleRequestBodySaveCRLevelTimeSpent, new MediaTypeHeaderValue("application/json"), "Course", "SaveCRLOTimeSpent");
            config.SetSampleResponse(sampleResponseBodySaveCRLevelTimeSpent, new MediaTypeHeaderValue("application/json"), "Course", "SaveCRLOTimeSpent");

            #endregion 

            #region

            //For Announcements
            //   api/Announcement/GetAnnouncementList
            string sampleRequestBodyGetAnnouncementList = "{\"UserId\":\"257560\",\"Announcement_Type\":\"3\",\"SectionId\":\"29582\",\"Announcement_OptSel\":\"0\",\"UserCurrZoneDate\":\"2020-04-29\"}\n";
            sampleRequestBodyGetAnnouncementList +="\n\n Note:";
            sampleRequestBodyGetAnnouncementList +="\n\n 1) 'Announcement_Type': '0' for General_Announcements,'3' for Course_Announcements";
            sampleRequestBodyGetAnnouncementList += "\n 2) 'Announcement_OptSel':'0/1/2/3/' for 'View All/View Today/Last 7 Days/Last 30 Days'";
            string sampleResponseBodyGetAnnouncementList = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"Succesfully retrieved announcements information\",\n\"response\": {\n \"AttachmentFiles\":\"1_body_banner2_large1.jpg\", \"Description\":\"hi\", \"FilePath\":\"http://localhost//Content/MessageAttachments/8DD27F4B-C3ED-4A64-987D-8723621C7E2B.jpg\", \"PostedDate\":\"04/25/2020\"\n  }\n} \n\nResponse Type: Json  Error \n\n{\n \"status\": 0,\n \"message\": \"User does not have access to section/No data found/Error while retrieving announcements information\"\n}";
            
            config.SetSampleRequest(sampleRequestBodyGetAnnouncementList, new MediaTypeHeaderValue("application/json"), "Announcement", "GetAnnouncementList");
            config.SetSampleResponse(sampleResponseBodyGetAnnouncementList, new MediaTypeHeaderValue("application/json"), "Announcement", "GetAnnouncementList");

            #endregion

            #region

            //For Assessments
            //   api/Assessments/GetAssessmentList
            string sampleRequestBodyGetAssessmentList = "{\"UserId\":\"257560\",\"SectionId\":\"29582\",\"CourseId\":\"452\",\"UserCurrZoneDate\":\"2020-04-29 04:49:36\",\"AssessmentType\":\"0\"}\n";
            sampleRequestBodyGetAssessmentList += "\n\n Note:";
            sampleRequestBodyGetAssessmentList += "\n\n 1) 'AssessmentType': '0' All,'1' for Homework, '2' for 'Quiz/Test', '3' for 'Practice', '4' for 'Custom'.";
            sampleRequestBodyGetAssessmentList += "\n\n 2) 'TestStatus': '1' for Test Active,'2' for Test Inactive, '3' for Test Completed, '4' for Test Expired, '5' for Exempted, '6' for Retake, '7' for Resume, '8' for Locked, '9' for Notapplicable, '10' for Improve";
            sampleRequestBodyGetAssessmentList += "\n\n 3)  if 'Course_ISDev' value 2 is a RMA Course, 'Course_ISDev' value get from response of 'GetCourseList' API Method \n";
            sampleRequestBodyGetAssessmentList += "     'AssessmentType' for RMA Course : 0 -Module Diagnostic Test(s),1-Module Mastery Test(s),2-Study Plan Test(s),3-Module Placement Test(s)";
            sampleRequestBodyGetAssessmentList += "\n\n 4) Assessment list details included for RMA Course like :'TestModuleId', 'CRModule_Name', 'UserActiveModule_Status', 'Test_IsLabTest', 'Test_ModulePercent', 'UserActiveModule_Diagnostic', 'IsExpmptLevelTest', 'CRModuleId', 'MMPT_ModuleId', 'Test_AutoPracticeTest', 'StudyPlanDuration', 'UserActiveModule_TestScore', 'SubmitStatus', 'GlobalPswd', 'SubmitStatus', 'TestSettings_TestModeType', 'isExemptByDiagnoSticTest', 'Test_Penaltypercent', 'SubmitStatus' ";
            string sampleResponseBodyGetAssessmentList = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"Succesfully retrieved assessments information\",\n\"response\": {\n \"TestId\":\"1195853\", \"TestName\":\"Check MC Issue\", \"Test_HasPreRequisites\":\"1\",\"Test_IsAutoPracticeTest\":\"0\", \"TestGBCataegory\":\"Quiz\", \"TestNumQues\":\"5\",\"MaxAttempts\":\"1\", \"ActualAttempts\":\"1\", \"TestTimeAllotted\":\"0\", \"TestDate\":\"1/18/2019 12:00:00 AM\", \"TestEndDate\":\"1/31/2019 11:59:00 PM\", \"TestMaxScoreObt\":\"70.0\", \"TestStatus\":\"3\", \"InitFlag\":\"(null)\", \"Test_LastAttemptedUserId\"=\"0\", \"Test_BestScoreUserId\"=\"0\", \"TestSettingsMode\":\"Practice\", \"TestType\":\"1\", \"TestPaperResultUrl\":\"http://localhost/Educo_Mobile/MobileApp/MApp_TestPaperResults.aspx?ID=0&IsReview=0&SectionId=29582&UserID=257560&USERTYPE=ST&TestId=1183022&TestUserID=0\",\"TestStatus_Desc\":\"Take\" \n  }\n} \n\nResponse Type: Json  Error \n\n{\n \"status\": 0,\n \"message\": \"User does not have access to section/No data found/Error while retrieving assessments information\"\n}";
            config.SetSampleRequest(sampleRequestBodyGetAssessmentList, new MediaTypeHeaderValue("application/json"), "Assessments", "GetAssessmentList");
            config.SetSampleResponse(sampleResponseBodyGetAssessmentList, new MediaTypeHeaderValue("application/json"), "Assessments", "GetAssessmentList");

            //  api/Assessments/GetInitTestPaperInfo
            
            string sampleRequestBodyGetInitTestPaperList = "{\"UserId\":\"257560\",\"SectionId\":\"29582\",\"CourseId\":\"452\",\"TestId\":\"1195853\"}\n";
            sampleRequestBodyGetInitTestPaperList += "\n\n Note :";
            sampleRequestBodyGetInitTestPaperList += "\n\n 1) 'PreReqAttachedAssessmentInfo' return list details for RMA Course : 'TestId',','TestPreAssID','TestPreAss_PreReqTestId','TestPreAss_MaxScore','TotalMarksObtained','TotalScoreForPreReq','PreRequistitTestName','TotalQuestForMainAss','TestUserID','PreReqTotQuesWithSolVarient','preReqMaxAttempt','preReqAttemptCount','TestEndDate','Test_Type','";
            sampleRequestBodyGetInitTestPaperList += "\n\n 2) 'PreqAttendancePercentInfo' return list details for RMA Course : 'Test_ModuleId','Min_AttendancePercent','AttendancePercentObtain','UserActiveModule_ActiveDate'";
            string sampleResponseBodyGetInitTestPaperList = "Response Type: Json  Success \n\n{\n \"status\":\"1\",\"message\":\"Successfully retrieved assessments information\",\n \"response\":{\"TestId\":\"1195853\",\"TestName\":\"TestApi03\",\"TestHeadName\":\"TestApi03\",\"TestType\":\"1\",\"Test_HasPreRequisite\":\"1\",\"Test_IsComp\":\"1\",\"Test_ModuleId\"=\"442870\",\"TotalTime\":\"0\",\"TestDate\":\"1/16/2021 12:00:00 AM\",\"TestEndDate\":\"2/28/2021 11:59:59 PM\",\"DispRes\":\"1\",\"ResDate\":\"\",\"IsAutoPracticeTest\":\"1\",\"NoAtmpts\":\"10\",\"NegMrks\":\"0\",\"TotalQues\":\"4\",\"TotalMarks\":\"4\",\"Penaltypercent\":\"0\",\"PenaltyExpDate\":\"\",\"DynDispFlag\":\"3\",\"DynDispMode\":\"2\",\"ModulePercent\":\"50\",\"IsStudyPlan\":\"1\",\"IsEnableLockdownBrowser\":\"0\",\"IsLabTest\":\"0\",\n  \"PreReqLOsInfo\":[{\"TestId\":\"1195853\",\"TestPreReqId\":\"235889\",\"TestPreReq_LevelID\":\"14703\",\"TestPreReq_TimeAlloted\":\"5\",\"TestPreReq_LevelParent\":\"3612\",\"TestPreReq_LevelLo\":\"12954\",\"CRTimeSpent_TimeSpentMin\":0.0,\"CourseHirarchy\":\"1.1 Review of Integers11 > Tutorial: Review of Integers : Important Vocabulary\", \"FilePath\"=\"http://localhost/Content/Course/Beginning Algebra/12954/57C32EAA-D73D-4BF4-A17B-252A337D2EB1.swf\"}],\n \"PreReqAttachedAssessmentInfo\":[{\"TestId\":\"1195853\",\"TestPreAssID\":\"74209\",\"TestPreAss_PreReqTestId\":\"1195848\",\"TestPreAss_MaxScore\":\"50\",\"TotalMarksObtained\":\"0\",\"TotalScoreForPreReq\":\"0\",\"PreRequistitTestName\":\"Master Homework\",\"TotalQuestForMainAss\":\"0\",\"PreReqTotQuesWithSolVarient\":\"4\",\"preReqMaxAttempt\":\"5\",\"preReqAttemptCount\":\"0\",\"TestEndDate\":\"2/28/2021 11:59:59 PM\",\"Test_Type\":\"1\",\"TestUserID\":\"446128673\",\"IsLabTest\":\"0\",\"Test_Penaltypercent\":\"0\",\"Test_PreReqGroup\":\"1\",\"IsEnableLockdownBrowser\":\"0\"}], \n  \"PreqAttendancePercentInfo\":[{\"TestId\":\"1195853\",\"Min_AttendancePercent\":\"50\",\"AttenPreReqTestAtm\":\"1\",\"AttendancePercentObtain\":\"0\"}],\n \"PreReqVideosInfo\":[{\"TestId\":\"1195853\",\"TestPrevideoID\":\"711\",\"TestPreReqVideo_uploadID\":\"1498043\",\"TestPreReqVideo_MinTime\":\"4\",\"TestPreReqVideo_TimeSpent\":\"0\",\"TestPreReqVideo_FileName\":\"YouTubewn\",\"TestPreReqVideo_VideoType\":\"3\",\"IsAdminVideo\":\"0\" \"FilePath\"=\"//www.youtube.com/embed/73VCLo1qxso?wmode=opaque&autoplay=1\"}]} \n }  \n\nResponse Type: Json  Error \n\n{\n \"status\": 0,\n \"message\": \"User does not have access to section/No data found/Error while retrieving assessments information\"\n}";
            
            config.SetSampleRequest(sampleRequestBodyGetInitTestPaperList, new MediaTypeHeaderValue("application/json"), "Assessments", "GetInitTestPaperInfo");  
            config.SetSampleResponse(sampleResponseBodyGetInitTestPaperList, new MediaTypeHeaderValue("application/json"), "Assessments", "GetInitTestPaperInfo");
            #endregion

            #region

            //For InternalMail
            //   api/InternalMail/GetInternalMailList
            string sampleRequestBodyGetInternalMailList = "{\"UserId\":\"257560\"}\n";
            string sampleResponseBodyGetInternalMailList = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"Succesfully retrieved InternalMail information\",\n\"response\": {\"MsgRecieverId\":\"2487836\",\"MsgSenderId\":\"1475542\",\"MsgSender_Date\":\"3/9/2022 11:42:44 AM\",\"MsgSender_Attachment\":\"0\",\"MsgSender_Body\":\"<br />\r\n<div>Hello <br />\r\n</div>\r\n<div><br />\r\n</div>\r\n<div>Test<br />\r\n</div>\r\n<br />\r\n<br />\r\n<br />\r\n<strong>See attachment</strong><br />\r\n<img style=\"vertical-align:\" middle; border:\" 0px solid;\" alt=\"emoticon\" src=\"https://www.educosoft.com/Common/Editor/Plugins/emoticons/Icons/3.gif\" /> CC1\",\"MsgSender_Subject\":\"InternalMail_Test_API with attachment\",\"MsgSender_Sender_SectionId\":\"29582\",\"Section_Name\":\"Math 105 Demo\",\"Course_Name\":\"Beginning Algebra\",\"LastName\":\"Hawkins\",\"FirstName\":\"Stephen\",\"SectionId\":\"29582\" \n }\n\nResponse Type: Json  Error \n{\n \"status\": 0,\n \"message\": \"User does not have access to section/No data found/Error while retrieving InternalMail information\"\n}";
            config.SetSampleRequest(sampleRequestBodyGetInternalMailList, new MediaTypeHeaderValue("application/json"), "InternalMail", "GetInternalMailList");
            config.SetSampleResponse(sampleResponseBodyGetInternalMailList, new MediaTypeHeaderValue("application/json"), "InternalMail", "GetInternalMailList");

            //   api/InternalMail/GetInternalMailView
            string sampleRequestBodyGetInternalMailView = "{\"UserId\":\"257560\",\"MsgSenderId\":\"1475542\"}\n";
            string sampleResponseBodyGetInternalMailView = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"Succesfully retrieved InternalMail information\",\n \"response\": {\"MsgRecieverId\":\"2487836\",\"MsgSenderId\":\"1475542\",\"MsgSender_Date\":\"3/9/2022 11:42:44 AM\",\"MsgSender_Attachment\":\"0\",\"MsgSender_Body\":\"<br />\r\n<div>Hello <br />\r\n</div>\r\n<div><br />\r\n</div>\r\n<div>Test<br />\r\n</div>\r\n<br />\r\n<br />\r\n<br />\r\n<strong>See attachment</strong><br />\r\n<img style=\"vertical-align:\" middle; border:\" 0px solid;\" alt=\"emoticon\" src=\"https://www.educosoft.com/Common/Editor/Plugins/emoticons/Icons/3.gif\" /> CC1\",\"MsgSender_Subject\":\"InternalMail_Test_API with attachment\",\"MsgSender_Sender_SectionId\":\"29582\",\"Section_Name\":\"Math 105 Demo\",\"Course_Name\":\"Beginning Algebra\",\"LastName\":\"Hawkins\",\"FirstName\":\"Stephen\",\"SectionId\":\"29582\", \n \"FileAttachmentlist\":\"[]\", \n \"FromRecieveMailIds\":\"[{\"UserId\":\"257560\",\"MsgReciever_MsgSenderId\":\"1475542\",\"RecieverType\":\"TO\",\"Resource_Name\":\"atest-104@edu.com\",\"Name\":\"104, A1\"},\n{\"UserId\":\"1336\",\"MsgReciever_MsgSenderId\":\"1475542\",\"MsgReciever_RecieverType\":\"TO\",\"Resource_Name\":\"cc1@educo-int.com\",\"Name\":\"Hawkins\", \"Stephen\" \n}] } \n\nResponse Type: Json  Error \n{\n \"status\": 0,\n \"message\": \"User does not have access to section/No data found/Error while retrieving InternalMail information\"\n}";
            config.SetSampleRequest(sampleRequestBodyGetInternalMailView, new MediaTypeHeaderValue("application/json"), "InternalMail", "GetInternalMailView");
            config.SetSampleResponse(sampleResponseBodyGetInternalMailView, new MediaTypeHeaderValue("application/json"), "InternalMail", "GetInternalMailView");

             //   api/InternalMail/DeleteMail
            string sampleRequestBodyDeleteMail = "{\"MsgRecieverId\":\"2487838\"}\n";
            string sampleResponseBodyDeleteMail = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"Successfully deleted internal mail\",\n\"response\": {\"MsgRecieverId\":\"2487836\" }\n} \n\nResponse Type: Json  Error \n{\n \"status\": 0,\n \"message\": \"User does not exit/No data found/Error while retrieving InternalMail information\"\n}";
            config.SetSampleRequest(sampleRequestBodyDeleteMail, new MediaTypeHeaderValue("application/json"), "InternalMail", "DeleteMail");
            config.SetSampleResponse(sampleResponseBodyDeleteMail, new MediaTypeHeaderValue("application/json"), "InternalMail", "DeleteMail");

             //   api/InternalMail/SendInternalMail
            string sampleRequestBodySendInternalMail = "{\"UserId\":\"257560\",\"Body\":\"hello test1\",\"Subject\":\"Hello subject\",\"MsgSendToUsersIds\":\"1336,257561\",\"MsgSendBccUsersIds\":\"1336,257562\",\"SectionId\":\"29582\",\"AttachmentOriginalFiles\":\"file.pdf,Image,jpg\",\"AttachmentBase64StringFiles\":\"Base64StringFile,Base64StringFile\"}\n";
            sampleRequestBodySendInternalMail += "\n\n Note :";
            sampleRequestBodySendInternalMail += "\n\n 1) 'NonAuthorizedUserIds': if you mention any NonAthorized user id(s) in 'MsgSendToUsersIds', 'MsgSendBccUsersIds' then those user id(s) has displays in this field";
            sampleRequestBodySendInternalMail += "\n\n 2) 'AttachmentBase64StringFiles': is in Base64String format, if AttachmentBase64StringFiles are more than one then add file suffix with ',' ";
            sampleRequestBodySendInternalMail += "\n\n 3) 'AttachmentOriginalFiles': is in file name(s) should be sequence of 'AttachmentBase64StringFiles', if AttachmentOriginalFiles are more than one then add file suffix with ',' ";

            string sampleResponseBodySendInternalMail = "Response Type: Json  Success \n\n{\n\"status\":\"1\",\n\"message\":\"Message has been sent successfully.\",\n\"response\": {\"UserId\":\"257560\",\n\"NonAuthorizedUserIds\":{\"NonAuthorizedUserIds\":\"257561,257562\",\"NonAutorizedUserMsg\":\"You are not authorized to send mail to this user(s).\" }\n} \n\nResponse Type: Json  Error \n{\n \"status\": 0,\n \"message\": \"User does not exit/No data found/Error while retrieving send internal mail information\"\n}";
            config.SetSampleRequest(sampleRequestBodySendInternalMail, new MediaTypeHeaderValue("application/json"), "InternalMail", "SendInternalMail");
            config.SetSampleResponse(sampleResponseBodySendInternalMail, new MediaTypeHeaderValue("application/json"), "InternalMail", "SendInternalMail");

            #endregion

        }
    }
}

