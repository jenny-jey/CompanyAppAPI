using FluentValidation;

namespace CompanyAPI.Domain
{
    public class CompanyValidation : AbstractValidator<Company>
    {
        public CompanyValidation() 
        {
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.Ticker).NotEmpty();
            RuleFor(c => c.Exchange).NotEmpty();
            RuleFor(c => c.Isin)
                .NotEmpty()
                .Matches(@"^[A-Z]{2}[A-Z0-9]{10}$")
                .WithMessage("ISIN must start with 2 letters followed by 10 alphanumeric characters.");
        }
    }
}
