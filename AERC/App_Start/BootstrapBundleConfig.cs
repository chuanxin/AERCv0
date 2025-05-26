using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace BootstrapSupport
{
    public class BootstrapBundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.IgnoreList.Clear();//
            bundles.Add(new ScriptBundle("~/js").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/bootstrap.js",                
                "~/Scripts/jquery-migrate-{version}.js",
                "~/Scripts/jquery-ui-{version}.js",               
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.unobtrusive-ajax.js",
                "~/scripts/jquery.validate.unobtrusive.js",                
                "~/Scripts/jquery.validate.unobtrusive-custom-for-bootstrap.js",                            
                "~/Scripts/typeahead.bundle.js",//new add
                //"~/Scripts/handlebars.js",//new add
                "~/Scripts/hogan-2.0.0.js",//new add
                "~/Scripts/jquery.marquee.js",//new add
                "~/Scripts/jquery.blockUI.js"//new add
                ));
            bundles.Add(new ScriptBundle("~/bundles/jqueryjqgrid").Include(
                "~/Scripts/jquery.jqGrid.src.js",
                "~/Scripts/i18n/grid.locale-tw.js"));

            bundles.Add(new StyleBundle("~/content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/body.css",
                "~/Content/bootstrap-responsive.css",
                "~/Content/bootstrap-mvc-validation.css",
                "~/Content/jquery.marquee.css"
                ));
            //for JqGrid
            bundles.Add(new StyleBundle("~/jqgrid/css").Include(
                "~/Content/jquery.jqGrid/ui.jqgrid.css"));
            //for jQuery UI
            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
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
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}