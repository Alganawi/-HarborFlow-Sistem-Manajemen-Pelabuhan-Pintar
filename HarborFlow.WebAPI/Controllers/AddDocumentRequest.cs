using System.ComponentModel.DataAnnotations;

namespace HarborFlow.WebAPI.Controllers
{
    public class AddDocumentRequest
    {
        [Required]
        public string DocumentName { get; set; } = string.Empty;
        [Required]
        public string FilePath { get; set; } = string.Empty;
    }
}
