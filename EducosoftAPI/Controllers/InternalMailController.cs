using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EducosoftAPI.Models.Announcement;
using System.Web.Http.ModelBinding;
using Educosoft.Api.Communication;
using EducosoftAPI.Models.User;
using EducosoftAPI.Models.InternalMail;
using System.Data;
using System.Text;
using EducosoftAPI.Models;
using Educo.ELS.Encryption;
using System.IO;

namespace EducosoftAPI.Controllers
{
    public class InternalMailController : ApiController
    {
        /// <summary>
        ///This api call takes InternalMailCredential i.e., 'UserId' and 
        ///return InternalMail list details like 'MsgRecieverId', 'MsgSenderId', 'MsgSender_Date', 'MsgSender_Attachment', 'MsgSender_Body', 'MsgSender_Subject', 'MsgSender_Sender_SectionId', 'Section_Name', 'Course_Name', 'LastName', 'FirstName', 'SectionId'
        ///with 'status' and 'message' upon successful retrieval of InternalMail information.
        ///Incase of failure, it returns only 'status' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in retrieving InternalMail information.
        /// </summary>
        /// <param name="InternalMailCredential">InternalMail retrieval credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": 1,
        ///   "message": "Succesfully retrieved InternalMail information.",
        ///   "response": 
        ///      {  
        ///        "MsgRecieverId": "",
        ///        "MsgSender_Date": "",
        ///        "MsgSender_Attachment": "",
        ///        "MsgSender_Body":"",
        ///        "MsgSender_Subject":"",
        ///        "MsgSender_Sender_SectionId":"",
        ///        "Section_Name":"",
        ///        "Course_Name":"",
        ///        "LastName":"",
        ///        "FirstName":"",
        ///        "SectionId":""
        ///   }
        ///Error:
        ///{
        ///"status": 0,
        ///"message": "User does not have access to section/Error while retrieving Internal Mail information."
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("GetInternalMailList")]
        public HttpResponseMessage GetInternalMailList([ModelBinder] InternalMailCredential InternalMailCredential)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(InternalMailCredential.UserId))
                {
                    try
                    {
                        Api_InternalMail objInterMail = new Api_InternalMail();
                        char colSeperator = (char)195;
                        DataSet dst, dstFileInfo;
                        StringBuilder spParam = new StringBuilder();
                        string UserId = "", UserCurrZoneDate = "", Tz_Name = "", TzID = "";
                        string strUserZoneCurrDate = string.Empty;


                        UserId = InternalMailCredential.UserId.Trim();

                        User objUser = new User();
                        dst = objUser.GetUserInfo(UserId);
                        if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                        {
                            Tz_Name = dst.Tables[0].Rows[0]["Tz_Name"].ToString();
                            TzID = dst.Tables[0].Rows[0]["Resource_TzId"].ToString();
                            DateTime dtCurrentDate = DateTime.Now;
                            TimeZoneDateTime ObjTimeZoneDateTime = new TimeZoneDateTime();
                            strUserZoneCurrDate = ObjTimeZoneDateTime.GetUserTimeZoneDateTime(Tz_Name, dtCurrentDate).ToString();

                            spParam.Length = 0;
                            spParam.Append("1").Append(colSeperator).Append(UserId).Append(colSeperator);
                            spParam.Append("2").Append(colSeperator).Append("1").Append(colSeperator);
                            spParam.Append("8").Append(colSeperator).Append(DateTime.Parse(strUserZoneCurrDate)).Append(colSeperator);
                            spParam.Append("9").Append(colSeperator).Append("2");

                            dst = null;

                            dst = objInterMail.GetMessage(spParam.ToString());

                            if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                            {

                                var userInternalMailList = dst.Tables[0].AsEnumerable().ToList();

                                var result = (from dr in userInternalMailList
                                              select new
                                              {
                                                  MsgRecieverId = dr["intPkVal"],
                                                  MsgSenderId = dr["MsgSenderId"],
                                                  MsgSender_Date = dr["MsgSender_Date"].ToString(),
                                                  MsgSender_Attachment = dr["MsgSender_Attachment"],
                                                  MsgSender_Body = dr["MsgSender_Body"],
                                                  MsgSender_Subject = dr["MsgSender_Subject"],
                                                  MsgSender_Sender_SectionId = dr["MsgSender_Sender_SectionId"],
                                                  Section_Name = dr["Section_Name"],
                                                  Course_Name = dr["Course_Name"],
                                                  LastName = dr["LastName"],
                                                  FirstName = dr["FirstName"],
                                                  SectionId = dr["SectionId"],
                                              }).ToList();


                                var resMessage = new
                                {
                                    status = "1",
                                    message = "Successfully retrieved Internal Mail information",
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
                            message = "Exception while retrieving Internal Mail information"
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                    }
                }
                else
                {
                    var resMessage = new
                    {
                        status = "0",
                        message = "InternalMail info retrieval credentials are required"
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                }

            }
            else
            {
                var resMessage = new
                {
                    status = "0",
                    message = "Invalid/malformed input(InternalMail credential)"
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400                 
            }

            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while retrieving Internal Mail information."
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201
        }

        /// <summary>
        ///This api call takes InternalMailViewCredential i.e., 'UserId', 'MsgSenderId' and 
        ///return InternalMail list details like 'MsgRecieverId', 'MsgSenderId', 'MsgSender_Date', 'MsgSender_Attachment', 'MsgSender_Body', 'MsgSender_Subject', 'MsgSender_Sender_SectionId', 'Section_Name', 'Course_Name', 'LastName', 'FirstName', 'SectionId', 'FileAttachmentlist' ,'FromRecieveMailIds'
        ///with 'status' and 'message' upon successful retrieval of InternalMail information.
        ///Incase of failure, it returns only 'status' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in retrieving InternalMail information.
        /// </summary>
        /// <param name="InternalMailViewCredential">InternalMailView retrieval credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": 1,
        ///   "message": "Succesfully retrieved InternalMail information.",
        ///   "response": 
        ///      {  
        ///        "MsgRecieverId": "",
        ///        "MsgSender_Date": "",
        ///        "MsgSender_Attachment": "",
        ///        "MsgSender_Body":"",
        ///        "MsgSender_Subject":"",
        ///        "MsgSender_Sender_SectionId":"",
        ///        "Section_Name":"",
        ///        "Course_Name":"",
        ///        "LastName":"",
        ///        "FirstName":"",
        ///        "SectionId":"",
        ///        "FileAttachmentlist":"",
        ///        "FromRecieveMailIds":"",
        ///   }
        ///Error:
        ///{
        ///"status": 0,
        ///"message": "User does not have access to section/Error while retrieving Internal Mail information."
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("GetInternalMailView")]
        public HttpResponseMessage GetInternalMailView([ModelBinder] InternalMailViewCredential InternalMailViewCredential)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(InternalMailViewCredential.UserId) && !string.IsNullOrEmpty(InternalMailViewCredential.MsgSenderId))
                {
                    try
                    {
                        Api_InternalMail objInterMail = new Api_InternalMail();
                        char colSeperator = (char)195;
                        DataSet dst, dstFileInfo;
                        StringBuilder spParam = new StringBuilder();
                        string UserId = "", UserCurrZoneDate = "", SenderId = "", FilePath = "", Tz_Name = "", TzID = "";
                        string strUserZoneCurrDate = string.Empty;
                        EncryptDecrypt objDecrypt = null;

                        UserId = InternalMailViewCredential.UserId.Trim();
                        SenderId = InternalMailViewCredential.MsgSenderId.Trim();
                        User objUser = new User();

                        dst = objUser.GetUserInfo(UserId);
                        if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                        {
                            Tz_Name = dst.Tables[0].Rows[0]["Tz_Name"].ToString();
                            TzID = dst.Tables[0].Rows[0]["Resource_TzId"].ToString();

                            DateTime dtCurrentDate = DateTime.Now;
                            TimeZoneDateTime ObjTimeZoneDateTime = new TimeZoneDateTime();
                            strUserZoneCurrDate = ObjTimeZoneDateTime.GetUserTimeZoneDateTime(Tz_Name, dtCurrentDate).ToString();

                            spParam.Length = 0;
                            spParam.Append("1").Append(colSeperator).Append(UserId).Append(colSeperator);
                            spParam.Append("2").Append(colSeperator).Append("1").Append(colSeperator);
                            spParam.Append("8").Append(colSeperator).Append(DateTime.Parse(strUserZoneCurrDate)).Append(colSeperator);
                            spParam.Append("9").Append(colSeperator).Append("2");

                            dst = null;

                            dst = objInterMail.GetMessage(spParam.ToString());

                            if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                            {
                                List<DataRow> listInternalMailList = null;
                                listInternalMailList = dst.Tables[0].Select("MsgSenderId = " + SenderId + "").AsEnumerable().ToList();

                                if (listInternalMailList != null && listInternalMailList.Count > 0)
                                {
                                    dst = null;
                                    List<DataRow> listAttachmentInfo = null;
                                    List<DataRow> listMessageReciverInfo = null;
                                    var strlistAttachmentInfo = new List<string>();
                                    var strlistMessageReciverInfo = new List<string>();

                                    dst = objInterMail.GetMessageReciverId("1" + colSeperator + SenderId);

                                    if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                                    {
                                        listMessageReciverInfo = dst.Tables[0].AsEnumerable().ToList();
                                    }
                                    if (listMessageReciverInfo != null && listMessageReciverInfo.Count > 0)
                                    {
                                        objDecrypt = new EncryptDecrypt();

                                        var varMessageReciverlist = (from dr in listMessageReciverInfo
                                                                     select new
                                                                     {
                                                                         UserId = dr["intPkVal"],
                                                                         MsgReciever_MsgSenderId = dr["MsgReciever_MsgSenderId"],
                                                                         RecieverType = dr["MsgReciever_RecieverType"].ToString() == "T" ? "TO" : dr["MsgReciever_RecieverType"].ToString() == "B" ? "BCC" : "CC",
                                                                         Resource_Name = objDecrypt.Decrypt(Convert.ToString(dr["Resource_Name"])).ToLower(),
                                                                         Name = dr[7],
                                                                     }).ToList();


                                        dst = null;
                                        dst = objInterMail.GetMessageAttachments("1" + colSeperator + SenderId);
                                        if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                                        {
                                            Educosoft.Api.Utilities.FileUpload fileObj = new Educosoft.Api.Utilities.FileUpload();
                                            dstFileInfo = fileObj.GetVirtualPath("1" + colSeperator + "FP7");
                                            if (dstFileInfo.Tables[0].Rows[0][0].ToString() == "0")
                                                FilePath = dstFileInfo.Tables[0].Rows[0][4].ToString();

                                            listAttachmentInfo = dst.Tables[0].AsEnumerable().ToList();

                                        }

                                        if (listAttachmentInfo != null && listAttachmentInfo.Count > 0)
                                        {
                                            var varAttachmentlist = (from dr in listAttachmentInfo
                                                                     select new
                                                                     {
                                                                         MsgAttachId = dr["intPkVal"],
                                                                         MsgAttach_MsgSenderId = dr["MsgAttach_MsgSenderId"],
                                                                         MsgAttach_Attach_filename = dr["MsgAttach_Attach_filename"],
                                                                         MsgAttach_Attach_orgfilename = dr["MsgAttach_Attach_orgfilename"],
                                                                         FilePath = FilePath + Convert.ToString(dr["MsgAttach_Attach_filename"]),
                                                                     }).ToList();

                                            var result = (from dr in listInternalMailList
                                                          select new
                                                          {
                                                              MsgRecieverId = dr["intPkVal"],
                                                              MsgReciever_ResourceId = dr["MsgReciever_ResourceId"],
                                                              MsgSenderId = dr["MsgSenderId"],
                                                              MsgSender_Date = dr["MsgSender_Date"].ToString(),
                                                              MsgSender_Attachment = dr["MsgSender_Attachment"],
                                                              MsgSender_Msgsaved = dr["MsgSender_Msgsaved"],
                                                              MsgSender_Body = dr["MsgSender_Body"],
                                                              MsgSender_Subject = dr["MsgSender_Subject"],
                                                              MsgReciever_RecieverType = dr["MsgReciever_RecieverType"],
                                                              MsgReciever_Msgread = dr["MsgReciever_Msgread"],
                                                              MsgReciever_MsgFolderId = dr["MsgReciever_MsgFolderId"],
                                                              MsgSender_Sender_SectionId = dr["MsgSender_Sender_SectionId"],
                                                              Section_Name = dr["Section_Name"],
                                                              Course_Name = dr["Course_Name"],
                                                              LastName = dr["LastName"],
                                                              FirstName = dr["FirstName"],
                                                              SectionId = dr["SectionId"],
                                                              FileAttachmentlist = varAttachmentlist,
                                                              RecieverIds = varMessageReciverlist,
                                                          }).ToList();

                                            var resMessage = new
                                            {
                                                status = "1",
                                                message = "Successfully retrieved Internal Mail information",
                                                response = result,
                                            };

                                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200   
                                        }
                                        else
                                        {

                                            var result = (from dr in listInternalMailList
                                                          select new
                                                          {
                                                              MsgRecieverId = dr["intPkVal"],
                                                              MsgReciever_ResourceId = dr["MsgReciever_ResourceId"],
                                                              MsgSenderId = dr["MsgSenderId"],
                                                              MsgSender_Date = dr["MsgSender_Date"].ToString(),
                                                              MsgSender_Attachment = dr["MsgSender_Attachment"],
                                                              MsgSender_Msgsaved = dr["MsgSender_Msgsaved"],
                                                              MsgSender_Body = dr["MsgSender_Body"],
                                                              MsgSender_Subject = dr["MsgSender_Subject"],
                                                              MsgReciever_RecieverType = dr["MsgReciever_RecieverType"],
                                                              MsgReciever_Msgread = dr["MsgReciever_Msgread"],
                                                              MsgReciever_MsgFolderId = dr["MsgReciever_MsgFolderId"],
                                                              MsgSender_Sender_SectionId = dr["MsgSender_Sender_SectionId"],
                                                              Section_Name = dr["Section_Name"],
                                                              Course_Name = dr["Course_Name"],
                                                              LastName = dr["LastName"],
                                                              FirstName = dr["FirstName"],
                                                              SectionId = dr["SectionId"],
                                                              FileAttachmentlist = strlistAttachmentInfo,
                                                              FromRecieveMailIds = varMessageReciverlist,

                                                          }).ToList();

                                            var resMessage = new
                                            {
                                                status = "1",
                                                message = "Successfully retrieved Internal Mail information",
                                                response = result,
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
                    catch (Exception ex)
                    {
                        var resMessage = new
                        {
                            status = "0",
                            message = "Exception while retrieving Internal information"
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                    }
                }
                else
                {
                    var resMessage = new
                    {
                        status = "0",
                        message = "Internal Mail View info retrieval credentials are required"
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                }

            }
            else
            {
                var resMessage = new
                {
                    status = "0",
                    message = "Invalid/malformed input(Internal Mail credential)"
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400                 
            }

            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while retrieving Internal Mail information."
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201
        }

        /// <summary>
        ///This api call takes DeleteInternalMailCredential i.e., 'MsgRecieverId', and 
        ///return InternalMail list details like 'MsgRecieverId' with 'status' and 'message' upon successfully deleted internal Mail.
        ///Incase of failure, it returns only 'status' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in retrieving Delete InternalMail information.
        /// </summary>
        /// <param name="DeleteInternalMailCredential">DeleteInternalMail retrieval credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": 1,
        ///   "message": "Succesfully deleted internal Mail.",
        ///   "response": 
        ///      {  
        ///        "MsgRecieverId": "",
        ///     }
        ///Error:
        ///{
        ///"status": 0,
        ///"message": "User does not have access to section/Error while retrieving Internal Mail information."
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("DeleteMail")]
        public HttpResponseMessage DeleteMail([ModelBinder] DeleteInternalMailCredential DeleteInternalMailCredential)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(DeleteInternalMailCredential.MsgRecieverId))
                {
                    try
                    {
                        Api_InternalMail objInterMail = new Api_InternalMail();
                        char colSeperator = (char)195;
                        DataSet dst, dstFileInfo;
                        StringBuilder spParam = new StringBuilder();
                        string RecieverId = "";

                        RecieverId = DeleteInternalMailCredential.MsgRecieverId.Trim();
                        dst = objInterMail.DeleteMessage("1" + colSeperator + RecieverId);
                        if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                        {

                            var resMessage = new
                               {
                                   status = "1",
                                   message = "Successfully deleted internal mail.",
                                   response = RecieverId,
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

                    catch (Exception ex)
                    {
                        var resMessage = new
                        {
                            status = "0",
                            message = "Exception while deleting internal mail information"
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                    }
                }
                else
                {
                    var resMessage = new
                    {
                        status = "0",
                        message = "Delete internal mail info retrieval credentials are required"
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                }

            }
            else
            {
                var resMessage = new
                {
                    status = "0",
                    message = "Invalid/malformed input(Delete internal mail credential)"
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400                 
            }

            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while retrieving Delete internal mail information."
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201
        }

        /// <summary>
        ///This api call takes SendInternalMailCredential i.e., 'UserId', 'Subject', 'Body', 'MsgSendToUsersIds', 'MsgSendBccUsersIds' 'SectionId', 'AttachmentBase64StringFiles','AttachmentOriginalFiles' and 
        ///return SendInternalMail list details like 'UserId', 'NonAuthorizedUserIds' with 'status' and 'message' upon Message has been sent successfully.
        ///Incase of failure, it returns only 'status' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in retrieving Send InternalMail information.
        /// Note:  
        ///     1) 'NonAuthorizedUserIds': if you mention any NonAthorized user id(s) in 'MsgSendToUsersIds', 'MsgSendBccUsersIds' then those user id(s) has displays in this field.
        ///     2) 'AttachmentBase64StringFiles': is in Base64String format, if AttachmentBase64StringFiles are more than one then add file suffix with ','.
        ///     3) 'AttachmentOriginalFiles': is in file name(s) should be sequence of 'AttachmentBase64StringFiles', if AttachmentOriginalFiles are more than one then add file suffix with ','.
        ///     
        /// </summary>
        /// <param name="SendInternalMailCredential">SendInternalMail retrieval credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": 1,
        ///   "message": "Message has been sent successfully.",
        ///   "response": 
        ///      {  
        ///        "UserId": "",
        ///        "NonAuthorizedUserIds": "",
        ///      }
        ///   }
        ///Error:
        ///{
        ///"status": 0,
        ///"message": "User doenot exit/Error while retrieving Send Internal Mail information."
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("SendInternalMail")]
        public HttpResponseMessage SendInternalMail([ModelBinder] SendInternalMailCredential SendInternalMailCredential)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(SendInternalMailCredential.UserId) && !string.IsNullOrEmpty(SendInternalMailCredential.Body) && !string.IsNullOrEmpty(SendInternalMailCredential.Subject) &&
                    !string.IsNullOrEmpty(SendInternalMailCredential.MsgSendToUsersIds) && !string.IsNullOrEmpty(SendInternalMailCredential.SectionId) ||
                 (!string.IsNullOrEmpty(SendInternalMailCredential.AttachmentBase64StringFiles) && !string.IsNullOrEmpty(SendInternalMailCredential.AttachmentOriginalFiles)))
                {
                    try
                    {
                        Api_InternalMail objInterMail = new Api_InternalMail();
                        char colSeperator = (char)195;
                        DataSet dst, dstFileInfo;
                        StringBuilder spParam = new StringBuilder();
                        string IPAddress = "101";
                        string SectiondId = "", UserId = "", Body = "", Subject = "", SectionId = "";
                        string IsAttachment = "0";
                        string MsgSendToUsersIds = "", MsgSendBccUsersIds = "", MsgSendUsersIds = "", NonAuthorizedUserIds = "", NonAutorizedUserMsg = "";
                        int folderId = 1; // Folder Id Default '1' Inbox    
                        const char columnSeparator = (char)197;
                        const char rowSeparator = (char)198;
                        string InstutionId = "0";
                        string msg_ImageSaveStatus = "";
                        string[] ToIds = null;
                        string[] BcIds = null;
                        string[] arrAttachmentBaseFiles = null, arrAttachmentOrigFiles = null;
                        string msgFileAttachments = "", AttachmentBaseFiles = "", AttachOrginalFiles = "";

                        if (!string.IsNullOrEmpty(SendInternalMailCredential.MsgSendToUsersIds))
                        {
                            MsgSendToUsersIds = SendInternalMailCredential.MsgSendToUsersIds.Trim();
                            ToIds = MsgSendToUsersIds.Split(',');
                        }
                        if (!string.IsNullOrEmpty(SendInternalMailCredential.MsgSendBccUsersIds))
                        {
                            MsgSendBccUsersIds = SendInternalMailCredential.MsgSendBccUsersIds.Trim();
                            BcIds = MsgSendBccUsersIds.Split(',');
                        }

                        UserId = SendInternalMailCredential.UserId.Trim();
                        Body = SendInternalMailCredential.Body.Trim();
                        Subject = SendInternalMailCredential.Subject.Trim();
                        SectionId = SendInternalMailCredential.SectionId.Trim();

                        User objUser = new User();
                        dst = objUser.GetUserInfo(UserId);
                        if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                        {
                            InstutionId = dst.Tables[0].Rows[0][28].ToString();
                            string UserType = "/zQKgIk22cdCpmOGSPveVw==";

                            if (ToIds != null)
                            {
                                foreach (var item in ToIds)
                                {
                                    spParam.Length = 0;
                                    spParam.Append("1").Append(colSeperator).Append(UserId).Append(colSeperator);
                                    spParam.Append("2").Append(colSeperator).Append(item).Append(colSeperator);
                                    spParam.Append("3").Append(colSeperator).Append(UserType).Append(colSeperator);
                                    spParam.Append("4").Append(colSeperator).Append(InstutionId).Append(colSeperator);
                                    spParam.Append("5").Append(colSeperator).Append(SectionId);
                                    dst = null;
                                    dst = objInterMail.AuthoriseMessageFlow(spParam.ToString());
                                    if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                                    {
                                        if (MsgSendUsersIds != "")
                                            MsgSendUsersIds += rowSeparator + "T" + columnSeparator + item + columnSeparator + folderId;
                                        else
                                            MsgSendUsersIds = "T" + columnSeparator + item + columnSeparator + folderId;
                                    }
                                    else if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "-2")
                                    {
                                        if (NonAuthorizedUserIds != "")
                                            NonAuthorizedUserIds += "," + NonAuthorizedUserIds + "," + item;
                                        else
                                            NonAuthorizedUserIds = item;

                                    }

                                }
                            }

                            if (BcIds != null)
                            {
                                foreach (var item in BcIds)
                                {

                                    spParam.Length = 0;
                                    spParam.Append("1").Append(colSeperator).Append(UserId).Append(colSeperator);
                                    spParam.Append("2").Append(colSeperator).Append(item).Append(colSeperator);
                                    spParam.Append("3").Append(colSeperator).Append(UserType).Append(colSeperator);
                                    spParam.Append("4").Append(colSeperator).Append(InstutionId).Append(colSeperator);
                                    spParam.Append("5").Append(colSeperator).Append(SectionId);
                                    dst = null;
                                    dst = objInterMail.AuthoriseMessageFlow(spParam.ToString());
                                    if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                                    {
                                        if (MsgSendUsersIds != "")
                                            MsgSendUsersIds += rowSeparator + "B" + columnSeparator + item + columnSeparator + folderId;
                                        else
                                            MsgSendUsersIds = "B" + columnSeparator + item + columnSeparator + folderId;
                                    }
                                    else if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "-2")
                                    {
                                        if (NonAuthorizedUserIds != "")
                                            NonAuthorizedUserIds += "," + NonAuthorizedUserIds + "," + item;
                                        else
                                            NonAuthorizedUserIds = item;
                                    }

                                }
                            }
                            if (NonAuthorizedUserIds != "")
                            {
                                NonAutorizedUserMsg = "You are not authorized to send mail to this user(s)";
                            }
                            if (MsgSendUsersIds != "")
                                MsgSendUsersIds += rowSeparator + "T" + columnSeparator + UserId + columnSeparator + "2";


                            spParam.Length = 0;
                            spParam.Append("1").Append(colSeperator).Append(UserId).Append(colSeperator);

                            spParam.Append("3").Append(colSeperator).Append("1").Append(colSeperator);
                            spParam.Append("4").Append(colSeperator).Append(Body).Append(colSeperator);
                            spParam.Append("5").Append(colSeperator).Append(Subject).Append(colSeperator);
                            spParam.Append("6").Append(colSeperator).Append(MsgSendUsersIds).Append(colSeperator);

                            if (!string.IsNullOrEmpty(SendInternalMailCredential.AttachmentBase64StringFiles))
                            {
                                AttachmentBaseFiles = SendInternalMailCredential.AttachmentBase64StringFiles.Trim();
                                arrAttachmentBaseFiles = AttachmentBaseFiles.Split(',');
                            }
                            if (!string.IsNullOrEmpty(SendInternalMailCredential.AttachmentOriginalFiles))
                            {
                                AttachOrginalFiles = SendInternalMailCredential.AttachmentOriginalFiles.Trim();
                                arrAttachmentOrigFiles = AttachOrginalFiles.Split(',');
                            }

                            if (AttachmentBaseFiles != "" && AttachOrginalFiles != "")
                            {
                                try
                                {
                                    string guid;
                                    string filePath = "";
                                    FileInfo fileInfo;
                                    string guidFileName = "";
                                    string fileExtension = "";
                                    DataSet dstDestnPath;
                                    string OrinalFile = "";
                                    Educosoft.Api.Utilities.FileUpload fileObj = new Educosoft.Api.Utilities.FileUpload();
                                    dstFileInfo = fileObj.GetVirtualPath("1" + colSeperator + "FP7");
                                    if (dstFileInfo.Tables[0].Rows[0][0].ToString() == "0")
                                    {
                                        int i = 0;

                                        foreach (var Bfile in arrAttachmentBaseFiles)
                                        {
                                            i++;
                                            filePath = dstFileInfo.Tables[0].Rows[0][5].ToString();
                                            
                                            int j = 0;
                                            guid = System.Guid.NewGuid().ToString().ToUpper();
                                            foreach (var Ofile in arrAttachmentOrigFiles)
                                            {
                                                fileExtension = "";
                                                OrinalFile = "";
                                                j++;
                                                if (i == j)
                                                {
                                                    fileExtension = Ofile.Substring(Ofile.IndexOf('.')).ToLower();
                                                    OrinalFile = Ofile.ToLower();
                                                    break;
                                                }
                                                
                                            }
                                            guidFileName = guid + fileExtension;
                                            if (Bfile.Length > 100)
                                            {
                                                if (!Directory.Exists(filePath))
                                                    Directory.CreateDirectory(filePath);

                                                File.WriteAllBytes(filePath + guidFileName, Convert.FromBase64String(Bfile));

                                                if (msgFileAttachments == "")
                                                {
                                                    msgFileAttachments = guidFileName + columnSeparator + OrinalFile;
                                                }
                                                else
                                                {
                                                    msgFileAttachments += rowSeparator + guidFileName + columnSeparator + OrinalFile;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    msg_ImageSaveStatus = " file save status: " + ex.Message;
                                }

                                IsAttachment = "1";
                                spParam.Append("2").Append(colSeperator).Append(IsAttachment).Append(colSeperator);
                                spParam.Append("7").Append(colSeperator).Append(msgFileAttachments).Append(colSeperator);
                                spParam.Append("8").Append(colSeperator).Append(IPAddress).Append(colSeperator);


                            }
                            else
                            {
                                spParam.Append("2").Append(colSeperator).Append(IsAttachment).Append(colSeperator);
                                spParam.Append("8").Append(colSeperator).Append(IPAddress).Append(colSeperator);

                            }

                            spParam.Append("23").Append(colSeperator).Append(SectionId);

                            dst = null;
                            dst = objInterMail.InsertMessage(spParam.ToString());
                        }
                        if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                        {
                            if (NonAuthorizedUserIds != "")
                            {
                                var varNonAuthorizedResult = new
                                {
                                    NonAuthorizedUserIds = NonAuthorizedUserIds,
                                    NonAutorizedUserMsg = NonAutorizedUserMsg,
                                };

                                var result = new
                                {
                                    UserId = UserId,
                                    NonAuthorizedUserIds = varNonAuthorizedResult,
                                };

                                var resMessage = new
                                   {
                                       status = "1",
                                       message = "Message has been sent successfully.",
                                       response = result,
                                   };
                                return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200            
                            }
                            else
                            {
                                var resMessage = new
                                {
                                    status = "1",
                                    message = "Message has been sent successfully.",
                                    response = UserId,
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
                            message = "Exception while sending internal mail information"
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                    }
                }
                else
                {
                    var resMessage = new
                    {
                        status = "0",
                        message = "Send internal mail info retrieval credentials are required"
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                }

            }
            else
            {
                var resMessage = new
                {
                    status = "0",
                    message = "Invalid/malformed input(Send internal mail credential)"
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400                 
            }

            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while retrieving Send internal mail information."
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201
        }

    }
}
