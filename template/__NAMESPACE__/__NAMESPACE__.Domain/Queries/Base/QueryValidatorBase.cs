using System.Linq.Expressions;
using FluentValidation;

namespace __NAMESPACE__.Domain.Queries.Base
{
    public class QueryValidatorBase<TRequest> : AbstractValidator<TRequest>
    {
        public bool CustomValidationMessage(ValidationContext<TRequest> context, string message)
        {
            context.MessageFormatter.AppendArgument(Resources.Common.CustomValidationMessageArgument, message);
            return false;
        }

        protected IRuleBuilderOptions<TRequest, TProperty> RequiredInformation<TProperty>(Expression<Func<TRequest, TProperty>> expression, string? message = null)
            => RuleFor(expression).NotEmpty().WithMessage(message ?? Resources.Common.InformationRequired);

        protected IRuleBuilderOptions<TRequest, TProperty> RequiredField<TProperty>(Expression<Func<TRequest, TProperty>> expression, string field)
            => RuleFor(expression).NotEmpty().WithMessage(string.Format(Resources.Common.FieldRequired, field));
    }

    public static class BaseQueryValidatorExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> WithCustomValidationMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule)
            => rule.WithMessage(Resources.Common.CustomValidationMessage);
    }
}
