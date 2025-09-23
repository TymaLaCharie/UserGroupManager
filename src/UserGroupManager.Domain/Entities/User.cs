using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserGroupManager.Domain.Entities
{
    public class User
    {
       public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime AccountCreateStamp { get; set; }
        public DateTime AccountUpdated { get; set; }

        public ICollection<Group> Groups { get; set; }= new List<Group>();
    }
}
