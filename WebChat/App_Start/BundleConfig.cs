using System.Web.Optimization;

namespace WebChat
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Site.css"));

            bundles.Add(new ScriptBundle("~/bundles/chatjs").Include(
                      "~/Scripts/jquery.autosize.js",
                      "~/Scripts/jquery.chatjs.utils.js",
                      "~/Scripts/jquery.chatjs.adapter.servertypes.js",
                      "~/Scripts/jquery.chatjs.adapter.js",
                      "~/Scripts/jquery.chatjs.adapter.signalr.js",
                      "~/Scripts/jquery.chatjs.window.js",
                      "~/Scripts/jquery.chatjs.messageboard.js",
                      "~/Scripts/jquery.chatjs.userlist.js",
                      "~/Scripts/jquery.chatjs.pmwindow.js",
                      "~/Scripts/jquery.chatjs.friendswindow.js",
                      "~/Scripts/jquery.chatjs.controller.js"));

            bundles.Add(new ScriptBundle("~/bundles/signalr").Include(
                "~/Scripts/jquery.signalR-2.2.1.min.js"));

            bundles.Add(new StyleBundle("~/Content/chatcss").Include(
                      "~/Content/jquery.chatjs.css"));
        }
    }
}
