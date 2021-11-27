using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

namespace PodcastSync
{
	class Program
	{
		static void Main(string[] args)
		{
			Regex regex = new Regex(@"\s{2,}", RegexOptions.Multiline);

			string baseOutDir = Directory.GetCurrentDirectory();
			if (args.Length > 0)
			{
				baseOutDir = args[0];
			}

			string rss = PodcastRss("https://feeds.npr.org/510208/podcast.xml");

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(rss);

			XmlNamespaceManager nsMgr = new XmlNamespaceManager(doc.NameTable);
			nsMgr.AddNamespace("itunes", "https://www.itunes.com/dtds/podcast-1.0.dtd");

			XmlNode? root = doc.SelectSingleNode("rss");

			XmlNode? channel = root?.SelectSingleNode("channel");
			if (channel == null) { return; }

			XmlNodeList? items = channel.SelectNodes("item");
			if (items == null) { return; }

			foreach (XmlNode item in items)
			{
				XmlNode? pubDateNode = item.SelectSingleNode("pubDate");

				if (pubDateNode == null) { continue; }

				DateTime pubDate = DateTime.Parse(pubDateNode.InnerText);

				string episodeFolder = Path.Combine(baseOutDir, pubDate.ToString("yyyy-MM-dd"));
				if (Directory.Exists(episodeFolder))
				{
					continue;
				}

				Directory.CreateDirectory(episodeFolder);

				XmlNode? titleNode = item.SelectSingleNode("title");
				XmlNode? teaserNode = item.SelectSingleNode("description");
				XmlNode? enclosureNode = item.SelectSingleNode("enclosure");
				XmlNode? durationNode = new List<XmlNode>(item.ChildNodes.Cast<XmlNode>())
					.Find(node => node.Name.Contains("itunes:duration", StringComparison.OrdinalIgnoreCase));

				if (titleNode == null || teaserNode == null || enclosureNode == null)
				{
					continue;
				}

				// Get the publish date
				File.WriteAllText(Path.Combine(episodeFolder, "date.txt"), pubDate.ToString("yyyy-MM-dd"));

				// Get the episode title
				
				string title = titleNode.InnerText;
				title = regex.Replace(title, " ");
				File.WriteAllText(Path.Combine(episodeFolder, "title.txt"), title);

				// Get the teaser
				string teaser = teaserNode.InnerText;
				teaser = regex.Replace(teaser, " ");
				File.WriteAllText(Path.Combine(episodeFolder, "teaser.txt"), teaser);

				// Get the episode URL
				XmlNode? urlAttr = enclosureNode.Attributes?.GetNamedItem("url");
				string? url = urlAttr?.Value;
				if (!string.IsNullOrEmpty(url))
				{
					File.WriteAllText(Path.Combine(episodeFolder, "url.txt"), url);
					DownloadFile(url, episodeFolder);
				}

				if (durationNode != null)
                {
					string duration = durationNode.InnerText;
					File.WriteAllText(Path.Combine(episodeFolder, "duration.txt"), duration);
                }
			}
		}

		static string PodcastRss(string url)
		{
			string resultString = string.Empty;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.AutomaticDecompression = DecompressionMethods.GZip;

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				resultString = reader.ReadToEnd();
			}

			return resultString;
		}

		static void DownloadFile(string url, string outDir)
		{
			using (WebClient webClient = new WebClient())
			{
				Uri uri = new Uri(url);
				string filename = Path.GetFileName(uri.LocalPath);

				if (filename.Contains("?"))
				{
					filename = filename.Substring(0, filename.IndexOf('?'));
				}

				webClient.DownloadFile(url, Path.Combine(outDir, filename));
			}
		}
	}
}
