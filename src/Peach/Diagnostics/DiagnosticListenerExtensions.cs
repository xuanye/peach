using System;
using System.Diagnostics;
using Peach.Messaging;

namespace Peach.Diagnostics
{
    public static class DiagnosticListenerExtensions
    {
        public const string DiagnosticListenerName = "PeachDiagnosticListener";
        public const string DiagnosticServiceReieve = "Peach.Service.Recieve";
        public const string DiagnosticServiceReieveCompleted = "Peach.Service.ReieveCompleted";
        public const string DiagnosticServiceException = "Peach.Service.Exception";  
        public const string DiagnosticClientRecieve = "Peach.Client.Recieve";
        public const string DiagnosticClientRecieveCompleted = "Peach.Client.RecieveCompleted";
        public const string DiagnosticClientException = "Peach.Client.Exception";

        public static void ServiceRecieve<TMessage>(this DiagnosticListener listener, TMessage recieveMessage) where TMessage : IMessage
        {
            if (listener.IsEnabled(DiagnosticServiceReieve))
            {
                listener.Write(DiagnosticServiceReieve, new
                {
                    Message = recieveMessage
                });
            }
        }

        public static void ServiceRecieveCompleted<TMessage>(this DiagnosticListener listener, TMessage recieveMessage) where TMessage : IMessage
        {
            if (listener.IsEnabled(DiagnosticServiceReieveCompleted))
            {
                listener.Write(DiagnosticServiceReieveCompleted, new
                {
                    Request = recieveMessage
                });
            }
        }

        public static void ServiceException(this DiagnosticListener listener,  Exception exception)         
        {
            if (listener.IsEnabled(DiagnosticServiceException))
            {
                listener.Write(DiagnosticServiceException, new
                {                  
                    Exception = exception
                });
            }
        }
         

        public static void ClientRecieve<TMessage>(this DiagnosticListener listener, TMessage recieveMessage) where TMessage : IMessage
        {
            if (listener.IsEnabled(DiagnosticClientRecieve))
            {
                listener.Write(DiagnosticClientRecieve, new
                {
                    Message = recieveMessage                   
                });
            }
        }
        public static void ClientRecieveComplete<TMessage>(this DiagnosticListener listener, TMessage recieveMessage) where TMessage : IMessage
        {
            if (listener.IsEnabled(DiagnosticClientRecieve))
            {
                listener.Write(DiagnosticClientRecieve, new
                {
                    Message = recieveMessage
                });
            }
        }
        public static void ClientException(this DiagnosticListener listener,Exception exception) 
        {
            if (listener.IsEnabled(DiagnosticClientException))
            {
                listener.Write(DiagnosticClientException, new
                {    
                    Exception = exception
                });
            }
        }
    }

}