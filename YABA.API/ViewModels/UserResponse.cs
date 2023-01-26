using YABA.Service.DTO;

namespace YABA.API.ViewModels
{
    public class UserResponse
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModified { get; set; }

        public UserResponse(UserDTO value)
        {
            Id = value.Id;
            IsDeleted = value.IsDeleted;
            CreatedOn = value.CreatedOn;
            LastModified = value.LastModified;
        }
    }
}
