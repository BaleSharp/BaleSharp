using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using Bale.Enums;

namespace Bale.Objects
{
    public class PreCheckoutQuery
    {
        public string id { get; set; }
        public User from { get; set; }
        public string currency { get; set; }
        public int total_amount { get; set; }
        public string invoice_payload { get; set; }
        [JsonIgnore]
        public Client client { get; internal set; }
        public async void answer(bool ok, string errorMsg = null)
        {
            var dict = new Dictionary<string, object>
            {
                {"pre_checkout_query_id", this.id},
                {"ok", ok}
            };
            if (!ok)
            {
                if (!string.IsNullOrEmpty(errorMsg))
                    dict.Add("error_message", errorMsg);
                else
                {
                    throw new ArgumentException("When a preCheckoutQuery is not ok, you have to fill the error parameter", nameof(errorMsg));
                }
            }
            await client.ExecuteAsync("answerPreCheckoutQuery", dict);
        }
    }
    public class SuccessfulPayment
    {
        public string currency { get; set; }
        public int total_amount { get; set; }
        public string invoice_payload { get; set; }
        public string telegram_payment_charge_id { get; set; }
        [JsonProperty("provider_payment_charge_id")]
        public string trackingNumber { get; set; }
    }
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

    public class Sticker
    {
        public string file_id { get; set; }
        public string file_unique_id { get; set; }
        public StickerType type { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int? file_size { get; set; }
    }

    public class StickerSet
    {
        public string name { get; set; }
        public string title { get; set; }
        public Sticker[] stickers { get; set; }
        public PhotoSize? thumbnail { get; set; }
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
        public ChatType type { get; set; }
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

        public async Task<Message> answer(string text, InlineKeyboardMarkup markup)
        {
            if (markup == null)
            {
                Message tmp = await this.message.client.SendMessage(this.from.id, text);
                return tmp;
            }
            else
            {
                Message tmp = await this.message.client.SendMessage(this.from.id, text, markup);
                return tmp;
            }
        }

        public async Task<Message> answer(string text, ReplyKeyboardMarkup markup)
        {
            if (markup == null)
            {
                Message tmp = await this.message.client.SendMessage(this.from.id, text);
                return tmp;
            }
            else
            {
                Message tmp = await this.message.client.SendMessage(this.from.id, text, markup);
                return tmp;
            }
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

    public class Update
    {
        public int update_id { get; set; }
        public Message? message { get; set; }
        public Message? edited_message { get; set; }
        public CallbackQuery? callback_query { get; set; }
        public PreCheckoutQuery? pre_checkout_query { get; set; }
        [JsonIgnore]
        public Client client { get; internal set; }
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
        public void set_state(string state)
        {
            StateMachine.SetState(this.id, state);
        }
        public void del_state()
        {
            StateMachine.DeleteState(id);
        }

        public string get_state()
        {
            return StateMachine.GetState(this.id);
        }

    }

    public class LabeledPrice
    {
        public string label { get; set; }
        public int amount { get; set; }
    }
    public class Message
    {
        [JsonProperty("message_id")]
        public int id { get; set; }
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
        public PhotoSize? photo { get; set; }
        public Sticker? sticker { get; set; }
        public Video? video { get; set; }  // اختیاری  
        public Voice? voice { get; set; }  // اختیاری  
        public string? caption { get; set; }  // اختیاری  
        public Contact? contact { get; set; }  // اختیاری  
        public Location? location { get; set; }  // اختیاری  
        public List<User>? new_chat_members { get; set; }  // اختیاری - آرایه  
        public User? left_chat_member { get; set; }  // اختیاری  
        public SuccessfulPayment? successful_payment { get; set; }
        public InlineKeyboardButton? reply_markup { get; set; }  // اختیاری  

        [JsonIgnore]
        public Client client { get; internal set; }

        public async Task<Message> reply(string text, InlineKeyboardMarkup markup = null)
        {
            if (markup == null)
            {
                Message tmp = await this.client.SendMessage(this.from.id, text);
                return tmp;
            }
            else
            {
                Message tmp = await this.client.SendMessage(this.from.id, text, markup);
                return tmp;
            }
        }

        public async Task<Message> reply(string text, ReplyKeyboardMarkup markup = null)
        {
            if (markup == null)
            {
                Message tmp = await this.client.SendMessage(this.from.id, text);
                return tmp;
            }
            else
            {
                Message tmp = await this.client.SendMessage(this.from.id, text, markup);
                return tmp;
            }
        }

        public async void pin()
        {
            client.PinMessage(this, this.chat.id);
        }

        public async Task<Message> edit(string text)
        {
            Message tmp = await this.client.editTextMessage(this, text);
            return tmp;
        }
        public async Task<bool> delete()
        {
            bool res = await this.client.DeleteMessage(this);
            return res;
        }
        public async Task<int> copy(long ChatID)
        {
            int res = await client.CopyMessage(this, ChatID);
            return res;
        }
        public async Task<Message> forward(long ChatID)
        {
            Message res = await client.ForwardMessage(this, ChatID);
            return res;
        }
        public async void FullAdmin(long userID)
        {
            try
            {
                var dict = new Dictionary<string, object>
                {
                    {"chat_id", this.chat.id},
                    {"user_id", userID},
                    {"can_change_info", true},
                    {"can_post_messages", true},
                    {"can_edit_messages", true},
                    {"can_delete_messages", true},
                    {"can_manage_video_chats", true},
                    {"can_invite_users", true},
                    {"can_restrict_members", true}
                };
                await client.ExecuteAsync("promoteChatMember", dict);
            }
            catch
            {
                throw new Exception("Got an error during setting permissions");
            }
        }
    }
}

