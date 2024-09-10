using Hotel.ModelViews.AccountModelView;
using Microsoft.AspNetCore.Identity;

namespace Hotel.Contract.Services.IService
{
    public interface IAccountService
    {
        Task<IdentityResult> SignUp(SignUpModelView model);
        Task<string> SignIn(SignInModelView model);
    }
}
