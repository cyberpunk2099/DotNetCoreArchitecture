using DotNetCore.Objects;
using System.Collections.Generic;

namespace DotNetCoreArchitecture.Domain
{
    public sealed class SignIn : ValueObject
    {
        public SignIn(string login, string password, string salt)
        {
            Login = login;
            Password = password;
            Salt = salt;
        }

        public string Login { get; }

        public string Password { get; }

        public string Salt { get; }

        protected override IEnumerable<object> GetEquals()
        {
            yield return Login;
            yield return Password;
            yield return Salt;
        }
    }
}
