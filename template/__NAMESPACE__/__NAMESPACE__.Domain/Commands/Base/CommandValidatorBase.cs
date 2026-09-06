using System.Linq.Expressions;
using FluentValidation;

namespace __NAMESPACE__.Domain.Commands.Base
{
    public class CommandValidatorBase<TRequest> : AbstractValidator<TRequest>
    {
        public bool Enabled { get; private set; } = true;
        public void Enable() => Enabled = true;
        public void Disable() => Enabled = false;

        public bool CustomValidationMessage(ValidationContext<TRequest> context, string message)
        {
            context.MessageFormatter.AppendArgument(Resources.Common.CustomValidationMessageArgument, message);
            return false;
        }

        protected IRuleBuilderOptions<TRequest, TProperty> RequiredInformation<TProperty>(Expression<Func<TRequest, TProperty>> expression, string? message = null)
            => RuleFor(expression).NotEmpty().WithMessage(message ?? Resources.Common.InformationRequired);

        protected IRuleBuilderOptions<TRequest, TProperty> RequiredField<TProperty>(Expression<Func<TRequest, TProperty>> expression, string field)
            => RuleFor(expression).NotEmpty().WithMessage(string.Format(Resources.Common.FieldRequired, field));

        protected IRuleBuilderOptions<TRequest, string?> RequiredField(Expression<Func<TRequest, string?>> expression, string field)
            => RuleFor(expression).NotEmpty().WithMessage(string.Format(Resources.Common.FieldRequired, field));

        protected IRuleBuilderOptions<TRequest, string?> Length(Expression<Func<TRequest, string?>> expression, string field, int length)
            => RuleFor(expression).Length(length).WithMessage(string.Format(Resources.Common.FieldLength, field, length));

        protected IRuleBuilderOptions<TRequest, string?> MinimumLength(Expression<Func<TRequest, string?>> expression, string field, int minimumLength)
            => RuleFor(expression).MinimumLength(minimumLength).WithMessage(string.Format(Resources.Common.FieldMinLength, field, minimumLength));

        protected IRuleBuilderOptions<TRequest, string?> MaximumLength(Expression<Func<TRequest, string?>> expression, string field, int maximumLength)
            => RuleFor(expression).MaximumLength(maximumLength).WithMessage(string.Format(Resources.Common.FieldMaxLength, field, maximumLength));

        protected IRuleBuilderOptions<TRequest, string?> RequiredString(Expression<Func<TRequest, string?>> expression, string field, int? minimumLength = null, int? maximumLength = null)
        {
            var ruleBuilderOptions = RequiredField(expression, field);

            if (minimumLength.HasValue || maximumLength.HasValue)
            {
                ruleBuilderOptions = ruleBuilderOptions.DependentRules(() =>
                {
                    if (minimumLength.HasValue && minimumLength != default)
                        ruleBuilderOptions = MinimumLength(expression, field, minimumLength.Value);

                    if (maximumLength.HasValue && maximumLength != default)
                        ruleBuilderOptions = MaximumLength(expression, field, maximumLength.Value);
                });
            }

            return ruleBuilderOptions;
        }

        protected IRuleBuilderOptions<TRequest, string?> RequiredString(Expression<Func<TRequest, string?>> expression, string field, int? length = null)
        {
            var ruleBuilderOptions = RequiredField(expression, field);

            if (length.HasValue)
            {
                ruleBuilderOptions = ruleBuilderOptions.DependentRules(() =>
                {
                    if (length.HasValue && length != default)
                        ruleBuilderOptions = Length(expression, field, length.Value);
                });
            }

            return ruleBuilderOptions;
        }

        protected IRuleBuilderOptions<TRequest, string?> ValidMail(Expression<Func<TRequest, string?>> expression, string field)
        {
            return RuleFor(expression)
                .EmailAddress()
                .WithMessage(string.Format(Resources.Common.EmailInvalid, field));
        }
    }

    public static class BaseCommandValidatorExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> WithCustomValidationMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule)
            => rule.WithMessage(Resources.Common.CustomValidationMessage);
    }
}
