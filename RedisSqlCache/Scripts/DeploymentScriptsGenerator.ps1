param(
[string]$InstallerTemplatePath,
[string]$UninstallerTemplatePath,
[string]$FinalInstallerScriptPath,
[string]$FinalUninstallerScriptPath,
[string]$SolutionDir,
[string]$RedisqlBinDir,
[string]$RedisqlProjDir
)
$sqlScriptsGeneratorPath = [IO.Path]::Combine($SolutionDir, "ScriptGenerator","InstallerScriptGenerator.exe")
Write-Host $sqlScriptsGeneratorPath
$installerTemplateText = [IO.File]::ReadAllText($InstallerTemplatePath)
$tmpInstallerFile = [IO.Path]::GetTempFileName()
[IO.File]::WriteAllText($tmpInstallerFile, $installerTemplateText.Replace("!!!binDir!!!", $RedisqlBinDir).Replace("!!!projDir!!!", $RedisqlProjDir))
write-host $tmpInstallerFile
Start-Process -FilePath $sqlScriptsGeneratorPath -ArgumentList "$tmpInstallerFile $FinalInstallerScriptPath" -Wait

$uninstallerTemplateText = [IO.File]::ReadAllText($UninstallerTemplatePath)
$tmpUninstallerFile = [IO.Path]::GetTempFileName()
[IO.File]::WriteAllText($tmpUninstallerFile, $uninstallerTemplateText.Replace("!!!binDir!!!", $RedisqlBinDir).Replace("!!!projDir!!!", $RedisqlProjDir))
write-host $tmpUninstallerFile
Start-Process -FilePath $sqlScriptsGeneratorPath -ArgumentList "$tmpUninstallerFile $FinalUninstallerScriptPath" -Wait