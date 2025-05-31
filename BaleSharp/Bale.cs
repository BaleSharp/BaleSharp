using Bale;
using Bale.Objects;
using Bale.Helpers;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Diagnostics;
using Bale.Enums;

namespace Bale
{   
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
                string content = $"error:{res.StatusCode}";
                return content;
            }

        }

        public static async Task<string> ExecuteMultipart(this Client client, string method, FileInfo file, Dictionary<string, object>? parameters = null, string fileFormFieldName = "photo")
        {
            string url = $"{client.BaseUrl}{client.Token}/{method}";

            using var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data");

            using var form = new MultipartFormDataContent
            {
                { fileContent, fileFormFieldName, file.Name }
            };

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    form.Add(new StringContent(param.Value.ToString()), param.Key);
                }
            }

            var response = await _client.PostAsync(url, form);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                return content;
            }
            else
            {
                string content = $"error:{response.StatusCode}";
                return content;
            }
        }

        public static async Task<Message> SendVideo(this Client client, long ChatID, FileInfo file, string? caption = null)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", ChatID}
            };
            if (!string.IsNullOrEmpty(caption))
            {
                dict.Add("caption", caption);
            }
            string res = await client.ExecuteMultipart("sendVideo", file, dict, fileFormFieldName: "video");
            return JsonConvert.DeserializeObject<ApiResponse<Objects.Message>>(res).Result;
        }

        public static async Task<Objects.Transaction> InquireTransaction(this Client client, string transaction_id)
        {
            var dict = new Dictionary<string, object>
            {
                {"transaction_id", transaction_id}
            };
            string res = await client.ExecuteAsync("inquireTransaction", dict);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<Objects.Transaction>>(res, options);

            return apiResponse.Result;
        }

        public static async Task<Objects.Message> SendVideo(this Client client, long chatID, string FileIdOrURL, string? caption = null)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", chatID},
                {"from_chat_id", null},
                {"video", FileIdOrURL}
            };
            if (!string.IsNullOrEmpty(caption))
            {
                dict.Add("caption", caption);
            }
            string res = await client.ExecuteAsync("sendVideo", dict);
            var tmp = JsonConvert.DeserializeObject<ApiResponse<Message>>(res);
            return tmp.Result;
        }

        public static async Task<Message> SendDocument(this Client client, long ChatID, FileInfo file, string? caption = null)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", ChatID}
            };
            if (!string.IsNullOrEmpty(caption))
            {
                dict.Add("caption", caption);
            }
            string res = await client.ExecuteMultipart("sendDocument", file, dict, fileFormFieldName: "document");
            return JsonConvert.DeserializeObject<ApiResponse<Objects.Message>>(res).Result;
        }

        public static async Task<Objects.Message> SendDocument(this Client client, long chatID, string FileIdOrURL, string? caption = null)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", chatID},
                {"from_chat_id", null},
                {"document", FileIdOrURL}
            };
            if (!string.IsNullOrEmpty(caption))
            {
                dict.Add("caption", caption);
            }
            string res = await client.ExecuteAsync("sendDocument", dict);
            var tmp = JsonConvert.DeserializeObject<ApiResponse<Message>>(res);
            return tmp.Result;
        }

        public static async Task<Message> SendPhoto(this Client client, long chatID, FileInfo file, string? caption = null)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", chatID}
            };
            if (!string.IsNullOrEmpty(caption))
            {
                dict.Add("caption", caption);
            }
            string res = await client.ExecuteMultipart("sendPhoto", file, dict);
            return JsonConvert.DeserializeObject<ApiResponse<Objects.Message>>(res).Result;
        }

        public static async Task<Objects.Message> SendPhoto(this Client client, long chatID, string FileIdOrURL, string? caption = null)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", chatID},
                {"from_chat_id", null},
                {"photo", FileIdOrURL}
            };
            if (!string.IsNullOrEmpty(caption))
            {
                dict.Add("caption", caption);
            }
            string res = await client.ExecuteAsync("sendPhoto", dict);
            var tmp = JsonConvert.DeserializeObject<ApiResponse<Message>>(res);
            return tmp.Result;
        }

        public static async Task<Objects.Message> SendAudio(this Client client, long ChatID, string FileIdOrUrl, string? caption = null)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", ChatID},
                {"audio", FileIdOrUrl}
            };
            if (!string.IsNullOrEmpty(caption))
            {
                dict.Add("caption", caption);
            }
            string res = await client.ExecuteAsync("sendAudio", dict);
            return JsonConvert.DeserializeObject<ApiResponse<Message>>(res).Result;
        }

        public static async Task<Objects.Message> SendAudio(this Client client, long ChatID, FileInfo audio, string? caption = null)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", ChatID}
            };
            if (!string.IsNullOrEmpty(caption))
            {
                dict.Add("caption", caption);
            }
            string res = await client.ExecuteMultipart("sendAudio", audio, dict, "audio");
            return JsonConvert.DeserializeObject<ApiResponse<Message>>(res).Result;
        }

        public static async Task<Objects.Message> SendVoice(this Client client, long ChatID, string FileIdOrUrl, string? caption = null)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", ChatID},
                {"voice", FileIdOrUrl}
            };
            if (!string.IsNullOrEmpty(caption))
            {
                dict.Add("caption", caption);
            }
            string res = await client.ExecuteAsync("sendVoice", dict);
            return JsonConvert.DeserializeObject<ApiResponse<Message>>(res).Result;
        }

        public static async Task<Objects.Message> SendVoice(this Client client, long ChatID, FileInfo voice, string? caption = null)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", ChatID}
            };
            if (!string.IsNullOrEmpty(caption))
            {
                dict.Add("caption", caption);
            }
            string res = await client.ExecuteMultipart("sendVoice", voice, dict, "voice");
            return JsonConvert.DeserializeObject<ApiResponse<Message>>(res).Result;
        }

        public static async Task<Objects.Message> SendAnimation(this Client client, long ChatID, string FileIdOrURL)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", ChatID},
                {"animation", FileIdOrURL}
            };
            string res = await client.ExecuteAsync("sendAnimation", dict);
            return JsonConvert.DeserializeObject<ApiResponse<Message>>(res).Result;
        }

        public static async Task<Objects.Message> SendAnimation(this Client client, long ChatID, FileInfo animation)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", ChatID}
            };
            string res = await client.ExecuteMultipart("sendAnimation", animation, dict, "animation");
            return JsonConvert.DeserializeObject<ApiResponse<Message>>(res).Result;
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

        public static async Task<Objects.Message> SendLocation(this Client client, long chatID, float latitude, float longitude, float? horizontal_accuracy = null)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", chatID},
                {"latitude", latitude},
                {"longitude", longitude}
            };
            if (horizontal_accuracy != null)
            {
                dict.Add("horizontal_accuracy", horizontal_accuracy);
            }
            string res = await client.ExecuteAsync("sendLocation", dict);
            var tmp = JsonConvert.DeserializeObject<ApiResponse<Objects.Message>>(res);
            return tmp.Result;
        }

        public static async Task<Objects.Message> SendContact(this Client client, long ChatID, string phone_number, string first_name, string? last_name = null)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", ChatID},
                {"first_name", first_name}                
            };
            if (!string.IsNullOrEmpty(last_name))
            {
                dict.Add("last_name", last_name);
            }
            string res = await client.ExecuteAsync("sendContact", dict);
            return JsonConvert.DeserializeObject<ApiResponse<Message>>(res).Result;
        }

        public static async Task<Objects.Message> SendContact(this Client client, long ChatID, Objects.Contact contact)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", ChatID},
                {"first_name", contact.first_name}
            };
            if (!string.IsNullOrEmpty(contact.last_name))
            {
                dict.Add("last_name", contact.last_name);
            }
            string res = await client.ExecuteAsync("sendContact", dict);
            return JsonConvert.DeserializeObject<ApiResponse<Message>>(res).Result;
        }

        public static async void PinMessage(this Client client, Message msg, long ChatID)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", ChatID},
                {"message_id", msg.id}
            };
            await client.ExecuteAsync("pinChatMessage", dict);
        }

        public static async Task<Objects.File> GetFile(this Client client, string file_id)
        {
            var dict = new Dictionary<string, object>
            {
                {"file_id", file_id}
            };
            string res = await client.ExecuteAsync("getFile", dict);
            var tmp = JsonConvert.DeserializeObject<ApiResponse<Objects.File>>(res);
            return tmp.Result;
        }

        public static async Task<string> FileLink(this Client client, string file_id)
        {
            Objects.File file = await client.GetFile(file_id);
            string output = $"https://tapi.bale.ai/file/bot{client.Token}/{file.file_path}";
            return output;
        }

        public static async Task<bool> banChatMember(this Client client, long UserID, long ChatID)
        {
            var dict = new Dictionary<string, object>
            {
                {"user_id", UserID},
                {"chat_id", ChatID}
            };
            string res = await client.ExecuteAsync("banChatMember", dict);
            var tmp = JsonConvert.DeserializeObject<ApiResponse<bool>>(res);
            return tmp.Result != null ? tmp.Result : false;
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
            try
            {
                var m = JsonConvert.DeserializeObject<ApiResponse<Objects.Message>>(res);
                return m.Result != null ? m.Result : new Message();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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

        public static async Task<int> GetChatMembersCount(this Client client, long chat_id)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", chat_id}
            };
            string res = await client.ExecuteAsync("getChatMembersCount", dict);
            int result = JsonConvert.DeserializeObject<ApiResponse<int>>(res).Result;
            return result != null ? result : 0;
        }

        public static async void sendChatAction(this Client client, long chatID, ChatAction mode)
        {
            var dict = new Dictionary<string, object>
            {
                {"chat_id", chatID},
                {"action", mode.ActionEncode()}
            };
            await client.ExecuteAsync("sendChatAction", dict);
        }

        public static async void testMessage(this Client client, long ChatID)
        {
            await client.SendMessage(ChatID, "پیام تست بله شارپ\n اگر این پیام را دریافت میکنید به این معناست که با موفقیت توانستید بله شارپ را بر روی ربات خود نصب کنید\n کانال بله شارپ : @BaleSharp \n گروه پشتیبانی بله شارپ : @bleSharpGP");
        }

        public static async void ReplyKeyboardRemove(this Client client, bool confirm)
        {
            var dict = new Dictionary<string, object>
            {
                {"remove_keyboard", confirm}
            };
            await client.ExecuteAsync("ReplyKeyboardRemove", dict);
        }

        public static async void sendInvoice(this Client client, long chat_id, string title, string description, string payload, string provider_token, Objects.LabeledPrice[] prices, string? photo_url = null)
        {
            /// <summary>Sends an message including an invoice for user to pay</summary>
            /// <param name="chat_id">The chatID that invoice should be send there</param>
            /// <param name="title">The title of invoice</param>
            /// <param name="description">the description of invoice</param>
            /// <param name="payload">a data that will return after successful payment (user won't see this)</param>
            /// <param name="provider_token">the card number or wallet token of reciever</param>
            /// <param name="prices">a array of prices</param>
            /// <param name="photo_url">url of photo that could be sent in invoice (OPTIONAL)</param>
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

        public static async Task<bool> answerCallbackquery(this Client client, CallbackQuery callbackquery, string text, bool? show_alert = null)
        {
            var dict = new Dictionary<string, object>
            {
                {"callback_query_id", callbackquery.id},
                {"text", text}
            };
            if (show_alert != null)
            {
                dict.Add("show_alert", show_alert);
            }
            string res = await client.ExecuteAsync("answerCallbackQuery", dict);
            var tmp = JsonConvert.DeserializeObject<ApiResponse<bool>>(res);
            return tmp.Result;
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

        public static async Task<bool> logout(this Client client)
        {
            string res = await client.ExecuteAsync("logout");
            return JsonConvert.DeserializeObject<ApiResponse<bool>>(res).Result;
        }

        public static async void close(this Client client)
        {
            await client.ExecuteAsync("close");
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
            /// <summary>Deletes a message from specified chat</summary>
            /// <param name="msg">the message to delete from it's chat</param>
            /// <returns>True if success</returns>
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
            /// <summary>leaves a specified chat (Group or channel)</summary>
            /// <param name="chatID">the chat id of group or channel</param>
            /// <returns>true if success</returns>
            string res = await client.ExecuteAsync("leaveChat", dict);
            var tmp = JsonConvert.DeserializeObject<ApiResponse<bool>>(res);
            return tmp.Result;
        }
        public static async Task<Objects.Message> reply_to(this Client client, Objects.Message msg, string text, object? reply_markup = null)
        {
            Objects.Message m = await client.SendMessage(msg.chat.id, text, reply_markup, msg.id);
            return m;
        }
        public static async Task<Objects.Update[]> GetUpdates(this Client client, int offset, int? limit = 100, int? timout = 30)
        {
            var dict = new Dictionary<string, object>
            {
                {"offset", offset},
                {"limit", limit},
                {"timeout", timout}
            };

            string res = await client.ExecuteAsync("getUpdates", dict);
            var response = JsonConvert.DeserializeObject<ApiResponse<Objects.Update[]>>(res);
            if (response?.Result != null)
            {
                foreach (var update in response.Result)
                {
                    update.client = client;

                    if (update.pre_checkout_query != null)
                        update.pre_checkout_query.client = client;

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
