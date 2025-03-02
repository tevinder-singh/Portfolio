using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlavourVault.SharedCore.Interfaces;

public interface IUserContext
{
    Guid? GetUserId();
    string? GetUserName();
}
