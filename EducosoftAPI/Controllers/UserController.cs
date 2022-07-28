using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EducosoftAPI.Models;
using System.Web.Http.ModelBinding;
using Educo.ELS.UserLogin;
using Educo.ELS.CountryPortal;
using System.Configuration;
using System.Data;
using Educosoft.Api.Utilities;
using EducosoftAPI.Models.User;
using Educosoft.Api.User;
using System.IO;
using System.Text;
using Educo.ELS.Encryption;
using Educo.ELS.SystemSettings;
using Educosoft.Api.User;

namespace EducosoftAPI.Controllers
{
    public class UserController : ApiController
    {
        #region
        /// <summary>
        ///This api call takes VerifyLogin credential i.e., 'Email', 'Password' and
        ///returns student details like 'User_FirstName', 'User_LastName', 'User_Email', 'UserId' with 'status', 'country' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in transaction of information.
        /// </summary>
        /// <param name="userVerifyLoginCredential">VerifyLogin credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": "1",
        ///   "message": "User is successfully verified",
        ///   "country" : "US",
        ///   "response:":
        ///     {
        ///         "User_FirstName":"",
        ///         "User_LastName":"",
        ///         "User_Email":"",
        ///         "UserId":"",
        ///     }
        ///}
        ///Error:
        ///{
        ///     "status": "0",
        ///     "message": "User does not exist / Error message regarding the transaction failure"
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("VerifyLogin")]
        public HttpResponseMessage VerifyLogin([ModelBinder] UserVerifyLoginCredential userVerifyLoginCredential)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (userVerifyLoginCredential.Email == null || userVerifyLoginCredential.Password == null)
                    {
                        if (userVerifyLoginCredential.Email == null)
                            throw new ArgumentNullException("Email", "Parameter 'Email' is required & non-null");
                        if (userVerifyLoginCredential.Password == null)
                            throw new ArgumentNullException("Password", "Parameter 'Password' is required & non-null");
                    }
                    if (userVerifyLoginCredential.Email.Trim() != "" && userVerifyLoginCredential.Password.Trim() != "")
                    {
                        //CountryPortal countryPortal = new CountryPortal();
                        //string country = countryPortal.GetClientCountry();
                        string country = "";
                        DataSet dst;
                        char colSep = (char)195;
                        UserLogin objUserLogin = new UserLogin();
                        string UserLoginDtls = objUserLogin.VerifyAppMaster(userVerifyLoginCredential.Email.Trim().ToUpper(), userVerifyLoginCredential.Password.Trim());
                        string instUserDtls = UserLoginDtls;
                        string[] instUsrLogins = instUserDtls.Split('$');

                        if (instUsrLogins.Length > 0 && instUsrLogins[0].ToUpper() == "TRUE") //Institution user
                        {
                            User objUser = new User();
                            List<DataRow> UserDetails = null;
                            if (!string.IsNullOrEmpty(instUsrLogins[2]))
                            {
                                dst = objUser.GetUserInfo(instUsrLogins[2]);
                                if (dst != null && dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0)
                                {
                                    if (dst.Tables[0].Rows[0][0].ToString() == "0")
                                    {
                                        UserDetails = dst.Tables[0].AsEnumerable().ToList();

                                        dst = null;
                                        objUser = null;

                                        var listUserInfo = from dr in UserDetails
                                                           select new
                                                           {
                                                               UserId = dr["intPkVal"],
                                                               User_FirstName = dr["Users_FirstName"],
                                                               User_LastName = dr["Users_LastName"],
                                                               User_EmailId = dr["Users_Email"]
                                                           };

                                        var userDetails = listUserInfo.Distinct();

                                        var resMessage = new
                                        {
                                            status = "1",
                                            message = "User is successfully verified",
                                            country = GetCountry(instUsrLogins[5].ToString()),
                                            response = userDetails
                                        };
                                        return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200
                                    }
                                    else if (dst.Tables[0].Rows[0][0].ToString() == "-1")
                                    {
                                        var resMessage = new
                                      {
                                          status = "0",
                                          message = dst.Tables[0].Rows[0]["VarErrorMsg"].ToString()
                                      };

                                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                                    }
                                    else if (dst.Tables[0].Rows[0][0].ToString() == "-2")
                                    {
                                        var resMessage = new
                                       {
                                           status = "0",
                                           message = dst.Tables[0].Rows[0]["VarErrorMsg"].ToString()
                                       };
                                        return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200  
                                    }
                                }
                            }
                            else
                            {
                                var resMessage = new
                                {
                                    status = "0",
                                    message = "No data found"
                                };
                                return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200  
                            }
                        }
                        else
                        {
                            var resMessage = new
                            {
                                status = "0",
                                message = "Invalid user credentials...!!! Please check your Username and Password.",
                            };
                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                        }
                    }
                    else
                    {
                        var resMessage = new
                        {
                            status = "0",
                            message = "VerifyLogin credential are required"
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                    }

                }
                catch (Exception ex)
                {
                    string ExMessage = "Exception occured while verifying user's information";
                    ExMessage = ex.Message;

                    var resMessage = new
                    {
                        status = "0",
                        message = ExMessage
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                }
            }
            else
            {
                var resMessage = new
                {
                    status = "0",
                    message = "Invalid/malformed input(VerifyLogin credential data)"
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400                 
            }

            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while transacting the information"
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201

        }
        #endregion

