using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.WellKnownTypes;

namespace AccuBotClientAvalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        public string Greeting
        {
            get
            {
                try
                {
                    var user = AccuBotClientAvalonia.Program.gRPCClient.API.UserListGet(new Empty());
                    return $"Welcome to Avalonia \r\n\r\n Name:{user.Users[0].Name}";
                }
                catch (Exception ex)
                {
                    return $"Welcome to Avalonia! \r\n\r\n gRPC Error:{ex.Message}";
                }
            }
        }
    }
}