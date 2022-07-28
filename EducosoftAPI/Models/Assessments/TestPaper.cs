using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Educosoft.Api.Assessments;
using System.Text;
using System.Data;
using System.Xml.Linq;
using System.Configuration;
using Educo.ELS.Portal;
using Educo.ELS.CountryPortal;

namespace EducosoftAPI.Models.Assessments
{
    public class TestPaper : PageBase
    {
        public TestPaper()
        {
        }
        public string CheckPreReqExmpt_M(string TestID, string UserId, string SectionId)
        {
            DataSet dstGetUser;
            string hdnTutorialExmpt = "0";
            string hdnAssessmentExmpt = "0";
            string isExempted = "0";

            PGCC_TestPaper ObjPGCCTestPaper = new PGCC_TestPaper();
            StringBuilder spParams = new StringBuilder();
            try
            {
                dstGetUser = new DataSet();
                spParams.Append("1").Append(colSeperator).Append(TestID).Append(colSeperator);
                spParams.Append("2").Append(colSeperator).Append(UserId).Append(colSeperator);
                spParams.Append("3").Append(colSeperator).Append(SectionId);
                dstGetUser = ObjPGCCTestPaper.GetPreReqExmptUsers_M(spParams.ToString());

                if (dstGetUser.Tables[0].Rows[0][0].ToString() == "0")
                {
                    hdnTutorialExmpt = dstGetUser.Tables[0].Rows[0][6].ToString();
                    hdnAssessmentExmpt = dstGetUser.Tables[0].Rows[0][7].ToString();
                    isExempted = "1";
                }
                else
                {
                    isExempted = "0";
                }
            }
            catch (Exception ex)
            {
                //exception occured
                isExempted = "0";
            }
            return hdnTutorialExmpt + "$" + hdnAssessmentExmpt + "$" + isExempted;
        }

