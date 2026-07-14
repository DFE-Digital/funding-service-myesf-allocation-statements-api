using System.ComponentModel.DataAnnotations;

namespace AllocationStatementsApi.Enums
{
    /// <summary>
    /// The types of allocation line that can be in a funding stream.
    /// Remember to also update the same enum in the monolith solution.
    /// </summary>
    public enum AllocationLineType
    {
        [Display(Name = "Unknown allocation line type", Description = "Unknown Allocation line type", ShortName = "unknown")]
        Unknown = 0,

        [Display(Name = "19 to 24 traineeships allocation", Description = "19 to 24 traineeships allocation", ShortName = "19-24 traineeships")]
        NineteenToTwentyFourTraineeshipsAllocation = 1,

        [Display(Name = "19 to 24 traineeships allocation", Description = "19 to 24 traineeships allocation", ShortName = "19-24 traineeships allocation")]
        NineteenToTwentyFourTraineeshipsAllocation2 = 2,

        [Display(Name = "Adult skills allocation", Description = "adult skills allocation", ShortName = "adult skills")]
        AdultSkillsAllocation = 3,

        [Display(Name = "Adult skills allocation", Description = "adult skills allocation", ShortName = "adult skills allocation")]
        AdultSkillsAllocation2 = 4,

        [Display(Name = "Illustrative continuing learners", Description = "illustrative continuing learners", ShortName = "illustrative continuing learners")]
        IllustrativeContinuingLearners = 5,

        [Display(Name = "Learner support allocation", Description = "learner support allocation", ShortName = "learner support")]
        LearnerSupportAllocation = 6,

        [Display(Name = "Community learning allocation", Description = "learner support allocation", ShortName = "community learning allocation")]
        CommunityLearningAllocation = 7,

        [Display(Name = "Community learning allocation", Description = "learner support allocation", ShortName = "community learning")]
        CommunityLearningAllocation2 = 8,

        [Display(Name = "Illustrative continuing learners", Description = "illustrative continuing learners", ShortName = "illustrative continuing")]
        IllustrativeContinuingLearners2 = 9,

        [Display(Name = "Continuing learners in a devolved area", Description = "Continuing learners in a devolved area", ShortName = "continuing learners in a devolved area")]
        ContinuingLearnersInADevolvedArea = 10,

        [Display(Name = "Continuing learners outside a devolved area", Description = "Continuing learners outside a devolved area", ShortName = "continuing learners outside a devolved area")]
        ContinuingLearnersOutsideADevolvedArea = 11,

        [Display(Name = "16 to 18 apprentices that started before 1 May 2017", Description = "16 to 18 apprenticeship carry-in that started before 1 may 2017", ShortName = "16-18 apprentices that started before 1 may 2017")]
        SixteenToEightteenCarryInApprenticesPreMay = 12,

        [Display(Name = "16 to 18 apprentices that started before 1 May 2017", Description = "16 to 18 apprenticeship carry-in that started before 1 may 2017", ShortName = "16 to 18 apprentices that started before 1 may 2017")]
        SixteenToEightteenCarryInApprenticesPreMay2 = 13,

        [Display(Name = "19+ apprentices that started before 1 May 2017", Description = "19+ apprenticeship carry-in that started before 1 may 2017", ShortName = "adult apprentices that started before 1 may 2017")]
        NineteenPlusCarryInApprenticesPreMay = 14,

        [Display(Name = "19+ apprentices that started before 1 May 2017", Description = "19+ apprenticeship carry-in that started before 1 may 2017", ShortName = "19+ apprentices that started before 1 may 2017")]
        NineteenPlusCarryInApprenticesPreMay2 = 15,

        [Display(Name = "16 to 18 non-levy apprentices that started between 1 May 2017 and 31 December 2017", Description = "16 to 18 apprenticeship carry-in non-levy that started before 1 may 2017", ShortName = "16-18 non-levy apprentices that started between 1 may 2017 and 31 december 2017")]
        SixteenToEightteenCarryInApprenticesNonLevyMayDecember = 16,

