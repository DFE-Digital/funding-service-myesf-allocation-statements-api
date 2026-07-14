using AllocationStatementsApi.Enums;
using AllocationStatementsApi.Models;
using AllocationStatementsApi.Services.Implementations.IAllocationSearchService.Models;
using AllocationStatementsApi.Services.Implementations.IFileMetadata.Models;
using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models;
using AllocationStatementsApi.Services.Interfaces.IFileMetadata.Models;
using FileAction = AllocationStatementsApi.Services.Enums.FileAction;
using Type = AllocationStatementsApi.Services.Implementations.IFileMetadata.Models.Type;

namespace AllocationStatementsApi.Tests
{
    public class BaseDomainUnitLogicTest
    {
        public string MainUkprn => "12345678";

        #region Helpers

        protected static Services.Interfaces.IAllocationSearchService.Models.ISearchResult<IFundingSearchDocument> GetRawSearchResult(string id)
        {
            return new AzureSearchResult<IFundingSearchDocument>
            {
                Documents = new List<IFundingSearchDocument>
                {
                    new AzureFundingSearchDocument
                    {
                        Id = id
                    }
                }
            };
        }

        protected static Type GetRawAllocationStatement(
            string ukprn, FundingStatementType fundingStatementType, int year = 1920, int versionNumber = 1)
        {
            var financialEnvelopes = new List<FinancialEnvelope>
            {
                new FinancialEnvelope { Name = "Aug_Ma", Amount = 200 },
                new FinancialEnvelope { Name = "Apr_Jul", Amount = 250 }
            };

            var extraInfo = new Dictionary<string, object>
            {
                { string.Empty, 1 }
            };

            var allocationLines = new List<Line>();

            var typeName = string.Empty;

            var fundingStreams = new List<FundingStream>();

            if (fundingStatementType == FundingStatementType.AdultEducationBudget)
            {
                typeName = "Adults";

                allocationLines = new List<Line>
                {
                    new Line
                    {
                        Name = "adult skills",
                        Amount = 50
                    },
                    new Line
                    {
                        Name = "community learning",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "19-24 traineeships",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "continuing learners in a devolved area",
                        Amount = 200
                    },
                    new Line
                    {
                        Name = "continuing learners outside a devolved area",
                        Amount = 0
                    }
                };

                fundingStreams.Add(
                    new FundingStream
                    {
                        Name = "AEB (cfs)",
                        Lines = allocationLines,
                        BaselineAmount = 400,
                        Envelopes = financialEnvelopes,
                        TotalAmount = 450
                    });

                var allocationLines2 = new List<Line>
                {
                    new Line
                    {
                        Name = "adult skills",
                        Amount = 50
                    },
                    new Line
                    {
                        Name = "community learning",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "19-24 traineeships",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "continuing learners in a devolved area",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "continuing learners outside a devolved area",
                        Amount = 200
                    }
                };

                fundingStreams.Add(
                    new FundingStream
                    {
                        Name = "AEB (grant)",
                        Lines = allocationLines2,
                        BaselineAmount = 400,
                        Envelopes = financialEnvelopes,
                        TotalAmount = 450
                    });
            }
            else if (fundingStatementType == FundingStatementType.AdvancedLearnerLoans)
            {
                typeName = "Loans";

                fundingStreams.Add(
                    new FundingStream
                    {
                        Name = "Loan facility",
                        Lines = allocationLines,
                        BaselineAmount = 400,
                        Envelopes = financialEnvelopes,
                        TotalAmount = 450
                    });
                fundingStreams.Add(
                    new FundingStream
                    {
                        Name = "Loan bursary",
                        Lines = allocationLines,
                        BaselineAmount = 400,
                        Envelopes = financialEnvelopes,
                        TotalAmount = 450
                    });
            }
            else if (fundingStatementType == FundingStatementType.Traineeships)
            {
                typeName = "Traineeships";

                fundingStreams.Add(
                    new FundingStream
                    {
                        Name = "16-18 Traineeships",
                        Lines = allocationLines,
                        BaselineAmount = 400,
                        Envelopes = financialEnvelopes,
                        TotalAmount = 450
                    });
            }
            else if (fundingStatementType == FundingStatementType.ApprenticeshipCarryIn)
            {
                typeName = "Apprenticeships";

                allocationLines = new List<Line>
                {
                    new Line
                    {
                        Name = "16-18 apprentices that started before 1 May 2017",
                        Amount = 200
                    },
                    new Line
                    {
                        Name = "adult apprentices that started before 1 May 2017",
                        Amount = 250
                    }
                };

                fundingStreams.Add(
                    new FundingStream
                    {
                        Name = "Apprenticeship carry-in",
                        Lines = allocationLines,
                        BaselineAmount = 400,
                        Envelopes = financialEnvelopes,
                        TotalAmount = 450
                    });
            }
            else if (fundingStatementType == FundingStatementType.ApprenticeshipNonLevy)
            {
                typeName = "Apprenticeshipnon-levy";

                allocationLines = new List<Line>
                {
                    new Line
                    {
                        Name = "16-18 non-levy apprentices",
                        Amount = 200
                    },
                    new Line
                    {
                        Name = "adult  non-levy apprentices",
                        Amount = 250
                    }
                };

                fundingStreams.Add(
                    new FundingStream
                    {
                        Name = "Apprenticeshipnon-levy",
                        Lines = allocationLines,
                        BaselineAmount = 400,
                        Envelopes = financialEnvelopes,
                        TotalAmount = 450
                    });
            }
            else if (fundingStatementType == FundingStatementType.ESFAAdultSkillsFund)
            {
                typeName = "Adults";

                allocationLines = new List<Line>
                {
                    new Line
                    {
                        Name = "adult skills core aug_mar",
                        Amount = 50
                    },
                    new Line
                    {
                        Name = "adult skills core apr_jul",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "tailored learning total aug_mar",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "tailored learning total apr_jul",
                        Amount = 200
                    },
                    new Line
                    {
                        Name = "funding for innovative provision aug_mar",
                        Amount = 153216
                    },
                    new Line
                    {
                        Name = "funding for innovative provision apr_jul",
                        Amount = 76596
                    },
                    new Line
                    {
                        Name = "free courses for jobs aug_mar",
                        Amount = 153216
                    },
                    new Line
                    {
                        Name = "free courses for jobs apr_jul",
                        Amount = 76596
                    },
                    new Line
                    {
                        Name = "indicative continuing learners in newly devolved areas - adult skills core aug_mar",
                        Amount = 3348
                    },
                    new Line
                    {
                        Name = "indicative continuing learners in newly devolved areas - adult skills core apr_jul",
                        Amount = 1674
                    },
                    new Line
                    {
                        Name = "indicative continuing learners in newly devolved areas - tailored learning aug_mar",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "indicative continuing learners in newly devolved areas - tailored learning apr_jul",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "indicative  continuing learners in newly devolved areas - free courses for jobs aug_mar",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "indicative  continuing learners in newly devolved areas - free courses for jobs apr_jul",
                        Amount = 0
                    },
                };

                fundingStreams.Add(
                    new FundingStream
                    {
                        Name = "Adult skills fund (grant)",
                        Lines = allocationLines,
                        BaselineAmount = 400,
                        Envelopes = financialEnvelopes,
                        TotalAmount = 450
                    });

                var allocationLines2 = new List<Line>
                {
                    new Line
                    {
                        Name = "adult skills core aug_mar",
                        Amount = 60
                    },
                    new Line
                    {
                        Name = "adult skills core apr_jul",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "adult skills core learner support aug_mar",
                        Amount = 200
                    },
                    new Line
                    {
                        Name = "adult skills core learner support apr_jul",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "free courses for jobs aug_mar",
                        Amount = 153216
                    },
                    new Line
                    {
                        Name = "free courses for jobs apr_jul",
                        Amount = 76596
                    },
                    new Line
                    {
                        Name = "free courses for jobs learner support aug_mar",
                        Amount = 3348
                    },
                    new Line
                    {
                        Name = "free courses for jobs learner support apr_jul",
                        Amount = 1674
                    },
                    new Line
                    {
                        Name = "indicative continuing learners in newly devolved areas - adult skills core aug_mar",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "indicative continuing learners in newly devolved areas - adult skills core apr_jul",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "indicative continuing learners in newly devolved areas -free courses for jobs aug_mar",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "indicative continuing learners in newly devolved areas - free courses for jobs apr_jul",
                        Amount = 0
                    },
                };

                fundingStreams.Add(
                    new FundingStream
                    {
                        Name = "Adult skills fund (cfs)",
                        Lines = allocationLines2,
                        BaselineAmount = 400,
                        Envelopes = financialEnvelopes,
                        TotalAmount = 450
                    });
            }
            else if (fundingStatementType == FundingStatementType.DFEAdultSkillsFund)
            {
                typeName = "Adults";

                allocationLines = new List<Line>
                {
                    new Line
                    {
                        Name = "adult skills core aug_mar",
                        Amount = 50
                    },
                    new Line
                    {
                        Name = "adult skills core apr_jul",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "tailored learning total aug_mar",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "tailored learning total apr_jul",
                        Amount = 200
                    },
                    new Line
                    {
                        Name = "funding for innovative provision aug_mar",
                        Amount = 153216
                    },
                    new Line
                    {
                        Name = "funding for innovative provision apr_jul",
                        Amount = 76596
                    },
                    new Line
                    {
                        Name = "free courses for jobs aug_mar",
                        Amount = 153216
                    },
                    new Line
                    {
                        Name = "free courses for jobs apr_jul",
                        Amount = 76596
                    },
                    new Line
                    {
                        Name = "continuing learners in newly devolved areas - adult skills core aug_mar",
                        Amount = 3348
                    },
                    new Line
                    {
                        Name = "continuing learners in newly devolved areas - adult skills core apr_jul",
                        Amount = 1674
                    },
                    new Line
                    {
                        Name = "continuing learners in newly devolved areas - tailored learning aug_mar",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "continuing learners in newly devolved areas - tailored learning apr_jul",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "continuing learners in newly devolved areas - free courses for jobs aug_mar",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "continuing learners in newly devolved areas - free courses for jobs apr_jul",
                        Amount = 0
                    },
                };

                fundingStreams.Add(
                    new FundingStream
                    {
                        Name = "Adult skills fund (grant)",
                        Lines = allocationLines,
                        BaselineAmount = 400,
                        Envelopes = financialEnvelopes,
                        TotalAmount = 450
                    });

                var allocationLines2 = new List<Line>
                {
                    new Line
                    {
                        Name = "adult skills core aug_mar",
                        Amount = 60
                    },
                    new Line
                    {
                        Name = "adult skills core apr_jul",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "adult skills core learner support aug_mar",
                        Amount = 200
                    },
                    new Line
                    {
                        Name = "adult skills core learner support apr_jul",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "free courses for jobs aug_mar",
                        Amount = 153216
                    },
                    new Line
                    {
                        Name = "free courses for jobs apr_jul",
                        Amount = 76596
                    },
                    new Line
                    {
                        Name = "free courses for jobs learner support aug_mar",
                        Amount = 3348
                    },
                    new Line
                    {
                        Name = "free courses for jobs learner support apr_jul",
                        Amount = 1674
                    },
                    new Line
                    {
                        Name = "continuing learners in newly devolved areas - adult skills core aug_mar",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "continuing learners in newly devolved areas - adult skills core apr_jul",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "continuing learners in newly devolved areas -free courses for jobs aug_mar",
                        Amount = 0
                    },
                    new Line
                    {
                        Name = "continuing learners in newly devolved areas - free courses for jobs apr_jul",
                        Amount = 0
                    },
                };

                fundingStreams.Add(
                    new FundingStream
                    {
                        Name = "Adult skills fund (cfs)",
                        Lines = allocationLines2,
                        BaselineAmount = 400,
                        Envelopes = financialEnvelopes,
                        TotalAmount = 450
                    });
            }
            var allocationStatement = new Type
            {
                Id = "a47cb3fe-0016-4b57-9fcf-c9313dc97df4",
                Name = typeName,
                UKPRN = ukprn.ToString(),
                Year = year,
                Version = versionNumber,
                FundingStreams = fundingStreams,
                TotalAmount = fundingStreams.Sum(stream => stream.TotalAmount),
                ExtraInfo = extraInfo,
                History = new List<ITypeHistory>
                {
                    new TypeHistory
                    {
                        Action = FileAction.Imported,
                        ActionDateTimeUtc = new DateTime(2019, 3, 11)
                    }
                }
            };
            return allocationStatement;
        }

