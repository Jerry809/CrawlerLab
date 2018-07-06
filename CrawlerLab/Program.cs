using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using HtmlAgilityPack;
using System.Linq;
using Newtonsoft.Json;

namespace CrawlerLab
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<Letou>();

            var data = JsonConvert.DeserializeObject<List<Letou>>(File.ReadAllText(@"D:\My\程式\CrawlerLab\CrawlerLab\Letou.json"));
            for (int i = 1; i <= 16; i++)
            {
                WebClient client = new WebClient();
                MemoryStream ms = new MemoryStream(client.DownloadData($"http://www.lotto-8.com/listltobig.asp?indexpage={i}&orderby=new"));

                // 使用預設編碼讀入 HTML
                HtmlDocument doc = new HtmlDocument();
                doc.Load(ms, Encoding.Default);

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("/html[1]/body[1]/table[3]/tr[1]/td[1]/table[2]/tr");

                foreach (HtmlNode node in nodes)
                {
                    var td = node.SelectNodes("td[2]").FirstOrDefault().InnerHtml;
                    if (td == "大樂透中獎號碼")
                        continue;
                    try
                    {
                        var date = node.SelectNodes("td[1]").FirstOrDefault().InnerHtml;
                        var number = td.Replace("&nbsp;", "");
                        var special = node.SelectNodes("td[3]").FirstOrDefault().InnerHtml;

                        if (data.FirstOrDefault(x=> Convert.ToDateTime(x.Date) == Convert.ToDateTime(date)) != null)
                            break;
                        else
                            data.Add(new Letou() { Date = Convert.ToDateTime(date).ToShortDateString(), Numbers = number, Special = special });
                       
                        //list.Add(new Letou() { Date = Convert.ToDateTime(date).ToShortDateString(), Numbers = number, Special = special });
                    }
                    catch
                    {

                    }
                }
                doc = null;
                nodes = null;
            }

            File.WriteAllText(@"D:\My\程式\CrawlerLab\CrawlerLab\Letou.json", JsonConvert.SerializeObject(data.OrderByDescending(x=>Convert.ToDateTime(x.Date))));

            Console.WriteLine("Completed.");
            Console.ReadLine();
        }
    }
}
