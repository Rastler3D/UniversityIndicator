using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Univer.Models;

[HtmlTargetElement("li", Attributes = "asp-action,asp-controller")]
public class SelectionTagHelper : TagHelper
{
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        string currentAction = ViewContext.RouteData.Values["action"] as string;
        string currentController = ViewContext.RouteData.Values["controller"] as string;
        string action = context.AllAttributes["asp-action"].Value.ToString();
        string controller = context.AllAttributes["asp-controller"].Value.ToString();
        if (currentAction == action && currentController == controller)
        {
            string cssClass = $"{context.AllAttributes["class"].Value} active";
            output.Attributes.SetAttribute("class", cssClass);
        }
        


    }
        
}

