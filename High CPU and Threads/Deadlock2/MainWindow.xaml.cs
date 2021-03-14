using System.Threading.Tasks;
using System.Windows;

namespace Deadlock2
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CoolButton.Content = GetJson().Result;
        }

        private async void Button_Click2(object sender, RoutedEventArgs e)
        {
            CoolButton2.IsEnabled = false;
            await DoLongTask();
            CoolButton2.IsEnabled = true;
        }

        private async Task DoLongTask(int iterations = 1_000_000)
        {
            for(int i = 0; i < iterations; i++)
                await Task.Delay(100);
        }



        public Task<string> GetJson() => GetJsonFromEndpointAsync("http://this.not-exists.com/foo/bar");

        //this is "library" method
        public async Task<string> GetJsonFromEndpointAsync(string url)
        {
            await DoLongTask(5);
            return "{ 'Foo': 123 }";
        }
    }
}
