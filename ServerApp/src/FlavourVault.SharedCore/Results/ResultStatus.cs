using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlavourVault.SharedCore.Results;
public enum ResultStatus
{
    Ok,
    Created,
    Error,
    Forbidden,
    Unauthorized,    
    NotFound,
    Invalid
}