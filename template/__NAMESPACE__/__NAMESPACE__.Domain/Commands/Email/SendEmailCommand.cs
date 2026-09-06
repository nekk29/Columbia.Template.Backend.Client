using __NAMESPACE__.Domain.Commands.Base;
using __NAMESPACE__.Dto.Email;

namespace __NAMESPACE__.Domain.Commands.Email
{
    public class SendEmailCommand(SendEmailDto emailDto) : CommandBase
    {
        public SendEmailDto EmailDto { get; set; } = emailDto;
    }
}
