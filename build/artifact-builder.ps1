function Add-Property {
    param(
        [System.Xml.XmlDocument] $doc,
        [string] $name,
        [string] $value
    )
    
    $child = $doc.CreateElement("Property", $doc.DocumentElement.NamespaceURI)
    $child.SetAttribute("Name", $name)
    $child.InnerText = $value
    $doc.DocumentElement.AppendChild($child)
}

$doc = New-Object System.Xml.XmlDocument
$doc.Load("./Build.Artifacts.temp.xml")

Add-Property -doc $doc -name "build.version" -value $env:GITVERSION_SEMVER
Add-Property -doc $doc -name "github.commitSha" -value $env:GITVERSION_SHA
Add-Property -doc $doc -name "github.commitShortSha" -value $env:GITVERSION_SHORTSHA

$doc.Save("./Build.Artifacts.xml");