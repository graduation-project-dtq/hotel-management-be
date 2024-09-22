using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Infrastructure.Data
{
    public class HotelDBContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, ApplicationUserClaims, ApplicationUserRoles, ApplicationUserLogins, ApplicationRoleClaims, ApplicationUserTokens>
    {
        public HotelDBContext(DbContextOptions<HotelDBContext> options) : base(options)
        {

        }

        // user
        #region USER    
        public virtual DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
        public virtual DbSet<ApplicationRole> ApplicationRoles => Set<ApplicationRole>();
        public virtual DbSet<ApplicationUserClaims> ApplicationUserClaims => Set<ApplicationUserClaims>();
        public virtual DbSet<ApplicationUserRoles> ApplicationUserRoles => Set<ApplicationUserRoles>();
        public virtual DbSet<ApplicationUserLogins> ApplicationUserLogins => Set<ApplicationUserLogins>();
        public virtual DbSet<ApplicationRoleClaims> ApplicationRoleClaims => Set<ApplicationRoleClaims>();
        public virtual DbSet<ApplicationUserTokens> ApplicationUserTokens => Set<ApplicationUserTokens>();

        #endregion
        #region DATA   
         
        #endregion
    }
}
