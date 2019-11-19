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

        private void BtnFile_Click(object sender, RoutedEventArgs e)
        {
            
            //OpenFileDialog fileDialog = new OpenFileDialog
            //{
            //    Filter = "Excel(*.xlsx)|*.xlsx;*.xls",
            //    RestoreDirectory = true,
            //};

            //if(fileDialog.ShowDialog() == true)
            //{
            //    tbFileName.Text = fileDialog.FileName;
            //    var datas = ExcelHelper.FromExcel(fileDialog.FileName, "Sheet1");
            //    datas.RemoveAt(0);
            //    var gradeEntities = GradeHelper.GetEntitiesByExcelDatas(datas);
            //    m_dataGrid.ItemsSource = gradeEntities;
            //}
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
                    var sourcePath = Path.Combine(System.Windows.Forms.Application.StartupPath, "Result.xlsx");
                    var destPath = $@"{path}\{d.Id}年级_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    File.Copy(sourcePath, destPath, true);
                    foreach (GradeEnum.SubjectName n in Enum.GetValues(typeof(GradeEnum.SubjectName)))
                    {
                        GradeHelper.GetEntitiesByExcelDatas(d, ref rankGrade, n);
                        //每一科目list写入excel
                        //TODO
                        rankGrade.RankDict.TryGetValue(n.ToString(), out List<SubjectEntity> list);
                        ExcelHelper.ToExcel<SubjectEntity>(destPath, list.OrderByDescending(l => l.StandGrade).ToList(), "result");
                        list.ForEach(l => allList.Add(l));
                    }
                    System.Windows.MessageBox.Show("导入完毕");
                    //rankGrade.RankDict.TryGetValue("数学", out List<SubjectEntity> list);
                    //m_dataGrid.ItemsSource = allList;
                    //ExcelHelper.ToExcel();
                }
            }
        }
    }
}
