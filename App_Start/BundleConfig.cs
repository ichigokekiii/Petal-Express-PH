using System.Web.Optimization;

public class BundleConfig
{
    public static void RegisterBundles(BundleCollection bundles)
    {

        bundles.Add(new ScriptBundle("~/bundles/angularApp").Include(
                    "~/Scripts/angular.min.js",
                    "~/Scripts/HolyScripts/Module.js",
                    "~/Scripts/HolyScripts/Service.js",
                    "~/Scripts/HolyScripts/Controller.js"));
    }
}