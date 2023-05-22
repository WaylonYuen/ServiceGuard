using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using ServiceGuard.Middlewares;
using ServiceGuard.AppLibs;
using ServiceGuard.Databases;

// Main Program

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("https://localhost:5000");

var environment = builder.Environment;
var configuration = builder.Configuration;
configuration.SetBasePath(environment.ContentRootPath);
configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
configuration.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true);
configuration.AddEnvironmentVariables();

#region Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddControllersWithViews(); // 控制器路由視圖服務 index.cshtml

/************************************************
* NewtonsoftJson
*/
builder.Services.AddMvcCore().AddNewtonsoftJson();

/************************************************
* OpenApi: Swagger >> [ .../swagger/index.html ]
*/
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    // 避免 Swagger 的 SchemaId 相同 (以'名稱_Id'避免相同名稱)
    options.CustomSchemaIds(type => $"{type.Name}_{Guid.NewGuid()}");
});

/************************************************
* CorsPolicy 跨域策略
* >> 允許來自不同源(域名、端口或協議)的 HTTP請求
* >> 預設情況瀏覽器會阻擋跨域請求，以避免惡意攻擊
*/
builder.Services.AddCors(options => {
    // 跨域策略名稱: CorsPolicy
    options.AddPolicy("CorsPolicy", builder => {
        builder
        //.WithOrigins(AppSettings.Origins.ToArray()) // 跨域許可
        .AllowAnyOrigin()       // 任何來源
        .AllowAnyHeader()       // 任何標頭
        .AllowAnyMethod()       // 任何方法
        .AllowCredentials()    // 允許携帶身份驗證信息 (JWT)
        ;
    });
});

/************************************************
* JWT 身份驗證
*/
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // 使用 JWT Bearer 身份驗證方案
// 配置 JWT Bearer
.AddJwtBearer(options => {
    // 是否將詳細錯誤信息傳送給 Client (當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因)
    options.IncludeErrorDetails = true;
    options.TokenValidationParameters = new TokenValidationParameters { // JWT 令牌驗證的參數設定

        // 透過這項宣告，就可以從 "sub" 取值並設定給 User.Identity.Name
        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
        // 透過這項宣告，就可以從 "roles" 取值，並可讓 [Authorize] 判斷角色
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

        /*  ** 發行者 **
        *   生成 JWT 的服務器的識別符。它通常是發行 JWT 的伺服器的網址或名稱
        */
        ValidateIssuer = true,                      // 是否驗證設定
        ValidIssuer = "https://meowlien.com",       // 發行 JWT 的服務器網址或名稱

        /*  ** 受衆 **
        *   JWT 的預期接收和處理 JWT 的實體。可以是特定的使用者、應用程式或服務
        */
        ValidateAudience = false,                   // 是否驗證設定 >> 單一伺服器通常不太需要驗證
        ValidAudience = "https://meowlien.com",     // JWT 的預期接收和處理 JWT 的實體 (例如：特定的使用者、應用程式或服務)

        /*  ** 有效期 **
        *   JWT 的有效期限。它表示 JWT 在何時過期，過期後將無法再被使用。在 JWT 的 exp（expiry）聲明中指定，使用 UTC 時間表示
        */
        ValidateLifetime = true,                    // 是否驗證有效期
        RequireExpirationTime = false,              // JWT 過期-是否驗證設定

        /*  ** 密鑰 **
        *   用於對 JWT 進行簽名和驗證的秘密金鑰。它必須是一個安全且機密的值，只有伺服器和授權方才能知道。 
        */
        ValidateIssuerSigningKey = false,           // 是否驗證密鑰 (如果 Token 中包含 key 才需要驗證，一般都只有簽章而已)
        IssuerSigningKey =                          // JWT 加密鑰匙-設定
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234567890123456"))
    };
});

/************************************************
* Database 資料庫
*/
builder.Services.AddNpgsql();   // Postgre-SQL
//builder.Services.AddMSSQL();    // MS-SQL

/************************************************
* Others | Test
*/

// 設定請求正文最大緩衝區大小
builder.Services.Configure<IISServerOptions>(options  => {
    options.MaxRequestBodyBufferSize = AppSettings.MaxRequestBodyBufferSize;
});

builder.Services.AddLogging(); // 依賴注入: ILogger
var app = builder.Build();
#endregion

#region Configure the HTTP request pipeline.

// 是否在開發環境中運行
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage(); // 提供錯誤詳細訊息
    app.UseSwagger();                // 啓用-Swagger API 測試頁面
    app.UseSwaggerUI();              // '''
}

/************************************************
* IApplicationBuilder
*/
app.UseHttpsRedirection();        // 啓用-HTTPS導向功能 (將HTTP重新導向到HTTPS)
//app.UseStaticFiles();             // 啓用-靜態文件
app.UseRouting();                 // 啓用-URL路由
app.UseCors("CorsPolicy");        // 啓用-跨域策略
app.UseAuthentication();          // 啓用-身份驗證
app.UseAuthorization();           // 啓用-授權功能

// 備份-請求正文
//app.UseMiddleware<RequestBodyReaderMiddleware>();
app.Use(async (context, next) => { // RequestBodyReader

    var request = context.Request;

    if (request.Method != HttpMethods.Post) {
        await next(context);
        return;
    }

    // 讀取-請求正文
    using MemoryStream ms = new(); {
        await request.BodyReader.CopyToAsync(ms);
        ms.Seek(0, SeekOrigin.Begin);
        request.Body = ms;
    }

    

    // 執行-下一個中間件
    await next(context);
});

/************************************************
* IEndpointRouterBuilder
*/
app.MapControllers();               // 將控制器類型映射到 URL 路由       [Services >> AddControllers()]
app.MapControllerRoute(             // 預設 URL 路由頁面 index.cshtml   [Services >> AddControllers() | AddControllersWithViews()]
    name: "default",
    pattern: "{controller}/{action}/{id?}",
    defaults: new { controller = "Home", action = "Index" }
);

/************************************************
* IHostBuilder
*/

app.Run();
#endregion