using AllocationStatementsApi.Services.Implementations.IAllocationSearchService.Models;
using AllocationStatementsApi.Services.Implementations.IFileMetadata.Models;
using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllocationStatementsApi.Services.Extensions
{
    public static class AllocationSearchMappings
    {
        public static AllocationApiSearchAllocation ToAllocationApiSearchAllocation(this IAllocationSearchDocument allocationSearchDocument)
        {
            return new AllocationApiSearchAllocation
            {
                Ukprn = allocationSearchDocument.Ukprn,
                ProviderType = allocationSearchDocument.ProviderType,
                ProviderSubType = allocationSearchDocument.ProviderSubType,
                Year = allocationSearchDocument.Year,
                Version = allocationSearchDocument.Version,
                Name = allocationSearchDocument.Name,
                EstablishmentNumber = allocationSearchDocument.EstablishmentNumber,
                LaCode = allocationSearchDocument.LaCode,
                LaName = allocationSearchDocument.LaName,
                Postcode = allocationSearchDocument.Postcode,
                Town = allocationSearchDocument.Town,
                PublishedDateTime = allocationSearchDocument.PublishedDateTime,
                TotalAmount = allocationSearchDocument.TotalAmount,
                Streams = allocationSearchDocument.Streams?.Select(s => JsonConvert.DeserializeObject<FundingStream>(s))
            };
        }
    }
}
