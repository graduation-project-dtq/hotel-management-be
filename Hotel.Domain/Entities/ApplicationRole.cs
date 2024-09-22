
using Hotel.Core.Common;
using Microsoft.AspNetCore.Identity;

namespace Hotel.Domain.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    public DateTimeOffset CreatedTime { get; set; }

    public DateTimeOffset LastUpdatedTime { get; set; }

    public DateTimeOffset? DeletedTime { get; set; }
    public ApplicationRole()
    {
        CreatedTime = CoreHelper.SystemTimeNow;
        LastUpdatedTime = CreatedTime;
    }
}