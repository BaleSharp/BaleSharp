using System.Reflection.Metadata;
using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Collections.Concurrent;

namespace Bale
{
    public delegate Task MessageHandler(Message message);
    public delegate Task CallbackQueryHandler(CallbackQuery callbackQuery);
    public delegate Task CommandHandler(Message message, string command, string[] args);
    public class PhotoSize
    {
        public string file_id { get; set; }
        public string file_unique_id { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int? file_size { get; set; }
    }

    public class WebhookInfo
    {
        public string url { get; set; }
    }
    public class Animation
    {
        public string file_id { get; set; }
        public string file_unique_id { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int duration { get; set; }
        public PhotoSize thumbnail { get; set; }
        public string file_name { get; set; }
        public string mime_type { get; set; }
        public int? file_size { get; set; }
    }

    public class Audio
    {
        public string file_id { get; set; }
        public string file_unique_id { get; set; }
        public int duration { get; set; }
        public string title { get; set; }
        public string file_name { get; set; }
        public string mime_type { get; set; }
        public int? file_size { get; set; }
    }

    public class Document
    {
        public string file_id { get; set; }
        public string file_unique_id { get; set; }
        public PhotoSize thumbnail { get; set; }
        public string file_name { get; set; }
        public string mime_type { get; set; }
        public int? file_size { get; set; }
    }

    public class Video
    {
        public string file_id { get; set; }
        public string file_unique_id { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int duration { get; set; }
        public string file_name { get; set; }
        public string mime_type { get; set; }
        public int? file_size { get; set; }
    }

    public class Voice
    {
        public string file_id { get; set; }
        public string file_unique_id { get; set; }
        public int duration { get; set; }
        public string mime_type { get; set; }
        public int? file_size { get; set; }
    }

    public class Contact
    {
        public string phone_number { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public int? user_id { get; set; }
    }

    public class Location
    {
        public float longitude { get; set; }
        public float latitude { get; set; }
    }

    public class File
    {
        public string file_id { get; set; }
        public string file_unique_id { get; set; }
        public int? file_size { get; set; }
        public string file_path { get; set; }
    }

    public class WebAppInfo
    {
        public string url { get; set; }
    }

    public class CopyTextButton
    {
        public string text { get; set; }
    }

    public class KeyboardButton
    {
        public string text { get; set; }
        public bool? request_contact { get; set; }
        public bool? request_location { get; set; }
        public WebAppInfo web_app { get; set; }
    }

    public class ReplyKeyboardMarkup
    {
        public List<List<KeyboardButton>> keyboard { get; set; }
    }

    public class InlineKeyboardButton
    {
        public string text { get; set; }
        public string url { get; set; }
        public string callback_data { get; set; }
        public WebAppInfo web_app { get; set; }
        public CopyTextButton copy_text { get; set; }
    }

    public class InlineKeyboardMarkup
    {
        public List<List<InlineKeyboardButton>> inline_keyboard { get; set; }
    }


