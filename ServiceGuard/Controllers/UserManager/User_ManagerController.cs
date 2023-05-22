using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace ServiceGuard.Controllers.UserManager {

    // For 請求  (前置檢查 & 響應)
    [ApiController]                         // 標記-此類作爲API
    [Route("api/[controller]/helper")]      // 啓用-URL路由
    [EnableCors("CorsPolicy")]              // 啓用-跨域策略 (似情況開啓，請遵循安全策略)
    public class User_ManagerController : Controller {

        [HttpGet]
        public string Index() {
            return "This is UserManager Helper";
        }
    }

}
