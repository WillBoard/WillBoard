# Installation

## 1. Add FFmpeg/FFprobe binaries (optional)
You do not need to add binaries for all platforms, place only those you will need.

- `FFmpeg` (`src\WillBoard.Web\Assets\FFmpeg`):
  - Windows: `ffmpeg-win-64.exe`
  - Linux: `ffmpeg-linux-64`
  - macOS: `ffmpeg-osx-64`
- `FFprobe` (`src\WillBoard.Web\Assets\FFprobe`):
  - Windows: `ffprobe-win-64.exe`
  - Linux: `ffprobe-linux-64`
  - macOS: `ffprobe-osx-64`

If you want to use system environment variables, modify `InstanceFFtoolService`.

## 2. Setup database

In `scripts` folder you can find file to setup database, tables and default data. 

By default first administrator `AccountId` is `00000000-0000-0000-0000-000000000000` and `Password` is `password`.

## 3. Add configuration

Update `configuration.json` file (`src\WillBoard.Web`) and add database connection string.

## 4. Run application

- If you want run it locally use [`dotnet run`](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-run "dotnet run")

- If you want run it on server use [`dotnet publish`](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-publish "dotnet publish") for publish application (eg. `dotnet publish "src\WillBoard.Web\WillBoard.Web.csproj" -c Release -r linux-x64 --self-contained -o "OUTPUT_DIRECTORY"`) and deploy it. After deploy run application on server (eg. `./WillBoard.Web --urls "http://localhost:5000"`)

Remember to set proper file permission for application and binaries (read, write, execute).

