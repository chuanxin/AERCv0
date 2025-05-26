using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace BootstrapSupport
{
    public class BootstrapBundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region JavaScript
            bundles.Add(new ScriptBundle("~/js").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-migrate-{version}.js",
                "~/Scripts/jquery-ui-{version}.js", //new add
                "~/Scripts/jquery.ui.datepicker-zh-TW.js", //new add
                "~/Scripts/bootstrap.js",
                "~/Scripts/jquery.validate.js",
                //"~/scripts/jquery.validate.unobtrusive.js",
                "~/Scripts/jquery.validate.unobtrusive-custom-for-bootstrap.js",
                "~/Scripts/jquery.unobtrusive-ajax.js",
                "~/Scripts/typeahead.bundle.js", //new add
                "~/Scripts/hogan-2.0.0.js", //new add
                "~/Scripts/jquery.marquee.js", //new add
                "~/Scripts/jquery.blockUI.js", //new add
                "~/Scripts/_BlockUI.js" //new add
                ));
            bundles.Add(new ScriptBundle("~/jqueryui").Include(
                "~/Scripts/jquery-ui-{version}.js"));
            bundles.Add(new ScriptBundle("~/jquery.validate").Include(
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive-custom-for-bootstrap.js"
                ));
            //for jqGrid
            bundles.Add(new ScriptBundle("~/bundles/jqueryjqgrid").Include(
                "~/Scripts/jquery.jqGrid.min.js",
                "~/Scripts/i18n/grid.locale-tw.js"));
            //the number format for input text
            bundles.Add(new ScriptBundle("~/numberformat").Include(
                "~/Scripts/jquery.number.js"));
            //for easyUI
            bundles.Add(new ScriptBundle("~/jquery.easyui").Include(
                "~/Scripts/jquery.easyui-{version}.js",
                "~/Scripts/easyloader.js",
                "~/Scripts/locale/easyui-lang-zh_TW.js"));
            #endregion



            #region CSS
            bundles.Add(new StyleBundle("~/content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-responsive.css",
                "~/Content/bootstrap-mvc-validation.css",
                "~/Content/jquery.marquee.css", //new add
                "~/Content/jquery.Site.css", //new add
                "~/Content/jquery.step.css", //new add
                "~/Content/jquery.style.css", //new add
                "~/Content/jquery.table.css" //new add
                ));
            //for jQuery UI
            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                "~/Content/themes/base/all.css"/*,
                "~/Content/themes/base/jquery.ui.resizable.css",
                "~/Content/themes/base/jquery.ui.selectable.css",
                "~/Content/themes/base/jquery.ui.accordion.css",
                "~/Content/themes/base/jquery.ui.autocomplete.css",
                "~/Content/themes/base/jquery.ui.button.css",
                "~/Content/themes/base/jquery.ui.dialog.css",
                "~/Content/themes/base/jquery.ui.slider.css",
                "~/Content/themes/base/jquery.ui.tabs.css",
                "~/Content/themes/base/jquery.ui.datepicker.css",
                "~/Content/themes/base/jquery.ui.progressbar.css",
                "~/Content/themes/base/jquery.ui.theme.css"*/));
            //for jquery FileUpload
            bundles.Add(new StyleBundle("~/jqueryfileupload/css").Include(
                //"~/Content/jQuery.FileUpload/css/jquery.fileupload-noscript.css",
                //"~/Content/jQuery.FileUpload/css/jquery.fileupload-ui-noscript.css",
                "~/Content/jQuery.FileUpload/css/jquery.fileupload-ui.css",
                "~/Content/jQuery.FileUpload/css/jquery.fileupload.css"));

            //for JqGrid
            bundles.Add(new StyleBundle("~/jqgrid/css").Include(
                "~/Content/jquery.jqGrid/ui.jqgrid.css"));
            //for easyUI
            bundles.Add(new StyleBundle("~/default/easyui").Include(
                //"~/Content/themes/gray/tabs.css",
                "~/Content/themes/gray/easyui.css"));
            #endregion
            
            
        }
    }
}