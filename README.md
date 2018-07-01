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
npm install
dotnet restore
dotnet ef database update
dotnet run
```

## Database setup
To get the database up and running in the development environment, there's nothing else to do after running the Entity Framework migrations `dotnet ef database update`.

## Configuration settings and sections

When deploying to production you will have to generate the SQL scripts from the updated database and edit the `src/appsettings.json` file accordingly.

1. `ConnectionStrings`: Change the `DefaultConnection` property to a valid connection string.

2. `IdentityInitializerSettings`: Pay special attention to this configuration section. The `AdminId` property needs to be changed to a new GUID as well as the other properties of that section: `AdminEmail, AdminUserName, AdminPassword`. Change them to your liking. You should normally edit this section BEFORE navigating to the application's launch URL (http://localhost:5000 in dev mode) because it seeds the database with these settings. In other words, these settings are used to create the application's built-in (super) administrator. The password should be changed after the first time login. Unfortunately, this feature (change password) hasn't been implemented yet by the time of this writing. 

3. `JwtIssuerOptions`: The `Audience` property, which defaults to `http://localhost:5000` needs to be changed to the host name of the deployed application; otherwise, API calls coming from other URLs will fail. At the same time you should make the same changes to the `src/src/environments/environment.prod.ts` file like so:
`export const environment = { production: true, baseUrl: 'http://example.com/api' };`

4. `FacebookAuthSettings`: If you'd like to allow users to authenticate on the site via Facebook, then you'll have to set the appropriate `AppId` and `AppSecret` accordingly. The default AppId and AppSecret are those from [the project this one is derived from](https://fullstackmark.com/post/13/jwt-authentication-with-aspnet-core-2-web-api-angular-5-net-core-identity-and-facebook-login#creating-a-facebook-application).

5. `SecurityKey`: Update this configuration setting to something else, unique and complex. It's used in `Startup.cs` to initialize an instance of the `SymmetricSecurityKey` class, which in turn is used to configure the `JwtIssuerOptions.SigningCredentials` property.

## Deployment

### Deployment on Windows with IIS

#### Supported operating systems
Deploying an ASP.NET Core application is supported only on Windows 7 or later, and Windows Server 2008 R2 or later.

#### Step by step instructions

1. Make sure that the web server features are installed. Read [this article](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-2.0&tabs=aspnetcore2x) for detailed instructions.
2. Install the [.NET Core Hosting Bundle](https://www.microsoft.com/net/download/thank-you/dotnet-runtime-2.0.8-windows-hosting-bundle-installer).
3. Download and install the [.NET Core 2.0 Runtime (v2.0.8)](https://www.microsoft.com/net/download/dotnet-core/runtime-2.0.8). Installing the .NET Core 2.1 Runtime will break the application because of a regression bug in the latter.

   Here are some quick links for different architectures:
   * [x64 Installer](https://www.microsoft.com/net/download/thank-you/dotnet-runtime-2.0.8-windows-x64-installer) (To prevent the installer from installing x86 packages on an x64 OS, run the installer from an administrator command prompt with the switch `OPT_NO_X86=1` e.g. `DotNetCore.2.0.8-WindowsHosting.exe /OPT_NO_X86=1`)
   * [x86 Installer](https://www.microsoft.com/net/download/thank-you/dotnet-runtime-2.0.8-windows-x86-installer)
4. Create the IIS site (e.g. HelpDeskCore). Troubles come when configuring the access rights to the folder that contains the application files. You merely have to deal with the Application Pool Identity permissions. The easiest way to avoid trouble is to use an administrative user account, explicitly picking up a user and set the appropriate password. Check out [this article](https://blogs.msdn.microsoft.com/johan/2008/10/01/powershell-editing-permissions-on-a-file-or-folder/) for a better approach. In the context of an ASP.NET Core application deployment on Windows, try the following:
```
$Path = "C:\inetpub\wwwroot\helpdeskcore"  
$User = "IIS AppPool\HelpDeskCore"  
$Acl = Get-Acl $Path  
$Ar = New-Object  system.security.accesscontrol.filesystemaccessrule($User,"FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")  
$Acl.SetAccessRule($Ar)  
Set-Acl $Path $Acl  
```
Of course, the above parameters need to be replaced with those you've actually used.