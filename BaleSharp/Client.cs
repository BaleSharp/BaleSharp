using Bale;
using Bale.Objects;
using Bale.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bale
{
    public delegate Task MessageHandler(Objects.Message message);
    public delegate Task CallbackQueryHandler(Objects.CallbackQuery callbackQuery);
    public delegate Task CommandHandler(Objects.Message message, string command, string[] args);
    public delegate Task PaymentHandler(Objects.Message message, Objects.SuccessfulPayment payment);
    public delegate Task PreCheckoutQueryHandler(Objects.PreCheckoutQuery precheckoutquery);
    public delegate Task EditedMessageHandler(Objects.Message message);
    public delegate Task ContactHandler(Objects.Message msg, Objects.Contact contact);
    public delegate Task DocumentHandler(Objects.Message msg, Objects.Document doc);
    public delegate Task LocationHandler(Objects.Message msg, Objects.Location location);
    public delegate Task PhotoHandler(Objects.Message msg, PhotoSize[] Photos);
    public delegate Task VideoHandler(Objects.Message msg, Video video);
    public delegate Task StickerHandler(Objects.Message msg, Objects.Sticker sticker);
    public delegate Task AudioHandler(Objects.Message msg, Objects.Audio audio);
    public delegate Task NewUserHandler(Objects.User[] new_users);
    public delegate Task LeftUserHandler(Objects.User left_user);
    public delegate Task WebAppDataHandler(Objects.WebAppData data);

    public class Client
    {
        protected readonly string _token;
        protected readonly string baseUrl;
        protected readonly bool debug;
        public User self;


        public MessageHandler OnMessage { get; set; }
        public CommandHandler OnCommand { get; set; }
        public CallbackQueryHandler OnCallbackQuery { get; set; }
        public PaymentHandler OnSuccessfulPayment { get; set; }
        public PreCheckoutQueryHandler OnPreCheckoutQuery { get; set; }
        public EditedMessageHandler OnEditedMessage { get; set; }
        public ContactHandler OnContact { get; set; }
        public DocumentHandler OnDocument { get; set; }
        public LocationHandler OnLocation { get; set; }
        public PhotoHandler OnPhoto { get; set; }
        public VideoHandler OnVideo { get; set; }
        public AudioHandler OnAudio { get; set; }
        public StickerHandler OnSticker { get; set; }
        public NewUserHandler OnNewUser { get; set; }
        public LeftUserHandler OnLeftUser { get; set; }
        public WebAppDataHandler OnWebappData { get; set; }

        private bool _isReceiving;
        private int _lastUpdateId;
        public Client(string token, string? _baseUrl = null, bool debug = false)
        {
            if (!string.IsNullOrEmpty(_baseUrl)) baseUrl = _baseUrl;
            else baseUrl = "https://tapi.bale.ai/bot";
            this.debug = debug;
            _token = token;
            clientProfile();
        }

        public async void clientProfile()
        {
            self = await this.getMe();
        }

        public async void StartReceiving()
        {
            if (_isReceiving) return;

            _isReceiving = true;
            var me = await this.getMe();
            Console.WriteLine($"--({me.username}) Started getting updates...");
            Console.WriteLine("Powered by BaleSharp\nDocs at : @BaleSharp");
            await Task.Run(ReceiveUpdates);
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
                var updates = await this.GetUpdates(_lastUpdateId + 1);
                try
                {
                    foreach (var update in updates)
                    {
                        _lastUpdateId = update.update_id;
                        if (update != null)
                            if (update.message != null)
                            {
                                if (update.message.new_chat_members != null && OnNewUser != null)
                                {
                                    await OnNewUser(update.message.new_chat_members);
                                }

                                if (update.message.left_chat_member != null && OnLeftUser != null)
                                {
                                    await OnLeftUser(update.message.left_chat_member);
                                }

                                if (update.message.web_app_data != null && OnWebappData != null)
                                {
                                    await OnWebappData(update.message.web_app_data);
                                }

                                if (update.message.contact != null && OnContact != null)
                                {
                                    await OnContact(update.message, update.message.contact);
                                }

                                if (update.message.document != null && OnDocument != null)
                                {
                                    await OnDocument(update.message, update.message.document);
                                }

                                if (update.message.location != null && OnLocation != null)
                                {
                                    await OnLocation(update.message, update.message.location);
                                }

                                if (update.message.photo != null && OnPhoto != null)
                                {
                                    await OnPhoto(update.message, update.message.photo);
                                }

                                if (update.message.audio != null && OnAudio != null)
                                {
                                    await OnAudio(update.message, update.message.audio);
                                }

                                if (update.message.sticker != null && OnSticker != null)
                                {
                                    await OnSticker(update.message, update.message.sticker);
                                }

                                if (update.message.video != null && OnVideo != null)
                                {
                                    await OnVideo(update.message, update.message.video);
                                }

                                // Handle regular messages (non-command and not payment)
                                if (update.message.text != null && !update.message.text.StartsWith("/"))
                                {
                                    if (OnMessage != null)
                                    {
                                        if (debug) Console.WriteLine($"Message recieved : {update.message}");
                                        await OnMessage(update.message);
                                    }
                                }

                                // Handle commands (messages starting with "/")
                                if (update.message.text?.StartsWith("/") == true && OnCommand != null)
                                {
                                    string[] parts = update.message.text.Split(' ');
                                    var command = parts[0][1..]; // Remove the '/'
                                    var args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();
                                    if (debug) Console.WriteLine($"Command recieved : {update.message}");
                                    await OnCommand(update.message, command, args);
                                }

                                // Handle successful payments (if needed)
                                if (update.message.successful_payment != null && OnSuccessfulPayment != null)
                                {
                                    await OnSuccessfulPayment(update.message, update.message.successful_payment);
                                }

                            }
                            else if (update.edited_message != null)
                            {
                                if (update.edited_message.text != null && OnEditedMessage != null)
                                {
                                    await OnEditedMessage(update.edited_message);
                                }
                            }
                            else if (update.callback_query != null && OnCallbackQuery != null)
                            {
                                await OnCallbackQuery(update.callback_query);
                            }
                            else if (update.pre_checkout_query != null && OnPreCheckoutQuery != null)
                            {
                                await OnPreCheckoutQuery(update.pre_checkout_query);
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
}
