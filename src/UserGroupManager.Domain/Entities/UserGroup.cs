using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserGroupManager.Domain.Entities
{
    public enum RequestStatus { Pending, Approved }
    public class UserGroup
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;

        public RequestStatus Status { get; set; } = RequestStatus.Pending;

    }
}
