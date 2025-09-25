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
        [Authorize(Policy = "CanManageGroups")]
        public async Task<ActionResult<IEnumerable<GroupDTO>>> GetAllGroups()
        {
            return Ok(await _context.Groups
            .Select(g => new GroupDTO { Id = g.Id, Name = g.Name })
            .ToListAsync());
        }

        [HttpPost("groups")]
        [Authorize(Policy = "CanManageGroups")]
        public async Task<IActionResult> CreateGroup([FromBody] UpdateGroupDTO createGroupDto)
        {
            if (string.IsNullOrWhiteSpace(createGroupDto.Name))
                return BadRequest("Group name cannot be empty.");

            var newGroup = new Group { Name = createGroupDto.Name };
            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync();

            return Ok(new GroupDTO
            {
                Id = newGroup.Id, Name = newGroup.Name 
            });
        }

        [HttpGet("groups/{id}")]
        [Authorize(Policy = "CanManageGroups")]
        public async Task<ActionResult<GroupDTO>> GetGroup(int id)
        {
            var group = await _context.Groups
                .Where(g => g.Id == id)
                .Select(g => new GroupDTO { Id = g.Id, Name = g.Name })
                .FirstOrDefaultAsync();

            if (group == null)
                return NotFound();

            return Ok(group);
        }

        [HttpPut("groups/{id}")]
        [Authorize(Policy = "CanManageGroups")]
        public async Task<IActionResult> UpdateGroup(int id, [FromBody] UpdateGroupDTO updateGroupDto)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
                return NotFound("Group not found.");

            if (string.IsNullOrWhiteSpace(updateGroupDto.Name))
                return BadRequest("Group name cannot be empty.");

            group.Name = updateGroupDto.Name;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("groups/{id}")]
        [Authorize(Policy = "CanManageGroups")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
                return NotFound("Group not found.");

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    Status = u.Status.ToString()
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///Group Membership Management Endpoints


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

            var currentUserGroups = await _context.UserGroups
                .Where(ug => ug.UserId == id)
                .ToListAsync(); 

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



         ///////////////////////////////////////////////////////////////////////////////

        //Permissions Management Endpoints
        // GET: /api/admin/permissions - Get a list of all possible permissions
        [HttpGet("permissions")]
        [Authorize(Policy = "CanManageGroups")]
        public async Task<IActionResult> GetAllPermissions()
        {
            return Ok(await _context.Permissions
                .Select(p => new { p.Id, p.Name })
                .ToListAsync());
        }

        
        [HttpGet("groups/{id}/permissions")]
        [Authorize(Policy = "CanManageGroups")]
        public async Task<IActionResult> GetGroupPermissions(int id)
        {
            var groupPermissions = await _context.Groups
                .Where(g => g.Id == id)
                .SelectMany(g => g.Permissions)
                .Select(p => p.Id)
                .ToListAsync();

            return Ok(groupPermissions);
        }

        [HttpPut("groups/{id}/permissions")]
        [Authorize(Policy = "CanManageGroups")]
        public async Task<IActionResult> UpdateGroupPermissions(int id, [FromBody] List<int> permissionIds)
        {
            var group = await _context.Groups
                .Include(g => g.Permissions)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
                return NotFound("Group not found.");

            var permissionsToSet = await _context.Permissions
                .Where(p => permissionIds.Contains(p.Id))
                .ToListAsync();

            group.Permissions = permissionsToSet;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Group permissions updated successfully." });
        }
    }
}
