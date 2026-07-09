using System.ComponentModel.DataAnnotations;

namespace AllocationStatementsApi.Enums
{
    /// <summary>
    /// The allocation funding stream type.
    /// Remember to also update the same enum in the monolith solution.
    /// </summary>
    public enum AllocationFundingStreamType
    {
        [Display(Name = "Unknown Allocation funding stream type", Description = "Unknown Allocation funding stream type", ShortName = "UNKNOWN")]
        Unknown = 0,

        [Display(Name = "ESFA funded adult education budget grant", Description = "ESFA funded adult education budget grant", ShortName = "AEB (grant)")]
        AdultEducationBudgetGrant = 1,

        [Display(Name = "ESFA funded adult education budget contract for services", Description = "ESFA funded adult education budget contract for services", ShortName = "AEB (cfs)")]
        AdultEducationBudgetContractForService = 2,

        [Display(Name = "16 to 18 traineeships", Description = "16 to 18 traineeships", ShortName = "16-18 Traineeships")]
        SixteenToEightteenTraineeships = 3,

        [Display(Name = "16 to 18 traineeships bursary", Description = "16 to 18 traineeships bursary", ShortName = "16-18 Traineeships bursary")]
        SixteenToEightteenTraineeshipsBursary = 4,

        [Display(Name = "Advanced learner loan facility", Description = "advanced learner loan facility", ShortName = "Loan facility")]
        LoanFacillity = 5,

        [Display(Name = "Advanced learner loan bursary", Description = "advanced learner loan bursary", ShortName = "Loan bursary")]
        LoanBursary = 6,

        [Display(Name = "Apprenticeship carry-in", Description = "apprenticeship carry-in", ShortName = "Apprenticeship carry-in")]
        ApprenticeshipCarryIn = 7,

        [Display(Name = "Non-levy apprenticeship", Description = "non-levy apprenticeship", ShortName = "Apprenticeshipnon-levy")]
        NonLevyApprenticeship = 8,

        [Display(Name = "Adult skills fund grant", Description = "Adult skills fund grant", ShortName = "Adult skills fund (grant)")]
        AdultSkillsFundGrant = 9,

        [Display(Name = "Adult skills fund contract for services", Description = "Adult skills fund contract for services", ShortName = "Adult skills fund (cfs)")]
        AdultSkillsFundContractForService = 10,
    }
}