        protected static AllocationStatement GetExpectedDFEAdultSkillsFundStatement(string id, string ukprn)
        {
            return new AllocationStatement
            {
                CreatedAtDate = new DateTime(2019, 3, 11),
                Id = id,
                Period = "2526",
                TotalValue = 900,
                Type = FundingStatementType.DFEAdultSkillsFund,
                Ukprn = ukprn,
                Version = 1,
                AllocationFundingStreams = new List<AllocationFundingStream>
                    {
                        new AllocationFundingStream
                        {
                            Title = "Adult skills fund (grant)",
                            Baseline = 400,
                            SplitA = 200,
                            SplitB = 250,
                            Total = 450,
                            Type = AllocationFundingStreamType.AdultSkillsFundGrant,
                            AllocationLines = new List<AllocationLine>
                            {
                                new AllocationLine
                                {
                                    Title = "Adult skills core",
                                    Total = 50,
                                    Type = AllocationLineType.AdultSkillsCore1
                                },
                                new AllocationLine
                                {
                                    Title = "Adult skills core",
                                    Total = 0,
                                    Type = AllocationLineType.AdultSkillsCore2
                                },
                                new AllocationLine
                                {
                                    Title = "Tailored learning",
                                    Total = 0,
                                    Type = AllocationLineType.TailoredLearningAllocation1
                                },
                                new AllocationLine
                                {
                                    Title = "Tailored learning",
                                    Total = 200,
                                    Type = AllocationLineType.TailoredLearningAllocation2
                                },
                                new AllocationLine
                                {
                                    Title = "Funding for innovative provision",
                                    Total = 153216,
                                    Type = AllocationLineType.FundingForInnovativeProvision1
                                },
                                new AllocationLine
                                {
                                    Title = "Funding for innovative provision",
                                    Total = 76596,
                                    Type = AllocationLineType.FundingForInnovativeProvision2
                                },
                                new AllocationLine
                                {
                                    Title = "Free courses for jobs",
                                    Total = 153216,
                                    Type = AllocationLineType.FreeCoursesForJobs1
                                },
                                new AllocationLine
                                {
                                    Title = "Free courses for jobs",
                                    Total = 76596,
                                    Type = AllocationLineType.FreeCoursesForJobs2
                                },
                                new AllocationLine
                                {
                                    Title = "Continuing learners in newly devolved areas - adult skills core",
                                    Total = 3348,
                                    Type = AllocationLineType.ContinuingAdultSkillsCore1
                                },
                                new AllocationLine
                                {
                                    Title = "Continuing learners in newly devolved areas - adult skills core",
                                    Total = 1674,
                                    Type = AllocationLineType.ContinuingAdultSkillsCore2
                                },
                                new AllocationLine
                                {
                                    Title = "Continuing learners in newly devolved areas - tailored learning",
                                    Total = 0,
                                    Type = AllocationLineType.ContinuingTailoredLearning1
                                },
                                new AllocationLine
                                {
                                    Title = "Continuing learners in newly devolved areas - tailored learning",
                                    Total = 0,
                                    Type = AllocationLineType.ContinuingTailoredLearning2
                                },
                                new AllocationLine
                                {
                                    Title = "Continuing learners in newly devolved areas - free courses for jobs",
                                    Total = 0,
                                    Type = AllocationLineType.ContinuingFreeCourses1
                                },
                                new AllocationLine
                                {
                                    Title = "Continuing learners in newly devolved areas - free courses for jobs",
                                    Total = 0,
                                    Type = AllocationLineType.ContinuingFreeCourses3
                                },
                            }
                        },

                        new AllocationFundingStream
                        {
                            Title = "Adult skills fund (cfs)",
                            Baseline = 400,
                            SplitA = 200,
                            SplitB = 250,
                            Total = 450,
                            Type = AllocationFundingStreamType.AdultSkillsFundContractForService,
                            AllocationLines = new List<AllocationLine>
                            {
                                new AllocationLine
                                {
                                    Title = "Adult skills core",
                                    Total = 60,
                                    Type = AllocationLineType.AdultSkillsCore1
                                },
                                new AllocationLine
                                {
                                    Title = "Adult skills core",
                                    Total = 0,
                                    Type = AllocationLineType.AdultSkillsCore2
                                },
                                new AllocationLine
                                {
                                    Title = "Adult skills core learner support",
                                    Total = 200,
                                    Type = AllocationLineType.AdultSkillsCoreLearnerSupport1
                                },
                                new AllocationLine
                                {
                                    Title = "Adult skills core learner support",
                                    Total = 0,
                                    Type = AllocationLineType.AdultSkillsCoreLearnerSupport2
                                },
                                new AllocationLine
                                {
                                    Title = "Free courses for jobs",
                                    Total = 153216,
                                    Type = AllocationLineType.FreeCoursesForJobs1
                                },
                                new AllocationLine
                                {
                                    Title = "Free courses for jobs",
                                    Total = 76596,
                                    Type = AllocationLineType.FreeCoursesForJobs2
                                },
                                new AllocationLine
                                {
                                    Title = "Free courses for jobs learner support",
                                    Total = 3348,
                                    Type = AllocationLineType.FreeCoursesForJobsLearnerSupport1
                                },
                                new AllocationLine
                                {
                                    Title = "Free courses for jobs learner support",
                                    Total = 1674,
                                    Type = AllocationLineType.FreeCoursesForJobsLearnerSupport2
                                },
                                new AllocationLine
                                {
                                    Title = "Continuing learners in newly devolved areas - adult skills core",
                                    Total = 0,
                                    Type = AllocationLineType.ContinuingAdultSkillsCore1
                                },
                                new AllocationLine
                                {
                                    Title = "Continuing learners in newly devolved areas - adult skills core",
                                    Total = 0,
                                    Type = AllocationLineType.ContinuingAdultSkillsCore2
                                },
                                new AllocationLine
                                {
                                    Title = "Continuing learners in newly devolved areas - free courses for jobs",
                                    Total = 0,
                                    Type = AllocationLineType.ContinuingFreeCourses2
                                },
                                new AllocationLine
                                {
                                    Title = "Continuing learners in newly devolved areas - free courses for jobs",
                                    Total = 0,
                                    Type = AllocationLineType.ContinuingFreeCourses3
                                },
                            }
                        }
                    },
                HasBeenRead = false
            };
        }

        #endregion
    }
}