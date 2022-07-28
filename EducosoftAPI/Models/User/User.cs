using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using Educosoft.Api.User;
using EducosoftAPI.Models;
using Educo.ELS.Encryption;
using Educo.ELS.SystemSettings;
using Educo.ELS.Ecom;
using AspDotNetStorefrontCore;
using AspDotNetStorefrontEncrypt;
using System.Net.Mail;

namespace EducosoftAPI.Models.User
{
    public class User : PageBase
    {
        private SurveyAppUser objSurveyAppUser = null;
        private StringBuilder spParam = null;
        private DataSet dst = null;
        private EncryptDecrypt objEncrypt = null;
        private AutomatedMails objAutomatedMails = null;
        private string langFile = "Strings-En";
        private string customerId = string.Empty;

        public User()
        {
            objSurveyAppUser = new SurveyAppUser();
        }

        public DataSet SaveSurveyUser(string et_name, string et_email, string et_mobile)
        {
            spParam = new StringBuilder();

            spParam.Append("1").Append(colSeperator).Append(et_name).Append(colSeperator);
            spParam.Append("2").Append(colSeperator).Append(et_email).Append(colSeperator);
            spParam.Append("3").Append(colSeperator).Append(et_mobile);

            dst = objSurveyAppUser.SaveSurveyUser(spParam.ToString());

            return dst;
        }

        public DataSet SaveSurveyFeedback(string et_email, string et_feedbackstring)
        {
            spParam = new StringBuilder();

            spParam.Append("1").Append(colSeperator).Append(et_email).Append(colSeperator);
            spParam.Append("2").Append(colSeperator).Append(et_feedbackstring);

            dst = objSurveyAppUser.SaveSurveyFeedback(spParam.ToString());

            return dst;
        }

        //UserInfo
        public DataSet GetUserInfo(string UserID)
        {
            spParam = new StringBuilder();
            spParam.Append("1").Append(colSeperator).Append(UserID);
            dst = objSurveyAppUser.GetUserInfo(spParam.ToString());
            return dst;
        }

        public DataSet GetForgotPassword(string userLogInName)
        {
            spParam = new StringBuilder();

            spParam.Append("1").Append(colSeperator).Append(userLogInName);

            dst = objSurveyAppUser.GetForgotPassword(spParam.ToString());

            return dst;
        }

        public DataSet GetCountries(string countryID)
        {
            spParam = new StringBuilder();
            spParam.Append("1").Append(colSeperator).Append(countryID);
            dst = objSurveyAppUser.GetCountries(spParam.ToString());
            return dst;
        }

        public DataSet GetState(string stateID, string CountryID)
        {
            spParam = new StringBuilder();
            if (stateID != "")
                spParam.Append("2").Append(colSeperator).Append(stateID);
            else if (CountryID != "")
                spParam.Append("1").Append(colSeperator).Append(CountryID);
            dst = objSurveyAppUser.GetStates(spParam.ToString());
            return dst;
        }

        public DataSet GetStates(string stateID)
        {
            spParam = new StringBuilder();
            spParam.Append("2").Append(colSeperator).Append(stateID).Append(colSeperator);
            spParam.Append("8").Append(colSeperator).Append("1");
            dst = objSurveyAppUser.GetStates(spParam.ToString());
            return dst;
        }

