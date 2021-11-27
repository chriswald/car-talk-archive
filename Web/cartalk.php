<?php
header("Content-Type: application/xml");
?>
<?xml version="1.0" encoding="UTF-8"?>
<rss xmlns:npr="https://www.npr.org/rss/" xmlns:nprml="https://api.npr.org/nprml" xmlns:itunes="http://www.itunes.com/dtds/podcast-1.0.dtd" xmlns:content="http://purl.org/rss/1.0/modules/content/" xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:media="http://search.yahoo.com/mrss/" version="2.0">
    <channel>
        <title>The Best of Car Talk - Archive</title>
        <link>http://www.cartalk.com</link>
        <description><![CDATA[America's funniest auto mechanics take calls from weary car owners all over the country, and crack wise while they diagnose Dodges and dismiss Diahatsus. You don't have to know anything about cars to love this one hour weekly laugh fest.]]></description>
        <language>en</language>
        <itunes:summary><![CDATA[America's funniest auto mechanics take calls from weary car owners all over the country, and crack wise while they diagnose Dodges and dismiss Diahatsus. You don't have to know anything about cars to love this one hour weekly laugh fest.]]></itunes:summary>
        <itunes:author>NPR</itunes:author>
        <itunes:block>no</itunes:block>
        <itunes:category text="Leisure">
            <itunes:category text="Automotive"/>
        </itunes:category>
        <itunes:category text="Comedy"/>
        <itunes:image href="https://podcast.chriswald.com/image?p=cartalk.jpg"/>
        <itunes:type>episodic</itunes:type>
        <itunes:explicit>false</itunes:explicit>
        <media:restriction type="country" relationship="allow">es fi fr gb nl no se us dk at ch be au nz ca de it ie lu li ad mc pl pt is mx ee lv lt hk sg my tr gr ar tw hu cz mt bg sk cy br cl co uy sv py hn pa ni pe ec do gt cr bo ph id jp th vn ro il za in</media:restriction>
        <image>
            <url>https://podcast.chriswald.com/image?p=cartalk.jpg</url>
            <title>The Best of Car Talk</title>
            <link>http://www.cartalk.com</link>
        </image>

<?php

$todayString = date("D, d M Y H:i:s O");
echo "<lastBuildDate>" . $todayString . "</lastBuildDate>\n";

$podcastRoot = "/media/trek/Podcasts/Car Talk/episodes";
$podcasts = scandir($podcastRoot);

foreach ($podcasts as $key => $value)
{
    if (!in_array($value, array(".", "..")))
    {
        if (is_dir($podcastRoot . DIRECTORY_SEPARATOR . $value))
        {
            $episodeDir = $podcastRoot . DIRECTORY_SEPARATOR . $value;
            $pubDateFromFile = file_get_contents($episodeDir . DIRECTORY_SEPARATOR . "date.txt");
            $pubDate = strtotime($pubDateFromFile);
            $pubDateString = date("D, d M Y H:i:s O", $pubDate);

            $title = file_get_contents($episodeDir . DIRECTORY_SEPARATOR . "title.txt");
            $title = htmlspecialchars($title, ENT_XML1);
            $teaser = file_get_contents($episodeDir . DIRECTORY_SEPARATOR . "teaser.txt");
            $teaser = htmlspecialchars($teaser, ENT_XML1);

            if (file_exists($episodeDir . DIRECTORY_SEPARATOR . "duration.txt"))
            {
                $duration = file_get_contents($episodeDir . DIRECTORY_SEPARATOR . "duration.txt");
            }
            else
            {
                $duration = 3376;
            }

            $episodeUrl = "https://podcast.chriswald.com/episode?p=" . $pubDateFromFile;

            echo "<item>\n";
            echo "<title>" . $title . "</title>\n";
            echo "<description>" . $teaser . "</description>\n";
            echo "<itunes:summary>" . $teaser . "</itunes:summary>\n";
            echo "<itunes:author>NPR</itunes:author>\n";
            echo "<itunes:explicit>no</itunes:explicit>\n";
            echo "<itunes:episodeType>full</itunes:episodeType>\n";
            echo "<itunes:duration>" . $duration . "</itunes:duration>\n";
            echo "<enclosure url=\"" . $episodeUrl . "\" length=\"53032053\" type=\"audio/mpeg\"/>\n";
            echo "<pubDate>" . $pubDateString . "</pubDate>\n";
            echo "</item>\n";
        }
    }
}

?>
    </channel>
</rss>
