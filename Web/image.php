<?php

header("Content-Type: image/jpeg");

$p = $_GET["p"];
$filename = basename($p);
$podcastRoot = "/media/trek/Podcasts/Car Talk/channel";
$filePath = $podcastRoot . DIRECTORY_SEPARATOR . $filename;

$canReturn = false;

if (file_exists($filePath))
{
    if (!is_dir($filePath))
    {
        $realPath = realpath($filePath);
        
        // Ensure the file returned is in the root directory
        if (strpos($realPath, $podcastRoot) !== false)
        {
            $canReturn = true;
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

?>
