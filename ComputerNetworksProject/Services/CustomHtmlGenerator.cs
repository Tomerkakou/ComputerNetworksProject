using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace ComputerNetworksProject.Services
{
    public class CustomHtmlGenerator : DefaultHtmlGenerator
    {
        public CustomHtmlGenerator(IAntiforgery antiforgery, IOptions<MvcViewOptions> optionsAccessor,
            IModelMetadataProvider metadataProvider, IUrlHelperFactory urlHelperFactory, HtmlEncoder htmlEncoder,
            ValidationHtmlAttributeProvider validationAttributeProvider) :
            base(antiforgery, optionsAccessor, metadataProvider, urlHelperFactory, htmlEncoder, validationAttributeProvider)
        {
        }

        protected override TagBuilder GenerateInput(ViewContext viewContext, InputType inputType, ModelExplorer modelExplorer, string expression,
            object value, bool useViewData, bool isChecked, bool setId, bool isExplicitValue, string format,
            IDictionary<string, object> htmlAttributes)
        {
            var tagBuilder = base.GenerateInput(viewContext, inputType, modelExplorer, expression, value, useViewData, isChecked, setId, isExplicitValue, format, htmlAttributes);
            FixValidationCssClassNames(tagBuilder);

            return tagBuilder;
        }

        public override TagBuilder GenerateTextArea(ViewContext viewContext, ModelExplorer modelExplorer, string expression, int rows,
            int columns, object htmlAttributes)
        {
            var tagBuilder = base.GenerateTextArea(viewContext, modelExplorer, expression, rows, columns, htmlAttributes);
            FixValidationCssClassNames(tagBuilder);

            return tagBuilder;
        }

        private void FixValidationCssClassNames(TagBuilder tagBuilder)
        {
            tagBuilder.ReplaceCssClass("form-control "+HtmlHelper.ValidationInputCssClassName, "form-control is-invalid");
            tagBuilder.ReplaceCssClass("form-control " + HtmlHelper.ValidationInputValidCssClassName, "form-control is-valid");
        }
    }

    public static class TagBuilderHelpers
    {
        public static void ReplaceCssClass(this TagBuilder tagBuilder, string old, string val)
        {
            if (!tagBuilder.Attributes.TryGetValue("class", out string str)) return;
            tagBuilder.Attributes["class"] = str.Replace(old, val);
        }
    }
}
