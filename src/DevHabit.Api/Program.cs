using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 配置 OpenTelemetry
builder.Services.AddOpenTelemetry()
    // 把应用的名称添加到遥测数据中
    .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
    // 开启（链路追踪） ：是一次请求的完整调用链
    .WithTracing(tracing => tracing
        // 自动追踪通过 HttpClient 发出的出站 HTTP 请求
        .AddHttpClientInstrumentation()
        // 自动追踪进入 ASP.NET Core 的入站 HTTP 请求
        .AddAspNetCoreInstrumentation())
    // 开启（指标） ：是一次请求的性能指标
    .WithMetrics(metrics => metrics
        // 自动采集通过 HttpClient 发出的出站 HTTP 请求的指标
        .AddHttpClientInstrumentation()
        // 自动采集进入 ASP.NET Core 的入站 HTTP 请求的指标 
        .AddAspNetCoreInstrumentation())
    // 把前面配置好的 Traces、Metrics（以及下面配置的 Logs）统一通过 OTLP 协议导出
    .UseOtlpExporter();

// 开启（日志） ：是一次请求的日志记录
builder.Logging.AddOpenTelemetry(options =>
{
    // 把日志的 Scope（作用域）包含进去
    options.IncludeScopes = true;
    // 把格式化后的日志消息包含进去
    options.IncludeFormattedMessage = true;
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.MapControllers();
await app.RunAsync();
