namespace VeloNSK.APIServise
{
    class Lincs
    {
        public string GetServer() { return "http://90.189.158.10/"; }
        public string GetAPI() { return GetServer() + "api/"; }
        public string GetImageServerLinks(){ return GetServer() + "ImagesGaleri/"; }
    }
}
