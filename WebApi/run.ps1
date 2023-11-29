# I admire your curiosity, but there is nothing to look at... really
# When I say "A little script" I mean it

# Build Api
dotnet build

# Specify the path to the API project folder
$apiPath = "C:\Git\BlackLion\WebApi\WebApi"

# Navigate to the API project folder
set-location $apiPath

# Run the .NET API in the background using `dotnet run`
Start-Process -FilePath "dotnet" -ArgumentList "run", "--urls=https://localhost:44381/" -NoNewWindow -PassThru