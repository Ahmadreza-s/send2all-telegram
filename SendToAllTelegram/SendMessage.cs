using TeleSharp.TL;
using TLSharp.Core;

namespace SendToAllTelegram
{
    class SendMessage
    {
        public delegate void Completedelegate();
        public event Completedelegate OnComplete;

        public int Id { set; get; }
        public string Message { get; set; }
        public TelegramClient Client { get; set; }
        public SendMessage(int id,string message,TelegramClient client)
        {
            Id = id;
            Message = message;
            Client = client;
            Send();
            
        }

        private async void Send()
        {
            await Client.SendMessageAsync(new TLInputPeerUser {user_id = Id}, Message);
            OnComplete?.Invoke();
        }
    }
}
