using System;
using System.ComponentModel.DataAnnotations;

namespace AllocationStatementsApi.Enums
{
    /// <summary>
    /// The type of allocation.
    /// Remember to also update the same enum in the monolith solution.
    /// </summary>
    public enum FundingStatementType
    {
        [Display(Name = "Other", Description = "Other", ShortName = "other")]
        Other = -1,

        [Display(Name = "ESFA funded adult education budget", Description = "ESFA funded adult education budget", ShortName = "adulteducationbudget")]
        AdultEducationBudget = 0,

        [Display(Name = "Advanced learner loans", Description = "advanced learner loans", ShortName = "advancedlearnerloans")]
        AdvancedLearnerLoans = 1,

        [Display(Name = "Apprenticeship carry-in", Description = "apprenticeship carry-in", ShortName = "apprenticeshipcarryin")]
        ApprenticeshipCarryIn = 2,

        [Display(Name = "Historic", Description = "Historic", ShortName = "historic")]
        Historic = 3,

        [Display(Name = "16 to 18 traineeships", Description = "16 to 18 traineeships", ShortName = "traineeships")]
        Traineeships = 4,

        [Obsolete]
        [Display(Name = "PE And Sport", Description = "PE And Sport", ShortName = "peandsport")]
        PEandSport = 5,

        [Display(Name = "19 to 24 traineeships (2020 procurement)", Description = "19 to 24 traineeships (2020 procurement)", ShortName = "traineeshipprocurement19to24")]
        TraineeshipProcurement19to24 = 6,

        [Display(Name = "Non-levy apprenticeship", Description = "non-levy apprenticeship", ShortName = "apprenticeshipnonlevy")]
        ApprenticeshipNonLevy = 7,

        [Display(Name = "ESFA adult skills fund", Description = "ESFA adult skills fund", ShortName = "esfaadultskillsfund")]
        ESFAAdultSkillsFund = 8,

        [Display(Name = "Adult skills fund", Description = "Adult skills fund", ShortName = "dfeadultskillsfund")]
        DFEAdultSkillsFund = 9,
    }
}