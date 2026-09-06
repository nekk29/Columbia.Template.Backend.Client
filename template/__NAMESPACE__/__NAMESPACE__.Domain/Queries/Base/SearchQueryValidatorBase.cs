using FluentValidation;

namespace __NAMESPACE__.Domain.Queries.Base
{
    public class SearchQueryValidatorBase<TRequest, TFilter, TResponse> : QueryValidatorBase<TRequest>
        where TRequest : SearchQueryBase<TFilter, TResponse>
    {
        private readonly string[] SortDirenctions = ["asc", "desc"];

        public SearchQueryValidatorBase()
        {
            RequiredInformation(x => x.SearchParams, Resources.Common.SearchInformationRequired)
                .DependentRules(() =>
                {
                    RequiredInformation(x => x.SearchParams.Page, Resources.Common.SearchPageInformationRequired)
                        .DependentRules(() =>
                        {
                            RequiredField(x => x.SearchParams.Page!.Page, Resources.Common.PageField)
                                .DependentRules(() =>
                                {
                                    RuleFor(x => x.SearchParams.Page!.Page)
                                        .GreaterThan(0)
                                        .WithMessage(Resources.Common.PageFieldMinValue);
                                });

                            RequiredField(x => x.SearchParams.Page!.PageSize, Resources.Common.PageSizeField)
                                .DependentRules(() =>
                                {
                                    RequiredField(x => x.SearchParams.Page!.PageSize, Resources.Common.PageSizeField)
                                        .GreaterThan(0)
                                        .WithMessage(Resources.Common.PageSizeFieldMinValue)
                                        .LessThanOrEqualTo(1000)
                                        .WithMessage(Resources.Common.PageSizeFieldMaxValue);
                                });
                        });

                    RuleFor(x => x.SearchParams.Sort).Must((request, sort, context) =>
                    {
                        if (sort == null) return true;
                        if (!sort.Any()) return true;

                        var invalidSortDirs = sort.Where(x => !SortDirenctions.Contains(x.Direction));
                        if (invalidSortDirs.Any())
                            return CustomValidationMessage(context, Resources.Common.SortDirectionNotValid);

                        return true;
                    }).WithCustomValidationMessage();
                });
        }
    }
}
