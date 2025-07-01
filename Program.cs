using CommandLine;
using Newtonsoft.Json;
using RedSsl;

namespace RedSslChecker;

class Program
{
    public enum OutputType
    {
        TUI = 1,
        JSON = 2,
    }

    public class Options
    {
        [Option('d', "domain", Required = false, HelpText = "Domain to test.")]
        public string Domain { get; set; } = String.Empty;

        [Option('f', "file", Required = false, HelpText = "Path to a file containing URLs to test.")]
        public string UrlsToTestFilePath { get; set; } = "ssldomains";

        [Option('o', "output", Required = false, HelpText = $"Output type: TUI;JSON.")]
        public OutputType Output { get; set; } = OutputType.TUI;
    }

    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       Options options = o;
                       // You can use the 'options' object here
                       Work(options);
                   })
                   .WithNotParsed(errors => HandleParseError(errors));
    }

    static void Work(Options options)
    {
        List<SslCertInfo> sslCertInfos = new List<SslCertInfo>();

        if (options.Domain.Length > 0)
        {
            sslCertInfos.Add(RedSslUtils.CheckCertificateAsync(options.Domain).Result);
        }
        else
        {
            if (File.Exists(options.UrlsToTestFilePath))
            {
                List<DomainInfo> domainInfos = RedSslUtils.FromFile(options.UrlsToTestFilePath);
                sslCertInfos = RedSslUtils.CheckCertificatesAsync(domainInfos).Result;
            }
            else{
                throw new FileNotFoundException($"The file '{options.UrlsToTestFilePath}' does not exist.");
            }
        }

        switch (options.Output)
        {
            case OutputType.TUI:
                RedSslUtils.PrintTui(sslCertInfos);
                break;
            case OutputType.JSON:
                Console.WriteLine(JsonConvert.SerializeObject(sslCertInfos, Formatting.Indented));
                break;
        }
    }

    static void HandleParseError(IEnumerable<Error> errors)
    {
        foreach (var error in errors)
        {
            Console.Error.WriteLine(error.ToString());
        }
    }
}