        public string CheckPreRequisite_M(string TestID, string UserId, string CourseId, string SectionId, string Tz_Name)
        {
            StringBuilder spParam = null;
            StringBuilder LosTable = new StringBuilder();
            int IntAssmentToBeScored = 0;
            int IntUnCompletedLosFlag = 0;
            double persent = 0;
            double numberScored = 0;
            double maxQuestion = 0;
            int minutes = 0;

            DataSet dstTest = new DataSet();
            //TestPaper ObjTestPaper = new TestPaper();
            PGCC_TestPaper ObjPGCCTestPaper = null;
            spParam = new StringBuilder();
            int CurrGpVal = 0, PrevGpVal = 0;
            bool completePreReqAss = false;
            bool completePreReqAssGroup = true;
            bool completePreReqAttendance = false;
            string hdnTutorialExmpt = "0";
            string hdnAssessmentExmpt = "0";
            string hdnAttendanceExmpt = "0";

            try
            {
                ObjPGCCTestPaper = new PGCC_TestPaper();
                spParam.Length = 0;
                spParam.Append("1").Append(colSeperator).Append(TestID).Append(colSeperator);
                spParam.Append("2").Append(colSeperator).Append("1").Append(colSeperator);
                spParam.Append("3").Append(colSeperator).Append(UserId).Append(colSeperator);
                spParam.Append("4").Append(colSeperator).Append(CourseId).Append(colSeperator);
                spParam.Append("5").Append(colSeperator).Append(SectionId);

                dstTest = ObjPGCCTestPaper.GetPreRequisitesInfo_M(spParam.ToString());

                if (hdnTutorialExmpt != "1")
                {
                    //check if the Pre-RequisiteLos assigned and minimum time is spent on them, else give lilk to study them. This Informantion comes in "Table[0]"
                    if (dstTest.Tables[0].Rows[0][0].ToString() == "0")
                    {

                        for (int i = 0; i < dstTest.Tables[0].Rows.Count; i++)
                        {
                            minutes = 0;
                            minutes = Convert.ToInt32(Convert.ToInt32(dstTest.Tables[0].Rows[i]["CRTimeSpent_TimeSpent"].ToString()) / 60);
                            if (minutes < Convert.ToInt32(dstTest.Tables[0].Rows[i]["TestPreReq_TimeAlloted"].ToString()))
                            {
                                IntUnCompletedLosFlag = 1;
                            }
                        }
                    }
                    else
                    {
                        hdnTutorialExmpt = "1";
                    }
                }

                if (hdnAssessmentExmpt.ToString() != "1")
                {
                    //check if the Pre-Requisite Assessments Assigned and student scored that min score. Else give link to take PreRequsites Assessment, That Informantion comes in "Table[1]"
                    if (dstTest.Tables.Count > 4)
                    {
                        if (dstTest.Tables[4].Rows[0][0].ToString() == "0")
                        {
                            for (int i = 0; i < dstTest.Tables[1].Rows.Count; i++)
                            {
                                persent = 0;
                                numberScored = 0;
                                maxQuestion = 0;
                                numberScored = Convert.ToDouble(dstTest.Tables[4].Rows[i]["TotalMarksObtained"].ToString());
                                maxQuestion = Convert.ToDouble(dstTest.Tables[4].Rows[i]["TotalScoreForPreReq"].ToString());

                                if (maxQuestion == 0 && numberScored != 0)
                                {
                                    persent = Convert.ToDouble(100 * (numberScored / 100));
                                }
                                else
                                {
                                    if (numberScored != 0 && maxQuestion != 0)
                                    {
                                        persent = Convert.ToDouble(100 * (numberScored / maxQuestion));
                                    }
                                }
                                if (i == 0)
                                    PrevGpVal = Convert.ToInt32(dstTest.Tables[4].Rows[i]["Test_PreReqGroup"]);
                                CurrGpVal = Convert.ToInt32(dstTest.Tables[4].Rows[i]["Test_PreReqGroup"]);
                                if (PrevGpVal != CurrGpVal)// when the prereq group changes
                                {
                                    if (completePreReqAssGroup)
                                        completePreReqAss = true;
                                    completePreReqAssGroup = true;
                                }
                                //if user not attemp  prerequisits then score and maxQuesion will come as zero then show prerequist link , OR when attempted and scored Less,
                                if ((persent < Convert.ToDouble(dstTest.Tables[4].Rows[i]["TestPreAss_MaxScore"].ToString())) || (numberScored == 0 && maxQuestion == 0))
                                {
                                    IntAssmentToBeScored = 1;
                                    completePreReqAssGroup = false;
                                    if (PrevGpVal == CurrGpVal)// if same group, then one fails, then set group as fail (WITHOUT OR ) 
                                    {
                                        if (!completePreReqAssGroup)
                                            completePreReqAss = false;
                                    }
                                }
                                else
                                {
                                    if (PrevGpVal != CurrGpVal)// when the prereq group changes
                                    {
                                        if (completePreReqAssGroup)
                                            completePreReqAss = true;
                                    }
                                    else
                                    {
                                        if (completePreReqAssGroup)
                                            completePreReqAss = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            hdnAssessmentExmpt = "1";
                        }
                    }
                }
                //Check for attendance pre-requisite
                if ((hdnTutorialExmpt == "1" || IntUnCompletedLosFlag == 0) && (completePreReqAss == true || hdnAssessmentExmpt == "1"))
                {
                    if (dstTest.Tables.Count > 2)
                    {
                        if (dstTest.Tables[2].Rows.Count > 0)
                        {
                            if (dstTest.Tables[2].Rows[0][0].ToString() == "0")
                            {
                                if (dstTest.Tables[2].Rows[0]["Test_AttendancePercent"].ToString() != "0")
                                {
                                    if (dstTest.Tables[2].Rows[0]["Section_AttendanceXMLGuidFileName"].ToString().Trim() != "")
                                    {
                                        double userAttendancePercent = GetUserAttendance(SectionId, UserId, dstTest.Tables[2].Rows[0]["UserActiveModule_ActiveDate"].ToString(), dstTest.Tables[2].Rows[0]["Section_AttendanceXMLGuidFileName"].ToString(), Tz_Name);
                                        if (userAttendancePercent >= 0)
                                        {
                                            double roundedValue = Math.Round(userAttendancePercent, 2);
                                            if (roundedValue >= double.Parse(dstTest.Tables[2].Rows[0]["Test_AttendancePercent"].ToString().Trim()))
                                            {
                                                completePreReqAttendance = true;
                                                hdnAttendanceExmpt = "1";
                                            }
                                            else
                                            {
                                                hdnAttendanceExmpt = "0";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    hdnAttendanceExmpt = "1";
                                }
                            }
                            else
                            {
                                hdnAttendanceExmpt = "1";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            if ((completePreReqAss == false && hdnAssessmentExmpt == "0") || (IntUnCompletedLosFlag == 1 && hdnTutorialExmpt == "0") || (completePreReqAttendance == false && hdnAttendanceExmpt == "0"))
            {
                return hdnTutorialExmpt + "$" + hdnAssessmentExmpt + "$" + hdnAttendanceExmpt + "$" + "0";
            }
            else
            {
                return hdnTutorialExmpt + "$" + hdnAssessmentExmpt + "$" + hdnAttendanceExmpt + "$" + "1";
            }
        }

        public bool CheckIsEnablePwd(string TestID, string UserId, string SectionId)
        {
            bool isEnablePws = false;
            DataSet dsTestPaperInfo = null;
            PGCC_TestPaper ObjPGCCTestPaper = new PGCC_TestPaper();
            StringBuilder spParams = new StringBuilder();
            TimeZoneDateTime ObjTimeZoneDateTime = new TimeZoneDateTime();
            spParams.Append("1").Append(colSeperator).Append(TestID).Append(colSeperator);
            spParams.Append("2").Append(colSeperator).Append(UserId).Append(colSeperator);
            spParams.Append("3").Append(colSeperator).Append(SectionId);

            dsTestPaperInfo = ObjPGCCTestPaper.GetTestInfo_M(spParams.ToString());

            if (dsTestPaperInfo.Tables[0].Rows[0][0].ToString() == "0")
            {
                if (dsTestPaperInfo.Tables[0].Rows[0]["TestSecLink_TimedPswd"].ToString().Trim().Length > 0 && dsTestPaperInfo.Tables[0].Rows[0]["TestSecLink_TimedPswdFromDate"].ToString() != "" && dsTestPaperInfo.Tables[0].Rows[0]["TestSecLink_TimedPswdToDate"].ToString() != "")
                {
                    var FromDatetime = DateTime.Parse(dsTestPaperInfo.Tables[0].Rows[0]["TestSecLink_TimedPswdFromDate"].ToString());
                    var ToDatetime = DateTime.Parse(dsTestPaperInfo.Tables[0].Rows[0]["TestSecLink_TimedPswdToDate"].ToString());
                    var ToDay = DateTime.Now;
                    if (FromDatetime.CompareTo(ToDay) <= 0 && ToDatetime.CompareTo(ToDay) >= 0)
                    {
                        if (dsTestPaperInfo.Tables[0].Rows[0]["TestSecLink_TimedPswd"].ToString().Trim().Length > 0)
                        {
                            isEnablePws = true;
                        }
                    }
                }
            }
            return isEnablePws;
        }

        public double GetUserAttendance(string sectionId, string userId, string moduleStartDate, string attendanceFile, string Tz_Name)
        {
            double attendancePercent = 0;
            try
            {
                //Objects
                XElement objXElMain;
                XElement objXElUser;
                XElement objXElStartMonth;
                XElement objXElEndMonth;
                XElement objXElMonth;
                StringBuilder attInfo = new StringBuilder();
                TimeZoneDateTime ObjTimeZoneDateTime = null;

                //Variables
                string xmlFilePath = HttpContext.Current.Server.MapPath(ConfigurationSettings.AppSettings["AttendanceXmlPath"]);
                string exemptedDays = string.Empty;
                string considerWeekend = string.Empty;
                int totalDays = 0;
                int daysPresent = 0;
                int startMonth = 0;
                int endMonth = 0;
                int moduleStartDay = 0;
                int currentDay = 0;
                xmlFilePath = xmlFilePath + attendanceFile;
                ObjTimeZoneDateTime = new TimeZoneDateTime();
                objXElMain = XElement.Load(xmlFilePath);

                if (objXElMain != null)
                {
                    //Take out details from sections node
                    objXElUser = objXElMain.Descendants("User").Where(e => ((String)e.Attribute("UserID") == userId)).FirstOrDefault();
                    if (objXElUser != null)
                    {
                        //Month Loop
                        moduleStartDay = GetFormattedDate(moduleStartDate, 'D');
                        currentDay = GetFormattedDate(ObjTimeZoneDateTime.GetUserTimeZoneDateTime(Tz_Name, DateTime.Now).ToString(), 'D');
                        objXElStartMonth = objXElUser.Descendants("Month").Where(e => (GetFormattedDate((string)e.Attribute("MonthValue"), 'M') == GetFormattedDate(moduleStartDate, 'M'))).FirstOrDefault();
                        objXElEndMonth = objXElUser.Descendants("Month").Where(e => (GetFormattedDate((string)e.Attribute("MonthValue"), 'M') == GetFormattedDate(ObjTimeZoneDateTime.GetUserTimeZoneDateTime(Tz_Name, DateTime.Now).ToString(), 'M'))).FirstOrDefault();
                        if (objXElStartMonth != null && objXElEndMonth != null)
                        {
                            startMonth = GetFormattedDate(objXElStartMonth.Attribute("MonthValue").Value, 'M');
                            endMonth = GetFormattedDate(objXElEndMonth.Attribute("MonthValue").Value, 'M');
                            for (int i = startMonth; i <= endMonth; i++)
                            {
                                objXElMonth = objXElUser.Descendants("Month").Where(e => (GetFormattedDate((string)e.Attribute("MonthValue"), 'M') == i)).FirstOrDefault();
                                if (objXElMonth != null)
                                {
                                    foreach (XElement objSElDate in objXElMonth.Descendants())
                                    {
                                        string attendanceValue = string.Empty;
                                        string dateValue = string.Empty;
                                        //Day Values
                                        if (i == startMonth)
                                        {
                                            attendanceValue = objSElDate.Attribute("Attendance").Value;
                                            dateValue = objSElDate.Attribute("DateValue").Value;
                                            if ((attendanceValue == "T" || attendanceValue == "P" || attendanceValue == "E" || attendanceValue == "A") && (Int32.Parse(dateValue) >= moduleStartDay))
                                            {
                                                totalDays += 1;
                                                if (attendanceValue != "A")
                                                    daysPresent += 1;
                                            }
                                        }
                                        else if (i == endMonth)
                                        {
                                            attendanceValue = objSElDate.Attribute("Attendance").Value;
                                            dateValue = objSElDate.Attribute("DateValue").Value;
                                            if ((attendanceValue == "T" || attendanceValue == "P" || attendanceValue == "E" || attendanceValue == "A") && (Int32.Parse(dateValue) <= currentDay))
                                            {
                                                totalDays += 1;
                                                if (attendanceValue != "A")
                                                    daysPresent += 1;
                                            }
                                        }
                                        else
                                        {
                                            attendanceValue = objSElDate.Attribute("Attendance").Value;
                                            if (attendanceValue == "T" || attendanceValue == "P" || attendanceValue == "E" || attendanceValue == "A")
                                            {
                                                totalDays += 1;
                                                if (attendanceValue != "A")
                                                    daysPresent += 1;
                                            }
                                        }
                                    }
                                }
                            }
                            if (totalDays > 0)
                                attendancePercent = (double.Parse(daysPresent.ToString()) / double.Parse(totalDays.ToString())) * 100;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return attendancePercent;
        }

        private int GetFormattedDate(string date, char type)
        {
            DateTime retVal = new DateTime();
            retVal = Convert.ToDateTime(date);
            if (type == 'D')
                return retVal.Day;
            else
                return retVal.Month;
        }

        public string TimeFormat(int intTotSec)
        {
            int intHour = 0;
            int intMin = 0;
            int intSec = 0;
            StringBuilder strTotTime = null;

            try
            {
                if (intTotSec >= 3600)
                {
                    intHour = (intTotSec / 3600);
                    intMin = intTotSec - (intHour * 3600);

                    if (intMin >= 60)
                    {
                        intTotSec = intMin;
                        intMin = (intTotSec / 60);
                        intSec = intTotSec - (intMin * 60);
                    }
                    else
                    {
                        intSec = intMin;
                        intMin = 0;
                    }
                }
                else if (intTotSec >= 60)
                {
                    intMin = (intTotSec / 60);
                    intSec = intTotSec - (intMin * 60);
                }
                else
                {
                    intSec = intTotSec;
                }

                strTotTime = new StringBuilder();

                if (intHour <= 9)
                {
                    strTotTime.Append("0").Append(intHour.ToString()).Append(":");
                }
                else
                {
                    strTotTime.Append(intHour.ToString()).Append(":");
                }

                if (intMin <= 9)
                {
                    strTotTime.Append("0").Append(intMin.ToString());
                }
                else
                {
                    strTotTime.Append(intMin.ToString());
                }
            }
            catch (Exception ex)
            {
            }

            return strTotTime.ToString();
        }

        public string CheckLabAccess(string InstionId)
        {
            CountryPortal objCountryPortal = new CountryPortal();
            string strLabCheck = "0";
            string RigIP = string.Empty;
            string dbIPValue = string.Empty;
            int index = 0;

            try
            {
                StringBuilder sbParamList = new StringBuilder();
                DataSet dstIPAddress = null;
                PGCC_Portal objPGCCPortal = new PGCC_Portal();

                sbParamList.Append("1").Append(colSeperator).Append(InstionId);
                dstIPAddress = objPGCCPortal.GetInstitutionIPAddress(sbParamList.ToString());

                if (dstIPAddress.Tables[0].Rows[0][0].ToString() == "0")
                {
                    //Get Client's IP Address
                    RigIP = objCountryPortal.GetClientIPA();

                    if (RigIP != "")
                    {
                        index = RigIP.LastIndexOf('.');
                        if (index > 0)
                        {
                            RigIP = RigIP.Remove(index);

                            if (InstionId == "556") //556:Bakersfield compare ip address for first two octets
                            {
                                index = RigIP.LastIndexOf('.');
                                RigIP = RigIP.Remove(index);
                            }
                        }

                        foreach (DataRow dsRow in dstIPAddress.Tables[0].Rows)
                        {
                            index = 0;
                            dbIPValue = dsRow["InstitutionIPAddress_IPAddress"].ToString();
                            index = dbIPValue.LastIndexOf('.');
                            dbIPValue = dbIPValue.Remove(index);

                            if (InstionId == "556") //556:Bakersfield compare ip address for first two octets
                            {
                                index = dbIPValue.LastIndexOf('.');
                                dbIPValue = dbIPValue.Remove(index);
                            }

                            if (dbIPValue == RigIP) //Check IP address
                            {
                                strLabCheck = "1";
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            // strLabCheck = "1"; //Testing : please remove
            return strLabCheck;
        }

        public double GetUserAttendance(string sectionId, string userId, string attendanceFile)
        {
            double attendancePercent = 0;
            try
            {
                //Objects
                XElement objXElMain;
                XElement objXElUser;
                StringBuilder attInfo = new StringBuilder();
                TimeZoneDateTime ObjTimeZoneDateTime = null;

                //Variables
                string xmlFilePath = HttpContext.Current.Server.MapPath(ConfigurationSettings.AppSettings["AttendanceXmlPath"]);
                string exemptedDays = string.Empty;
                string considerWeekend = string.Empty;

                xmlFilePath = xmlFilePath + attendanceFile;
                ObjTimeZoneDateTime = new TimeZoneDateTime();
                objXElMain = XElement.Load(xmlFilePath);
                string TotMarkedDaysForTerm = "0";
                string percentageforTerm = "0";

                if (objXElMain != null)
                {
                    //Take out details from sections node
                    TotMarkedDaysForTerm = objXElMain.FirstNode.Parent.Attribute("TotalMarkedDaysForTerm").Value;
                    objXElUser = objXElMain.Descendants("User").Where(e => ((String)e.Attribute("UserID") == userId)).FirstOrDefault();
                    if (objXElUser != null)
                    {
                        percentageforTerm = objXElUser.FirstNode.Parent.Attribute("PercentageForTerm").Value;
                        attendancePercent = (double.Parse(percentageforTerm.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return attendancePercent;
        }

        public double GetUserAttendancePercent(string sectionId, string userId, string attendanceFile)
        {
            double attendancePercent = 0;
            //Objects
            XElement objXElMain;
            XElement objXElUser;
            TimeZoneDateTime ObjTimeZoneDateTime = null;
            try
            {
                //Variables
                string xmlFilePath = HttpContext.Current.Server.MapPath(ConfigurationSettings.AppSettings["AttendanceXmlPath"]);
                string exemptedDays = string.Empty;
                string considerWeekend = string.Empty;

                xmlFilePath = xmlFilePath + attendanceFile;
                ObjTimeZoneDateTime = new TimeZoneDateTime();
                objXElMain = XElement.Load(xmlFilePath);


                if (objXElMain != null)
                {
                    //Take out details from sections node
                    objXElUser = objXElMain.Descendants("User").Where(e => ((String)e.Attribute("UserID") == userId)).FirstOrDefault();
                    if (objXElUser != null)
                    {
                        attendancePercent = double.Parse(objXElUser.Attribute("PercentageForTerm").Value);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                objXElMain = null;
                objXElUser = null;
                ObjTimeZoneDateTime = null;
            }
            return attendancePercent;
        }

        public string GetVideoFile(string UploadId, string SectionId, string FilePath, string VideoType)
        {
            DataSet dsVideoFileInfo = null;
            string VFilePath = "";
            Api_TestPaper ObjTestPaper = new Api_TestPaper();
            StringBuilder spParams = new StringBuilder();

            spParams.Append("2").Append(colSeperator).Append(UploadId).Append(colSeperator);

            if (VideoType == "2")   //uploaded videos
            {
                spParams.Append("3").Append(colSeperator).Append(0).Append(colSeperator);
                spParams.Append("5").Append(colSeperator).Append(1).Append(colSeperator);
                spParams.Append("7").Append(colSeperator).Append("zQKgIk22cdCpmOGSPveVw==");
            }
            else //  youtube videos
            {
                spParams.Append("3").Append(colSeperator).Append(SectionId);
            }                              

            dsVideoFileInfo = ObjTestPaper.GetNoteFiles(spParams.ToString());
            if (dsVideoFileInfo.Tables[0].Rows[0].ItemArray[0].ToString() == "0")
            {
                if (VideoType =="2")
                    VFilePath = FilePath + dsVideoFileInfo.Tables[0].Rows[0]["Upload_GUID"].ToString();
                else
                    VFilePath = "https://www.youtube.com/embed/" + dsVideoFileInfo.Tables[0].Rows[0]["Upload_OriginalFileName"].ToString() + "?wmode=opaque&autoplay=1";

            }
            return VFilePath;
        }


    }
}