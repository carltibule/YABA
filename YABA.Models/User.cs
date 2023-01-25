using System;
using System.ComponentModel.DataAnnotations;
using YABA.Models.Interfaces;

namespace YABA.Models
{
    public class User : IIdentifiable, ISoftDeletable, IDateCreatedTrackable, IDateModifiedTrackable
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModified { get; set; }

        [Required]
        public string Auth0Id { get; set; }
    }
}
