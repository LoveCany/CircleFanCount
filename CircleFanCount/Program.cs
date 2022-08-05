using Spectre.Console;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using CircleFanCount.Localization;

namespace CircleFanCount
{
    public static class CircleFanCount
    {
        static bool runInCmder = false;
        public static async Task Main(string[] args)
        {
            Console.Title = $"CircleFanCount v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
            Console.OutputEncoding = Encoding.UTF8;
            //加载设置
            Config.Initialize();
            await ParseArguments(args);

            if (!AnsiConsole.Profile.Capabilities.Ansi && !runInCmder)
            { //不支持ANSI Escape Sequences，用Cmder打开
                var cmderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CircleFanCount", "cmder");
                var docPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CircleFanCount");
                if (!Directory.Exists(docPath))
                    Directory.CreateDirectory(docPath);
                using var Proc = new Process();
                var StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(cmderPath, "Cmder.exe"),
                    Arguments = $"/start \"{Environment.CurrentDirectory}\" /task CFC",
                    CreateNoWindow = false,
                    UseShellExecute = true
                };
                Proc.StartInfo = StartInfo;
                Proc.Start();
                Proc.WaitForExit();
                Environment.Exit(0);
            }

            string prompt;
            do
            {
                prompt = ShowMenu();
            }
            while (prompt != Resource.LaunchMenu_Start); //如果不是启动则重新显示主菜单

            if (File.Exists(DMM.DMM_CONFIG_FILEPATH) && Config.Get(Resource.ConfigSet_DMMLaunch)) //如果存在DMM的token文件则启用直接登录功能
                DMM.RunUmamusume();
            Server.Start(); //启动HTTP服务器
            AnsiConsole.MarkupLine(Resource.LaunchMenu_Start_Started);

            while (true)
            {
                Console.ReadLine();
            }
        }
        static string ShowMenu()
        {
            var selections = new SelectionPrompt<string>()
                .Title(Resource.LaunchMenu)
                .PageSize(10)
                .AddChoices(new[]
                {
                        Resource.LaunchMenu_Start,
                        Resource.LaunchMenu_Options
                }
                );
            var prompt = AnsiConsole.Prompt(selections);
            if (prompt == Resource.LaunchMenu_Options)
            {
                var multiSelection = new MultiSelectionPrompt<string>()
                    .Title(Resource.LaunchMenu_Options)
                    .Mode(SelectionMode.Leaf)
                    .PageSize(20)
                    .InstructionsText(Resource.LaunchMenu_Options_Instruction);

                foreach (var i in Config.ConfigSet) //根据预设值添加选项
                {
                    if (i.Value == Array.Empty<string>())
                    {
                        multiSelection.AddChoice(i.Key);
                    }
                    else
                    {
                        multiSelection.AddChoiceGroup(i.Key, i.Value);
                    }
                }

                foreach (var i in Config.Configuration) //复原配置文件的选择情况
                {
                    if (i.Value.GetType() == typeof(bool) && (bool)i.Value)
                    {
                        multiSelection.Select(i.Key);
                    }
                }

                foreach (var i in Config.ConfigSet) //如果配置文件中的某一组全被选中，则也选中对应的组
                {
                    if (i.Value != Array.Empty<string>() && Config.Configuration.Where(x => x.Value.GetType() == typeof(bool) && (bool)x.Value == true).Select(x => x.Key).Intersect(i.Value).Count() == i.Value.Length)
                    {
                        multiSelection.Select(i.Key);
                    }
                }

                var options = AnsiConsole.Prompt(multiSelection);

                foreach (var i in Config.Configuration.Keys)
                {
                    if (Config.Get<object>(i).GetType() != typeof(bool)) continue;
                    if (options.Contains(i))
                        Config.Set(i, true);
                    else
                        Config.Set(i, false);
                }
                Config.Save();
            }

            AnsiConsole.Clear();

            return prompt;
        }
        static async Task ParseArguments(string[] args)
        {
            switch (args.Length)
            {
                case 1:
                    {
                        switch (args[0])
                        {
                            case "-v":
                            case "--version":
                                {
                                    Console.Write(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                                    Environment.Exit(0);
                                    return;
                                }
                            case "--get-dmm-onetime-token":
                                {
                                    Console.Write(await DMM.GetExecuteArgsAsync());
                                    Environment.Exit(0);
                                    return;
                                }
                            case "--cmder":
                                {
                                    runInCmder = true;
                                    return;
                                }
                        }
                        return;
                    }
            }
        }
    }
}