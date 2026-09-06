namespace __NAMESPACE__.Dto.Email
{
    public class SendEmailDto
    {
        public string EmailCode { get; set; } = null!;
        public IEnumerable<string>? ToEmails { get; set; } = null!;
        public IEnumerable<string>? CcEmails { get; set; } = null!;
        public Dictionary<string, string>? SubjectParams { get; set; } = null!;
        public Dictionary<string, string>? BodyParams { get; set; } = null!;
        public string SuccesMessage { get; set; } = null!;
    }
}
