using AutoMapper;
using MeetupApp.Core.DataTransferObjects;
using MeetupApp.DataBase.Entities;

namespace MeetupApp.WebAPI.MappingProfiles
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<Event, EventDto>();

            CreateMap<EventDto, Event>()
                .ForMember(ent => ent.Id, opt => opt.MapFrom(dto => Guid.NewGuid()))
                .ForMember(ent => ent.CreatedDate, opt => opt.MapFrom(dto => DateTime.Now));
        }
    }
}
