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
    [GColumn("NAME")] public string Name;
    
    // This will be mapped by Enemy->Loot column using Loot's Index field
    [GColumn("LOOT")] public Loot Loot; 
}
```

```c#
[GTable("LOOT")]
public class Loot {
    [GColumn("NAME"), GIndex] public string Name;

    // GConverter used for defining field-level converters. It also can be defined on Rarity Class
    [GColumn("RARITY"), GConverter(typeof(RarityConverter))] public Rarity Rarity;
}

public class RarityConverter : IConverter<Rarity> {
    Rarity Convert(string value) {
        return new Rarity(value);
    }
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
