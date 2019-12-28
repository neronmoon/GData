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

Unity example
```c#
public class GameDataAsset : ScriptableObject
    {
        [GData.Attribute.GTable("Rooms")]
        public Room[] Levels;
        
        [GData.Attribute.GTable("Trees")]
        public Seed[] Seeds;
        
        [GData.Attribute.GTable("Leveling")]
        public Leveling[] Leveling;
        
        public string GDataSpreadsheetId;
        public string GDataApiKey;
        
        [ContextMenu("Reload from GData!")]
        public void Reload()
        {
            Debug.Log("[GameData] Loading!");
            DataSource ds = new DataSource(GDataApiKey, GDataSpreadsheetId);
            EntityLoader loader = new EntityLoader(ds);
            loader.LoadToInstance(this);
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
```