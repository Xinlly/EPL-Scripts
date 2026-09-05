// ContextMenuHelloWorld.cs
// EPLAN Electric P8 2.9 脚本 - 图纸右键菜单 Hello World
// 加载后，在图纸上选中对象右键，菜单中会出现「Hello World」选项
// 对话框名：Editor.Ged

public class ContextMenuHelloWorld
{
    [DeclareRegister]
    public void OnRegister()
    {
        // 注册图纸右键菜单
        Eplan.EplApi.Gui.ContextMenuLocation oLoc = 
            new Eplan.EplApi.Gui.ContextMenuLocation();
        oLoc.DialogName = "Editor";
        oLoc.ContextMenuName = "Ged";

        Eplan.EplApi.Gui.ContextMenu oMenu = new Eplan.EplApi.Gui.ContextMenu();
        oMenu.AddMenuItem(
            oLoc,
            "Hello World",
            "ContextMenuHello",
            false,
            true);   // 放在分隔线之后（菜单底部区域）
    }

    [DeclareAction("ContextMenuHello")]
    public void Run()
    {
        System.Windows.Forms.MessageBox.Show(
            "Hello from right-click menu!",
            "Hello World",
            System.Windows.Forms.MessageBoxButtons.OK,
            System.Windows.Forms.MessageBoxIcon.Information);
    }

    [DeclareUnregister]
    public void OnUnregister()
    {
    }
}
