// DumpSelectedObject.cs
// 输出选中对象的基本信息（类型、ID、属性等）
// 用法：选中图纸上的一个对象 → 右键 → Dump Selected Object
// 或者：实用工具 → Dump Selected Object

public class DumpSelectedObject
{
    [DeclareRegister]
    public void OnRegister()
    {
        // 注册到图纸右键菜单（Editor.Ged）
        Eplan.EplApi.Gui.ContextMenuLocation oLoc = 
            new Eplan.EplApi.Gui.ContextMenuLocation();
        oLoc.DialogName = "Editor.Ged";
        oLoc.ContextMenuName = "1000";

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
            // 获取选中对象
            Eplan.EplApi.HEServices.SelectionSet selSet = 
                new Eplan.EplApi.HEServices.SelectionSet();
            
            if (!selSet.IsOnlyOneObjectSelected)
            {
                result += "状态: 没有选中对象 或 选中了多个对象\n";
                result += "IsOnlyOneObjectSelected: " + selSet.IsOnlyOneObjectSelected.ToString() + "\n";
                result += "IsPageSelected: " + selSet.IsPageSelected.ToString() + "\n";
                ShowResult(result);
                return;
            }
            
            // 获取第一个选中对象
            object selObj = selSet.GetSelectedObject();
            
            if (selObj == null)
            {
                result += "GetSelectedObject() 返回 null\n";
                ShowResult(result);
                return;
            }
            
            // 基本信息
            result += "对象类型: " + selObj.GetType().FullName + "\n";
            result += "ToString(): " + selObj.ToString() + "\n\n";
            
            // 尝试转换为 StorableObject 获取更多信息
            Eplan.EplApi.DataModel.StorableObject storable = 
                selObj as Eplan.EplApi.DataModel.StorableObject;
            
            if (storable != null)
            {
                result += "--- StorableObject 信息 ---\n";
                result += "ObjectIdentifier: " + storable.ObjectIdentifier.ToString() + "\n";
                result += "TypeIdentifier: " + storable.TypeIdentifier.ToString() + "\n";
                result += "DatabaseIdentifier: " + storable.DatabaseIdentifier.ToString() + "\n";
                result += "IsLocked: " + storable.IsLocked.ToString() + "\n";
                result += "IsReadOnly: " + storable.IsReadOnly.ToString() + "\n";
                result += "IsValid: " + storable.IsValid.ToString() + "\n";
                result += "IsTransient: " + storable.IsTransient.ToString() + "\n";
                
                if (storable.Project != null)
                {
                    result += "Project: " + storable.Project.ProjectName + "\n";
                }
                
                result += "\n--- 属性预览（前20个） ---\n";
                try
                {
                    Eplan.EplApi.DataModel.PropertyValueList props = storable.Properties;
                    if (props != null)
                    {
                        int count = 0;
                        foreach (Eplan.EplApi.DataModel.PropertyValue prop in props)
                        {
                            if (count >= 20)
                            {
                                result += "  ... (更多属性省略)\n";
                                break;
                            }
                            result += "  " + prop.Id.ToString() + " = " + prop.ToString() + "\n";
                            count++;
                        }
                        if (count == 0)
                        {
                            result += "  (属性列表为空)\n";
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    result += "  属性读取失败: " + ex.Message + "\n";
                }
            }
            else
            {
                result += "(不是 StorableObject 类型，跳过属性读取)\n";
            }
        }
        catch (System.Exception ex)
        {
            result += "\n=== 错误 ===\n";
            result += ex.GetType().Name + ": " + ex.Message + "\n";
            result += ex.StackTrace + "\n";
        }
        
        ShowResult(result);
    }

    private void ShowResult(string text)
    {
        // 输出到消息框
        System.Windows.Forms.MessageBox.Show(
            text,
            "Dump Selected Object",
            System.Windows.Forms.MessageBoxButtons.OK,
            System.Windows.Forms.MessageBoxIcon.Information);
    }

    [DeclareUnregister]
    public void OnUnregister()
    {
    }
}
