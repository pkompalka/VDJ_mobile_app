using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VDJApp.Models;
using Xamarin.Forms;

namespace VDJApp.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        public VDJAppSingleton VDJAppSingletonInstance { get; set; }

        public ObservableCollection<string> ChatObservableCollection { get; set; }

        public bool IsAppearingFlag { get; set; } = false;

        public bool FirstAppearFlag { get; set; } = false;

        public DateTime MessageSentDate { get; set; }

        private string messageToSend = string.Empty;

        public string MessageToSend
        {
            get { return messageToSend; }
            set { SetProperty(ref messageToSend, value); }
        }

        public Command SendMessageCommand
        {
            get { return new Command(async () => await SendMessage()); }
        }

        public Command StartChatCommand
        {
            get { return new Command(async () => await StartChat()); }
        }

        public Command EndChatCommand
        {
            get { return new Command(() => EndChat()); }
        }

        public ChatViewModel()
        {
            Title = "Chat";
            VDJAppSingletonInstance = VDJAppSingleton.Instance;
            ChatObservableCollection = new ObservableCollection<string>();
            MessageSentDate = DateTime.Now;
            MessageSentDate = MessageSentDate.Add(new TimeSpan(0, -10, 0));
        }

        public async Task StartChat()
        {
            IsAppearingFlag = true;
            FirstAppearFlag = true;
            while (IsAppearingFlag == true)
            {
                await LoadChat();
                await Task.Delay(TimeSpan.FromMilliseconds(2000));
            }
        }

        public void EndChat()
        {
            IsAppearingFlag = false;
        }

        public async Task LoadChat()
        {
            try
            {
                HttpResponseMessage chatResponse = await VDJAppSingletonInstance.HttpClientSi.GetAsync($"api/Chats/{VDJAppSingletonInstance.ChatNumber}");

                if (chatResponse.IsSuccessStatusCode)
                {
                    string jsonChatResponse = await chatResponse.Content.ReadAsStringAsync();
                    List<string> chatGot = JsonConvert.DeserializeObject<List<string>>(jsonChatResponse);
                    if (chatGot.Count == 0)
                    {

                    }
                    else
                    {
                        for (int i = 0; i < chatGot.Count; i++)
                        {
                            ChatObservableCollection.Add(chatGot[i]);
                        }
                        VDJAppSingletonInstance.ChatNumber = VDJAppSingletonInstance.ChatNumber + chatGot.Count;
                        MessagingCenter.Send<object, object>(this, "ScollList", ChatObservableCollection.Last());
                    }

                    if (FirstAppearFlag == true)
                    {
                        MessagingCenter.Send<object, object>(this, "ScollList", ChatObservableCollection.Last());
                        FirstAppearFlag = false;
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Unable to load chat", "OK");
                }
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("Unable to connect", "Check your network connection and try different page", "OK");
            }
        }

        public async Task SendMessage()
        {
            if (MessageToSend == "")
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "Type message first", "OK");
            }
            else
            {
                TimeSpan interval = DateTime.Now - MessageSentDate;
                int secondsPassed = (int)interval.TotalSeconds;

                if (secondsPassed < 10)
                {
                    await Application.Current.MainPage.DisplayAlert("Hold up!", "In order to prevent spam you can only send one message every 10 seconds", "OK");
                }
                else
                {
                    string fullMessageToSend = VDJAppSingletonInstance.Username + ": " + MessageToSend;
                    string tmpMessage = MessageToSend;
                    MessageToSend = "";

                    try
                    {
                        HttpResponseMessage sendMessageResponse = await VDJAppSingletonInstance.HttpClientSi.PutAsJsonAsync($"api/Chats", fullMessageToSend);

                        if (sendMessageResponse.IsSuccessStatusCode)
                        {
                            MessageSentDate = DateTime.Now;
                        }
                        else
                        {
                            MessageToSend = tmpMessage;
                            await Application.Current.MainPage.DisplayAlert("Error", "Unable to send message", "OK");
                        }
                    }
                    catch
                    {
                        await Application.Current.MainPage.DisplayAlert("Unable to connect", "Check your network connection", "OK");
                    }
                }
            }
        }
    }
}
