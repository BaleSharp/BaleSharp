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
    public delegate Task CommandHandler(Bale.Objects.Message message, string command, string[] args);
    
    public class ReplyKeyboardBuilder
    {
        private List<List<Objects.KeyboardButton>> _keyboard;
        private List<Objects.KeyboardButton> _currentRow;

        public ReplyKeyboardBuilder()
        {
            _keyboard = new List<List<Objects.KeyboardButton>>();
            _currentRow = new List<Objects.KeyboardButton>();
        }

        public ReplyKeyboardBuilder AddButton(string text, bool requestContact = false, bool requestLocation = false, Objects.WebAppInfo webApp = null)
        {
            _currentRow.Add(new Objects.KeyboardButton
            {
                text = text,
                request_contact = requestContact ? true : null,
                request_location = requestLocation ? true : null,
                web_app = webApp
            });
            return this;
        }

        public ReplyKeyboardBuilder NewRow()
        {
            if (_currentRow.Count > 0)
            {
                _keyboard.Add(_currentRow);
                _currentRow = new List<Objects.KeyboardButton>();
            }
            return this;
        }

        public Objects.ReplyKeyboardMarkup Build()
        {
            if (_currentRow.Count > 0)
            {
                _keyboard.Add(_currentRow);
            }
            return new Objects.ReplyKeyboardMarkup { keyboard = _keyboard };
        }
    }
    
    public class InlineKeyboardBuilder
    {
        private List<List<Objects.InlineKeyboardButton>> _keyboard;
        private List<Objects.InlineKeyboardButton> _currentRow;

        public InlineKeyboardBuilder()
        {
            _keyboard = new List<List<Objects.InlineKeyboardButton>>();
            _currentRow = new List<Objects.InlineKeyboardButton>();
        }

        public InlineKeyboardBuilder AddButton(string text, string callbackData = null, string url = null, Objects.WebAppInfo webApp = null, string copyText = null)
        {
            _currentRow.Add(new Objects.InlineKeyboardButton
            {
                text = text,
                callback_data = callbackData,
                url = url,
                web_app = webApp,
                copy_text = copyText != null ? new Objects.CopyTextButton { text = copyText } : null
            });
            return this;
        }

        public InlineKeyboardBuilder NewRow()
        {
            if (_currentRow.Count > 0)
            {
                _keyboard.Add(_currentRow);
                _currentRow = new List<Objects.InlineKeyboardButton>();
            }
            return this;
        }

        public Objects.InlineKeyboardMarkup Build()
        {
            if (_currentRow.Count > 0)
            {
                _keyboard.Add(_currentRow);
            }
            return new Objects.InlineKeyboardMarkup { inline_keyboard = _keyboard };
        }
    }
    public static class StateMachine
    {
        private static readonly ConcurrentDictionary<int, string> _userStates = new ConcurrentDictionary<int, string>();

        /// <summary>
        /// Gets the current state for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Current state or null if no state exists</returns>
        public static string GetState(int userId)
        {
            return _userStates.TryGetValue(userId, out var state) ? state : null;
        }

        /// <summary>
        /// Sets or updates the state for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="state">New state value</param>
        public static void SetState(int userId, string state)
        {
            _userStates.AddOrUpdate(userId, state, (_, __) => state);
        }

        /// <summary>
        /// Deletes the state for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>True if state was deleted, false if user had no state</returns>
        public static bool DeleteState(int userId)
        {
            return _userStates.TryRemove(userId, out _);
        }
    }

    
    public class Client
    {
        protected readonly string _token;
        protected readonly string baseUrl;

        public MessageHandler OnMessage { get; set; }
        public CommandHandler OnCommand { get; set; }
        public CallbackQueryHandler OnCallbackQuery { get; set; }

        private bool _isReceiving;
        private int _lastUpdateId;
        public Client(string token, string? _baseUrl = null)
        {
            if (!string.IsNullOrEmpty(_baseUrl)) baseUrl = _baseUrl;
            else baseUrl = "https://tapi.bale.ai/bot";
            _token = token;
        }

        public void StartReceiving()
        {
            if (_isReceiving) return;

            _isReceiving = true;
            Task.Run(ReceiveUpdates);
        }

        public void StopReceiving()
        {
            _isReceiving = false;
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

                        if (update.message != null)
                        {
                            // Handle regular messages
                            if (OnMessage != null)
                            {
                                if (update.message.text?.StartsWith("/") != true)
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
                // Remove the trailing '&' if parameters were added
                url = url.TrimEnd('&');
            }

            HttpResponseMessage res = await _client.GetAsync(url);
            try { res.EnsureSuccessStatusCode(); }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            string content = await res.Content.ReadAsStringAsync();
            return content;
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
            await client.ExecuteAsync("setWebhook");

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
        public static async void deleteMessage(this Client client, Objects.Message msg)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", msg.chat.id},
                {"message_id", msg.id}
            };
            await client.ExecuteAsync("deleteMessage", dict);
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
