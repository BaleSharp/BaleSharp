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


int YOUR_CHAT_ID = 000000000;
string token = "TOKEN";
Bale.Client cli = new Client(token);

cli.testMessage(YOUR_CHAT_ID);


try
{
    cli.OnCommand = async (message, command, args) =>
    {
        switch (command)
        {
            case "start": await cli.reply_to(message, "Welcome to this bot, This bot is made by BaleSharp"); break;
        }
    };

    cli.OnMessage = async (message) =>
    {
        switch (message.text)
        {
            case "Inline":
                var key = new InlineKeyboardBuilder().AddButton("CallbackButton", "call")
                .NewRow()
                .AddButton("urlButton", url: "https://ble.ir/BaleSharp")
                .NewRow()
                .AddButton("copyTextButton", copyText:"text is copied")
                .Build();
                await cli.reply_to(message, "Here is some of Inline buttons", key); 
                break;
            case "Reply":
                var keyboard = new ReplyKeyboardBuilder().AddButton("Made by")
                .NewRow()
                .AddButton("BaleSharp")
                .Build();
                await cli.reply_to(message, "Here are some ReplyKeyboard buttons", keyboard);
                break;
            default:
                await cli.reply_to(message, $"You said : {message.text}");
                break;
        }
    };

    
    cli.OnCallbackQuery = async (callback_query) =>
    {
        switch (callback_query.data)
        {
            case "call":
                await cli.SendMessage(callback_query.from.id, "callback_query button clicked!");
                break;
        }
    };
    cli.StartReceiving();
    await Task.Delay(-1);
}catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    cli.StopReceiving();
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
