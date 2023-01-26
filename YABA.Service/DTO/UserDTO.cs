using System;
using System.Collections.Generic;
using System.Text;
using YABA.Models;

namespace YABA.Service.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModified { get; set; }

        public UserDTO(User value)
        {
            Id = value.Id;
            IsDeleted = value.IsDeleted;
            CreatedOn = value.CreatedOn;
            LastModified = value.LastModified;
        }
    }
}
