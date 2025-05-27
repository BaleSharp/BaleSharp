using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Bale.Helpers
{
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

        public Objects.ReplyKeyboardMarkup EmptyMarkup()
        {
            return new Objects.ReplyKeyboardMarkup { keyboard = [] };
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

        public InlineKeyboardBuilder AddButton(string text, string callbackData = null, string url = null, string webApp = null, string copyText = null)
        {
            _currentRow.Add(new Objects.InlineKeyboardButton
            {
                text = text,
                callback_data = callbackData,
                url = url,
                web_app = webApp != null ? new Objects.WebAppInfo(webApp) : null,
                copy_text = copyText != null ? new Objects.CopyTextButton(copyText) : null
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

    public class OTPResponse<Type>
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("result")]
        public Type Result { get; set; }

        [JsonProperty("error")]
        public string? Error { get; set; }
    }

    public class BaleOTP
    {
        public string client { get; set; }
        public string client_secret { get; set; }
        public BaleOTP(string username, string secret)
        {
            client = username;
            client_secret = secret;
        }
        private static readonly HttpClient _client = new HttpClient();
        public async Task<object> sendCode(string phone)
        {
            var res = await _client.GetAsync($"https://aladdin4api.pythonanywhere.com/baleotp/balesharp?secret={this.client_secret}&username={this.client}&phone={phone}");
            string result = await res.Content.ReadAsStringAsync();
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                
                var response = JsonConvert.DeserializeObject<OTPResponse<int>>(result);
                return response.Result;
            }
            else
            {
                var response = JsonConvert.DeserializeObject<OTPResponse<int>>(result);
                return response.Error;
            }
        }
    }

}
