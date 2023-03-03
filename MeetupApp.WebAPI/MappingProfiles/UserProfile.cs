﻿using AutoMapper;
using MeetupApp.Core.DataTransferObjects;
using MeetupApp.DataBase.Entities;

namespace MeetupApp.WebAPI.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile() 
        {
            CreateMap<User, UserDto>()
                .ForMember(dto => dto.RoleName, opt => opt.MapFrom(entity => entity.Role.Name));

            CreateMap<UserDto, User>()
                .ForMember(ent => ent.Id, opt => opt.MapFrom(dto => Guid.NewGuid()))
                .ForMember(ent => ent.RegistrationDate, opt => opt.MapFrom(dto => DateTime.Now));
        }
    }
}
