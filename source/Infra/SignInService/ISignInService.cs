using DotNetCoreArchitecture.Model;

namespace DotNetCoreArchitecture.Infra
{
    public interface ISignInService
    {
        SignInModel CreateSignIn(SignInModel signInModel);

        TokenModel CreateToken(SignedInModel signedInModel);

        bool Validate(SignedInModel signedInModel, SignInModel signInModel);
    }
}
