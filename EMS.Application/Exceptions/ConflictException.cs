using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.Exceptions
{
    public class ConflictException : Exception                      // Custom exception class inheriting from .NET’s built-in Exception to represent a conflict error, typically used when a resource already exists.
    {
        public ConflictException(string message) : base(message)    // Constructor that takes a message parameter and passes it to the base Exception class constructor, allowing for a custom error message to be provided when the exception is thrown.
        {
        }                                                           // Body is empty because the parent already does all the work, and we only wanted a new name.
    }
}
// NOTE : Why ConflictException? InvalidOperationException is thrown by .NET for many unrelated reasons, so using it for 409 (Already Exists) would mislabel real bugs. A dedicated type lets the global handler be precise.