        #region
        /// <summary>
        ///This api call takes ForgetPassword credential i.e.,  'Email' to retrieve forgotten password and 
        ///returns 'status' and 'message' upon Your password has been sent to your email id entered..
        ///Incase of failure, it returns only 'status' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in login.
        /// </summary>
        /// <param name="userForgetPasswordCredential">ForgetPassword credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": 1,
        ///   "message": "Your password has been sent to your email id entered.",
        /// }
        ///Error:
        ///{
        ///"status": 0,
        ///"message": "Username does not exist/Error message"
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("ForgetPassword")]
        public HttpResponseMessage ForgotPassword([ModelBinder] UserForgetPasswordCredential userForgetPasswordCredential)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(userForgetPasswordCredential.Email))
                {
                    try
                    {
                        char colSeperator = (char)195;
                        SurveyAppUser objUser = new SurveyAppUser();
                        EncryptDecrypt objEncrypt = new EncryptDecrypt();
                        string password = "", userId = "";
                        string msgForgetPasswordExternalMailStatus = "";
                        UserForgetPasswordDetails userForgetPasswordDetails;
                        StringBuilder spParam = new StringBuilder();
                        DataSet dstForgotPassword, dst, dstDetails;
                        string UName = userForgetPasswordCredential.Email.Trim();

                        spParam.Length = 0;

                        spParam.Append("1").Append(colSeperator).Append(objEncrypt.Encrypt(UName.ToUpper()));
                        spParam.Append(colSeperator).Append("3").Append(colSeperator).Append("1");

                        dst = objUser.VerifyUserLogin(spParam.ToString());
                        if (dst != null && dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                        {
                            userId = dst.Tables[0].Rows[0][3].ToString();
                            userForgetPasswordDetails = new UserForgetPasswordDetails();
                            spParam.Length = 0;
                            spParam.Append("1").Append(colSeperator).Append(userId);
                            dstForgotPassword = objUser.GetForgotPassword(spParam.ToString());

                            if (dstForgotPassword.Tables[0].Rows[0][0].ToString() == "0")
                            {
                                password = objEncrypt.Decrypt(dstForgotPassword.Tables[0].Rows[0][4].ToString());
                            }

                            spParam.Length = 0;
                            spParam.Append("1").Append(colSeperator).Append(userId);

                            dstDetails = objUser.GetUserInfo(spParam.ToString());
                            if (dstDetails.Tables[0].Rows[0][0].ToString() == "0")
                            {
                                userForgetPasswordDetails.userId = userId;
                                userForgetPasswordDetails.first_name = dstDetails.Tables[0].Rows[0][7].ToString();
                                userForgetPasswordDetails.last_name = dstDetails.Tables[0].Rows[0][9].ToString();
                                userForgetPasswordDetails.email = objEncrypt.Decrypt(dstDetails.Tables[0].Rows[0][4].ToString()).ToLower();
                                userForgetPasswordDetails.password = password;

                                //Begin -- Sending external mail to registered email
                                try
                                {
                                    SendForgetPasswordExternalMail(userForgetPasswordDetails.email, userForgetPasswordDetails.password, userForgetPasswordDetails.first_name + " " + userForgetPasswordDetails.last_name);
                                    msgForgetPasswordExternalMailStatus += "Your password has been sent to the email id entered.";
                                }
                                catch (Exception ex)
                                {
                                    msgForgetPasswordExternalMailStatus += "Exception in sending external mail to registered mail: " + ex.Message;
                                }
                                var resMessage = new
                                {
                                    status = "1",
                                    message = msgForgetPasswordExternalMailStatus
                                };

                                return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200  

                                //End -- Sending external mail to registered email
                            }
                        }
                        else
                        {
                            var resMessage = new
                            {
                                status = "0",
                                message = "Email I'd entered does not exist"

                            };
                            return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201  
                        }
                    }
                    catch (Exception ex)
                    {
                        var resMessage = new
                        {
                            status = "0",
                            message = "Exception occured while retrieving the password."
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                    }
                }
                else
                {
                    var resMessage = new
                    {
                        status = "0",
                        message = "user required forget password credential."
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                }
            }
            else
            {
                var resMessage = new
                {
                    status = "0",
                    message = "Invalid/malformed forget password credential."
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400                 
            }
            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while retrieving the password."
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201
        }
        #endregion

        #region
        /// <summary>
        ///This api call takes ResetPassword credentials i.e., 'UserId', 'OldPassword' and 'NewPassword' 
        ///returns 'status' and 'message' upon Your password has been changed successfully.
        ///Incase of failure, it returns only 'status' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in reset password.
        /// </summary>
        /// <param name="userResetPasswordCredential">ResetPassword credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": 1,
        ///   "message": "Your password has been changed successfully.",
        ///Error:
        ///{
        ///"status": 0,
        ///"message": "User does not exist/Error message"
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("ResetPassword")]
        public HttpResponseMessage ResetPassword([ModelBinder] UserResetPasswordCredential userResetPasswordCredential)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(userResetPasswordCredential.UserId) && !string.IsNullOrEmpty(userResetPasswordCredential.OldPassword)
                    && !string.IsNullOrEmpty(userResetPasswordCredential.NewPassword))
                {
                    try
                    {
                        char colSeperator = (char)195;
                        SurveyAppUser objUser = new SurveyAppUser();
                        EncryptDecrypt objEncrypt = new EncryptDecrypt();
                        string password = string.Empty, UserId = string.Empty, encryptedValue = string.Empty;
                        StringBuilder spParam = new StringBuilder();
                        DataSet dst, dstUserInfo;
                        string OldPassword = "", NewPassword = "";
                        string userType = "", email = "", dispName = "", msgResetPwdExternalMailStatus = "";
                        User objCmUser = new User();

                        UserId = userResetPasswordCredential.UserId.Trim();
                        spParam.Length = 0;
                        spParam.Append("1").Append(colSeperator).Append(UserId);

                        dstUserInfo = objUser.GetUserInfo(spParam.ToString());

                        if (dstUserInfo.Tables[0].Rows[0].ItemArray[0].ToString() == "0")
                        {
                            userType = objEncrypt.Decrypt(dstUserInfo.Tables[0].Rows[0][5].ToString());
                            email = dstUserInfo.Tables[0].Rows[0][14].ToString();
                            dispName = dstUserInfo.Tables[0].Rows[0][7].ToString() + " " + dstUserInfo.Tables[0].Rows[0][9].ToString();
                            spParam.Length = 0;
                            spParam.Append("1").Append(colSeperator).Append(UserId);
                            dst = objUser.GetForgotPassword(spParam.ToString());

                            if (dst.Tables[0].Rows[0][0].ToString() == "0")
                            {
                                password = objEncrypt.Decrypt(dst.Tables[0].Rows[0][4].ToString());
                                OldPassword = userResetPasswordCredential.OldPassword.Trim();
                                if (password.ToUpper() == OldPassword.ToUpper())
                                {
                                    NewPassword = userResetPasswordCredential.NewPassword.Trim();

                                    spParam.Length = 0;

                                    encryptedValue = objEncrypt.Encrypt(NewPassword);

                                    spParam.Append("1").Append(colSeperator).Append(UserId).Append(colSeperator);
                                    spParam.Append("2").Append(colSeperator).Append(encryptedValue).Append(colSeperator);
                                    spParam.Append("3").Append(colSeperator).Append(string.Empty).Append(colSeperator);
                                    spParam.Append("4").Append(colSeperator).Append(UserId).Append(colSeperator);
                                    spParam.Append("5").Append(colSeperator).Append("2").Append(colSeperator);
                                    spParam.Append("6").Append(colSeperator).Append(NewPassword).Append(colSeperator);
                                    spParam.Append("8").Append(colSeperator).Append(string.Empty);

                                    //Component
                                    dst = objUser.SavePassword(spParam.ToString());
                                    if (dst.Tables[0].Rows[0].ItemArray[0].ToString() == "0")
                                    {
                                        int updateECOM = -1, InsertECOM = -1;

                                        if (dstUserInfo.Tables[0].Rows[0].ItemArray[0].ToString() == "0")
                                        {
                                            userType = objEncrypt.Decrypt(dstUserInfo.Tables[0].Rows[0][5].ToString());

                                            if (userType.Trim() == "ST")
                                            {
                                                InsertECOM = objCmUser.InsertECOM(UserId);
                                                if (InsertECOM == 0)
                                                {
                                                    updateECOM = objCmUser.UpdateECOM(NewPassword);
                                                    if (updateECOM == 0)
                                                    {
                                                        //Begin -- Sending external mail to registered email
                                                        try
                                                        {
                                                            SendResetPwdExternalMail(email, NewPassword, dispName);
                                                            msgResetPwdExternalMailStatus += "Your password has been changed successfully.";
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            msgResetPwdExternalMailStatus += "Exception in sending external mail to registered mail: " + ex.Message;
                                                        }
                                                        //End -- Sending external mail to registered email

                                                        var resMessage = new
                                                       {
                                                           status = "1",
                                                           message = msgResetPwdExternalMailStatus
                                                       };
                                                        return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                                                    }
                                                    else
                                                    {
                                                        var resMessage = new
                                                        {
                                                            status = "0",
                                                            message = "Password changed successfully in ECF but failed in ECOM-ASPDNSF."
                                                        };
                                                        return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                                                    }
                                                }
                                                else
                                                {
                                                    var resMessage = new
                                                        {
                                                            status = "0",
                                                            message = "Password changed successfully in ECF but failed in ECOM-ASPDNSF."
                                                        };
                                                    return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                                                }
                                            }
                                        }
                                    }//END PASSWORD 
                                    else
                                    {
                                        var resMessage = new
                                        {
                                            status = "0",
                                            message = "Error occured while saving the password."
                                        };
                                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201 
                                    }
                                }
                                else
                                {
                                    var resMessage = new
                                    {
                                        status = "0",
                                        message = "The passwords you entered did not match."
                                    };
                                    return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201 
                                }
                            }
                        }
                        else
                        {
                            var resMessage = new
                            {
                                status = "0",
                                message = "No data found"
                            };
                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200  
                        }
                    }
                    catch (Exception ex)
                    {
                        var resMessage = new
                        {
                            status = "0",
                            message = "Exception occured while reseting the password."
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                    }
                }
                else
                {
                    var resMessage = new
                    {
                        status = "0",
                        message = "User has required reset password credential."
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                }
            }
            else
            {
                var resMessage = new
                {
                    status = "0",
                    message = "Invalid/malformed reset password credential"
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400                 
            }
            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while reseting password."
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201
        }
        #endregion

        #region
        /// <summary>
        ///This api call takes ViewUser credential i.e., 'UserId' and
        ///returns user details like 'address1', 'address2', 'cityName', 'countryName', 'emailId', 'firstName', 'language', 'lastName', 
        ///'mobile', 'phone', 'salutation', 'stateName', 'timeZone', "timeZoneId":"", 'zipcode'  with 'status', 'country' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in transaction of information.
        /// </summary>
        /// <param name="userViewCredential">ViewUser credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": "1",
        ///   "message": "User is successfully verified",
        ///   "response:":
        ///     {
        ///         "timeZoneId":"",
        ///         "timeZone":"",
        ///         "studentId":"",
        ///         "emailId":"",
        ///         "salutation":"",
        ///         "firstName":"" ,
        ///         "lastName":"" ,
        ///         "language":"" ,
        ///         "address1":"",
        ///         "address2":"",
        ///         "cityName":"",
        ///         "countryId":"",
        ///         "countryName":"" ,
        ///         "stateId":"",
        ///         "stateName":"" ,
        ///         "zipcode":"",
        ///         "phone":"" ,
        ///         "mobile":"" ,
        ///         "imageName":"",
        ///          "imagePath:"",
        ///         "studentId":""
        ///     }
        ///}
        ///Error:
        ///{
        ///     "status": "0",
        ///     "message": "User does not exist / Error message regarding the transaction failure"
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("ViewUser")]
        public HttpResponseMessage ViewUser([ModelBinder] UserViewCredential userViewCredential)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (userViewCredential.UserId != null && userViewCredential.UserId != "")
                    {
                        string LangDataText = null;
                        string CountryName, StateName, stateId, countryId, languageId, image, base64String = null;
                        DataSet dst, dstFileInfo;
                        User objUser = new User();
                        const char colSeperator = (char)195;
                        string UserId = userViewCredential.UserId.Trim();
                        dst = objUser.GetUserInfo(UserId);

                        if (dst != null && dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                        {
                            List<DataRow> UserDetails = null;
                            UserDetails = dst.Tables[0].AsEnumerable().ToList();
                            stateId = dst.Tables[0].Rows[0][23].ToString();
                            countryId = dst.Tables[0].Rows[0][24].ToString();
                            languageId = dst.Tables[0].Rows[0][16].ToString();
                            image = dst.Tables[0].Rows[0][15].ToString();
                            StateName = GetStateCountryName(stateId, 0);
                            CountryName = GetStateCountryName(countryId, 1);
                            string imageUrl = "", imageName = "";

                            if (image != "" && image != null)
                            {
                                Educosoft.Api.Utilities.FileUpload fileObj = new Educosoft.Api.Utilities.FileUpload();
                                dstFileInfo = fileObj.GetVirtualPath("1" + colSeperator + "FP16");

                                if (dstFileInfo.Tables[0].Rows[0][0].ToString() == "0")
                                {
                                    string imageFilePath = dstFileInfo.Tables[0].Rows[0][4].ToString();
                                    imageUrl = imageFilePath + UserId + "/"; //Image path is made up of the User's ID and the file name (Vishal - 23/01/2006)
                                    imageName = image;
                                }
                            }

                            if (languageId == "1")
                            {
                                LangDataText = "English";
                            }
                            else if (languageId == "2")
                            {
                                LangDataText = "Spanish";
                            }
                            dst = null;
                            objUser = null;

                            var userDetails = (from dr in UserDetails
                                               select new
                                               {
                                                   timeZone = dr[19].ToString(),
                                                   timeZoneId = dr[18].ToString(),
                                                   studentId = dr[32].ToString(),
                                                   emailId = dr[14].ToString(),
                                                   salutation = dr[6].ToString(),
                                                   firstName = dr[7].ToString(),
                                                   lastName = dr[9].ToString(),
                                                   language = LangDataText,
                                                   address1 = dr[10].ToString(),
                                                   address2 = dr[21].ToString(),
                                                   cityName = dr[22].ToString(),
                                                   countryId = countryId,
                                                   countryName = CountryName,
                                                   stateId = stateId,
                                                   stateName = StateName,
                                                   zipcode = dr[25].ToString(),
                                                   phone = dr[11].ToString(),
                                                   mobile = dr[12].ToString(),
                                                   imagePath = imageUrl,
                                                   imageName = imageName,
                                               }).FirstOrDefault();

                            var resMessage = new
                            {
                                status = "1",
                                message = "User is successfully verified",
                                response = userDetails
                            };
                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200
                        }
                        else
                        {
                            var resMessage = new
                            {
                                status = "0",
                                message = "No data found",
                            };
                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200
                        }

                    }
                    else
                    {
                        var resMessage = new
                        {
                            status = "0",
                            message = "ViewUser credential are required"
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                    }
                }
                catch (Exception ex)
                {

                    string ExMessage = "Exception occured while ViesUser user's information";
                    //ExMessage = ex.Message;

                    var resMessage = new
                    {
                        status = "0",
                        message = ExMessage
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201

                }
            }
            else
            {
                var resMessage = new
                {
                    status = "0",
                    message = "Invalid/malformed input(ViewUser credential data)"
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400                 
            }

            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while transacting the information"
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201
        }
        #endregion

        #region
        /// <summary>
        ///This api call takes EditUser credential i.e., 'UserId', 'Salutation', 'Address1', 'Address2', 'City', 'FirstName', 'LastName', 
        ///'CountryId', 'StateId', 'Mobile', 'Phone', 'ZipCode', 'strImage' and 'strImageType'
        ///returns  
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in transaction of information.
        /// Notes: 1) EditUser is required, 'UserId' and at least one field needed from EditUser credential.
        ///        2) dropdown values of 'Salutation' :Mr.,Mrs.,Ms.,Dr.,Prof.
        ///        3) 'strImage' is in Base64String format and 'strImageType' is the extension of image i.e, jpg, png etc.
        /// </summary>
        /// <param name="userProfileEditCredential">EditUser credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": "1",
        ///   "message": "User has updated successfully".
        ///}
        ///Error:
        ///{
        ///     "status": "0",
        ///     "message": "User does not exist / Error message regarding the transaction failure"
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("EditUser")]
        public HttpResponseMessage EditUser([ModelBinder] UserProfileEditCredential userProfileEditCredential)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(userProfileEditCredential.UserId) && ((!string.IsNullOrEmpty(userProfileEditCredential.Salutation) ||
                     !string.IsNullOrEmpty(userProfileEditCredential.StateId) || !string.IsNullOrEmpty(userProfileEditCredential.CountryId) ||
                     !string.IsNullOrEmpty(userProfileEditCredential.FirstName) || !string.IsNullOrEmpty(userProfileEditCredential.LastName) ||
                     !string.IsNullOrEmpty(userProfileEditCredential.Address1) || !string.IsNullOrEmpty(userProfileEditCredential.Address2) ||
                     !string.IsNullOrEmpty(userProfileEditCredential.City) || !string.IsNullOrEmpty(userProfileEditCredential.Mobile) ||
                     !string.IsNullOrEmpty(userProfileEditCredential.Phone) || !string.IsNullOrEmpty(userProfileEditCredential.strImage) ||
                     !string.IsNullOrEmpty(userProfileEditCredential.strImageType) || !string.IsNullOrEmpty(userProfileEditCredential.ZipCode)
                     )))
                {
                    try
                    {
                        string imgExtn = ".jpg";
                        string msg_ImageSaveStatus = "";
                        StringBuilder spParam = new StringBuilder();
                        char colSeperator = (char)195;
                        string salutation = "", studentId = "", firstName = "", lastName = "", address1 = "", address2 = "", phone = "", email = "", mobile = "",
                            timeZone = "", UserId = "", city = "", stateId = "", zipeCode = "", countryId = "", languageId = "", imageName = "", imagePath = "", base64String = null;

                        DataSet dst, dstFileInfo;
                        User objUser = new User();
                        char colSeparator = (char)195;
                        UserId = userProfileEditCredential.UserId.Trim();
                        dst = objUser.GetUserInfo(UserId);
                        if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                        {
                            if (userProfileEditCredential.Salutation != null && userProfileEditCredential.Salutation != "")
                                salutation = userProfileEditCredential.Salutation.Trim();
                            else
                                salutation = Convert.ToString(dst.Tables[0].Rows[0]["Users_Salutation"]);

                            if (userProfileEditCredential.FirstName != null && userProfileEditCredential.FirstName != "")
                                firstName = userProfileEditCredential.FirstName.Trim();
                            else
                                firstName = Convert.ToString(dst.Tables[0].Rows[0]["Users_FirstName"]);

                            if (userProfileEditCredential.LastName != null && userProfileEditCredential.LastName != "")
                                lastName = userProfileEditCredential.LastName.Trim();
                            else
                                lastName = Convert.ToString(dst.Tables[0].Rows[0]["Users_LastName"]);

                            if (userProfileEditCredential.CountryId != null && userProfileEditCredential.CountryId != "")
                                countryId = userProfileEditCredential.CountryId.Trim();
                            else
                                countryId = Convert.ToString(dst.Tables[0].Rows[0]["Users_Country"]);

                            if (userProfileEditCredential.StateId != null && userProfileEditCredential.StateId != "")
                                stateId = userProfileEditCredential.StateId.Trim();
                            else
                                stateId = Convert.ToString(dst.Tables[0].Rows[0]["Users_State"]);

                            email = Convert.ToString(dst.Tables[0].Rows[0]["Users_Email"]);

                            timeZone = Convert.ToString(dst.Tables[0].Rows[0]["Resource_TzId"]);

                            if (dst.Tables[0].Rows[0]["Resource_LanguageId"].ToString() != "")
                                languageId = Convert.ToString(dst.Tables[0].Rows[0]["Resource_LanguageId"]);
                            else
                                languageId = "1";

                            if (userProfileEditCredential.Address1 != null && userProfileEditCredential.Address1 != "")
                                address1 = userProfileEditCredential.Address1.Trim();
                            else
                                address1 = Convert.ToString(dst.Tables[0].Rows[0]["Users_Address1"]);

                            if (userProfileEditCredential.Address2 != null && userProfileEditCredential.Address2 != "")
                                address2 = userProfileEditCredential.Address2.Trim();
                            else
                                address2 = Convert.ToString(dst.Tables[0].Rows[0]["Users_Address2"]);

                            if (userProfileEditCredential.City != null && userProfileEditCredential.City != "")
                                city = userProfileEditCredential.City.Trim();
                            else
                                city = Convert.ToString(dst.Tables[0].Rows[0]["Users_City"]);

                            if (userProfileEditCredential.ZipCode != null && userProfileEditCredential.ZipCode != "")
                                zipeCode = userProfileEditCredential.ZipCode.Trim();
                            else
                                zipeCode = Convert.ToString(dst.Tables[0].Rows[0]["Users_ZipCode"]);

                            if (userProfileEditCredential.Mobile != null && userProfileEditCredential.Mobile != "")
                                mobile = userProfileEditCredential.Mobile.Trim();
                            else
                                mobile = Convert.ToString(dst.Tables[0].Rows[0]["Users_Mobile"]);

                            if (userProfileEditCredential.Phone != null && userProfileEditCredential.Phone != "")
                                phone = userProfileEditCredential.Phone.Trim();
                            else
                                phone = Convert.ToString(dst.Tables[0].Rows[0]["Users_Phone"]);

                            studentId = Convert.ToString(dst.Tables[0].Rows[0]["Users_StudentId"]);

                            spParam.Append("1").Append(colSeperator).Append(salutation);

                            if (firstName != "")
                                spParam.Append(colSeperator).Append("2").Append(colSeperator).Append(firstName);

                            if (lastName != "")
                                spParam.Append(colSeperator).Append("4").Append(colSeperator).Append(lastName);

                            if (address1 != "")
                                spParam.Append(colSeperator).Append("5").Append(colSeperator).Append(address1);

                            if (phone != "")
                                spParam.Append(colSeperator).Append("6").Append(colSeperator).Append(phone);

                            if (mobile != "")
                            {
                                if (countryId == "2")
                                {
                                    if (mobile.StartsWith("1") == false)
                                    {
                                        mobile = "1" + mobile;
                                    }
                                }
                                else if (countryId == "1")
                                {
                                    if (mobile.StartsWith("91") == false)
                                    {
                                        mobile = "91" + mobile;
                                    }
                                }
                                spParam.Append(colSeperator).Append("7").Append(colSeperator).Append(mobile);
                            }
                            spParam.Append(colSeperator).Append("9").Append(colSeperator).Append(email);

                            spParam.Append(colSeperator).Append("11").Append(colSeperator).Append(languageId);
                            if (UserId != "")
                            {
                                spParam.Append(colSeperator).Append("12").Append(colSeperator).Append(UserId);
                                spParam.Append(colSeperator).Append("14").Append(colSeperator).Append(UserId);
                            }

                            if (timeZone != "")
                                spParam.Append(colSeperator).Append("17").Append(colSeperator).Append(timeZone);

                            if (city != "")
                            {
                                spParam.Append(colSeperator).Append("18").Append(colSeperator).Append(city);
                            }
                            if (stateId != "")
                            {
                                spParam.Append(colSeperator).Append("19").Append(colSeperator).Append(stateId);
                            }

                            if (countryId != "")
                            {
                                spParam.Append(colSeperator).Append("20").Append(colSeperator).Append(countryId);
                            }

                            if (zipeCode != "")
                                spParam.Append(colSeperator).Append("21").Append(colSeperator).Append(zipeCode);

                            if (address2 != "")
                                spParam.Append(colSeperator).Append("22").Append(colSeperator).Append(address2);

                            if (studentId != "")
                            {
                                spParam.Append(colSeperator).Append("30").Append(colSeperator).Append(studentId);
                            }
                            if (!string.IsNullOrEmpty(userProfileEditCredential.strImage))
                            {
                                try
                                {
                                    Educosoft.Api.Utilities.FileUpload fileObj = new Educosoft.Api.Utilities.FileUpload();
                                    dstFileInfo = fileObj.GetVirtualPath("1" + colSeparator + "FP16");
                                    if (dstFileInfo.Tables[0].Rows[0][0].ToString() == "0")
                                    {
                                        imagePath = dstFileInfo.Tables[0].Rows[0][5].ToString();
                                        imagePath = imagePath + UserId;
                                        if (userProfileEditCredential.strImage.Length > 100)
                                        {
                                            if (!Directory.Exists(imagePath))
                                                Directory.CreateDirectory(imagePath);

                                            if (userProfileEditCredential.strImageType != null && userProfileEditCredential.strImageType != "")
                                                imgExtn = "." + userProfileEditCredential.strImageType.Trim().ToLower();

                                            string[] files = Directory.GetFiles(imagePath);
                                            foreach (string file in files)
                                            {
                                                File.Delete(file);
                                            }

                                            File.WriteAllBytes(imagePath + "\\" + UserId + imgExtn, Convert.FromBase64String(userProfileEditCredential.strImage));

                                            imageName = UserId + imgExtn;
                                            if (imageName != "")        //imageName
                                                spParam.Append(colSeperator).Append("13").Append(colSeperator).Append(imageName);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    msg_ImageSaveStatus = " Image save status: " + ex.Message;
                                }
                            }
                            else
                            {
                                imageName = Convert.ToString(dst.Tables[0].Rows[0]["Users_Image"]);
                                if (imageName != "")        //imageName
                                    spParam.Append(colSeperator).Append("13").Append(colSeperator).Append(imageName);
                            }
                            dst = null;
                            SurveyAppUser objapiUser = new SurveyAppUser();
                            dst = objapiUser.UpdateUser(spParam.ToString());
                            if (dst.Tables[0].Rows[0][0].ToString() == "0")
                            {
                                var resMessage = new
                                {
                                    status = "1",
                                    message = "User has updated successfully",
                                };
                                return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200

                            }
                            else
                            {
                                var resMessage = new
                                {
                                    status = "1",
                                    message = dst.Tables[0].Rows[0][2].ToString(),
                                };
                                return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200
                            }
                        }
                        else
                        {
                            var resMessage = new
                            {
                                status = "0",
                                message = "No data found",
                            };
                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200
                        }
                    }
                    catch (Exception ex)
                    {
                        string ExMessage = "Exception occured while EditUser user's information";
                        ExMessage = ex.Message;

                        var resMessage = new
                        {
                            status = "0",
                            message = ExMessage
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                    }
                }
                else
                {
                    var resMessage = new
                    {
                        status = "0",
                        message = "EditUser credential is required,'UserId' along with one field needed from EditUser credential"
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                }
            }
            else
            {
                var resMessage = new
                {
                    status = "0",
                    message = "Invalid/malformed input(EditUser credential data)"
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400                 
            }

            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while transacting the information"
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201
        }
        #endregion

        #region
        /// <summary>
        ///This api call takes GetCountryList 
        ///returns Country list details like 'countryId', 'country_Name', with 'status' and 'message'.
        ///'Status':'1' means Success and 'Status':'0' means failure/error in transaction of information.
        /// </summary>
        /// <returns>
        ///Success:
        ///{
        ///   "status": "1",
        ///   "message": "Country list are successfully verified",
        ///   "response:":
        ///     {
        ///         "country_Name":"",
        ///         "countryId":""
        ///     }
        ///}
        ///Error:
        ///{
        ///     "status": "0",
        ///     "message": "Country list does not exist / Error message regarding the transaction failure"
        ///}
        /// </returns>
        [HttpGet]
        [ActionName("GetCountryList")]
        public HttpResponseMessage GetCountryList()
        {
            DataSet dst;
            User objUser = new User();
            string CountryId = null;
            try
            {
                dst = objUser.GetCountries(CountryId);
                if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                {
                    var lsitContries = dst.Tables[0].AsEnumerable().ToList();
                    var queryState = from country in lsitContries
                                     select new
                                     {
                                         countryId = country["intPkVal"],
                                         country_Name = country["Country_Name"]
                                     };
                    var resMessage = new
                    {
                        status = "1",
                        message = "Country list are successfully verified",
                        response = queryState
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200
                }
                else
                {
                    var resMessage = new
                    {
                        status = "0",
                        message = "No data found",
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200
                }
            }
            catch (Exception ex)
            {

                var resMessage = new
                {
                    status = "0",
                    message = "Exception occured while retreaving country list information"
                };

                return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 200

            }

            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while transacting the information"
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201
        }
        #endregion

        #region
        /// <summary>
        ///This api call takes GetStateList param required to specify in query string. i.e., 'CountryId' and
        ///returns State list details like 'stateId', 'state_CountryId', 'state_Name' with 'status' and 'message'.
        ///'Status':'1' means Success and 'Status':'0' means failure/error in transaction of information.
        /// </summary>
        /// <param name="CountryId">GetStateList param required to specify in query string.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": "1",
        ///   "message": "State list are successfully verified",
        ///   "response:":
        ///     {
        ///         "stateId":"",
        ///         "state_CountryId":"",
        ///         "state_Name":"",
        ///     }
        ///}
        ///Error:
        ///{
        ///     "status": "0",
        ///     "message": "CountryId does not exist / Error message regarding the transaction failure"
        ///}
        /// </returns>
        [HttpGet]
        [ActionName("GetStateList")]
        public HttpResponseMessage GetStateList(string CountryId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DataSet dst;
                    User objUser = new User();

                    if (CountryId != null && CountryId != "")
                    {
                        string countryId = CountryId.Trim();
                        dst = objUser.GetState("", countryId);
                        if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                        {
                            var listContries = dst.Tables[0].AsEnumerable().ToList();
                            var queryResult = (from state in listContries
                                               select new
                                               {
                                                   stateId = Convert.ToString(state["intPkVal"]),
                                                   state_CountryId = Convert.ToString(state["State_CountryId"]),
                                                   state_Name = Convert.ToString(state["State_Name"])
                                                   //state_Abbrevation =Convert.ToString(state["State_Abbrevation"]),
                                               }).ToList();

                            var resMessage = new
                            {
                                status = "1",
                                message = "State list are successfully verified",
                                response = queryResult
                            };
                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200
                        }
                        else
                        {
                            var resMessage = new
                            {
                                status = "0",
                                message = "No data found",
                            };
                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200
                        }
                    }
                    else
                    {
                        var resMessage = new
                        {
                            status = "0",
                            message = "CountryId param required.",
                        };
                        return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200
                    }
                }
                catch (Exception ex)
                {
                    var resMessage = new
                    {
                        status = "0",
                        message = "Exception occured while GetStateList information"
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                }
            }
            else
            {
                var resMessage = new
                {
                    status = "0",
                    message = "Invalid/malformed input(GetStateList credential data)"
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400  
            }

            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while transacting the information"
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201

        }
        #endregion

        #region
        /// <summary>
        ///This api call takes User Support credential i.e., 'UserId', 'SectionId', 'UserType', 'PhoneNumber', 'TellAboutText', 'AttachmentFile', 'Date', 'Time' and
        ///returns Support details like 'UserId' with 'status' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in transaction of information.
        /// Note: 1) Optional Fields are 'AttachmentFile', Date', 'Time', 2)'AttachmentFile': AttachmentFile' is in Base64String format
        /// </summary>
        /// <param name="userSupportCredential">UserSupport credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": "1",
        ///   "message": Your support email has been successfully sent.",
        ///   "response:":
        ///     {
        ///         "UserId":"",
        ///     }
        ///}
        ///Error:
        ///{
        ///     "status": "0",
        ///     "message": "User does not exist / Error message regarding the transaction failure"
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("Support")]
        public HttpResponseMessage Support([ModelBinder] UserSupportCredential userSupportCredential)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (!string.IsNullOrEmpty(userSupportCredential.UserId) && !string.IsNullOrEmpty(userSupportCredential.SectionId) && !string.IsNullOrEmpty(userSupportCredential.PhoneNumber)
                        && !string.IsNullOrEmpty(userSupportCredential.TellAboutText) && !string.IsNullOrEmpty(userSupportCredential.UserType))
                    {

                        char colSeperator = (char)195;
                        DataSet dst;
                        DateTime dtCurrentDate;
                        TimeZoneDateTime ObjTimeZoneDateTime = null;
                        string UserId = "", CountryId = "", InstutionId = "", SectionId = "", UserType = "";
                        string fullName = "", eMail = "", PhoneNumber = "", institution = "", AboutText = "";
                        string Date = "", Time = "";
                        bool exitsectionId = false;
                        UserId = userSupportCredential.UserId;
                        SectionId = userSupportCredential.SectionId;
                        UserType = userSupportCredential.UserType;
                        PhoneNumber = userSupportCredential.PhoneNumber;
                        AboutText = userSupportCredential.TellAboutText;

                        if (!string.IsNullOrEmpty(userSupportCredential.Date))
                        {
                            Date = userSupportCredential.Date;
                        }
                        if (!string.IsNullOrEmpty(userSupportCredential.Time))
                        {
                            Time = userSupportCredential.Time;
                        }

                        User objUser = new User();
                        dst = objUser.GetUserInfo(UserId);
                        if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                        {
                            Educo.ELS.Encryption.EncryptDecrypt objEncrypt = new Educo.ELS.Encryption.EncryptDecrypt();
                            fullName = dst.Tables[0].Rows[0][9].ToString() + ", " + dst.Tables[0].Rows[0][7].ToString();
                            eMail = objEncrypt.Decrypt(dst.Tables[0].Rows[0][4].ToString()).ToLower();

                            if (!string.IsNullOrEmpty(Convert.ToString(dst.Tables[0].Rows[0][28])))
                            {
                                InstutionId = dst.Tables[0].Rows[0][28].ToString();

                            }
                            CountryId = dst.Tables[0].Rows[0]["Users_Country"].ToString();

                            foreach (DataRow item in dst.Tables[0].Rows)
                            {
                                if (SectionId == item["CRUserCourses_SectionId"].ToString())
                                    exitsectionId = true;
                            }
                            if (exitsectionId)
                            {
                                bool result = objUser.UserSupport(UserId, SectionId, UserType, PhoneNumber, AboutText,
                                                                  "", CountryId, Date, Time, fullName, eMail,
                                                                  InstutionId);
                                if (result)
                                {
                                    var objSupportUserResponseDetails = new
                                                                            {
                                                                                UserId = UserId
                                                                            };
                                    var resMessage = new
                                                         {
                                                             status = "1",
                                                             message = "Your support email has been successfully sent.",
                                                             response = objSupportUserResponseDetails,
                                                         };
                                    return Request.CreateResponse(HttpStatusCode.OK, resMessage);
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
                            message = "User support credential are required"
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                    }

                }
                catch (Exception ex)
                {
                    string ExMessage = "Exception occured while sending user's mail information";
                    ExMessage = ex.Message;

                    var resMessage = new
                    {
                        status = "0",
                        message = ExMessage
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                }
            }
            else
            {
                var resMessage = new
                {
                    status = "0",
                    message = "Invalid/malformed input(user support credential data)"
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400                 
            }

            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while transacting the information"
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201

        }
        #endregion

        private string GetCountry(string PortalId)
        {
            string countryCode = "N/A";
            if (ConfigurationManager.AppSettings["IndiaPortalID"] != null && ConfigurationManager.AppSettings["JamaicaPortalID"] != null &&
                ConfigurationManager.AppSettings["MIDEASTPortalID"] != null && ConfigurationManager.AppSettings["PPPortalID"] != null &&

                ConfigurationManager.AppSettings["MarshallIslandsPortalID"] != null && ConfigurationManager.AppSettings["NigeriaPortalID"] != null &&
                ConfigurationManager.AppSettings["CanadaPortalID"] != null && ConfigurationManager.AppSettings["USPortalID"] != null)
            {
                if (ConfigurationManager.AppSettings["IndiaPortalID"].ToString().Trim() == PortalId)
                    countryCode = "IN";
                else if (ConfigurationManager.AppSettings["JamaicaPortalID"].ToString() == PortalId)
                    countryCode = "JM";
                else if (ConfigurationManager.AppSettings["MIDEASTPortalID"].ToString() == PortalId)
                    countryCode = "GU";
                else if (ConfigurationManager.AppSettings["PPPortalID"].ToString() == PortalId)
                    countryCode = "PP";
                else if (ConfigurationManager.AppSettings["MarshallIslandsPortalID"].ToString() == PortalId)
                    countryCode = "MP";
                else if (ConfigurationManager.AppSettings["NigeriaPortalID"].ToString() == PortalId)
                    countryCode = "NG";
                else if (ConfigurationManager.AppSettings["CanadaPortalID"].ToString() == PortalId)
                    countryCode = "CA";
                else if (ConfigurationManager.AppSettings["USPortalID"].ToString() == PortalId)
                    countryCode = "US";
            }
            return countryCode;
        }

        private string GetStateCountryName(string StateCountryId, int flagStateCountry)
        {
            DataSet dstStateCountry = new DataSet();
            string spParamReturnValue = "";
            User objUser = new User();
            if (flagStateCountry == 1)//Country
            {
                dstStateCountry = objUser.GetCountries(StateCountryId);
            }
            else if (flagStateCountry == 0)//state
            {
                dstStateCountry = objUser.GetState(StateCountryId, "");
            }
            spParamReturnValue = "";
            if (dstStateCountry.Tables[0].Rows[0][0].ToString() == "0")
            {
                if (flagStateCountry == 1)//Country
                {
                    spParamReturnValue = dstStateCountry.Tables[0].Rows[0][4].ToString();
                }
                else if (flagStateCountry == 0)//state
                {
                    spParamReturnValue = dstStateCountry.Tables[0].Rows[0][5].ToString();
                }
            }
            return spParamReturnValue;
        }

        private void SendForgetPasswordExternalMail(string username, string password, string display_name)
        {
            //Object
            AutomatedMails objAutomatedMails = new AutomatedMails();
            DataSet dstForgotPassword = new DataSet();
            StringBuilder paramList = new StringBuilder();

            //Variables
            string msgbody = "";
            string msgsalutation = "";
            string msgEnding = "";
            string msgSubject = "";
            string msgText = "";
            char colSep = (char)195;
            string langFile = "Strings-En";

            //ParamList
            paramList.Length = 0;
            paramList.Append("1").Append(colSep).Append("73");

            dstForgotPassword = objAutomatedMails.getExternalMailDetails(paramList.ToString(), langFile);
            if (dstForgotPassword.Tables[0].Rows[0][0].ToString() == "0")
            {
                msgbody = dstForgotPassword.Tables[0].Rows[0][6].ToString();
                msgsalutation = dstForgotPassword.Tables[0].Rows[0][10].ToString();
                msgEnding = dstForgotPassword.Tables[0].Rows[0][11].ToString();
                msgSubject = dstForgotPassword.Tables[0].Rows[0][5].ToString();
            }
            msgbody = msgbody.Replace("DISP_NAME", display_name.ToString());
            msgbody = msgbody.Replace("USER_NAME", username.ToLower().ToString());
            msgbody = msgbody.Replace("PASS_WORD", password.ToString());
            msgText = msgsalutation.ToString() + "<BR>" + msgbody.ToString() + "<BR>" + msgEnding.ToString();

            //Send Email
            objAutomatedMails.ExternalMail(username, msgSubject.ToString(), msgText.ToString(), langFile);

            objAutomatedMails = null;
            dstForgotPassword = null;
            paramList = null;
        }

        private void SendResetPwdExternalMail(string email, string password, string display_name)
        {
            //Variables
            string msgSubject = string.Empty;
            string langFile = "Strings-En";

            //Objects
            AutomatedMails objAutomatedMail = new AutomatedMails();
            StringBuilder msgBody = new StringBuilder();

            msgBody.Append("Dear ").Append(display_name).Append(",<BR><BR>");
            msgBody.Append("Your Password has been changed.<BR>");
            msgBody.Append("Your User Name is: ").Append(email.ToLower()).Append("<BR>");
            msgBody.Append("Your new password is: ").Append(password);
            msgSubject = "Password changed";

            msgBody.Append("<BR><BR>Educo Team<BR><BR><BR>This is system generated e-mail so please do not reply to this e-mail.<BR>");
            msgBody.Append("If you have any questions about your account then please send an e-mail to: educosoftsupport@educo-int.com.<BR>");

            objAutomatedMail.ExternalMail(email, msgSubject.ToString(), msgBody.ToString(), langFile);

        }

    }
}
