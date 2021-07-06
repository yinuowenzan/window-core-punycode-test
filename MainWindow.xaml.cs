//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
using static punycode转换.Punycode转换;

namespace core版解码
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void 按钮1_Click(object sender, RoutedEventArgs e)
        {
            string a1 = 文本框1.Text.Trim(' ');
            a1 = 编码项目(a1);
            文本框1.Text = a1;
        }

        private void 按钮2_Click(object sender, RoutedEventArgs e)
        {
            string a1 = 文本框2.Text;//解码文本用的新字符串
            a1 = 解码项目(a1);//传递值到punycodef返回
            文本框2.Text = a1;//文本框文字模式内容等于新字符串
        }

        private void 关闭_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            //关闭程序
        }

        private void 改变_Click(object sender, RoutedEventArgs e)
        {
            bvcb.Text = 文本框2.Text;
        }

    }
}
