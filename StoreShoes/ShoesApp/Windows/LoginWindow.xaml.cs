using System.Globalization;
using System.Net.Http;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Net.Http.Json;

namespace ShoesApp.Windows
{
    public partial class LoginWindow : Window
    {
        private string captchaCode;

        public LoginWindow()
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

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (CaptchaTextBox.Text != captchaCode)
            {
                MessageBox.Show("Не верная капча");
                GenerateCaptcha();
                return;
            }

            var loginModel = new
            {
                Login = LoginTextBox.Text,
                Password = PasswordBox.Password
            };

            var response = await PostRequest("api/auth/login", loginModel);
            if (response == "Неавторизован")
            {
                MessageBox.Show("Неверный логин или пароль  ");
            }
            else
            {
                MessageBox.Show(response);
            }
        }


        

        private async void RegButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow rw = new RegisterWindow();
            rw.Show();
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
