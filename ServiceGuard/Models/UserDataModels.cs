using System.ComponentModel.DataAnnotations;

namespace ServiceGuard.Models {

    /// <summary>
    /// 用戶-數據模型
    /// </summary>
    public class UserDataModels {

        public class UserLogin {
            /// <summary>
            /// 綜合查詢(查詢時需要的必備資料)
            /// </summary>
            /// <remarks>
            /// 全稱：Language Integrated Query
            /// </remarks>
            public struct Linq {
                #region 僅作參考
                /*  **資料庫查詢時所需要的必要資料欄位**
                *   todo: 自行添加查詢時需要資料欄位，以下僅作參考：
                */
                public string Id { get; set; }
                public string Password { get; set; }
                #endregion
            }
            /// <summary>
            /// 查詢結果(查詢結果資料載體)
            /// </summary>
            public class Result {
                #region 僅作參考
                /*  **資料庫查詢結果所需的資料承載**
                *   todo: 自行添加需要的資料承載欄位，以下僅作參考：
                */
                [Key] // 請將此標簽標注於'主鍵'
                public string SessionKey { get; set; } = "";
                #endregion
            }
        }

    }

}
