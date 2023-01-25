using System;

namespace YABA.Models.Interfaces
{
    public interface IDateModifiedTrackable
    {
        public DateTimeOffset LastModified { get; set; }
    }
}
