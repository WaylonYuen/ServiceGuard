using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ServiceGuard.Commons;
using ServiceGuard.Databases;
using ServiceGuard.Models;

namespace ServiceGuard.Controllers.UserManager {

    // For 請求  (前置檢查 & 響應)
    [ApiController]                 // 標記-此類作爲API
    [Route("api/[controller]")]     // 啓用-URL路由
    [EnableCors("CorsPolicy")]      // 啓用-跨域策略 (似情況開啓，請遵循安全策略)
    public partial class User_LoginController : WebApiTemplate
        <User_LoginController.RequestDataModel, User_LoginController.ResponseDataModel> {

        /// <summary>
        /// API入口 ( 當收到請求時執行 )
        /// </summary>
        /// <param name="value">請求内容</param>
        /// <returns>響應結果</returns>
        [HttpPost] // 請求數據在數據體
        public virtual async Task<object> Login([FromBody] RequestDataModel value) {
            RequestData = value; // 緩存-請求内容

            // 解析-請求内容
            if (await BuildRequest() == false) {
                BuildResponse(); // 建立-響應(打包響應資訊)
                Logger.LogInformation($"{ResponseData}\n");
                return ResponseData; // 回復請求結果
            }

            /*  **前置檢查-説明**
            *   - 1：檢查請求所携帶的請求正文是否符合此<資料模型(RequestDataModel)>
            *   - 2：檢查請求正文是否携帶必要欄位資訊以做校驗等
            *   - 原因：排除 | 過濾不符條件的請求，以提高整體系統效率
            *   - 注意：過多的檢查會降低整體效率!!! 如非必要請不要涵蓋所有欄位進行檢查，可空欄不予以檢查
            *   - 以下為前置檢查流程，檢查條件、流程如需調整可於此調整
            */

            // 前置檢查
            if (
                // 資料頭(Head)
                CheckValidDataHead(@"") == true
                // 資料體(Body)
                && CheckValidDataBody(@"") == true
                // 跨域資訊(CorsPolicy)
                && CheckHasCorsPolicy() == true
                // 高速資料庫比對資料(Redis)
                && ComparisonData() == true
                // 數據解密(Decription)
                && DataDecription() == true
                )
            // 放行(Pass) 前置檢查全部通過
            {
                // 驗證用戶
                if (AuthenticateUser() == true) {
                    BuildResult(WebApiResult.Code.Success);
                }
                // else 失敗：錯誤資訊由内部建立
            }

            BuildResponse(); // 建立-響應(打包響應資訊)
            return ResponseData; // 回復請求結果
        }
    
    }
    
    // For 處理  (資料庫查詢 & 處理邏輯)
    public partial class User_LoginController : WebApiTemplate
        <User_LoginController.RequestDataModel, User_LoginController.ResponseDataModel> {

        /// <summary>
        /// 用戶驗證
        /// </summary>
        /// <returns></returns>
        protected bool AuthenticateUser() {
            // 嘗試呼叫-資料庫
            try {

                // 呼叫資料庫 & 綜合查詢成功
                if(UserMgrDbCtx.UserLogin(new UserDataModels.UserLogin.Linq() {
                    Id = RequestData.Id,
                    Password = RequestData.Password,

                }, out UserDataModels.UserLogin.Result? data) == true) {

                    // 檢查：資料是否存在?
                    if (data != null) {
                        // 寫入-響應正文
                        ResponseData.SessionKey = data.SessionKey; // 身份驗證令牌
                        // More...
                        return true;
                    }

                    Logger.LogInformation("Linq result data is null!");
                    BuildResult(WebApiResult.Code.Fail, "No Data");
                    return false;
                }

                // 查詢失敗，沒有相關 or 符合條件的資料
                BuildResult(WebApiResult.Code.Fail, "Cannot Found.");
                return false;
            }

            // 捕獲例外狀況
            catch (Exception ex) {
#pragma warning disable CA2254
                Logger.LogError(ex.Message);
                Logger.LogError(ex.StackTrace);
#pragma warning restore CA2254

#if DEBUG
                // 暴露例外訊息不安全
                BuildResult(WebApiResult.Code.Exception, ex.Message);
#else
                BuildResult(WebApiResult.Code.Exception);
#endif
                return false;
            }
        }

    }

    // For 構建  (資料模型)
    public partial class User_LoginController : WebApiTemplate
        <User_LoginController.RequestDataModel, User_LoginController.ResponseDataModel> {

        #region DataModel 資料模型
        public struct RequestDataModel {
            #region 僅作參考
            /*  **請求時需要的資料**
            *   todo: 自行添加請求時需要資料欄位，以下僅作參考：
            */
            public string Id { get; set; }
            public string Password { get; set; }
            #endregion
        }
        public struct ResponseDataModel : Commons.IResult {
            public int ResultCode { get; set; }     // 響應代碼 ( 操作成功時為 0 )
            public string ResultMsg { get; set; }   // 響應資訊 ( 錯誤時應記錄錯誤資訊 )

            #region 僅作參考
            /*  **請求時需要的資料**
            *   todo: 自行添加請求時需要資料欄位，以下僅作參考：
            */
            public string SessionKey { get; set; }
            #endregion

            public override string ToString() {
                return "\n" + base.ToString() + " {\n"
                    + $"  >> ResultCode: {ResultCode}\n"
                    + $"  >> ResultMsg: {ResultMsg}\n"
                    + "}\n";
            }
        }
        #endregion

        protected override ILogger Logger { get; set; }
        protected Npgsql_UserManagerDbCtx UserMgrDbCtx { get; set; }

        /// <summary>
        /// Constructor 構建式
        /// </summary>
        /// <param name="logger">依賴注入: 日志</param>
        public User_LoginController(ILogger<User_LoginController> logger, Npgsql_UserManagerDbCtx dbContext) {
            Logger = logger;
            UserMgrDbCtx = dbContext;
            Initialize(this);
        }

        /// <summary>
        /// **建立-響應**
        /// </summary>
        protected override void BuildResponse() {
            ResponseData.ResultCode = result.ResultCode;
            ResponseData.ResultMsg = result.ResultMsg;
            Logger.LogInformation($"{ResponseData}\n");
        }

    }

}
