<?php

header("Content-Type: audio/mpeg");

$p = $_GET["p"];
$pubDate = basename($p);
$podcastRoot = "/media/trek/Podcasts/Car Talk/episodes";
$episodePath = $podcastRoot . DIRECTORY_SEPARATOR . $pubDate;

$canReturn = false;

if (is_dir($episodePath))
{
    $dirContents = scandir($episodePath);

    foreach ($dirContents as $key => $dirItem)
    {
        $fullDirItem = $episodePath . DIRECTORY_SEPARATOR . $dirItem;

        if (!in_array($fullDirItem, array(".", "..")))
        {
            if (!is_dir($fullDirItem))
            {
                $extension = pathinfo($fullDirItem, PATHINFO_EXTENSION);
                $extension = strtolower($extension);

                if (str_ends_with($extension, "mp3"))
                {
                    $realPath = realpath($fullDirItem);
            
                    // Ensure the file returned is in the root directory
                    if (strpos($realPath, $podcastRoot) !== false)
                    {
                        $canReturn = true;
                        break;
                    }
                }
            }
        }
    }
}

if ($canReturn)
{
    readfile($realPath);
}
else
{
    header("HTTP/1.0 404 Not Found");
}

function str_ends_with( $haystack, $needle ) {
    $length = strlen( $needle );
    if( !$length ) {
        return true;
    }
    return substr( $haystack, -$length ) === $needle;
}

?>