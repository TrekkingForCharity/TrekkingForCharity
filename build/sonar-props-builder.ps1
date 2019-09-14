Add-Content ./sonar.properties "sonar.projectKey=trekking-for-charity"
Add-Content ./sonar.properties "sonar.projectName=TrekkingForCharity"
Add-Content ./sonar.properties "sonar.projectVersion=$env:BUILD_BUILDNUMBER"


$prId = $env:SYSTEM_PULLREQUEST_PULLREQUESTID
if ($prId) {
    Add-Content ./sonar.properties "sonar.pullrequest.base=$env:SYSTEM_PULLREQUEST_TARGETBRANCH"
    Add-Content ./sonar.properties "sonar.pullrequest.branch=$env:SYSTEM_PULLREQUEST_SOURCEBRANCH"
    
    Add-Content ./sonar.properties "sonar.pullrequest.key=$env:SYSTEM_PULLREQUEST_PULLREQUESTNUMBER"
    Add-Content ./sonar.properties "sonar.pullrequest.provider=github"
    Add-Content ./sonar.properties "sonar.pullrequest.github.repository=$env:REPO_NAME_VAR"
} else {
    $isDefaultBranch = true;
    $currentBranch = $env:BUILD.SOURCEBRANCH
    $isDefaultBranch = $currentBranch -eq 'refs/heads/master';
    if (!isDefaultBranch) {
      # // VSTS-165 don't use Build.SourceBranchName
      Add-Content ./sonar.properties "sonar.branch.name=".BranchName($env:BUILD_SOURCEBRANCH)
    }
}

function BranchName {
 param( [string]$fullName )
 if ($fullName.StartsWith("refs/heads/")) {
    return $fullName.Substring(11);
  }
  return $fullName;
}