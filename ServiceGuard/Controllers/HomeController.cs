using Microsoft.AspNetCore.Mvc;

namespace ServiceGuard.Controllers {

    public class HomeController : Controller {
        
        public string Index() {
            return "This is Home Index";
        }

    }
}