        public int InsertECOM(string userId)
        {
            //Variables
            int retValue = -1;
            string eMail = string.Empty;
            string passWord = string.Empty;
            string fName = string.Empty;
            string lName = string.Empty;
            string phone = string.Empty;
            string address = string.Empty;
            string city = string.Empty;
            string state = string.Empty;
            string country = string.Empty;
            string zip = string.Empty;
            string spParams1 = "";
            string stateAbrv = "";
            string currency = "USD";
            //string customerId = string.Empty;

            //Objects        
            EncryptDecrypt objEncrypt = new EncryptDecrypt();
            Users objECOMUsers = new Users();
            DataSet dstUser;
            spParam = new StringBuilder();
            spParam.Length = 0;
            DataSet dstState;
            User objU = new User();
            if (userId != null)
            {
                //Take values from ECF
                spParam.Length = 0;
                spParam.Append("1").Append(colSeperator).Append(userId);

                dstUser = objSurveyAppUser.GetUserInfo(spParam.ToString());

                if (dstUser.Tables[0].Rows[0][0].ToString() == "0")
                {
                    eMail = dstUser.Tables[0].Rows[0][14].ToString();
                    passWord = objEncrypt.Decrypt(dstUser.Tables[0].Rows[0][29].ToString());
                    fName = dstUser.Tables[0].Rows[0][7].ToString();
                    lName = dstUser.Tables[0].Rows[0][9].ToString();
                    phone = dstUser.Tables[0].Rows[0][11].ToString();
                    address = dstUser.Tables[0].Rows[0][10].ToString();
                    city = dstUser.Tables[0].Rows[0][22].ToString();
                    state = dstUser.Tables[0].Rows[0][23].ToString();
                    country = dstUser.Tables[0].Rows[0][24].ToString();
                    zip = dstUser.Tables[0].Rows[0][25].ToString();
                }
                if (address.Trim() == "")
                {
                    address = "address";
                }
                // Get the State Abbrevation to Insert in ECOM 
                if (state.Trim() != "")
                {
                    dstState = objU.GetStates(state);
                    if (dstState.Tables[0].Rows[0][0].ToString() == "0") // if there are no errors
                    {
                        stateAbrv = dstState.Tables[0].Rows[0][6].ToString();
                    }
                }
                if (country == "1")
                {
                    currency = "INR";
                }
                //Put values to ECOM            
                Password objPwd = new Password(passWord);
                spParam.Length = 0;
                spParam.Append("1").Append(colSeperator).Append(DB.GetNewGUID()).Append(colSeperator);
                spParam.Append("2").Append(colSeperator).Append(1).Append(colSeperator);
                spParam.Append("3").Append(colSeperator).Append(CommonLogic.Left(eMail, 100)).Append(colSeperator);
                spParam.Append("4").Append(colSeperator).Append((objPwd.SaltedPassword).ToString()).Append(colSeperator);
                spParam.Append("5").Append(colSeperator).Append(objPwd.Salt.ToString()).Append(colSeperator);
                spParam.Append("10").Append(colSeperator).Append("1").Append(colSeperator);
                spParam.Append("11").Append(colSeperator).Append(CommonLogic.Left(fName, 50)).Append(colSeperator);
                spParam.Append("12").Append(colSeperator).Append(CommonLogic.Left(lName, 50)).Append(colSeperator);
                spParam.Append("13").Append(colSeperator).Append(CommonLogic.Left(phone, 25)).Append(colSeperator);
                spParam.Append("15").Append(colSeperator).Append(currency).Append(colSeperator);
                spParam.Append("18").Append(colSeperator).Append(CommonLogic.Left(city, 100)).Append(colSeperator);
                spParam.Append("19").Append(colSeperator).Append(CommonLogic.Left(stateAbrv, 100)).Append(colSeperator);
                spParam.Append("20").Append(colSeperator).Append(CommonLogic.Left(address, 100)).Append(colSeperator);
                spParam.Append("21").Append(colSeperator).Append(CommonLogic.Left(country, 100)).Append(colSeperator);
                spParam.Append("22").Append(colSeperator).Append(CommonLogic.Left(zip, 100));

                dstUser = objECOMUsers.InsertUserIntoECOM(spParam.ToString(), langFile);

                if (dstUser.Tables[0].Rows[0][0].ToString() == "0")
                {
                    customerId = dstUser.Tables[0].Rows[0][3].ToString();

                    //Update value to ECF 
                    spParam.Length = 0;
                    spParam.Append("1").Append(colSeperator).Append(userId).Append(colSeperator);
                    spParam.Append("2").Append(colSeperator).Append(customerId);
                    dstUser = objECOMUsers.UpdateCustomerInECF(spParam.ToString(), langFile);
                    if (dstUser.Tables[0].Rows[0][0].ToString() == "0")
                    {
                        retValue = 0;
                    }
                }
            }
            return retValue;
        }

