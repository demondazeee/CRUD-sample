using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;
using test_crud.Entities;
using test_crud.Models;
using test_crud.Services;
using test_crud.Validators;

namespace test_crud.Controllers
{
    [Route("/tasks")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        public ITasksRepository taskRepo { get; set; }
        public IUsersRepository userRepo { get; set; }
        public IMapper mapper { get; set; }
        public TasksController(IMapper mapper, ITasksRepository taskRepo, IUsersRepository userRepo)
        {
            this.userRepo= userRepo;
            this.taskRepo= taskRepo;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDetailsDto>>> GetTasks(
            string? name,
            string? search,
            TasksStatus status,
            int pageSize = 10,
            int pageNumber = 1
            )
        {
            var userId = userRepo.GetAuthenticatedUserId();
            var (results, pageMetadata) = await taskRepo.GetAllTasks(name, search , pageNumber, pageSize, new Guid(userId), status);

            var mappedResults = mapper.Map<IEnumerable<TaskDetailsDto>>(results);


           Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pageMetadata));

            return Ok(mappedResults);
        }

        [HttpGet("{taskId}")]
        public async Task<ActionResult<TaskDetailsDto>> GetTask(string taskId)
        {
            var userId = userRepo.GetAuthenticatedUserId();

            var tasks = await taskRepo.GetValueByExpression(v => v.Id == new Guid(taskId) && v.OwnerId== new Guid(userId));

            if(tasks == null)
            {
                return NotFound();
            }

            var result = mapper.Map<TaskDetailsDto>(tasks);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<TaskDetailsDto>> CreateTask(CreateTaskDto taskDto)
        {

            var validator = new CreateTaskValidator();
            var validationResult = validator.Validate(taskDto);

            if(!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            var userId = userRepo.GetAuthenticatedUserId();

            var user = await userRepo.GetValueByExpression(v => v.Id ==new Guid(userId));

            if(user == null)
            {
                return Unauthorized();
            }

            var mappedTasks = mapper.Map<Tasks>(taskDto);

            user.Tasks.Add(mappedTasks);

            await userRepo.SaveChanges();


            var result = mapper.Map<TaskDetailsDto>(mappedTasks);
            return Ok(result);
        }

        [HttpPatch("{taskId}")]
        public async Task<ActionResult<TaskDetailsDto>> PatchTask(string taskId, JsonPatchDocument<UpdateTaskDto> updateDto)
        {
            var userId = userRepo.GetAuthenticatedUserId();

            var tasks = await taskRepo.GetValueByExpression(v => v.Id == new Guid(taskId) && v.OwnerId == new Guid(userId));

            if(tasks == null)
            {
                return NotFound();
            }

            var mappedTasks = mapper.Map<UpdateTaskDto>(tasks);

            updateDto.ApplyTo(mappedTasks, ModelState);

            if(!TryValidateModel(mappedTasks)) 
            {
                return BadRequest(ModelState);
            }

            var validator = new UpdateTaskValidator();

            var validatorResult = validator.Validate(mappedTasks);

            if(!validatorResult.IsValid) {
                return BadRequest(validatorResult.Errors);
            }

            // if(!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            var result = mapper.Map(mappedTasks, tasks);

            await taskRepo.SaveChanges();

            return Ok(mapper.Map<TaskDetailsDto>(result));
        }

        [HttpDelete("{taskId}")]
        public async Task<ActionResult<TaskDetailsDto>> DeleteTask(string taskId)
        {
            var userId = userRepo.GetAuthenticatedUserId();

            var tasks = await taskRepo.GetValueByExpression(v => v.Id == new Guid(taskId) && v.OwnerId == new Guid(userId));

            if(tasks == null)
            {
                return NotFound();
            }

            await taskRepo.Delete(tasks);

            return Ok(mapper.Map<TaskDetailsDto>(tasks));
        }
    }
}