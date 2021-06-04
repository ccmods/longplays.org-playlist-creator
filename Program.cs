using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Web;
using System.Xml;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace retrolongplay
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 2) {return;}
            int cat = -1;
            string tarPlist = args[1];
            if(!int.TryParse(args[0],out cat) || string.IsNullOrEmpty(tarPlist.Trim())) {return;}
            try {
                Console.WriteLine("Parameters correct. Retrieving video list.");
                string games = getGamesByCategory(cat);
                Console.WriteLine("Games retrieved. Formatting playlist.");
                Videos allVideos = JsonConvert.DeserializeObject<Videos>(games); 
                HtmlDocument doc = new HtmlDocument();
                StringBuilder sb = new StringBuilder();
                foreach(AaData gameData in allVideos.aaData) {                
                    doc.LoadHtml(gameData.game);
                    HtmlNodeCollection gameNodes = doc.DocumentNode.ChildNodes;
                    string gameName = gameNodes[0].InnerText;
                    string gameId = gameNodes[0].GetAttributeValue("href","");
                    if(gameId.Contains("="))
                        gameId = gameId.Split(new string[] {Constants.Tokens.longPlayId}, StringSplitOptions.None)[1].Substring(1);
                    doc.LoadHtml(gameData.players);
                    HtmlNodeCollection playerNodes = doc.DocumentNode.ChildNodes;    
                    string player = playerNodes[1].InnerText;
                    sb.AppendLine(
                        string.Format(Constants.trackItem,
                            Constants.Endpoints.videoUrl+gameId,
                            SecurityElement.Escape(HttpUtility.HtmlDecode(gameName.Trim())),player.Trim())
                    );
                }
                File.WriteAllText(tarPlist,
                    Constants.playlistTemplate.Replace(Constants.Tokens.playlistToken, sb.ToString()));
            } catch (Exception ex) {
                Console.WriteLine("Error generating playlist - details:"+ex.ToString());
            }   
            Console.WriteLine("Process halted.");         
        }

        static string getGamesByCategory(int catId) {
            var request = (HttpWebRequest)WebRequest.Create(Constants.Endpoints.AjaxLookup);
            var postData = Constants.postVars.Replace(Constants.Tokens.postVarToken, catId.ToString());
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = Constants.contentType;
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            return new StreamReader(response.GetResponseStream()).ReadToEnd();            
        }
    }
}
