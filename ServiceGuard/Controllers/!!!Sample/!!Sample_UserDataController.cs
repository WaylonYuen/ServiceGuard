using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ServiceGuard.Commons;
using ServiceGuard.Databases.Sample;
using ServiceGuard.Models.Sample;

// 注意: 此命名空間為：參考範本，禁止使用範本空間 ( 即：Sample 結尾的命名空間 )
namespace ServiceGuard.Controllers.Sample {

    // For 請求  (前置檢查 & 響應)
    [ApiController]                 // 標記-此類作爲API
    [Route("api/[controller]")]     // 啓用-URL路由
    [EnableCors("CorsPolicy")]      // 啓用-跨域策略 (似情況開啓，請遵循安全策略)
    public partial class Sample_UserDataController {

        [HttpGet("id/{id}")] // 請求數據附加在 URL
        public virtual async Task<object> GetUserById(
            [FromRoute] string id
        /*  [FromRoute] string name...更多參數繼續往下寫(參數之間記得以逗號結尾，最後一個參數例外)*/) {   // 此變數名稱 id 必須和 HttpGet 標簽中的名稱 {id} 一致

            // 在此方法的最上方定義了路由方式：
            // [HttpGet("id/{id}")] 即可以如此呼叫：GET api/SampleUserData/id/123 將獲得 id = 123

            // 解析-請求内容
            if(await BuildRequest() == false) {
                BuildResponse(); // 建立-響應(打包響應資訊)
                Logger.LogInformation($"{ResponseData}\n");
                return ResponseData; // 回復請求結果
            }

            // 前置檢查
            if (id.Length > 0) {
                // 透過 ID 查找用戶
                if (TryGetUserById(id) == true) {
                    BuildResult(WebApiResult.Code.Success);
                }
                // else 失敗：錯誤資訊由内部建立
            }

            BuildResponse(); // 建立-響應(打包響應資訊)
            Logger.LogInformation($"{ResponseData}\n"); // Debug log

            return ResponseData; // 回復請求結果
        }

        [HttpGet("name/{name}")] // 請求數據附加在 URL
        public virtual async Task<object> GetUserByName(
            [FromRoute] string name) {

            // 解析-請求内容
            if(await BuildRequest() == false) {
                BuildResponse(); // 建立-響應(打包響應資訊)
                Logger.LogInformation($"{ResponseData}\n");
                return ResponseData; // 回復請求結果
            }

            // 前置檢查
            if (name.Length > 0) {
                // 透過 ID 查找用戶
                if (TryGetUserByName(name) == true) {
                    BuildResult(WebApiResult.Code.Success);
                }
                // else 失敗：錯誤資訊由内部建立
            }

            BuildResponse(); // 建立-響應(打包響應資訊)
            return ResponseData; // 回復請求結果
        }

    }

    // For 處理  (資料庫查詢 & 處理邏輯)
    public partial class Sample_UserDataController : WebApiTemplate
        <Sample_UserDataController.RequestDataModel, Sample_UserDataController.ResponseDataModel> {

        protected bool TryGetUserById(string id) {
            // 嘗試呼叫-資料庫
            try {

                // 呼叫資料庫 & 綜合查詢成功
                if(UserMgrDbCtx.FetchUserById(id, out UserDataModel.User.Result? record) == true) {
                    // 檢查：資料是否存在?
                    if (record != null) {
                        // 寫入-響應正文
                        ResponseData.UserDataList = new(); // 創建一筆資料集
                        var data = new UserData() { // 創建一筆記錄
                            Id = record.Id,
                            Name = record.Name,
                            Gender = record.Gender,
                            Email = record.Email
                        };
                        ResponseData.UserDataList.Add(data); // 添加記錄到清單中
                        return true;
                    }
                    else {
                        Logger.LogInformation("Linq result data is null!");
                        BuildResult(WebApiResult.Code.Fail, "No Data");
                        return false;
                    }
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
                BuildResult(WebApiResult.Code.CheckFailed_ValidData);
#else
                // undone
                ResponseData = (ResponseDataModel)Result.BuildExceptionInfo(ResponseData);
#endif
                return false;
            }
        }

        protected bool TryGetUserByName(string name) {
            // 嘗試呼叫-資料庫
            try {

                // 呼叫資料庫 & 綜合查詢成功
                if(UserMgrDbCtx.FetchUserByName(name, out List<UserDataModel.User.Result>? dataList) == true) {
                    // 檢查：資料是否存在?
                    if (dataList != null) {
                        // 寫入-響應正文
                        ResponseData.UserDataList = new(); // 創建一筆資料集
                        foreach (var record in dataList) {
                            var data = new UserData() { // 創建一筆記錄
                                Id = record.Id,
                                Name = record.Name,
                                Gender = record.Gender,
                                Email = record.Email
                            };
                            ResponseData.UserDataList.Add(data); // 添加記錄到清單中
                        }
                        return true;
                    }
                    else {
                        Logger.LogInformation("Linq result data is null!");
                        BuildResult(WebApiResult.Code.Fail, "No Data");
                        return false;
                    }
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
    public partial class Sample_UserDataController : WebApiTemplate
        <Sample_UserDataController.RequestDataModel, Sample_UserDataController.ResponseDataModel> {

        #region DataModel 資料模型
        public struct RequestDataModel {
        
        }
        public struct ResponseDataModel : Commons.IResult {
            public int ResultCode { get; set; }     // 響應代碼 ( 操作成功時為 0 )
            public string ResultMsg { get; set; }   // 響應資訊 ( 錯誤時應記錄錯誤資訊 )

            public List<UserData> UserDataList { get; set; }

            public override string ToString() {
                return "\n" + base.ToString() + " {\n"
                    + $"  >> ResultCode: {ResultCode}\n"
                    + $"  >> ResultMsg: {ResultMsg}\n"
                    + "}\n";
            }
        }
        public struct UserData {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Gender { get; set; }
            public string Email { get; set; }
        }
        #endregion

        protected override ILogger Logger { get; set; }
        protected Npgsql_UserManagerDbCtx UserMgrDbCtx { get; set; }

        /// <summary>
        /// Constructor 構建式
        /// </summary>
        /// <param name="logger">依賴注入: 日志</param>
        public Sample_UserDataController(ILogger<Sample_UserDataController> logger, Npgsql_UserManagerDbCtx dbContext) {
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
