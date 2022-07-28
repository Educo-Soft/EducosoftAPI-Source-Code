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
using System.Data;
using System.Text;

namespace EducosoftAPI.Controllers
{
    public class AnnouncementController : ApiController
    {
        /// <summary>
        ///This api call takes AnnouncementCredential i.e., 'UserId', 'SectionId', 'Announcement_Type', 'Announcement_OptSel' and 'UserCurrZoneDate' and 
        ///return Announcement list details like 'AttachmentFiles', 'Description', 'FilePath', 'PostedDate', with 'status' and 'message' upon successful retrieval of announcement information.
        ///Incase of failure, it returns only 'status' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in retrieving announcements information.
        ///Note:1) 'Announcement_Type': '0' for General_Announcements,'3' for Course_Announcements, 2) 'Announcement_OptSel':'0/1/2/3/' for 'View All/View Today/Last 7 Days/Last 30 Days'
        /// </summary>
        /// <param name="AnnouncementCredential">Announcements retrieval credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": 1,
        ///   "message": "Succesfully retrieved announcements information.",
        ///   "response": 
        ///      {  
        ///        "AttachmentFiles": "",
        ///        "Description": "",
        ///        "FilePath": "",
        ///        "PostedDate":""
        ///   }
        ///Error:
        ///{
        ///"status": 0,
        ///"message": "User does not have access to section/Error while retrieving announcements information."
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("GetAnnouncementList")]
        public HttpResponseMessage GetAnnouncementList([ModelBinder] AnnouncementCredential AnnouncementCredential)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(AnnouncementCredential.UserId) && !string.IsNullOrEmpty(AnnouncementCredential.SectionId) &&
                    !string.IsNullOrEmpty(AnnouncementCredential.Announcement_Type) && !string.IsNullOrEmpty(AnnouncementCredential.Announcement_OptSel) &&
                    !string.IsNullOrEmpty(AnnouncementCredential.UserCurrZoneDate))
                {
                    try
                    {
                        Api_Announcements objAnnoun = new Api_Announcements();
                        char colSeperator = (char)195;
                        DataSet dst, dstFileInfo;
                        StringBuilder spParam = new StringBuilder();
                        string UserId = "", SectionId = "", Announcement_Type = "", Announcement_OptSel = "", UserCurrZoneDate = "",
                            PortalID = "", InstututionId = "", FilePath = "";

                        UserId = AnnouncementCredential.UserId.Trim();
                        SectionId = AnnouncementCredential.SectionId.Trim();
                        Announcement_Type = AnnouncementCredential.Announcement_Type.Trim();
                        Announcement_OptSel = AnnouncementCredential.Announcement_OptSel.Trim();
                        UserCurrZoneDate = AnnouncementCredential.UserCurrZoneDate.Trim();

                        User objUser = new User();
                        dst = objUser.GetUserInfo(UserId);
                        if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                        {
                            InstututionId = dst.Tables[0].Rows[0][28].ToString();
                            //if (Announcement_Type=="0" && InstututionId != "")
                            //{
                            //    spParam.Length = 0;
                            //    spParam.Append("2").Append(colSeperator).Append("InstututionId");
                            //    //dst=objAnnoun.PortalGetInstitutionList(spParam.ToString());    //Institution_PortalId
                            //    //PortalID = dst.Tables[0].Rows[0]["Institution_PortalId"].ToString();
                            //}

                            spParam.Length = 0;
                            spParam.Append("1").Append(colSeperator).Append("3").Append(colSeperator);
                            //spParam.Append("2").Append(colSeperator).Append("PortalID").Append(colSeperator);

                            if (Announcement_Type == "0")
                                spParam.Append("3").Append(colSeperator).Append(InstututionId).Append(colSeperator);

                            spParam.Append("4").Append(colSeperator).Append(UserId).Append(colSeperator);
                            spParam.Append("5").Append(colSeperator).Append(Announcement_Type).Append(colSeperator);
                            spParam.Append("6").Append(colSeperator).Append(SectionId).Append(colSeperator);
                            spParam.Append("7").Append(colSeperator).Append(Announcement_OptSel).Append(colSeperator);
                            spParam.Append("8").Append(colSeperator).Append(UserCurrZoneDate);

                            dst = null;

                            dst = objAnnoun.GetAssignedAnnouncements(spParam.ToString());

                            if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                            {
                                Educosoft.Api.Utilities.FileUpload fileObj = new Educosoft.Api.Utilities.FileUpload();
                                dstFileInfo = fileObj.GetVirtualPath("1" + colSeperator + "FP7");
                                if (dstFileInfo.Tables[0].Rows[0][0].ToString() == "0")
                                    FilePath = dstFileInfo.Tables[0].Rows[0][4].ToString();

                                var userAnnounList = dst.Tables[0].AsEnumerable().ToList();

                                var result = (from dr in userAnnounList
                                              select new
                                              {
                                                  //PostedDate = dr["dtcreattiondate"],
                                                  Description = dr["nvarannondesc"],
                                                  PostedDate = dr["dtannouncement_startdate"],
                                                  //dtannouncement_enddate = dr["dtannouncement_enddate"],
                                                  FilePath = dr["nvarannattaedoriginalfile"].ToString() =="" ? string.Empty : FilePath + dr["nvarannattaedoriginalfile"].ToString().Split('/')[1],
                                                  AttachmentFiles = dr["nvarannattaedoriginalfile"].ToString() == "" ? string.Empty : dr["nvarannattaedoriginalfile"].ToString().Split('/')[0],
                                              }).ToList();

                                var resMessage = new
                                {
                                    status = "1",
                                    message = "Successfully retrieved announcements information",
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
                            message = "Exception while retrieving announcement information"
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                    }
                }
                else
                {
                    var resMessage = new
                    {
                        status = "0",
                        message = "Announcement info retrieval credentials are required"
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                }

            }
            else
            {
                var resMessage = new
                {
                    status = "0",
                    message = "Invalid/malformed input(Announcement credential)"
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400                 
            }

            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while retrieving announcement information."
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201
        }
    }
}
