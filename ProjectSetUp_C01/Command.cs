#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;


using Excel = Microsoft.Office.Interop.Excel;


#endregion

namespace ProjectSetUp_C01
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Step 1: put any code needed for the form here

            // Step 2: open form
            MyForm currentForm = new MyForm()
            {
                Width = 500,
                Height = 450,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            currentForm.ShowDialog();

            // Step 3: get form data and do something
            if(currentForm.DialogResult == false)
            {
                return Result.Cancelled;
            }

            // do somenthing 
            string textboxresult = currentForm.GetTextBoxValue();


            //Get Levels metric 
            string excelFile = currentForm.GetTextBoxValue();

            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook excelWb = excelApp.Workbooks.Open(excelFile);

            //Get Level data from Excel
            int levels = GetExcelSheetByName(excelWb, "RAA_Level_2_Module_01_Challenge");
            Excel.Worksheet excelWslvls = excelApp.Worksheets.Item["RAA_Level_2_Module_01_Challenge"];
            Excel.Range excelRngName = excelWslvls.UsedRange;
            int rowCountLN = excelRngName.Rows.Count;

            List<LevelData> levelsList = new List<LevelData>();

            //Imperial 
            if (currentForm.GetRadioButton1())
            {
                for (int i = 2; i <= rowCountLN; i++)
                {
                    Excel.Range cellName = excelWslvls.Cells[i, 1];
                    Excel.Range cellElev = excelWslvls.Cells[i, 2];

                    LevelData levelData = new LevelData();
                    levelData.levelName = cellName.Value.ToString();
                    levelData.levelElev = cellElev.Value;

                    levelsList.Add(levelData);
                }
            }

            //Metric 
            else if (currentForm.GetRadioButton2())
            {
                for (int i = 2; i <= rowCountLN; i++)
                {
                    Excel.Range cellName = excelWslvls.Cells[i, 1];
                    Excel.Range cellElev = excelWslvls.Cells[i, 3];

                    LevelData levelData = new LevelData();
                    levelData.levelName = cellName.Value.ToString();
                    levelData.levelElev = cellElev.Value;

                    levelsList.Add(levelData);
                }
            }

            //Not necessary 
            else if (currentForm.GetRadioButton1() && currentForm.GetCheckBox2())
            {
                TaskDialog.Show("Revit Api-Academy", "Select only one radio button");
            }
            

            

            //Create Floor Plans


            //Create Ceiling Plans



            //Select FloorPlan type - FP or RCP
            FilteredElementCollector collectorFPV = new FilteredElementCollector(doc);
            collectorFPV.OfClass(typeof(ViewFamilyType));

            ViewFamilyType curVFT = null;
            ViewFamilyType curRCPVFT = null;

            foreach (ViewFamilyType curElem in collectorFPV)
            {
                if (curElem.ViewFamily == ViewFamily.FloorPlan)
                {
                    curVFT = curElem;
                }
                else if (curElem.ViewFamily == ViewFamily.CeilingPlan)
                {
                    curRCPVFT = curElem;
                }
            }

            //Modify Revit model - Create levels, views, and sheets
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Project Setup");

                //Create levels
                for (int i = 0; i < levelsList.Count; i++)
                {
                    LevelData curLevel = levelsList[i];

                    //Floor plan
                    if (currentForm.GetCheckBox1())
                    {
                        try
                        {
                            Level newLevel = Level.Create(doc, curLevel.levelElev); ////////////////


                            //Floor plan
                            ViewPlan curFP = ViewPlan.Create(doc, curVFT.Id, newLevel.Id);
                            curFP.Name = curLevel.levelName;

                            //RCP
                           
                        }
                        catch (Exception ex)
                        {
                            Debug.Print(ex.Message);
                        }
                    }

                    //RCP
                    else if (currentForm.GetCheckBox2())
                    {
                        Level newLevel = Level.Create(doc, curLevel.levelElev);

                        ViewPlan curRCP = ViewPlan.Create(doc, curRCPVFT.Id, newLevel.Id);
                        curRCP.Name = curLevel.levelName + " RCP";
                    }

                    else if (currentForm.GetCheckBox1() && currentForm.GetCheckBox2())
                    {
                        Level newLevel = Level.Create(doc, curLevel.levelElev); ////////////////


                        //Floor plan
                        ViewPlan curFP = ViewPlan.Create(doc, curVFT.Id, newLevel.Id);
                        curFP.Name = curLevel.levelName;

                        //RCP
                        ViewPlan curRCP = ViewPlan.Create(doc, curRCPVFT.Id, newLevel.Id);
                        curRCP.Name = curLevel.levelName + " RCP";
                    }

                    else
                    {
                        continue;
                    }
                    
                }

                t.Commit();


            }

            excelWb.Close();
            excelApp.Quit();

            // TaskDialog.Show("SBR", levelList.ToString());

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }

        //Methods
        public struct LevelData
        {
            public string levelName;
            public double levelElev;
        }

        // Get excel worksheet by name 
        internal int GetExcelSheetByName(Excel.Workbook excelWb, string name)
        {
            int count = excelWb.Worksheets.Count;
            int index = 1;

            for (int i = 1; i <= count; i++)
            {
                Excel.Worksheet ws = excelWb.Worksheets[i];
                if (ws.Name == name)
                {
                    index = ws.Index;
                }
            }
            return index;
        }






    }
}

