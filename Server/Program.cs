using System.Drawing;
using System.Net.Sockets;

UdpClient server = new UdpClient(45678);

while (true)
{

    var result = await server.ReceiveAsync();

    new Task(async () =>
    {

        var remoteEP = result.RemoteEndPoint;
        while (true)
        {
            await Task.Delay(50);

            Bitmap img = new(1366, 768);

            Graphics memoryGraphics = Graphics.FromImage(img);
            memoryGraphics.CopyFromScreen(0, 0, 0, 0, img.Size);


            byte[] imgBytes;

            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                imgBytes = stream.ToArray();
            }


            var chunkArr = imgBytes.Chunk(ushort.MaxValue - 29);

            foreach (var array in chunkArr)
                await server.SendAsync(array, array.Length, remoteEP);
        }

    }).Start();
}