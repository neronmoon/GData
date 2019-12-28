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
```

### Unity usage example
- Create class for game data like this
- create instance of it in Unity
- Fill credentials fields
- hit "Reload from GData!" in context menu.
```c#
[CreateAssetMenu(menuName = "<MyGame>/GameDataAsset")]
public class GameDataAsset : GData.Unity.GameDataAsset { // This is Scriptable object with helpers
    [GData.Attribute.GTable("Rooms")]
    public Room[] Levels;
    
    [GData.Attribute.GTable("Trees")]
    public Seed[] Seeds;
    
    [GData.Attribute.GTable("Leveling")]
    public Leveling[] Leveling;
}
```

 