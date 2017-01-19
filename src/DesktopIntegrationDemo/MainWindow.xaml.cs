using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace DesktopIntegrationDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string scope = "wl.emails%20wl.basic%20wl.offline_access%20wl.signin";
        static string subscriptionKey = "Yours here";
        static string client_id = "Yours here";
        static string client_secret = "Yours here";
        static Uri signInUrl = new Uri(String.Format(@"https://login.live.com/oauth20_authorize.srf?client_id={0}&redirect_uri=https://login.live.com/oauth20_desktop.srf&response_type=code&scope={1}", client_id, scope));

        static string accessTokenUrl = String.Format(@"https://login.live.com/oauth20_token.srf?client_id={0}&client_secret={1}&redirect_uri=https://login.live.com/oauth20_desktop.srf&grant_type=authorization_code&code=", client_id, client_secret);
        static string refreshTokenUrl = String.Format(@"https://login.live.com/oauth20_token.srf?client_id={0}&client_secret={1}&redirect_uri=https://login.live.com/oauth20_desktop.srf&grant_type=refresh_token&refresh_token=", client_id, client_secret);
        
        public MainWindow()
        {
            InitializeComponent();
            browserWindow.LoadCompleted += BrowserWindow_LoadCompleted;
            LoginButton.Click += LoginButton_Click;
        }

        private void BrowserWindow_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (e.Uri.AbsoluteUri.Contains("code="))
            {
                string auth_code = Regex.Split(e.Uri.AbsoluteUri, "code=")[1];
                Properties.Settings.Default["auth_code"] = auth_code;
                Properties.Settings.Default.Save();
                browserWindow.Visibility = Visibility.Collapsed;
                makeAccessTokenRequest(accessTokenUrl + auth_code);
            }
        }

        public async void makeAccessTokenRequest(string requestUrl)
        {
            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            string responseTxt = String.Empty;
            using (WebResponse response = await request.GetResponseAsync())
            {
                var reader = new StreamReader(response.GetResponseStream());
                responseTxt = reader.ReadToEnd();

                var tokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseTxt);
                if (tokenData.ContainsKey("access_token"))
                {
                    Properties.Settings.Default["access_token"] = tokenData["access_token"];
                    Properties.Settings.Default["refresh_token"] = tokenData["refresh_token"];
                    Properties.Settings.Default.Save();
                }
            }
            testApiAccess();
        }

        public async void testApiAccess()
        {
            var authorizationBearer = string.Format("Bearer {0}", Properties.Settings.Default["access_token"].ToString());

            Dictionary<string, List<string>> customHeaders = new Dictionary<string, List<string>>();
            customHeaders.Add("Authorization", new List<string>() { authorizationBearer });
            MVP.MVPProduction prodClient = new MVP.MVPProduction();
            var result = await prodClient.GetContributionTypesWithHttpMessagesAsync(null, subscriptionKey, customHeaders);
            var result2 = await prodClient.GetMVPProfileWithHttpMessagesAsync(null, subscriptionKey, customHeaders);
            
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            browserWindow.Navigate(signInUrl);
        }
    }
}
