using System.ComponentModel.DataAnnotations;

namespace YABA.Common.Lookups
{
    public enum CrudResultLookup 
    {
        [Display(Name = "Insert failed")]
        CreateFailed = 1,

        [Display(Name = "Insert succeeded")]
        CreateSucceeded = 2,

        [Display(Name = "Insert failed. Entry already exists")]
        CreateFailedEntryExists = 3,

        [Display(Name = "Update failed")]
        UpdateFailed = 4,

        [Display(Name = "Update succeeded")]
        UpdateSucceeded = 5,

        [Display(Name = "Delete failed")]
        DeleteFailed = 6,

        [Display(Name = "Delete succeeded")]
        DeleteSucceeded = 7,

        [Display(Name = "Retrieve failed")]
        RetrieveFailed = 8,

        [Display(Name = "Retrieve successful")]
        RetrieveSuccessful = 9
    }
}
