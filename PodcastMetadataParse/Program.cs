using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

namespace PodcastMetadataParse
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 1 ||
				!File.Exists(args[0]))
			{
				Console.WriteLine("First parameter must be file that exists");
				return;
			}

			XmlDocument doc = new XmlDocument();
			doc.Load(args[0]);

			XmlNode? root = doc.FirstChild;
			if (root == null) { return; }

			foreach (XmlNode div in root.ChildNodes)
			{
				XmlNodeList? articles = div.SelectNodes("article");
				if (articles == null) { continue; }
				foreach (XmlNode article in articles)
				{
					ProcessArticle(article);
				}
			}
		}

		static void ProcessArticle(XmlNode article)
		{
			Regex regex = new Regex(@"\s{2,}", RegexOptions.Multiline);

			XmlNode? itemInfo = article.FirstChild;
			if (itemInfo == null) { return; }

			// Get the publish date
			XmlNode? timeNode = itemInfo.SelectSingleNode("h3/time");
			if (timeNode == null) { return; }
			string? time = timeNode.Attributes?.GetNamedItem("datetime")?.Value;
			if (string.IsNullOrEmpty(time)) { return; }

			if (Directory.Exists(time))
			{
				Directory.Delete(time, recursive: true);
			}

			Directory.CreateDirectory(time);

			File.WriteAllText(Path.Combine(time, "date.txt"), time);

			// Get the episode title
			XmlNode? titleNode = itemInfo.SelectSingleNode("h2/a");
			if (titleNode != null)
			{
				string title = titleNode.InnerText;
				title = regex.Replace(title, " ");
				File.WriteAllText(Path.Combine(time, "title.txt"), title);
			}

			// Get the teaser
			XmlNode? teaserNode = itemInfo.SelectSingleNode("p[@class=\"teaser\"]");
			if (teaserNode != null)
			{
				string teaser = teaserNode.InnerText;
				teaser = regex.Replace(teaser, " ");
				File.WriteAllText(Path.Combine(time, "teaser.txt"), teaser);
			}

			// Get the episode URL
			XmlNode? episodeNode = itemInfo.SelectSingleNode("article/div/div/div/a[@class=\"audio-module-listen\"]");
			if (episodeNode != null)
			{
				string? url = episodeNode.Attributes?.GetNamedItem("href")?.Value;
				if (!string.IsNullOrEmpty(url))
				{
					File.WriteAllText(Path.Combine(time, "url.txt"), url);
					DownloadFile(url, time);
				}
			}
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
