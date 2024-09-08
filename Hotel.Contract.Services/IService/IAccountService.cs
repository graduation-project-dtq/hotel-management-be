using Hotel.ModelViews.AccountModelView;
using Microsoft.AspNetCore.Identity;

namespace Hotel.Contract.Services.IService
{
    public interface IAccountService
    {
        Task<string> SignInAsync(SignInViewModel signInViewModel);
        Task<IdentityResult> SignUpAsync(SignUpViewModel signUpViewModel);
       
    }
}
