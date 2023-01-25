# YABA: Yet Another Bookmark App

## Developer Guides

### Running Migrations
When running migrations, .NET seems to be ignoring dependency injection settings. In order to get around this, be sure to add the connection string to the command like. For example, when adding the migration:

```
    dotnet ef migrations add InitialMigration -p YABA.Data -s YABA.API --context YABABaseContext -- {CONNECTION_STRING_HERE}
```

When removing last migration:
```
    dotnet ef migrations remove InitialMigration -p YABA.Data -s YABA.API --context YABABaseContext -- {CONNECTION_STRING_HERE}
```

When applying migrations:
```
    dotnet ef database update -p YABA.Data -s YABA.API -c YABABaseContext -- { CONNECTION_STRING_HERE }
```

As per the documentation [on MSDN](https://learn.microsoft.com/en-ca/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#from-application-services):
> The -- token directs dotnet ef to treat everything that follows as an argument and not try to parse them as options. Any extra arguments not used by dotnet ef are forwarded to the app.