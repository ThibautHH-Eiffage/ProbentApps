using MudBlazor;

namespace ProbentApps.Components;

public class FilterOperators
{
    public static readonly HashSet<string> KeyColumn =
        [
            FilterOperator.String.Contains,
            FilterOperator.String.Equal,
            FilterOperator.String.StartsWith,
            FilterOperator.String.EndsWith,
            FilterOperator.String.NotContains
        ];

    public static readonly HashSet<string> NameColumn =
        [
            FilterOperator.String.Contains,
            FilterOperator.String.StartsWith,
            FilterOperator.String.NotContains
        ];
}
