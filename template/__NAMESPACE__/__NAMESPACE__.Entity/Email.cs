using __NAMESPACE__.Entity.Base;

namespace __NAMESPACE__.Entity
{
    public class Email : SystemEntity
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Language { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string? ToEmails { get; set; }
        public string? CcEmails { get; set; }
        public string Body { get; set; } = null!;
    }
}
