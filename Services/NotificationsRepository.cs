using System.Collections;
using Domain.Responses;
using AfricasTalkingCS;

namespace Infrastructure.Services;

public class NotificationsRepository : ISmsRepository
{

    public async Task<(bool, string)> SendNotification(string recipientNumber)
    {
        const string username = "sandbox"; // Replace with your actual username
        const string apiKey = "f10bdc5ab8a82081b07be6d28abcfc949ff713df1cb67c6c7db91ceaf1026b44"; // Replace with your actual API key

        var gateway = new AfricasTalkingGateway(username, apiKey);

        var opts = new Hashtable { ["keyword"] = "R56yyh%^&76fjkj" }; // Customize keyword
        var from = "32999"; // Replace with your short code
        const string message = "Notifications testing, romano";

        try
        {
            var res = await gateway.SendPremiumMessage(recipientNumber, message, from, 0, opts);

            if (res.IsSuccess)
            {
                Console.WriteLine("Premium SMS sent successfully!");
                return (true, $"Sending premium notification successful. Recipient: {recipientNumber}");
            }
            else
            {
                Console.WriteLine($"Failed to send premium SMS. Error Code: {res.ErrorCode}, Error Message: {res.ErrorMessage}");
                return (false, $"Sending premium notification Failed. Error: {res.ErrorMessage}");
            }
        }
        catch (AfricasTalkingGatewayException ex)
        {
            // Log the exception details (e.g., to a centralized logging system)
            Console.WriteLine($"Error sending premium SMS: {ex.Message}");
            return (false, "Sending premium notification Failed.");
        }
    }

    public async Task<bool> ProcessCallBackResult(object body)
    {
        try
        {
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
public interface ISmsRepository
{
    Task<(bool, string)> SendNotification(string phone);
    Task<bool> ProcessCallBackResult(object body);
}