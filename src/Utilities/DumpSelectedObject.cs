// DumpSelectedObject.cs
// 输出选中对象的基本信息（脚本模式版本）
// 用法：选中图纸上的一个对象 → 右键 → Dump Selected Object
// 或者：实用工具 → Dump Selected Object
// 日志输出：$(MD_SCRIPTS)\EPL-Scripts\.log\yyyy-mm-dd.log
// 说明：脚本模式下 HEServices/DataModel 不可用，通过 selectionset Action 获取选中的页

public class DumpSelectedObject
{
    // 写日志
    private void Log(string message)
    {
        try
        {
            string strLogDir = Eplan.EplApi.Base.PathMap.SubstitutePath("$(MD_SCRIPTS)") + @"\EPL-Scripts\.log";
            
            // 确保目录存在
            if (!System.IO.Directory.Exists(strLogDir))
            {
                System.IO.Directory.CreateDirectory(strLogDir);
            }
            
            string strLogFile = strLogDir + @"\" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            string strTimestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            System.IO.File.AppendAllText(strLogFile, "[" + strTimestamp + "] " + message + "\r\n");
        }
        catch (System.Exception ex)
        {
            // 日志失败不影响主流程
            System.Diagnostics.Debug.WriteLine("Log failed: " + ex.Message);
        }
    }

    [DeclareRegister]
    public void OnRegister()
    {
        Log("DumpSelectedObject: 注册");
        
        // 注册到图纸右键菜单（DialogName=Editor, ContextMenuName=Ged）
        Eplan.EplApi.Gui.ContextMenuLocation oLoc = 
            new Eplan.EplApi.Gui.ContextMenuLocation();
        oLoc.DialogName = "Editor";
        oLoc.ContextMenuName = "Ged";

        Eplan.EplApi.Gui.ContextMenu oMenu = new Eplan.EplApi.Gui.ContextMenu();
        oMenu.AddMenuItem(
            oLoc,
            "Dump Selected Object",
            "DumpSelectedObject",
            false,
            true);
    }

    [DeclareMenu]
    public void RegisterMenu()
    {
        // 也注册到实用工具菜单
        Eplan.EplApi.Gui.Menu oMenu = new Eplan.EplApi.Gui.Menu();
        oMenu.AddMenuItem(
            "Dump Selected Object",
            "DumpSelectedObject",
            "输出选中对象的详细信息",
            35284,
            0,
            false,
            false);
    }

    [DeclareAction("DumpSelectedObject")]
    public void Run()
    {
        string result = "=== Dump Selected Object ===\n\n";
        Log("DumpSelectedObject: 开始执行");
        
        try
        {
            Eplan.EplApi.ApplicationFramework.CommandLineInterpreter cli = 
                new Eplan.EplApi.ApplicationFramework.CommandLineInterpreter();
            
            // 获取选中的页
            Eplan.EplApi.ApplicationFramework.ActionCallingContext acc = 
                new Eplan.EplApi.ApplicationFramework.ActionCallingContext();
            acc.AddParameter("TYPE", "PAGES");
            cli.Execute("selectionset", acc);
            
            string pages = "";
            acc.GetParameter("PAGES", ref pages);
            
            if (!string.IsNullOrEmpty(pages))
            {
                result += "--- 选中的页 ---\n";
                string[] pageArr = pages.Split(';');
                result += "数量: " + pageArr.Length + "\n";
                foreach (string p in pageArr)
                {
                    if (!string.IsNullOrEmpty(p))
                    {
                        result += "  " + p + "\n";
                    }
                }
                result += "\n";
                Log("选中的页: " + pages.Replace(';', ','));
            }
            
            // 获取选中的对象（尝试多种参数名）
            string[] typeNames = { "OBJECTS", "ELEMENTS", "PLACEMENTS", "FUNCTIONS", "SYMBOLS" };
            foreach (string typeName in typeNames)
            {
                try
                {
                    acc = new Eplan.EplApi.ApplicationFramework.ActionCallingContext();
                    acc.AddParameter("TYPE", typeName);
                    cli.Execute("selectionset", acc);
                    
                    string val = "";
                    acc.GetParameter(typeName, ref val);
                    
                    if (!string.IsNullOrEmpty(val))
                    {
                        result += "--- 选中的对象 (TYPE=" + typeName + ") ---\n";
                        string[] objArr = val.Split(';');
                        result += "数量: " + objArr.Length + "\n";
                        foreach (string o in objArr)
                        {
                            if (!string.IsNullOrEmpty(o))
                            {
                                result += "  " + o + "\n";
                            }
                        }
                        result += "\n";
                        Log("TYPE=" + typeName + ": " + val.Replace(';', ','));
                        break; // 找到第一个有结果的就停
                    }
                }
                catch
                {
                    // 这个 TYPE 不支持，试下一个
                }
            }
            
            // 尝试获取 MD_SCRIPTS 路径验证日志位置
            try
            {
                string strMdScripts = Eplan.EplApi.Base.PathMap.SubstitutePath("$(MD_SCRIPTS)");
                result += "--- 日志路径 ---\n";
                result += "$(MD_SCRIPTS) = " + strMdScripts + "\n";
                result += "日志文件: " + strMdScripts + @"\EPL-Scripts\.log\" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".log" + "\n";
                result += "\n";
                Log("MD_SCRIPTS = " + strMdScripts);
            }
            catch (System.Exception exPath)
            {
                result += "--- 日志路径 (获取失败) ---\n";
                result += exPath.Message + "\n\n";
            }
            
            if (string.IsNullOrEmpty(pages))
            {
                result += "(没有选中任何页，请在页导航器中选中一页)\n";
                result += "\n提示：如果选中了图形对象但没显示，可能是脚本模式下 selectionset Action 不返回对象列表\n";
                Log("无选中内容");
            }
        }
        catch (System.Exception ex)
        {
            result += "\n=== 错误 ===\n";
            result += ex.GetType().Name + ": " + ex.Message + "\n";
            result += ex.StackTrace + "\n";
            Log("错误: " + ex.GetType().Name + " - " + ex.Message);
        }
        
        Log("DumpSelectedObject: 执行完成");
        
        System.Windows.Forms.MessageBox.Show(
            result,
            "Dump Selected Object",
            System.Windows.Forms.MessageBoxButtons.OK,
            System.Windows.Forms.MessageBoxIcon.Information);
    }

    [DeclareUnregister]
    public void OnUnregister()
    {
        Log("DumpSelectedObject: 注销");
    }
}
