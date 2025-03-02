using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlavourVault.SharedCore.Extensions;
public static class GuidExtensions
{
    public static bool IsEmpty(this Guid val)
    {
        return val == Guid.Empty;
    }

    public static string GetStringValueOrEmpty(this Guid? val)
    {
        return val.HasValue ? val.Value.ToString() : string.Empty;
    }

}
