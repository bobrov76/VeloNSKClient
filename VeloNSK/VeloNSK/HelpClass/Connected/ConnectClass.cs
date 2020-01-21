using Plugin.Connectivity;

namespace VeloNSK.HelpClass.Connected
{
    class ConnectClass
    {       
        // получаем состояние подключения
        public bool CheckConnection()
        {
            if (CrossConnectivity.Current != null && CrossConnectivity.Current.ConnectionTypes != null && CrossConnectivity.Current.IsConnected == true)
            {                
                return true;
            }
            return false;
        }
    }
}
