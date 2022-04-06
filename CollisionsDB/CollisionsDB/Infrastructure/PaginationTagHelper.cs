using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using CollisionsDB.Models;
using CollisionsDB.Models.ViewModels;

namespace CollisionsDB.Infrastructure
{
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PaginationTagHelper : TagHelper
    {
        private IUrlHelperFactory uhf;

        public PaginationTagHelper(IUrlHelperFactory temp)
        {
            uhf = temp;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext vc { get; set; }

        public PageInfo PageModel { get; set; }
        public string PageAction { get; set; }
        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        private void RenderPageLink(TagHelperContext thc, TagHelperOutput tho, TagBuilder final, int page)
        {
            IUrlHelper uh = uhf.GetUrlHelper(vc);

            TagBuilder tb = new TagBuilder("a");

            tb.Attributes["href"] = uh.Action(PageAction, new { pageNum = page });
            if (PageClassesEnabled)
            {
                tb.AddCssClass(PageClass);
                tb.AddCssClass(page == PageModel.CurrentPage
                    ? PageClassSelected : PageClassNormal);
            }
            tb.InnerHtml.Append(page.ToString());

            final.InnerHtml.AppendHtml(tb);
        }

        private void RenderDotDotDot(TagHelperContext thc, TagHelperOutput tho, TagBuilder final)
        {
            IUrlHelper uh = uhf.GetUrlHelper(vc);

            TagBuilder tb = new TagBuilder("a");

            if (PageClassesEnabled)
            {
                tb.AddCssClass(PageClass);
                tb.AddCssClass(PageClassNormal);
            }
            tb.InnerHtml.Append("...");

            final.InnerHtml.AppendHtml(tb);
        }

        public override void Process(TagHelperContext thc, TagHelperOutput tho)
        {

            IUrlHelper uh = uhf.GetUrlHelper(vc);

            TagBuilder final = new TagBuilder("div");

            RenderPageLink(thc, tho, final, 1);

            const int NUM_PAGES = 3;

            // render the dot dot dot if necessary
            if (PageModel.CurrentPage - NUM_PAGES - 1 > 1) RenderDotDotDot(thc, tho, final);

            for(int i = PageModel.CurrentPage - NUM_PAGES; i <= PageModel.CurrentPage + NUM_PAGES; i++)
            {
                // don't double-render page 1 or last page
                if (i <= 1 || i >= PageModel.TotalPages) continue;
                RenderPageLink(thc, tho, final, i);
            }

            if (PageModel.CurrentPage + NUM_PAGES < PageModel.TotalPages) RenderDotDotDot(thc, tho, final);

            RenderPageLink(thc, tho, final, PageModel.TotalPages);

            tho.Content.AppendHtml(final.InnerHtml);
        }
    }
}
