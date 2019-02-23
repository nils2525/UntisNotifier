# UntisNotifier
## What is this?
This is a tool for the [WebUntis](https://mese.webuntis.com/WebUntis) website, that sends notifications through different configurable channels like Telegram or E-Mail to inform you about changes of your school lessons.

## Which services do whe currently support?
* Telegram
* E-Mail

## How do I install this?
* [Download the latest release](https://github.com/nils2525/UntisNotifier/releases)
* Unzip the binaries in folder that your prefer (for example `$pwd/bin/UntisNotifier`)
* Create a configuration in the Configfile location
* Start it with `dotnet $binariesLocation/UntisNotifier.dll`

Configfile location under Linux: `$pwd/.config/UntisNotifier`  
Configfile location under Windows: `%appdata%\UntisNotifier\UntisNotifier.json`

## Requirements
[Latest .NET Core Runtime](https://dotnet.microsoft.com/download)

## Example config file
```json
{
  "NOTE": "THIS IS ONLY A EXAMPLE CONFIG",
  "user" : {
    "name" : "userName",
    "password" : "password",
    "school" : "school"
  },
  "notifiers" : {
    "console" : {
      "active" : "true"
    },
    "email" : {
      "active" : "true",
      "fromEmail" : "email",
      "toEmail" : "email",
      "password" : "password",
      "smtpServer" : "server",
      "smtpPort" : "port"
    },
	"telegram" : {
	  "active" : "true",
      "token" : "token",
      "chatId" : "chatId"
	}
  }
}
```
  
## Troubleshooting
### The programm returns exit code -2147450749 (0x80008083)
You need to have installed [the latest version of .NET Core](https://dotnet.microsoft.com/download)

