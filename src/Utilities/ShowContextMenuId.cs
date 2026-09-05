// ShowContextMenuId.cs
// 适用 EPLAN 版本：EPLAN Electric P8 2.9
// 显示/隐藏右键菜单的ID（对话框名 + 菜单ID）
// 加载后开启显示，卸载后关闭
// 用法：加载此脚本 → 在EPLAN中右键任何菜单 → 菜单顶部会显示对话框名和ID

public class ShowContextMenuId
{
    [DeclareRegister]
    public void OnRegister()
    {
        Eplan.EplApi.Base.Settings oSettings = new Eplan.EplApi.Base.Settings();
        oSettings.SetBoolSetting("USER.EnfMVC.ContextMenuSetting.ShowIdentifier", true, 0);
        
        System.Windows.Forms.MessageBox.Show(
            "右键菜单ID显示已开启\r\n在EPLAN中右键即可看到对话框名和菜单ID",
            "ShowContextMenuId",
            System.Windows.Forms.MessageBoxButtons.OK,
            System.Windows.Forms.MessageBoxIcon.Information);
    }

    [DeclareUnregister]
    public void OnUnregister()
    {
        Eplan.EplApi.Base.Settings oSettings = new Eplan.EplApi.Base.Settings();
        oSettings.SetBoolSetting("USER.EnfMVC.ContextMenuSetting.ShowIdentifier", false, 0);
        
        System.Windows.Forms.MessageBox.Show(
            "右键菜单ID显示已关闭",
            "ShowContextMenuId",
            System.Windows.Forms.MessageBoxButtons.OK,
            System.Windows.Forms.MessageBoxIcon.Information);
    }
}
