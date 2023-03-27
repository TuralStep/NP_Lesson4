using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
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
using System.IO;

namespace ClientWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var ip = IPAddress.Parse("127.0.0.1");
            var port = 45678;

            client = new UdpClient();
            remoteEP = new(ip, port);
        }

        UdpClient client;
        IPEndPoint remoteEP;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var buffer = new byte[ushort.MaxValue - 29];
            await client.SendAsync(buffer, buffer.Length, remoteEP);


            var len = 0;
            var limitLen = buffer.Length;

            var list = new List<byte>();

            while (true)
            {
                do
                {
                    var result = await client.ReceiveAsync();
                    buffer = result.Buffer;
                    len = buffer.Length;
                    list.AddRange(buffer);

                } while (len == limitLen);

                var imageData = list.ToArray();
                var image = new BitmapImage();

                if (!(imageData == null || imageData.Length == 0))
                {
                    using (var mem = new MemoryStream(imageData))
                    {
                        mem.Position = 0;
                        image.BeginInit();
                        image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.UriSource = null;
                        image.StreamSource = mem;
                        image.EndInit();
                    }
                    image.Freeze();
                }

                imageBox.Source = image;
                list.Clear();
            }
        }
    }
}
