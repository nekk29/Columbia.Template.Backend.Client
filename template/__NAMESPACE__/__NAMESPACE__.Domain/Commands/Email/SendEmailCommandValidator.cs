using System.ComponentModel.DataAnnotations;
using FluentValidation;
using __NAMESPACE__.Domain.Commands.Base;
using __NAMESPACE__.Repository.Abstractions.Base;
using Microsoft.Extensions.Configuration;

namespace __NAMESPACE__.Domain.Commands.Email
{
    public class SendEmailCommandValidator : CommandValidatorBase<SendEmailCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<Entity.Email> _emailRepository;

        public SendEmailCommandValidator(
            IConfiguration configuration,
            IRepository<Entity.Email> emailRepository
        )
        {
            _configuration = configuration;
            _emailRepository = emailRepository;

            RequiredInformation(x => x.EmailDto)
                .DependentRules(() =>
                {
                    RuleFor(x => x.EmailDto.EmailCode)
                        .MustAsync(ValidateEmailAsync)
                        .WithCustomValidationMessage();
                })
                .DependentRules(() =>
                {
                    RuleFor(x => x.EmailDto.ToEmails)
                        .Must(ValidateToEmailsFormat)
                        .WithCustomValidationMessage();

                    RuleFor(x => x.EmailDto.CcEmails)
                        .Must(ValidateCcEmailsFormat)
                        .WithCustomValidationMessage();
                });
        }

        protected async Task<bool> ValidateEmailAsync(SendEmailCommand command, string emailCode, ValidationContext<SendEmailCommand> context, CancellationToken cancellationToken)
        {
            var language =
                _configuration.GetValue<string>("AppSettings:DefaultCulture") ??
                System.Globalization.CultureInfo.CurrentCulture.Name;

            var email = await _emailRepository.GetByAsNoTrackingAsync(x => x.Code == emailCode && x.Language == language);

            email ??= await _emailRepository.GetByAsNoTrackingAsync(x => x.Code == emailCode);

            if (email == null)
                return CustomValidationMessage(context, Resources.Email.EmailDoesNotExist);

            if (!string.IsNullOrEmpty(email.ToEmails))
            {
                var emails = email.ToEmails.Split(";");
                var validEmails = ValidateEmails(emails);
                if (!validEmails) return CustomValidationMessage(context, Resources.Email.ToEmailsNotValid);
            }

            if (!string.IsNullOrEmpty(email.CcEmails))
            {
                var emails = email.CcEmails.Split(";");
                var validEmails = ValidateEmails(emails);
                if (!validEmails) return CustomValidationMessage(context, Resources.Email.CcEmailsNotValid);
            }

            return true;
        }

        protected bool ValidateToEmailsFormat(SendEmailCommand command, IEnumerable<string>? emails, ValidationContext<SendEmailCommand> context)
            => ValidateEmailsFormat(command, emails, Resources.Email.ToEmailsNotValid, context);

        protected bool ValidateCcEmailsFormat(SendEmailCommand command, IEnumerable<string>? emails, ValidationContext<SendEmailCommand> context)
            => ValidateEmailsFormat(command, emails, Resources.Email.CcEmailsNotValid, context);

        protected bool ValidateEmailsFormat(SendEmailCommand _, IEnumerable<string>? emails, string message, ValidationContext<SendEmailCommand> context)
        {
            var validEmails = ValidateEmails(emails);
            if (!validEmails) return CustomValidationMessage(context, message);
            return true;
        }

        private static bool ValidateEmails(IEnumerable<string>? emails)
        {
            if (emails == null) return true;

            foreach (var email in emails)
            {
                var emailValidator = new EmailAddressAttribute();
                if (!emailValidator.IsValid(email)) return false;
            }

            return true;
        }
    }
}
