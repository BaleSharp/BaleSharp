using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bale.Objects
{

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

        public async Task<Message> answer(string text)
        {
            Message tmp = await this.message.client.SendMessage(this.from.id, text);
            return tmp;
        }

    }
    public class Update
    {
        public int update_id { get; set; }
        public Message message { get; set; }
        public Message edited_message { get; set; }
        public CallbackQuery callback_query { get; set; }
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
            StateMachine.SetState(id, state);
        }
        public void del_state()
        {
            StateMachine.DeleteState(id);
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
        public Video? video { get; set; }  // اختیاری  
        public Voice? voice { get; set; }  // اختیاری  
        public string? caption { get; set; }  // اختیاری  
        public Contact? contact { get; set; }  // اختیاری  
        public Location? location { get; set; }  // اختیاری  
        public List<User>? new_chat_members { get; set; }  // اختیاری - آرایه  
        public User? left_chat_member { get; set; }  // اختیاری  
        public InlineKeyboardButton? reply_markup { get; set; }  // اختیاری  

        [JsonIgnore]
        public Client client { get; internal set; }

        public async Task<Message> reply(string text)
        {
            Message tmp = await this.client.SendMessage(this.from.id, text);
            return tmp;
        }
        public async Task<Message> edit(string text)
        {
            Message tmp = await this.client.editTextMessage(this, text);
            return tmp;
        }
        public async void FullAdmin(long userID)
        {
            try
            {
                var dict = new Dictionary<string, object>
                {
                    {"chat_id", this.id},
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

