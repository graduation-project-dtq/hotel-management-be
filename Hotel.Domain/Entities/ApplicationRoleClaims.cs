
using Hotel.Core.Common;
using Microsoft.AspNetCore.Identity;

namespace Hotel.Domain.Entities;

public class ApplicationRoleClaims : IdentityRoleClaim<Guid>
{
    public DateTimeOffset CreatedTime { get; set; }

    public DateTimeOffset LastUpdatedTime { get; set; }

    public DateTimeOffset? DeletedTime { get; set; }
    public ApplicationRoleClaims()
    {
        CreatedTime = CoreHelper.SystemTimeNow;
        LastUpdatedTime = CreatedTime;
    }
}