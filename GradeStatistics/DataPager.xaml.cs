using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GetWorkSafetyHisDatas
{
    /// <summary>
    /// DataPager.xaml 的交互逻辑
    /// </summary>
    public partial class DataPager : UserControl, INotifyPropertyChanged
    {
        public DataPager()
        {
            InitializeComponent();
        }

        public event PageChangingRouteEventHandler PageChanging;

        public event PageChangedRouteEventHandler PageChanged;

        #region 依赖属性
        public int PageIndex
        {
            get => (int)GetValue(PageIndexProperty);
            set => SetValue(PageIndexProperty, value);
        }

        public static readonly DependencyProperty PageIndexProperty =
            DependencyProperty.Register(nameof(PageIndex), typeof(int), typeof(DataPager),
                new UIPropertyMetadata(1, (sender, e) =>
                {
                    DataPager dp = sender as DataPager;
                    dp?.ChangeNavigationButtonState();
                }));

        public int PageSize
        {
            get => (int)GetValue(PageSizeProperty);
            set { SetValue(PageSizeProperty, value); InitData(); }
        }

        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register(nameof(PageSize), typeof(int), typeof(DataPager),
                new UIPropertyMetadata(50, (sender, e) =>
                {
                    DataPager dp = sender as DataPager;
                    dp?.ChangeNavigationButtonState();
                }));

        public int TotalCount
        {
            get => (int)GetValue(TotalCountProperty);
            set => SetValue(TotalCountProperty, value);
        }

        public static readonly DependencyProperty TotalCountProperty =
            DependencyProperty.Register(nameof(TotalCount), typeof(int), typeof(DataPager),
                new UIPropertyMetadata(0, (sender, e) =>
                {
                    var dp = sender as DataPager;
                    if (dp == null) return;
                    dp.InitData();
                    dp.ChangeNavigationButtonState();
                }));
        public int PageCount
        {
            get => (int)GetValue(PageCountProperty);
            private set => SetValue(PageCountProperty, value);
        }

        public static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register(nameof(PageCount), typeof(int), typeof(DataPager), new UIPropertyMetadata(1));

        public bool CanGoFirstOrPrev => PageIndex > 1;

        public bool CanGoLastOrNext => PageIndex < PageCount;
        #endregion

        private void btnFirst_Click(object sender, RoutedEventArgs e) => OnPageChanging(1);

        private void btnPrev_Click(object sender, RoutedEventArgs e) => OnPageChanging(this.PageIndex - 1);

        private void btnNext_Click(object sender, RoutedEventArgs e) => OnPageChanging(this.PageIndex + 1);

        private void btnLast_Click(object sender, RoutedEventArgs e) => OnPageChanging(this.PageCount);

        private void btnGoTo_Click(object sender, RoutedEventArgs e)
        {
            int pageIndex = 1;
            try
            {
                pageIndex = Convert.ToInt32(txtPageIndex.Text);
            }
            catch
            {
                // ignored
            }
            finally
            {
                OnPageChanging(pageIndex);
            }
        }

        internal void OnPageChanging(int pageIndex)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageIndex > this.PageCount) pageIndex = this.PageCount;

            var oldPageIndex = this.PageIndex;
            var newPageIndex = pageIndex;
            var eventArgs = new PageChangingEventArgs() { OldPageIndex = oldPageIndex, NewPageIndex = newPageIndex };
            PageChanging?.Invoke(this, eventArgs);
            if (!eventArgs.IsCancel)
            {
                this.PageIndex = newPageIndex;
                PageChanged?.Invoke(this, new PageChangedEventArgs() { CurrentPageIndex = this.PageIndex });
            }
        }

        void ChangeNavigationButtonState()
        {
            this.NotifyPropertyChanged(nameof(CanGoFirstOrPrev));
            this.NotifyPropertyChanged(nameof(CanGoLastOrNext));
        }

        void InitData()
        {
            if (this.TotalCount == 0)
            {
                this.PageCount = 1;
            }
            else
            {
                this.PageCount = this.TotalCount % this.PageSize > 0
                    ? (this.TotalCount / this.PageSize) + 1
                    : this.TotalCount / this.PageSize;
            }
            if (this.PageIndex < 1)
            {
                this.PageIndex = 1;
            }
            if (this.PageIndex > this.PageCount)
            {
                this.PageIndex = this.PageCount;
            }
            if (this.PageSize < 1)
            {
                this.PageSize = 100;
            }
        }

        #region INotifyPropertyChanged成员
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public delegate void PageChangingRouteEventHandler(object sender, PageChangingEventArgs e);
    public delegate void PageChangedRouteEventHandler(object sender, PageChangedEventArgs e);

    public class PageChangedEventArgs : RoutedEventArgs
    {
        public int CurrentPageIndex { get; set; }
    }

    public class PageChangingEventArgs : RoutedEventArgs
    {
        public int OldPageIndex { get; set; }
        public int NewPageIndex { get; set; }
        public bool IsCancel { get; set; }
    }
}
