using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeloNSK.HelpClass.Messaging
{
    class MessagingAPI
    {
        public bool SendEmail(string addres,string subject,string body)//Отправка письма на почту
        {
            if ( addres!="" && subject != "" && body != "" && CrossMessaging.Current.EmailMessenger.CanSendEmail)
            {
                CrossMessaging.Current.EmailMessenger.SendEmail(addres,subject,body);
                return true;
            }
            else return false;
        }
        public bool MakePhoneCall(string phone)//Звонок
        {
            if (phone != "" && CrossMessaging.Current.PhoneDialer.CanMakePhoneCall)
            {
                CrossMessaging.Current.PhoneDialer.MakePhoneCall(phone);
                return true;
            }
            else return false;
        }
        public bool MakeSmsMessenge(string phone, string body)//Отправка сообщения
        {
            if (phone != "" && body != "" && CrossMessaging.Current.SmsMessenger.CanSendSms)
            {
                CrossMessaging.Current.SmsMessenger.SendSms(phone,body);
                return true;
            }
            else return false;
        }
    }
}
