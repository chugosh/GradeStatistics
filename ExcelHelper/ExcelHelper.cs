using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Aspose.Cells;
using GradePackage;

namespace ExcelUnity
{
    public static class ExcelHelper
    {
        static ExcelHelper()
        {

        }

        public static bool ToExcel(IList<IList<string>> datas, string name, bool isOpen = false)
        {
            return ToExcelPrivate(name, delegate (Worksheet worksheet)
            {
                for (int i = 0; i < datas.Count; i++)
                {
                    for (int j = 0; j < datas[i].Count; j++)
                    {
                        worksheet.Cells[i, j].PutValue(datas[i][j]);
                    }
                }
                //((ExcelRangeBase)worksheet.Cells["A1"]).LoadFromArrays((IEnumerable<object[]>)datas.Select((IList<string> data) => data.ToArray()));
            }, isOpen);
        }


        private static bool ToExcelPrivate(string name, Action<Worksheet> action, bool isOpen)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel(*.xlsx)|*.xlsx",
                    RestoreDirectory = true,
                    FileName = $"{name}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
                if (DialogResult.OK != saveFileDialog.ShowDialog())
                {
                    return false;
                }
                return ToExcelPrivate(name, saveFileDialog.FileName, action, isOpen);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static IList<IList<object>> GetExcelFiles(string path)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                foreach (var file in directoryInfo.GetFiles())
                {
                    var list = new List<ClassGradeEntity>();
                    var rank = new RankGradeEntity();
                    CalculationFromExcel(file.FullName, out var classGrade);
                    rank.list.Add(classGrade);
                    list.Add(classGrade);
                    //return FromExcel(file.FullName);
                }
            }
            catch(Exception e)
            {

            }

            return null;
        }

        private static bool ToExcelPrivate(string name, string path, Action<Worksheet> action, bool isOpen)
        {
            try
            {
                if (!path.EndsWith(".xlsx"))
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    path = $"{path}\\{name}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                }
                if (!File.Exists(path))
                {
                    FileStream fs1 = new FileStream(path, FileMode.Create, FileAccess.Write);//创建写入文件 
                    fs1.Close();
                }
                Workbook val = new Workbook(path);
                
                try
                {
                    Worksheet obj = val.Worksheets[0];
                    action(obj);
                    val.Save(path);
                    if (isOpen)
                    {
                        Process.Start(path);
                    }
                    return true;
                }
                finally
                {
                    ((IDisposable)val)?.Dispose();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// 读取Excel数据
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="sheetName">sheet名称</param>
        /// <returns></returns>
        public static IList<IList<object>> FromExcel(string fileName, string sheetName = "Sheet1")
        {
            List<IList<object>> list = new List<IList<object>>();
            if (!File.Exists(fileName))
            {
                return list;
            }
            try
            {
                Workbook val = new Workbook(fileName);
                try
                {
                    Worksheet val2 = val.Worksheets[sheetName];
                    if (val2 == null)
                    {
                        return list;
                    }
                    var cells = val2.Cells.ExportArray(val2.Cells.MinRow, val2.Cells.MinColumn, val2.Cells.MaxRow + 1, val2.Cells.MaxColumn + 1);

                    for (int i = val2.Cells.MinRow; i <= val2.Cells.MaxRow; i++)
                    {
                        List<object> list2 = new List<object>();
                        for (int j = val2.Cells.MinColumn; j <= val2.Cells.MaxColumn; j++)
                        {
                            object value = cells[i, j];
                            list2.Add(value);
                        }
                        list.Add(list2);
                    }
                    return list;
                }
                finally
                {
                    ((IDisposable)val)?.Dispose();
                }
            }
            catch (Exception ex)
            {
                return list;
            }
        }

        /// <summary>
        /// 直接用公式计算数据
        /// 每个班的成绩
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="sheetName"></param>
        public static void CalculationFromExcel(string fileName, out ClassGradeEntity classGrade, string sheetName = "Sheet1")
        {
            classGrade = new ClassGradeEntity()
            {
                Id = "1"
            };
            if (!File.Exists(fileName))
                return;
            try
            {
                var workBook = new Workbook(fileName);
                var workSheet = workBook.Worksheets[sheetName];
                if (workSheet == null) return;

                for(var c = 2; c <= workSheet.Cells.MaxColumn; c++)
                {
                    var start = workSheet.Cells[1, c].Name;
                    var end = workSheet.Cells[workSheet.Cells.MaxRow, c].Name;
                    var sumGradeformula = $"SUM({start}:{end})";
                    var excellent = $"COUNTIFS({start}:{end},\">={GradeEnum.Grade.优秀}\")";
                    var good = $"COUNTIFS({start}:{end},\">={GradeEnum.Grade.良好}\",{start}:{end},\"<{GradeEnum.Grade.优秀}\")";
                    var pass = $"COUNTIFS({start}:{end},\">={GradeEnum.Grade.及格}\",{start}:{end},\"<{GradeEnum.Grade.良好}\")";
                    var sub = new SubjectEntity
                    {
                        Id = Path.GetFileNameWithoutExtension(fileName),
                        Name = workSheet.Cells[0, c].Value.ToString(),
                        SumGrade = (double)workSheet.CalculateFormula(sumGradeformula),
                        Sum = workSheet.Cells.MaxRow - 1,
                        Excellent = (double)workSheet.CalculateFormula(excellent),
                        Good = (double)workSheet.CalculateFormula(good),
                        Pass =(double)workSheet.CalculateFormula(pass),
                    };
                    classGrade.SubjectList.Add(sub);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
