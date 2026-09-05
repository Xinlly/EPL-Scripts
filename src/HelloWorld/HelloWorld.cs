// HelloWorld.cs
// 适用 EPLAN 版本：EPLAN Electric P8 2.9
// EPLAN Electric P8 2.9 脚本 - Hello World
// 加载脚本时弹出提示，并在「实用工具」菜单添加「Hello World」菜单项

public class HelloWorld
{
    [DeclareRegister]
    public void OnRegister()
    {
        System.Windows.Forms.MessageBox.Show(
            "HelloWorld 脚本已加载！",
            "Hello World",
            System.Windows.Forms.MessageBoxButtons.OK,
            System.Windows.Forms.MessageBoxIcon.Information);
    }

    [DeclareMenu]
    public void RegisterMenu()
    {
        Eplan.EplApi.Gui.Menu oMenu = new Eplan.EplApi.Gui.Menu();
        oMenu.AddMenuItem(
            "Hello World",       // 菜单项显示名称
            "HelloWorld",        // 关联的 Action 名称
            "Hello World demo",  // 状态栏提示
            35284,               // 父菜单 ID（实用工具）
            0,                   // 位置
            false,               // 分隔符之前
            false);              // 分隔符之后
    }

    [DeclareAction("HelloWorld")]
    public void Run()
    {
        System.Windows.Forms.MessageBox.Show(
            "Hello, EPLAN!",
            "Hello World",
            System.Windows.Forms.MessageBoxButtons.OK,
            System.Windows.Forms.MessageBoxIcon.Information);
    }

    [DeclareUnregister]
    public void OnUnregister()
    {
        // 脚本卸载时调用（可用于清理资源）
    }
}
