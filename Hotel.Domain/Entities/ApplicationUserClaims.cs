
using Hotel.Core.Common;
using Microsoft.AspNetCore.Identity;

namespace Hotel.Domain.Entities;

public class ApplicationUserClaims : IdentityUserClaim<Guid>
{
    public DateTimeOffset CreatedTime { get; set; }

    public DateTimeOffset LastUpdatedTime { get; set; }

    public DateTimeOffset? DeletedTime { get; set; }
    public ApplicationUserClaims()
    {
        CreatedTime = CoreHelper.SystemTimeNow;
        LastUpdatedTime = CreatedTime;
    }
}