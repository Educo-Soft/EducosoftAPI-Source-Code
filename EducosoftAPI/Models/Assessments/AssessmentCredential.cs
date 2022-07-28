using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace EducosoftAPI.Models.Assessments
{
    public class AssessmentCredential
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string CourseId { get; set; }
        [Required]
        public string SectionId { get; set; }
        [Required]
        public string UserCurrZoneDate { get; set; }
        [Required]
        public string AssessmentType { get; set; }
    }

    public class InitTestPaperCredential
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string CourseId { get; set; }
        [Required]
        public string SectionId { get; set; }
        [Required]
        public string TestId { get; set; }
    }

    public class AssessmentPreReqVideoInfo
    {
        [Required]
        public string TestId { get; set; }
        public string TestPrevideoID { get; set; }
        public string TestPreReqVideo_uploadID { get; set; }
        public string TestPreReqVideo_MinTime { get; set; }
        public string TestPreReqVideo_TimeSpent { get; set; }
        public string TestPreReqVideo_FileName { get; set; }
        public string TestPreReqVideo_VideoType { get; set; }
        public string IsAdminVideo { get; set; }
        public string FilePath { get; set; }

    }
    
    public class TestPaperAttachedAssInfo
    {
        [Required]
        public string TestId { get; set; }
        public string TestPreAssID { get; set; }
        public string TestPreAss_PreReqTestId { get; set; }
        public string TestPreAss_MaxScore { get; set; }
        public string TotalMarksObtained { get; set; }
        public string TotalScoreForPreReq { get; set; }
        public string PreRequistitTestName { get; set; }
        public string TotalQuestForMainAss { get; set; }
        public string PreReqTotQuesWithSolVarient { get; set; }
        public string preReqMaxAttempt { get; set; }
        public string preReqAttemptCount { get; set; }
        public string TestEndDate { get; set; }
        public string Test_Type { get; set; }
        public string TestUserID { get; set; }
        public string IsLabTest { get; set; }
        public string Test_Penaltypercent { get; set; }
        public string Test_PreReqGroup { get; set; }
        public string IsEnableLockdownBrowser { get; set; }
    }

    public class AssAttendancePercentaceInfo
    {
        [Required]
        public string TestId { get; set; }
        public string Min_AttendancePercent { get; set; }
        public string AttenPreReqTestAtm { get; set; }
        public string AttendancePercentObtain { get; set; }

    }

    public class TestPaperLoInfo
    {
        [Required]
        public string TestId { get; set; }
        public string TestPreReqId { get; set; }
        public string TestPreReq_LevelID { get; set; }
        public string TestPreReq_TimeAlloted { get; set; }
        public string TestPreReq_LevelParent { get; set; }
        public string TestPreReq_LevelLo { get; set; }
        public string CRTimeSpent_TimeSpentMin { get; set; }
        public string CourseHirarchy { get; set; }
        public string FilePath { get; set; }
    }
}