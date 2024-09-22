
using Hotel.Core.Common;
using Microsoft.AspNetCore.Identity;

namespace Hotel.Domain.Entities;

public class ApplicationUserTokens : IdentityUserToken<Guid>
{
    public DateTimeOffset CreatedTime { get; set; }

    public DateTimeOffset LastUpdatedTime { get; set; }

    public DateTimeOffset? DeletedTime { get; set; }
    public ApplicationUserTokens()
    {
        CreatedTime = CoreHelper.SystemTimeNow;
        LastUpdatedTime = CreatedTime;
    }
}