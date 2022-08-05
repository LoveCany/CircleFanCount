# CircleFanCount

Inspired by [UmamusumeResponseAnalyzer](https://github.com/UmamusumeResponseAnalyzer/UmamusumeResponseAnalyzer).

一个用于将公会粉丝量导出为CSV格式文件的工具，支持[UmamusumeResponseAnalyzer](https://github.com/UmamusumeResponseAnalyzer/UmamusumeResponseAnalyzer)中脱离DMM启动功能。

## 前置 Prerequisite
* 任意可以向指定URL发送数据包的插件，比如[EXNOA-CarrotJuicer](https://github.com/CNA-Bld/EXNOA-CarrotJuicer)或修改版[umamusume-localify](https://github.com/EtherealAO/umamusume-localify)
* (可选，如果需要脱离DMM启动游戏)DMM Game Player β及HTTPS proxy，比如[Fiddler](https://www.telerik.com/fiddler/fiddler-classic)或[mitmproxy](https://mitmproxy.org/)

## 安装 Installation
* 在[Release](https://github.com/LoveCany/CircleFanCount/releases)页面下载最新版本程序
* 将程序放在任意位置，运行其中的CircleFanCount.exe
* 返回主页面后选择启动！，程序显示已启动后即完成CircleFanCount的安装及启动
* 若非使用修改版的[umamusume-localify](https://github.com/EtherealAO/umamusume-localify)，则需要在对应的插件配置中将URL设置为`http://127.0.0.1:4693`

## 检查安装 Checking
* 启动umamusume后，前往公会时若CircleFanCount有信息显示则证明配置正确。

## TODO

- [x] 配置文件与UmamusumeResponseAnalyzer分离，避免配置冲突相互覆盖
- [ ] 支持手动指定CSV数据导出地址
- [ ] 功能剥离为独立模块，发起PR合并到URA中
- [ ] 游戏启动->进入公会过程自动执行，实现BOT指令集成（由于DMM端OS限制目前仅能在Win端实现）
