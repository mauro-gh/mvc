using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Ganss.Xss;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace mvc.Customizations.TagHelpers
{

    // verrà usato dal html se incontra un <p html-sanitize>@Model.Description</p>

    [HtmlTargetElement(Attributes = "html-sanitize")]
    public class HtmlSanitizeTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            //Otteniamo il contenuto del tag
            TagHelperContent tagHelperContent = await output.GetChildContentAsync(NullHtmlEncoder.Default);
            string content = tagHelperContent.GetContent(NullHtmlEncoder.Default);

            // Sanitizzazione custom (solo alcuni tag, ecc)
            var sanitizer = CreateSanitizer();
            content = sanitizer.Sanitize(content);

            //Reimpostiamo il contenuto del tag
            output.Content.SetHtmlContent(content);
        }

        private static HtmlSanitizer CreateSanitizer()
        {
            var sanitizer = new HtmlSanitizer();

            //Tag consentiti
            //sanitizer.AllowedTags.Clear();
            // sanitizer.AllowedTags.Add("b");
            // sanitizer.AllowedTags.Add("i");
            // sanitizer.AllowedTags.Add("p");
            // sanitizer.AllowedTags.Add("br");
            // sanitizer.AllowedTags.Add("ul");
            // sanitizer.AllowedTags.Add("ol");
            // sanitizer.AllowedTags.Add("li");
            // sanitizer.AllowedTags.Add("iframe");
            // sanitizer.AllowedTags.Add("strong");
            // sanitizer.AllowedTags.Add("span");
            // sanitizer.AllowedTags.Add("s");
            // sanitizer.AllowedTags.Add("img");

            //Attributi consentiti
            //sanitizer.AllowedAttributes.Clear();
            //sanitizer.AllowedAttributes.Add("src");
            //sanitizer.AllowDataAttributes = true;


            //Stili consentiti
            //sanitizer.AllowedCssProperties.Clear();
            //sanitizer.AllowedCssProperties.Add("background-color");


            //sanitizer.FilterUrl += FilterUrl;
            sanitizer.PostProcessNode += ProcessIFrames;

            return sanitizer;
        }
        
        private static void FilterUrl(object sender, FilterUrlEventArgs filterUrlEventArgs)
        {
            if (!filterUrlEventArgs.OriginalUrl.StartsWith("//www.youtube.com/") && !filterUrlEventArgs.OriginalUrl.StartsWith("https://www.youtube.com/") && !filterUrlEventArgs.OriginalUrl.StartsWith("https://www.eurospin-viaggi.it/"))
            {
                filterUrlEventArgs.SanitizedUrl = null;
            }
        }
        
        private static void ProcessIFrames(object sender, PostProcessNodeEventArgs postProcessNodeEventArgs)
        {
            var iframe = postProcessNodeEventArgs.Node as IHtmlInlineFrameElement;
            if (iframe == null) {
                return;
            }
            var container = postProcessNodeEventArgs.Document.CreateElement("span");
            container.ClassName = "video-container";
            container.AppendChild(iframe.Clone(true));
            postProcessNodeEventArgs.ReplacementNodes.Add(container);
        }        
        
    }
}