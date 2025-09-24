using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserGroupManager.Api.DataTranferObjects;
using UserGroupManager.Domain.Entities;
using UserGroupManager.Infrastructure.Data;

namespace UserGroupManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StatsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public StatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("userCount")]
        public async Task<ActionResult<TotalUserCountDTO>> GetTotalUserCount()
        {
            var userCount = await _context.Users.CountAsync();

            return Ok(new TotalUserCountDTO 
            { 
                Count = userCount 
            });
        }

        [HttpGet("usersPerGroup")]
        public async Task<ActionResult<IEnumerable<GroupUserCountDTO>>> GetUsersPerGroupCount()
        {
            var stats = await _context.Groups
                .Select(g => new GroupUserCountDTO
                {
                    GroupName = g.Name,                    
                    UserCount = _context.UserGroups.Count(ug => ug.GroupId == g.Id && ug.Status == RequestStatus.Approved)
                })
                .ToListAsync();

            return Ok(stats);
        }


    }
}
