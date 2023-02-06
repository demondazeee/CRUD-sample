using AutoMapper;
using test_crud.Entities;
using test_crud.Models;

namespace test_crud.Profiles
{
    public class TasksProfile : Profile
    {
        public TasksProfile()
        {
            CreateMap<Tasks, TaskDetailsDto>();
            CreateMap<UpdateTaskDto, Tasks>();
            CreateMap<Tasks, UpdateTaskDto>();
            CreateMap<CreateTaskDto, Tasks>();
        }
    }
}
