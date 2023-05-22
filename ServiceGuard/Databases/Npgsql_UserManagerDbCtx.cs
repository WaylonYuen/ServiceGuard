#define DEBUG_UseFakeData

using Npgsql;
using Microsoft.EntityFrameworkCore;
using ServiceGuard.Commons;
using ServiceGuard.Models;

namespace ServiceGuard.Databases {

    // 構建
    public partial class Npgsql_UserManagerDbCtx : NpgsqlDbCtxTemplate {
        public Npgsql_UserManagerDbCtx(ILogger<DbCtxTemplate> logger)
            : base(logger) {
        }
        public Npgsql_UserManagerDbCtx(DbContextOptions<DbCtxTemplate> options, ILogger<DbCtxTemplate> logger)
            : base(options, logger) {
        }
    }

    // 結果資料模型  (承載體)
    public partial class Npgsql_UserManagerDbCtx : NpgsqlDbCtxTemplate {
        public DbSet<UserDataModels.UserLogin.Result> UserLoginResultModel { get; set; }
        // More...
    }

    // 服務  (業務邏輯)
    public partial class Npgsql_UserManagerDbCtx : NpgsqlDbCtxTemplate {

        /// <summary>
        /// 用戶-登入
        /// </summary>
        /// <param name="parameter">查詢參數</param>
        /// <param name="result">結果資料載體</param>
        /// <returns></returns>
        public bool UserLogin(UserDataModels.UserLogin.Linq parameter, out UserDataModels.UserLogin.Result? result) {
#if DEBUG_UseFakeData
            // 使用模擬資料
            result = new() {
                SessionKey = "FakeData_SKey",
            };
            return true;
#else
            /* 建立-查詢指令
            *   - SQL 範例：SELECT 結果欄位 FROM 資料庫名稱 WHERE 條件
            */
            string cmd = "SELECT * FROM User WHERE id=@id, password=@password";

            // 建立-查詢參數
            NpgsqlParameter[] param = new NpgsqlParameter[] {
                new NpgsqlParameter("@id", parameter.Id),
                new NpgsqlParameter("@password", parameter.Password),
            };

            // 呼叫-資料庫
            var resultList = UserLoginResultModel.FromSqlRaw(cmd, param).ToList();

            // 獲取一筆資料
            result = resultList.FirstOrDefault(); // 多筆資料需注解此行

            /* 獲取多筆資料
            *   - 1. 請注解掉該行内容: result = resultList.FirstOrDefault();
            *   - 2. 將 result 參數改爲: out List<UserDataModel.Login.Result>? result
            */

            // 返回執行判斷結果
            return resultList.Count > 0; // 大於 0 筆資料即為查詢到結果，返回 true; 反之亦然。
#endif
        }

    }

}
