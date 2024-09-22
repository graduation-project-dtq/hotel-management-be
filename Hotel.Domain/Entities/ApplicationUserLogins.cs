
using Hotel.Core.Common;
using Microsoft.AspNetCore.Identity;

namespace Hotel.Domain.Entities;

public class ApplicationUserLogins : IdentityUserLogin<Guid>
{
    public DateTimeOffset CreatedTime { get; set; }

    public DateTimeOffset LastUpdatedTime { get; set; }

    public DateTimeOffset? DeletedTime { get; set; }
    public ApplicationUserLogins()
    {
        CreatedTime = CoreHelper.SystemTimeNow;
        LastUpdatedTime = CreatedTime;
    }
}