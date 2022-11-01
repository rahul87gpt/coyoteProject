namespace Coyote.Console.Common
{
    public static class EmailTemplateNameConstants
    {
        public const string NewUserCreation = "New User Creation";
        public const string ForgotPassword = "Forgot Password";

        public const string UserFirstNameMergeField = "%UserFirstName%";
        public const string UserLastNameMergeField = "%UserLastName%";
        public const string UserNameMergeField = "%Username%";
        public const string PasswordMergeField = "%Password%";
        public const string TemporaryPasswordURLMergeField = "%TemporaryPasswordURL%";

    }
}
