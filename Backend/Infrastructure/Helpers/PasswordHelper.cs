using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Helpers
{
    public static class PasswordHelper
    {
        private static readonly PasswordHasher<object> _hasher = new PasswordHasher<object>();

        private static readonly object _dummyUser = new object();

        public static string Hash(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            return _hasher.HashPassword(_dummyUser, password);
        }

        public static bool Verify(string hashedPassword, string providedPassword)
        {
            var result = _hasher.VerifyHashedPassword(_dummyUser, hashedPassword, providedPassword);

            switch (result)
            {
                case PasswordVerificationResult.Failed:
                    return false;

                case PasswordVerificationResult.Success:
                    return true;

                case PasswordVerificationResult.SuccessRehashNeeded:
                    return true;

                default:
                    return false;
            }
        }
    }
}
