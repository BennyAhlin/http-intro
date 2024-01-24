using System.ComponentModel;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;


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

        string dateTime = $"\n\n{DateTime.Now}\n\n";
        

        string commands = $"Avaiable commands: ";
        byte[] avaiable = System.Text.Encoding.UTF8.GetBytes(commands);


        byte[] date = System.Text.Encoding.UTF8.GetBytes(dateTime);
       
        string time = $"\n  /Time";
        byte[] timeCommand = System.Text.Encoding.UTF8.GetBytes(time);
       
        string user = $"\n  /User";
        byte[] userCommand = System.Text.Encoding.UTF8.GetBytes(user);
       
        string db = $"\n  /Db";
        byte[] dbCommand = System.Text.Encoding.UTF8.GetBytes(db);

        switch (request != null, request?.Url != null, request?.Url?.AbsolutePath.ToLower())
        {
            case (true, true, "/time"):
                response.OutputStream.Write(date);
                break;

            case (true, true, "/user"):
                response.OutputStream.Write(userCommand);

                break;

            case (true, true, "/db"):
                response.OutputStream.Write(dbCommand);
                break;

            default:
                response.OutputStream.Write(avaiable);
                response.OutputStream.Write(timeCommand);
                response.OutputStream.Write(userCommand);
                response.OutputStream.Write(dbCommand);

                break;
        }

        if (request != null && request.Url != null && request.Url.AbsolutePath.Contains("Time"))
        {
            
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

       


        StringBuilder resultBuilder = new StringBuilder();


        // Check if the reader has rows
        if (reader.HasRows)
        {
            // Iterate through the rows and append values to the StringBuilder
            while (reader.Read())
            {
                resultBuilder.AppendLine($"Book ID: {reader.GetInt32(reader.GetOrdinal("book_id"))}");
                resultBuilder.AppendLine($"Title: {reader.GetString(reader.GetOrdinal("title"))}");
                resultBuilder.AppendLine($"Publication Date: {reader.GetDateTime(reader.GetOrdinal("publication_date")):yyyy-MM-dd}");
                resultBuilder.AppendLine($"Price: {reader.GetDecimal(reader.GetOrdinal("price"))}");
                resultBuilder.AppendLine($"ISBN: {reader.GetInt32(reader.GetOrdinal("isbn"))}");
                resultBuilder.AppendLine("-----------------------------------------");
            }
        }
        byte[] responseBytes = Encoding.UTF8.GetBytes(resultBuilder.ToString());
        response.OutputStream.Write(responseBytes, 0, responseBytes.Length);




    }
}

