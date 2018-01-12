# L2ACP-Web
This is the web interface project for L2ACP

|  Supported languages        |
|:-------------:|
| English |
| Portuguese |

## Requirements to build and run
* .NET Core runtime 1.1
* .NET Core SDK 1.1
* Visual Studio 2017 Community Edition

## How to build
1. Clone the source code:
```
git clone https://github.com/Elfocrash/L2ACP-Web.git
```
2. Open Visual Studio, do a `dotnet restore` and then publish the project.

## L2OFF Configuration
* Install L2OFF server database
* Run L2OFF_L2ACP.SQL script to modify lin2world and lin2db database
* Change appsettings.json:
** Set TargetServerType to "L2OFF"
** Set PasswordHashType to "Default" if using default password hashing or "MD5" if using hAuthD's MD5 hashing
** Set appropriate connection strings for the lin2world and lin2db databases
* In server's ilExt.ini set "[PrivateStore] StoreInDB=1" for private stores and "[ItemDelivery] Enabled=1" for item delivery