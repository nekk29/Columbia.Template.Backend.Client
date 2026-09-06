using AutoMapper;
using __NAMESPACE__.Domain.Commands.Base;
using __NAMESPACE__.Dto.Base;
using __NAMESPACE__.EmailClient;
using __NAMESPACE__.Repository.Abstractions.Base;
using __NAMESPACE__.Repository.Abstractions.Transactions;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace __NAMESPACE__.Domain.Commands.Email
{
    public class SendEmailCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IMediator mediator,
        SendEmailCommandValidator validator,
        IEmailClient emailClient,
        IConfiguration configuration,
        IRepository<Entity.Email> emailRepository
    ) : CommandHandlerBase<SendEmailCommand>(unitOfWork, mapper, mediator, validator)
    {
        public override async Task<ResponseDto> HandleCommand(SendEmailCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            var language =
                configuration.GetValue<string>("AppSettings:DefaultCulture") ??
                System.Globalization.CultureInfo.CurrentCulture.Name;

            var email = await emailRepository.GetByAsNoTrackingAsync(
                x => x.Code == request.EmailDto.EmailCode && x.Language == language
            );

            email ??= await emailRepository.GetByAsNoTrackingAsync(
                x => x.Code == request.EmailDto.EmailCode
            );

            if (email != null)
            {
                var toEmails = new List<string>();
                var ccEmails = new List<string>();

                if (request.EmailDto.ToEmails != null)
                    toEmails.AddRange(request.EmailDto.ToEmails);

                if (!string.IsNullOrEmpty(email.ToEmails))
                    toEmails.AddRange(email.ToEmails.Split(";"));

                if (request.EmailDto.CcEmails != null)
                    toEmails.AddRange(request.EmailDto.CcEmails);

                if (!string.IsNullOrEmpty(email.CcEmails))
                    ccEmails.AddRange(email.CcEmails.Split(";"));

                var subject = ReplaceParams(email.Subject ?? "", request.EmailDto.SubjectParams);
                var emailBody = ReplaceParams(email.Body ?? "", request.EmailDto.BodyParams);

                await emailClient.SendEmailAsync(
                    toEmails,
                    ccEmails,
                    null!,
                    null!,
                    subject,
                    emailBody,
                    true
                );
            }

            var successMessage = request.EmailDto.SuccesMessage;
            if (string.IsNullOrEmpty(successMessage))
                successMessage = Resources.Email.SendMailSuccess;

            response.AddOkResult(successMessage);

            return response;
        }

        private static string ReplaceParams(string text, Dictionary<string, string>? textParams)
        {
            string replaced = text;

            if (textParams == null) return replaced;

            foreach (var textParam in textParams)
                replaced = replaced.Replace(textParam.Key, textParam.Value);

            return replaced;
        }
    }
}
