namespace RedSsl;

public enum SslStatus
{
    Unset, //unset
    Valid, // 20 days left or more
    Warning, //10-20 days left
    ExpireSoon, //5-10 days left
    Critical, // 0-5 days left
    Expired, //0 days left
}