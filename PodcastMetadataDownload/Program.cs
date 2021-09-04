using System.IO;
using System.Net;
using System.Text;

namespace Podcast
{
	class Program
	{
		static void Main(string[] args)
		{
			StringBuilder sb = new StringBuilder();
			int start = 1;
			int count = RemainingCount(start);

			while (count > 0)
			{
				string html = GetNextHtml(start);
				sb.AppendLine(html);
				start += count;

				count = RemainingCount(start);
			}

			File.WriteAllText("results.html", sb.ToString());
		}

		static int RemainingCount(int start)
		{
			string resultString = string.Empty;
			string url = $"https://www.npr.org/get/510208/remainingCount?start={start}";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.AutomaticDecompression = DecompressionMethods.GZip;

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				resultString = reader.ReadToEnd();
			}

			return int.Parse(resultString);
		}

		static string GetNextHtml(int start)
		{
			string resultString = string.Empty;
			string url = $"https://www.npr.org/get/510208/render/partial/next?start={start}";

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
	}
}