        [Display(Name = "16 to 18 non-levy apprentices that started between 1 May 2017 and 31 December 2017", Description = "16 to 18 apprenticeship carry-in non-levy that started before 1 may 2017", ShortName = "16 to 18 non-levy apprentices that started between 1 may 2017 and 31 december 2017")]
        SixteenToEightteenCarryInApprenticesNonLevyMayDecember2 = 17,

        [Display(Name = "19+ non-levy apprentices that started between 1 May 2017 and 31 December 2017", Description = "19+ apprenticeship carry-in non-levy that started between 1 May 2017 and 31 December 2017", ShortName = "adult non-levy apprentices that started between 1 may 2017 and 31 december 2017")]
        NineteenPlusCarryInApprenticesNonLevyMayDecember = 18,

        [Display(Name = "19+ non-levy apprentices that started between 1 May 2017 and 31 December 2017", Description = "19+ apprenticeship carry-in non-levy that started between 1 May 2017 and 31 December 2017", ShortName = "19+ non-levy apprentices that started between 1 may 2017 and 31 december 2017")]
        NineteenPlusCarryInApprenticesNonLevyMayDecember2 = 19,

        [Display(Name = "Illustrative continuing learners in newly devolved areas", Description = "Illustrative continuing learners in newly devolved areas", ShortName = "illustrative continuing learners in newly devolved areas")]
        IllustrativeContinuingLearnersInNewlyDevolvedAreas = 20,

        [Display(Name = "19 to 24 traineeships allocation including learner support", Description = "19 to 24 traineeships allocation including learner support ", ShortName = "19-24 traineeships including learner support")]
        NineteenToTwentyFourTraineeshipsAllocation3 = 21,

        [Display(Name = "Covid-19 skills offer: High value courses for school and college leavers", Description = "Covid-19 skills offer: High value courses for school and college leavers", ShortName = "covid-19 skills offer: high value courses for school and college leavers")]
        Covid19SkillsOffer = 22,

        [Display(Name = "Sector-based work academies", Description = "sector-based work academies", ShortName = "sector-based work academies")]
        SectorBasedWorAcademies = 23,

        [Display(Name = "Continuing learners in newly devolved areas", Description = "continuing learners in newly devolved areas", ShortName = "continuing learners in newly devolved areas")]
        ContinuingLearnersInNewlyDevolvedAreas = 24,

        [Display(Name = "Covid-19 skills offer: national skills fund level 3 offer for 24 year old and over", Description = "covid-19 skills offer: national skills fund level 3 offer for 24 year old and over", ShortName = "covid-19 skills offer: national skills fund level 3 offer for 24 year old and over")]
        Covid19SkillsOfferNationalSkillsFundLevel3OfferFor24YearOldAndOver = 25,

        [Display(Name = "National skills fund: level 3 offer for 24 year olds and over", Description = "national skills fund: level 3 offer for 24 year olds and over", ShortName = "national skills fund: level 3 offer for 24 year olds and over")]
        NationalSkillsFundLevel3OfferFor4YearOldsAndOver = 26,

        [Display(Name = "National Skills Fund: level 3 free courses for jobs", Description = "national skills fund: level 3 free courses for jobs", ShortName = "national skills fund: level 3 free courses for jobs")]
        NationalSkillsFundLevel3FreeCoursesForJobs = 27,

        [Display(Name = "National Skills Fund: level 3 free courses for jobs learner support", Description = "national skills fund: level 3 free courses for jobs learner support", ShortName = "national skills fund: level 3 free courses for jobs learner support")]
        NationalSkillsFundLevel3FreeCoursesForJobsLearnerSupport = 28,

        [Display(Name = "16 to 18 non-levy apprenticeship allocation", Description = "16 to 18 non-levy apprenticeship allocation", ShortName = "16-18 non-levy apprentices")]
        SixteenToEighteenNonLevyApprentices = 29,

