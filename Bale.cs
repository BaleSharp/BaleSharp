using System.Reflection.Metadata;
using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Collections.Concurrent;
using Bale.Objects;
using Bale.Enums;

namespace Bale
{
    public delegate Task MessageHandler(Objects.Message message);
    public delegate Task CallbackQueryHandler(Objects.CallbackQuery callbackQuery);
    public delegate Task CommandHandler(Objects.Message message, string command, string[] args);
    public delegate Task PaymentHandler(Objects.Message message, Objects.SuccessfulPayment payment);
    public delegate Task PreCheckoutQueryHandler(Objects.PreCheckoutQuery precheckoutquery);

    



    public class Client
    {
        protected readonly string _token;
        protected readonly string baseUrl;

        public MessageHandler OnMessage { get; set; }
        public CommandHandler OnCommand { get; set; }
        public CallbackQueryHandler OnCallbackQuery { get; set; }
        public PaymentHandler OnSuccessfulPayment { get; set; }
        public PreCheckoutQueryHandler OnPreCheckoutQuery { get; set; }

        private bool _isReceiving;
        private int _lastUpdateId;
        public Client(string token, string? _baseUrl = null)
        {
            if (!string.IsNullOrEmpty(_baseUrl)) baseUrl = _baseUrl;
            else baseUrl = "https://tapi.bale.ai/bot";
            _token = token;
        }

        public async void StartReceiving()
        {
            if (_isReceiving) return;

            _isReceiving = true;
            await Task.Run(ReceiveUpdates);
            var me = await this.getMe();
            Console.WriteLine($"--({me.username}) Started getting updates...");
            Console.WriteLine("Powered by BaleSharp\nDocs at : @BaleSharp");
        }

        public async void StopReceiving()
        {
            _isReceiving = false;
            var me = await this.getMe();
            Console.WriteLine($"--({me.username}) Stopped getting updates--");
        }

        private async Task ReceiveUpdates()
        {
            while (_isReceiving)
            {
                try
                {
                    var updates = await this.GetUpdates(_lastUpdateId + 1);

                    foreach (var update in updates)
                    {
                        _lastUpdateId = update.update_id;
                        if (update.pre_checkout_query != null)
                        {
                            await OnPreCheckoutQuery(update.pre_checkout_query);
                        }
                        else if (update.message != null)
                        {
                            if (update.message.successful_payment != null)
                            {
                                await OnSuccessfulPayment(update.message, update.message.successful_payment);
                            }

                            // Handle regular messages
                            if (OnMessage != null)
                            {
                                if (update.message.text?.StartsWith("/") != true && update.message.successful_payment != null)
                                    await OnMessage(update.message);
                            }

                            // Handle commands
                            if (update.message.text?.StartsWith("/") == true && OnCommand != null)
                            {
                                string[] parts = update.message.text.Split(' ');
                                var command = parts[0][1..]; // Remove the '/'
                                var args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();
                                await OnCommand(update.message, command, args);
                            }
                        }
                        else if (update.callback_query != null && OnCallbackQuery != null)
                        {
                            await OnCallbackQuery(update.callback_query);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving updates: {ex.Message}");
                    await Task.Delay(1000);
                }
            }
        }

        // Optionally expose public accessors if needed  
        public string Token => _token;
        public string BaseUrl => baseUrl;
    }
    public class ApiResponse<T>
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("result")]
        public T Result { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("error_code")]
        public int? ErrorCode { get; set; }
    }

    public static class Bot
    {
        private static readonly HttpClient _client = new HttpClient();

        public static async Task<string> ExecuteAsync(this Client client, string method, Dictionary<string, object>? parameters = null)
        {
            string url = $"{client.BaseUrl}{client.Token}/{method}";

            if (parameters != null && parameters.Count != 0)
            {
                url += "?";
                foreach (var param in parameters)
                {
                    url += $"{param.Key}={param.Value}&";
                }

                url = url.TrimEnd('&');
            }


            HttpResponseMessage res = await _client.GetAsync(url);
            if (res.IsSuccessStatusCode)
            {
                string content = await res.Content.ReadAsStringAsync();
                return content;
            }
            else
            {
                string content = "error";
                return content;
            }

        }

