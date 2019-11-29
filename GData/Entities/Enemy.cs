using GData.Attribute;

namespace GData.Entities {
    [GTable("ENEMIES")]
    public class Enemy {
        [GColumn("NAME")] public string[] Name;
    }
}