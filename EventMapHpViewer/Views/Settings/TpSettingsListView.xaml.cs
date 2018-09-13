using System;
using System.Collections.Generic;
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

namespace EventMapHpViewer.Views.Settings
{
    /// <summary>
    /// TpSettingsListView.xaml の相互作用ロジック
    /// </summary>
    public partial class TpSettingsListView : UserControl
    {
        public TpSettingsListView()
        {
            InitializeComponent();
        }

        public double IdColumnWidth
        {
            get { return (double)GetValue(IdColumnWidthProperty); }
            set { SetValue(IdColumnWidthProperty, value); }
        }
        public static readonly DependencyProperty IdColumnWidthProperty =
            DependencyProperty.Register(nameof(IdColumnWidth), typeof(double), typeof(TpSettingsListView), new PropertyMetadata(double.NaN));

        public double NameColumnWidth
        {
            get { return (double)GetValue(NameColumnWidthProperty); }
            set { SetValue(NameColumnWidthProperty, value); }
        }
        public static readonly DependencyProperty NameColumnWidthProperty =
            DependencyProperty.Register(nameof(NameColumnWidth), typeof(double), typeof(TpSettingsListView), new PropertyMetadata(double.NaN));

        public double TpColumnWidth
        {
            get { return (double)GetValue(TpColumnWidthProperty); }
            set { SetValue(TpColumnWidthProperty, value); }
        }
        public static readonly DependencyProperty TpColumnWidthProperty =
            DependencyProperty.Register(nameof(TpColumnWidth), typeof(double), typeof(TpSettingsListView), new PropertyMetadata(double.NaN));

        public double TypeIdColumnWidth
        {
            get { return (double)GetValue(TypeIdColumnWidthProperty); }
            set { SetValue(TypeIdColumnWidthProperty, value); }
        }
        public static readonly DependencyProperty TypeIdColumnWidthProperty =
            DependencyProperty.Register(nameof(TypeIdColumnWidth), typeof(double), typeof(TpSettingsListView), new PropertyMetadata(double.NaN));

        public double TypeNameColumnWidth
        {
            get { return (double)GetValue(TypeNameColumnWidthProperty); }
            set { SetValue(TypeNameColumnWidthProperty, value); }
        }
        public static readonly DependencyProperty TypeNameColumnWidthProperty =
            DependencyProperty.Register(nameof(TypeNameColumnWidth), typeof(double), typeof(TpSettingsListView), new PropertyMetadata(double.NaN));
    }
}
