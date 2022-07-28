using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Web.Http.ModelBinding;
using EducosoftAPI.Models.Assessments;
using Educosoft.Api.Assessments;
using EducosoftAPI.Models.User;
using EducosoftAPI.Models;
using System.Text;
using System.Web;
using System.Configuration;

namespace EducosoftAPI.Controllers
{
    public class AssessmentsController : ApiController
    {
        /// <summary>
        ///This api call takes Assessment Credential i.e., 'UserId', 'SectionId', 'CourseId', 'UserCurrZoneDate', 'AssessmentType' and 
        ///return Assessment list details like 'ActualAttempts', 'MaxAttempts', 'Test_HasPreRequisites', 'Test_IsAutoPracticeTest', 'TestDate', 'TestEndDate', 'TestGBCataegory', 
        ///'TestMaxScoreObt', 'TestId', 'TestName', 'TestNumQues', 'TestStatus', 'InitFlag', 'TestSettingsMode', 'TestType', 'TestTimeAllotted', 'Test_LastAttemptedUserId', 'TestPaperResultUrl' and 'Test_BestScoreUserId' with 'status' and 'message' upon successful retrieval of assessment information.
        ///Incase of failure, it returns only 'status' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in retrieving assessments information.
        ///Note:
        ///1) 'AssessmentType': '0' All,'1' for Homework, '2' for 'Quiz/Test', '3' for 'Practice', '4' for 'Custom', 
        ///2) 'TestStatus': '1' for Test Active,'2' for Test Inactive, '3' for Test Completed, '4' for Test Expired, '5' for Exempted, '6' for Retake, '7' for Resume, '8' for Locked, '9' for Notapplicable, '10' for Improve.
        ///3) 'AssessmentType' for RMA Course : 0 -Module Diagnostic Test(s),1-Module Mastery Test(s),2-Study Plan Test(s),3-Module Placement Test(s),
        ///4)  Assessment list details included for RMA Course like :'TestModuleId', 'CRModule_Name', 'UserActiveModule_Status', 'Test_IsLabTest', 'Test_ModulePercent', 'UserActiveModule_Diagnostic', 'IsExpmptLevelTest', 'CRModuleId', 'MMPT_ModuleId', 'Test_AutoPracticeTest', 'StudyPlanDuration', 'UserActiveModule_TestScore', 'SubmitStatus', 'GlobalPswd', 'SubmitStatus', 'TestSettings_TestModeType', 'isExemptByDiagnoSticTest', 'Test_Penaltypercent', 'SubmitStatus', 'TestStatus_Desc'
        /// </summary>
        /// <param name="AssessmentCredential">Assessments retrieval credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": 1,
        ///   "message": "Succesfully retrieved assessments information.",
        ///   "response": 
        ///      {  
        ///       "TestId":"",
        ///       "TestName" : "",
        ///       "Test_IsAutoPracticeTest":"",
        ///       "TestGBCataegory" : "",
        ///       "TestNumQues" : "",
        ///       "MaxAttempts" : "",
        ///       "ActualAttempts" : "",
        ///       "TestTimeAllotted" : "",
        ///       "TestDate" : "",
        ///       "TestEndDate" : "",
        ///       "TestMaxScoreObt" : "",
        ///       "TestSettingsMode":"",
        ///       "InitFlag":"", 
        ///       "TestType":"",
        ///       "TestStatus":"",
        ///       "Test_HasPreRequisites"="",
        ///       "TestPaperResultUrl"="",
        ///       "TestModuleId" :"",
        ///       "CRModule_Name" :"",
        ///       "UserActiveModule_Status" :"",
        ///       "Test_IsLabTest" :"",
        ///       "Test_ModulePercent" :"",
        ///       "UserActiveModule_Diagnostic" :"",
        ///       "IsExpmptLevelTest" :"",
        ///       "Course_ModuleId" :"",
        ///       "MMPT_ModuleId" :"",
        ///       "Test_AutoPracticeTest" :"",
        ///       "StudyPlanDuration" :"",
        ///       "UserActiveModule_TestScore" :"",
        ///       "SubmitStatus" :"",
        ///       "GlobalPswd" :"",
        ///       "TestSettings_TestModeType" :"",
        ///       "UserActiveModule_TestScore" :"",
        ///       "isExemptByDiagnoSticTest" :"",
        ///       "Test_Penaltypercent":"",
        ///       "TestStatus_Desc":"",
        ///       "Test_BestScoreUserId":"",
        ///       "Test_LastAttemptedUserId":"",
        ///   }
        ///Error:
        ///{
        ///"status": 0,
        ///"message": "User does not have access to section/Error while retrieving assessment information."
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("GetAssessmentList")]
        public HttpResponseMessage GetAssessmentList([ModelBinder] AssessmentCredential AssessmentCredential)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(AssessmentCredential.UserId) && !string.IsNullOrEmpty(AssessmentCredential.SectionId) &&
                    !string.IsNullOrEmpty(AssessmentCredential.CourseId) && !string.IsNullOrEmpty(AssessmentCredential.UserCurrZoneDate))
                {
                    try
                    {
                        Api_TestPaper objAssmts = new Api_TestPaper();
                        PGCC_TestPaper ObjPGCCTestPaper = null;
                        char colSeperator = (char)195;
                        DataSet dst, dtCourse;
                        DateTime dtCurrentDate;
                        TimeZoneDateTime ObjTimeZoneDateTime = null;
                        string Testids = string.Empty;
                        bool blnbypassexp = false;
                        bool IsPenaltyDateExpired = false;
                        string UnitModuleId = "0";
                        string ActiveModules = "0";
                        bool IsUDTtaken = true;
                        int IsUnitBased = 0;
                        UserCourse objUserCourse = null;
                        string IsCourseDev = "";
                        StringBuilder spParam = new StringBuilder();
                        string UserId = "", SectionId = "", AssessmentType = "", strUserZoneCurrDate = "", CourseId = "", Tz_Name = "", TzID = "";
                        string[] arrAssType = { "0", "1", "2", "3", "4" };
                        string InstutionId = "0";
                        string CountryId = "2";
                        UserId = AssessmentCredential.UserId.Trim();
                        SectionId = AssessmentCredential.SectionId.Trim();
                        CourseId = AssessmentCredential.CourseId.Trim();
                        strUserZoneCurrDate = AssessmentCredential.UserCurrZoneDate.Trim();
                        AssessmentType = AssessmentCredential.AssessmentType.Trim();

                        if (arrAssType.Contains(AssessmentType))
                        {
                            User objUser = new User();
                            dst = objUser.GetUserInfo(UserId);
                            if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(dst.Tables[0].Rows[0][28])))
                                {
                                    InstutionId = dst.Tables[0].Rows[0][28].ToString();

                                }
                                CountryId = dst.Tables[0].Rows[0]["Users_Country"].ToString();

                                objUserCourse = new UserCourse();
                                dtCourse = objUserCourse.GetCourseLevelInfo(CourseId);

                                if (dtCourse != null && dtCourse.Tables[0].Rows.Count > 0 && dtCourse.Tables[0].Rows[0][0].ToString() == "0")
                                {
                                    IsCourseDev = dtCourse.Tables[0].Rows[0]["Course_ISDev"].ToString();

                                    Tz_Name = dst.Tables[0].Rows[0]["Tz_Name"].ToString();
                                    TzID = dst.Tables[0].Rows[0]["Resource_TzId"].ToString();
                                    dtCurrentDate = DateTime.Now;
                                    ObjTimeZoneDateTime = new TimeZoneDateTime();
                                    strUserZoneCurrDate = ObjTimeZoneDateTime.GetUserTimeZoneDateTime(Tz_Name, dtCurrentDate).ToString();

                                    //Educosoft.Api.Utilities.FileUpload uploadObj;
                                    //uploadObj = new Educosoft.Api.Utilities.FileUpload();
                                    //dst = null;
                                    //spParam = new StringBuilder();
                                    //spParam.Length = 0;
                                    //spParam.Append("1").Append(colSeperator).Append("FP5");
                                    //dst = uploadObj.GetVirtualPath(spParam.ToString());

                                    //string strVFilePathUrl = "";
                                    //if (dst.Tables[0].Rows[0].ItemArray[0].ToString() == "0")
                                    //{
                                    //    string VFilePath = dst.Tables[0].Rows[0].ItemArray[4].ToString();
                                    //    string[] arrVFilePath = VFilePath.Split('/');
                                    //    string strurl = arrVFilePath[2];
                                    //    strVFilePathUrl = "http://" + strurl + "/educotablet/MobileApp/MApp_TestPaperResults.aspx?ID=";
                                    //}

                                    string strVFilePathUrl = "https://www.educosoft.com/EducoTablet/MobileApp/MApp_TestPaperResults.aspx?ID=";
                                    if (IsCourseDev == "2")
                                    {
                                        int ActivateOnlyCDT = 0;
                                        int MMPTExists = 0;
                                        string ActivaModuleId = "0";
                                        string UserCurrentModuleId = "0";
                                        string PreviousModuleId = "0";
                                        string ActivateSummary = "0";
                                        int HasDataInUserActiveModule = -1;
                                        bool boolappplty = false, blnDispScore = true;
                                        string IsLabTest = "0"; //strResult = string.Empty, strResultParam = string.Empty;
                                        string valPreReq = "0";
                                        int isExempted = 0;
                                        string hdnTutorialExmpt = "0";
                                        string hdnAssessmentExmpt = "0";
                                        string hdnAttendanceExmpt = "0";
                                        int attemptsleft = 0;
                                        string PenaltyPer = "";
                                        int intTotSec = 0;
                                        int LObasedTest = 0;
                                        TestPaper objTesPaper = new TestPaper();
                                        DateTime UserZoneDtTm = DateTime.Now;
                                        Boolean PreRequisiteCompleted = false;
                                        //string strmsg, strTitle;
                                        string hdnAccessFromLab = "0";
                                        TestPaper objTestPaper = null;
                                        bool IsExpiredTerm = false;

                                        spParam.Length = 0;
                                        spParam.Append("1").Append(colSeperator).Append(UserId).Append(colSeperator);
                                        spParam.Append("2").Append(colSeperator).Append(CourseId).Append(colSeperator);
                                        spParam.Append("3").Append(colSeperator).Append("0").Append(colSeperator);
                                        spParam.Append("4").Append(colSeperator).Append(SectionId).Append(colSeperator);
                                        spParam.Append("5").Append(colSeperator).Append(DateTime.Parse(strUserZoneCurrDate)).Append(colSeperator);
                                        spParam.Append("6").Append(colSeperator).Append(AssessmentType);

                                        dst = null;
                                        ObjPGCCTestPaper = new PGCC_TestPaper();
                                        dst = ObjPGCCTestPaper.LoadTestPapers_M(spParam.ToString());

                                        if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                                        {
                                            if (dst.Tables[0].Rows[0][0].ToString() == "0")
                                            {
                                                if (dst.Tables.Count > 1)
                                                {
                                                    MMPTExists = Convert.ToInt16(dst.Tables[1].Rows[0]["MMPTExists"].ToString());
                                                    UserCurrentModuleId = dst.Tables[1].Rows[0]["ActiveCRModuleId"].ToString();
                                                    ActivaModuleId = dst.Tables[1].Rows[0]["ActiveModuleID"].ToString();
                                                    PreviousModuleId = dst.Tables[1].Rows[0]["PreviousModuleId"].ToString();
                                                    HasDataInUserActiveModule = Convert.ToInt32(dst.Tables[1].Rows[0]["HasDataInUserActiveModule"].ToString());
                                                    if (dst.Tables[1].Rows[0]["Institution_MandateCDT"].ToString().Trim() == "1")
                                                    {
                                                        if (dst.Tables[1].Rows[0]["CRModuleActivated"].ToString().Trim() == "2")
                                                            ActivateOnlyCDT = 1;
                                                    }
                                                }
                                                for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                                                {
                                                    IsLabTest = dst.Tables[0].Rows[i][34].ToString();
                                                    if (TzID != dst.Tables[0].Rows[i][24].ToString())
                                                    {
                                                        dst.Tables[0].Rows[i][5] = ObjTimeZoneDateTime.GetUserTimeZoneDateTime(Tz_Name, DateTime.Parse(dst.Tables[0].Rows[i][5].ToString()));
                                                        dst.Tables[0].Rows[i][13] = ObjTimeZoneDateTime.GetUserTimeZoneDateTime(Tz_Name, DateTime.Parse(dst.Tables[0].Rows[i][13].ToString()));
                                                    }
                                                    if (dst.Tables[0].Rows[i][10].ToString() == "3")
                                                    {
                                                        dst.Tables[0].Rows[i][11] = ObjTimeZoneDateTime.GetUserTimeZoneDateTime(Tz_Name, DateTime.Parse(dst.Tables[0].Rows[i][11].ToString()));
                                                    }
                                                    if (DateTime.Parse(strUserZoneCurrDate) < DateTime.Parse(dst.Tables[0].Rows[i][5].ToString()))
                                                    {
                                                        dst.Tables[0].Rows[i][20] = "2";    //Test Inactive.
                                                    }
                                                    else if (Convert.ToInt32(dst.Tables[0].Rows[i][7].ToString()) >= Convert.ToInt32(dst.Tables[0].Rows[i][6].ToString()))
                                                    {
                                                        dst.Tables[0].Rows[i][20] = "3";    //Test Completed.
                                                    }
                                                    else
                                                    {
                                                        dst.Tables[0].Rows[i][20] = "1"; //Take test.
                                                    }
                                                }

                                                if (Testids.Length > 0)
                                                {
                                                    spParam.Length = 0;
                                                    spParam.Append("1").Append(colSeperator).Append(Testids).Append(colSeperator);
                                                    spParam.Append("2").Append(colSeperator).Append(UserId);
                                                    objAssmts.UpdateStatusFlags(spParam.ToString());
                                                }

                                                switch (AssessmentType)             //0-Module Diagnostic Test(s),1-Module Mastery Test(s),2-Study Plan Test(s),3-Module Placement Test(s)- 
                                                {
                                                    case "0": dst.Tables[0].DefaultView.RowFilter = "TestType IN (12,19)";  //
                                                        break;
                                                    case "1": dst.Tables[0].DefaultView.RowFilter = "TestType=13 OR TestType=18";
                                                        break;
                                                    case "2": dst.Tables[0].DefaultView.RowFilter = "TestType<>12 AND TestType<>13 AND TestType<>18";
                                                        break;
                                                    case "3":
                                                        if (dst.Tables.Count > 1 && dst.Tables[1].Rows[0]["ActiveModuleID"].ToString().Trim() != "0")
                                                            dst.Tables[0].DefaultView.RowFilter = "TestType=16 AND MMPT_ModuleId<>0";
                                                        else
                                                            dst.Tables[0].DefaultView.RowFilter = "TestType=16";
                                                        break;
                                                    //Added 0 for Claflin PreTest where Test_ModuleId will be -1 (19-Feb-2015)
                                                    case "4": dst.Tables[0].DefaultView.RowFilter = "TestType=17 AND UserActiveModule_Status IN (0,1,2,3)";
                                                        if (dst.Tables[1].Rows[0]["LastModuleCompleted"].ToString().Trim() == "1")
                                                            ActivateSummary = "1";
                                                        break;
                                                    default: dst.Tables[0].DefaultView.RowFilter = "TestType IN (12,19)";
                                                        break;
                                                }

                                                DataView dtview = dst.Tables[0].DefaultView;
                                                DataTable dt = dtview.ToTable();

                                                //added grid code
                                                if (dt.Rows.Count > 0)
                                                {
                                                    objTestPaper = new TestPaper();
                                                    hdnAccessFromLab = objTesPaper.CheckLabAccess(InstutionId);

                                                    foreach (DataRow row in dt.Rows)
                                                    {

                                                        IsLabTest = row["Test_IsLabTest"].ToString();      //34
                                                        if (row["CRModule_Name"].ToString() == "&nbsp;")   //15
                                                            row["CRModule_Name"] = "NA";

                                                        if (row["TestResultMode"].ToString() == "4")    //Do not show results after students have completed an assessment  11
                                                        {
                                                            blnDispScore = false;
                                                        }
                                                        else if (row["TestResultMode"].ToString() == "3")//Show results after given date
                                                        {
                                                            if (DateTime.Parse(strUserZoneCurrDate) < DateTime.Parse(row["TestResultDate"].ToString()))
                                                            {
                                                                blnDispScore = false;
                                                            }
                                                        }
                                                        else if (row["ActualAttempts"].ToString() == "0")   //6
                                                        {
                                                            blnDispScore = false;
                                                        }

                                                        PenaltyPer = row["Test_Penaltypercent"].ToString();
                                                        string TestName = row["TestName"].ToString();   //2

                                                        if (blnDispScore)
                                                        {
                                                            if (PenaltyPer != "0" && row["TestMaxScoreObt"].ToString() != "0" && ObjTimeZoneDateTime.Checkpenaltyassdate(DateTime.Parse(row["TestEndDate"].ToString()), DateTime.Parse(row["Test_AtptDate"].ToString()), Tz_Name) == true) //included for test_penaltypercent Diwakar                     
                                                            {
                                                                row["TestMaxScoreObt"] = row["TestMaxScoreObt"].ToString();   //18
                                                            }
                                                        }
                                                        else
                                                        {
                                                            row["TestMaxScoreObt"] = row["TestMaxScoreObt"].ToString().Trim();
                                                        }

                                                        if (PenaltyPer != "0" && DateTime.Parse(strUserZoneCurrDate) >= DateTime.Parse(row["TestEndDate"].ToString()))
                                                        {
                                                            boolappplty = true;
                                                        }
                                                        else
                                                        {
                                                            boolappplty = false;
                                                        }

                                                        Boolean blnLOTest = false;
                                                        LObasedTest = 0;
                                                        if (row["TestType"].ToString() == "3")
                                                        {
                                                            blnLOTest = true;
                                                            LObasedTest = 1;
                                                        }

                                                        valPreReq = row["HasPreRequisites"].ToString();

                                                        if (IsExpiredTerm)
                                                        {
                                                            //row["TestStatus"] = "Take";
                                                            row["TestStatus"] = "1";

                                                        }
                                                        else
                                                        {
                                                            if (Convert.ToInt32(valPreReq) == 1)
                                                            {
                                                                string PreReqExmptUsers = objTesPaper.CheckPreReqExmpt_M(row["intPkVal"].ToString(), UserId, SectionId);
                                                                string[] arrPreReqExmptUsers = PreReqExmptUsers.Split('$');
                                                                isExempted = Convert.ToInt16(arrPreReqExmptUsers[2]);
                                                                hdnTutorialExmpt = arrPreReqExmptUsers[1];
                                                                hdnAssessmentExmpt = arrPreReqExmptUsers[2];

                                                                if (isExempted == 1)
                                                                {
                                                                    if (hdnTutorialExmpt.ToString() == "1" && hdnAssessmentExmpt.ToString() == "1" && hdnAttendanceExmpt.ToString() == "1")
                                                                        valPreReq = "0";
                                                                    else
                                                                    {
                                                                        valPreReq = "1";
                                                                    }
                                                                }

                                                                if (row["TestType"].ToString() == "3")
                                                                {
                                                                    row["TestTimeAllotted"] = "";
                                                                }
                                                                else if (row["CRModule_Name"].ToString() == "0")
                                                                {
                                                                    //row["TestTimeAllotted"] = "NA";
                                                                }
                                                                else
                                                                {
                                                                    if (row["TestTimeAllotted"].ToString() == "0")
                                                                    {
                                                                        row["TestTimeAllotted"] = "0";
                                                                    }
                                                                    else
                                                                    {
                                                                        intTotSec = Convert.ToInt32(row["TestTimeAllotted"].ToString());
                                                                        row["TestTimeAllotted"] = objTestPaper.TimeFormat(intTotSec);
                                                                    }
                                                                }

                                                                if (row["InitFlag"].ToString() == "2")
                                                                {
                                                                    row["ActualAttempts"] = Convert.ToString((Convert.ToInt32(row["ActualAttempts"].ToString())));
                                                                }

                                                            }
                                                        }

                                                        row["TestResultMode"] = row["Test_IsLabTest"].ToString();
                                                        switch (row["TestStatus"].ToString().ToString())
                                                        {
                                                            case "1":   // Active Assessment
                                                                if (row["InitFlag"].ToString() != "2")    // Check for Resumed Attempt
                                                                {
                                                                    if (row["TestType"].ToString() == "3") // LO Test
                                                                    {
                                                                        if (IsExpiredTerm)
                                                                        {
                                                                            //row["TestStatus"] = "Take";
                                                                            row["TestStatus"] = "1";
                                                                            row[2] = "Take";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Convert.ToInt32(row["ActualAttempts"].ToString()) > 0)
                                                                            {
                                                                                if (IsLabTest == "1" && hdnAccessFromLab == "0")  //Check lab assessment are accessed from lab or not)
                                                                                {
                                                                                    //row["TestStatus"] = "Retake lab access";  //ShowLabAccessMsg
                                                                                    row["TestStatus"] = "6";
                                                                                    row[2] = "This test can only be taken in the assigned campus lab. Check with your instructor for specific locations";
                                                                                }
                                                                                else
                                                                                    //row["TestStatus"] = "Retake LOTest";   //LOTest
                                                                                    row["TestStatus"] = "6";
                                                                                row[2] = "Retake LOTest";
                                                                            }
                                                                            else
                                                                            {
                                                                                if (IsLabTest == "1")
                                                                                {
                                                                                    if (hdnAccessFromLab == "0")
                                                                                    {
                                                                                        //Check lab assessment are accessed from lab or not)
                                                                                        //row["TestStatus"] = "actvie_labtest"; //ShowLabAccessMsg
                                                                                        row["TestStatus"] = "1";
                                                                                        row[2] = "This test can only be taken in the assigned campus lab. Check with your instructor for specific locations";

                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        //row["TestStatus"] = "actvie_labtest";   //LOTest
                                                                                        row["TestStatus"] = "1";
                                                                                        row[2] = "Retake LOTest";
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    //row["TestStatus"] = "Take";  //LOTest
                                                                                    row["TestStatus"] = "1";
                                                                                    row[2] = "Take";
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else if (row["TestType"].ToString() == "5") //LO Score (TYS)
                                                                    {
                                                                        if (Convert.ToInt32(valPreReq) == 1)     // 1: Assessment has prerequisite
                                                                        {
                                                                            string PreReqResult = objTesPaper.CheckPreRequisite_M(row["intPkVal"].ToString(), UserId, CourseId, SectionId, Tz_Name);
                                                                            string[] arrPreReqResult = PreReqResult.Split('$');
                                                                            if (arrPreReqResult[3].ToString() == "0")
                                                                                PreRequisiteCompleted = false;
                                                                            else
                                                                                PreRequisiteCompleted = true;

                                                                            hdnTutorialExmpt = arrPreReqResult[0];
                                                                            hdnAssessmentExmpt = arrPreReqResult[1];
                                                                            hdnAttendanceExmpt = arrPreReqResult[2];

                                                                            if (hdnTutorialExmpt == "1" && hdnAssessmentExmpt == "1" && hdnAttendanceExmpt == "1")
                                                                            {
                                                                                valPreReq = "0";
                                                                            }
                                                                        }
                                                                        if (IsExpiredTerm)
                                                                        {
                                                                            //row["TestStatus"] = "Take";
                                                                            row["TestStatus"] = "1";
                                                                            row[2] = "Take";

                                                                        }
                                                                        else
                                                                        {
                                                                            if (Convert.ToInt32(valPreReq) == 1)
                                                                            {
                                                                                if (Convert.ToInt32(row["ActualAttempts"].ToString()) > 0)
                                                                                {
                                                                                    if (IsLabTest == "1" && hdnAccessFromLab == "0")
                                                                                    //Check lab assessment are accessed from lab or not)
                                                                                    {
                                                                                        //row["TestStatus"] = "Retake lab access";  //ShowLabAccessMsg
                                                                                        row["TestStatus"] = "6";
                                                                                        row[2] = "This test can only be taken in the assigned campus lab. Check with your instructor for specific locations";
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        row[2] = "Retake";  //InitTest
                                                                                        row["TestStatus"] = "6";
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (IsLabTest == "1")
                                                                                    {
                                                                                        if (hdnAccessFromLab == "0")
                                                                                        {
                                                                                            //Check lab assessment are accessed from lab or not)
                                                                                            //row["TestStatus"] = "actvie_labtest";
                                                                                            row["TestStatus"] = "1";
                                                                                            row[2] = "This test can only be taken in the assigned campus lab. Check with your instructor for specific locations";

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            //row["TestStatus"] = "actvie_labtest";
                                                                                            row["TestStatus"] = "1";
                                                                                            row[2] = "This test can only be taken in the assigned campus lab. Check with your instructor for specific locations";

                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        row[2] = "Take";
                                                                                        row["TestStatus"] = "1";
                                                                                    }

                                                                                }

                                                                                if (PreRequisiteCompleted == true) // PreRequisite completed
                                                                                {
                                                                                    row[2] = "Prerequisite(s) completed";
                                                                                    //row["TestStatus"] = "3";
                                                                                }
                                                                                else
                                                                                {
                                                                                    row[2] = "This assessment has prerequisite(s)";
                                                                                    //row["TestStatus"] = "1";
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (Convert.ToInt32(row["ActualAttempts"].ToString()) > 0)
                                                                                {
                                                                                    if (IsLabTest == "1" && hdnAccessFromLab == "0")
                                                                                    //Check lab assessment are accessed from lab or not)
                                                                                    {
                                                                                        //row["TestStatus"] = "Retake lab access";  //ShowLabAccessMsg
                                                                                        row["TestStatus"] = "6";
                                                                                        row[2] = "This test can only be taken in the assigned campus lab. Check with your instructor for specific locations";


                                                                                    }
                                                                                    else
                                                                                        //row["TestStatus"] = "Retake";
                                                                                        row["TestStatus"] = "6";
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (IsLabTest == "1")
                                                                                    {
                                                                                        if (hdnAccessFromLab == "0")
                                                                                        {
                                                                                            //Check lab assessment are accessed from lab or not)
                                                                                            //row["TestStatus"] = "actvie_labtest";   //ShowLabAccessMsg
                                                                                            row["TestStatus"] = "1";
                                                                                            row[2] = "This test can only be taken in the assigned campus lab. Check with your instructor for specific locations";

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            row[2] = "actvie_labtest";  //LOScoreTest
                                                                                            row["TestStatus"] = "1";
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        row["TestStatus"] = "1";   //LOScoreTest
                                                                                        row[2] = "Take";
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else if (row["TestType"].ToString() == "6") //Graded Practice Sheet
                                                                    {
                                                                        if (Convert.ToInt32(valPreReq) == 1)     // 1: Assessment has prerequisite
                                                                        {
                                                                            string PreReqResult = objTesPaper.CheckPreRequisite_M(row["intPkVal"].ToString(), UserId, CourseId, SectionId, Tz_Name);
                                                                            string[] arrPreReqResult = PreReqResult.Split('$');
                                                                            if (arrPreReqResult[3].ToString() == "0")
                                                                                PreRequisiteCompleted = false;
                                                                            else
                                                                                PreRequisiteCompleted = true;

                                                                            hdnTutorialExmpt = arrPreReqResult[0];
                                                                            hdnAssessmentExmpt = arrPreReqResult[1];
                                                                            hdnAttendanceExmpt = arrPreReqResult[2];

                                                                            if (hdnTutorialExmpt == "1" && hdnAssessmentExmpt == "1" && hdnAttendanceExmpt == "1")
                                                                            {
                                                                                valPreReq = "0";
                                                                            }
                                                                        }
                                                                        if (IsExpiredTerm)
                                                                        {
                                                                            row["TestStatus"] = "1";
                                                                            row[2] = "Take";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Convert.ToInt32(valPreReq) == 1)
                                                                            {
                                                                                if (PreRequisiteCompleted == true) // PreRequisite completed
                                                                                {
                                                                                    if (Convert.ToInt32(row["ActualAttempts"].ToString()) > 0)
                                                                                    {
                                                                                        if (IsLabTest == "1" && hdnAccessFromLab == "0")
                                                                                        {
                                                                                            //Check lab assessment are accessed from lab or not)
                                                                                            row["TestStatus"] = "6";  //ShowLabAccessMsg
                                                                                            row[2] = "This test can only be taken in the assigned campus lab. Check with your instructor for specific locations";

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            row["TestStatus"] = "6";
                                                                                            row[2] = "Retake";
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (IsLabTest == "1")
                                                                                        {
                                                                                            if (hdnAccessFromLab == "0")
                                                                                            {
                                                                                                row["TestStatus"] = "1";
                                                                                                row[2] = "This test can only be taken in the assigned campus lab. Check with your instructor for specific locations";

                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                row["TestStatus"] = "1";
                                                                                                row[2] = "This test can only be taken in the assigned campus lab. Check with your instructor for specific locations";

                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            row["TestStatus"] = "1";
                                                                                            row[2] = "Take";
                                                                                        }
                                                                                    }
                                                                                    row[2] = "Prerequisite(s) completed";
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (Convert.ToInt32(row["ActualAttempts"].ToString()) > 0)
                                                                                    {
                                                                                        if (IsLabTest == "1" && hdnAccessFromLab == "0")
                                                                                        {
                                                                                            //Check lab assessment are accessed from lab or not)
                                                                                            row["TestStatus"] = "6";
                                                                                            row[2] = "This test can only be taken in the assigned campus lab. Check with your instructor for specific locations";

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            row["TestStatus"] = "6";
                                                                                            row[2] = "Retake";
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (IsLabTest == "1")
                                                                                        {
                                                                                            if (hdnAccessFromLab == "0")
                                                                                            {
                                                                                                //Check lab assessment are accessed from lab or not)
                                                                                                {
                                                                                                    row["TestStatus"] = "1";  //ShowLabAccessMsg
                                                                                                    row[2] = "This test can only be taken in the assigned campus lab. Check with your instructor for specific locations";

                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                row["TestStatus"] = "1";
                                                                                                row[2] = "actvie_labtest";
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            row["TestStatus"] = "1";
                                                                                            row[2] = "Take";
                                                                                        }
                                                                                    }
                                                                                    row["TestStatus"] = row["TestStatus"];
                                                                                }
                                                                            }
                                                                            else
                                                                            {

                                                                                if (Convert.ToInt32(row["ActualAttempts"].ToString()) > 0)
                                                                                {
                                                                                    if (IsLabTest == "1" && hdnAccessFromLab == "0")
                                                                                    {
                                                                                        //Check lab assessment are accessed from lab or not)
                                                                                        row["TestStatus"] = "6";  //ShowLabAccessMsg
                                                                                        row[2] = "This test can only be taken in the assigned campus lab. Check with your instructor for specific locations";

                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        row["TestStatus"] = "6";  //ShowLabAccessMsg
                                                                                        row[2] = "This test can only be taken in the assigned campus lab. Check with your instructor for specific locations";
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (IsLabTest == "1")
                                                                                    {
                                                                                        if (hdnAccessFromLab == "0")        //Check lab assessment are accessed from lab or not)
                                                                                            row["TestStatus"] = "1";  //ShowLabAccessMsg
                                                                                        else
                                                                                            row["TestStatus"] = "1";  //ShowLabAccessMsg

                                                                                        row[2] = "This test can only be taken in the assigned campus lab. Check with your instructor for specific locations";

                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        row["TestStatus"] = "1";
                                                                                        row[2] = "Take";
                                                                                    }
                                                                                }
                                                                                row["TestStatus"] = row["TestStatus"];
                                                                            }
                                                                        }

                                                                    }//6 TYPE END
                                                                    else
                                                                    {       // Online assessment

                                                                        if (Convert.ToInt32(valPreReq) == 1)     // 1: Assessment has prerequisite
                                                                        {
                                                                            string PreReqResult = objTesPaper.CheckPreRequisite_M(row["intPkVal"].ToString(), UserId, CourseId, SectionId, Tz_Name);
                                                                            string[] arrPreReqResult = PreReqResult.Split('$');
                                                                            if (arrPreReqResult[3].ToString() == "0")
                                                                                PreRequisiteCompleted = false;
                                                                            else
                                                                                PreRequisiteCompleted = true;

                                                                            hdnTutorialExmpt = arrPreReqResult[0];
                                                                            hdnAssessmentExmpt = arrPreReqResult[1];
                                                                            hdnAttendanceExmpt = arrPreReqResult[2];

                                                                            if (hdnTutorialExmpt == "1" && hdnAssessmentExmpt == "1" && hdnAttendanceExmpt == "1")
                                                                            {

                                                                                valPreReq = "0";
                                                                            }
                                                                        }

                                                                        if (IsExpiredTerm)
                                                                        {
                                                                            row["TestStatus"] = "1";
                                                                            row[2] = "Take";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (Convert.ToInt32(row["ActualAttempts"].ToString()) > 0)
                                                                            {
                                                                                if (IsLabTest == "1" && hdnAccessFromLab == "0")
                                                                                {
                                                                                    //Check lab assessment are accessed from lab or not)
                                                                                    row["TestStatus"] = "6";  //ShowLabAccessMsg
                                                                                    row[2] = "This test can only be taken in the assigned campus lab. Check with your instructor for specific locations";

                                                                                }
                                                                                else
                                                                                {
                                                                                    if (row["TestSettings_MinReviewExePerScore"].ToString() != "0" && float.Parse(row["LastAttemptPerScore"].ToString()) < 100 && (float.Parse(row["TestSettings_MinReviewExePerScore"].ToString()) > float.Parse(row["MaxReviewPerScorObt"].ToString())))    // Check Review Attempt Enabled
                                                                                    {
                                                                                        //strTitle = "This assessment has review exercise. You should have score minimum " + row["TestSettings_MinReviewExePerScore"].ToString() + "% in this review exercise to take an additional attempt.";
                                                                                        row[2] = "review";
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (row["TestSettings_TestModeType"].ToString() == "2")    //practice test
                                                                                            row["TestStatus"] = "6";   //practice1.png
                                                                                        else
                                                                                            row["TestStatus"] = "6";   //InitTest
                                                                                        row[2] = "Retake";
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (IsLabTest == "1")
                                                                                {
                                                                                    if (hdnAccessFromLab == "0") //Check lab assessment are accessed from lab or not)
                                                                                        row["TestStatus"] = "1";   //practice1.png
                                                                                    else
                                                                                        row["TestStatus"] = "1";
                                                                                    row[2] = "Take";
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (row["TestSettings_TestModeType"].ToString() == "2")    //practice test
                                                                                        //    row["TestStatus"] = "practice";
                                                                                        //else
                                                                                        row["TestStatus"] = "1";
                                                                                    row[2] = "Take";
                                                                                }
                                                                            }
                                                                            if (Convert.ToInt32(valPreReq) == 1)
                                                                            {
                                                                                if (PreRequisiteCompleted == true) // PreRequisite completed
                                                                                {
                                                                                    row[2] = "Prerequisite(s) completed";
                                                                                }
                                                                                else
                                                                                {
                                                                                    row[2] = "This assessment has prerequisite(s)";
                                                                                }
                                                                            }
                                                                        }
                                                                    }//end online
                                                                }
                                                                else
                                                                {
                                                                    if (IsExpiredTerm)
                                                                    {
                                                                        row["TestStatus"] = "7";
                                                                        row[2] = "Resume";
                                                                    }
                                                                    else
                                                                    {
                                                                        if (IsLabTest == "1" && hdnAccessFromLab == "0")
                                                                        {
                                                                            row["TestStatus"] = "7";
                                                                            row[2] = "Resume";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (row["TestType"].ToString() == "6")
                                                                            {
                                                                                row["TestStatus"] = "7";
                                                                                row[2] = "Resume";
                                                                            }
                                                                            else if (row["PswdMode"].ToString() == "1" || row["PswdMode"].ToString() == "2")
                                                                            {
                                                                                bool IsEnablePwd = true;

                                                                                if (row["PswdMode"].ToString() == "1" && (row["GlobalPswd"].ToString() == "" || row["GlobalPswd"].ToString() == "&nbsp;"))
                                                                                {
                                                                                    IsEnablePwd = objTesPaper.CheckIsEnablePwd(row["intPkVal"].ToString(), UserId, SectionId);
                                                                                }

                                                                                if (IsEnablePwd)
                                                                                {
                                                                                    row["TestStatus"] = "7";
                                                                                    row[2] = "Resume";
                                                                                }
                                                                                else
                                                                                {
                                                                                    row["TestStatus"] = "7";
                                                                                    row[2] = "Resume";
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                row["TestStatus"] = "7";
                                                                                row[2] = "Resume";
                                                                            }
                                                                        }
                                                                    }

                                                                    //// Link the test name to test paper results page
                                                                    if ((row["LastAttemptedID"].ToString() != "0"))
                                                                    {
                                                                        // Check Display Results setting if it is 3 the result will be shown after selected date.
                                                                        if (row["TestResultMode"].ToString() == "3")
                                                                        {
                                                                            if (DateTime.Parse(strUserZoneCurrDate) >= DateTime.Parse(row["TestResultDate"].ToString()))
                                                                            {
                                                                                //strResultParam = "onclick=ViewResult('" + row["LastAttemptedID"].ToString() + "','" + row[1].ToString() + "','" + LObasedTest.ToString() + "','" + row[38].ToString() + "')";
                                                                                //strResult = "<img " + strResultParam + " src=\"../../images/result.png\" align=\"right\" title='Click here to view the results of previous attempt(s)' class = 'LinkNormal' />";
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (row["TestResultMode"].ToString() != "4")
                                                                            {
                                                                                if (row["TestType"].ToString() == "5" || row["TestType"].ToString() == "6")
                                                                                {
                                                                                    //strResultParam = "onclick=ViewLOResult('" + row[1].ToString() + "','" + Session["USERID"].ToString() + "','" + row["LastAttemptedID"].ToString() + "','" + row["TestType"].ToString() + "')";
                                                                                    //strResult = "<img " + strResultParam + " src=\"../../images/result.png\" align=\"right\" title='Click here to view the results of previous attempt(s)' class = 'LinkNormal' />";
                                                                                }
                                                                                else
                                                                                {
                                                                                    //strResultParam = "onclick=ViewResult('" + row["LastAttemptedID"].ToString() + "','" + row[1].ToString() + "','" + LObasedTest.ToString() + "','" + row[38].ToString() + "')";
                                                                                    //strResult = "<img " + strResultParam + " src=\"../../images/result.png\" align=\"right\" title='Click here to view the results of previous attempt(s)' class = 'LinkNormal' />";
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else if (row["LastAttemptedID"].ToString() == "0" && Convert.ToInt32(row["ActualAttempts"].ToString()) > 0)
                                                                    {
                                                                        if (row["TestResultMode"].ToString() != "4")
                                                                        {
                                                                            //strResultParam = "onclick=\"alert('This attempt was created by the instructor.')\"";
                                                                            //strResult = "<img " + strResultParam + " src=\"../../images/result.png\" align=\"right\" title='Click here to view the results of previous attempt(s)' class = 'LinkNormal' />";
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                            case "2":   // Inactive Assessment
                                                                row["TestStatus"] = "2";
                                                                break;
                                                            case "3":   // Completed Assessment
                                                                {
                                                                    row["TestStatus"] = "3";
                                                                    break;
                                                                }
                                                            case "4":   // Expired Assessment
                                                                if (row["Test_Penaltypercent"].ToString() == "0")
                                                                    row["TestStatus"] = "4";
                                                                break;
                                                            default:
                                                                row["TestStatus"] = "";
                                                                break;
                                                        }

                                                        //if ((row["MaxTestUserID"].ToString() != "0") && (row[10].ToString() != "2"))
                                                        // {
                                                        //     // Check Display Results setting if it is 3 the result will be shown after selected date.
                                                        //     if (row["TestResultMode"].ToString() == "3")
                                                        //     {
                                                        //         if (DateTime.Parse(strUserZoneCurrDate) >= DateTime.Parse(row["TestResultDate"].ToString()))
                                                        //         {
                                                        //             //strResultParam = "onclick=ViewResult('" + row[20].ToString() + "','" + row[1].ToString() + "','" + LObasedTest.ToString() + "','" + row[38].ToString() + "')";
                                                        //             //strResult = "<img " + strResultParam + " src=\"../../images/result.png\" align=\"right\" title='Click here to view the results of previous attempt(s)' class = 'LinkNormal' />";
                                                        //         }
                                                        //     }
                                                        //     else
                                                        //     {
                                                        //         if (row["TestResultMode"].ToString() != "4")
                                                        //         {
                                                        //             if (row["TestType"].ToString() == "5" || row["TestType"].ToString() == "6")
                                                        //             {
                                                        //                 //strResultParam = "onclick=ViewLOResult('" + row[1].ToString() + "','" + Session["USERID"].ToString() + "','" + row[20].ToString() + "','" + row["TestType"].ToString() + "')";
                                                        //                 //strResult = "<img " + strResultParam + " src=\"../../images/result.png\" align=\"right\" title='Click here to view the results of previous attempt(s)' class = 'LinkNormal' />";
                                                        //             }
                                                        //             else
                                                        //             {
                                                        //                 //strResultParam = "onclick=ViewResult('" + row[20].ToString() + "','" + row[1].ToString() + "','" + LObasedTest.ToString() + "','" + row[38].ToString() + "')";
                                                        //                 //strResult = "<img " + strResultParam + " src=\"../../images/result.png\" align=\"right\" title='Click here to view the results of previous attempt(s)' class = 'LinkNormal' />";
                                                        //             }
                                                        //         }
                                                        //     }
                                                        // }
                                                        //else if (row["MaxTestUserID"].ToString() == "0" && Convert.ToInt32(row["ActualAttempts"].ToString()) > 0)
                                                        //{
                                                        //    if (row["TestResultMode"].ToString() == "3")
                                                        //    {
                                                        //        if (DateTime.Now >= DateTime.Parse(row["TestResultDate"].ToString()))
                                                        //        {
                                                        //            //strResultParam = "onclick=ViewResult('" + row[20].ToString() + "','" + row[1].ToString() + "','" + LObasedTest.ToString() + "','" + row[38].ToString() + "')";
                                                        //            //strResult = "<img " + strResultParam + " src=\"../../images/result.png\" align=\"right\" title='Click here to view the results of previous attempt(s)' class = 'LinkNormal' />";
                                                        //        }
                                                        //    }
                                                        //    else
                                                        //    {

                                                        //        if (row["TestResultMode"].ToString() != "4")
                                                        //        {
                                                        //            //strResultParam = "onclick=\"alert('This attempt was created by the instructor.')\"";
                                                        //            //strResult = "<img " + strResultParam + " src=\"../../images/result.png\" align=\"right\" title='Click here to view the results of previous attempt(s)' class = 'LinkNormal' />";
                                                        //        }
                                                        //    }
                                                        //}

                                                        if (row["isExemptByDiagnoSticTest"] != null && row["isExemptByDiagnoSticTest"].ToString() == "1")
                                                        {
                                                            row["TestStatus"] = "4";
                                                            row[2] = "Expired";
                                                        }

                                                        //else   //END BLOCK request data
                                                        //{

                                                        //}

                                                        //if (row["TestGBCataegory"].ToString() == "&nbsp;")
                                                        //{
                                                        //    row["TestGBCataegory"].ToString()  "NA";
                                                        //}

                                                        if (row["TestType"].ToString() == "5")
                                                        {
                                                            row["MaxAttempts"] = row["MaxAttempts"];
                                                        }
                                                        else
                                                        {
                                                            if (row["MaxAttempts"].ToString() == "0")
                                                            {
                                                                row["MaxAttempts"] = "1";
                                                            }

                                                            //if (Convert.ToInt32(row["ActualAttempts"].ToString()) < Convert.ToInt32(row["MaxAttempts"].ToString()))
                                                            //{
                                                            //    row["MaxAttempts"] = row["ActualAttempts"].ToString() + " of " + row["MaxAttempts"].ToString();
                                                            //    if (row["TestType"].ToString() == "17")
                                                            //        attemptsleft = 1;
                                                            //}
                                                            //else
                                                            //    row["MaxAttempts"].ToString() = row["ActualAttempts"].ToString() + " of " + row["ActualAttempts"].ToString();

                                                        }

                                                        if (row["TestType"].ToString() == "3" || row["TestType"].ToString() == "5" || row["TestType"].ToString() == "6")
                                                        {
                                                            row["TestName"] = "";
                                                        }

                                                        //ECFDateTime dateFormat = new ECFDateTime();

                                                        //if (!row["TestResultMode"].ToString().Equals("") && !row["TestResultMode"].ToString().Equals("&nbsp;"))
                                                        //{
                                                        //    row["TestResultMode"] = dateFormat.getECFDateFormat(row["TestResultMode"].ToString(), true);
                                                        ////}

                                                        //if (row[26].ToString() != "&nbsp;" && row["CRModule_Name"].ToString() != "0")
                                                        //{
                                                        //    string AssTimeMsg = string.Empty;

                                                        //    string tottimeFirst = "0";
                                                        //    string tottimeSecond = "0";

                                                        //    tottimeFirst = row[26].ToString().Split('$')[0];
                                                        //    tottimeSecond = row[26].ToString().Split('$')[1];

                                                        //    AssTimeMsg = (Int32.Parse(tottimeFirst) / 3600).ToString() + ":" + ((Int32.Parse(tottimeFirst) % 3600) / 60).ToString() + ":" + ((Int32.Parse(tottimeFirst) % 3600) % 60).ToString() + "$" + (Int32.Parse(tottimeSecond) / 3600).ToString() + ":" + ((Int32.Parse(tottimeSecond) % 3600) / 60).ToString() + ":" + ((Int32.Parse(tottimeSecond) % 3600) % 60).ToString();

                                                        //}

                                                        //if (row[28].ToString() != "0")
                                                        //{
                                                        //    row[2].ToString() = row[2].ToString() + "Assessment assigned to " +
                                                        //                          row["HasPreRequisites"].ToString().Remove(0, 1);

                                                        //}

                                                        if (IsExpiredTerm)
                                                        {
                                                            row["TestStatus"] = "4";
                                                            row[2] = "Expired";
                                                        }

                                                        if (row["UserActiveModule_Status"].ToString() == "0" && row["TestModuleId"].ToString() == "-1" && row["TestType"].ToString() == "19" && HasDataInUserActiveModule == 0)  //Module Test is locked
                                                        {
                                                            row["TestStatus"] = "8";
                                                            row[2] = "Locked";
                                                        }
                                                        else if (row["UserActiveModule_Status"].ToString() == "0" && row["TestModuleId"].ToString() != "-1") //Module Test is locked
                                                        {
                                                            //Response.Write(row["TestType"].ToString());
                                                            if (ActivateOnlyCDT != 1)
                                                            {
                                                                if (row["TestType"].ToString() != "16")
                                                                {
                                                                    row["TestStatus"] = "8";
                                                                    row[2] = "Locked";
                                                                    if (MMPTExists != 0)
                                                                    {
                                                                        row[2] = "Take the module placement assessment to activate.";
                                                                    }
                                                                    else if ((row["UserActiveModule_Status"].ToString() == "0" || row["UserActiveModule_Status"].ToString() == "3") && row["TestType"].ToString() == "17")
                                                                    {
                                                                        if (row["UserActiveModule_Diagnostic"].ToString() == "0") //Diagnostic Status
                                                                        {
                                                                            row["TestStatus"] = "8";
                                                                            row[2] = "Take the module diagnostic assessment to activate.";
                                                                        }
                                                                    }
                                                                }
                                                                else if (row["TestType"].ToString() == "16")
                                                                {
                                                                    if (row["TestSettings_TestModeType"].ToString().Trim() != "2" || row["MMPT_ModuleId"].ToString().Trim() != ActivaModuleId)
                                                                    {
                                                                        if (row["MaxTestUserID"].ToString().Trim() != "0")
                                                                        {
                                                                            row["TestStatus"] = "3";
                                                                            row[2] = "Completed";
                                                                        }
                                                                        else
                                                                        {
                                                                            row["TestStatus"] = "9";
                                                                            row[2] = "This test is not applicable as module is already active";
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (row["TestType"].ToString() != "16")
                                                                {
                                                                    row["TestStatus"] = "8";
                                                                    row[2] = "Locked";
                                                                    if (MMPTExists != 0)
                                                                    {
                                                                        row[2] = "Take the module placement assessment to activate.";
                                                                    }
                                                                }
                                                                else if (row["TestType"].ToString() == "16" && row["MMPT_ModuleId"].ToString().Trim() != ActivaModuleId && ActivaModuleId != "0")
                                                                {
                                                                    if (Convert.ToInt16(row["Course_ModuleId"].ToString().Trim()) <= Convert.ToInt16(UserCurrentModuleId.Trim()))
                                                                    {
                                                                        if (row["MaxTestUserID"].ToString().Trim() != "0")
                                                                        {
                                                                            row["TestStatus"] = "3";
                                                                            row[2] = "Completed";
                                                                        }
                                                                        else
                                                                        {
                                                                            row["TestStatus"] = "9";
                                                                            row[2] = "This test is not applicable as module is already active";
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        row["TestStatus"] = "8";
                                                                        row[2] = "Locked";
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        //Module Test is completed 
                                                        else if ((row["UserActiveModule_Status"].ToString() == "1" && row["TestType"].ToString() != "17") || (row["UserActiveModule_Status"].ToString() == "1" && row["TestType"].ToString() == "17" && UserCurrentModuleId != row["CRModuleId"].ToString().Trim() && row["TestMaxScoreObt"].ToString().Trim() != "0" && row["TestModuleId"].ToString() != "-1"))
                                                        {
                                                            if (InstutionId == "56")//Check for 'Antelope Valley College'
                                                            {
                                                                if (row["UserActiveModule_Status"].ToString() == "1" && row["TestType"].ToString() != "12" && row["TestType"].ToString() != "13")
                                                                {
                                                                    row["TestStatus"] = "3";
                                                                    row[2] = "Completed";
                                                                }
                                                                else if (row["UserActiveModule_Status"].ToString() == "1" && (row["TestMaxScoreObt"].ToString() == "100" || row["ActualAttempts"].ToString() == "0") && (row["TestType"].ToString() == "12" || row["TestType"].ToString() == "13"))
                                                                {
                                                                    row["TestStatus"] = "3";
                                                                    row[2] = "Completed";
                                                                }

                                                                row["TestStatus"] = "10";//Improve
                                                                row[2] = "Improve";//

                                                            }
                                                            else
                                                            {
                                                                if (row["TestType"].ToString() == "17" && attemptsleft == 1)
                                                                {
                                                                    //Changes for summary test if needed
                                                                }
                                                                else
                                                                {
                                                                    row["TestStatus"] = "3";
                                                                    row[2] = "Completed";
                                                                }
                                                            }
                                                        }
                                                        else if ((row["UserActiveModule_Status"].ToString() == "1" || row["UserActiveModule_Status"].ToString() == "2") && row["TestType"].ToString() == "17" && UserCurrentModuleId != row["CRModuleId"].ToString().Trim() && row["TestMaxScoreObt"].ToString().Trim() == "0" && row["TestModuleId"].ToString() != "-1" && ActivateSummary != "1")
                                                            row[2] = "This test is not applicable for the current module";
                                                        //Module Test is  exempted  
                                                        else if (row["UserActiveModule_Status"].ToString() == "2" && row["TestType"].ToString() != "17")
                                                            if (row[51].ToString() == "-1")
                                                            {
                                                                row["TestStatus"] = "9";
                                                                row[2] = "Notapplicable";
                                                            }
                                                            else
                                                            {
                                                                row["TestStatus"] = "5";
                                                                row[2] = "Exempted";
                                                            }
                                                        //Module Diagnostic Test is active
                                                        else if ((row["UserActiveModule_Status"].ToString() == "0" || row["UserActiveModule_Status"].ToString() == "3") && row["TestType"].ToString() != "12")
                                                        {
                                                            if (row["UserActiveModule_Diagnostic"].ToString() == "0") //Diagnostic Status
                                                            {
                                                                row["TestStatus"] = "8";
                                                                row[2] = "Take the module diagnostic assessment to activate.";
                                                            }
                                                            if (row["UserActiveModule_Status"].ToString() == "3" && row["UserActiveModule_Diagnostic"].ToString() == "1" && row["TestType"].ToString() == "17") //Mastry Status
                                                            {
                                                                row["TestStatus"] = "8";
                                                                row[2] = "Take the module mastry assessment to activate.";
                                                            }
                                                            else if (row["TestType"].ToString() == "16")
                                                            {
                                                                if (row["MaxTestUserID"].ToString().Trim() != "0")
                                                                {
                                                                    row["TestStatus"] = "3";
                                                                    row[2] = "Completed";
                                                                }
                                                                else
                                                                    row[2] = "This test is not applicable as module is already active";
                                                            }
                                                        }
                                                        else if (row["UserActiveModule_Status"].ToString() == "3" && row["TestType"].ToString() == "12")   //Module Diagnostic Test is locked as MMPT is not taken 
                                                        {
                                                            if (ActivateOnlyCDT == 1 && row["UserActiveModule_Diagnostic"].ToString() != "1")
                                                            {
                                                                row["TestStatus"] = "8";
                                                                row[2] = "Take the module placement assessment to activate.";
                                                            }
                                                        }

                                                        if (row["IsExpmptLevelTest"].ToString() == "1" && row["TestType"].ToString() != "17")    //Module Test is exempted based on diagnostic test score
                                                        {
                                                            row["TestStatus"] = "5";
                                                            row[2] = "This assessment is exempted based on the performance in the diagnostic test";
                                                        }
                                                        //if (row[52].ToString().Trim() == "2") //Test_IsAutoPracticeTest
                                                        //{
                                                        //    row[2].ToString() = row[2].ToString() + "(p)";
                                                        //    //row[2].ToolTip = "'Practice assessment' (P)";
                                                        //}

                                                        //For MMT DrobBox Test
                                                        if (row["TestType"].ToString() == "18" && row["DropBoxId"].ToString().Trim() != "0")
                                                        {

                                                            if (row["SubmitStatus"].ToString().Trim() == "3")
                                                            {
                                                                row["TestMaxScoreObt"] = "6";
                                                                row[2] = "Click to retake the assignment";
                                                            }
                                                            else if (row["SubmitStatus"].ToString().Trim() == "2" && row["IsEval"].ToString().Trim() == "-1")
                                                            {
                                                                row[2] = "Edit/modify your saved assignment and submit";
                                                                //row[2] = row["TestMaxScoreObt"];
                                                            }
                                                        }

                                                        if (row["StudyPlanDuration"].ToString().Trim() != "" && Convert.ToInt32(row["StudyPlanDuration"].ToString().Trim()) > 0 && row["UserActiveModule_Status"].ToString() == "3")
                                                        {
                                                            if (row["MDTTakenDate"].ToString().Trim() != "" && row["MDTTakenDate"].ToString().Trim() != "&nbsp;")
                                                            {
                                                                DateTime dtmSPCompletion = ObjTimeZoneDateTime.GetUserTimeZoneDateTime(Tz_Name, DateTime.Parse(row["MDTTakenDate"].ToString().Trim()).AddDays(Convert.ToInt32(row["StudyPlanDuration"].ToString().Trim())));
                                                                DateTime dtmUserZonalTime = ObjTimeZoneDateTime.GetUserTimeZoneDateTime(Tz_Name, DateTime.Now);
                                                                if (dtmUserZonalTime.Date > dtmSPCompletion.Date)
                                                                {
                                                                    row["TestStatus"] = "8";
                                                                    row[2] = "Access is blocked as study plan completion date";
                                                                }
                                                            }
                                                        }
                                                        ECFDateTime dateFormat = new ECFDateTime();
                                                        if (!row["TestDate"].ToString().Equals("") && !row["TestDate"].ToString().Equals("&nbsp;"))
                                                        {
                                                            row["TestDate"] = dateFormat.getECFDateFormat(row["TestDate"].ToString(), true, CountryId);
                                                        }
                                                        if (!row["TestEndDate"].ToString().Equals("") && !row["TestEndDate"].ToString().Equals("&nbsp;"))
                                                        {
                                                            row["TestEndDate"] = dateFormat.getECFDateFormat(row["TestEndDate"].ToString(), true, CountryId);
                                                        }

                                                    }   //for loop block
                                                }
                                                //

                                                var userAssessmentList = dt.AsEnumerable().ToList();
                                                var result = (from dr in userAssessmentList
                                                              select new
                                                              {
                                                                  TestId = dr["intPkVal"],
                                                                  TestName = dr["TestName"],
                                                                  Test_IsAutoPracticeTest = dr["Test_IsAutoPracticeTest"],
                                                                  TestGBCataegory = dr["TestGBCataegory"],
                                                                  TestSettingsMode = dr["TestSettingsMode"],
                                                                  TestNumQues = dr["TestNumQues"],
                                                                  MaxAttempts = dr["MaxAttempts"],
                                                                  ActualAttempts = dr["ActualAttempts"],
                                                                  TestTimeAllotted = dr["TestTimeAllotted"],
                                                                  TestDate = dr["TestDate"].ToString(),
                                                                  TestEndDate = dr["TestEndDate"].ToString(),
                                                                  TestMaxScoreObt = dr["TestMaxScoreObt"],
                                                                  TestStatus = dr["TestStatus"],
                                                                  InitFlag = dr["InitFlag"],
                                                                  TestType = dr["TestType"],
                                                                  TestModuleId = dr["TestModuleId"],
                                                                  CRModule_Name = dr["CRModule_Name"],
                                                                  UserActiveModule_Status = dr["UserActiveModule_Status"],
                                                                  Test_IsLabTest = dr["Test_IsLabTest"],
                                                                  Test_ModulePercent = dr["Test_ModulePercent"],
                                                                  UserActiveModule_Diagnostic = dr["UserActiveModule_Diagnostic"],
                                                                  IsExpmptLevelTest = dr["IsExpmptLevelTest"],
                                                                  CRModuleId = dr["CRModuleId"],
                                                                  Course_ModuleId = dr["Course_ModuleId"],
                                                                  MMPT_ModuleId = dr["MMPT_ModuleId"],
                                                                  Test_AutoPracticeTest = dr["Test_AutoPracticeTest"],
                                                                  StudyPlanDuration = dr["StudyPlanDuration"],
                                                                  UserActiveModule_TestScore = dr["UserActiveModule_TestScore"],
                                                                  SubmitStatus = dr["SubmitStatus"],
                                                                  GlobalPswd = dr["GlobalPswd"],
                                                                  TestSettings_TestModeType = dr["TestSettings_TestModeType"],
                                                                  isExemptByDiagnoSticTest = dr["isExemptByDiagnoSticTest"],
                                                                  Test_Penaltypercent = dr["Test_Penaltypercent"],
                                                                  Test_HasPreRequisites = dr["HasPreRequisites"],
                                                                  Test_BestScoreUserId = dr["LastAttemptedID"],
                                                                  Test_LastAttemptedUserId = dr["MaxTestUserID"],
                                                                  TestPaperResultUrl = dr["MaxTestUserID"].ToString()=="0"?"":strVFilePathUrl + dr["MaxTestUserID"] + "&IsReview=0&SectionId=" + SectionId + "&UserID=" + UserId + "&USERTYPE=ST&TestId=" + dr["intPkVal"].ToString()+ "&TestUserID=0",
                                                                  TestStatus_Desc = dr[2],
                                                              }).ToList();

                                                var resMessage = new
                                                {
                                                    status = "1",
                                                    message = "Successfully retrieved assessments information",
                                                    response = result,
                                                };
                                                return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200      
                                            }
                                            else
                                            {
                                                var resMessage = new
                                                {
                                                    status = "0",
                                                    message = "No data found."
                                                };
                                                return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                                            }
                                        }
                                        else
                                        {
                                            var resMessage = new
                                            {
                                                status = "0",
                                                message = "No data found."
                                            };
                                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                                        }
                                    }
                                    #region      //regular courses
                                    else
                                    {
                                        spParam.Length = 0;
                                        spParam.Append("1").Append(colSeperator).Append(UserId).Append(colSeperator);
                                        spParam.Append("2").Append(colSeperator).Append(CourseId).Append(colSeperator);
                                        spParam.Append("3").Append(colSeperator).Append("0").Append(colSeperator);
                                        spParam.Append("4").Append(colSeperator).Append(SectionId).Append(colSeperator);
                                        spParam.Append("5").Append(colSeperator).Append(DateTime.Parse(strUserZoneCurrDate)).Append(colSeperator);
                                        spParam.Append("6").Append(colSeperator).Append(AssessmentType);

                                        dst = null;

                                        dst = objAssmts.LoadTestPapers(spParam.ToString());

                                        if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                                        {
                                            //added Start Block
                                            for (int i = 0; i < dst.Tables[0].Rows.Count; i++)
                                            {
                                                if (dst.Tables[0].Rows[i]["TestType"].ToString() == "22" && dst.Tables[0].Rows[i]["TestSettings_TestModeType"].ToString() == "1" && dst.Tables[0].Rows[i]["MaxTestUserId"].ToString() == "0")
                                                {
                                                    if (DateTime.Parse(strUserZoneCurrDate) >= DateTime.Parse(dst.Tables[0].Rows[i][13].ToString()))
                                                        IsUDTtaken = true;
                                                    else
                                                        IsUDTtaken = false;
                                                    UnitModuleId = dst.Tables[0].Rows[i]["Test_ModuleId"].ToString();
                                                }

                                                if (dst.Tables[0].Rows[i]["PenaltyExpDate"].ToString() != "")
                                                {
                                                    if (DateTime.Parse(strUserZoneCurrDate) > DateTime.Parse(dst.Tables[0].Rows[i]["PenaltyExpDate"].ToString()))
                                                    {
                                                        IsPenaltyDateExpired = true;
                                                    }
                                                    else
                                                    {
                                                        IsPenaltyDateExpired = false;
                                                    }
                                                }

                                                if (TzID != dst.Tables[0].Rows[i][24].ToString())
                                                {
                                                    dst.Tables[0].Rows[i][5] = ObjTimeZoneDateTime.GetUserTimeZoneDateTime(Tz_Name, DateTime.Parse(dst.Tables[0].Rows[i][5].ToString()));
                                                    dst.Tables[0].Rows[i][13] = ObjTimeZoneDateTime.GetUserTimeZoneDateTime(Tz_Name, DateTime.Parse(dst.Tables[0].Rows[i][13].ToString()));
                                                }
                                                if (dst.Tables[0].Rows[i][10].ToString() == "3")
                                                {
                                                    dst.Tables[0].Rows[i][11] = ObjTimeZoneDateTime.GetUserTimeZoneDateTime(Tz_Name, DateTime.Parse(dst.Tables[0].Rows[i][11].ToString()));
                                                }
                                                if (dst.Tables[0].Rows[i][34].ToString() != "0" && DateTime.Parse(strUserZoneCurrDate) >= DateTime.Parse(dst.Tables[0].Rows[i][13].ToString()) && IsPenaltyDateExpired == false)
                                                {
                                                    blnbypassexp = true;
                                                }


                                                if (blnbypassexp == false)
                                                {
                                                    if (dst.Tables.Count > 3 && dst.Tables[3] != null && dst.Tables[3].Rows.Count > 0
                                                        && dst.Tables[3].Rows[0]["IsCoRequisiteCourse"].ToString() != "1")
                                                    {
                                                        if (dst.Tables[0].Rows[i]["ZS_Applicable"].ToString() != "1")
                                                        {
                                                            //dst.Tables[0].Rows[i][34] = "0";    
                                                            if (DateTime.Parse(strUserZoneCurrDate) >= DateTime.Parse(dst.Tables[0].Rows[i][13].ToString()))
                                                            {
                                                                dst.Tables[0].Rows[i][20] = "4"; //Test Expired
                                                                if (dst.Tables[0].Rows[i][15].ToString() == "2")
                                                                {
                                                                    //concatenating the testids for updating the status flags
                                                                    Testids = Testids.Length > 0 ? Testids + "," + dst.Tables[0].Rows[i][3].ToString() : dst.Tables[0].Rows[i][3].ToString();
                                                                    dst.Tables[0].Rows[i][7] = Convert.ToInt32(dst.Tables[0].Rows[i][7].ToString()) + 1;
                                                                }
                                                            }
                                                            else if (DateTime.Parse(strUserZoneCurrDate) < DateTime.Parse(dst.Tables[0].Rows[i][5].ToString()))
                                                            {
                                                                dst.Tables[0].Rows[i][20] = "2";    //Test Inactive.
                                                            }
                                                            else if ((DateTime.Parse(strUserZoneCurrDate) >= DateTime.Parse(dst.Tables[0].Rows[i][5].ToString())) && (DateTime.Parse(strUserZoneCurrDate) < DateTime.Parse(dst.Tables[0].Rows[i][13].ToString())))
                                                            {
                                                                if (Convert.ToInt32(dst.Tables[0].Rows[i][7].ToString()) >= Convert.ToInt32(dst.Tables[0].Rows[i][6].ToString()))
                                                                {
                                                                    dst.Tables[0].Rows[i][20] = "3";    //Test Completed.
                                                                }
                                                                else
                                                                {
                                                                    dst.Tables[0].Rows[i][20] = "1"; //Take test.
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (dst.Tables.Count > 3 && dst.Tables[3] != null && dst.Tables[3].Rows.Count > 0
                                                        && dst.Tables[3].Rows[0]["IsCoRequisiteCourse"].ToString() != "1")
                                                    {
                                                        if (dst.Tables[0].Rows[i]["ZS_Applicable"].ToString() != "1")
                                                        {
                                                            if (DateTime.Parse(strUserZoneCurrDate) < DateTime.Parse(dst.Tables[0].Rows[i][5].ToString()))
                                                            {
                                                                dst.Tables[0].Rows[i][20] = "2";    //Test Inactive.
                                                            }
                                                            //  else if ((DateTime.Parse(strUserZoneCurrDate) >= DateTime.Parse(dst.Tables[0].Rows[i][5].ToString())) && (DateTime.Parse(strUserZoneCurrDate) < DateTime.Parse(dst.Tables[0].Rows[i][13].ToString())))
                                                            //  {
                                                            else if (Convert.ToInt32(dst.Tables[0].Rows[i][7].ToString()) >= Convert.ToInt32(dst.Tables[0].Rows[i][6].ToString()))
                                                            {
                                                                dst.Tables[0].Rows[i][20] = "3";    //Test Completed.
                                                            }
                                                            else
                                                            {
                                                                dst.Tables[0].Rows[i][20] = "1"; //Take test.
                                                            }
                                                        }
                                                    }
                                                }

                                                //Begin--Modified for Co-Requisite Course Flow--(Indra)
                                                //TestStatus updating for ZS Auto Test, ZS Mastery Test and Module level Test.

                                                if (dst.Tables[0].Rows[i]["ZS_Applicable"].ToString() == "1")
                                                {
                                                    if (dst.Tables[0].Rows[i][20].ToString() == "1")
                                                    {
                                                        if (dst.Tables[0].Rows[i]["TestType"].ToString() == "30" || dst.Tables[0].Rows[i]["TestType"].ToString() == "28")
                                                        {
                                                            int InitFlag = 0;
                                                            DataRow[] drZSDT = dst.Tables[0].Select("TestType = 27 AND Test_IsAutoPracticeTest <> 2 AND Test_ModuleId = " + dst.Tables[0].Rows[i]["Test_ModuleId"].ToString());
                                                            try
                                                            {
                                                                if (drZSDT != null && drZSDT.Count() > 0)
                                                                    if (drZSDT[0]["InitFlag"] != null && drZSDT[0]["InitFlag"].ToString() == "1")
                                                                        InitFlag = 1;
                                                            }
                                                            catch (Exception ex)
                                                            {

                                                            }

                                                            if (dst.Tables[0].Rows[i]["ZSDT_PS_Achieved"].ToString() == "1")
                                                                dst.Tables[0].Rows[i][20] = "5";    //Test Exempted.

                                                            if (drZSDT != null && drZSDT.Count() > 0)
                                                            {
                                                                if (dst.Tables[0].Rows[i]["ZSDT_PS_Achieved"].ToString() == "0")
                                                                    if (InitFlag == 1)
                                                                        dst.Tables[0].Rows[i][20] = "1";    //Test Active.
                                                                    else
                                                                        dst.Tables[0].Rows[i][20] = "2";    //Test InActive.
                                                            }
                                                        }

                                                        if (dst.Tables[0].Rows[i]["TestType"].ToString() != "27" && dst.Tables[0].Rows[i]["TestType"].ToString() != "30" && dst.Tables[0].Rows[i]["TestType"].ToString() != "28")
                                                        {
                                                            if (dst.Tables[0].Rows[i]["CPDT_PS_Achieved"].ToString() == "0")
                                                            {
                                                                if (dst.Tables[0].Rows[i]["ZSDT_PS_Achieved"].ToString() == "1")
                                                                    dst.Tables[0].Rows[i][20] = "1";    //Test Active.
                                                                else
                                                                {
                                                                    if (dst.Tables[0].Rows[i]["ZSMT_PS_Achieved"].ToString() == "1")
                                                                        dst.Tables[0].Rows[i][20] = "1";    //Test Active.
                                                                    else
                                                                        dst.Tables[0].Rows[i][20] = "2";    //Test Inactive.
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                //End--Modified for Co-Requisite Course Flow--(Indra)

                                                if (DateTime.Parse(dst.Tables[0].Rows[i][56].ToString()) > DateTime.Parse("2000-01-01 00:00:00.000") && dst.Tables[0].Rows[i][54].ToString().Trim() != "0")
                                                {
                                                    if (DateTime.Parse(dst.Tables[0].Rows[i][56].ToString()) < DateTime.Parse(strUserZoneCurrDate).Date)
                                                        dst.Tables[0].Rows[i][50] = "-1";
                                                }

                                                //if (IsUDTtaken == false && ActiveModules != "0" && ActiveModules.IndexOf(dst.Tables[0].Rows[i]["Test_ModuleId"].ToString()) > -1 && dst.Tables[0].Rows[i]["TestType"].ToString() != "22")
                                                if (IsUDTtaken == false && ActiveModules != "0" && UnitModuleId == dst.Tables[0].Rows[i]["Test_ModuleId"].ToString() && dst.Tables[0].Rows[i]["TestType"].ToString() != "22")
                                                    dst.Tables[0].Rows[i]["UnitAssignmentsLocked"] = "1";

                                                //Enable pre-req icon for HW if auto LO pre-req exists
                                                if (IsUnitBased == 1 && dst.Tables[0].Rows[i]["TestType"].ToString() == "1" && dst.Tables[0].Rows[i]["TestSettings_TestModeType"].ToString() == "3")
                                                {
                                                    if (dst.Tables.Count > 2)
                                                    {
                                                        if (dst.Tables[2].Rows.Count > 0)
                                                        {
                                                            if (dst.Tables[2].Rows[0][0].ToString() == "0")
                                                            {
                                                                if (dst.Tables[2].Rows[0]["AutoHWList"].ToString().Contains("," + dst.Tables[0].Rows[i]["intPkVal"].ToString() + ","))
                                                                {
                                                                    if (dst.Tables[0].Rows[i]["HasPreRequisites"].ToString().Trim() == "0")
                                                                        dst.Tables[0].Rows[i]["HasPreRequisites"] = "1";
                                                                }
                                                            }
                                                        }
                                                    }
                                                }


                                                ECFDateTime dateFormat = new ECFDateTime();

                                                if (!dst.Tables[0].Rows[i][5].ToString().Equals("") && !dst.Tables[0].Rows[i][5].ToString().Equals("&nbsp;"))
                                                {

                                                    dst.Tables[0].Rows[i][5] = dateFormat.getECFDateFormat(dst.Tables[0].Rows[i][5].ToString(), true, CountryId);
                                                }

                                                if (!dst.Tables[0].Rows[i][13].ToString().Equals("") && !dst.Tables[0].Rows[i][13].ToString().Equals("&nbsp;"))
                                                {
                                                    dst.Tables[0].Rows[i][13] = dateFormat.getECFDateFormat(dst.Tables[0].Rows[i][13].ToString(), true, CountryId);
                                                }

                                                blnbypassexp = false;
                                            }

                                            if (Testids.Length > 0)
                                            {
                                                spParam.Length = 0;
                                                spParam.Append("1").Append(colSeperator).Append(Testids).Append(colSeperator);
                                                spParam.Append("2").Append(colSeperator).Append(UserId);
                                                objAssmts.UpdateStatusFlags(spParam.ToString());

                                            }

                                            //End Block

                                            DataRow[] dtr = dst.Tables[0].Select("ISNULL(IsStudyPlanAttempted,0)<>-1");

                                            var userAssessmentList = dtr.AsEnumerable().ToList();

                                            var result = (from dr in userAssessmentList
                                                          select new
                                                          {
                                                              TestId = dr["intPkVal"],
                                                              TestName = dr["TestName"],
                                                              Test_IsAutoPracticeTest = dr["Test_IsAutoPracticeTest"],
                                                              TestGBCataegory = dr["TestGBCataegory"],
                                                              TestSettingsMode = dr["TestSettingsMode"],
                                                              TestNumQues = dr["TestNumQues"],
                                                              MaxAttempts = dr["MaxAttempts"],
                                                              ActualAttempts = dr["ActualAttempts"],
                                                              TestTimeAllotted = dr["TestTimeAllotted"],
                                                              TestDate = dr["TestDate"].ToString(),
                                                              TestEndDate = dr["TestEndDate"].ToString(),
                                                              TestMaxScoreObt = dr["TestMaxScoreObt"],
                                                              TestStatus = dr["TestStatus"],
                                                              InitFlag = dr["InitFlag"],
                                                              TestType = dr["TestType"],
                                                              Test_HasPreRequisites = dr["HasPreRequisites"],
                                                              Test_BestScoreUserId = dr["LastAttemptedID"],
                                                              Test_LastAttemptedUserId = dr["MaxTestUserID"],
                                                              TestPaperResultUrl = dr["MaxTestUserID"].ToString()=="0"?"":strVFilePathUrl + dr["MaxTestUserID"] + "&IsReview=0&SectionId=" + SectionId + "&UserID=" + UserId + "&USERTYPE=ST&TestId=" + dr["intPkVal"].ToString() + "&TestUserID=0",
                                                          }).ToList();

                                            var resMessage = new
                                            {
                                                status = "1",
                                                message = "Successfully retrieved assessments information",
                                                response = result,
                                            };
                                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200            
                                        }
                                        else
                                        {
                                            var resMessage = new
                                            {
                                                status = "0",
                                                message = "No data found."
                                            };
                                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                                        }
                                    }// end block of IsDevCourse
                                    #endregion
                                }
                                else
                                {
                                    var resMessage = new
                                    {
                                        status = "0",
                                        message = "No data found."
                                    };
                                    return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                                }
                            }//asss
                            else
                            {
                                var resMessage = new
                                {
                                    status = "0",
                                    message = "UserId does not exit."
                                };
                                return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                            }
                        }
                        else
                        {
                            var resMessage = new
                            {
                                status = "0",
                                message = "No data found."
                            };
                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                        }
                    }
                    catch (Exception ex)
                    {
                        var resMessage = new
                        {
                            status = "0",
                            message = ex.Message.ToString()
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                    }
                }
                else
                {
                    var resMessage = new
                    {
                        status = "0",
                        message = "Assessments info retrieval credentials are required"
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                }
            }
            else
            {
                var resMessage = new
                {
                    status = "0",
                    message = "Invalid/malformed input(Assessments credential)"
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400                 
            }

            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while retrieving assessments information."
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201
        }


        /// <summary>
        ///This api call takes InitTestPage Credential i.e., 'TestId', 'UserId', 'SectionId', 'CourseId' and 
        ///return Assessment list details like 'TestId','TestName','TestHeadName','TestDate','TestEndDate','TotalQues','TotalMarks','TestType','Test_HasPreRequisite','Test_IsComp','Test_ModuleId','TotalTime',TestDate,DispRes','ResDate','IsAutoPracticeTest','NoAtmpts','NegMrks','Penaltypercent',
        ///'PenaltyExpDate','DynDispFlag','DynDispMode','ModulePercent','IsStudyPlan','PreReqLOsInfo','PreReqAttachedAssessmentInfo','PreqAttendancePercentInfo','PreReqVideosInfo' with 'status' and 'message' upon successful retrieval of assessment information.
        ///Incase of failure, it returns only 'status' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in retrieving assessments information.
        ///Note:
        ///1)'PreReqLOsInfo' return list details like 'TestId','TestPreReqId','TestPreReq_LevelID','TestPreReq_TimeAlloted','TestPreReq_LevelParent','TestPreReq_LevelLo','CRTimeSpent_TimeSpentMin','CourseHirarchy', 'FilePath'.
        ///2)'PreReqVideosInfo' return list details like 'TestId','TestPrevideoID','TestPreReqVideo_uploadID','TestPreReqVideo_MinTime','TestPreReqVideo_FileName','TestPreReqVideo_TimeSpent','TestPreReqVideo_VideoType','IsAdminVideo','FilePath'.
        ///3)'PreqAttendancePercentInfo' return list details like 'TestId','Min_AttendancePercent','TAttendancePercentObtain','TAttenPreReqTestAtm'.
        ///4)'PreReqAttachedAssessmentInfo' return list details like 'TestId','TestPreAssID','TestPreAss_PreReqTestId','TestPreAss_MaxScore','TotalMarksObtained','TotalScoreForPreReq','PreRequistitTestName','TotalQuestForMainAss',
        /// 'TestUserID','PreReqTotQuesWithSolVarient','preReqMaxAttempt','preReqAttemptCount','TestEndDate','Test_Type','Test_Penaltypercent','Test_PreReqGroup','IsLabTest','IsEnableLockdownBrowser'
        /// </summary>
        /// <param name="InitPageCredential">Assessments retrieval credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": 1,
        ///   "message": "Succesfully retrieved assessments information.",
        ///   "response": 
        ///      {  
        ///       "TestId" : "",
        ///       "TestName":"",
        ///       "TestHeadName" : "",
        ///       "TestType" : "",
        ///       "TestDate" : "",
        ///       "TestEndDate:"",
        ///       "TotalQues" : "",
        ///       "TotalMarks":"",
        ///       "Test_HasPreRequisite" : "",
        ///       "Test_IsComp" : "",
        ///       "TotalTime" : "",
        ///       "TestDate" : "",
        ///       "DispRes" : "",
        ///       "ResDate" : "",
        ///       "IsPracticeTest":"",
        ///       "NoAtmpts":"", 
        ///       "NegMrks":"",
        ///       "Penaltypercent ":"",
        ///       "PenaltyExpDate ":"",
        ///       "DynDispFlag ":"",
        ///       "DynDispMode ":"",
        ///       "ModulePercent ":"",
        ///       "IsStudyPlan ":"",
        ///       "IsEnableLockdownBrowser":"",
        ///             
        ///       "PreReqLOsInfo":"",
        ///       "PreReqAttachedAssessmentInfo":"",
        ///       "PreqAttendancePercentInfo":"",
        ///       "PreReqVideosInfo":"",
        ///   }
        ///Error:
        ///{
        ///"status": 0,
        ///"message": "User does not have access to section/Error while retrieving assessment information."
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("GetInitTestPaperInfo")]
        public HttpResponseMessage GetAssessmentList([ModelBinder] InitTestPaperCredential InitPageCredential)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(InitPageCredential.TestId) && !string.IsNullOrEmpty(InitPageCredential.UserId) &&
                    !string.IsNullOrEmpty(InitPageCredential.CourseId) && !string.IsNullOrEmpty(InitPageCredential.SectionId))
                {
                    try
                    {
                        Api_TestPaper objAssmts = new Api_TestPaper();
                        PGCC_TestPaper ObjPGCCTestPaper = null;
                        char colSeperator = (char)195;
                        DataSet dst, dtCourse, dsTestPaperInfo;
                        DateTime dtCurrentDate;
                        TimeZoneDateTime ObjTimeZoneDateTime = null;
                        string Testids = string.Empty;
                        UserCourse objUserCourse = null;
                        string IsCourseDev = "";
                        StringBuilder spParam = new StringBuilder();
                        string UserId = "", SectionId = "", TestId = "", CourseId = "", Tz_Name = "", TzID = "";
                        string InstutionId = "0";
                        string CountryId = "2";
                        string strUserZoneCurrDate = string.Empty;
                        double AttendancePercent = 0;
                        string TestType = string.Empty;
                        TestPaper objTestPaper = null;
                        UserId = InitPageCredential.UserId.Trim();
                        SectionId = InitPageCredential.SectionId.Trim();
                        CourseId = InitPageCredential.CourseId.Trim();
                        TestId = InitPageCredential.TestId.Trim();

                        User objUser = new User();
                        dst = objUser.GetUserInfo(UserId);

                        if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(dst.Tables[0].Rows[0][28])))
                            {
                                InstutionId = dst.Tables[0].Rows[0][28].ToString();

                            }
                            CountryId = dst.Tables[0].Rows[0]["Users_Country"].ToString();

                            objUserCourse = new UserCourse();
                            dtCourse = objUserCourse.GetCourseLevelInfo(CourseId);

                            if (dtCourse != null && dtCourse.Tables[0].Rows[0][0].ToString() == "0" && dtCourse.Tables[0].Rows.Count > 0)
                            {
                                IsCourseDev = dtCourse.Tables[0].Rows[0]["Course_ISDev"].ToString();

                                Tz_Name = dst.Tables[0].Rows[0]["Tz_Name"].ToString();
                                TzID = dst.Tables[0].Rows[0]["Resource_TzId"].ToString();
                                dtCurrentDate = DateTime.Now;
                                ObjTimeZoneDateTime = new TimeZoneDateTime();
                                strUserZoneCurrDate = ObjTimeZoneDateTime.GetUserTimeZoneDateTime(Tz_Name, dtCurrentDate).ToString();

                                var strPreReqLOsInfo = new List<string>();
                                var strPreReqAttachedAssessmentInfo = new List<string>();
                                var strPreqAttendancePercentInfo = new List<string>();
                                var strPreReqVideosInfo = new List<string>();
                                string VFilePath = string.Empty;

                                if (IsCourseDev == "2")
                                {

                                    string callType = "1";

                                    spParam.Length = 0;
                                    spParam.Append("1").Append(colSeperator).Append(TestId).Append(colSeperator);
                                    spParam.Append("2").Append(colSeperator).Append(UserId).Append(colSeperator);
                                    spParam.Append("3").Append(colSeperator).Append(SectionId);

                                    dsTestPaperInfo = null;
                                    ObjPGCCTestPaper = new PGCC_TestPaper();
                                    dsTestPaperInfo = ObjPGCCTestPaper.GetTestInfo_M(spParam.ToString());

                                    if (dsTestPaperInfo.Tables[0].Rows[0][0].ToString() == "0" && dsTestPaperInfo.Tables[0].Rows.Count > 0)
                                    {
                                        List<DataRow> listTestPaperInfo = null;
                                        List<DataRow> listTestPaperLoInfo = null;
                                        List<DataRow> listTestPaperUserAttandanceInfo = null;
                                        List<DataRow> listTestPaperAttachedAssInfo = null;
                                        dst = null;
                                        listTestPaperInfo = dsTestPaperInfo.Tables[0].AsEnumerable().ToList();

                                        if (dsTestPaperInfo.Tables.Count > 6)
                                        {
                                            if (dsTestPaperInfo.Tables[6].Rows[0][0].ToString() == "0" && dsTestPaperInfo.Tables[6].Rows.Count > 0)
                                            {
                                                if (dsTestPaperInfo.Tables[6].Rows[0]["Section_AttendanceXMLGuidFileName"].ToString().Trim() != "")
                                                {
                                                    if (HttpContext.Current.Server.MapPath(ConfigurationSettings.AppSettings["ATTENDANCEPRECAL"]) != null)
                                                    {
                                                        //callType = HttpContext.Current.Server.MapPath(ConfigurationSettings.AppSettings["ATTENDANCEPRECAL"].ToString());
                                                        //if (callType.Trim().ToString() == "2")
                                                        //    AttendancePercent = objTestPaper.GetUserAttendance(SectionId, UserId,
                                                        //                                              dsTestPaperInfo.Tables[6].Rows[0][
                                                        //                                                  "UserActiveModule_ActiveDate"]
                                                        //                                                  .ToString(),
                                                        //                                              dsTestPaperInfo.Tables[6].Rows[0][
                                                        //                                                  "Section_AttendanceXMLGuidFileName"
                                                        //                                                  ].ToString(),Tz_Name);
                                                        //else
                                                        AttendancePercent =
                                                            objTestPaper.GetUserAttendancePercent(SectionId, UserId,
                                                                                     dsTestPaperInfo.Tables[6].Rows[0][
                                                                                         "Section_AttendanceXMLGuidFileName"]
                                                                                         .ToString());
                                                    }
                                                    else
                                                    {
                                                        AttendancePercent = objTestPaper.GetUserAttendancePercent(
                                                            SectionId, UserId,
                                                            dsTestPaperInfo.Tables[6].Rows[0]["Section_AttendanceXMLGuidFileName"]
                                                                .ToString());
                                                    }
                                                }

                                            }
                                        }


                                        spParam.Length = 0;
                                        spParam.Append("1").Append(colSeperator).Append(TestId).Append(colSeperator);
                                        spParam.Append("2").Append(colSeperator).Append("1").Append(colSeperator);
                                        spParam.Append("3").Append(colSeperator).Append(UserId).Append(colSeperator);
                                        spParam.Append("4").Append(colSeperator).Append(CourseId).Append(colSeperator);
                                        spParam.Append("5").Append(colSeperator).Append(SectionId);

                                        dst = ObjPGCCTestPaper.GetPreRequisitesInfo_M(spParam.ToString());

                                        //
                                        if ((dst.Tables[0].Rows[0][0].ToString() == "0" && dst.Tables[0].Rows.Count > 0) ||
                                            (dst.Tables[1].Rows[0][0].ToString() == "0" && dst.Tables[1].Rows.Count > 0) ||
                                            (dst.Tables[2].Rows[0][0].ToString() == "0" && dst.Tables[2].Rows.Count > 0) ||
                                            (dst.Tables[3].Rows[0][0].ToString() == "0" && dst.Tables[3].Rows.Count > 0) ||
                                            (dst.Tables[4].Rows[0][0].ToString() == "0" && dst.Tables[4].Rows.Count > 0))
                                        {
                                            if (dst.Tables[0].Rows[0][0].ToString() == "0" && dst.Tables[0].Rows.Count > 0)
                                            {
                                                listTestPaperLoInfo = dst.Tables[0].AsEnumerable().ToList();
                                            }

                                            if (dst.Tables[1].Rows[0][0].ToString() == "0" && dst.Tables[1].Rows.Count > 0)
                                            {
                                                listTestPaperAttachedAssInfo = dst.Tables[1].AsEnumerable().ToList();
                                            }

                                            if (dst.Tables[2].Rows[0][0].ToString() == "0" && dst.Tables[2].Rows.Count > 0)
                                            {
                                                listTestPaperUserAttandanceInfo = dst.Tables[2].AsEnumerable().ToList();
                                            }

                                            List<TestPaperLoInfo> listTestPaperLosInfo = new List<TestPaperLoInfo>();
                                            if (listTestPaperLoInfo != null && listTestPaperLoInfo.Count > 0)
                                            {
                                                Educosoft.Api.Utilities.FileUpload uploadObj;
                                                uploadObj = new Educosoft.Api.Utilities.FileUpload();
                                                dst = null;
                                                spParam = new StringBuilder();
                                                spParam.Length = 0;
                                                spParam.Append("1").Append(colSeperator).Append("FP5");
                                                dst = uploadObj.GetVirtualPath(spParam.ToString());

                                                if (dst.Tables[0].Rows[0].ItemArray[0].ToString() == "0")
                                                {
                                                    VFilePath = "";
                                                    VFilePath = dst.Tables[0].Rows[0].ItemArray[4].ToString();
                                                }

                                                UserCourse userCourse = new UserCourse();
                                                foreach (DataRow dr in listTestPaperLoInfo)
                                                {
                                                    listTestPaperLosInfo.Add(new TestPaperLoInfo
                                                    {
                                                        TestId = dr["TestPreReq_TestID"].ToString(),
                                                        TestPreReqId = dr["intPk"].ToString(),
                                                        TestPreReq_LevelID = dr["TestPreReq_LevelID"].ToString(),
                                                        TestPreReq_TimeAlloted = dr["TestPreReq_TimeAlloted"].ToString(),
                                                        TestPreReq_LevelParent = dr["TestPreReq_LevelParent"].ToString(),
                                                        TestPreReq_LevelLo = dr["TestPreReq_LevelLo"].ToString(),
                                                        CRTimeSpent_TimeSpentMin = Convert.ToDouble(Convert.ToInt32(dr["CRTimeSpent_TimeSpent"].ToString()) / 60).ToString(),
                                                        CourseHirarchy = dr["LevelParentName"].ToString() + " > " + dr["LevelName"].ToString(),
                                                        FilePath = VFilePath + userCourse.GetFilePath(dr["TestPreReq_LevelLo"].ToString()),
                                                    });
                                                }
                                            }

                                            List<AssAttendancePercentaceInfo> listAttendancePercentaceInfo = new List<AssAttendancePercentaceInfo>();
                                            if (listTestPaperUserAttandanceInfo != null && listTestPaperUserAttandanceInfo.Count > 0)
                                            {
                                                foreach (DataRow dr in listTestPaperUserAttandanceInfo)
                                                {
                                                    listAttendancePercentaceInfo.Add(new AssAttendancePercentaceInfo
                                                    {
                                                        TestId = dr["Test_ModuleId"].ToString(),
                                                        Min_AttendancePercent = dr["Test_AttendancePercent"].ToString(),
                                                        AttendancePercentObtain = AttendancePercent.ToString(),
                                                        AttenPreReqTestAtm = dr["UserActiveModule_ActiveDate"].ToString(),

                                                    });
                                                }
                                            }

                                            List<TestPaperAttachedAssInfo> TestPaperAttachedAssInfo = new List<TestPaperAttachedAssInfo>();
                                            if (listTestPaperAttachedAssInfo != null && listTestPaperAttachedAssInfo.Count > 0)
                                            {
                                                foreach (DataRow dr in listTestPaperAttachedAssInfo)
                                                {
                                                    TestPaperAttachedAssInfo.Add(new TestPaperAttachedAssInfo
                                                    {
                                                        TestId = dr["TestPreAss_TestId"].ToString(),
                                                        TestPreAssID = dr["TestPreAssID"].ToString(),
                                                        TestPreAss_PreReqTestId = dr["TestPreAss_PreReqTestId"].ToString(),
                                                        TestPreAss_MaxScore = dr["TestPreAss_MaxScore"].ToString(),
                                                        TotalMarksObtained = dr["TotalMarksObtained"].ToString(),
                                                        TotalScoreForPreReq = dr["TotalScoreForPreReq"].ToString(),
                                                        PreRequistitTestName = dr["PreRequistitTestName"].ToString(),
                                                        TotalQuestForMainAss = dr["TotalQuestForMainAss"].ToString(),
                                                        TestUserID = dr["TestUserID"].ToString(),
                                                        PreReqTotQuesWithSolVarient = dr["PreReqTotQuesWithSolVarient"].ToString(),
                                                        preReqMaxAttempt = dr["preReqMaxAttempt"].ToString(),
                                                        preReqAttemptCount = dr["preReqAttemptCount"].ToString(),
                                                        TestEndDate = dr["TestEndDate"].ToString(),
                                                        Test_Type = dr["Test_Type"].ToString(),
                                                        Test_Penaltypercent = dr["Test_Penaltypercent"].ToString(),
                                                        Test_PreReqGroup = dr["Test_PreReqGroup"].ToString(),
                                                        IsLabTest = dr["IsLabTest"].ToString(),
                                                        IsEnableLockdownBrowser = "",

                                                    });
                                                }
                                            }

                                            var varlistAttendancePercentaceInfo = (from item in listAttendancePercentaceInfo
                                                                                   select new
                                                                                   {
                                                                                       Test_ModuleId = item.TestId,
                                                                                       Min_AttendancePercent = item.Min_AttendancePercent,
                                                                                       AttendancePercentObtain = item.AttendancePercentObtain,
                                                                                       UserActiveModule_ActiveDate = item.AttenPreReqTestAtm
                                                                                   }).ToList();


                                            var varlistTestPaperAttachedAssInfo = (from item in TestPaperAttachedAssInfo
                                                                                   select new
                                                                                   {
                                                                                       TestId = item.TestId,
                                                                                       TestPreAssID = item.TestPreAssID,
                                                                                       TestPreAss_PreReqTestId = item.TestPreAss_PreReqTestId,
                                                                                       TestPreAss_MaxScore = item.TestPreAss_MaxScore,
                                                                                       TotalMarksObtained = item.TotalMarksObtained,
                                                                                       TotalScoreForPreReq = item.TotalScoreForPreReq,
                                                                                       PreRequistitTestName = item.PreRequistitTestName,
                                                                                       TotalQuestForMainAss = item.TotalQuestForMainAss,
                                                                                       TestUserID = item.TestUserID,
                                                                                       PreReqTotQuesWithSolVarient = item.PreReqTotQuesWithSolVarient,
                                                                                       preReqMaxAttempt = item.preReqMaxAttempt,
                                                                                       preReqAttemptCount = item.preReqAttemptCount,
                                                                                       TestEndDate = item.TestEndDate,
                                                                                       Test_Type = item.Test_Type,
                                                                                   }).ToList();


                                            var result = (from dr in listTestPaperInfo
                                                          select new
                                                          {
                                                              TestId = dr["TestId"].ToString(),
                                                              TestName = dr["Test_Name"].ToString(),
                                                              TestHeadName = dr["Test_HeadName"].ToString(),
                                                              TestType = dr["Test_Type"].ToString(),
                                                              Test_HasPreRequisite = dr["Test_HasPreRequisite"].ToString(),
                                                              Test_IsComp = dr["Test_IsComp"].ToString(),
                                                              TotalTime = dr["TestSettings_TotalTime"].ToString(),
                                                              TestDate = dr["TestSettings_TestDate"].ToString(),
                                                              TestEndDate = dr["TestSettings_TestEnddate"].ToString(),
                                                              Test_ModuleId = string.IsNullOrEmpty(dr["Test_ModuleId"].ToString()) ? "" : dr["Test_ModuleId"].ToString(),
                                                              DispRes = dr["TestSettings_DispResImm"].ToString(),
                                                              ResDate = dr["TestSettings_ResDate"].ToString(),
                                                              IsAutoPracticeTest = dr["Test_IsAutoPracticeTest"].ToString(),
                                                              NoAtmpts = dr["TestSettings_NoAtmpts"].ToString(),
                                                              NegMrks = dr["TestSettings_NegMrks"].ToString(),
                                                              TotalQues = dr["TestNumQues"].ToString(),
                                                              TotalMarks = dr["TestSettings_totmarks"].ToString(),
                                                              DynDispFlag = dr["TestSettings_DynDispFlag"].ToString(),
                                                              DynDispMode = dr["TestSettings_DynDispMode"].ToString(),
                                                              ModulePercent = dr["Test_ModulePercent"].ToString(),
                                                              Penaltypercent = string.Empty,
                                                              PenaltyExpDate = string.Empty,
                                                              IsStudyPlan = string.Empty,
                                                              IsEnableLockdownBrowser = string.Empty,
                                                              IsLabTest = string.Empty,
                                                              PreReqLOsInfo = listTestPaperLosInfo,
                                                              PreReqAttachedAssessmentInfo = varlistTestPaperAttachedAssInfo,
                                                              PreqAttendancePercentInfo = varlistAttendancePercentaceInfo,
                                                              PreReqVideosInfo = strPreReqVideosInfo,
                                                          }).ToList();

                                            var resMessage = new
                                            {
                                                status = "1",
                                                message = "Successfully retrieved assessments information",
                                                response = result,
                                            };
                                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200            
                                        }
                                        else if (listTestPaperInfo.Count > 0)
                                        {
                                            var resut = (from dr in listTestPaperInfo
                                                         select new
                                                         {
                                                             TestId = dr["TestId"].ToString(),
                                                             TestName = dr["Test_Name"].ToString(),
                                                             TestHeadName = dr["Test_HeadName"].ToString(),
                                                             TestType = dr["Test_Type"].ToString(),
                                                             Test_HasPreRequisite = dr["Test_HasPreRequisite"].ToString(),
                                                             Test_IsComp = dr["Test_IsComp"].ToString(),
                                                             TotalTime = dr["TestSettings_TotalTime"].ToString(),
                                                             TestDate = dr["TestSettings_TestDate"].ToString(),
                                                             TestEndDate = dr["TestSettings_TestEnddate"].ToString(),
                                                             Test_ModuleId = dr["Test_ModuleId"].ToString(),
                                                             DispRes = dr["TestSettings_DispResImm"].ToString(),
                                                             ResDate = dr["TestSettings_ResDate"].ToString(),
                                                             IsAutoPracticeTest = dr["Test_IsAutoPracticeTest"].ToString(),
                                                             NoAtmpts = dr["TestSettings_NoAtmpts"].ToString(),
                                                             NegMrks = dr["TestSettings_NegMrks"].ToString(),
                                                             TotalQues = dr["TestNumQues"].ToString(),
                                                             TotalMarks = dr["TestSettings_totmarks"].ToString(),
                                                             DynDispFlag = dr["TestSettings_DynDispFlag"].ToString(),
                                                             DynDispMode = dr["TestSettings_DynDispMode"].ToString(),
                                                             ModulePercent = dr["Test_ModulePercent"].ToString(),
                                                             Penaltypercent = string.Empty,
                                                             PenaltyExpDate = string.Empty,
                                                             IsStudyPlan = string.Empty,
                                                             IsEnableLockdownBrowser = string.Empty,
                                                             IsLabTest = string.Empty,
                                                             PreReqLOsInfo = strPreReqLOsInfo,
                                                             PreReqAttachedAssessmentInfo = strPreReqAttachedAssessmentInfo,
                                                             PreqAttendancePercentInfo = strPreqAttendancePercentInfo,
                                                             PreReqVideosInfo = strPreReqVideosInfo,
                                                         }).ToList();

                                            var resMessage = new
                                            {
                                                status = "1",
                                                message = "Successfully retrieved assessments information",
                                                response = resut,
                                            };
                                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200            
                                        }
                                    }
                                    else
                                    {
                                        var resMessage = new
                                        {
                                            status = "0",
                                            message = "No data found."
                                        };
                                        return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                                    }

                                }
                                #region      //regular courses
                                else
                                {
                                    //double AttendancePercent = 0;
                                    //string TestType = string.Empty;

                                    //My Code Start..........

                                    spParam.Length = 0;
                                    spParam.Append("1").Append(colSeperator).Append(TestId).Append(colSeperator);
                                    spParam.Append("2").Append(colSeperator).Append(UserId).Append(colSeperator);
                                    spParam.Append("3").Append(colSeperator).Append(SectionId);

                                    dsTestPaperInfo = null;

                                    dsTestPaperInfo = objAssmts.GetTestInfo(spParam.ToString());

                                    if (dsTestPaperInfo.Tables[0].Rows[0][0].ToString() == "0")
                                    {
                                        List<DataRow> listTestPaperInfo = null;
                                        List<DataRow> listTestPaperVideoInfo = null;
                                        List<DataRow> listTestPaperLoInfo = null;
                                        List<DataRow> listTestPaperUserAttandanceInfo = null;
                                        List<DataRow> listTestPaperAttachedAssInfo = null;
                                        //List<DataRow> listTestPaperStudyInfo = null;
                                        dst = null;
                                        listTestPaperInfo = dsTestPaperInfo.Tables[0].AsEnumerable().ToList();

                                        if (dsTestPaperInfo.Tables.Count > 6)
                                        {
                                            if (dsTestPaperInfo.Tables[6].Rows[0][0].ToString() == "0" && dsTestPaperInfo.Tables[6].Rows.Count > 0)
                                            {
                                                string Attemptnum = dsTestPaperInfo.Tables[4].Rows[0][0].ToString();
                                                string attendanceattemptnum = dsTestPaperInfo.Tables[6].Rows[0]["TestSettings_AttenPreReqTestAtm"].ToString();
                                                if (int.Parse(attendanceattemptnum) == int.Parse(Attemptnum) + 1 || attendanceattemptnum == "0")
                                                {
                                                    if (dsTestPaperInfo.Tables[6].Rows[0]["Section_AttendanceXMLGuidFileName"].ToString().Trim() != "")
                                                    {
                                                        objTestPaper = new TestPaper();
                                                        AttendancePercent = objTestPaper.GetUserAttendance(SectionId, UserId, dsTestPaperInfo.Tables[6].Rows[0]["Section_AttendanceXMLGuidFileName"].ToString());
                                                    }
                                                }
                                            }
                                        }


                                        spParam.Length = 0;
                                        spParam.Append("1").Append(colSeperator).Append(TestId).Append(colSeperator);
                                        spParam.Append("2").Append(colSeperator).Append("1").Append(colSeperator);
                                        spParam.Append("3").Append(colSeperator).Append(UserId).Append(colSeperator);
                                        spParam.Append("4").Append(colSeperator).Append(CourseId).Append(colSeperator);
                                        spParam.Append("5").Append(colSeperator).Append(SectionId);

                                        dst = objAssmts.GetPreRequisitesInfo(spParam.ToString());

                                        // if (dsTestPaperInfo.Tables[3].Rows[0][0].ToString() == "0")
                                        //{
                                        //    Tz_Name = dsTestPaperInfo.Tables[3].Rows[0]["Tz_Name"].ToString();
                                        //}

                                        if ((dst.Tables[0].Rows[0][0].ToString() == "0" && dst.Tables[0].Rows.Count > 0) ||
                                            (dst.Tables[1].Rows[0][0].ToString() == "0" && dst.Tables[1].Rows.Count > 0) ||
                                            (dst.Tables[2].Rows[0][0].ToString() == "0" && dst.Tables[2].Rows.Count > 0) ||
                                            (dst.Tables[3].Rows[0][0].ToString() == "0" && dst.Tables[3].Rows.Count > 0) ||
                                            (dst.Tables[4].Rows[0][0].ToString() == "0" && dst.Tables[4].Rows.Count > 0))
                                        {
                                            if (dst.Tables[0].Rows[0][0].ToString() == "0" && dst.Tables[0].Rows.Count > 0)
                                            {
                                                listTestPaperLoInfo = dst.Tables[0].AsEnumerable().ToList();
                                            }

                                            if (dst.Tables[1].Rows[0][0].ToString() == "0" && dst.Tables[1].Rows.Count > 0)
                                            {
                                                listTestPaperAttachedAssInfo = dst.Tables[1].AsEnumerable().ToList();
                                            }

                                            if (dst.Tables[2].Rows[0][0].ToString() == "0" && dst.Tables[2].Rows.Count > 0)
                                            {
                                                listTestPaperUserAttandanceInfo = dst.Tables[2].AsEnumerable().ToList();
                                            }

                                            if (dst.Tables[3].Rows[0][0].ToString() == "0" && dst.Tables[3].Rows.Count > 0)
                                            {
                                                listTestPaperVideoInfo = dst.Tables[3].AsEnumerable().ToList();
                                            }

                                            List<TestPaperLoInfo> listTestPaperLosInfo = new List<TestPaperLoInfo>();
                                            if (listTestPaperLoInfo != null && listTestPaperLoInfo.Count > 0)
                                            {
                                                Educosoft.Api.Utilities.FileUpload uploadObj;
                                                uploadObj = new Educosoft.Api.Utilities.FileUpload();
                                                dst = null;
                                                spParam = new StringBuilder();
                                                spParam.Length = 0;
                                                spParam.Append("1").Append(colSeperator).Append("FP5");
                                                dst = uploadObj.GetVirtualPath(spParam.ToString());
                                                if (dst.Tables[0].Rows[0].ItemArray[0].ToString() == "0")
                                                {
                                                    VFilePath = "";
                                                    VFilePath = dst.Tables[0].Rows[0].ItemArray[4].ToString();
                                                }
                                                UserCourse userCourse = new UserCourse();

                                                foreach (DataRow dr in listTestPaperLoInfo)
                                                {
                                                    listTestPaperLosInfo.Add(new TestPaperLoInfo
                                                    {
                                                        TestId = dr["TestPreReq_TestID"].ToString(),
                                                        TestPreReqId = dr["intPk"].ToString(),
                                                        TestPreReq_LevelID = dr["TestPreReq_LevelID"].ToString(),
                                                        TestPreReq_TimeAlloted = dr["TestPreReq_TimeAlloted"].ToString(),
                                                        TestPreReq_LevelParent = dr["TestPreReq_LevelParent"].ToString(),
                                                        TestPreReq_LevelLo = dr["TestPreReq_LevelLo"].ToString(),
                                                        CRTimeSpent_TimeSpentMin = Convert.ToDouble(Convert.ToInt32(dr["CRTimeSpent_TimeSpent"].ToString()) / 60).ToString(),
                                                        CourseHirarchy = dr["LevelParentName"].ToString() + " > " + dr["LevelName"].ToString(),
                                                        FilePath = VFilePath + userCourse.GetFilePath(dr["TestPreReq_LevelLo"].ToString()),
                                                    });
                                                }
                                            }

                                            List<AssAttendancePercentaceInfo> listAttendancePercentaceInfo = new List<AssAttendancePercentaceInfo>();
                                            if (listTestPaperUserAttandanceInfo != null && listTestPaperUserAttandanceInfo.Count > 0)
                                            {
                                                foreach (DataRow dr in listTestPaperUserAttandanceInfo)
                                                {
                                                    listAttendancePercentaceInfo.Add(new AssAttendancePercentaceInfo
                                                    {
                                                        TestId = dr["TestID"].ToString(),
                                                        Min_AttendancePercent = dr["Test_AttendancePercent"].ToString(),
                                                        AttendancePercentObtain = AttendancePercent.ToString(),
                                                        AttenPreReqTestAtm = dr["TestSettings_AttenPreReqTestAtm"].ToString(),

                                                    });
                                                }
                                            }

                                            List<AssessmentPreReqVideoInfo> listVideoInfo = new List<AssessmentPreReqVideoInfo>();
                                            if (listTestPaperVideoInfo != null && listTestPaperVideoInfo.Count > 0)
                                            {
                                                dst = null;

                                                Educosoft.Api.Utilities.FileUpload fileObj = new Educosoft.Api.Utilities.FileUpload();
                                                dst = fileObj.GetVirtualPath("1" + colSeperator + "FP3");
                                                if (dst.Tables[0].Rows[0][0].ToString() == "0")
                                                {
                                                    VFilePath = "";
                                                    VFilePath = dst.Tables[0].Rows[0][4].ToString();
                                                }

                                                double videoTimeSpent = 0;
                                                string formattedMinDuration = string.Empty;
                                                string formattedViewedDuration = string.Empty;
                                                objTestPaper = new TestPaper();
                                                foreach (DataRow dr in listTestPaperVideoInfo)
                                                {
                                                    if (dr["TestPreReqVideo_MinTime"].ToString() != "")
                                                    {
                                                        videoTimeSpent = 0;
                                                        string timeSpentfromDB = dr["TestPreReqVideo_TimeSpent"].ToString() == "" ? "0" : dr["TestPreReqVideo_TimeSpent"].ToString();
                                                        var timespanValues = TimeSpan.Parse(timeSpentfromDB);
                                                        videoTimeSpent = timespanValues.TotalSeconds;
                                                        var minTimeSpanValues = TimeSpan.FromMinutes(Convert.ToDouble(dr["TestPreReqVideo_MinTime"].ToString()));
                                                        var vartimespanValues = timespanValues.Minutes;
                                                        var varMinTimeSpanValues = minTimeSpanValues.Minutes;
                                                        listVideoInfo.Add(new AssessmentPreReqVideoInfo
                                                        {
                                                            TestId = dr["TestPreReqVideo_TestID"].ToString(),
                                                            TestPrevideoID = dr["TestPrevideoID"].ToString(),
                                                            TestPreReqVideo_uploadID = dr["TestPreReqVideo_uploadID"].ToString(),
                                                            TestPreReqVideo_MinTime = varMinTimeSpanValues.ToString(),
                                                            TestPreReqVideo_FileName = dr["TestPreReqVideo_FileName"].ToString(),
                                                            TestPreReqVideo_TimeSpent = vartimespanValues.ToString(),
                                                            TestPreReqVideo_VideoType = dr["TestPreReqVideo_VideoType"].ToString(),
                                                            IsAdminVideo = dr["IsAdminVideo"].ToString(),
                                                            FilePath = objTestPaper.GetVideoFile(dr["TestPreReqVideo_uploadID"].ToString(), SectionId, VFilePath, dr["TestPreReqVideo_VideoType"].ToString()).ToString(),

                                                        });

                                                    }
                                                    else
                                                    {
                                                        listVideoInfo.Add(new AssessmentPreReqVideoInfo
                                                        {
                                                            TestId = dr["TestPreReqVideo_TestID"].ToString(),
                                                            TestPrevideoID = dr["TestPrevideoID"].ToString(),
                                                            TestPreReqVideo_uploadID = dr["TestPreReqVideo_uploadID"].ToString(),
                                                            TestPreReqVideo_MinTime = dr["TestPreReqVideo_MinTime"].ToString(),
                                                            TestPreReqVideo_FileName = dr["TestPreReqVideo_FileName"].ToString(),
                                                            TestPreReqVideo_TimeSpent = dr["TestPreReqVideo_TimeSpent"].ToString(),
                                                            TestPreReqVideo_VideoType = dr["TestPreReqVideo_VideoType"].ToString(),
                                                            IsAdminVideo = dr["IsAdminVideo"].ToString(),
                                                            FilePath = objTestPaper.GetVideoFile(dr["TestPreReqVideo_uploadID"].ToString(), SectionId, VFilePath, dr["TestPreReqVideo_VideoType"].ToString()).ToString(),
                                                        });
                                                    }
                                                }
                                            }

                                            List<TestPaperAttachedAssInfo> TestPaperAttachedAssInfo = new List<TestPaperAttachedAssInfo>();
                                            if (listTestPaperAttachedAssInfo != null && listTestPaperAttachedAssInfo.Count > 0)
                                            {
                                                foreach (DataRow dr in listTestPaperAttachedAssInfo)
                                                {
                                                    TestPaperAttachedAssInfo.Add(new TestPaperAttachedAssInfo
                                                    {
                                                        TestId = dr["TestPreAss_TestId"].ToString(),
                                                        TestPreAssID = dr["TestPreAssID"].ToString(),
                                                        TestPreAss_PreReqTestId = dr["TestPreAss_PreReqTestId"].ToString(),
                                                        TestPreAss_MaxScore = dr["TestPreAss_MaxScore"].ToString(),
                                                        TotalMarksObtained = dr["TotalMarksObtained"].ToString(),
                                                        TotalScoreForPreReq = dr["TotalScoreForPreReq"].ToString(),
                                                        PreRequistitTestName = dr["PreRequistitTestName"].ToString(),
                                                        TotalQuestForMainAss = dr["TotalQuestForMainAss"].ToString(),
                                                        TestUserID = dr["TestUserID"].ToString(),
                                                        PreReqTotQuesWithSolVarient = dr["PreReqTotQuesWithSolVarient"].ToString(),
                                                        preReqMaxAttempt = dr["preReqMaxAttempt"].ToString(),
                                                        preReqAttemptCount = dr["preReqAttemptCount"].ToString(),
                                                        TestEndDate = dr["TestEndDate"].ToString(),
                                                        Test_Type = dr["Test_Type"].ToString(),
                                                        Test_Penaltypercent = dr["Test_Penaltypercent"].ToString(),
                                                        Test_PreReqGroup = dr["Test_PreReqGroup"].ToString(),
                                                        IsLabTest = dr["IsLabTest"].ToString(),
                                                        IsEnableLockdownBrowser = dr["IsEnableLockdownBrowser"].ToString(),

                                                    });
                                                }
                                            }

                                            var result = (from dr in listTestPaperInfo
                                                          select new
                                                          {
                                                              TestId = dr["TestId"].ToString(),
                                                              TestName = dr["Test_Name"].ToString(),
                                                              TestHeadName = dr["Test_HeadName"].ToString(),
                                                              TestType = dr["Test_Type"].ToString(),
                                                              Test_HasPreRequisite = dr["Test_HasPreRequisite"].ToString(),
                                                              Test_IsComp = dr["Test_IsComp"].ToString(),
                                                              TotalTime = dr["TestSettings_TotalTime"].ToString(),
                                                              TestDate = dr["TestSettings_TestDate"].ToString(),
                                                              TestEndDate = dr["TestSettings_TestEnddate"].ToString(),
                                                              Test_ModuleId = string.Empty,
                                                              DispRes = dr["TestSettings_DispResImm"].ToString(),
                                                              ResDate = dr["TestSettings_ResDate"].ToString(),
                                                              IsAutoPracticeTest = dr["Test_IsAutoPracticeTest"].ToString(),
                                                              NoAtmpts = dr["TestSettings_NoAtmpts"].ToString(),
                                                              NegMrks = dr["TestSettings_NegMrks"].ToString(),
                                                              TotalQues = dr["TestNumQues"].ToString(),
                                                              TotalMarks = dr["TestSettings_totmarks"].ToString(),
                                                              Penaltypercent = dr["Test_Penaltypercent"].ToString(),
                                                              PenaltyExpDate = dr["TestSettings_PenaltyExpDate"].ToString(),
                                                              DynDispFlag = dr["TestSettings_DynDispFlag"].ToString(),
                                                              DynDispMode = dr["TestSettings_DynDispMode"].ToString(),
                                                              //IsActive = dr["TestSettings_IsActive"].ToString(),
                                                              ModulePercent = dr["Test_ModulePercent"].ToString(),
                                                              IsStudyPlan = dr["Test_ISStudyPlan"].ToString(),
                                                              IsEnableLockdownBrowser = dr["TestSettings_IsEnableLockdownBrowser"].ToString(),
                                                              IsLabTest = dr["IsLabTest"].ToString(),
                                                              PreReqLOsInfo = listTestPaperLosInfo,
                                                              PreReqAttachedAssessmentInfo = TestPaperAttachedAssInfo,
                                                              PreqAttendancePercentInfo = listAttendancePercentaceInfo,
                                                              PreReqVideosInfo = listVideoInfo,
                                                          }).ToList();

                                            var resMessage = new
                                            {
                                                status = "1",
                                                message = "Successfully retrieved assessments information",
                                                response = result,
                                            };
                                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200            
                                        }  //end lo list block
                                        else if (listTestPaperInfo.Count > 0)
                                        {
                                            var resut = (from dr in listTestPaperInfo
                                                         select new
                                                         {
                                                             TestId = dr["TestId"].ToString(),
                                                             TestName = dr["Test_Name"].ToString(),
                                                             TestHeadName = dr["Test_HeadName"].ToString(),
                                                             TestType = dr["Test_Type"].ToString(),
                                                             Test_HasPreRequisite = dr["Test_HasPreRequisite"].ToString(),
                                                             Test_IsComp = dr["Test_IsComp"].ToString(),
                                                             TotalTime = dr["TestSettings_TotalTime"].ToString(),
                                                             TestDate = dr["TestSettings_TestDate"].ToString(),
                                                             TestEndDate = dr["TestSettings_TestEnddate"].ToString(),
                                                             Test_ModuleId = string.Empty,
                                                             DispRes = dr["TestSettings_DispResImm"].ToString(),
                                                             ResDate = dr["TestSettings_ResDate"].ToString(),
                                                             IsAutoPracticeTest = dr["Test_IsAutoPracticeTest"].ToString(),
                                                             NoAtmpts = dr["TestSettings_NoAtmpts"].ToString(),
                                                             NegMrks = dr["TestSettings_NegMrks"].ToString(),
                                                             TotalQues = dr["TestNumQues"].ToString(),
                                                             TotalMarks = dr["TestSettings_totmarks"].ToString(),
                                                             Penaltypercent = dr["Test_Penaltypercent"].ToString(),
                                                             PenaltyExpDate = dr["TestSettings_PenaltyExpDate"].ToString(),
                                                             DynDispFlag = dr["TestSettings_DynDispFlag"].ToString(),
                                                             DynDispMode = dr["TestSettings_DynDispMode"].ToString(),
                                                             //IsActive = dr["TestSettings_IsActive"].ToString(),
                                                             ModulePercent = dr["Test_ModulePercent"].ToString(),
                                                             IsStudyPlan = dr["Test_ISStudyPlan"].ToString(),
                                                             IsEnableLockdownBrowser = dr["TestSettings_IsEnableLockdownBrowser"].ToString(),
                                                             IsLabTest = dr["IsLabTest"].ToString(),
                                                             PreReqLOsInfo = strPreReqLOsInfo,
                                                             PreReqAttachedAssessmentInfo = strPreReqAttachedAssessmentInfo,
                                                             PreqAttendancePercentInfo = strPreqAttendancePercentInfo,
                                                             PreReqVideosInfo = strPreReqVideosInfo,
                                                         }).ToList();

                                            var resMessage = new
                                            {
                                                status = "1",
                                                message = "Successfully retrieved assessments information",
                                                response = resut,
                                            };
                                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200            
                                        }
                                    }
                                    else
                                    {
                                        var resMessage = new
                                        {
                                            status = "0",
                                            message = "No data found."
                                        };
                                        return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                                    }

                                    //My Code End.............

                                }// end block of IsDevCourse
                                #endregion
                            }
                            else
                            {
                                var resMessage = new
                                {
                                    status = "0",
                                    message = "No data found."
                                };
                                return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                            }
                        }
                        else
                        {
                            var resMessage = new
                            {
                                status = "0",
                                message = "UserId does not exit."
                            };
                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                        }
                    }
                    catch (Exception ex)
                    {
                        var resMessage = new
                        {
                            status = "0",
                            message = ex.Message.ToString()
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                    }
                }
                else
                {
                    var resMessage = new
                    {
                        status = "0",
                        message = "Assessments info retrieval credentials are required"
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                }
            }
            else
            {
                var resMessage = new
                {
                    status = "0",
                    message = "Invalid/malformed input(Assessments credential)"
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400                 
            }

            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while retrieving assessments information."
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201
        }
    }
}
