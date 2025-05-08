using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ApiSender
    {
        public async Task<string> SendUrl(string url, Dictionary<string, object>? parameters = null)
        {
            HttpClient _http = new HttpClient();            

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

            HttpResponseMessage res = await _http.GetAsync(url);
            try { res.EnsureSuccessStatusCode(); }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            string content = await res.Content.ReadAsStringAsync();
            return JsonConvert.SerializeObject(content);
        }
    }
}
