using System.Web;
using System.Web.Optimization;

namespace GTDoro.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-unobtrusive").Include(
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/Scripts/colorbox").Include(
                      "~/Scripts/jquery.colorbox-min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css",
            //          "~/Content/colorbox.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/site.css",
                        "~/Content/colorbox.css"));

            bundles.Add(new ScriptBundle("~/Scripts/jqueryui/combobox/js").Include(
                        "~/Scripts/jquery.ui.combobox.js"));

            bundles.Add(new StyleBundle("~/Content/jqueryui/combobox/css").Include(
                        "~/Content/themes/base/jquery.ui.combobox.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new StyleBundle("~/Content/jqueryui").Include(
                        "~/Content/themes/base/jquery-ui.css"));

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

            bundles.Add(new ScriptBundle("~/Scripts/toggle-menu").Include(
                        "~/Scripts/toggle-menu.js"));
            bundles.Add(new StyleBundle("~/Content/toggle-menu").Include(
                        "~/Content/toggle-menu.css"));

            bundles.Add(new ScriptBundle("~/Scripts/gtdoro").Include(
                "~/Scripts/gtdoro.js"));

            bundles.Add(new ScriptBundle("~/Scripts/dates").Include(
                "~/Scripts/gtdoro.dates.js"));

            //datatables
            bundles.Add(new StyleBundle("~/Content/dataTables").Include(
                        "~/Content/dataTables.bootstrap.css", 
                        "~/Content/jquery.dataTables.css"));
            bundles.Add(new ScriptBundle("~/Scripts/dataTables").Include(
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/dataTables.bootstrap.js"));

            //morris charts
            bundles.Add(new ScriptBundle("~/Scripts/morris").Include(
                        "~/Scripts/raphael-min.js",
                        "~/Scripts/morris.js"));
            bundles.Add(new StyleBundle("~/Content/morris").Include(
                        "~/Content/morris.css"));

            //full calendar
            bundles.Add(new ScriptBundle("~/Scripts/fullcalendar").Include(
                        "~/Scripts/fullcalendar.min.js"));
            bundles.Add(new StyleBundle("~/Content/fullcalendar").Include(
                        "~/Content/fullcalendar.css"));
            bundles.Add(new StyleBundle("~/Content/fullcalendar-print").Include(
                        "~/Content/fullcalendar.print.css"));

            //WYSIHTML5
            bundles.Add(new ScriptBundle("~/Scripts/wysihtml5").Include(
                        "~/Scripts/bootstrap3-wysihtml5.all.min.js"));
            bundles.Add(new StyleBundle("~/Content/wysihtml5").Include(
                        "~/Content/bootstrap3-wysihtml5.min.css"));

            //howler
            bundles.Add(new ScriptBundle("~/Scripts/howler").Include(
                "~/Scripts/howler.min.js"));

        }
    }
}
