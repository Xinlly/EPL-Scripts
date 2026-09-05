// DumpSelectedObject.cs
// 输出选中对象的基本信息（脚本模式版本，仅用 Base + ApplicationFramework + Gui）
// 用法：选中图纸上的一个对象 → 右键 → Dump Selected Object
// 或者：实用工具 → Dump Selected Object
// 说明：脚本模式无法直接使用 DataModel/HEServices，本脚本通过 selectionset Action 获取信息

public class DumpSelectedObject
{
    [DeclareRegister]
    public void OnRegister()
    {
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
        
        try
        {
            Eplan.EplApi.ApplicationFramework.CommandLineInterpreter cli = 
                new Eplan.EplApi.ApplicationFramework.CommandLineInterpreter();
            Eplan.EplApi.ApplicationFramework.ActionCallingContext acc = 
                new Eplan.EplApi.ApplicationFramework.ActionCallingContext();
            
            // 获取选中的页
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
            }
            
            // 获取选中的对象
            acc = new Eplan.EplApi.ApplicationFramework.ActionCallingContext();
            acc.AddParameter("TYPE", "OBJECTS");
            cli.Execute("selectionset", acc);
            
            string objects = "";
            acc.GetParameter("OBJECTS", ref objects);
            
            if (!string.IsNullOrEmpty(objects))
            {
                result += "--- 选中的对象 ---\n";
                string[] objArr = objects.Split(';');
                result += "数量: " + objArr.Length + "\n";
                foreach (string o in objArr)
                {
                    if (!string.IsNullOrEmpty(o))
                    {
                        result += "  " + o + "\n";
                    }
                }
                result += "\n";
            }
            
            // 尝试获取更多信息
            acc = new Eplan.EplApi.ApplicationFramework.ActionCallingContext();
            acc.AddParameter("TYPE", "SELECTION");
            cli.Execute("selectionset", acc);
            
            string selection = "";
            acc.GetParameter("SELECTION", ref selection);
            
            if (!string.IsNullOrEmpty(selection))
            {
                result += "--- SELECTION ---\n";
                result += selection + "\n\n";
            }
            
            if (string.IsNullOrEmpty(pages) && string.IsNullOrEmpty(objects) && string.IsNullOrEmpty(selection))
            {
                result += "(没有选中任何对象，或 selectionset Action 返回空)\n";
                result += "\n提示：请在图纸上选中一个对象后再运行此脚本\n";
            }
        }
        catch (System.Exception ex)
        {
            result += "\n=== 错误 ===\n";
            result += ex.GetType().Name + ": " + ex.Message + "\n";
            result += ex.StackTrace + "\n";
        }
        
        System.Windows.Forms.MessageBox.Show(
            result,
            "Dump Selected Object",
            System.Windows.Forms.MessageBoxButtons.OK,
            System.Windows.Forms.MessageBoxIcon.Information);
    }

    [DeclareUnregister]
    public void OnUnregister()
    {
    }
}
