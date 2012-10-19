using System.Linq;
using System.Web.Mvc;
using LeadGen.Core;
using Raven.Client;

namespace LeadGen.Web.Controllers
{
    public class ControllerBase : Controller
    {



        public User CurrentUser
        {
            get { return RavenSession.Load<User>("users/" + User.Identity.Name); }
        }

        public IDocumentSession RavenSession { get; protected set; }

    

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RavenSession = MvcApplication.Store.OpenSession();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var viewResult = filterContext.Result as ViewResult;

            if (viewResult == null)
                return;

            viewResult.ViewBag.CurrentUser = ((ControllerBase)filterContext.Controller).CurrentUser;

            base.OnActionExecuted(filterContext);

            if (filterContext.IsChildAction)
                return;
        }

    }
}
