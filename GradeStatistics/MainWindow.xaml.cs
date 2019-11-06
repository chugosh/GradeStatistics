using ExcelUnity;
using GradePackage;
using Microsoft.Win32;
using System.IO;
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
            var datas = ExcelHelper.GetExcelFiles(@"G:\Test");
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
                var datas = ExcelHelper.GetExcelFiles(path);
                //System.Windows.MessageBox.Show(path);
            }
        }
    }
}