        [Display(Name = "19+ non-levy apprenticeship allocation", Description = "19+ non-levy apprenticeship allocation", ShortName = "adult  non-levy apprentices")]
        NineteenPlusNonLevyApprentices = 30,

        [Display(Name = "Funding for innovative provision", Description = "Funding for innovative provision", ShortName = "funding for innovative provision")]
        FundingForInnovativeProvision = 31,

        [Display(Name = "19 to 24 traineeships for continuing learners only", Description = "19 to 24 traineeships for continuing learners only", ShortName = "19 to 24 traineeships for continuing learners only")]
        NineteenToTwentyFourTraineeshipsAllocation4 = 32,

        [Display(Name = "Regulated provision", Description = "regulated provision for august to march", ShortName = "regulated provision aug_mar")]
        RegulatedProvision1 = 33,

        [Display(Name = "Regulated provision", Description = "regulated provision for april to july", ShortName = "regulated provision apr_jul")]
        RegulatedProvision2 = 34,

        [Display(Name = "Tailored learning", Description = "tailored learning allocation for august to march", ShortName = "tailored learning total aug_mar")]
        TailoredLearningAllocation1 = 35,

        [Display(Name = "Tailored learning", Description = "tailored learning allocation for april to july", ShortName = "tailored learning total apr_jul")]
        TailoredLearningAllocation2 = 36,

        [Display(Name = "Funding for innovative provision", Description = "Funding for innovative provision for august to march", ShortName = "funding for innovative provision aug_mar")]
        FundingForInnovativeProvision1 = 37,

        [Display(Name = "Funding for innovative provision", Description = "Funding for innovative provision for april to july", ShortName = "funding for innovative provision apr_jul")]
        FundingForInnovativeProvision2 = 38,

        [Display(Name = "NSF free courses for jobs", Description = "nsf free courses for jobs for august to march", ShortName = "nsf free courses for jobs aug_mar")]
        NsfFreeCoursesForJobs1 = 39,

        [Display(Name = "NSF free courses for jobs", Description = "nsf free courses for jobs for april to july", ShortName = "nsf free courses for jobs apr_jul")]
        NsfFreeCoursesForJobs2 = 40,

        [Display(Name = "Indicative continuing learners in newly devolved areas - regulated provision", Description = "indicative continuing learners in newly devolved areas - regulated provision for august to march", ShortName = "indicative continuing learners in newly devolved areas - regulated provision aug_mar")]
        IndicativeContinuingRegulatedProvision1 = 41,

        [Display(Name = "Indicative continuing learners in newly devolved areas - regulated provision", Description = "indicative continuing learners in newly devolved areas - regulated provision for april to july", ShortName = "indicative continuing learners in newly devolved areas - regulated provision apr_jul")]
        IndicativeContinuingRegulatedProvision2 = 42,

        [Display(Name = "Indicative continuing learners in newly devolved areas - tailored learning", Description = "indicative continuing learners in newly devolved areas - tailored learning for august to march", ShortName = "indicative continuing learners in newly devolved areas - tailored learning aug_mar")]
        IndicativeContinuingTailoredLearning1 = 43,

        [Display(Name = "Indicative continuing learners in newly devolved areas - tailored learning", Description = "indicative continuing learners in newly devolved areas - tailored learning for april to july", ShortName = "indicative continuing learners in newly devolved areas - tailored learning apr_jul")]
        IndicativeContinuingTailoredLearning2 = 44,

        [Display(Name = "Indicative continuing learners in newly devolved areas - NSF free courses for jobs", Description = "indicative continuing learners in newly devolved areas - nsf free courses for august to march (Adult Skills Fund(grant)", ShortName = "indicative  continuing learners in newly devolved areas - nsf free courses for jobs aug_mar")]
        IndicativeContinuingNsfFreeCourses1 = 45,

