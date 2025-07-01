using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Spectre.Console;

namespace RedSsl;

public static class RedSslUtils
{
    public static SslStatus CalculateSslStatusFromDaysLeft(int daysLeft)
    {
        switch (daysLeft)
        {
            case >= 20:
                return SslStatus.Valid;
            case >= 10:
                return SslStatus.Warning;
            case >= 5:
                return SslStatus.ExpireSoon;
            case >= 1:
                return SslStatus.Critical;
            default:
                return SslStatus.Expired;
        }
    }

    public static async Task<SslCertInfo> CheckCertificateAsync(string url, int port = 443)
    {
        return await CheckCertificateAsync(new DomainInfo(url, port));
    }

    internal static async Task<SslCertInfo> CheckCertificateAsync(DomainInfo domainInfo)
    {
        try
        {
            var uri = new UriBuilder(domainInfo.Url).Uri;

            using TcpClient client = new(uri.Host, domainInfo.Port);
            using SslStream sslStream = new(client.GetStream(), false, new RemoteCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true), null);

            sslStream.AuthenticateAsClient(uri.Host);
            X509Certificate? cert = sslStream.RemoteCertificate;

            ArgumentNullException.ThrowIfNull(cert, "Remote certificate is null");

            var cert2 = new X509Certificate2(cert);

            return await Task.FromResult(new SslCertInfo(domainInfo, cert2.NotAfter));
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error checking certificate [{domainInfo.Url}:{domainInfo.Port}]: " + ex.Message);
            return new SslCertInfo(domainInfo, DateTime.MinValue, ex);
        }
    }

    internal static async Task<List<SslCertInfo>> CheckCertificatesAsync(List<DomainInfo> domainInfos)
    {
        List<Task<SslCertInfo>> tasks = domainInfos.Select(domainInfo => CheckCertificateAsync(domainInfo)).ToList();

        await Task.WhenAll(tasks);

        return tasks.Select(task => task.Result).ToList();
    }

    internal static List<DomainInfo> FromFile(string path)
    {
        List<DomainInfo> domains = new();

        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            domains.Add(new DomainInfo(line));
        }

        return domains;
    }

    internal static void PrintTui(List<SslCertInfo> sslCertInfos)
    {
        sslCertInfos.Sort((a, b) => a.DaysLeft.CompareTo(b.DaysLeft));

        Table table = new Table();

        table.AddColumn("Domain");
        table.AddColumn("Status");
        table.AddColumn("ExpirationDate");
        table.AddColumn("Days");

        foreach (var certInfo in sslCertInfos)
        {
            string c = GetColor(certInfo.Status);

            table.AddRow(
                $"[{c}]" + certInfo.DomainInfo.Url + "[/]",
                $"[{c}]" + certInfo.Status.ToString() + "[/]",
                $"[{c}]" + certInfo.ExpirationDate.ToString("yyyy-MM-dd HH:mm:ss") + "[/]",
                $"[{c}]" + certInfo.DaysLeft.ToString() + "[/]"
            );
        }

        AnsiConsole.Write(table);
    }

    internal static string GetColor(SslStatus status)
    {
        switch (status)
        {
            case SslStatus.Valid:
                return "green";
            case SslStatus.Warning:
                return "yellow";
            case SslStatus.ExpireSoon:
                return "orange";
            case SslStatus.Critical:
                return "red";
            case SslStatus.Expired:
                return "purple";
            default:
                return "white";
        }
    }
}