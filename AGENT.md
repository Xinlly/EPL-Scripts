# AGENT.md — EPL-Scripts

## 项目概况

EPLAN Electric P8 2.9 脚本集合（C#）

- 目标版本：EPLAN Electric P8 2.9
- 许可证：AGPL-3.0
- 版权：Xinlly
- 工作目录：`/var/lib/hermes/workspace/projects/our/EPL-Scripts/`

---

## 目录结构

```
EPL-Scripts/
├── src/                    # 脚本源码
│   ├── HelloWorld/         # Hello World 示例
│   ├── Tests/              # 测试脚本（SNC_Test.cs 等，2024版）
│   └── Utilities/          # 通用工具脚本
├── References/             # 参考资料
│   ├── api-2.9/            # EPLAN API 2.9 离线文档
│   ├── 2.9脚本开发避坑指南.md
│   ├── 英文社区资源汇总.md
│   └── 中文社区实战经验汇总.md
├── .log/                   # 脚本运行日志（仅本地）
├── AGENT.md                # 本文件
├── LICENSE                 # AGPL-3.0
└── README.md               # 项目说明
```

上游参考仓库：
- `projects/upstream/EPLAN-Scripting/` — Suplanus 教程示例（81个）
- `projects/upstream/Eplan-scripts/` — m1cha1 实用小工具（3个）

---

## EPLAN 2.9 脚本基础

### 三种开发方式

| 方式 | 形式 | 语言 | 运行环境 | 难度 |
|------|------|------|----------|------|
| 脚本 (Script) | .cs / .vb 源代码文件 | C# / VB.NET | EPLAN 内加载运行 | ★★☆☆☆ |
| 插件 (Add-in / DLL) | 编译后的 DLL | C# / VB.NET | EPLAN 进程内加载 | ★★★★☆ |
| 离线程序 (EXE) | 独立 EXE | C# / VB.NET | 独立进程，通过 API 连接 EPLAN | ★★★★★ |

### 脚本可用 API 边界（新手最常踩的坑）

脚本（不是 Add-in）默认只能引用以下程序集：
- `System`、`System.XML`、`System.Drawing`、`System.Windows.Forms`
- `Eplan.EplApi.Base`
- `Eplan.EplApi.ApplicationFramework`
- `Eplan.EplApi.Gui`

脚本（不是 Add-in）默认只能引用以下程序集：
- `System`、`System.XML`、`System.Drawing`、`System.Windows.Forms`
- `Eplan.EplApi.Base`
- `Eplan.EplApi.ApplicationFramework`
- `Eplan.EplApi.Gui`
- `Eplan.EplApi.MasterData`（部件数据库操作）

**重要**：`Eplan.EplApi.DataModel` 和 `Eplan.EplApi.HEServices` 在**脚本中不可用**，编译报 CS0234。DataModel 需要 API Extension 许可证，必须通过 Add-in 方式（编译为 DLL 注册）。HEServices 同理。

操作项目数据的替代方案：
- 通过内置 Action 间接操作（CommandLineInterpreter.Execute）
- 通过剪贴板/对话框交互（ReplaceText 模式）
- 通过插入宏文件（InsertComment 模式）

### 脚本开发约定

| 约定 | 说明 |
|------|------|
| 无 namespace | EPLAN 脚本加载器只扫描全局命名空间的类，类必须写在顶层 |
| 全名引用 | 不用 `using`，直接写全名如 `Eplan.EplApi.Gui.ContextMenu` |
| 日志路径 | `$(MD_SCRIPTS)\EPL-Scripts\.log\yyyy-MM-dd.log`，用 `PathMap.SubstitutePath("$(MD_SCRIPTS)")` 静态方法获取实际路径 |
| 右键菜单 | `DialogName="Editor"`, `ContextMenuName="Ged"`（图纸右键菜单），菜单显示格式为 `DialogName.ContextMenuName` |
| 清单同步 | 每个脚本必须同步登记到金山文档脚本清单 |