        [Display(Name = "Indicative continuing learners in newly devolved areas - NSF free courses for jobs", Description = "indicative continuing learners in newly devolved areas - nsf free courses for april to july (Adult Skills Fund(grant)", ShortName = "indicative  continuing learners in newly devolved areas - nsf free courses for jobs apr_jul")]
        IndicativeContinuingNsfFreeCourses2 = 46,

        [Display(Name = "Regulated provision learner support", Description = "regulated provision learner support for august to march", ShortName = "regulated provision learner support aug_mar")]
        RegulatedProvisionLearnerSupport1 = 47,

        [Display(Name = "Regulated provision learner support", Description = "regulated provision learner support for april to july", ShortName = "regulated provision learner support apr_jul")]
        RegulatedProvisionLearnerSupport2 = 48,

        [Display(Name = "NSF free courses for jobs learner support", Description = "nsf free courses for jobs learner support for august to march", ShortName = "nsf free courses for jobs learner support aug_mar")]
        NsfFreeCoursesForJobsLearnerSupport1 = 49,

        [Display(Name = "NSF free courses for jobs learner support", Description = "nsf free courses for jobs learner support for april to july", ShortName = "nsf free courses for jobs learner support apr_jul")]
        NsfFreeCoursesForJobsLearnerSupport2 = 50,

        [Display(Name = "Indicative continuing learners in newly devolved areas - NSF free courses for jobs", Description = "indicative continuing learners in newly devolved areas - nsf free courses for august to march (Adult Skills Fund(cfs)", ShortName = "indicative continuing learners in newly devolved areas - nsf free courses for jobs aug_mar")]
        IndicativeContinuingNsfFreeCourses3 = 51,

        [Display(Name = "Indicative continuing learners in newly devolved areas - NSF free courses for jobs", Description = "indicative continuing learners in newly devolved areas - nsf free courses for april to july (Adult Skills Fund(cfs)", ShortName = "indicative continuing learners in newly devolved areas - nsf free courses for jobs apr_jul")]
        IndicativeContinuingNsfFreeCourses4 = 52,

        [Display(Name = "Adult skills core", Description = "adult skills core for august to march", ShortName = "adult skills core aug_mar")]
        AdultSkillsCore1 = 53,

        [Display(Name = "Adult skills core", Description = "adult skills core for april to july", ShortName = "adult skills core apr_jul")]
        AdultSkillsCore2 = 54,

        [Display(Name = "Indicative continuing learners in newly devolved areas - adult skills core", Description = "indicative continuing learners in newly devolved areas - adult skills core for august to march", ShortName = "indicative continuing learners in newly devolved areas - adult skills core aug_mar")]
        IndicativeContinuingAdultSkillsCore1 = 55,

        [Display(Name = "Indicative continuing learners in newly devolved areas - adult skills core", Description = "indicative continuing learners in newly devolved areas - adult skills core for april to july", ShortName = "indicative continuing learners in newly devolved areas - adult skills core apr_jul")]
        IndicativeContinuingAdultSkillsCore2 = 56,

        [Display(Name = "Indicative continuing learners in newly devolved areas - free courses for jobs", Description = "indicative continuing learners in newly devolved areas - free courses for august to march (Adult Skills Fund(grant))", ShortName = "indicative  continuing learners in newly devolved areas - free courses for jobs aug_mar")]
        IndicativeContinuingFreeCourses1 = 57,

        [Display(Name = "Indicative continuing learners in newly devolved areas - free courses for jobs", Description = "indicative continuing learners in newly devolved areas - free courses for april to july (Adult Skills Fund(grant))", ShortName = "indicative  continuing learners in newly devolved areas - free courses for jobs apr_jul")]
        IndicativeContinuingFreeCourses2 = 58,

        [Display(Name = "Free courses for jobs", Description = "free courses for jobs for august to march", ShortName = "free courses for jobs aug_mar")]
        FreeCoursesForJobs1 = 59,

