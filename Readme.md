# Blazor Server JWT (Identity OR LDAP) Sample

## Backend.API

### Overview

Backend.API is an ASP.NET Core Web API project built using .NET 8. It provides a powerful authentication and authorization system using a customized ASP.NET Core Identity, role-based and policy-based authorization, JWT token generation (both access-token and refresh-token), and support for LDAP (Active Directory) authentication. The project utilizes Entity Framework Core in a Code-First approach to handle database entities.

### Features

- Customized ASP.NET Core Identity for authentication.
- Refresh token generation for extended user sessions.
- Role-based and policy-based authorization for controlling access to API endpoints.
- LDAP (Active Directory) feature for authentication using Windows passwords.
- Swagger integration for easy testing of Web API.
- Entity Framework Core for handling database entities and migrations.
- Configuration settings for various features (LDAP, JWT, ConnectionStrings) in `appsettings.development.json`.

### Getting Started

1. Ensure you have installed the latest .NET 8 SDK release.

2. Clone the repository and navigate to the project root.

3. Configure the launch URLs of both projects in the `launchSettings.json` files located in the "Properties" directories of the projects.

4. Modify the `appsettings.development.json` file in the `Backend.API` project to configure the following settings:

   - `LdapSetting:Enable`: Set to `true` to enable Active Directory (LDAP) authentication, or `false` to use Identity User password during registration.
   - `LdapSetting:LdapAdminUser`: Set the admin user of LDAP.
   - `LdapSetting:LdapAdminPassword`: Set the admin password of LDAP.
   - `LdapSetting:LdapPath`: Set the LDAP host.
   - `LdapSetting:LdapDomain`: Set the LDAP domain.
   - `ConnectionStrings:IdentityDB`: Set the SQL Server Connection string for the database.
   - `JWTSettings:Issuer`: Set the issuer of JWT tokens.
   - `JWTSettings:Secret`: Set the Secret Key for generating JWT tokens.
   - `JWTSettings:JWTExpirationTime`: Set the expiration time (in minutes) of the Access Token.
   - `JWTSettings:RefreshExpirationTime`: Set the expiration time (in minutes) of the Refresh Token.

5. Execute the migration.sql script to generate the Users database and initialize data. Additionally, you can generate scripts from migration files using the `dotnet ef` command.

6. Start both projects simultaneously. You can use the multiple startup projects feature of Visual Studio 2022.

7. Use the provided credentials to log in to the Frontend.Blazor login page:
   - For "admin" role: Use email `milad.ashrafi@gmail.com` with the password `Mil@d1234`.
   - For "user" role: Use email `ashrafi.milad@gmail.com` with the password `Mil@d1234`.

## Frontend.Blazor

### Overview

Frontend.Blazor is a .NET 8 Blazor Server project that serves as the front-end for the application. It utilizes JWT tokens for authentication and authorization, obtaining a Refresh-Token automatically from the Backend.API if the Access-Token expires.

### Features

- Authentication and authorization using JWT tokens (Access-Token and Refresh-Token).
- Usage of `Authorize` attributes on pages and menus to check roles or policies for the current user.
- A login page that requests Access-Token and Refresh-Token from Backend.API.
- Usage of Typed HttpClient to send requests to the Backend.API.
- Configuration settings for JWT in `appsettings.development.json` and `appsettings.json`.

### Getting Started

1. Ensure you have installed the latest .NET 8 SDK release.

2. Clone the repository and navigate to the project root.

3. Modify the `appsettings.development.json` and `appsettings.json` files in the `Frontend.Blazor` project to configure the following settings:

   - `JWTSettings:ValidIssuer`: Set the valid issuer of JWT tokens to validate tokens.
   - `JWTSettings:Secret`: Set the Secret Key for generating JWT tokens.
   - `Urls:BackendAPI`: Set the URL of the Backend.API.

4. Start both projects simultaneously. You can use the multiple startup projects feature of Visual Studio 2022.

5. The Frontend.Blazor login page will handle the authentication process. Users can log in with the provided credentials for the "admin" or "user" roles.

## Contributions

Contributions to this project are welcome! If you find any issues, have suggestions, or want to add new features, feel free to submit a pull request or open an issue on GitHub.

## License

This project is licensed under the [MIT License](LICENSE).


---


Happy coding!

###### [Milad Ashrafi](mailto:milad.ashrafi@gmail.com)