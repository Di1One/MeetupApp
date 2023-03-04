# MeetupApp


The project implements a crud service for events.

Table of contents

- [About](#1-about)
  - [Main application features](#11-main-application-features)
- [How to configure](#2-how-to-configure)
  - [Configure WebAPI application](#21-configure-webapi-application)
- [How to run](#3-how-to-run)
  - [How to run WebAPI application](#31-how-to-run-webapi-application)
- [Description of the project architectur](#4-description-of-the-project-architectur)
  - [Summary](#41-summary)
  - [Web API project](#42-web-api-project)

## 1. About

This project implements an API for working with events using ASP.NET Core WebAPI.

### 1.1. Main application features

- authorization system,
- crud operations with events.

## 2. How to configure

### 2.1. Configure WebAPI application

#### 2.1.1. Change database connection string

You should change connection string in `appsettings.json`

```json
"ConnectionStrings": {
    "Default": "Server=myServer;Database=myDataBase;Trusted_Connection=True;;TrustServerCertificate=True"
  },
```

 **Note**
> The project uses the GUID type as the primary and therefore using other database providers requires additional changes and adjustments.

#### 2.1.2. Initiate database

Open the Packege Manager Console in Visual Studio (VIew -> Other Windows -> Packege Manager Console). Choose MeetupApp.DataBase in Default project.

Use the following command to create or update the database schema.

```console
PM> update-database
```

> **Note**
> In this step, you will create a new database and. In table Roles you need to add from MS SQl Management [Guid("040B4705-7386-4A11-B73B-32AE4BB7469E")] and name "User", there is no starter set sry =(

#### 2.1.3. Change path to documentation file for WebAPI

You need to specify the path to the file in which the documentation will be saved when building the solution. To do this, change the following code block in the MeetupApp.WebAPI project settings:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    ...
    <DocumentationFile>C:\Users\user\source\repos\ElectronicsStore\WebAPI\ElectronicsStore.WebAPI\doc.xml</DocumentationFile>
  </PropertyGroup>
  ...
</Project>
```

> **Note**
> Make sure that the `doc.xml` file exists in the path you specify.
After that it is necessary to specify the same path in in `appsettings.json`
```json
"XmlDoc": "E:\\C#\\MeetupApp\\MeetupApp.WebAPI\\doc.xml",
```

You need to set the path to the `doc.xml` file, which is in the root of the WebAPI solution. You can also create (or copy) the `doc.xml` file to any convenient location. In this case, specify the path to the new location of the doc file. In this case, the doc.xml file can be empty. Documentation is generated automatically each time the solution is built.

> **Note**
> This documentation file is an important part of the solution. It allows API users to understand what the system expects from them and what they can get from the server. The solution will generate an error when building without the correct path to the documentation file.

#### 2.1.4. Change the salt

Be sure to change the salt value before running the application. This will increase the reliability of created passwords.

Enter any phrase or character set in `appsettings.json`:
```json
  "Secrets": {
    "PasswordSalt": "WRITE YOUR SECRET PHRASE HERE"
  },
```
#### 2.1.5. Change Token configuration

The application uses Bearer JWT authorization.

You need to change the value of JWT Secret in `appsettings.json`:
```json
"Token": {
    "JwtSecret": "YOUR API SECRET",
```
In addition, you should change the expiration date of the generated tokens. It is 15 minute by default, but it is acceptable to use a value between 15 minutes and several hours.
To override the default value change `appsettings.json`:
```json
  "Token": {
    "ExpiryMinutes": 15
  }
```
You can leave the value unchanged. It won't affect the user experience, but it will reduce the load on the server and user security. The application uses RefreshToken to automatically generate new tokens to replace expired ones.

## 3. How to run

### 3.1. How to run WebAPI application

Run the project using the standard Visual Studio tools or the dotnet CLI.

## 4. Description of the project architectur.

### 4.1. Summary

The application consists of the Web API project. 

### 4.2. Web API project

The application is based on ASP.NET Core Web API and Microsoft SQL Server. Entity Framework Core is used to work with the database. The interaction between the application and the database is done using the Generic Repository and Unit of Work.

The application writes logs to a new file for each run. Logging is based on the Serilog library.

Key functions of the server part:

- generic repository
- unit of work
- rest API
- Swagger
- API documentation (not all OpenAPI Specification requirements are met)

#### 4.2.3. Composition of Web API solution

The solution contains the main project and several libraries:

- **MeetupApp.WebAPI:** main API project
- **MeetupApp.Business:** contains the basic business logic of the application not directly related to API (services implementations and etc.)
- **MeetupApp.Core:** contains entities that do not depend on the implementation of other parts of the application (interfaces of services, data transfer objects, patch model)
- **MeetupApp.Data.Abstractions:** contains interfaces for database logic utils
- **MeetupApp.Data.Repositories:** contains implementation of Data.Abstractions
- **MeetupApp.DataBase:** contains entities and DBContext class

#### 4.2.4. Controllers

The **MeetupApp.WebAPI** contains four controllers:

- **UserController:** controller providing 1 endpoint (new user registration).
- **TokenController:** controller that provides multiple endpoints for interacting with the access token:
  - _CreateJwtToken:_ endpoint for user login.
  - _RefreshToken:_ generate a new access token based on the refresh token.
  - _RevokeToken:_ revoke refresh token in the database. Use this endpoint every time the user logs out.
- **EventController:** controller providing access to the `Events` resource.



## Key features:

ASP.Net Core WebAPI, Entity Framework Core, Microsoft SQL Server, C#, Serilog, Automapper, Dependepcy Injection, Generic Repository.
