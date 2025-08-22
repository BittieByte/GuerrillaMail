# GuerrillaMailClient C# Example
Website: https://www.guerrillamail.com/

API Docs: https://www.guerrillamail.com/GuerrillaMailAPI.html

This is a simple example showing how to use the `GuerrillaMailClient` library in C# to create temporary email addresses, read emails, delete them, and optionally set a custom username.

## Example Usage

```csharp
static async Task Main()
{
    // Replace with your own IP & User-Agent
    var client = new GuerrillaMailClient(string.Empty, "MyApp/1.0");

    // 1. Get or create a new temp email address
    var emailInfo = await client.GetEmailAddressAsync();
    Console.WriteLine($"Your temp email: {emailInfo?.EmailAddr}");

    // 1a. Try to set a custom username
    try
    {
        var newEmail = await client.SetEmailUserAsync("mydesiredname", throwIfMismatch: true);
        Console.WriteLine($"Assigned email after username change: {newEmail?.EmailAddr}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Could not set desired username: {ex.Message}");
    }

    // 2. Check for new emails (polling)
    var inbox = await client.CheckEmailAsync(1); // Use 1 to Skip service email
    Console.WriteLine($"You have {inbox?.Count} emails");

    foreach (var email in inbox.Emails)
    {
        Console.WriteLine($"New email from: {email.From}");
        Console.WriteLine($"Subject: {email.Subject}");

        // 3. Fetch full email content
        var fullEmail = await client.FetchEmailAsync(email.Id);
        Console.WriteLine("--- Email Body ---");
        Console.WriteLine(fullEmail?.Body);
    }

    // 4. Delete all emails in inbox
    if(inbox.Count > 0)
    {
        var response = await client.DeleteEmailAsync(inbox.Emails.Select(x => x.Id));
        Console.WriteLine($"Deleted IDs: {string.Join(',', response.DeletedIds)}");
    }

    //// 5. Extend email life by 1 hour
    //var extended = await client.ExtendAsync();
    //Console.WriteLine($"Extended: {extended?.Affected == 1}, Expires? {extended?.Expired}");
}
```

## Features Demonstrated

1. **Create/Get a temporary email address**  
2. **Optionally set a custom username for the email**  
3. **Check inbox for new emails**  
4. **Fetch full email content**  
5. **Delete emails**  
6. **Extend the life of your temp email**  

## Notes

- Ensure your IP and User-Agent are set correctly.
- Polling too frequently may hit API limits.
- Username change may fail if the desired name is already taken; `throwIfMismatch` controls behavior.

