using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using TaskManagementMvc.Models;

namespace TaskManagementMvc.TagHelpers
{
    /// <summary>
    /// Tag helper for Microsoft Clarity analytics integration
    /// </summary>
    [HtmlTargetElement("clarity-analytics")]
    public class ClarityAnalyticsTagHelper : TagHelper
    {
        private readonly AnalyticsOptions _analyticsOptions;

        public ClarityAnalyticsTagHelper(IOptions<AnalyticsOptions> analyticsOptions)
        {
            _analyticsOptions = analyticsOptions.Value;
        }

        /// <summary>
        /// Optional override for project ID
        /// </summary>
        public string? ProjectId { get; set; }

        /// <summary>
        /// Optional override for enabled status
        /// </summary>
        public bool? Enabled { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var projectId = ProjectId ?? _analyticsOptions.MicrosoftClarity.ProjectId;
            var enabled = Enabled ?? _analyticsOptions.MicrosoftClarity.Enabled;

            output.TagName = null; // Remove the clarity-analytics tag

            if (!enabled || string.IsNullOrEmpty(projectId))
            {
                output.SuppressOutput();
                return;
            }

            output.Content.SetHtmlContent($@"
<script type=""text/javascript"">
    (function(c,l,a,r,i,t,y){{
        c[a]=c[a]||function(){{(c[a].q=c[a].q||[]).push(arguments)}};
        t=l.createElement(r);t.async=1;t.src=""https://www.clarity.ms/tag/""+i;
        y=l.getElementsByTagName(r)[0];y.parentNode.insertBefore(t,y);
    }})(window, document, ""clarity"", ""script"", ""{projectId}"");
</script>");
        }
    }
}
