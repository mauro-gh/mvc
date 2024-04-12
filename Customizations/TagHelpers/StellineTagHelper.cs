using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace mvc.Customizations.TagHelpers
{

    [HtmlTargetElement("stelline")]
    public class StellineTagHelper : TagHelper
    {

        public double Punteggio { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // si puo' leggere dal contest o dichiare una proprieta' es. Value
            //double value = (double) context.AllAttributes["value"].Value



            for(int i = 1; i <=5; i++)
            {
            if (Punteggio >= i)
            {
                // invio codice html
                output.TagName = "span";
                output.Content.AppendHtml("<i class=\"fas fa-star\"></i>");
            }
            else if (Punteggio > i -1 )
            {
                output.TagName = "span";
                output.Content.AppendHtml("<i class=\"fas fa-star-half-alt\"></i>");
            }
            else
            {
                output.TagName = "span";
                output.Content.AppendHtml("<i class=\"far fa-star\"></i>");
            }

        }

        }
    }
}