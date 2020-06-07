using ExcelUnity;
using GradePackage;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace GradeStatistics
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if(folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var path = folderBrowserDialog.SelectedPath;
                tbFileName.Text = path;
                //所有年级所有班级的所有科目成绩
                var datas = ExcelHelper.GetExcelFiles(path);
                foreach(var d in datas)
                {
                    var rankGrade = new RankGradeEntity();
                    var allList = new List<SubjectEntity>();
                    var sourceFile = Path.Combine(System.Windows.Forms.Application.StartupPath, "Result.xlsx");
                    var destPath = $@"{path}\Result";
                    var destFile = $@"{destPath}\{d.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    if (!Directory.Exists(destPath))
                        Directory.CreateDirectory(destPath);
                    File.Copy(sourceFile, destFile, true);
                    
                    foreach (GradeEnum.SubjectName n in Enum.GetValues(typeof(GradeEnum.SubjectName)))
                    {
                        GradeHelper.GetEntitiesByExcelDatas(d, ref rankGrade, n); 
                        //每一科目list写入excel
                        //TODO
                        rankGrade.RankDict.TryGetValue(n.ToString(), out List<SubjectEntity> list);
                        var sort = 1;
                        var sortList = list.OrderByDescending(l => l.StandGrade).ToList();
                        sortList.ForEach(s => s.ClassSort = sort++);
                        allList = allList.Concat(sortList).ToList();
                        //ExcelHelper.ToExcel(destFile, sortList, "result");
                        //list.OrderByDescending(l => l.StandGrade).ToList().ForEach(l => allList.Add(l));
                    }
                    ExcelHelper.ToExcel(destFile, allList, "result", 3);
                    //
                    BindsList(d, allList);
                    //rankGrade.RankDict.TryGetValue("数学", out List<SubjectEntity> list);
                    //m_dataGrid.ItemsSource = allList;
                    //ExcelHelper.ToExcel();
                }
            }
            System.Windows.MessageBox.Show("计算完毕并保存文件成功");
        }

        private void BindsList(GradeEntity d, List<SubjectEntity> allList)
        {
            if (d.Id.Equals("1年级") || d.Id.Equals("1") || d.Id.Equals("一年级"))
                m_dataGrid.ItemsSource = allList;
            else if (d.Id.Equals("2年级") || d.Id.Equals("2") || d.Id.Equals("二年级"))
                m_dataGrid2.ItemsSource = allList;
            else if (d.Id.Equals("3年级") || d.Id.Equals("3") || d.Id.Equals("三年级"))
                m_dataGrid3.ItemsSource = allList;
            else if (d.Id.Equals("4年级") || d.Id.Equals("4") || d.Id.Equals("四年级"))
                m_dataGrid4.ItemsSource = allList;
            else if (d.Id.Equals("5年级") || d.Id.Equals("5") || d.Id.Equals("五年级"))
                m_dataGrid5.ItemsSource = allList;
            else if (d.Id.Equals("6年级") || d.Id.Equals("6") || d.Id.Equals("六年级"))
                m_dataGrid6.ItemsSource = allList;
        }
    }
}
