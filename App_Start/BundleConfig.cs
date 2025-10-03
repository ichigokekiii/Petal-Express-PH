// File: ~/App_Start/BundleConfig.cs
using System.Web.Optimization;

public class BundleConfig
{
    public static void RegisterBundles(BundleCollection bundles)
    {
        // This is the bundle for our AngularJS application.
        // The order of the files here is CRITICAL.
        bundles.Add(new ScriptBundle("~/bundles/angularApp").Include(
                    "~/Scripts/angular.min.js",
                    "~/Scripts/HolyScripts/Module.js",
                    "~/Scripts/HolyScripts/Service.js",
                    "~/Scripts/HolyScripts/Controller.js"));
    }
}