namespace Vrnz2.Infra.LocalMessaging.Example
{
    internal class Run
    {
        public void Exec() 
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;
            var topicName = "Teste123";

            Console.WriteLine($"Nome do Tópico => {topicName}...");

            var listener = new Listener(PrintDebug);
            var listenerTask = listener.Run(topicName, Print, token);

            for (int i = 0; i < 5; i++) 
            {
                Publisher.WriteMessage(topicName, $"Valor {i}");                
            }
            
            cancelTokenSource.Cancel();
            listenerTask.Wait();
            cancelTokenSource.Dispose();
        }

        private void Print(string message) 
        {
            Console.WriteLine($"Mensagem => {message}...");
        }

        private void PrintDebug(string message)
        {
            Console.WriteLine($"Mensagem Debug => {message}...");
        }
    }
}
