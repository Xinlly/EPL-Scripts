//#################################################################################################################################################
// ESS - PageNavi_ContextMenu_OpenFolders
//#################################################################################################################################################
// Erweiterung des Kontextmenüs im Seitennavigator zum schnellen Öffnen der Verzeichnisse $(P), $(DOC) und $(IMG)
// EPLAN GmbH & Co. KG
//#################################################################################################################################################
//#################################################################################################################################################

using System.IO;
using System.Windows.Forms;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.Gui;
using Eplan.EplApi.Base;
using Eplan.EplApi.Scripting;

//[C#]
public class PageNavi_ContextMenu_OpenFolders
{
    #region define global variables
    public static ISOCode.Language global_GuiLanguage = new Languages().GuiLanguage.GetNumber();
    #endregion

    [DeclareAction("OpenFolder")]
    public void XOpenFolder(string FolderName)
    {
        if (FolderName != string.Empty)
        {
            if (FolderName.StartsWith("$("))
            {
                FolderName = Eplan.EplApi.Base.PathMap.SubstitutePath(FolderName);
            }

            DirectoryInfo oDI = new DirectoryInfo(FolderName);
            if (oDI.Exists)
            {
                //Exportfile öffnen
                System.Diagnostics.ProcessStartInfo proc = new System.Diagnostics.ProcessStartInfo();
                proc.FileName = "explorer.exe";
                proc.Arguments = FolderName;
                System.Diagnostics.Process.Start(proc);
            }
        }
    }

    [DeclareAction("sncTestFun")]
    public void XSncTestFun()
    {
        new Decider().Decide(EnumDecisionType.eOkDecision, "MyFunction was called!", "VerySimpleScript", EnumDecisionReturn.eOK, EnumDecisionReturn.eOK);
        return;
    }

    [DeclareAction("sncNewPart")]
    public void XSncNewPart()
    {
        string partName = "MyTestPart-!@#$%^&*()_+-=[]{}|;:',.<>/?";
        MDPartsManagement oPartsManagement = new MDPartsManagement();
        MDPartsDatabase partsDatabase = oPartsManagement.OpenDatabase();

        var bdName = MDPartsManagement.SelectedPartsDatabaseAsString;
        new Decider().Decide(EnumDecisionType.eOkDecision, bdName, "DB", EnumDecisionReturn.eOK, EnumDecisionReturn.eOK);
        if (!partsDatabase.ExistsPart(partName))
        {       
            var part = partsDatabase.AddPart(partName, "1");
        }
        partsDatabase.Close();
        return;
    }

    [DeclareMenu()]
    public void CreateMenu()
    {

        #region language-depending menu-texts
        MultiLangString oMLSMenuText1 = new MultiLangString();
        oMLSMenuText1.AddString(ISOCode.Language.L_en_US, "Open $(P) folder...");
        oMLSMenuText1.AddString(ISOCode.Language.L_zh_CN, "打开 $(P) 目录...");
        string _sGui_MenuText1 = oMLSMenuText1.GetStringToDisplay(global_GuiLanguage);
        if (String.IsNullOrEmpty(_sGui_MenuText1))
        {
            //if actual GUI-language is not defined in multi-language-string, use en_US-text-version
            _sGui_MenuText1 = "Open $(P) folder...";
        }

        MultiLangString oMLSMenuText2 = new MultiLangString();
        oMLSMenuText2.AddString(ISOCode.Language.L_en_US, "Test");
        oMLSMenuText2.AddString(ISOCode.Language.L_zh_CN, "测试");
        string _sGui_MenuText2 = oMLSMenuText2.GetStringToDisplay(global_GuiLanguage);
        if (String.IsNullOrEmpty(_sGui_MenuText2))
        {
            //if actual GUI-language is not defined in multi-language-string, use en_US-texestt-version
            _sGui_MenuText2 = "Test";
        }

        MultiLangString oMLSMenuText3 = new MultiLangString();
        oMLSMenuText3.AddString(ISOCode.Language.L_en_US, "New Part (Test only)");
        oMLSMenuText3.AddString(ISOCode.Language.L_zh_CN, "新建零件(仅测试)");
        string _sGui_MenuText3 = oMLSMenuText3.GetStringToDisplay(global_GuiLanguage);
        if (String.IsNullOrEmpty(_sGui_MenuText3))
        {
            //if actual GUI-language is not defined in multi-language-string, use en_US-text-version
            _sGui_MenuText3 = "New Part(Test only)";
        }
        #endregion

        #region expan context menues
        //expand context-menu in page-navigator (tree-view)
        ContextMenuLocation oCtxLoc = new ContextMenuLocation();     
        oCtxLoc.DialogName = "PmPageObjectTreeDialog";
        oCtxLoc.ContextMenuName = "1007";

        Eplan.EplApi.Gui.ContextMenu oCTXMnu = new Eplan.EplApi.Gui.ContextMenu();
        oCTXMnu.AddMenuItem(oCtxLoc, _sGui_MenuText1, "OpenFolder /FolderName:$(PROJECTPATH)", true, false);
        oCTXMnu.AddMenuItem(oCtxLoc, _sGui_MenuText2, "sncTestFun", true, false);
        oCTXMnu.AddMenuItem(oCtxLoc, _sGui_MenuText3, "sncNewPart", false, false);
        #endregion
    }
   
}
