<p align="center">
<img height="128" width="128" src="logo.svg" alt="WillBoard logo">
</p>

## WillBoard

> Less repeat - More unique

WillBoard is an open source anonymous forum engine with a few main assumptions: 
- Ability to read and write posts without JavaScript 
- Using only HTTP protocol (without WebSockets)

## Requirements

- [.NET 9.0](https://github.com/dotnet/core/blob/main/release-notes/9.0/README.md ".NET 9.0")
- [MariaDB](https://mariadb.org/download/ "MariaDB") server (minimum [10.6.0](https://mariadb.com/kb/en/select-offset-fetch/ "10.6.0")) 
- [FFmpeg / FFprobe](https://ffmpeg.org/download.html "FFmpeg / FFprobe") (optional for audio/video processing)
- [Windows](https://github.com/dotnet/core/blob/main/release-notes/9.0/supported-os.md#windows "Windows") or [Linux](https://github.com/dotnet/core/blob/main/release-notes/9.0/supported-os.md#linux "Linux") or [macOS](https://github.com/dotnet/core/blob/main/release-notes/9.0/supported-os.md#macos "macOS")

## Engine

### WillBoard.Core

Project contains all entities, enums, exceptions, interfaces, types and logic specific to the core layer.

### WillBoard.Application

Project contains all the logic responsible for processing requests using CQRS pattern.

### WillBoard.Infrastructure

Project contains logic for accessing external resources such as database, cache, file systems, web services and so on. 

### WillBoard.Web

Startup project for the engine. Is responsible for handling requests and formatting answers (eg. HTML, JSON).

## Version

0.2.2

## Documentation

[Documentation](docs/README.md)

## License

[Unlicense](UNLICENSE.txt)