function Out-BranchName {
 param( [string]$fullName )
 if ($fullName.StartsWith("refs/heads/") -eq $TRUE) {
    return $fullName.Substring(11);
  }
  return $fullName;
}

function Add-Property {
    param(
        [System.Xml.XmlDocument] $doc,
        [string] $name,
        [string] $value
    )
    
    $child = $doc.CreateElement("Property")
    $child.SetAttribute("Name", $name)
    $child.InnerText = $value
    $child.RemoveAttribute("xmlns")
    $doc.DocumentElement.AppendChild($child)
}

$doc = New-Object System.Xml.XmlDocument
$doc.Load("./SonarQube.Analysis.temp.xml")

Add-Property -doc $doc -name "sonar.login" -value $env:SC_LOGIN
Add-Property -doc $doc -name "sonar.projectVersion" -value $env:BUILD_BUILDNUMBER

$prId = $env:SYSTEM_PULLREQUEST_PULLREQUESTID
if ($prId) {
    Add-Property -doc $doc -name "sonar.pullrequest.base" -value $env:SYSTEM_PULLREQUEST_TARGETBRANCH
    Add-Property -doc $doc -name "sonar.pullrequest.branch" -value $env:SYSTEM_PULLREQUEST_SOURCEBRANCH
    
    Add-Property -doc $doc -name "sonar.pullrequest.key" -value $env:SYSTEM_PULLREQUEST_PULLREQUESTNUMBER
    Add-Property -doc $doc -name "sonar.pullrequest.provider" -value github
    Add-Property -doc $doc -name "sonar.pullrequest.github.repository" -value $env:REPO_NAME_VAR
} else {
    $isDefaultBranch = $TRUE;
    $currentBranch = $env:BUILD_SOURCEBRANCH
    $isDefaultBranch = $currentBranch -eq 'refs/heads/master';
    if ($isDefaultBranch -ne $TRUE) {
        $formattedBranchName = Out-BranchName -fullName $env:BUILD_SOURCEBRANCH
      # // VSTS-165 don't use Build.SourceBranchName
      Add-Property -doc $doc -name "sonar.branch.name" -value $formattedBranchName
    }
}

$doc.Save("./SonarQube.Analysis.xml");
