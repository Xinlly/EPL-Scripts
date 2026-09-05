// ShowAllActions.cs
// 列出当前会话中已注册的 Action（脚本注册的 + 系统常用）
// 说明：EPLAN API 没有直接枚举所有 Action 的方法，这里列出：
//   1. 当前脚本自己注册的 Action
//   2. 常用系统 Action 列表（来自官方文档）
// 加载后在「实用工具」菜单增加「Show All Actions」选项

public class ShowAllActions
{
    // 常用系统 Action 列表（精选，脚本开发高频使用）
    private string[] _commonActions = new string[]
    {
        // 项目操作
        "XprjActionNew",
        "XprjActionOpen",
        "XprjActionClose",
        "XprjActionSave",
        "XprjActionSaveAs",
        "XprjActionBackup",
        "XprjActionRestore",
        
        // 页操作
        "XPgActionNew",
        "XPgActionDelete",
        "XPgActionCopy",
        "XPgActionNumber",
        "XPgActionExportDXFDWG",
        "XPgActionImportDXFDWG",
        "XPgActionPrint",
        "XPgActionExportPDF",
        
        // 编辑操作
        "GfDlgMgrActionIGfWind /function:Copy",
        "GfDlgMgrActionIGfWind /function:Paste",
        "GfDlgMgrActionIGfWind /function:Delete",
        "GfDlgMgrActionIGfWind /function:Undo",
        "GfDlgMgrActionIGfWind /function:Redo",
        
        // 查找替换
        "XSearchDlgAction",
        "XGedStartInteractionAction /Name:XFindTextInteraction",
        
        // 数据
        "selectionset",
        "XSettingsAction",
        "XPartSelectionStartAction",
        
        // 报表
        "XRepActionStart",
        "XRepActionLabel",
        "XRepActionPartsList",
        "XRepActionConnectionList",
        "XRepActionDeviceList",
        "XRepActionCableList",
        "XRepActionTerminalStripDesignation",
        
        // 设备/端子/电缆
        "XNmbActionStart",
        "XCblActionNumbering",
        "XTrmActionNumber",
        "XDevActionEdit",
        
        // 宏
        "XMacroActionCreate",
        "XMacroActionInsert",
        "XGedInsertMacroAction",
        
        // PLC
        "XPlcActionImport",
        "XPlcActionExport",
        "XPlcIoDataDlg",
        
        // 其他
        "XMlsActionEdit",
        "XTranslateAction",
        "XCheckAction",
        "UpdateConnections"
    };

    [DeclareRegister]
    public void OnRegister()
    {
    }

    [DeclareMenu]
    public void RegisterMenu()
    {
        Eplan.EplApi.Gui.Menu oMenu = new Eplan.EplApi.Gui.Menu();
        oMenu.AddMenuItem(
            "Show All Actions",
            "ShowAllActions",
            "列出已注册的 Action",
            35284,
            0,
            false,
            false);
    }

    [DeclareAction("ShowAllActions")]
    public void Run()
    {
        string msg = "=== 本脚本注册的 Action ===\n";
        msg += "  ShowAllActions (此脚本)\n";
        msg += "\n=== 常用系统 Action（精选） ===\n";
        
        foreach (string action in _commonActions)
        {
            // 尝试用 ActionManager 验证是否存在
            bool exists = false;
            try
            {
                Eplan.EplApi.ApplicationFramework.ActionManager mgr = 
                    new Eplan.EplApi.ApplicationFramework.ActionManager();
                Eplan.EplApi.ApplicationFramework.Action act = mgr.FindAction(action);
                exists = (act != null);
            }
            catch
            {
                // 找不到就当不存在
            }
            
            string status = exists ? "  [✓]" : "  [?]";
            msg += status + " " + action + "\n";
        }
        
        msg += "\n[✓] = 已确认存在  [?] = 待验证\n";
        msg += "完整列表请参考官方文档：EPLAN 2.9 Action List";
        
        System.Windows.Forms.MessageBox.Show(
            msg,
            "Show All Actions",
            System.Windows.Forms.MessageBoxButtons.OK,
            System.Windows.Forms.MessageBoxIcon.Information);
    }

    [DeclareUnregister]
    public void OnUnregister()
    {
    }
}
