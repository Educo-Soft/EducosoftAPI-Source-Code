using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EducosoftAPI.Models;
using System.Web.Http.ModelBinding;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using EducosoftAPI.Models.Course;
using EducosoftAPI.Models.User;
using System.Web;
using System.Text;
using Educosoft.Api.Course;
using Educosoft.Api.Utilities;

namespace EducosoftAPI.Controllers
{
    public class CourseController : ApiController
    {
        /// <summary>
        ///This api call takes CourseCredential i.e.,  'userId' and 'userType'  and 
        ///return course list details like 'Course_Name', 'Course_ISDev', 'CourseId', 'TermId', 'Term_Name', 'SectionId', 'Section_Name' with 'status' and 'message' upon successful retrieval of user course information.
        ///Incase of failure, it returns only 'status' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in retrieving course information.
        /// </summary>
        /// <param name="CourseCredential">Course(s) retrieval credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": 1,
        ///   "message": "Successfully retrieved user course information.",
        ///   "response": 
        ///      {  
        ///        "Course_Name": "",
        ///        "Course_ISDev":"",
        ///        "CourseId":"",
        ///        "TermId": "",
        ///        "Term_Name": "",
        ///        "SectionId": "",
        ///        "Section_Name":""
        ///   }
        ///Error:
        ///{
        ///"status": 0,
        ///"message": "User does not have access to section/Error while retrieving course information."
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("GetCourseList")]
        public HttpResponseMessage GetCourseList([ModelBinder] CourseCredential CourseCredential)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (CourseCredential.UserId != "" && CourseCredential.UserType != "")
                    {
                        UserCourse objUserCourse = new UserCourse();
                        DataSet dst = objUserCourse.GetUserCourseInfo(CourseCredential.UserId.Trim(), CourseCredential.UserType.Trim());

                        if (dst != null && dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0)
                        {

                            var userCourseList = dst.Tables[0].AsEnumerable().ToList();

                            var result = (from dr in userCourseList
                                          select new
                                          {
                                              Course_Name = dr["Course_Name"],
                                              CourseId = Convert.ToString(dr[4]).Split('$')[0],
                                              TermId = dr["TermId"],
                                              Term_Name = dr["TermId"],
                                              SectionId = dr["SectionId"],
                                              Section_Name = dr["Section_Name"],
                                              Course_ISDev = GetCourseType(Convert.ToString(dr[4]).Split('$')[0])

                                          }).ToList();
                            var resMessage = new
                           {
                               status = "1",
                               message = "successfully retrieved user course information",
                               response = result
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
                            message = "Course info retrieval credentials are required"
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                    }

                }
                catch (Exception ex)
                {
                    var resMessage = new
                    {
                        status = "0",
                        message = "Exception while retrieving course information"
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

            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while retrieving course information."
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201
        }

        /// <summary>
        ///This api call takes CourseLevelCredential i.e., 'UserId','SectionId','CourseId','CRLevelId'  and 
        ///return course level details like 'BlockedLevel', 'CR_LSchedule_FromDate', 'CR_LSchedule_ToDate', 'CRLevel_Name', 'CRLevel_ParentId', 'CRLevelId', 'Depth', 'HASNOLEVEL', 'HASNOLO', 'ISLO', 'LOID', 'LOType', 'SRLNo', 'TimeSpend'  with 'status' 
        ///and 'message' upon successful retrieval of course level information.
        ///Incase of failure, it returns only 'status' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in retrieving course level information.
        ///Note:'CRLevelId': default value is '-1'
        /// </summary>
        /// <param name="CourseLevelCredential">Course level Details(s) retrieval credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": 1,
        ///   "message": "Successfully retrieved course level information.",
        ///   "response": 
        ///      {  
        ///         "BlockedLevel":"",
        ///         "CR_LSchedule_FromDate":"",
        ///         "CR_LSchedule_ToDate":"",
        ///         "CRLevel_Name":"",
        ///         "CRLevel_ParentId":"",
        ///         "CRLevelId":"",
        ///         "Depth":"",
        ///         "HASNOLEVEL":"",
        ///         "HASNOLO":"",
        ///         "ISLO":"",
        ///         "LOID":"",
        ///         "LOType":"",
        ///         "SRLNo":"",
        ///         "TimeSpend":""
        ///   }
        ///Error:
        ///{
        ///"status": 0,
        ///"message": "Error while retrieving course level information."
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("GetCourseLevelInfo")]
        public HttpResponseMessage GetCourseLevelInfo([ModelBinder] CourseLevelCredential CourseLevelCredential)
        {
            if (ModelState.IsValid)
            {
                DataSet dst, dsXml;
                DataRow[] dtr = null;
                UserCourse objUserCourse = new UserCourse();
                string xmlFile = "", filename = "";
                //string strflotypcrst = " LOType NOT IN (108,111,112,113,114,115)";
                string strflotypcrst = " LOType NOT IN (108,114,115)";
                bool flagForNoteTimeSpend = false;

                try
                {
                    if (!string.IsNullOrEmpty(CourseLevelCredential.UserId) && !string.IsNullOrEmpty(CourseLevelCredential.SectionId)
                            && !string.IsNullOrEmpty(CourseLevelCredential.CourseId))
                    {
                        string UserId = CourseLevelCredential.UserId.Trim();
                        string CourseId = CourseLevelCredential.CourseId.Trim();
                        string SectionId = CourseLevelCredential.SectionId.Trim();
                        string CRLevelId;
                        if (!string.IsNullOrEmpty(CourseLevelCredential.CRLevelId))
                            CRLevelId = CourseLevelCredential.CRLevelId.Trim();
                        else
                            CRLevelId = "-1";

                        User objUser = new User();
                        dst = objUser.GetUserInfo(UserId);
                        if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                        {
                            dst = null;
                            dst = objUserCourse.GetCourseLevelInfo(CourseId);
                            if (dst != null && dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0)
                            {
                                if (dst.Tables[0].Rows[0][0].ToString() == "0")
                                {
                                    filename = dst.Tables[0].Rows[0][7].ToString();
                                    xmlFile = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["CRXMLFLPATH"] + filename);
                                    //xmlFile = ConfigurationManager.AppSettings["CRXMLFLPATH"] + filename;
                                    dsXml = new DataSet();
                                    dsXml.ReadXml(xmlFile);

                                    string strlotypefilter = "ParentIDN =" + "'" + CRLevelId + "'" + " AND " + strflotypcrst;
                                    string strSort = "SRLNo";
                                    dtr = dsXml.Tables[1].Select(strlotypefilter, strSort);
                                    dsXml.Dispose();
                                    dst = null;
                                    List<string> arrCCBlockedLevels = null;
                                    List<string> arrInsBlockedLevels = null;
                                    List<string> listBlockCCIns = null;
                                    if (dtr.Length > 0)
                                    {
                                        //Added for getting Blocked Levels
                                        dst = objUserCourse.GetBlockedLevels(CourseId);

                                        if (dst != null && dst.Tables[0].Rows[0][0].ToString() == "0" && dst.Tables[0].Rows.Count > 0)
                                        {
                                            if (dst.Tables[0].Rows[0][3].ToString() != "")
                                            {
                                                string BlockedLevels = dst.Tables[0].Rows[0][3].ToString().Substring(1);

                                                arrCCBlockedLevels = BlockedLevels.Split('#').ToList();
                                            }

                                        }
                                        dst = null;
                                        //Added for getting Instruction of Sheduled Blocked Levels
                                        List<DataRow> listRowsOfBlockedScheduleLevels = null;
                                        List<DataRow> listRowsOfScheduleLevels = null;
                                        List<CourseLevelSheduledDetails> listCourseLevelSheduledDetails = new List<CourseLevelSheduledDetails>();
                                        dst = objUserCourse.GetScheduledTopics(SectionId);
                                        if (dst != null && dst.Tables.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0" && dst.Tables[0].Rows.Count > 0)
                                        {
                                            listRowsOfScheduleLevels = dst.Tables[0].AsEnumerable().ToList();
                                            //listCourseLevelSheduledDetails = new List<CourseLevelSheduledDetails>();
                                            foreach (var item in listRowsOfScheduleLevels)
                                            {
                                                if (item["CR_LSchedule_LevelId"] != null && Convert.ToString(item["CR_LSchedule_FromDate"]) != "" && Convert.ToString(item["CR_LSchedule_ToDate"]) != "")
                                                {
                                                    listCourseLevelSheduledDetails.Add(new CourseLevelSheduledDetails
                                                    {
                                                        ScheduledLevelId = Convert.ToString(item["CR_LSchedule_LevelId"]),
                                                        ScheduledFromdate = Convert.ToString(item["CR_LSchedule_FromDate"]).Split(' ')[0],
                                                        ScheduledTodate = Convert.ToString(item["CR_LSchedule_ToDate"]).Split(' ')[0],
                                                    });
                                                }
                                            }

                                            if (dst != null && dst.Tables[1].Rows.Count > 0)
                                            {
                                                if (dst.Tables[1].Rows[0][0].ToString() == "0")
                                                {
                                                    listRowsOfBlockedScheduleLevels = dst.Tables[1].AsEnumerable().ToList();
                                                    arrInsBlockedLevels = new List<string>();
                                                    foreach (var item in listRowsOfBlockedScheduleLevels)
                                                    {
                                                        arrInsBlockedLevels.Add(Convert.ToString(item["CR_LSchedule_LevelId"]));
                                                    }
                                                }
                                            }
                                        }
                                        listBlockCCIns = new List<string>();
                                        if (arrCCBlockedLevels != null && arrInsBlockedLevels != null)
                                        {
                                            var listBlockedCCINsLevels = arrCCBlockedLevels.Union(arrInsBlockedLevels);
                                            var listDistictBlockedCCINsLevels = listBlockedCCINsLevels.Distinct();
                                            listBlockCCIns = listDistictBlockedCCINsLevels.ToList();

                                        }
                                        else if (arrInsBlockedLevels != null)
                                        {
                                            listBlockCCIns = arrInsBlockedLevels;

                                        }
                                        else if (arrCCBlockedLevels != null)
                                        {
                                            listBlockCCIns = arrCCBlockedLevels;
                                        }

                                        List<DataRow> listCRLevelDetails = dtr.AsEnumerable().ToList();
                                        var CRLevelsInfo = (from dr in listCRLevelDetails
                                                            join CRLevelBlock in listBlockCCIns on dr["CRLevelID"].ToString() equals Convert.ToString(CRLevelBlock)
                                                            into CRlevelBlockGroup
                                                            from CRLevelB in CRlevelBlockGroup.DefaultIfEmpty()
                                                            //from CRLevelBB in listCRLevelBlockCCAndInsSheduleDetails.Where(var_TimeSpend => CRLevelB == null ? false : var_TimeSpend.CRLevelBlockCCInsId == CRLevelB.CRLevelBlockCCInsId).DefaultIfEmpty()
                                                            select new
                                                            {
                                                                CRLevelId = dr["CRLevelID"],
                                                                CRLevel_ParentId = dr["ParentIDN"],
                                                                CRLevel_Name = dr["Text"],
                                                                Depth = dr["Depth"],
                                                                HASNOLEVEL = dr["HASNOLEVEL"],
                                                                HASNOLO = dr["HASNOLO"],
                                                                ISLO = dr["ISLO"],
                                                                LOID = dr["LOID"],
                                                                LOType = dr["LOType"],
                                                                SRLNo = dr["SRLNo"],
                                                                BlockedLevel = CRLevelB != null ? 1 : 0,

                                                            }).ToList();

                                        List<DataRow> listUserTimeSpendDetails = null;
                                        dst = null;

                                        if (CRLevelsInfo.Count() > 0)
                                        {

                                            dst = objUserCourse.GetUserTimeSpentAllLevels(UserId, CourseId, SectionId);

                                            if (dst != null && dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0)
                                            {
                                                if (dst.Tables[0].Rows[0][0].ToString() == "0")
                                                {
                                                    flagForNoteTimeSpend = true;

                                                    listUserTimeSpendDetails = dst.Tables[0].AsEnumerable().ToList();
                                                }
                                            }
                                        }
                                        var listUserCRLevelIdTimeSpend = new List<string>();
                                        if (flagForNoteTimeSpend)
                                        {
                                            var userTimeSpendDetails = (from datarow in listUserTimeSpendDetails
                                                                        select new
                                                                        {
                                                                            LevleTimeSpent = Convert.ToString(datarow["LevleTimeSpent"]),
                                                                        }).ToList();
                                            foreach (var data in userTimeSpendDetails)
                                            {
                                                var CRLevelIdTimeSpend = data.LevleTimeSpent.Split('#');
                                                listUserCRLevelIdTimeSpend = CRLevelIdTimeSpend.ToList();

                                            }
                                        }

                                        var Result = (from CRLevel in CRLevelsInfo
                                                      join UserCRLevelIdTimeSpend in listUserCRLevelIdTimeSpend on CRLevel.CRLevelId equals UserCRLevelIdTimeSpend.Split('$')[0]
                                                      into CRlevelCourseInfo
                                                      from CRlevelCourse in CRlevelCourseInfo.DefaultIfEmpty()
                                                      join CRLevelS in listCourseLevelSheduledDetails on CRLevel.CRLevelId equals CRLevelS.ScheduledLevelId
                                                      into CourseLevelSheduledDetailsGroup
                                                      from CourseLevelSheduled in CourseLevelSheduledDetailsGroup.DefaultIfEmpty()
                                                      from CRSL in listCourseLevelSheduledDetails.Where(var_TimeSpend => CourseLevelSheduled == null ? false : var_TimeSpend.ScheduledLevelId == CourseLevelSheduled.ScheduledLevelId).DefaultIfEmpty()
                                                      orderby CRLevel.SRLNo
                                                      select new
                                                      {
                                                          BlockedLevel = CRLevel.BlockedLevel,
                                                          CRLevelId = CRLevel.CRLevelId,
                                                          CRLevel_ParentId = CRLevel.CRLevel_ParentId,
                                                          CRLevel_Name = CRLevel.CRLevel_Name,
                                                          Depth = CRLevel.Depth,
                                                          HASNOLEVEL = CRLevel.HASNOLEVEL,
                                                          HASNOLO = CRLevel.HASNOLO,
                                                          ISLO = CRLevel.ISLO,
                                                          LOID = CRLevel.LOID,
                                                          LOType = CRLevel.LOType,
                                                          SRLNo = CRLevel.SRLNo,
                                                          TimeSpend = CRlevelCourse == null ? String.Empty : CRlevelCourse.Split('$')[1],
                                                          CR_LSchedule_FromDate = CourseLevelSheduled == null || CourseLevelSheduled.ScheduledFromdate == null ? string.Empty : CourseLevelSheduled.ScheduledFromdate,
                                                          CR_LSchedule_ToDate = CourseLevelSheduled == null || CourseLevelSheduled.ScheduledTodate == null ? string.Empty : CourseLevelSheduled.ScheduledTodate

                                                      }).ToList();

                                        var resMessage = new
                                        {
                                            status = "1",
                                            message = "successfully retrieved course level information",
                                            response = Result
                                        };
                                        return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200    
                                    }
                                    else
                                    {
                                        var resMessage = new
                                        {
                                            status = "0",
                                            message = "Course level id not exit"
                                        };
                                        return Request.CreateResponse(HttpStatusCode.NotFound, resMessage); //response code = 404 
                                    }
                                }
                                else
                                {
                                    var resMessage = new
                                    {
                                        status = "0",
                                        message = "Course level id not exit"
                                    };
                                    return Request.CreateResponse(HttpStatusCode.NotFound, resMessage); //response code = 404 
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
                            message = "Course level info retrieval credentials are required"
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201

                    }
                }
                catch (Exception ex)
                {
                    var resMessage = new
                    {
                        status = "0",
                        //message = "Exception while retrieving course level information"
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
                    message = "Invalid/malformed input(course level credential)"
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400       
            }

            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while retrieving course information."
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201
        }

        #region
        /// <summary>
        ///This api call takes GetCourseSelectedLevelInfo i.e., 'UserId', 'SectionId', 'CourseId', 'CRLevelId'  and 
        ///return course level details like 'CRLevel_Name', 'CRLevel_ParentId', 'CRLevelId', 'Depth', 'HASNOLEVEL', 'HASNOLO', 'ISLO', 'LevelHierarchy', 'LOID', 'LOType', 'SRLNo', 'TimeSpend', 'FilePath' with 'status 
        ///and 'message' upon successful retrieval of course level information.
        ///Incase of failure, it returns only 'status' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in retrieving course level information.
        /// </summary>
        /// <param name="CourseLevelCredential">Course level Details(s) retrieval credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": 1,
        ///   "levelHierarchy":"",
        ///   "message": "Successfully retrieved course level information.",
        ///   "response": 
        ///      {  
        ///         "CRLevel_Name":"",
        ///         "CRLevel_ParentId":"",
        ///         "CRLevelId":"",
        ///         "Depth":"",
        ///         "HASNOLEVEL":"",
        ///         "HASNOLO":"",
        ///         "ISLO":"",
        ///         "LevelHierarchy":"",
        ///         "LOID":"",
        ///         "LOType":"",
        ///         "SRLNo":"",
        ///         "TimeSpend":"",
        ///         "FilePath":""
        ///   }
        ///Error:
        ///{
        ///"status": 0,
        ///"message": "Error while retrieving course level information."
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("GetCourseSelectedLevelInfo")]
        public HttpResponseMessage GetCourseSelectedLevelInfo([ModelBinder] CourseLevelCredential CourseLevelCredential)
        {
            if (ModelState.IsValid)
            {
                DataSet dst, dsXml, dsCourse;
                DataRow[] dtr = null;
                UserCourse objUserCourse = new UserCourse();
                string xmlFile = "", filename = "";
                //string strflotypcrst = "LOType <> 114 AND LOType <> 115 AND LOType <> 108";
                string strflotypcrst = "LOType NOT IN (108,114,115)";
                StringBuilder spParam;
                char colSeperator = (char)195;
                string LangId = "Strings-En";
                string levelHierarchy = "";
                Api_Course objCourse = new Api_Course();
                string strlotypefilter = "", filePath = "";
                bool flagForNoteTimeSpend = false;

                try
                {
                    if (!string.IsNullOrEmpty(CourseLevelCredential.UserId) && !string.IsNullOrEmpty(CourseLevelCredential.SectionId)
                            && !string.IsNullOrEmpty(CourseLevelCredential.CourseId) && !string.IsNullOrEmpty(CourseLevelCredential.CRLevelId))
                    {
                        string UserId = CourseLevelCredential.UserId.Trim();
                        string CourseId = CourseLevelCredential.CourseId.Trim();
                        string SectionId = CourseLevelCredential.SectionId.Trim();
                        string CRLevelId = CourseLevelCredential.CRLevelId.Trim();

                        User objUser = new User();
                        UserCourse userCourse = new UserCourse();
                        dst = objUser.GetUserInfo(UserId);
                        if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                        {
                            dst = null;
                            dst = objUserCourse.GetCourseLevelInfo(CourseId);
                            dsCourse = dst;
                            if (dst != null && dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0)
                            {
                                if (dst.Tables[0].Rows[0][0].ToString() == "0")
                                {
                                    filename = dst.Tables[0].Rows[0][7].ToString();
                                    xmlFile = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["CRXMLFLPATH"] + filename);
                                    //xmlFile = ConfigurationManager.AppSettings["CRXMLFLPATH"] + filename;
                                    dsXml = new DataSet();
                                    dsXml.ReadXml(xmlFile);

                                    strlotypefilter = "CRLevelId=" + "'" + CRLevelId + "'" + "  AND " + strflotypcrst;

                                    string strSort = "SRLNo";
                                    dtr = dsXml.Tables[1].Select(strlotypefilter, strSort);
                                    dsXml.Dispose();
                                    dsXml = null;
                                    if (dtr.Length > 0)
                                    {
                                        List<DataRow> drList = dtr.AsEnumerable().ToList();

                                        var ListLevelInfo = (from dr in drList
                                                             select new
                                                             {
                                                                 CRLevelId = dr["CRLevelID"],
                                                                 CRLevel_ParentId = dr["ParentIDN"],
                                                                 CRLevel_Name = dr["Text"],
                                                                 Depth = dr["Depth"],
                                                                 HASNOLEVEL = dr["HASNOLEVEL"],
                                                                 HASNOLO = dr["HASNOLO"],
                                                                 ISLO = dr["ISLO"],
                                                                 LOID = dr["LOID"],
                                                                 LOType = dr["LOType"],
                                                                 SRLNo = dr["SRLNo"],
                                                             }).ToList();

                                        var ListLos = (from levelInfo in ListLevelInfo
                                                       where levelInfo.LOID.ToString() != "0" && levelInfo.ISLO.ToString() == "1"
                                                       select new
                                                       {
                                                           CRLevelId = levelInfo.CRLevelId,
                                                           CRLevel_ParentId = levelInfo.CRLevel_ParentId,
                                                           CRLevel_Name = levelInfo.CRLevel_Name,
                                                           Depth = levelInfo.Depth,
                                                           HASNOLEVEL = levelInfo.HASNOLEVEL,
                                                           HASNOLO = levelInfo.HASNOLO,
                                                           ISLO = levelInfo.ISLO,
                                                           LOID = levelInfo.LOID,
                                                           LOType = levelInfo.LOType,
                                                           SRLNo = levelInfo.SRLNo
                                                       }).ToList();

                                        string strCourseLevelId = "";
                                        string strDummyCourseLevelId = "";

                                        Educosoft.Api.Utilities.Utility objMappedId;
                                        objMappedId = new Educosoft.Api.Utilities.Utility();

                                        strCourseLevelId = objMappedId.GetMappingId(CRLevelId, CourseId, LangId);
                                        strDummyCourseLevelId = objMappedId.GetMappingId(strCourseLevelId, LangId);

                                        levelHierarchy = userCourse.GetLevelHierarchy(CourseId, strDummyCourseLevelId);

                                        dst = null;
                                        var listUserCRLevelIdTimeSpend = new List<string>();
                                        List<DataRow> listUserTimeSpendDetails = null;

                                        dst = objUserCourse.GetUserTimeSpentAllLevels(UserId, CourseId, SectionId);

                                        if (dst != null && dst.Tables.Count > 0 && dst.Tables[0].Rows.Count > 0)
                                        {
                                            if (dst.Tables[0].Rows[0][0].ToString() == "0")
                                            {
                                                flagForNoteTimeSpend = true;

                                                listUserTimeSpendDetails = dst.Tables[0].AsEnumerable().ToList();
                                            }
                                        }

                                        if (flagForNoteTimeSpend)
                                        {
                                            var userTimeSpendDetails = (from datarow in listUserTimeSpendDetails
                                                                        select new
                                                                        {
                                                                            LevleTimeSpent = Convert.ToString(datarow["LevleTimeSpent"]),
                                                                        }).ToList();
                                            foreach (var data in userTimeSpendDetails)
                                            {
                                                var CRLevelIdTimeSpend = data.LevleTimeSpent.Split('#');
                                                listUserCRLevelIdTimeSpend = CRLevelIdTimeSpend.ToList();
                                            }
                                        }
                                        dst = null;
                                        if (ListLevelInfo.Count > 0 || ListLos.Count > 0)
                                        {
                                            List<string> strLoIds = new List<string>();
                                            string fileNamePath = "";
                                            if (ListLos.Count > 0)
                                            {
                                                Educosoft.Api.Utilities.FileUpload uploadObj;
                                                uploadObj = new Educosoft.Api.Utilities.FileUpload();
                                                spParam = new StringBuilder();
                                                spParam.Length = 0;
                                                spParam.Append("1").Append(colSeperator).Append("FP5");
                                                dst = uploadObj.GetVirtualPath(spParam.ToString());
                                                if (dst.Tables[0].Rows[0].ItemArray[0].ToString() == "0")
                                                {
                                                    filePath = dst.Tables[0].Rows[0].ItemArray[4].ToString();
                                                }
                                                foreach (var LoId in ListLos)
                                                {
                                                    fileNamePath = userCourse.GetFilePath(LoId.LOID.ToString());
                                                    strLoIds.Add(LoId.LOID.ToString() + "$" + filePath + @"/" + fileNamePath);
                                                }

                                                var results = (from CRLevel in ListLos
                                                               join UserCRLevelIdTimeSpend in listUserCRLevelIdTimeSpend on CRLevel.CRLevelId equals UserCRLevelIdTimeSpend.Split('$')[0]
                                                               into CRlevelCourseInfo
                                                               from CRlevelCourse in CRlevelCourseInfo.DefaultIfEmpty()
                                                               join loId in strLoIds on CRLevel.LOID equals loId.Split('$')[0]
                                                               into loIdsFilePathInfo
                                                               from loIdsFilePath in loIdsFilePathInfo.DefaultIfEmpty()
                                                               orderby CRLevel.SRLNo
                                                               select new
                                                               {
                                                                   CRLevelId = CRLevel.CRLevelId,
                                                                   CRLevel_ParentId = CRLevel.CRLevel_ParentId,
                                                                   CRLevel_Name = CRLevel.CRLevel_Name,
                                                                   Depth = CRLevel.Depth,
                                                                   HASNOLEVEL = CRLevel.HASNOLEVEL,
                                                                   HASNOLO = CRLevel.HASNOLO,
                                                                   ISLO = CRLevel.ISLO,
                                                                   LOID = CRLevel.LOID,
                                                                   LOType = CRLevel.LOType,
                                                                   SRLNo = CRLevel.SRLNo,
                                                                   TimeSpend = CRlevelCourse == null ? String.Empty : CRlevelCourse.Split('$')[1],
                                                                   FilePath = loIdsFilePath == null ? string.Empty : loIdsFilePath.Split('$')[1],
                                                                   LevelHierarchy = levelHierarchy,
                                                               }).ToList();

                                                var resMessage = new
                                                {
                                                    status = "1",
                                                    message = "successfully retrieved course level information",
                                                    response = results
                                                };
                                                return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200   

                                            }
                                            else if (ListLevelInfo.Count > 0)
                                            {
                                                var results = (from CRLevel in ListLevelInfo
                                                               join UserCRLevelIdTimeSpend in listUserCRLevelIdTimeSpend on CRLevel.CRLevelId equals UserCRLevelIdTimeSpend.Split('$')[0]
                                                               into CRlevelCourseInfo
                                                               from CRlevelCourse in CRlevelCourseInfo.DefaultIfEmpty()
                                                               orderby CRLevel.SRLNo
                                                               select new
                                                               {
                                                                   CRLevelId = CRLevel.CRLevelId,
                                                                   CRLevel_ParentId = CRLevel.CRLevel_ParentId,
                                                                   CRLevel_Name = CRLevel.CRLevel_Name,
                                                                   Depth = CRLevel.Depth,
                                                                   HASNOLEVEL = CRLevel.HASNOLEVEL,
                                                                   HASNOLO = CRLevel.HASNOLO,
                                                                   ISLO = CRLevel.ISLO,
                                                                   LOID = CRLevel.LOID,
                                                                   LOType = CRLevel.LOType,
                                                                   SRLNo = CRLevel.SRLNo,
                                                                   TimeSpend = CRlevelCourse == null ? String.Empty : CRlevelCourse.Split('$')[1],
                                                                   FilePath = String.Empty,
                                                                   LevelHierarchy = levelHierarchy,
                                                               }).ToList();

                                                var resMessage = new
                                                {
                                                    status = "1",
                                                    message = "successfully retrieved course level information",
                                                    response = results
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
                                                return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 404 
                                            }
                                        }
                                        else
                                        {
                                            var resMessage = new
                                            {
                                                status = "0",
                                                message = "No data found."
                                            };
                                            return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 404 
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
                            message = "Course level info retrieval credentials are required"
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
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
                    message = "Invalid/malformed input(course level credential)"
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400       
            }

            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while retrieving course information."
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201
        }
        #endregion

        #region
        /// <summary>
        ///This api call takes CRLOTimeSpentCredential i.e., 'UserId','SectionId','CourseId','CRLOId','TimeSpent'  and 
        ///return with 'status' and 'message' upon Time spent has been successfully Updated.
        ///Incase of failure, it returns only 'status' and 'message'.
        ///'Status':'1'  means Success and  'Status':'0'  means failure/error in saving time spent.
        ///Note:'TimeSpent' value should be in seconds.
        /// </summary>
        /// <param name="CRLOTimeSpentCredential">Course level Details(s) retrieval credential to specify in request body in json format.</param>
        /// <returns>
        ///Success:
        ///{
        ///   "status": 1,
        ///   "message": "Time spent has been successfully Updated",
        ///Error:
        ///{
        ///"status": 0,
        ///"message": "Error occured while saving time spent."
        ///}
        /// </returns>
        [HttpPost]
        [ActionName("SaveCRLOTimeSpent")]
        public HttpResponseMessage SaveCRLOTimeSpent([ModelBinder] CRLOTimeSpentCredential CRLOTimeSpentCredential)
        {
            if (ModelState.IsValid)
            {
                Api_Course objCourse = new Api_Course();
                char colSeperator = (char)195;
                DataSet dst;
                StringBuilder spParam = null;
                try
                {
                    if (!string.IsNullOrEmpty(CRLOTimeSpentCredential.UserId) && !string.IsNullOrEmpty(CRLOTimeSpentCredential.SectionId)
                        && !string.IsNullOrEmpty(CRLOTimeSpentCredential.CourseId) && !string.IsNullOrEmpty(CRLOTimeSpentCredential.CRLOId)
                        && !string.IsNullOrEmpty(CRLOTimeSpentCredential.TimeSpent))
                    {
                        string UserId = CRLOTimeSpentCredential.UserId.Trim();
                        string CourseId = CRLOTimeSpentCredential.CourseId.Trim();
                        string SectionId = CRLOTimeSpentCredential.SectionId.Trim();
                        string CRLOId = CRLOTimeSpentCredential.CRLOId.Trim();
                        string TimeSpent = CRLOTimeSpentCredential.TimeSpent.Trim();
                        User objUser = new User();
                        dst = objUser.GetUserInfo(UserId);
                        if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                        {
                            dst = null;
                            UserCourse objUserCourse = new UserCourse();
                            dst = objUserCourse.GetCourseLevelInfo(CourseId);
                            if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                            {
                                dst = null;
                                spParam = new StringBuilder();
                                spParam.Length = 0;
                                spParam.Append("1").Append(colSeperator).Append(CRLOId);
                                dst = objCourse.GetLoIds(spParam.ToString());
                                if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                                {
                                    spParam.Length = 0;
                                    spParam.Append("1").Append(colSeperator).Append(CourseId).Append(colSeperator);
                                    spParam.Append("2").Append(colSeperator).Append(UserId).Append(colSeperator);
                                    spParam.Append("3").Append(colSeperator).Append(CRLOId).Append(colSeperator);
                                    spParam.Append("4").Append(colSeperator).Append(TimeSpent).Append(colSeperator);
                                    //spParam.Append("5").Append(colSeperator).Append(IPAddress).Append(colSeperator);
                                    spParam.Append("6").Append(colSeperator).Append(SectionId).Append(colSeperator);
                                    spParam.Append("7").Append(colSeperator).Append("ST");

                                    dst = objCourse.SaveTimeSpent(spParam.ToString());
                                    if (dst != null && dst.Tables[0].Rows.Count > 0 && dst.Tables[0].Rows[0][0].ToString() == "0")
                                    {

                                        var resMessage = new
                                        {
                                            status = "1",
                                            message = "Time spent has been successfully Updated.",
                                        };
                                        return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200   

                                    }
                                    else
                                    {
                                        var resMessage = new
                                        {
                                            status = "0",
                                            message = "Course level lo time spent retrieval credentials are required"
                                        };

                                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
                                    }
                                }//Loid verifivatin
                                else
                                {
                                    var resMessage = new
                                    {
                                        status = "0",
                                        message = "No data found."
                                    };
                                    return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                                }
                            }//course verificaion
                            else
                            {
                                var resMessage = new
                                {
                                    status = "0",
                                    message = "No data found."
                                };
                                return Request.CreateResponse(HttpStatusCode.OK, resMessage); //response code = 200 
                            }
                        }//user verification
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
                            message = "Course level time spent retrieval credentials are required"
                        };

                        return Request.CreateResponse(HttpStatusCode.Created, resMessage); //response code = 201
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
                    message = "Invalid/malformed input(course level credential)"
                };

                return Request.CreateResponse(HttpStatusCode.BadRequest, resMessage);  //response code = 400       
            }

            var resMessageUnKnown = new
            {
                status = "0",
                message = "Unknown error occured while saving times spent"
            };
            return Request.CreateResponse(HttpStatusCode.Created, resMessageUnKnown);  //response code = 201
        }
        #endregion

        private string GetCourseType(string CourseId)
        {
            UserCourse objCourse = new UserCourse();
            DataSet dtCourse;
            string IsCourseDev = "0";
            if (!string.IsNullOrEmpty(CourseId))
            {
                dtCourse = objCourse.GetCourseLevelInfo(CourseId);
                if (dtCourse != null && dtCourse.Tables[0].Rows.Count > 0 && dtCourse.Tables[0].Rows[0][0].ToString() == "0")
                {
                    IsCourseDev = dtCourse.Tables[0].Rows[0]["Course_ISDev"].ToString();
                }
            }
            return IsCourseDev;
        }
    }
}

