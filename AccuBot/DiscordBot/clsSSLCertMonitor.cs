using System;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DiscordBot;


namespace AccuBot
{
    public class clsSSLCertMonitor
    {
        static public DateTime? LastTest = null;
        
        public clsSSLCertMonitor()
        {
            
        }

        internal static void HeartbeatCheck(bool force = false)
        {

            var hoursSinceLastTest = LastTest.HasValue ? (DateTime.UtcNow - LastTest.Value).Hours : int.MaxValue;
        
            if (force || hoursSinceLastTest >= 24)
            {
                Console.WriteLine("clsSSLCertMonitor: HeartbeatCheck");
                LastTest = DateTime.UtcNow;

                Task.Run(() =>
                {
                    foreach (var ssl in Program.SSLCertsList.Values)
                    {
                        if (ssl.Monitor) CheckSSL(ssl);
                    }
                });
            }
        }

        private static async void CheckSSL(clsSSLCerts ssl)
        {       
            try
            {
                var url = ssl.URL.Trim();
                if (!url.StartsWith("https://")) url = $"https://{url}";

                var uri = new Uri(url);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                RequestCachePolicy policy = new  RequestCachePolicy( RequestCacheLevel.BypassCache);
                request.AllowAutoRedirect = true;
                request.CachePolicy = policy;

                HttpWebResponse response;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    response.Close();
                }
                catch { }
                
                
                var cert = request.ServicePoint.Certificate;
                var cert2 = new X509Certificate2(cert);
                
                ssl.CertExpiry = DateTime.Parse(cert2.GetExpirationDateString());
                var days = (ssl.CertExpiry - DateTime.UtcNow).Value.Days;

                if (days <= 7) SendMessage(ssl, $"{ssl.URL} certificate expires in {days} days");
                
            }
            catch (Exception ex)
            {
                var errorMsg = $"{ssl.URL} certificate read error: {ex.Message}";
                SendMessage(ssl, errorMsg);
                Console.WriteLine(errorMsg);
            }
        }

        private static void SendMessage(clsSSLCerts ssl, string messageText)
        {
            foreach (var user in ssl.Contacts.Split(';'))
            {
                if (user.StartsWith("@"))
                {
                    var hashsplit = user.Split('#');
                    if (hashsplit.Length == 2)
                    {
                        try
                        {
                            var discordUser = clsBotClient.Instance.Our_BotAlert.Guild.Members.FirstOrDefault(x => x.Discriminator == hashsplit[1]);
                            discordUser.SendMessageAsync(messageText);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error {ex.Message}");
                        }
                    }                    
                }
                else if (user.Contains("@"))
                {
                    clsEmail.SendEmail(user,"SSL Certificate",messageText);
                }
            }
        }
    }
}
