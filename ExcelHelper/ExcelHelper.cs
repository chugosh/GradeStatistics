using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Aspose.Cells;
using GradePackage;

namespace ExcelUnity
{
    public static class ExcelHelper
    {
        static ExcelHelper()
        {
            //t.License l = new Excel.License();
            //l.SetLicense("Aid/License.lic");
        }

        public static bool ToExcel<T>(string destPath, List<T> datas, string name, bool isOpen = false)
        {
            var dataTable = Utils.ToDataTable<T>(datas);
            return ToExcelPrivate(name, destPath, (Worksheet worksheet) =>
            {
                //worksheet.Cells.ImportData(dataTable, worksheet.Cells.MaxRow + 1, 0, null);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    var row = worksheet.Cells.MaxRow + 1;
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        var s = dataTable.Rows[i][j];
                        worksheet.Cells[row, j].PutValue(dataTable.Rows[i][j]);
                    }
                }
                //((ExcelRangeBase)worksheet.Cells["A1"]).LoadFromArrays((IEnumerable<object[]>)datas.Select((IList<string> data) => data.ToArray()));
            }, isOpen);// ? dataTable : null;
        }

        public static DataTable ExcelToDataTable(Cells cells)
        {
            return cells.ExportDataTable(3, 0, cells.MaxRow, cells.MaxColumn);
        }

        /// <summary>
        /// 返回所有年级的所有班级成绩 
        /// 年级实体
        /// </summary>
        /// <param name="path">年级文件夹路径：六个年级的文件</param>
        /// <returns>所有年级的list</returns>
        public static List<GradeEntity> GetExcelFiles(string path)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                var gradeList = new List<GradeEntity>();
                //file 每一个年级文件
                foreach (var file in directoryInfo.GetFiles())
                {
                    var id = CalculationFromExcel(file.FullName, out var classGrade);
                    var grade = new GradeEntity()
                    {
                        Id = file.Name,
                    };
                    grade.ClassLists = classGrade;
                    gradeList.Add(grade);
                }
                //grade.ClassLists = class 
                return gradeList;
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
                //if (!path.EndsWith(".xlsx"))
                //{
                //    if (!Directory.Exists(path))
                //    {
                //        Directory.CreateDirectory(path);
                //    }
                //    path = $@"{path}\{name}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                //}
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
                    val.Save(path, SaveFormat.Auto);
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
                Console.WriteLine(ex.Message);
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
        /// 一个年级所有班的各科成绩
        /// </summary>
        /// <param name="fileName">一个年级的表格</param>
        /// <param name="sheetName"></param>
        /// classGrade 一个年级所有班级list
        public static string CalculationFromExcel(string fileName, out List<ClassEntity> classGrade, string sheetName = "Sheet1")
        {
            classGrade = new List<ClassEntity>();
            if (!File.Exists(fileName))
                return string.Empty;
            try
            {
                using (var workBook = new Workbook(fileName))
                {
                    var workSheet = workBook.Worksheets[sheetName];
                    if (workSheet == null) return string.Empty;
                    //班级id 和 班级人数的 字典
                    var dic = new Dictionary<string, int>();
                    for(var r = 1; r < workSheet.Cells.Rows.Count; r++)
                    {
                        //第一列list
                        //取第七位
                        var i = workSheet.Cells[r, 0].Value.ToString();
                        var id = workSheet.Cells[r, 0].Value.ToString().Substring(6,1);
                        if(dic.TryGetValue(id, out _))
                            dic[id]++;
                        else
                        {
                            classGrade.Add(new ClassEntity() { Id = id});
                            dic.Add(id, 1);
                        }
                    }

                    for (var c = 2; c <= workSheet.Cells.MaxColumn; c++)
                    {
                        var start = 1;
                        var end = 0;
                        var startCell = workSheet.Cells[start,c].Name.ToString();
                        double maxExcellentRate = 0;
                        double maxGoodRate = 0;
                        double maxPassRate = 0;
                        double maxAver = 0;
                        var name = workSheet.Cells[0, c].Value.ToString();
                        //所有班级一个科目的成绩
                        //每个key是一个班
                        foreach (var key in dic.Keys)
                        {
                            end += dic[key];
                            var endCell = workSheet.Cells[end, c].Name.ToString();// 
                            
                            var sumGradeFormula = $"SUM({startCell}:{endCell})";
                            var excellentFormula = $"COUNTIFS({startCell}:{endCell},\">={(int)GradeEnum.Grade.优秀}\")";
                            var goodFormula = $"COUNTIFS({startCell}:{endCell},\">={(int)GradeEnum.Grade.良好}\",{startCell}:{endCell},\"<{(int)GradeEnum.Grade.优秀}\")";
                            var passFormula = $"COUNTIFS({startCell}:{endCell},\">={(int)GradeEnum.Grade.及格}\",{startCell}:{endCell},\"<{(int)GradeEnum.Grade.良好}\")";

                            var e = (double)workSheet.CalculateFormula(excellentFormula);
                            var g = (double)workSheet.CalculateFormula(goodFormula);
                            var p = (double)workSheet.CalculateFormula(passFormula);
                            var sub = new SubjectEntity
                            {
                                Id = key,
                                Name = name,
                                Sum = dic[key],
                                SumGrade = (double)workSheet.CalculateFormula(sumGradeFormula),
                                Excellent = e,
                                Good = g,
                                Pass = p,
                                ExcellentRate = Math.Round(e / dic[key], 2),
                                GoodRate = Math.Round(g / dic[key], 2),
                                PassRate = Math.Round(p / dic[key], 2),
                            };

                            maxExcellentRate = maxExcellentRate > sub.ExcellentRate ? maxExcellentRate : sub.ExcellentRate;
                            maxGoodRate = maxGoodRate > sub.GoodRate ? maxGoodRate : sub.GoodRate;
                            maxPassRate = maxPassRate > sub.PassRate ? maxPassRate : sub.PassRate;
                            maxAver = maxAver > sub.Average ? maxAver : sub.Average;

                            classGrade.Where(cg => cg.Id == key).FirstOrDefault().SubjectList.Add(sub);
                            start = end + 1;
                            startCell = workSheet.Cells[start, c].Name.ToString();
                        }

                        foreach(var g in classGrade)
                        {
                            var sub = g.SubjectList.Where(s => s.Name == name).FirstOrDefault();
                            sub.ExcellentRate /= Math.Round(maxExcellentRate, 2);
                            sub.GoodRate /= Math.Round(maxGoodRate, 2);
                            sub.PassRate /= Math.Round(maxPassRate, 2);
                            sub.AverageRate = sub.Average / Math.Round(maxAver,2);
                        }
                    }
                    return "";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return string.Empty;
            }
        }

        public static void OldWay(string filename)
        {
            //foreach (var key in dic.Keys)
            //{
            //    var ce = new ClassEntity() { Id=key, };
            //    var end = dic[key];
            //    for (var c = 2; c <= workSheet.Cells.MaxColumn; c++)
            //    {
            //        var sumGradeFormula = $"SUM({start}:{end})";
            //        var excellentFormula = $"COUNTIFS({start}:{end},\">={(int)GradeEnum.Grade.优秀}\")";
            //        var goodFormula = $"COUNTIFS({start}:{end},\">={(int)GradeEnum.Grade.良好}\",{start}:{end},\"<{(int)GradeEnum.Grade.优秀}\")";
            //        var passFormula = $"COUNTIFS({start}:{end},\">={(int)GradeEnum.Grade.及格}\",{start}:{end},\"<{(int)GradeEnum.Grade.良好}\")";

            //        var sub = new SubjectEntity
            //        {
            //            Id = Path.GetFileNameWithoutExtension(fileName),
            //            Name = workSheet.Cells[0, c].Value.ToString(),
            //            SumGrade = (double)workSheet.CalculateFormula(sumGradeFormula),
            //            Sum = end,
            //            Excellent = (double)workSheet.CalculateFormula(excellentFormula),
            //            Good = (double)workSheet.CalculateFormula(goodFormula),
            //            Pass = (double)workSheet.CalculateFormula(passFormula),
            //        };

            //        maxExcellentRate = maxExcellentRate > sub.ExcellentRate ? maxExcellentRate : sub.ExcellentRate;
            //        maxGoodRate = maxGoodRate > sub.GoodRate ? maxGoodRate : sub.GoodRate;
            //        maxPassRate = maxPassRate > sub.PassRate ? maxPassRate : sub.PassRate;
            //        maxAver = maxAver > sub.Average ? maxPassRate : sub.Average;

            //        ce.SubjectList.Add(sub);
            //    }
            //    classGrade.Add(ce);
            //    start = end + 1;
            //}
        }

    }
}
