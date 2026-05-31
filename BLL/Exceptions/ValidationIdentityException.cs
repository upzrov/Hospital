using Microsoft.AspNetCore.Identity;

namespace BLL.Exceptions;

public class ValidationIdentityException : Exception
{
    public IEnumerable<IdentityError> Errors;
    
    public ValidationIdentityException(IEnumerable<IdentityError> errors) : base(errors.ToString())
    {
        Errors = errors;
    }
}