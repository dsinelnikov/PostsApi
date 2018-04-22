Configuration Main
{

Param (
	[string] $nodeName,
	[string] $sqlServerName,
	[string] $sqlServerLogin,
	[string] $sqlServerPassword
)

Node $nodeName
  {
	Script InstallNetCore
	{
		TestScript={
			return $false
		}
		SetScript = {

			Write-Host "About to clear .NET cache from my profile..."
			$dotnetProfileFolder = "C:\Users\$env:USERNAME\.dotnet"

			If(Test-Path $dotnetProfileFolder){
				Remove-Item $dotnetProfileFolder\* -recurse
			}

			Write-Host "Install nuget"
			Install-PackageProvider -Name "Nuget" -Force

			Write-Host "About to delete existing .NET Core binaries..."
			$dotNetSdkFolder = "C:\Program Files\dotnet"
			If(Test-Path $dotNetSdkFolder){
				Remove-Item $dotNetSdkFolder\* -recurse
			}

			Write-Host "About to download latest .NET Core 2 binaries..." -ForegroundColor Green
			$url = "https://download.microsoft.com/download/1/1/5/115B762D-2B41-4AF3-9A63-92D9680B9409/dotnet-sdk-2.1.4-win-x64.zip"
			$output = "$dotNetSdkFolder\dotnet-dev-win-x64.latest.zip"

			Import-Module BitsTransfer
			If(!(Test-Path $dotNetSdkFolder)){
				New-Item -Path $dotNetSdkFolder -ItemType Directory -Force
			}
			Start-BitsTransfer -Source $url -Destination $output

			Write-Host "About to unzip latest .NET Core 2 binaries..." -ForegroundColor Green
			$shell = new-object -com shell.application
			$zip = $shell.NameSpace($output)
			foreach($item in $zip.items())
			{
				$shell.Namespace($dotNetSdkFolder).copyhere($item)
			}

			$env:PATH += ";" + $dotNetSdkFolder

			Write-Host "Done - dotnet version installed is:" -ForegroundColor Green
			dotnet --version

			Write-Host "Deploy PostApi service"
			$serviceSourseUrl = "https://github.com/dsinelnikov/PostsApi/archive/master.zip"
			$serviceTemp = "$env:TEMP\api-service1-" + (Get-Random -Minimum 10000000 -Maximum 99999999)
			$serviceOutput = "C:\www"
			$serviceZipOutput = $serviceTemp + "\sources.zip"

			If(!(Test-Path $serviceTemp)){
				New-Item -Path $serviceTemp -ItemType Directory -Force
			}

			Write-Host "Download posts service sources" -ForegroundColor Green
			Start-BitsTransfer -Source $serviceSourseUrl -Destination $serviceZipOutput

			$serviceShell = New-Object -com shell.application
			$serviceZip = $serviceShell.NameSpace($serviceZipOutput)

			Write-Host ("Unpack sources to temp directory '" +  $serviceTemp + "'") -ForegroundColor Green
			foreach($item in $serviceZip.items()){
				foreach($subItem in $serviceShell.Namespace($item).items()){		
					$serviceShell.Namespace($serviceTemp).copyhere($subItem)	
				}
			}

			Write-Host "Update Sql connection string" -ForegroundColor Green

			$appSettingsPath = $serviceTemp + '\appsettings.json'
			$appsettings = Get-Content $appSettingsPath -Raw | ConvertFrom-Json

			$conn = $appsettings.ConnectionStrings.MyDbConnection -replace '\{server\}', $sqlServerName
			$conn = $conn -replace '\{login\}', $sqlServerLogin
			$conn = $conn -replace '\{password\}', $sqlServerPassword

			$appsettings.ConnectionStrings.MyDbConnection = $conn
			$appsettings | ConvertTo-Json | Set-Content $appSettingsPath

			Write-Host "dotnet restore" -ForegroundColor Green
			Set-Location -Path $serviceTemp
			dotnet restore

			Write-Host "Publish service" -ForegroundColor Green
			If(!(Test-Path $serviceOutput)){
				New-Item -Path $serviceOutput -ItemType Directory -Force
			}
			dotnet publish -c Release -o $serviceOutput

			Write-Host "Run service" -ForegroundColor Green
			Set-Location -Path $serviceOutput
			$env:ASPNETCORE_URLS="http://*:80"
			Start-Process -FilePath "dotnet" -ArgumentList "PostsApi.dll"

			netsh advfirewall firewall add rule name="Open Port 80" dir=in action=allow protocol=TCP localport=80
		}
		GetScript = {@{Result = "InstallNetCore"}}
	}
  }
}