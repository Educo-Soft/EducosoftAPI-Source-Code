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

namespace EducosoftAPI.Controllers
{
    public class EducoUserController : ApiController
    {
        #region
        /// <summary>
        ///This api call takes VerifyLogin credential i.e., 'Email', 'Password' and
        ///returns 'status', 'country' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in transaction of information.
        /// </summary>
        /// <param name="userVerifyLoginCredential">VerifyLogin credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": "1",
        ///   "message": "User is successfully verified",
        ///   "country" : "US"
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

                        char colSep = (char)195;
                        UserLogin objUserLogin = new UserLogin();
                        string UserLoginDtls = objUserLogin.VerifyAppMaster(userVerifyLoginCredential.Email.Trim().ToUpper(), userVerifyLoginCredential.Password.Trim());
                        string instUserDtls = UserLoginDtls;
                        string[] instUsrLogins = instUserDtls.Split('$');

                        if (instUsrLogins.Length > 0 && instUsrLogins[0].ToUpper() == "TRUE") //Institution user
                        {
                            var resMessage = new
                            {
                                status = "1",
                                message = "User is successfully verified",
                                country = GetCountry(instUsrLogins[5].ToString())
                            };

                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200
                        }
                        else
                        {
                            var resMessage = new
                            {
                                status = "0",
                                message = "Invalid user credentials...!!! Please check your Username and Password.",
                                country = UserLoginDtls
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
                    //if (ex.GetType() == typeof(ArgumentNullException))
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
    }
}
