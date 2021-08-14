set -ex

cd $(dirname $0)/../

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

mkdir -p $artifactsFolder

dotnet build ./src/Peach/Peach.csproj -c Release

dotnet pack ./src/Peach/Peach.csproj -c Release -o ./$artifactsFolder

dotnet nuget push ./$artifactsFolder/Peach.*.nupkg -k $NUGET_KEY -s https://www.nuget.org