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
using System.Windows.Forms;


namespace ProjectSetUp_C01
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class MyForm : Window
    {
        public MyForm()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = @"C:\";
            openFile.Filter = "csv files (*.csv)|*.csv";

            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbxFile.Text = openFile.FileName;
            }
            else
            {
                tbxFile.Text = "";
            }
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        public string GetTextBoxValue()
        {
            return tbxFile.Text;
        }

        public bool GetRadioButton1()
        {
            if (rb1.IsChecked == true)
                return true;
            else
                return false;
        }


        public bool GetRadioButton2()
        {
            if (rb2.IsChecked == true)
                return true;
            else
                return false;
        }


        public bool GetCheckBox1()
        {
            if (chbCheck1.IsChecked == true)
                return true;
            else
                return false;
        }

        public bool GetCheckBox2()
        {
            if (chbCheck2.IsChecked == true)
                return true;
            else
                return false;
        }






















        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
