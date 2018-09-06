using Markdig;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScenariosExplorer.TagHelpers
{
    public class MarkdownTagHelper : TagHelper
    {
        public ModelExpression Content { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Content?.Model == null)
            {
                return;
            }

            output.TagMode = TagMode.SelfClosing;
            output.TagName = null;

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            output.Content.SetHtmlContent(Markdig.Markdown.ToHtml(Content.Model.ToString(), pipeline));
        }
    }
}
