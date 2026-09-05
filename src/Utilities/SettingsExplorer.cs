// SettingsExplorer.cs
// 适用 EPLAN 版本：EPLAN Electric P8 2.9
// 设置路径查看器 - 遍历 EPLAN 所有设置路径并输出到日志
// 用法：实用工具 → 设置路径查看器
// 日志输出：$(MD_SCRIPTS)\EPL-Scripts\.log\yyyy-MM-dd.log
// 说明：脚本模式可用（Base + ApplicationFramework + Gui）

using Eplan.EplApi.Base;
using Eplan.EplApi.Scripting;

public class SettingsExplorer
{
    [DeclareRegister]
    public void Register()
    {
        new Eplan.EplApi.Gui.Menu().AddMenuItem(
            "设置路径查看器",
            "SettingsExplorer",
            "执行设置路径查看器，导出所有设置路径",
            35380,  // 实用工具菜单
            1,
            false,
            false);
    }

    [DeclareAction("SettingsExplorer")]
    public void Run(ActionCallingContext oActionCallingContext)
    {
        string strLogPath = GetLogPath();
        System.IO.StreamWriter writer = new System.IO.StreamWriter(strLogPath, true, System.Text.Encoding.UTF8);
        writer.WriteLine("=== Settings Explorer ===");
        writer.WriteLine("时间: " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        
        try
        {
            SettingNode oNode = new SettingNode();
            oNode.Set("USER", SettingNode.Level.User);
            
            writer.WriteLine("");
            writer.WriteLine("--- USER 设置节点 ---");
            WriteNode(writer, oNode, "", 0);
            
            writer.WriteLine("");
            writer.WriteLine("=== 导出完成 ===");
        }
        catch (System.Exception ex)
        {
            writer.WriteLine("错误: " + ex.Message);
            writer.WriteLine("堆栈: " + ex.StackTrace);
        }
        
        writer.Close();
        
        new Eplan.EplApi.Base.BaseException(
            "设置路径查看器",
            "所有设置路径已导出到:\n" + strLogPath,
            0,
            Exception.Level.Information);
    }

    private void WriteNode(System.IO.StreamWriter writer, SettingNode oParentNode, string strParentPath, int depth)
    {
        string strIndent = new string(' ', depth * 2);
        
        // 输出当前节点下的所有设置
        int nSettingCount = oParentNode.GetCountOfSettings();
        for (int i = 0; i < nSettingCount; i++)
        {
            string strSettingPath = oParentNode.GetListOfSettings()[i];
            string strFullPath = strParentPath + "." + strSettingPath;
            if (strFullPath.StartsWith("."))
                strFullPath = strFullPath.Substring(1);
            
            // 读取设置类型和值
            try
            {
                ISettings.SettingType eType = oParentNode.GetTypeOfSetting(strSettingPath);
                string strType = eType.ToString();
                string strValue = "";
                
                try
                {
                    switch (eType)
                    {
                        case ISettings.SettingType.String:
                            strValue = oParentNode.GetStringSetting(strSettingPath);
                            break;
                        case ISettings.SettingType.Bool:
                            strValue = oParentNode.GetBoolSetting(strSettingPath).ToString();
                            break;
                        case ISettings.SettingType.Numeric:
                            strValue = oParentNode.GetNumericSetting(strSettingPath).ToString();
                            break;
                        case ISettings.SettingType.Double:
                            strValue = oParentNode.GetDoubleSetting(strSettingPath).ToString();
                            break;
                        case ISettings.SettingType.MultiLangString:
                            strValue = "[多语言字符串]";
                            break;
                        default:
                            strValue = "[" + eType.ToString() + "]";
                            break;
                    }
                }
                catch
                {
                    strValue = "[读取失败]";
                }
                
                writer.WriteLine(strIndent + "  " + strSettingPath + " (" + strType + ") = " + strValue);
            }
            catch
            {
                writer.WriteLine(strIndent + "  " + strSettingPath + " [类型未知]");
            }
        }
        
        // 递归子节点
        int nNodeCount = oParentNode.GetCountOfNodes();
        for (int i = 0; i < nNodeCount; i++)
        {
            string strNodeName = oParentNode.GetListOfNodes()[i];
            string strNodePath = strParentPath + "." + strNodeName;
            if (strNodePath.StartsWith("."))
                strNodePath = strNodePath.Substring(1);
            
            writer.WriteLine(strIndent + "[" + strNodeName + "]");
            
            SettingNode oSubNode = oParentNode.GetSubNode(strNodeName);
            WriteNode(writer, oSubNode, strNodePath, depth + 1);
        }
    }

    private string GetLogPath()
    {
        string strMdScripts = "";
        try
        {
            strMdScripts = PathMap.SubstitutePath("$(MD_SCRIPTS)");
        }
        catch
        {
            strMdScripts = System.IO.Directory.GetCurrentDirectory();
        }
        
        string strDir = strMdScripts + "\\EPL-Scripts\\.log";
        if (!System.IO.Directory.Exists(strDir))
            System.IO.Directory.CreateDirectory(strDir);
        
        return strDir + "\\" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".log";
    }

    [DeclareUnregister]
    public void Unregister()
    {
    }
}
