  ## State machine by Automatonymous
  ``` XML
  <ItemGroup>
      <PackageReference Include="Automatonymous" Version="5.1.3" />
  </ItemGroup>
  ```

  ## How to include/import a closs of other project.
   - in *Projectname*.csproj:
  ``` XML
  <ItemGroup>
    <Compile Include="../Pong/Screen.cs" />
  </ItemGroup>
  ```
   - The main file(*Program.cs*:direct statements) starts with:
  ``` csharp
  using pong;
  ```
   - Class files of other project start with:
  ``` csharp
  namespace pong;
  ```

   - Debug by Visual Studio Code:.vscode/launch.json
  ``` json
     "console": "integratedTerminal",
  ```

