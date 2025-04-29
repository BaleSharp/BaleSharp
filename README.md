# baleSharp

**baleSharp** is a powerful and easy-to-use C# library to create bots for Bale Messenger. Designed for developers aiming to build efficient, scalable, and maintainable bots with minimal effort.  

---  

## Features  

- Intuitive API tailored for Bale Messenger bot development  
- Support for message handling, event listening, and reply management  
- Fully asynchronous operations for high performance  
- Easy integration with existing C# projects  
- Flexible and extensible architecture  
- Compatible with .NET 8.0 and newer  

---  

## Example
```cs
using Bale;
using Newtonsoft.Json;


int YOUR_CHAT_ID = 2035204913;
string token = "1782917547:nJGgN2rH1fyXDYe9JnfXWai";
Bale.Client cli = new Client(token);
Bale.Message m = await cli.SendMessage(YOUR_CHAT_ID, "This is the test of the First BaleSharp bot, Made by CodeWizaard");

Bale.User me = await cli.getMe();

Console.WriteLine($"bot {me.first_name} started...");


int lastUpdateId = 0;

while (true)
{
    try
    {
        // Create parameters with offset to get only new updates

        int tmp = lastUpdateId + 1;
        Update[] u = await cli.GetUpdates(tmp);
        if (u != null && u.Length > 0)
        {
            foreach (Bale.Update update in u)
            {
                // Update our last processed ID
                lastUpdateId = update.update_id;

                // Check if this is a message with text
                if (update.message?.text != null)
                {
                    Console.WriteLine($"Received: {update.message.text}");
                    await cli.reply_to(update.message, $"Echo : {update.message.text}");
                }
            }
        }
        else
        {
            Console.WriteLine("No new updates...");
        }
    }
    catch (Exception ex)
    {
        await cli.SendMessage(YOUR_CHAT_ID, $"Error : {ex.Message}");
        break;
    }

    // Add a small delay to avoid hitting rate limits
    await Task.Delay(1000);
}
```
---

## Installation  

Install the balebot package via NuGet:  

```bash  
dotnet add package BaleSharp
  ```

Docs and News : [@BaleSharp](https://ble.ir/BaleSharp)
Support : [@BleSharpGP](https://ble.ir/BleSharpGP)
