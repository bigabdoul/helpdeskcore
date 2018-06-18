# Help Desk Core
Help Desk Core is an issues management application built with an Angular (v5.2.1) frontend and ASP.NET Core 2.0. Its goal is to provide the staff of corporate businesses with an issues reporting and tracking tool that they could use in their daily work.

## Development Environment
- Sql Server Express 2017 & Sql Server Management Studio 2017
- Runs in both Visual Studio 2017 & Visual Studio Code
- Node 8.11.1 & NPM 5.6.0
- .NET Core 2.0 sdk
- Angular CLI -> `npm install -g @angular/cli` https://github.com/angular/angular-cli

## Setup
To build and run the project using the command line:

```
git clone https://github.com/bigabdoul/helpdeskcore.git
git clone https://github.com/bigabdoul/efcore-repository.git
git clone https://github.com/bigabdoul/coretools.git
git clone https://github.com/bigabdoul/mailkit-tools.git
cd helpdeskcore/src
nmp install
dotnet restore
dotnet ef database update
dotnet run
```
