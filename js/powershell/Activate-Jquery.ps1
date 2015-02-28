param(
   [string]$SiteUrl,   
   [string]$UserName, 
   [string]$Password   
)
 
. ".\UserCustomActions.ps1"
 
<#  
.SYNOPSIS  
    Enable jQuery Library        
.DESCRIPTION  
    Enable jQuery Library in Office 365/SharePoint Online site 
.EXAMPLE
    .\Activate-JQuery.ps1 -SiteUrl "https://tenant-public.sharepoint.com" -UserName "username@tenant.onmicrosoft.com" -Password "password"
#>
Function Activate-JQuery([string]$SiteUrl,[string]$UserName,[string]$Password)
{
    $context = New-Object Microsoft.SharePoint.Client.ClientContext($SiteUrl)
    $context.Credentials = Get-SPOCredentials -UserName $UserName -Password $Password
 
    $sequenceNo = 1482
    $jQueryUrl = "~SiteCollection/SiteAssets/js/libs/jquery-2.1.3.min.js"
    Add-ScriptLinkAction -Context $Context -ScriptSrc $jQueryUrl -Sequence $sequenceNo

     $sequenceNo = 1483
    $jQueryUrl = "~SiteCollection/SiteAssets/js/appLoader.js"
    Add-ScriptLinkAction -Context $Context -ScriptSrc $jQueryUrl -Sequence $sequenceNo 
    $context.Dispose()
}
 
 
Activate-JQuery -SiteUrl $SiteUrl -UserName $UserName -Password $Password