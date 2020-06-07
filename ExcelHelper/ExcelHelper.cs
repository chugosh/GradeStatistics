using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Aspose.Cells;
using GradePackage;
using Newtonsoft.Json;

namespace ExcelUnity
{
    public static class ExcelHelper
    {
        public static Configs Rates;
        public static object thisRate = new object();
        private static int row = 3;
        static ExcelHelper()
        {
            var cfg = Path.Combine(Application.StartupPath, "Config.json");
            Rates = JsonConvert.DeserializeObject<Configs>(File.ReadAllText(cfg));
            //t.License l = new Excel.License();
            //l.SetLicense("Aid/License.lic");
        }

        public static bool ToExcel<T>(string destPath, List<T> datas,string name, int rowNum, bool isOpen = false)
        {
            var dataTable = Utils.ToDataTable<T>(datas);
            return ToExcelPrivate(name, destPath, (Worksheet worksheet) =>
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    //var rowActive = 3 + i;
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        var s = dataTable.Rows[i][j];
                        worksheet.Cells[rowNum, j].GetStyle().Font.Size = 11;
                        var style = new CellsFactory().CreateStyle();
                        style.Font.Size = 11;
                        worksheet.Cells[rowNum, j].SetStyle(style);
                        worksheet.Cells[rowNum, j].PutValue(dataTable.Rows[i][j]);
                    }
                    rowNum++;
                }
                //((ExcelRangeBase)worksheet.Cells["A1"]).LoadFromArrays((IEnumerable<object[]>)datas.Select((IList<string> data) => data.ToArray()));
            }, isOpen);// ? dataTable : null;
        }

        public static DataTable ExcelToDataTable(Cells cells) => cells.ExportDataTable(3, 0, cells.MaxRow, cells.MaxColumn);

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
                    if (file.Extension.Length <= 0) continue;
                    var id = CalculationFromExcel(file.FullName, out var classGrade);
                    var grade = new GradeEntity()
                    {
                        Id = Path.GetFileNameWithoutExtension(file.FullName),
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
        public static string CalculationFromExcel(string fileName,out List<ClassEntity> classGrade, string sheetName = "Sheet1")
        {
            //GetGrides(fileName, out int excegride, out int goodgride, out int passgride);
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
                        //取第4位班级号
                        var id = workSheet.Cells[r, 0].Value.ToString().Substring(3,1);
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
                        //科目名称 根据科目名称查找优秀分及格分 根据总分计算及格率优秀率
                        var name = workSheet.Cells[0, c].Value.ToString();
                        GetGrides(fileName, name, out int excegride, out int goodgride, out int passgride);
                        //所有班级一个科目的成绩
                        //每个key是一个班
                        foreach (var key in dic.Keys)
                        {
                            end += dic[key];
                            var endCell = workSheet.Cells[end, c].Name.ToString();// 
                            var sumGradeFormula = $"SUM({startCell}:{endCell})";
                            var excellentFormula = $"COUNTIFS({startCell}:{endCell},\">={(excegride == 0 ? (int)GradeEnum.Grade.优秀 : excegride)}\")";
                            var goodFormula = $"COUNTIFS({startCell}:{endCell},\">={(goodgride == 0 ? (int)GradeEnum.Grade.良好 : goodgride)}\")";
                            var passFormula = $"COUNTIFS({startCell}:{endCell},\">={(passgride == 0 ? (int)GradeEnum.Grade.及格 : passgride)}\")";

                            var sub = new SubjectEntity
                            {
                                Id = key,
                                Name = name,
                                Sum = dic[key],
                                SumGrade = (double)workSheet.CalculateFormula(sumGradeFormula),
                                Excellent = (double)workSheet.CalculateFormula(excellentFormula),
                                Good = (double)workSheet.CalculateFormula(goodFormula),
                                Pass = (double)workSheet.CalculateFormula(passFormula)
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
                            sub.StandGrade = Math.Round(Math.Round(sub.ExcellentRate / (maxExcellentRate == 0 ? 1 : maxExcellentRate), 2) * (double)GradeEnum.GradeRate.优秀率
                                        + Math.Round(sub.GoodRate / (maxGoodRate == 0 ? 1 : maxGoodRate), 2) * (double)GradeEnum.GradeRate.及格率
                                        + Math.Round(sub.PassRate / (maxPassRate == 0 ? 1 : maxPassRate), 2) * (double)GradeEnum.GradeRate.良好率
                                        + Math.Round(sub.Average / (maxAver == 0 ? 1 : maxAver), 2) * (double)GradeEnum.GradeRate.平均分率, 3);
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

        /// <summary>
        /// 获取各个年级的优秀分 及格分 良好分
        /// 新改版的配置里需要获取每个年级每一科目的 todo
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="excegride"></param>
        /// <param name="goodgride"></param>
        /// <param name="passgride"></param>
        private static void GetGrides(string fileName,string className, out int excegride, out int goodgride, out int passgride)
        {
            goodgride = 0;
            excegride = 0;
            passgride = 0;
            var filename = Path.GetFileNameWithoutExtension(fileName);
            if (filename.Equals("1年级") || filename.Equals("1") || filename.Equals("一年级"))
            {
                switch (className)
                {
                    case "语文":
                        excegride = Rates.一年级.语文.优秀分;
                        goodgride = Rates.一年级.语文.优良分;
                        passgride = Rates.一年级.语文.及格分;
                        break;
                    case "数学":
                        excegride = Rates.一年级.数学.优秀分;
                        goodgride = Rates.一年级.数学.优良分;
                        passgride = Rates.一年级.数学.及格分;
                        break;
                    case "英语":
                        excegride = Rates.一年级.英语.优秀分;
                        goodgride = Rates.一年级.英语.优良分;
                        passgride = Rates.一年级.英语.及格分;
                        break;
                    case "科学":
                        excegride = Rates.一年级.科学.优秀分;
                        goodgride = Rates.一年级.科学.优良分;
                        passgride = Rates.一年级.科学.及格分;
                        break;
                }
            }
            else if (filename.Equals("2年级") || filename.Equals("2") || filename.Equals("二年级"))
            {
                switch (className)
                {
                    case "语文":
                        excegride = Rates.二年级.语文.优秀分;
                        goodgride = Rates.二年级.语文.优良分;
                        passgride = Rates.二年级.语文.及格分;
                        break;
                    case "数学":
                        excegride = Rates.二年级.数学.优秀分;
                        goodgride = Rates.二年级.数学.优良分;
                        passgride = Rates.二年级.数学.及格分;
                        break;
                    case "英语":
                        excegride = Rates.二年级.英语.优秀分;
                        goodgride = Rates.二年级.英语.优良分;
                        passgride = Rates.二年级.英语.及格分;
                        break;
                    case "科学":
                        excegride = Rates.二年级.科学.优秀分;
                        goodgride = Rates.二年级.科学.优良分;
                        passgride = Rates.二年级.科学.及格分;
                        break;
                }
            }
            else if (filename.Equals("3年级") || filename.Equals("3") || filename.Equals("三年级"))
            {
                switch (className)
                {
                    case "语文":
                        excegride = Rates.三年级.语文.优秀分;
                        goodgride = Rates.三年级.语文.优良分;
                        passgride = Rates.三年级.语文.及格分;
                        break;
                    case "数学":
                        excegride = Rates.三年级.数学.优秀分;
                        goodgride = Rates.三年级.数学.优良分;
                        passgride = Rates.三年级.数学.及格分;
                        break;
                    case "英语":
                        excegride = Rates.三年级.英语.优秀分;
                        goodgride = Rates.三年级.英语.优良分;
                        passgride = Rates.三年级.英语.及格分;
                        break;
                    case "科学":
                        excegride = Rates.三年级.科学.优秀分;
                        goodgride = Rates.三年级.科学.优良分;
                        passgride = Rates.三年级.科学.及格分;
                        break;
                }
            }
            else if (filename.Equals("4年级") || filename.Equals("4") || filename.Equals("四年级"))
            {
                switch (className)
                {
                    case "语文":
                        excegride = Rates.四年级.语文.优秀分;
                        goodgride = Rates.四年级.语文.优良分;
                        passgride = Rates.四年级.语文.及格分;
                        break;
                    case "数学":
                        excegride = Rates.四年级.数学.优秀分;
                        goodgride = Rates.四年级.数学.优良分;
                        passgride = Rates.四年级.数学.及格分;
                        break;
                    case "英语":
                        excegride = Rates.四年级.英语.优秀分;
                        goodgride = Rates.四年级.英语.优良分;
                        passgride = Rates.四年级.英语.及格分;
                        break;
                    case "科学":
                        excegride = Rates.四年级.科学.优秀分;
                        goodgride = Rates.四年级.科学.优良分;
                        passgride = Rates.四年级.科学.及格分;
                        break;
                }
            }
            else if (filename.Equals("5年级") || filename.Equals("5") || filename.Equals("五年级"))
            {
                switch (className)
                {
                    case "语文":
                        excegride = Rates.五年级.语文.优秀分;
                        goodgride = Rates.五年级.语文.优良分;
                        passgride = Rates.五年级.语文.及格分;
                        break;
                    case "数学":
                        excegride = Rates.五年级.数学.优秀分;
                        goodgride = Rates.五年级.数学.优良分;
                        passgride = Rates.五年级.数学.及格分;
                        break;
                    case "英语":
                        excegride = Rates.五年级.英语.优秀分;
                        goodgride = Rates.五年级.英语.优良分;
                        passgride = Rates.五年级.英语.及格分;
                        break;
                    case "科学":
                        excegride = Rates.五年级.科学.优秀分;
                        goodgride = Rates.五年级.科学.优良分;
                        passgride = Rates.五年级.科学.及格分;
                        break;
                }
            }
            else if (filename.Equals("6年级") || filename.Equals("6") || filename.Equals("六年级"))
            {
                switch (className)
                {
                    case "语文":
                        excegride = Rates.六年级.语文.优秀分;
                        goodgride = Rates.六年级.语文.优良分;
                        passgride = Rates.六年级.语文.及格分;
                        break;
                    case "数学":
                        excegride = Rates.六年级.数学.优秀分;
                        goodgride = Rates.六年级.数学.优良分;
                        passgride = Rates.六年级.数学.及格分;
                        break;
                    case "英语":
                        excegride = Rates.六年级.英语.优秀分;
                        goodgride = Rates.六年级.英语.优良分;
                        passgride = Rates.六年级.英语.及格分;
                        break;
                    case "科学":
                        excegride = Rates.六年级.科学.优秀分;
                        goodgride = Rates.六年级.科学.优良分;
                        passgride = Rates.六年级.科学.及格分;
                        break;
                }
            }
        }

    }
}
