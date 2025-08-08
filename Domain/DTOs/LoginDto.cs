using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.DTOs
{
    public class LoginDto
    {
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
    }
}