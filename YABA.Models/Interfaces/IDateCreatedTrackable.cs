using System;

namespace YABA.Models.Interfaces
{
    public interface IDateCreatedTrackable
    {
        public DateTimeOffset CreatedOn { get; set; }
    }
}
