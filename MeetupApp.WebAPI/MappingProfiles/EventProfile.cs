﻿using AutoMapper;
using MeetupApp.Core.DataTransferObjects;
using MeetupApp.DataBase.Entities;
using MeetupApp.WebAPI.Models.Requests;
using MeetupApp.WebAPI.Models.Responces;

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

            CreateMap<AddOrUpdateEventRequestModel, EventDto>();
            CreateMap<EventDto, EventResponceModel>();
        }
    }
}
