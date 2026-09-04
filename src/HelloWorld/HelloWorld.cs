// HelloWorld.cs
// EPLAN Electric P8 2.9 脚本 - Hello World
// 用法：实用工具 > 脚本 > 加载脚本 > 选择此文件
// 加载后自动弹出消息框（[Start] 特性触发）

using Eplan.EplApi.Scripting;
using System.Windows.Forms;

public class HelloWorld
{
    [Start]
    public void Run()
    {
        MessageBox.Show(
            "Hello, EPLAN!",
            "Hello World",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
