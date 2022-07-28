using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EducosoftAPI.Models;
using System.Web.Http.ModelBinding;
using System.Data;

namespace EducosoftAPI.Controllers
{
    public class UserCourseController : ApiController
    {
        /// <summary>
        ///This api call takes userCourseCredential i.e.,  'userId' and 'userType'  and 
        ///return user course list details like 'Course_Name','TermId', 'Term_Name','SectionId', 'Section_Name' with 'status' and 'message' upon successful retrival of course information.
        ///Incase of failure, it returns only 'status' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in retriving course information.
        /// </summary>
        /// <param name="userCourseCredential">Returns the specific user courser based on userid from Url</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": 1,
        ///   "message": "Succesfully retrived course information",
        ///   "response": 
        ///      {  
        ///        "Course_Name": "",
        ///        "TermId": "",
        ///        "Term_Name": "",
        ///        "SectionId": "",
        ///        "Section_Name":""
        ///   }
        ///Error:
        ///{
        ///"status": 0,
        ///"message": "User does not have access to section/Error while retriving course information"
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("GetUserCourseInfo")]
        public HttpResponseMessage GetUserCourseInfo([ModelBinder] UserCourseCredential userCourseCredential)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (userCourseCredential.UserId != "" && userCourseCredential.UserType !="")
                    {
                        UserCourse objUserCourse = new UserCourse();
                        DataSet dst = objUserCourse.GetUserCourseInfo(userCourseCredential.UserId.Trim(), userCourseCredential.UserType.Trim());

                        if (dst != null && dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0)
                        {

                            List<UserCourseDetails> userCourseList = new List<UserCourseDetails>();
                            foreach (DataRow dr in dst.Tables[0].Rows)
                            {
                                UserCourseDetails userCourseDetails = new UserCourseDetails();
                                userCourseDetails.Course_Name = Convert.ToString(dr["Course_Name"]);
                                userCourseDetails.TermId = Convert.ToString(dr["TermId"]);
                                userCourseDetails.Term_Name = Convert.ToString(dr["Term_Name"]);
                                userCourseDetails.SectionId = Convert.ToString(dr["SectionId"]);
                                userCourseDetails.Section_Name = Convert.ToString(dr["Section_Name"]);
                                userCourseList.Add(userCourseDetails);
                            }

                            var resMessage = new
                            {
                                status = "1",
                                message = "succesfully retrived course information",
                                response = userCourseList
                            };

                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200                             
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
                            message = "Course info retrival credentials are required"
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                    }

                }
                catch (Exception ex)
                {
                    var resMessage = new
                    {
                        status = "0",
                        message = "Exception while retriving course information"
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                }
            }
            else
            {
                var resMessage = new
                {
                    status = "0",
                    message = "Invalid/malformed input(user course credential)"
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400                 
            }
        }
    }
}