    public class Chat
    {
        public long id { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public string username { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }

    public class CallbackQuery
    {
        public string id { get; set; }
        public User from { get; set; }
        public Message message { get; set; }
        public string inline_message_id { get; set; }
        public string data { get; set; }
    }
    public class Update
    {
        public int update_id { get; set; }
        public Message message { get; set; }
        public Message edited_message { get; set; }
        public CallbackQuery callback_query { get; set; }
    }
    public class Updates
    {
        public Update[] UpdateList { get; set; }
    }
    public class Messages
    {
        public int id { get; set; }
    }
    public class User
    {
        public int id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string? last_name { get; set; }
        public string? username { get; set; }
        public string? language_code { get; set; }
    }

    public class LabeledPrice
    {
        public string label { get; set; }
        public int amount { get; set; }
    }
    public class Message
    {
        public int message_id { get; set; }
        public User? from { get; set; }  // اختیاری  
        public int date { get; set; }
        public Chat chat { get; set; }
        public User? forward_from { get; set; }  // اختیاری  
        public Chat? forward_from_chat { get; set; }  // اختیاری  
        public int? forward_from_message_id { get; set; }  // اختیاری  
        public int? forward_date { get; set; }  // اختیاری  
        public Message? reply_to_message { get; set; }  // اختیاری  
        public int? edit_date { get; set; }  // اختیاری  
        public string? text { get; set; }  // اختیاری UTF-8  
        public Animation? animation { get; set; }  // اختیاری  
        public Audio? audio { get; set; }  // اختیاری  
        public Document? document { get; set; }  // اختیاری
        public Video? video { get; set; }  // اختیاری  
        public Voice? voice { get; set; }  // اختیاری  
        public string? caption { get; set; }  // اختیاری  
        public Contact? contact { get; set; }  // اختیاری  
        public Location? location { get; set; }  // اختیاری  
        public List<User>? new_chat_members { get; set; }  // اختیاری - آرایه  
        public User? left_chat_member { get; set; }  // اختیاری  
        public InlineKeyboardButton? reply_markup { get; set; }  // اختیاری  
    }
    public class ReplyKeyboardBuilder
    {
        private List<List<KeyboardButton>> _keyboard;
        private List<KeyboardButton> _currentRow;

        public ReplyKeyboardBuilder()
        {
            _keyboard = new List<List<KeyboardButton>>();
            _currentRow = new List<KeyboardButton>();
        }

        public ReplyKeyboardBuilder AddButton(string text, bool requestContact = false, bool requestLocation = false, WebAppInfo webApp = null)
        {
            _currentRow.Add(new KeyboardButton
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
                _currentRow = new List<KeyboardButton>();
            }
            return this;
        }

        public ReplyKeyboardMarkup Build()
        {
            if (_currentRow.Count > 0)
            {
                _keyboard.Add(_currentRow);
            }
            return new ReplyKeyboardMarkup { keyboard = _keyboard };
        }
    }
    
    public class InlineKeyboardBuilder
    {
        private List<List<InlineKeyboardButton>> _keyboard;
        private List<InlineKeyboardButton> _currentRow;

        public InlineKeyboardBuilder()
        {
            _keyboard = new List<List<InlineKeyboardButton>>();
            _currentRow = new List<InlineKeyboardButton>();
        }

        public InlineKeyboardBuilder AddButton(string text, string callbackData = null, string url = null, WebAppInfo webApp = null, string copyText = null)
        {
            _currentRow.Add(new InlineKeyboardButton
            {
                text = text,
                callback_data = callbackData,
                url = url,
                web_app = webApp,
                copy_text = copyText != null ? new CopyTextButton { text = copyText } : null
            });
            return this;
        }

        public InlineKeyboardBuilder NewRow()
        {
            if (_currentRow.Count > 0)
            {
                _keyboard.Add(_currentRow);
                _currentRow = new List<InlineKeyboardButton>();
            }
            return this;
        }

        public InlineKeyboardMarkup Build()
        {
            if (_currentRow.Count > 0)
            {
                _keyboard.Add(_currentRow);
            }
            return new InlineKeyboardMarkup { inline_keyboard = _keyboard };
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

        public static async Task<User> getMe(this Client client)
        {
            string res = await client.ExecuteAsync("getme");
            var tmp = JsonConvert.DeserializeObject<ApiResponse<User>>(res);
            return tmp.Result;
        }
        public static async Task<Chat> getChat(this Client client, long ChatID)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", ChatID}
            };
            string res = await client.ExecuteAsync("getChat", dict);
            var tmp = JsonConvert.DeserializeObject<ApiResponse<Chat>>(res);
            return tmp.Result;
        }

        public static async Task<Message> SendMessage(this Client client, long ChatID, string text, object? reply_markup = null, int? reply_to_id = null)
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
                    case ReplyKeyboardMarkup reply:
                    case InlineKeyboardMarkup inline:
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
            var m = JsonConvert.DeserializeObject<ApiResponse<Message>>(res);
            return m.Result;
        }
        public static async Task<WebhookInfo> GetWebhook(this Client client)
        {
            string res = await client.ExecuteAsync("getWebhookInfo");
            var tmp = JsonConvert.DeserializeObject<ApiResponse<WebhookInfo>>(res);
            return tmp.Result;
        }
        public static async Task<WebhookInfo> SetWebhook(this Client client, string url)
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
        public static async void sendInvoice(this Client client, long chat_id, string title, string description, string payload, string provider_token, LabeledPrice[] prices, string? photo_url = null)
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
        public static async Task<Message> editTextMessage(this Client client, Message msg, string text)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", msg.chat.id},
                {"message_id", msg.message_id},
                {"text", text}
            };
            string res = await client.ExecuteAsync("editMessageText", dict);
            var m = JsonConvert.DeserializeObject<ApiResponse<Message>>(res);
            return m.Result;
        }
        public static async void deleteMessage(this Client client, Message msg)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", msg.chat.id},
                {"message_id", msg.message_id}
            };
            await client.ExecuteAsync("deleteMessage", dict);
        }
        public static async Task<Message> reply_to(this Client client, Message msg, string text, object? reply_markup = null)
        {
            Message m = await client.SendMessage(msg.chat.id, text, reply_markup, msg.message_id);
            return m;
        }
        public static async Task<Update[]> GetUpdates(this Client client, int offset, int? timout = 30)
        {
            var dict = new Dictionary<string, object>
            {
                {"offset", offset},
                {"timout", timout}
            };

            string res = await client.ExecuteAsync("getUpdates", dict);
            var u = JsonConvert.DeserializeObject<ApiResponse<Update[]>>(res);
            return u.Result;
        }
    }
}
