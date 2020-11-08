# TN3270Sharp - 3270 Server Library for DotNetCore / C#
 a .net library to write applications/servers with C# that use TN3270 clients

## Inspiration
This library was heavily inspired by [racingmars/go3270](https://github.com/racingmars/go3270) which has a similar goal, except it is designed to use golang instead of C#.  After watching a demo video of go3270, I decided that I needed to have the same functionality for my favorite developer ecosystem.

## License
TN3270Sharp is licensed under the MIT License which permits commercial use; modification; distribution, and private use.

## Acknowledgements
I have borrowed / adapted some code from [racingmars/go3270](https://github.com/racingmars/go3270) and [Open3270/Open3270](https://github.com/Open3270/Open3270), both of which are also released under the MIT License.

## Status
This project is still very early in development and is not suitable for use any project beyond experimentation. There is most certainly bugs and missing features.  The API will also likely change many times before it is declared ready for any realworld use.

## Requirements
This will run on any machine support by Microsoft dotnetcore 3.1+.
This includes:
* Linux (x64, ARM32, ARM64) - This also includes Raspberry Pi 3 & 4.
* macOS (x64)
* Microsoft Windows (x86, x64)

## How to use
* Install [.NET Core 3.1+](https://dotnet.microsoft.com/download/dotnet-core/3.1)
* Close the repo
* Type the following in the shell

```
cd TN3270Sharp.Example.App
dotnet run
```
* launch your TN3270 emulator and connect to 127.0.0.1 port 9999

## Reference Material
* [racingmars/go3270](https://github.com/racingmars/go3270)
* [Open3270/Open3270](https://github.com/Open3270/Open3270)
* [Tommy Sprinkle's 3270 Data Stream Programming Material](http://www.tommysprinkle.com/mvs/P3270/)
* [RFC 1576: TN3270 Current Practices](https://tools.ietf.org/html/rfc1576)
* [RFC 1041: Telnet 3270 Regime Option](https://tools.ietf.org/html/rfc1041)
* [RFC 854: Telnet Protocol Specification](https://tools.ietf.org/html/rfc854)

## Contributions
Pull requests and bug reports are welcome.