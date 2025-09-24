using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserGroupManager.Domain.Entities;
using UserGroupManager.Infrastructure.Data;

namespace UserGroupManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "CanManageUsers")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("fetchPending")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var users = await _context.Users
                .Where(user => user.Status ==UserStatus.Pending)
                .Select(user => new { user.Id, user.FirstName, user.LastName,user.Email})
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("approveUser/{id}")]
        public async Task<IActionResult> ApproveUser(int id)
        {
            var user =await _context.Users.FindAsync(id);

            if (user == null) 
            { 
                return NotFound("User not found");
            }

            user.Status = UserStatus.Active;
            user.AccountUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new 
            { 
                message = "User approved successfully." 
            }
            );
        }

        [HttpPost("rejectUser/{id}")]
        public async Task<IActionResult> RejectUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            user.Status = UserStatus.Decline;
            user.AccountUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User Access Declined.."
            }
            );
        }

        [HttpGet("groups")]
        public async Task<IActionResult> GetAllGroups()
        {
            return Ok(await _context.Groups.ToListAsync());
        }

        [HttpPost("groups")]
        public async Task<IActionResult> CreateGroup([FromBody] string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return BadRequest("Group name cannot be empty.");
            }

            var newGroup = new Group 
            {
                Name = groupName 
            };

            _context.Groups.Add(newGroup);

            await _context.SaveChangesAsync();

            return Ok(newGroup);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new { u.Id, u.FirstName, u.LastName, u.Email,u.Status })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("users/{id}/groups")]
        public async Task<IActionResult> GetUserGroups(int id)
        {
            var userGroups = await _context.UserGroups
                .Where(ug => ug.UserId == id)
                .Select(ug => ug.GroupId)
                .ToListAsync();

            return Ok(userGroups);
        }

        [HttpPut("users/{id}/groups")]
        public async Task<IActionResult> UpdateUserGroups(int id, [FromBody] List<int> groupIds)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Find the user in current group memberships
            var currentUserGroups = _context.UserGroups.Where(ug => ug.UserId == id);

            // Remove the old memberships
            _context.UserGroups.RemoveRange(currentUserGroups);

            // Add the new memberships
            var newUserGroups = groupIds.Select(groupId => new UserGroup
            {
                UserId = id,
                GroupId = groupId,
                Status = RequestStatus.Approved // Admin changes are auto-approved
            });

            await _context.UserGroups.AddRangeAsync(newUserGroups);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User groups updated successfully." });
        }
    }
}
