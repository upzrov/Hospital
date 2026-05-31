using Microsoft.AspNetCore.Identity;

namespace BLL.Exceptions;

public class IdentityValidationException : Exception
{
    public IEnumerable<IdentityError> Errors;
    
    public IdentityValidationException(IEnumerable<IdentityError> errors) : base(errors.ToString())
    {
        Errors = errors;
    }
}