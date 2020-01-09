# GData
This is google sheets to class mapper

```c#
public class GameDataAsset {
    [GTable("ENEMIES")] // this can be ommited if GTable attribute is present of Enemy class
    public Enemy[] enemies;
}
```

```c#
[GTable("ENEMIES")]
public class Enemy {
    [GColumn("NAME")] public string[] Name;
}
```

```c#
DataSource ds = new DataSource(
    "...", // api key (create here: https://console.developers.google.com/apis/credentials)
    "..."  // spreadsheet id
);
EntityLoader loader = new EntityLoader(ds);
GameDataAsset asset = loader.Load(typeof(GameDataAsset)); // Recursively loads fields 
List<Enemy> enemies = loader.Load(typeof(Enemy)); // Loads all entities by class

GameDataAsset instance = new GameDataAsset();
loader.LoadToInstance(instance); // Loading to existing instance
```
