namespace __NAMESPACE__.Common
{
    public class Constants
    {
        public struct Culture
        {
            public const string enUSCulture = "en-US";
            public const string esESCulture = "es-ES";
        }

        public struct Security
        {
            public struct User
            {
                public const string Administrator = "administrator";
            }

            public struct ClaimTypes
            {
                public const string Role = "role";
            }
        }

        public struct Email
        {
            public struct User
            {
                public const string Registration = "User.Registration";
                public const string ResetPassword = "User.ResetPassword";
            }
        }

        public struct Settings
        {
            public struct Group
            {

            }

#pragma warning disable CA2211 // Non-constant fields should not be visible
            public static IEnumerable<Encrypted> EncryptedSettings = new List<Encrypted>
            {

            };
#pragma warning restore CA2211 // Non-constant fields should not be visible

            public class Encrypted
            {
                public string Group { get; set; } = null!;
                public string Code { get; set; } = null!;
            }
        }

        public struct Cache
        {

        }
    }
}
