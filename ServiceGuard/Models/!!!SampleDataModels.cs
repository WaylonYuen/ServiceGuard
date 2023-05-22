using System.ComponentModel.DataAnnotations;

// 注意: 此命名空間為：參考範本，禁止使用範本空間 ( 即：Sample 結尾的命名空間 )
namespace ServiceGuard.Models.Sample {

    /// <summary>
    /// 用戶資料模型
    /// </summary>
    public class UserDataModel {

        /* **** 關於用戶的綜合查詢資料模型 ****
        *
        *   - 命名規範：
        *       - 以名詞作爲名稱：
        *       - 
        *
        *   - 注意事項:
        *       - 如非必要，不要以 Data 之類的字詞作爲後綴，因爲主體(UserDataModel)已經表明此為資料模型
        */

        // 完整資料(包含)： Linq 查詢資料模型 + Result 結果資料載體
        /// <summary>
        /// 用戶登入
        /// </summary>
        public class Login {
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

        // 結果導向(僅有)：Result 結果資料載體 (沒有查詢限制，任意結果資料欄位都能成爲查詢條件)
        /// <summary>
        /// 用戶資料
        /// </summary>
        public class User {
            /// <summary>
            /// 查詢結果(查詢結果資料載體)
            /// </summary>
            public class Result {
                #region 僅作參考
                /*  **資料庫查詢結果所需的資料承載**
                *   todo: 自行添加需要的資料承載欄位，以下僅作參考：
                */
                [Key] // 請將此標簽標注於'主鍵'
                public string Id { get; set; } = "";
                public string Name { get; set; } = "";
                public string Gender { get; set; } = "";
                public string Email { get; set; } = "";
                #endregion
            }
        }

        // More...

    }

}