        [Display(Name = "Free courses for jobs", Description = "free courses for jobs for april to july", ShortName = "free courses for jobs apr_jul")]
        FreeCoursesForJobs2 = 60,

        [Display(Name = "Adult skills core learner support", Description = "Adult skills core learner support for august to march", ShortName = "adult skills core learner support aug_mar")]
        AdultSkillsCoreLearnerSupport1 = 61,

        [Display(Name = "Adult skills core learner support", Description = "Adult skills core learner support for april to july", ShortName = "adult skills core learner support apr_jul")]
        AdultSkillsCoreLearnerSupport2 = 62,

        [Display(Name = "Free courses for jobs learner support", Description = "free courses for jobs learner support for august to march", ShortName = "free courses for jobs learner support aug_mar")]
        FreeCoursesForJobsLearnerSupport1 = 63,

        [Display(Name = "Free courses for jobs learner support", Description = "free courses for jobs learner support for april to july", ShortName = "free courses for jobs learner support apr_jul")]
        FreeCoursesForJobsLearnerSupport2 = 64,

        [Display(Name = "Indicative continuing learners in newly devolved areas - free courses for jobs", Description = "indicative continuing learners in newly devolved areas - free courses for august to march (Adult Skills Fund(cfs))", ShortName = "indicative continuing learners in newly devolved areas -free courses for jobs aug_mar")]
        IndicativeContinuingFreeCourses3 = 65,

        [Display(Name = "Indicative continuing learners in newly devolved areas - free courses for jobs", Description = "indicative continuing learners in newly devolved areas - free courses for april to july (Adult Skills Fund(cfs))", ShortName = "indicative continuing learners in newly devolved areas - free courses for jobs apr_jul")]
        IndicativeContinuingFreeCourses4 = 66,

        [Display(Name = "Continuing learners in newly devolved areas - adult skills core", Description = "continuing learners in newly devolved areas - adult skills core for august to march", ShortName = "continuing learners in newly devolved areas - adult skills core aug_mar")]
        ContinuingAdultSkillsCore1 = 67,

        [Display(Name = "Continuing learners in newly devolved areas - adult skills core", Description = "continuing learners in newly devolved areas - adult skills core for april to july", ShortName = "continuing learners in newly devolved areas - adult skills core apr_jul")]
        ContinuingAdultSkillsCore2 = 68,

        [Display(Name = "Continuing learners in newly devolved areas - tailored learning", Description = "continuing learners in newly devolved areas - tailored learning for august to march", ShortName = "continuing learners in newly devolved areas - tailored learning aug_mar")]
        ContinuingTailoredLearning1 = 69,

        [Display(Name = "Continuing learners in newly devolved areas - tailored learning", Description = "continuing learners in newly devolved areas - tailored learning for april to july", ShortName = "continuing learners in newly devolved areas - tailored learning apr_jul")]
        ContinuingTailoredLearning2 = 70,

        [Display(Name = "Continuing learners in newly devolved areas - free courses for jobs", Description = "continuing learners in newly devolved areas - free courses for august to march (Adult Skills Fund(grant))", ShortName = "continuing learners in newly devolved areas - free courses for jobs aug_mar")]
        ContinuingFreeCourses1 = 71,

        [Display(Name = "Continuing learners in newly devolved areas - free courses for jobs", Description = "continuing learners in newly devolved areas - free courses for august to march (Adult Skills Fund(cfs))", ShortName = "continuing learners in newly devolved areas -free courses for jobs aug_mar")]
        ContinuingFreeCourses2 = 72,

        [Display(Name = "Continuing learners in newly devolved areas - free courses for jobs", Description = "continuing learners in newly devolved areas - free courses for april to july", ShortName = "continuing learners in newly devolved areas - free courses for jobs apr_jul")]
        ContinuingFreeCourses3 = 73
    }
}