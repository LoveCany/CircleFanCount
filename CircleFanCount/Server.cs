using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using System.Net;
using System.Runtime.InteropServices;
using CircleFanCount.Localization;
using CircleFanCount.Handler;
using CircleFanCount.Gallop;

namespace CircleFanCount
{
    internal static class Server
    {
        static HttpListener httpListener;
        static readonly object _lock = new();
        public static void Start()
        {
            try
            {
                httpListener = new();
                httpListener.Prefixes.Add("http://*:4693/");
                httpListener.Start();
                AnsiConsole.MarkupLine("服务器已于http://*:4693/启动");
            }
            catch
            {
                httpListener = new();
                httpListener.Prefixes.Add("http://127.0.0.1:4693/");
                httpListener.Start();
                AnsiConsole.MarkupLine("服务器已于http://127.0.0.1:4693/启动，如需模拟器/手机连入请以管理员权限运行");
            }
            Task.Run(async () =>
            {
                while (httpListener.IsListening)
                {
                    try
                    {
                        var ctx = await httpListener.GetContextAsync();

                        using var ms = new MemoryStream();
                        ctx.Request.InputStream.CopyTo(ms);
                        var buffer = ms.ToArray();

                        if (ctx.Request.RawUrl == "/notify/response")
                        {
#if DEBUG
                            Directory.CreateDirectory("packets");
                            File.WriteAllBytes($@"./packets/{DateTime.Now:yy-MM-dd HH-mm-ss-fff}R.bin", buffer);
                            File.WriteAllText($@"./packets/{DateTime.Now:yy-MM-dd HH-mm-ss-fff}R.json", JObject.Parse(MessagePackSerializer.ConvertToJson(buffer)).ToString());
#endif
                            if (Config.Get(Resource.ConfigSet_SaveResponseForDebug))
                            {
                                var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UmamusumeResponseAnalyzer", "packets");
                                if (Directory.Exists(directory))
                                {
                                    foreach (var i in Directory.GetFiles(directory))
                                    {
                                        var fileInfo = new FileInfo(i);
                                        if (fileInfo.CreationTime.AddDays(1) < DateTime.Now)
                                            fileInfo.Delete();
                                    }
                                }
                                else
                                {
                                    Directory.CreateDirectory(directory);
                                }
                                File.WriteAllBytes($"{directory}/{DateTime.Now:yy-MM-dd HH-mm-ss-fff}R.msgpack", buffer);
                            }
                            _ = Task.Run(() => ParseResponse(buffer));
                        }
                        else if (ctx.Request.RawUrl == "/notify/request")
                        {
#if DEBUG
                            Directory.CreateDirectory("packets");
                            File.WriteAllText($@"./packets/{DateTime.Now:yy-MM-dd HH-mm-ss-fff}Q.json", JObject.Parse(MessagePackSerializer.ConvertToJson(buffer.AsMemory()[170..])).ToString());
#endif
                            _ = Task.Run(() => ParseRequest(buffer[170..]));
                        }
                        else if (ctx.Request.RawUrl == "/notify/ping")
                        {
                            AnsiConsole.MarkupLine("[green]检测到从游戏发来的请求，配置正确[/]");
                            await ctx.Response.OutputStream.WriteAsync(System.Text.Encoding.UTF8.GetBytes("pong"));
                            ctx.Response.Close();
                            continue;
                        }

                        await ctx.Response.OutputStream.WriteAsync(Array.Empty<byte>());
                        ctx.Response.Close();
                    }
                    catch
                    {

                    }
                }
            });
        }
        static void ParseRequest(byte[] buffer)
        {
            try
            {
                lock (_lock)
                {
                    var str = MessagePackSerializer.ConvertToJson(buffer);
                    switch (str)
                    {
                        default:
                            return;
                    }
                }
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e);
            }
        }
        static void ParseResponse(byte[] buffer)
        {
            lock (_lock)
            {
                try
                {
                    var dyn = JsonConvert.DeserializeObject<dynamic>(MessagePackSerializer.ConvertToJson(buffer));
                    if (dyn == default(dynamic)) return;
                    var data = dyn.data;
                    if (data.circle_info != null && data.circle_ranking_this_month != null && data.circle_ranking_last_month != null && data.summary_user_info_array != null)
                    {
                        if (Config.Get(Resource.ConfigSet_ParseCircleInfoResponse))
                            Handlers.ParseCircleSummaryInfoResponse(dyn.ToObject<CircleInfoResponse>());
                    }
                }
                catch (Exception e)
                {
                    AnsiConsole.MarkupLine("[red]解析Response时出现错误: (如果程序运行正常则可以忽略)[/]");
                    AnsiConsole.WriteException(e);
                }
            }
        }
    }
}
