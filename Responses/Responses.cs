using System.Net;

namespace Domain.Responses;
public class Response
{
    public int StatusCode { get; set; } = 200;
    public bool Success { get; set; } = true;
    public string? Error { get; set; } = string.Empty;
    public object? Result { get; set; }
}
public class AppUserDTO
{
    public string Id { get; set; }
    public bool isDeleted { get; set; } = false;
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public bool approved { get; set; }
    public string CustomerInfoCustId { get; set; }
    public int TenantId { get; set; }
}

public class AfTalkingSmsMessageResponse
{
    public SMSMessageData SMSMessageData { get; set; }

}
    
public class Recipient
{
    public string cost { get; set; }
    public string messageId { get; set; }
    public int messageParts { get; set; }
    public string number { get; set; }
    public string status { get; set; }
    public int statusCode { get; set; }
}

public class SMSMessageData
{
    public string Message { get; set; }
    public List<Recipient> Recipients { get; set; }
}   