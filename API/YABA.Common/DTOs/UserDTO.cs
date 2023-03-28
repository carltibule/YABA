using System;

namespace YABA.Common.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModified { get; set; }
    }
}
