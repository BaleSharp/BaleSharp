using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bale.Enums
{
    public enum ChatAction
    {
        Typing,
        sendPhoto,
        sendVideo,
        recordVideo,
        recordVoice,
        sendVoice,
        sendDocument,
        sendLocation
    }
    public static class Extensions
    {
        public static string ActionEncode(this ChatAction action)
        {
            switch (action)
            {
                case ChatAction.Typing: return "typing"; break;
                case ChatAction.sendPhoto: return "upload_photo"; break;
                case ChatAction.sendVideo: return "upload_video"; break;
                case ChatAction.recordVideo: return "record_video"; break;
                case ChatAction.recordVoice: return "record_voice"; break;
                case ChatAction.sendVoice: return "upload_voice"; break;
                case ChatAction.sendDocument: return "upload_document"; break;
                case ChatAction.sendLocation: return "find_location"; break;
                default: return "typing"; break;
            }
        }
    }
}
