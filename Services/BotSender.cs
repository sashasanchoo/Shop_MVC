using Telegram.Bot;

namespace ASP.NET_Blog_MVC_Identity.Services
{
    public class BotSender
    {
        private ITelegramBotClient _bot { get; set; }
        public long _chatId { get; set; }
        public BotSender(string token, long chatId)
        {
            _bot = new TelegramBotClient(token);
            _chatId = chatId;
        }
        public async void SendMessage(string message)
        {
            await _bot.SendTextMessageAsync(_chatId, message);
        }
    }
}