        public static async Task<Objects.User> getMe(this Client client)
        {
            string res = await client.ExecuteAsync("getme");
            var tmp = JsonConvert.DeserializeObject<ApiResponse<Objects.User>>(res);
            return tmp.Result;
        }
        public static async Task<Objects.Chat> getChat(this Client client, long ChatID)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", ChatID}
            };
            string res = await client.ExecuteAsync("getChat", dict);
            var tmp = JsonConvert.DeserializeObject<ApiResponse<Objects.Chat>>(res);
            return tmp.Result;
        }

        public static async Task<Objects.Message> SendMessage(this Client client, long ChatID, string text, object? reply_markup = null, int? reply_to_id = null)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", ChatID},
                {"text", text}
            };
            if (reply_to_id != null)
            {
                dict.Add("reply_to_message_id", reply_to_id);
            }
            if (reply_markup != null)
            {
                switch (reply_markup)
                {
                    case Objects.ReplyKeyboardMarkup reply:
                    case Objects.InlineKeyboardMarkup inline:
                        dict.Add("reply_markup", JsonConvert.SerializeObject(reply_markup));
                        break;
                    default:
                        throw new ArgumentException(
                            "reply_markup must be either ReplyKeyboardMarkup or InlineKeyboardMarkup",
                            nameof(reply_markup)
                        );
                }
            }
            string res = await client.ExecuteAsync("sendMessage", dict);
            var m = JsonConvert.DeserializeObject<ApiResponse<Objects.Message>>(res);
            return m.Result;
        }
        public static async Task<Objects.WebhookInfo> GetWebhook(this Client client)
        {
            string res = await client.ExecuteAsync("getWebhookInfo");
            var tmp = JsonConvert.DeserializeObject<ApiResponse<Objects.WebhookInfo>>(res);
            return tmp.Result;
        }
        public static async Task<Objects.WebhookInfo> SetWebhook(this Client client, string url)
        {
            var dict = new Dictionary<string, object>
            {
                {"url", url}
            };
            await client.ExecuteAsync("setWebhook", dict);
            return await client.GetWebhook();
        }
        public static async void DeleteWebhook(this Client client)
        {
            string res = await client.ExecuteAsync("setWebhook");
            
        }


        public static async void sendChatAction(this Client client, long chatID, Enums.ChatAction mode)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", chatID},
                {"action", mode.ActionEncode()}
            };
            await client.ExecuteAsync("sendChatAction", dict);
        }
        public static async void sendInvoice(this Client client, long chat_id, string title, string description, string payload, string provider_token, Objects.LabeledPrice[] prices, string? photo_url = null)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", chat_id},
                {"title", title},
                {"description", description},
                {"payload", payload},
                {"provider_token", provider_token},
                {"prices", JsonConvert.SerializeObject(prices)},
            };
            if (photo_url != null)
            {
                dict.Add("photo_url", photo_url);
            }
            await client.ExecuteAsync("sendInvoice", dict);
        }
        public static async Task<Objects.Message> editTextMessage(this Client client, Objects.Message msg, string text)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", msg.chat.id},
                {"message_id", msg.id},
                {"text", text}
            };
            string res = await client.ExecuteAsync("editMessageText", dict);
            var m = JsonConvert.DeserializeObject<ApiResponse<Objects.Message>>(res);
            return m.Result;
        }

        public static async Task<int> CopyMessage(this Client client, Message msg, long ChatID)
        {
            ///<summary>copies every type of message into another chat</summary>
            /// <param name="msg">the message that you want to copy</param>
            /// <param name="ChatID">The ChatID you want to copy the message in it</param>
            /// <returns>id of copied message</returns>
            var dict = new Dictionary<string, object>
            {
                {"chat_id", ChatID},
                {"from_chat_id", msg.chat.id},
                {"message_id", msg.id}
            };
            string res = await client.ExecuteAsync("copyMessage", dict);
            var tmp = JsonConvert.DeserializeObject<ApiResponse<int>>(res);
            return tmp.Result;
        }
        public static async Task<Message> ForwardMessage(this Client client, Message msg, long ChatID)
        {
            ///<summary>forwards message into another chat</summary>
            /// <param name="msg">the message that you want to forward</param>
            /// <param name="ChatID">The ChatID you want to forward the message in it</param>
            /// <returns>forwarded message</returns>
            var dict = new Dictionary<string, object>
            {
                {"chat_id", ChatID},
                {"from_chat_id", msg.chat.id},
                {"message_id", msg.id}
            };
            string res = await client.ExecuteAsync("forwardMessage", dict);
            var tmp = JsonConvert.DeserializeObject<ApiResponse<Message>>(res);
            return tmp.Result;
        }
        public static async Task<bool> DeleteMessage(this Client client, Objects.Message msg)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", msg.chat.id},
                {"message_id", msg.id}
            };
            string res = await client.ExecuteAsync("deleteMessage", dict);
            var m = JsonConvert.DeserializeObject<ApiResponse<bool>>(res);
            return m.Result;
        }
        public static async Task<bool> leaveChat(this Client client, long chatID)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", chatID}
            };
            string res = await client.ExecuteAsync("leaveChat", dict);
            var tmp = JsonConvert.DeserializeObject<ApiResponse<bool>>(res);
            return tmp.Result;
        }
        public static async Task<Objects.Message> reply_to(this Client client, Objects.Message msg, string text, object? reply_markup = null)
        {
            Objects.Message m = await client.SendMessage(msg.chat.id, text, reply_markup, msg.id);
            return m;
        }
        public static async Task<Objects.Update[]> GetUpdates(this Client client, int offset, int? timout = 30)
        {
            var dict = new Dictionary<string, object>
            {
                {"offset", offset},
                {"timout", timout}
            };

            string res = await client.ExecuteAsync("getUpdates", dict);
            var response = JsonConvert.DeserializeObject<ApiResponse<Objects.Update[]>>(res);
            if (response?.Result != null)
            {
                foreach (var update in response.Result)
                {
                    update.client = client;

                    if (update.pre_checkout_query != null)
                    {
                        update.pre_checkout_query.client = client;
                    }

                    if (update.message != null)
                        update.message.client = client;

                    if (update.edited_message != null)
                        update.edited_message.client = client;

                    if (update.callback_query?.message != null)
                        update.callback_query.message.client = client;
                }
            }

            return response?.Result ?? Array.Empty<Objects.Update>();
        }
    }
}
