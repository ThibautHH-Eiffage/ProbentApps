using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace ProbentApps.Components;

public class PostForm : ComponentBase
{
    [Parameter]
    public string? FormName { get; set; }

    [Parameter]
    public EventCallback OnSubmit { get; set; }

    [Parameter]
    public string? Action { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder __builder)
    {
        __builder.OpenElement(0, "form");
        __builder.AddAttribute(1, "method", "post");
        __builder.AddAttribute(2, "class", Class);
        if (Action is string action)
        {
            __builder.AddAttribute(3, nameof(action), action);
        }
        else if (OnSubmit.HasDelegate)
        {
            __builder.AddAttribute(3, "onsubmit", OnSubmit);
        }
        __builder.AddNamedEvent("onsubmit", FormName ?? Action ?? nameof(PostForm));
        __builder.OpenComponent<AntiforgeryToken>(4);
        __builder.CloseComponent();
        if (ChildContent is not null)
        {
            __builder.AddContent(5, ChildContent);
        }
        __builder.CloseElement();
    }
}
