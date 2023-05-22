
namespace ServiceGuard.Databases {

    /// <summary>
    /// WebApplicationBuilder 拓展方法
    /// </summary>
    public static class WebApplicationBuilderExtensions {

        /// <summary>
        /// 注冊：Npgsql 資料庫 ( 依賴注入 DI：Dependence Injection )
        /// </summary>
        /// <param name="Services">服務注入器</param>
        public static void AddNpgsql(this IServiceCollection Services) {

            // 資料庫 NpgSQL
            // 直接複製多一行範例，然後將 '<xxx>' 尖括號中間的内容改成自己的 DbCtx
            Services.AddEntityFrameworkNpgsql().AddDbContext<Sample.Npgsql_UserManagerDbCtx>();
            Services.AddEntityFrameworkNpgsql().AddDbContext<Npgsql_UserManagerDbCtx>();
            // More...

        }

        /// <summary>
        /// 注冊：MSSQL 資料庫 ( 依賴注入 DI：Dependence Injection )
        /// </summary>
        /// <param name="Services">服務注入器</param>
        public static void AddMSSQL(this IServiceCollection Services) {

            // 資料庫 MSSQL
            // todo:

            // More...

        }


    }

}
