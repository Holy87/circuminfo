using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CircumInfo.Common
{
    public static class MessageSystem
    {
        private static readonly string MESSAGEFILE = "AdminMessage.xml";
        public static async Task<Message> checkMessages()
        {
            bool updated = await TextHandler.isUpToDate(MESSAGEFILE);
            if (!updated)
            {
                string xml = await TextHandler.download_text(MESSAGEFILE);
                try {
                    Message message = ObjectSerializer<Message>.FromXml(xml);
                    if (message.ID > Settings.LastMessage)
                    {
                        return message;
                    } else
                        return null;
                } catch (Exception) {
                    return null;
                }
            }
                
            return null;
        }
    }
}
