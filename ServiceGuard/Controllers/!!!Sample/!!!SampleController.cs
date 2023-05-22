using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ServiceGuard.Commons;
using ServiceGuard.Databases.Sample;
using Microsoft.AspNetCore.Authorization;

// 注意: 此命名空間為：參考範本，禁止使用範本空間 ( 即：Sample 結尾的命名空間 )
namespace ServiceGuard.Controllers.Sample {

    // For 請求  (前置檢查 & 響應)
    [ApiController]                 // 標記-此類作爲API
    [Route("api/[controller]")]     // 啓用-URL路由
    [EnableCors("CorsPolicy")]      // 啓用-跨域策略 (似情況來指定策略，請遵循安全策略)
    //[Authorize]                     // 啓用-身份驗證 (驗證通過才能夠訪問此資源)
    public partial class SampleController : WebApiTemplate
        <SampleController.RequestDataModel, SampleController.ResponseDataModel> {

#pragma warning disable CS1998 // Async 方法缺乏 'await' 運算子，將同步執行 

        [HttpPost] // 請求數據在數據體
        public virtual async Task<object> Post([FromBody] RequestDataModel value) {
            // todo: 處理 Post 請求的邏輯
            return ResponseData; // 回復請求結果
        }

        [HttpGet] // 請求數據附加在 URL
        public virtual async Task<object> Get([FromBody] RequestDataModel value) {
            // todo: 處理 Get 請求的邏輯
            return ResponseData; // 回復請求結果
        }
        
        [HttpDelete]
        public virtual async Task<object> Delete([FromBody] RequestDataModel value) {
            // todo: 處理 Delete 請求的邏輯
            return ResponseData; // 回復請求結果
        }

        [HttpPut]
        public virtual async Task<object> Put([FromBody] RequestDataModel value) {
            // todo: 處理 Put 請求的邏輯
            return ResponseData; // 回復請求結果
        }

        [HttpPatch]
        public virtual async Task<object> Patch([FromBody] RequestDataModel value) {
            // todo: 處理 Patch 請求的邏輯
            return ResponseData; // 回復請求結果
        }

        [HttpHead]
        public virtual async Task<object> Head([FromBody] RequestDataModel value) {
            // todo: 處理 Head 請求的邏輯
            return ResponseData; // 回復請求結果
        }

        [HttpOptions]
        public virtual async Task<object> Options([FromBody] RequestDataModel value) {
            // todo: 處理 Options 請求的邏輯
            return ResponseData; // 回復請求結果
        }

#pragma warning restore CS1998

    }

    // For 處理  (資料庫查詢 & 處理邏輯)
    public partial class SampleController : WebApiTemplate
        <SampleController.RequestDataModel, SampleController.ResponseDataModel> {

        protected override bool ProcessData() {
            // 如果僅單一職責，請使用此方法
            return false;
        }

    }

    // For 構建  (資料模型)
    public partial class SampleController : WebApiTemplate
        <SampleController.RequestDataModel, SampleController.ResponseDataModel> {

        #region DataModel 資料模型
        public struct RequestDataModel {
            #region 僅作參考
            /*  **請求時需要的資料**
            *   todo: 自行添加請求時需要資料欄位，以下僅作參考：
            */
            public string Data { get; set; }
            #endregion
        }
        public struct ResponseDataModel : Commons.IResult {
            public int ResultCode { get; set; }     // 響應代碼 ( 操作成功時為 0 )
            public string ResultMsg { get; set; }   // 響應資訊 ( 錯誤時應記錄錯誤資訊 )

            #region 僅作參考
            /*  **請求時需要的資料**
            *   todo: 自行添加請求時需要資料欄位，以下僅作參考：
            */
            public string Data { get; set; }
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
        public SampleController(ILogger<SampleController> logger, Npgsql_UserManagerDbCtx dbContext) {
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
