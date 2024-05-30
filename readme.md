## Hashicorp Vault as Configuration Backend for ASP.NET Core
This is a sample project to demonstrate how to use Hashicorp Vault as a configuration backend for ASP.NET Core applications. The project uses the [VaultSharp.Extensions.Configuration](https://github.com/MrZoidberg/VaultSharp.Extensions.Configuration) library to load configuration from Hashicorp Vault.

### Prerequisites
- Hashicorp Vault server running locally or in a cloud environment.
- .NET Core 3.1 SDK or later.

### Steps to get vault running locally with docker
1. Pull the vault image from docker hub
```bash
docker pull vault
```
2. Run the vault server in dev mode. Obtain token from the console output.
```bash
docker run --cap-add=IPC_LOCK -e 'VAULT_DEV_ROOT_TOKEN_ID=myroot' -p 8200:8200 vault
```
3. Open the vault UI in the browser
```
http://localhost:8200
```
4. Use the token obtained to log in to the vault UI.
5. Create a new secret engine of type KV and version 2 and mount it at `MyApp` path.
6. Create a new secret with key `appsettings.json` and  the following value as a JSON object with the configuration settings.
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "ApplicationSetting": {
    "DisplayTitle": "App using vault for config storage",
    "IsLive": true
  }
}
```
7. Create a new auth method of type userpass and enable it.
8. Create a new user with a password and assign it to the `default` policy.
9. Modify the default policy to allow access to the `MyApp` path with the following policy.
```hcl
   path "MyApp/*" {
    capabilities = ["create", "read", "update", "list"]
   }
```

All good! The new user should have access to the Configurations stored in the `MyApp` path.

### Steps to run the project
1. Clone the repository
2. Update `appsettings.json` with the vault server URL and the user credentials.
3. Run the project in your favourite IDE or using the dotnet CLI.
4. Visit the URL `https://localhost:5267/view-config` to see the configuration settings loaded from Hashicorp Vault.
5. Change the configuration settings in the vault UI, wait for the configured reload seconds to elapse and refresh the URl to see the updated configuration settings.

