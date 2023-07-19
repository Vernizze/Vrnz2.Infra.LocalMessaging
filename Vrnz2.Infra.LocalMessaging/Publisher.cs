using System.IO.Pipes;
using System.Text;

namespace Vrnz2.Infra.LocalMessaging
{
    public static class Publisher
    {
        public static void WriteMessage(string topicName, string message) 
        {
            using (NamedPipeClientStream namedPipeClient = new NamedPipeClientStream(topicName))
            {
                namedPipeClient.Connect();
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                namedPipeClient.Write(messageBytes, 0, messageBytes.Length);
            }
        }
    }
}
