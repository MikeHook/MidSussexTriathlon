#Use this command to run the script
#&("D:\dev\MidSussexTriathlon\deploymentScripts\beforePackaging.ps1")

Write-Host 'Running before packaging script'

#$root = 'D:\Temp'
#$root = 'C:\projects\mstc-f3l95\website'
$root =  $env:APPVEYOR_BUILD_FOLDER + '\build\MidSussexTriathlon.Web'
Write-Host $root

$configPath = $root + '\Web.config'
$webconfig = get-content $configPath
#Write-Host $webconfig

function ReplaceAppSetting($sourceFile, $keyName, $replacementValue) {
    $sourceFile -replace "<add key=`"$keyName`" value=`".*`"","<add key=`"$keyName`" value=`"$replacementValue`""
}

$webconfig = $webconfig -replace "connectionString=`"([^`"]+)`"","connectionString=`"$env:databaseConnectionString`""

if ($env:contactFormEmailTo)
{
	$webconfig = ReplaceAppSetting $webconfig "contactFormEmailTo" $env:contactFormEmailTo
}
if ($env:umbracoUseSSL)
{
    $webconfig = ReplaceAppSetting $webconfig "umbracoUseSSL" $env:umbracoUseSSL
}

[System.IO.File]::WriteAllLines($configPath, $webconfig)

Write-Host 'Finished before packaging script'