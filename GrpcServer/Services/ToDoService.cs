using Grpc.Core;
using GrpcServer.Data;
using GrpcServer.Models;
using Jackpot.Infra.RabbitConnection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;

namespace GrpcServer.Services
{
    public class ToDoService : ToDoIt.ToDoItBase
    {
        private readonly AppDbContext _dbContext;
        public ToDoService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request,
            ServerCallContext context)
        {
            if (request.UserId == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must send a valid object"));

            var toDoItem = new Item
            {
                UserId = request.UserId,
                JackpotValue = request.JackpotValue,
                Time = DateTime.Now.ToString()
            };

            await _dbContext.AddAsync(toDoItem);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new CreateToDoResponse
            {
                Id = toDoItem.Id
            });
        }

        public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
        {

            if (request.Id < 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "resource index must be greater than 0"));

            var toDoItem = await _dbContext.Items.FirstOrDefaultAsync(t => t.Id == request.Id);

            if (toDoItem != null)
            {
                return await Task.FromResult(new ReadToDoResponse
                {
                    Id = toDoItem.Id,
                    UserId = toDoItem.UserId,
                    JackpotValue = toDoItem.JackpotValue,
                    Time = toDoItem.Time
                });
            }

            throw new RpcException(new Status(StatusCode.NotFound, $"No task with id:{request.Id} was found"));
        }

        public override async Task<GetAllResponse> GetAllToDo(GetAllRequest request, ServerCallContext context)
        {
            var response = new GetAllResponse();
            var toDoItems = await _dbContext.Items.ToListAsync();

            foreach (var toDo in toDoItems)
            {
                response.ToDo.Add(new ReadToDoResponse
                {
                    Id = toDo.Id,
                    UserId = toDo.UserId,
                    JackpotValue = toDo.JackpotValue,
                    Time = toDo.Time
                });
            }

            return await Task.FromResult(response);
        }

        public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context) {
            if (request.Id <= 0 || request.UserId == string.Empty) {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must send a valid object"));
            }

            var toDoItem = await _dbContext.Items.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (toDoItem == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"No task with id:{request.Id} was found"));

            toDoItem.UserId = request.UserId;
            toDoItem.JackpotValue = request.JackpotValue;
            toDoItem.Time = request.Time;

            await _dbContext.SaveChangesAsync();
            return await Task.FromResult(new UpdateToDoResponse { 
                Id = request.Id
            });
        }

        public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context) {
            if (request.Id < 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "resource index must be greater than 0"));

            var toDoItem = await _dbContext.Items.FirstOrDefaultAsync(t => t.Id == request.Id);

            if (toDoItem == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"No task with id:{request.Id} was found"));

            _dbContext.Remove(toDoItem);
            await _dbContext.SaveChangesAsync();
            return await Task.FromResult(new DeleteToDoResponse
            {
                Id = request.Id
            });
        }
    }
}