        public int UpdateECOM(string email)
        {
            //Variables
            string encryptedPWD = string.Empty;
            string saltKey = string.Empty;
            int retValue = -1;

            //Objects            
            Users objECOMUsers = new Users();
            spParam = new StringBuilder();
            DataSet dstUser = new DataSet();
            spParam = new StringBuilder();
            Password objPwd;
            if (customerId.Trim() != "")
            {
                objPwd = new Password();
                encryptedPWD = objPwd.SaltedPassword.ToString();
                saltKey = objPwd.Salt.ToString();
                spParam.Length = 0;
                spParam.Append("1").Append(colSeperator).Append(email).Append(colSeperator);
                spParam.Append("2").Append(colSeperator).Append(encryptedPWD).Append(colSeperator);
                spParam.Append("6").Append(colSeperator).Append("2").Append(colSeperator);
                spParam.Append("7").Append(colSeperator).Append(customerId).Append(colSeperator);
                spParam.Append("8").Append(colSeperator).Append(saltKey);

                //Component
                dstUser = objECOMUsers.UpdateCustomers(spParam.ToString(), langFile);
                if (dstUser.Tables[0].Rows[0][0].ToString() == "0")
                {
                    retValue = 0;
                }
            }
            return retValue;
        }

        public bool UserSupport(string UserId, string SectionId, string UserType, string PhoneNumber, string TellAboutText, string AttachmentFile,
                                        string CountryId, string Date, string Time, string UserName, string eMail, string InstutionId)
        {
            string UploadFilePath = "";
            string institution = "";
            string term = "";
            string instructor = "";
            string section = "";
            string supportemail = "aditya@educo-int.com";
            string CcEmail = "0";

            if (SectionId != null)
                CcEmail = Getccdetails(SectionId);

            if (UserType == "CC")
            {
                UserType = "Campus Coordinator";
            }
            else if (UserType == "IN")
            {
                UserType = "Instructor";
            }
            else if (UserType == "ST")
            {
                UserType = "Student";
            }
            if (String.IsNullOrEmpty(eMail))
                eMail = "noreply@educo-int.com";

            spParam = new StringBuilder();
            spParam.Length = 0;
            spParam.Append("1").Append(colSeperator).Append(CountryId);
            dst = objSurveyAppUser.GetCountries(spParam.ToString());

            if (dst.Tables[0].Rows[0][0].ToString() == "0")
            {
                string Countryname = "";
                Countryname = dst.Tables[0].Rows[0][4].ToString();

                if (Countryname.ToUpper().ToString() == "INDIA")
                {
                    supportemail = "charan@educo-int.com";
                }
                dst = null;
            }

            if (SectionId != null)
            {
                spParam.Length = 0;
                spParam.Append("1").Append(colSeperator).Append(SectionId).Append(colSeperator);
                spParam.Append("2").Append(colSeperator).Append(InstutionId);
                DataSet dsUserInfo = objSurveyAppUser.UserGetInsInfo(spParam.ToString());
                if (dsUserInfo.Tables.Count > 0)
                {
                    institution = dsUserInfo.Tables[0].Rows[0].ItemArray[3].ToString();
                    section = dsUserInfo.Tables[0].Rows[0].ItemArray[5].ToString();
                    instructor = dsUserInfo.Tables[0].Rows[0].ItemArray[4].ToString();
                    term = dsUserInfo.Tables[0].Rows[0].ItemArray[7].ToString();
                }
                dsUserInfo = null;
            }

            MailMessage message = new MailMessage();
            message.From = new MailAddress(eMail);
            message.To.Add(new MailAddress(supportemail));


            if (CcEmail.ToString() != "0")
            {
                message.CC.Add(new MailAddress(CcEmail));
            }
            ECFDateTime dateFormat = new ECFDateTime();

            message.Subject = "EducoSoft Support Question";
            string strMessage = "<table cellspacing=5 border=0>";
            strMessage = strMessage + "<tr><td><b>Name&nbsp;</b></td><td>" + UserName.ToString() + "</td></tr>";
            strMessage = strMessage + "<tr><td><b>Email&nbsp;</b></td><td>" + eMail + "</td></tr>";
            strMessage = strMessage + "<tr><td><b>Phone Number&nbsp;</b></td><td>" + PhoneNumber + "</td></tr>";
            strMessage = strMessage + "<tr><td><b>Role&nbsp;</b></td><td><b>" + UserType + "</b></td></tr>";
            if (institution != "0")
                strMessage = strMessage + "<tr><td><b>Institution&nbsp;</b></td><td>" + institution + "</td></tr>";
            if (SectionId != null)
            {
                strMessage = strMessage + "<tr><td><b>Term&nbsp;</b></td><td>" + term + "</td></tr>";
                strMessage = strMessage + "<tr><td><b>Section&nbsp;</b></td><td>" + section + "</td></tr>";
            }
            if (UserType == "Student")
            {
                strMessage = strMessage + "<tr><td><b>Instructor&nbsp;</b></td><td>" + instructor + "</td></tr>";
            }

            strMessage = strMessage + "<tr><td>-------------------</td><td>--------------------</td></tr>";

            strMessage = strMessage + "<tr><td colspan=2><b>Problem Description:</b><br><SPAN style=\"COLOR: blue\">" +
                         TellAboutText + "</SPAN></td></tr>";
            strMessage = strMessage + "<tr><td><b>Date Occured</b></td><td>" + Date + "</td></tr>";
            strMessage = strMessage + "<tr><td><b>Time</b></td><td>" + Time + "</td></tr>";
            strMessage = strMessage + "</table>";

            message.Body = strMessage.ToString();
            message.IsBodyHtml = true;
            if (AttachmentFile != "" && AttachmentFile.Length > 0)
            {
                string fileNewGuid = "";
                fileNewGuid = System.Guid.NewGuid().ToString().ToUpper();

                string strDestinationFolder = "Support" + "\\" + UploadFilePath;
                string strDestFolder = HttpContext.Current.Server.MapPath(strDestinationFolder);

                UploadFilePath = strDestFolder;

                if (!Directory.Exists(UploadFilePath))
                    Directory.CreateDirectory(UploadFilePath);

                string[] files = Directory.GetFiles(UploadFilePath);
                foreach (string file in files)
                {
                    File.Delete(file);
                }

                File.WriteAllBytes(UploadFilePath + "\\" + AttachmentFile,
                                   Convert.FromBase64String(AttachmentFile));

                System.Net.Mail.Attachment objattach;
                objattach =
                    new System.Net.Mail.Attachment(UploadFilePath + "\\" + AttachmentFile.ToString());
                message.Attachments.Add(objattach);
                objattach = null;
            }

            SmtpClient client = new SmtpClient();
            client.Host = "mail.educo-int.com";
            client.Send(message);

            return true;

        }

        protected string Getccdetails(string sectionId)
        {
            string UsersEmail = "";
            string ChkCopymail = "FALSE";

            spParam = new StringBuilder();
            spParam.Append("1").Append(colSeperator).Append(sectionId);
            dst = objSurveyAppUser.GetCCDetails(spParam.ToString());
            if (dst.Tables[0].Rows[0][0].ToString() == "0")
            {
                ChkCopymail = dst.Tables[0].Rows[0][6].ToString();
            }
            if (ChkCopymail.ToUpper().ToString() == "TRUE")
            {
                UsersEmail = dst.Tables[0].Rows[0][3].ToString();
            }
            else
            {
                UsersEmail = "0";
            }
            dst = null;

            return UsersEmail;
        }
    }
}