namespace DashboardCompanion.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TryPostData
    {
        [RegularExpression(@"^[a-z]\w*$", ErrorMessage = "The account name is invalid.")]
        [Required(ErrorMessage = "The account domain is required.")]
        public string Account { get; set; }

        [Required(ErrorMessage = "The generated token is required.")]
        public string ApiToken { get; set; }

        public string Domain => $"{this.Account}.auth0.com";
    }
}