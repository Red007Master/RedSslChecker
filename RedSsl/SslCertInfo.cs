namespace RedSsl;

public class SslCertInfo
{
    public bool Failed { get; private set; } = false;
    public Exception? Ex { get; private set; } = null;


    public DomainInfo DomainInfo { get; private set; }

    public SslStatus Status { get { return RedSslUtils.CalculateSslStatusFromDaysLeft(DaysLeft); } }
    public DateTime ExpirationDate { get; private set; }
    public int DaysLeft
    {
        get { return (ExpirationDate - DateTime.UtcNow).Days; }
    }

    public SslCertInfo(DomainInfo domainInfo, DateTime expirationDate)
    {
        DomainInfo = domainInfo;
        ExpirationDate = expirationDate;
    }

    public SslCertInfo(DomainInfo domainInfo, DateTime expirationDate, Exception ex) : this(domainInfo, expirationDate)
    {
        Failed = true;
        Ex = ex;
    }
}
