function Output-BranchName {
 param( [string]$fullName )
 if ($fullName.StartsWith("refs/heads/") -eq $TRUE) {
    return $fullName.Substring(11);
  }
  return $fullName;
}

Add-Content ./sonar.properties "sonar.projectKey=trekking-for-charity"
Add-Content ./sonar.properties "sonar.projectName=TrekkingForCharity"
Add-Content ./sonar.properties "sonar.projectVersion=$env:BUILD_BUILDNUMBER"
Add-Content ./sonar.properties "sonar.cs.opencover.reportsPaths=**/results.opencover.xml"
Add-Content ./sonar.properties "sonar.host.url=https://sonarcloud.io"
Add-Content ./sonar.properties "sonar.login=$env:SC_LOGIN"



$prId = $env:SYSTEM_PULLREQUEST_PULLREQUESTID
if ($prId) {
    Add-Content ./sonar.properties "sonar.pullrequest.base=$env:SYSTEM_PULLREQUEST_TARGETBRANCH"
    Add-Content ./sonar.properties "sonar.pullrequest.branch=$env:SYSTEM_PULLREQUEST_SOURCEBRANCH"
    
    Add-Content ./sonar.properties "sonar.pullrequest.key=$env:SYSTEM_PULLREQUEST_PULLREQUESTNUMBER"
    Add-Content ./sonar.properties "sonar.pullrequest.provider=github"
    Add-Content ./sonar.properties "sonar.pullrequest.github.repository=$env:REPO_NAME_VAR"
} else {
    $isDefaultBranch = $TRUE;
    $currentBranch = $env:BUILD_SOURCEBRANCH
    $isDefaultBranch = $currentBranch -eq 'refs/heads/master';
    if ($isDefaultBranch -ne $TRUE) {
        $formattedBranchName = Output-BranchName -fullName $env:BUILD_SOURCEBRANCH
      # // VSTS-165 don't use Build.SourceBranchName
      Add-Content ./sonar.properties "sonar.branch.name=$formattedBranchName"
    }
}

