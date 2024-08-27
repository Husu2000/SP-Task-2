using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
namespace SP_Task_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private int value = 0;

        public string FromText { get; set; }
        public string ToText { get; set; }
        public int Value { get => value; set { this.value = value; OnPropertyChanged(); } }
        public int Maximum { get => maximum; set { maximum = value; OnPropertyChanged(); } }

        public bool IsToButtonClicked { get; set; } = false;
        public bool IsFromButtonClicked { get; set; } = false;
        byte[]? buffer = null;
        int BytesToRead = 0;
        int ByteRead = 0;
        private int maximum = 0;

        public MainWindow()
        {
            InitializeComponent();
            Maximum = 1;
            value = 0;
            DataContext = this;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void FileClickButton(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            OpenFileDialog fileDialog = new();
            fileDialog.Filter = "Text files (*.txt)|*.txt";
            fileDialog.ShowDialog();
            if (btn == FromButton)
            {
                FromBox.Text = fileDialog.FileName;
                FromText = fileDialog.FileName;
            }
            else
            {
                IsToButtonClicked = true;
                ToBox.Text = fileDialog.FileName;
                ToText = fileDialog.FileName;
            }

        }

        private void CopyClickButton(object sender, RoutedEventArgs e)
        {

            if (!IsToButtonClicked)
            {
                if (!File.Exists(ToBox.Text))
                {
                    using (File.Create(ToBox.Text)) { };
                }
                ToText = ToBox.Text;
            }
            if (!IsFromButtonClicked)
            {
                FromText = FromBox.Text;
            }
            int n = 0;
            using (FileStream fs = new FileStream(FromText, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[fs.Length];
                BytesToRead = Convert.ToInt32(fs.Length);
                ByteRead = 0;

                while (BytesToRead > 0)
                {
                    n = fs.Read(buffer, ByteRead, BytesToRead);
                    BytesToRead -= n;
                    ByteRead += n;
                }
            }
            Maximum = n;
            new Thread(() =>
            {
                using (FileStream fs = new FileStream(ToText, FileMode.Create, FileAccess.Write))
                {
                    int count = 0;

                    while (ByteRead > 0)
                    {
                        fs.Write(buffer, count, 1);
                        count++;
                        ByteRead--;
                        Value += 1;
                        fs.Flush();
                        Thread.Sleep(50);
                    }

                }
            }).Start();

        }
    }
}