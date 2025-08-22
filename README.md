# GuerrillaMailClient C# Example

This is a simple example showing how to use the `GuerrillaMailClient` library in C# to create temporary email addresses, read emails, delete them.

## Example Usage

```csharp
static async Task Main()
{
    // Replace with your own IP & User-Agent
    var client = new GuerrillaMailClient(string.Empty, "MyApp/1.0");

    // 1. Get or create a new temp email address
    var emailInfo = await client.GetEmailAddressAsync();
    Console.WriteLine($"Your temp email: {emailInfo?.EmailAddr}");

    // 2. Check for new emails (polling)
    var inbox = await client.CheckEmailAsync();
    Console.WriteLine($"You have {inbox?.Count} emails");

    foreach (var email in inbox.Emails)
    {
        Console.WriteLine($"New email from: {email.From}");
        Console.WriteLine($"Subject: {email.Subject}");

        // 4. Fetch full email content
        var fullEmail = await client.FetchEmailAsync(email.Id);
        Console.WriteLine("--- Email Body ---");
        Console.WriteLine(fullEmail?.Body);
    }
    var response = await client.DeleteEmailAsync(inbox.Emails.Select(x => x.Id));
    Console.WriteLine($"Deleted IDs: {string.Join(',', response.DeletedIds)}");

    //// 5. Extend email life by 1 hour
    //var extended = await client.ExtendAsync();
    //Console.WriteLine($"Extended: {extended?.Affected == 1}, Expires? {extended?.Expired}");
}
```

## Features Demonstrated

1. **Create/Get a temporary email address**  
2. **Check inbox for new emails**  
3. **Fetch full email content**  
4. **Delete emails**  
5. **Extend the life of your temp email**  

## Notes

- Ensure your IP and User-Agent are set correctly.
- Polling too frequently may hit API limits.
