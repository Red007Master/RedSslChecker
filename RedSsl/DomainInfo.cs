namespace RedSsl;

public class DomainInfo
{
    public const int DefaultPort = 443;

    public string Url { get; set; }
    public int Port { get; set; }

    public DomainInfo(string url, int port = DefaultPort)
    {
        Url = url;
        Port = port;
    }

    public DomainInfo(string dataString)
    {
        dataString.Trim();

        if (dataString.Contains(" "))
        {
            var parts = dataString.Split(' ');

            Url = parts[0];

            bool result = int.TryParse(parts[1], out int port);

            if (result)
            {
                Port = port;
            }
            else
            {
                Port = DefaultPort;
                Console.WriteLine("Invalid port number: " + parts[1]);
                Console.WriteLine("Using default port: " + Port);
            }

        }
        else
        {
            Url = dataString;
            Port = DefaultPort;
        }
    }
}
