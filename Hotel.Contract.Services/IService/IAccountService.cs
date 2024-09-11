using Hotel.ModelViews.AccountModelView;
using Microsoft.AspNetCore.Identity;

namespace Hotel.Contract.Services.IService
{
    public interface IAccountService
    {
        Task<IdentityResult> SignUpAsync(SignUpModelView model);
        Task<string> SignInAsync(SignInModelView model);
    }
}
