using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vrnz2.Infra.LocalMessaging
{
    public class Listener
    {
        private readonly Action<string> _printDebug = null;

        public Listener() 
        {
        
        }
        public Listener(Action<string> printDebug) 
        { 
            _printDebug = printDebug; 
        }

        public Task Run(string topicName, Action<string> writeResponse, CancellationToken cancellationToken) 
        {
            var task = new Task(() =>
            {
                var exec = true;

                while (exec)
                {
                    using (NamedPipeServerStream namedPipeServer = new NamedPipeServerStream(topicName, PipeDirection.InOut, 1, PipeTransmissionMode.Message))
                    {
                        PrintDebug("Listen...");
                        namedPipeServer.WaitForConnection();
                        StringBuilder messageBuilder = new StringBuilder();
                        string messageChunk = string.Empty;
                        byte[] messageBuffer = new byte[5];
                        do
                        {
                            namedPipeServer.Read(messageBuffer, 0, messageBuffer.Length);
                            messageChunk = Encoding.UTF8.GetString(messageBuffer);
                            messageBuilder.Append(messageChunk);
                            messageBuffer = new byte[messageBuffer.Length];
                        }
                        while (!namedPipeServer.IsMessageComplete);

                        PrintDebug(messageBuilder.ToString());
                        writeResponse.Invoke(messageBuilder.ToString());
                    }

                    exec = !cancellationToken.IsCancellationRequested;
                }

                PrintDebug("Goodbye :)...");
            }, cancellationToken);
            task.Start();

            return task;
        }

        private void PrintDebug(string msg) 
        {
            if (_printDebug == null)
                Console.WriteLine(msg);
            else
                _printDebug.Invoke(msg);

        }
    }
}
