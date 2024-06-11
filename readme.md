## Get started

```powershell
docker run --publish [port]:5432 -e POSTGRES_PASSWORD [password] postgres:11
```
**N.B.** Replace `[port]` with the port you want to expose (5432 is fine). Replace `[password]` with your desired password.

In the appsettings.json, set the Marten connection string to:
```
Host=localhost;Port=[port];User Id=postgres;Password=[password]
```
Again, replace `[port]` and `[password]` as above.

Run the application.