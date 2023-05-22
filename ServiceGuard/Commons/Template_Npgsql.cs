using Microsoft.EntityFrameworkCore;
using ServiceGuard.AppLibs;

namespace ServiceGuard.Commons {

    /// <summary>
    /// .Net PostgreSQL 資料庫上下文-模板
    /// </summary>
    public abstract class NpgsqlDbCtxTemplate : DbCtxTemplate {

        #region Constructor 構建式
        public NpgsqlDbCtxTemplate(ILogger<DbCtxTemplate> logger) : base(logger) { }
        public NpgsqlDbCtxTemplate(DbContextOptions<DbCtxTemplate> options, ILogger<DbCtxTemplate> logger) : base(options, logger) { }
        #endregion

        /// <summary>
        /// 資料庫連綫設定
        /// </summary>
        /// <param name="optionsBuilder">
        ///     A builder used to create or modify options for this context. Databases (and other extensions)
        ///     typically define extension methods on this object that allow you to configure the context.
        /// </param>
        protected override void Connect(DbContextOptionsBuilder optionsBuilder) {

            // 取得 App 參數設定值
            var connStr = AppSettings.DbConnectionStr;

            // 兼容無資料庫
            if (connStr == "") {
                Logger.LogInformation("No database connecting string.");
                return;
            }

            // 使用 Npgsql 建立連綫
            optionsBuilder.UseNpgsql(connStr);
        }

    }
}
