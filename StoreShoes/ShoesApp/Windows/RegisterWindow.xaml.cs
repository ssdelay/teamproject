using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.RightsManagement;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShoesApp.Windows
{
    public partial class RegisterWindow : Window
    {
        private string captchaCode; 

        public RegisterWindow()
        {
            InitializeComponent();
            GenerateCaptcha();
        }

        private void GenerateCaptcha()
        {
            Random random = new Random();
            captchaCode = new string(Enumerable.Range(0, 5).Select(_ => (char)random.Next(65, 90)).ToArray());
            CaptchaImage.Source = CreateCaptchaImage(captchaCode);
        }

        private BitmapSource CreateCaptchaImage(string captchaText)
        {
            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                dc.DrawText(new FormattedText(captchaText, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                    new Typeface("Arial"), 26, Brushes.Black), new Point(0, 0));
            }
            var bitmap = new RenderTargetBitmap(100, 40, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            return bitmap;
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (CaptchaTextBox.Text != captchaCode)
            {
                MessageBox.Show("Недействительная капча");
                GenerateCaptcha();
                return;
            }

            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }

            var registerModel = new
            {
                Name = NameTextBox.Text,
                Login = LoginTextBox.Text,
                Password = PasswordBox.Password,
                Email = EmailTextBox.Text
            };

            var response = await PostRequest("api/auth/registration", registerModel);
            if (response.Contains("Конфликт"))
            {
                MessageBox.Show("Пользователь уже существует");
            }
            else
            {
                MessageBox.Show(response);
                TransitionToWindowLog();
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            TransitionToWindowLog();
        }

        public async void TransitionToWindowLog()
        {
            LoginWindow lw = new LoginWindow();
            lw.Show();
            this.Close();
        }

        private async Task<string> PostRequest(string url, object data)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000/");
                var response = await client.PostAsJsonAsync(url, data);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