### 脚本核心特性

| 特性 | 作用 |
|------|------|
| `[Start]` | 脚本启动时执行 |
| `[DeclareAction("操作名")]` | 声明可被工具栏/命令行调用的操作 |
| `[DeclareRegister]` / `[DeclareUnregister]` | 脚本注册/卸载时执行 |
| `[DeclareEventHandler]` | 声明事件处理函数 |
| `[DeclareMenu]` | 声明菜单项 |

### 2.9 版本基本信息

- .NET Framework 版本：**4.7.2**
- 推荐开发工具：Visual Studio 2017 / 2019
- DLL 路径：`C:\Program Files\EPLAN\Platform\2.9.x\Bin\`
- 脚本无需 API 许可证即可使用
- 2.9 是 EPLAN 平台统一 API 架构的重要版本

---

## 核心 API 速查

### Eplan.EplApi.Base

| 类 | 用途 |
|----|------|
| `Settings` | 读写 EPLAN 设置（用户设置、项目设置） |
| `PathMap` | 路径变量替换（`$(DOC)`, `$(PROJECTNAME)`, `$(MD_PROJECTS)`, `$(P)` 等） |
| `Progress` | 进度条，支持取消 |
| `BaseException` | 异常处理 + 分级消息输出（`FixMessage()` 写入系统消息窗口） |
| `MultiLangString` | 多语言字符串 |
| `UserRights` | 获取当前用户名 |
| `Decider` | 决策对话框（确认/警告/输入等） |
| `LockingStep` | 批量锁定包裹，性能优化关键 |

### Eplan.EplApi.ApplicationFramework

| 类 | 用途 |
|----|------|
| `CommandLineInterpreter` | 执行 EPLAN 内置 Action |
| `ActionCallingContext` | Action 参数传递（输入参数 + 返回值） |
| `UndoManager` | 撤销管理，`CreateUndoStep()` 批量操作包裹 |
| `SafetyPoint` | 安全点，异常时自动回滚 |

### Eplan.EplApi.Gui

| 类 | 用途 |
|----|------|
| `Menu` | 添加主菜单项 |
| `ContextMenu` | 添加右键菜单项 |
| `ContextMenuLocation` | 指定对话框和菜单 ID |
| `RibbonBar` | 功能区（Ribbon）操作 |
| `RibbonTab` | Ribbon 选项卡 |

### Eplan.EplApi.MasterData（需 API Extension + Add-in）

| 类 | 用途 |
|----|------|
| `MDPartsManagement` | 部件管理入口 |
| `MDPartsDatabase` | 部件数据库操作 |
| `MDPart` | 部件对象 |

### Eplan.EplApi.DataModel（需 API Extension + Add-in）

| 类 | 用途 |
|----|------|
| `ProjectManager` | 项目管理（打开、获取当前项目） |
| `Project` | 项目对象 |
| `Page` | 页对象 |
| `Function` | 功能对象 |
| `DMObjectsFinder` | 高效查找对象（走索引，比遍历快 10-100 倍） |
| `StorableObject` | 所有可存储对象的基类 |

---

## 常用 Action 列表

通过 `CommandLineInterpreter.Execute()` 调用的常用 Action：

| Action 名 | 用途 |
|-----------|------|
| `reports` | 生成报表 |
| `print` | 打印 |
| `export` | 导出（PDF/DXF/DWG等） |
| `XSeShowSearchResultsAction` | 显示搜索结果 |
| `GfDlgMgrActionIGfWind` | GUI 窗口操作（Copy/Paste/DeleteAll） |
| `SystemErrDialog` | 弹出系统消息对话框 |
| `GetCurrentScriptPath` | 获取已加载脚本的文件路径（未文档化） |
| `GetCurrentLoadedScripts` | 获取当前已加载脚本列表（未文档化） |
| `XEsSetPropertyAction` | 设置属性（批量修改） |
| `XMIaInsertMacro` | 插入宏 |
| `XGedStartInteractionAction` | 启动交互操作 |

命令行启动脚本：`W3u.exe ExecuteScript /ScriptFile:<path> /Param1:val1`

---

## 开发环境与调试

### 环境搭建

1. 安装 Visual Studio（2017/2019 推荐，2022 兼容）
2. 引用 EPLAN DLL（`C:\Program Files\EPLAN\Platform\2.9.x\Bin\`）
3. Target Framework 设为 .NET Framework 4.7.2

### 调试方法

1. **开启脚本调试模式**：
   ```csharp
   var settings = new Settings();
   settings.SetBoolSetting("USER.EplanEplApiScriptLog.DebugScripts", true, 0);
   ```
2. 重启 EPLAN
3. VS 中 Attach 到 `W3u.exe` / `Eplan.exe` 进程
4. 在脚本中打断点

**⚠️ 危险**：调试完务必把 `DebugScripts` 改回 `false`，否则不 Attach 调试器会直接导致 EPLAN 崩溃。

### 调试技巧

- 用 `BaseException.FixMessage()` 替代 `MessageBox` 输出消息（写入系统消息窗口，不阻塞）
- 消息级别：`Assert` / `Error` / `FatalError` / `Message` / `Trace` / `Warning`
- 脚本编译错误在**系统消息**窗口查看，不是弹窗
- VS 中打开行号方便对照 EPLAN 报错行号
- VS 调试插件项目需勾选 **Enable native code debugging**（EPLAN 底层是混合 C++/C#）

---

## 性能优化

### 1. LockingStep 批量包裹

```csharp
using (var lockingStep = new LockingStep())
{
    foreach (var func in functions)
    {
        func.SmartLock();
        func.Properties[...] = newValue;
    }
} // 一次释放，一次提交
```

### 2. UndoManager + SafetyPoint

```csharp
using (var undoStep = new UndoManager().CreateUndoStep())
{
    undoStep.SetUndoDescription("批量修改");
    using (var sp = SafetyPoint.Create())
    {
        // ... modify many objects
        sp.Commit();
    }
}
```

### 3. 用 DMObjectsFinder 替代遍历

不要自己遍历全部对象，`DMObjectsFinder` 走索引快 10-100 倍。

### 4. 大操作优先用 Action 而不是逐对象 API

报表、打印、导出等，用 `CommandLineInterpreter.Execute("reports"/"print"/"export")` 比自己逐页处理快很多。

---

## 隐藏设置与黑知识

### 查看所有 Setting 路径

```csharp
var settings = new Settings();
settings.SetBoolSetting("USER.EnfMVC.ContextMenuSetting.ShowExtended", true, 0);
```
重启后，在任意设置页右键 → "Copy path for setting to Clipboard"。

### 查看右键菜单 ID

```csharp
var settings = new Settings();
settings.SetBoolSetting("USER.EnfMVC.ContextMenuSetting.ShowIdentifier", true, 0);
```

### 属性标识符

所有对象属性都有数字 ID（如 `20427`）和 identifying name（如 `"FUNCTION.DESIGNATION"`）两种表示。2.8 起两者都可用。

### /Quiet 静默模式

批处理脚本中用 `Eplan.exe /Quiet:2 /Auto <action>` 可抑制大多数对话框。

### 脚本可用的 .NET 程序集白名单

脚本默认只能引用：`System`、`System.XML`、`System.Drawing`、`System.Windows.Forms` + EPLAN 的 Base 和 ApplicationFramework。超过这个范围需要用反射（Late Binding）或者改用 Add-in。

---

## 常见坑点速查

| 现象 | 可能原因 | 排查方向 |
|------|----------|----------|
| 脚本加载失败，提示找不到类型 | DataModel/MasterData 不在脚本可用范围 | 改用 Add-in 或只用 Base/ApplicationFramework |
| 一运行脚本 EPLAN 就崩 | DebugScripts 开启但未 Attach 调试器 | 关闭 `USER.EplanEplApiScriptLog.DebugScripts` |
| 修改属性抛锁定异常 | 对象未锁定 / 多用户冲突 | 用 `SmartLock()` 或 `LockingVector` |
| Action 调用后 EPLAN 直接退出 | SEH 异常未被捕获 | 用 LockingVector 包裹，或先手动锁定所有对象 |
| 脚本报 "Object reference not set" | 遍历到了已删除/未初始化对象 | 判空，用 `DMObjectsFinder` 过滤有效对象 |
| 批量更新非常慢 | 每次修改都独立 Locking + Undo | 外层包 LockingStep + UndoStep |
| 导出 PDF 颜色不对 | BLACKWHITE 参数与 GUI 行为不同 | 用 `DegreeOfColor.Color` |
| 打开旧项目报错 | 项目数据库版本不兼容 | 用同版本 EPLAN 打开或先升级 |

---

## 2.9 版本特有问题

1. **项目数据库单向升级**：2.7 及更早项目必须更新后才能在 2.9 打开，更新后无法回退
2. **部件库格式变更**：2.9 部件库可在 2.5-2.8 中编辑，2.4 及更早只读
3. **Locking 异常无法被 try/catch 捕获**：Action 调用触发的锁定冲突是底层 C++ SEH 异常
4. **PDF 导出白色对象不显示**：用 `DegreeOfColor.Color` 替代 `BlackAndWhite`
5. **Modification Date 误改**：不要以修改日期判断项目是否变更
6. **VB 中 Function 关键字冲突**：用完整命名空间或 `[Function]`

---

## 参考资源

### 官方文档

- EPLAN 2.9 脚本文档（中文）：https://www.eplan.help/zh-cn/Infoportal/Content/Plattform/2.9/Content/htm/scripts_k_start.htm
- EPLAN 2.9 API 参考：https://www.eplan.help/en-us/infoportal/content/api/2.9/index.html
- EPLAN 2.9 Action 列表：https://www.eplan.help/en-us/Infoportal/Content/Plattform/2.9/Content/htm/availableactions_k_start.htm

### 社区资源

- Suplanus 脚本教程（最系统）：https://eplan-scripting.suplanus.de/v4/en/
- Suplanus GitHub：https://github.com/Suplanus/
- EplanWiki 脚本：https://github.com/DanielPa/Eplanwiki.Scripting
- 电气CAD吧 入门系列：https://www.cad-bbs.cn/eplan-actions/
- CSDN Leonard_Spark 系列：https://blog.csdn.net/zhshspark/
- B站 渭未安 二次开发教程：https://www.bilibili.com/video/BV1fPx5eoEta/
- EPLAN Forum (英文)：https://eplan.proboards.com/board/11/scripts
- Reddit r/EPlan：https://www.reddit.com/r/EPlan/

---

## 本地上游参考

| 路径 | 来源 | 数量 | 说明 |
|------|------|------|------|
| `projects/upstream/EPLAN-Scripting/` | Suplanus 教程 | 81个 | 按章节分类的入门示例，17个主题 |
| `projects/upstream/Eplan-scripts/` | m1cha1 | 3个 | ClearSearch、ReplaceText、InsertComment |
| `References/api-2.9/html/` | 官方文档 | 24235页 | API 2.9 离线 HTML，可浏览器直接打开 |
| `References/api-2.9/api_2.9.db` | 结构化提取 | 13.6MB | SQLite 数据库，26命名空间 / 917类型 / 39043成员 |
| `References/api-2.9/api_2.9.json` | 结构化提取 | 10.8MB | JSON 格式 API 数据 |
| `References/2.9脚本开发避坑指南.md` | 社区汇总 | 20KB | 8大类坑点与技巧 |
| `References/英文社区资源汇总.md` | 社区汇总 | 24KB | 40+ 英文资源 |
| `References/中文社区实战经验汇总.md` | 社区汇总 | 25KB | 45+ 中文资源 |

---

*最后更新：2026-08-26*
