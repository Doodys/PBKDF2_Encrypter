using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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

namespace PBKDF2
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

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void TextBox_Iter_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var issues = new StringBuilder();
            if (!int.TryParse(TextBox_Iter.Text, out int result) || TextBox_Iter.Text.Equals(""))
                issues.AppendLine("Number of iterations TextBox has wrong format!");
            if(TextBox_Pass.Text.Equals(""))
                issues.AppendLine("Password TextBox has wrong format!");
            if (!issues.Length.Equals(0))
                MessageBox.Show(issues.ToString());
            else
                TextBox_Result.Text = PBKDF2_Encrypt(sender, TextBox_Pass.Text, Convert.ToInt32(TextBox_Iter.Text));
        }

        private static string PBKDF2_Encrypt(object sender, string pass, int iters)
        {
            var hashedPassword = PBKDF2_Encrypt(Encoding.UTF8.GetBytes(pass), GenerateSalt(), iters);
            return Convert.ToBase64String(hashedPassword);
        }

        private static byte[] PBKDF2_Encrypt(byte[] toBeHashed, byte[] salt, int iters)
        {
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(toBeHashed, salt, iters))
                return rfc2898DeriveBytes.GetBytes(32);
        }

        private static byte[] GenerateSalt()
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[32];
                randomNumberGenerator.GetBytes(randomNumber);
                return randomNumber;
            }
        }
    }
}
