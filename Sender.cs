using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace PushBulletMessageProvider
{
    public class Sender
    {
        public Sender(string apikey)
        {
            ApiKey = apikey;
        }

        public string ApiKey { get; set; }

        public bool PushLinkToAllDevices(string title = "", string body = "", string url = "")
        {
            const string type = "link";

            var data =
                Encoding.ASCII.GetBytes(
                    string.Format("{{ \"type\": \"{0}\", \"title\": \"{1}\", \"body\": \"{2}\", \"url\": \"{3}\" }}",
                        type, title, body, url));

            return SendData(data);
        }

        public bool PushListToAllDevices(string title = "", List<string> items = null)
        {
            if (items == null) return false;
            const string type = "list";
            //var items = "[\"Sign up for Pushbullet\", \"Learn from examples\", \"???\", \"Profit!\" ]";
            var data =
                Encoding.ASCII.GetBytes(string.Format("{{ \"type\": \"{0}\", \"title\": \"{1}\", \"items\": {2} }}",
                    type, title, items));
            return SendData(data);
        }

        public bool PushNoteToAllDevices(string title = "", string body = "")
        {
            const string type = "note";
            var data =
                Encoding.ASCII.GetBytes(string.Format("{{ \"type\": \"{0}\", \"title\": \"{1}\", \"body\": \"{2}\" }}",
                    type, title, body));
            return SendData(data);
        }

        public bool PushAddressToAllDevices(string name = "", string address = "")
        {
            // Push an address to all devices.

            const string type = "address";

            var data =
                Encoding.ASCII.GetBytes(string.Format(
                    "{{ \"type\": \"{0}\", \"name\": \"{1}\", \"address\": \"{2}\" }}", type, name, address));
            return SendData(data);
        }

        private bool SendData(byte[] data)
        {
            var request = WebRequest.Create("https://api.pushbullet.com/v2/pushes") as HttpWebRequest;
            if (request == null) return true;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Credentials = new NetworkCredential(ApiKey, "");

            request.ContentLength = data.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
            }

            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response == null) return false;
                var respstr = response.GetResponseStream();

                if (respstr == null) return false;
                using (var reader = new StreamReader(respstr))
                {
                    reader.ReadToEnd();
                }
            }

            return true;
        }
    }
}
