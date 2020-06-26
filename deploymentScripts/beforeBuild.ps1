#Use this command to run the script
#&("D:\dev\MidSussexTriathlon\deploymentScripts\beforePackaging.ps1")

Write-Host 'Running before build script'

#$root = 'D:\Temp'
#$root = 'D:\dev\MidSussexTriathlon\build\MidSussexTriathlon.Web'
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
if ($env:stripePublicKey)
{
    $webconfig = ReplaceAppSetting $webconfig "stripePublicKey" $env:stripePublicKey
}
if ($env:stripeSecretKey)
{
    $webconfig = ReplaceAppSetting $webconfig "stripeSecretKey" $env:stripeSecretKey
}
if ($env:stripeEndpointSecret)
{
    $webconfig = ReplaceAppSetting $webconfig "stripeEndpointSecret" $env:stripeEndpointSecret
}
if ($env:emailTestMode)
{
    $webconfig = ReplaceAppSetting $webconfig "emailTestMode" $env:emailTestMode
}
if ($env:emailTestAddress)
{
    $webconfig = ReplaceAppSetting $webconfig "emailTestAddress" $env:emailTestAddress
}
if ($env:emailUserName)
{
    $webconfig = ReplaceAppSetting $webconfig "emailUserName" $env:emailUserName
}
if ($env:emailPassword)
{
    $webconfig = ReplaceAppSetting $webconfig "emailPassword" $env:emailPassword
}
if ($env:entryEmailUserName)
{
    $webconfig = ReplaceAppSetting $webconfig "entryEmailUserName" $env:entryEmailUserName
}
if ($env:entryEmailPassword)
{
    $webconfig = ReplaceAppSetting $webconfig "entryEmailPassword" $env:entryEmailPassword
}


[System.IO.File]::WriteAllLines($configPath, $webconfig)

Write-Host 'Finished before build script'