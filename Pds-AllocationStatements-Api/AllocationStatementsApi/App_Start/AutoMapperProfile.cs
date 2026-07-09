using AllocationStatementsApi.Services.Implementations.IAllocationSearchService.Models;
using AllocationStatementsApi.Services.Implementations.IFileMetadata.Models;
using AllocationStatementsApi.Services.Interfaces.IAllocationSearchService.Models;
using AutoMapper;
using Newtonsoft.Json;

namespace AllocationStatementsApi
{
    /// <summary>
    /// Class to configure Automapper.
    /// </summary>
    /// /// <seealso cref="AutoMapper.Profile"/>
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfile"/> class.
        /// Configures Automapper mappings.
        /// </summary>
        public AutoMapperProfile()
        {
            CreateMap<IAllocationSearchDocument, AllocationApiSearchAllocation>()
                    .ForMember(
                        d => d.Streams,
                        o => o.MapFrom(
                            src => src.Streams.Select(s => JsonConvert.DeserializeObject<FundingStream>(s))))
                    .ReverseMap();
        }
    }
}