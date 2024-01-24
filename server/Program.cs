using System.ComponentModel;
using System.Net;
using System.Security.Cryptography.X509Certificates;


bool listen = true;

//Cancel listener
Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
{
    Console.WriteLine("Interupting cancel event");
    e.Cancel = true;
    listen = false;
};


//crate a http listener
int port = 3000;
HttpListener listener = new();
listener.Prefixes.Add($"http://localhost:{port}/");
listener.Start();
Console.WriteLine($"Server started with port:{port}");

//startar ny context        //Asynkront
listener.BeginGetContext(new AsyncCallback(Router), listener);

//låter server vara startad tills vi trycker på ctrl + c
while (listen) { }

//avslutar server
listener.Stop();
Console.WriteLine("Server stopped");


void Router(IAsyncResult result)
{
    //avslutar contexten
    if (result.AsyncState is HttpListener listener)
    {

        //Request
        HttpListenerContext context = listener.EndGetContext(result);
        HttpListenerRequest request = context.Request;
        Console.WriteLine($"{request.Url}");

        HttpListenerResponse response = context.Response;
        response.StatusCode = (int)HttpStatusCode.OK;
        response.ContentType = "text/plain";

        if (request != null && request.Url != null && request.Url.AbsolutePath.Contains("Time"))
        {
            //Response
            string message = $"\n\n{DateTime.Now}\n\n";
            byte[] time = System.Text.Encoding.UTF8.GetBytes(message);
            response.OutputStream.Write(time);
        }

        else
        {
            //Response
            string message = $"\n\nType /Time to check the time right now.\n\n";
            byte[] timeHelper = System.Text.Encoding.UTF8.GetBytes(message);
            response.OutputStream.Write(timeHelper);
        }
        response.OutputStream.Close();
        listener.BeginGetContext(new AsyncCallback(Router), listener);

    }
}

