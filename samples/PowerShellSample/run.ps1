Set-Location $PSScriptRoot
[IO.Directory]::SetCurrentDirectory($PSScriptRoot)

Add-Type -AssemblyName PresentationFramework
[Reflection.Assembly]::LoadFrom('bin\Debug\net5.0-windows10.0.18362.0\System.ValueTuple.dll') | Out-Null
[Reflection.Assembly]::LoadFrom('bin\Debug\net5.0-windows10.0.18362.0\ModernWpf.dll') | Out-Null
[Reflection.Assembly]::LoadFrom('bin\Debug\net5.0-windows10.0.18362.0\ModernWpf.Controls.dll') | Out-Null

$xaml = Get-Content "MainWindow.xaml" -Raw
$window = [Windows.Markup.XamlReader]::Parse($xaml)
$window.ShowDialog()