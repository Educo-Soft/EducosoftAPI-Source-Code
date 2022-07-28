using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using EducosoftAPI.Models;
using System.Data;
using System.Web.Http.Description;
using EducosoftAPI.Models.User;

namespace EducosoftAPI.Controllers
{
    public class SurverUserController : ApiController
    {
        #region
        ///<summary>
        ///This api is for testing purpose only.
        /// </summary>    
        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetTestUser()
        {
            return "Educosoft Value: Indranarayan Bishi";
        }
        #endregion

        #region
        /// <summary>
        ///This api call takes SurveyUser credential i.e., 'Name', 'Email', 'Mobile No' and
        ///return SurveyUserId. Note: If user already exists with the Email id then Name and Mobile No will be updated.
        ///with 'status' and 'message' upon successful transaction of information.
        ///Incase of failure, it returns only 'status' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in transaction of information.
        /// </summary>
        /// <param name="userCredential">SurveyUser credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": "1",
        ///   "message": "Success message related to transaction of information",
        ///   "response":
        ///   {                      
        ///	    "SurveyUserId":"18"
        ///   }
        ///}
        ///Error:
        ///{
        ///     "status": "0",
        ///     "message": "Error message related to transaction of information"
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("SaveSurveyUser")]
        public HttpResponseMessage ECFSaveSurveyUser([ModelBinder] UserCredential userCredential)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (userCredential.Name == null || userCredential.Email == null || userCredential.MobileNo == null)
                    {
                        if (userCredential.Name == null)
                            throw new ArgumentNullException("Name", "Parameter 'Name' is required & non-null");
                        if (userCredential.Email == null)
                            throw new ArgumentNullException("Email", "Parameter 'Email' is required & non-null");
                        if (userCredential.MobileNo == null)
                            throw new ArgumentNullException("MobileNo", "Parameter 'MobileNo' is required & non-null");
                    }
                    if (userCredential.Name.Trim() != "" && userCredential.Email.Trim() != "" && userCredential.MobileNo.Trim() != "")
                    {
                        User objUser = new User();
                        DataSet dst = objUser.SaveSurveyUser(userCredential.Name.Trim(), userCredential.Email.Trim(), userCredential.MobileNo.Trim());

                        if (dst != null && dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0)
                        {
                            if (dst.Tables[0].Rows[0][0].ToString() == "0")
                            {
                                //----Response object
                                var objSurverUserResponseDetails = new
                                {
                                    SurveyUserId = dst.Tables[0].Rows[0]["intAppSurUserId"].ToString()
                                };

                                var resMessage = new
                                {
                                    status = "1",
                                    message = dst.Tables[0].Rows[0]["nVarErrorMsg"].ToString(),
                                    response = objSurverUserResponseDetails
                                };

                                return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200                             
                            }
                            else if (dst.Tables[0].Rows[0][0].ToString() == "-1")
                            {
                                var resMessage = new
                                {
                                    status = "0",
                                    message = dst.Tables[0].Rows[0]["nVarErrorMsg"].ToString()
                                };

                                return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                            }
                            else if (dst.Tables[0].Rows[0][0].ToString() == "-2")
                            {
                                var resMessage = new
                                {
                                    status = "0",
                                    message = dst.Tables[0].Rows[0]["nVarErrorMsg"].ToString()
                                };
                                return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200                             
                            }
                        }
                        else
                        {
                            var resMessage = new
                            {
                                status = "0",
                                message = "Sql Server return no data"
                            };
                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                        }
                    }
                    else
                    {
                        var resMessage = new
                        {
                            status = "0",
                            message = "SaveSurveyUser credential are required"
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                    }
                }
                catch (Exception ex)
                {
                    string ExMessage = "Exception occured while saving/updating of information";
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
                    message = "Invalid/malformed input(SurveyUser credential data)"
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
        ///This api call takes User Credential SurveyFeedback i.e., 'Email', 'Feedback' and
        ///return SurveyUserId, SurveyFeedbackId . Note: If user's feedback already exists with the Email id then feedback data will be updated.
        ///with 'status' and 'message' upon successful transaction of information.
        ///Incase of failure, it returns only 'status' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in transaction of information.
        /// </summary>
        /// <param name="userCredentialSurveyFeedback">SurveyFeedback User Credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": 1,
        ///   "message": "Success message related to transaction of information",
        ///   "response":
        ///   {               
        ///		SurveyFeedbackId=13,       
        ///		SurveyUserId=18        
        ///   }
        ///}
        ///Error:
        ///{
        ///"status": 0,
        ///"message": "Error message related to transaction of information"
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("SaveSurveyFeedback")]
        public HttpResponseMessage ESP_AppSaveSurveyFeedback([ModelBinder] UserCredentialSurveyFeedback userCredentialSurveyFeedback)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (userCredentialSurveyFeedback.Email == null || userCredentialSurveyFeedback.Feedback == null)
                    {
                        if (userCredentialSurveyFeedback.Email == null)
                            throw new ArgumentNullException("Email", "Parameter 'Email' is required & non-null");
                        if (userCredentialSurveyFeedback.Feedback == null)
                            throw new ArgumentNullException("Feedback", "Parameter 'Feedback' is required & non-null");
                    }
                    if (userCredentialSurveyFeedback.Email.Trim() != "" && userCredentialSurveyFeedback.Feedback.Trim() != "")
                    {
                        User objUser = new User();
                        DataSet dst = objUser.SaveSurveyFeedback(userCredentialSurveyFeedback.Email.Trim(), userCredentialSurveyFeedback.Feedback.Trim());

                        if (dst != null && dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0)
                        {
                            if (dst.Tables[0].Rows[0][0].ToString() == "0")
                            {
                                //----Response object
                                var objSurverFeedbackResponseDetails = new
                                {
                                    SurveyUserId = dst.Tables[0].Rows[0]["intAppSurUserId"].ToString(),
                                    SurveyFeedbackId = dst.Tables[0].Rows[0]["intAppSurFeedbackId"].ToString()
                                };

                                var resMessage = new
                                {
                                    status = "1",
                                    message = dst.Tables[0].Rows[0]["nVarErrorMsg"].ToString(),
                                    response = objSurverFeedbackResponseDetails
                                };

                                return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200                             
                            }
                            else if (dst.Tables[0].Rows[0][0].ToString() == "-1")
                            {
                                var resMessage = new
                                {
                                    status = "0",
                                    message = dst.Tables[0].Rows[0]["nVarErrorMsg"].ToString()
                                };

                                return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                            }
                            else if (dst.Tables[0].Rows[0][0].ToString() == "-2")
                            {
                                var resMessage = new
                                {
                                    status = "0",
                                    message = dst.Tables[0].Rows[0]["nVarErrorMsg"].ToString()
                                };
                                return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200                             
                            }
                        }
                        else
                        {
                            var resMessage = new
                            {
                                status = "0",
                                message = "Sql Server return no data"
                            };
                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                        }
                    }
                    else
                    {
                        var resMessage = new
                        {
                            status = "0",
                            message = "SaveSurveyUser credential are required"
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                    }
                }
                catch (Exception ex)
                {
                    string ExMessage = "Exception occured while saving/updating of information";
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
                    message = "Invalid/malformed input(SurveyUser credential data)"
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
    }
}
