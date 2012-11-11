using System.Collections.Generic;
using System.Web.Mvc;

namespace LeadGen.Web.Application
{
    public static class ControllerUserNotificationExtensions
    {
        public static List<string> HighFives(this HtmlHelper helper )
        {
            if (helper.ViewContext.TempData["HighFives"] == null)
                return new List<string>();

            var highFives = (List<string>)helper.ViewContext.TempData["HighFives"];

            return highFives;
        }

        public static void HighFive(this ControllerBase controller, string whyTheUserWasSuccessful)
        {
            if (controller.TempData["HighFives"] == null)
                controller.TempData["HighFives"] = new List<string>();

            var highFives = (List<string>)controller.TempData["HighFives"];

            if (!string.IsNullOrWhiteSpace(whyTheUserWasSuccessful))
                highFives.Add(whyTheUserWasSuccessful);
        }

        public static void AddError(this ControllerBase controller, string whyTheUserFailedLol)
        {
            if (controller.TempData["Errors"] == null)
                controller.TempData["Errors"] = new List<string>();

            var highFives = (List<string>)controller.TempData["Errors"];

            if (!string.IsNullOrWhiteSpace(whyTheUserFailedLol))
                highFives.Add(whyTheUserFailedLol);
        }
    }
}
