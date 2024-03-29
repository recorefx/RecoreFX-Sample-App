# App playground

This is a convenient directory for testing the app end-to-end.

Run

```bash
./setup
```

to set up the directories with test data.
If you are on Windows, you can use Git Bash to run the script.

Then in separate terminal processes run:

```bash
# in service/
dotnet ../../bin/FileSync.Service/Debug/net5.0/FileSync.Service.dll
```

```bash
# in client/
dotnet run  --project ../../src/FileSync.Client/
```

You can't use `dotnet run` for the service because `<Project Sdk="Microsoft.NET.Sdk.Web">` sets `<RunWorkingDirectory>$(MSBuildProjectDirectory)</RunWorkingDirectory>`, so it will always run in the source tree.
