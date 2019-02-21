# UntisNotifier
## What is this?
This is a tool for the [WebUntis](https://mese.webuntis.com/WebUntis) website, that sends notifications through different configurable channels like Telegram or E-Mail to inform you about changes of your school lessons.

## Which services do whe currently support?
* Telegram
* E-Mail

## How do I install this?
Configfile location under Linux: `$pwd/.config/UntisNotifier`

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
  


