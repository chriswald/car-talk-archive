//----------------------------------------------------
// Copyright 2021 Epic Systems Corporation
//----------------------------------------------------

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

			XmlNode root = doc.FirstChild;

			foreach (XmlNode div in root.ChildNodes)
			{
				foreach (XmlNode article in div.SelectNodes("article"))
				{
					ProcessArticle(article);
				}
			}
		}

		static void ProcessArticle(XmlNode article)
		{
			Regex regex = new Regex(@"\s{2,}", RegexOptions.Multiline);

			XmlNode itemInfo = article.FirstChild;

			// Get the publish date
			XmlNode timeNode = itemInfo.SelectSingleNode("h3/time");
			string time = timeNode.Attributes.GetNamedItem("datetime").Value;

			if (Directory.Exists(time))
			{
				Directory.Delete(time, recursive: true);
			}

			Directory.CreateDirectory(time);

			File.WriteAllText(Path.Combine(time, "date.txt"), time);

			// Get the episode title
			XmlNode titleNode = itemInfo.SelectSingleNode("h2/a");
			string title = titleNode.InnerText;
			title = regex.Replace(title, " ");
			File.WriteAllText(Path.Combine(time, "title.txt"), title);

			// Get the teaser
			XmlNode teaserNode = itemInfo.SelectSingleNode("p[@class=\"teaser\"]");
			string teaser = teaserNode.InnerText;
			teaser = regex.Replace(teaser, " ");
			File.WriteAllText(Path.Combine(time, "teaser.txt"), teaser);

			// Get the episode URL
			XmlNode episodeNode = itemInfo.SelectSingleNode("article/div/div/div/a[@class=\"audio-module-listen\"]");
			if (episodeNode != null)
			{
				string url = episodeNode.Attributes.GetNamedItem("href").Value;
				File.WriteAllText(Path.Combine(time, "url.txt"), url);
				DownloadFile(url, time);
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
