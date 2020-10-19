using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GameStore.Web.Util
{
    public class PageLinkTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            _urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PageViewModel PageModel { get; set; }

        public string PageAction { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (PageModel.TotalPages > 1)
            {
                IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
                output.TagName = "div";

                TagBuilder tag = new TagBuilder("ul");
                tag.AddCssClass("pagination");

                if (PageModel.PageNumber > 1)
                {
                    TagBuilder prevItem = CreateTag("<");

                    prevItem.MergeAttribute(
                       "onclick",
                       $"updatePageURLParameter({PageModel.PageNumber - 1});");

                    tag.InnerHtml.AppendHtml(prevItem);
                }

                for (int i = 1; i <= PageModel.TotalPages; i++)
                {
                    TagBuilder item;
                    if (i == PageModel.PageNumber)
                    {
                        item = CreateTag(i.ToString(), true);
                    }
                    else
                    {
                        item = CreateTag(i.ToString());
                    }

                    item.MergeAttribute(
                        "onclick",
                        $"updatePageURLParameter({i});");

                    tag.InnerHtml.AppendHtml(item);
                }

                if (PageModel.PageNumber < PageModel.TotalPages)
                {
                    TagBuilder nextItem = CreateTag(">");

                    nextItem.MergeAttribute(
                       "onclick",
                       $"updatePageURLParameter({PageModel.PageNumber + 1});");

                    tag.InnerHtml.AppendHtml(nextItem);
                }

                output.Content.AppendHtml(tag);
            }
        }

        private TagBuilder CreateTag(string pageNumber, bool active = false)
        {
            TagBuilder item = new TagBuilder("li");
            TagBuilder link = new TagBuilder("button");

            if (active)
            {
                item.AddCssClass("active");
            }

            item.AddCssClass("page-item");
            link.AddCssClass("page-link");
            link.InnerHtml.Append(pageNumber.ToString());
            item.InnerHtml.AppendHtml(link);
            return item;
        }
    }
}
