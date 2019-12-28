using UnityEditor;
using UnityEngine;

namespace GData.Unity
{
    public abstract class GameDataAsset : ScriptableObject
    {
        [InspectorName("SpreadsheetId")]
        public string GDataSpreadsheetId;
        [InspectorName("ApiKey"), Tooltip("Create on here: https://console.developers.google.com/apis/credentials")]
        public string GDataApiKey;
        
        [ContextMenu("Reload from GData!")]
        public virtual void LoadFromGData()
        {
            if (GDataApiKey == "" || GDataSpreadsheetId == "") {
                Debug.LogError("Fill SpreadsheetId and ApiKey fields to load");
            }
            
            Debug.Log("[GameData] Loaded!");
            DataSource ds = new DataSource(GDataApiKey, GDataSpreadsheetId);
            EntityLoader loader = new EntityLoader(ds);
            loader.LoadToInstance(this);
            Debug.Log("[GameData] Loaded!");
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}