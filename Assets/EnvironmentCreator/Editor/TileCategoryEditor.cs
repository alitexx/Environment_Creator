using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class TileCategoryEditor : EditorWindow
{
    private string categoryName = "NewCategory";
    private string folderName = "NewCategoryFolder";

    private string[] userDefinedTileCategories; // For deletion

    private TileCategory selectedCategoryToDelete;


    // As of right now, this script does not check if another asset exists with the same name. This should be changed.

    [MenuItem("Window/Environment Creator/Tile Category Editor")]
    public static void ShowWindow()
    {
        GetWindow<TileCategoryEditor>("Tile Category Editor");
        tileCategoryManagement.checkIfEmpty();
    }

    private void OnGUI()
    {
        // When UI is opened
        GUILayout.Label("Create a New Tile Category", EditorStyles.boldLabel);

        categoryName = EditorGUILayout.TextField("Category Name", categoryName);
        folderName = EditorGUILayout.TextField("Folder Name", folderName);

        if (GUILayout.Button("Create Tile Category"))
        {
            if (categoryName.Any(char.IsDigit) || folderName.Any(char.IsDigit))
            {
                Debug.LogError("Category Name and Folder Name cannot have numbers.");
            }
            else
            {
                CreateTileCategory();
            }
        }

        GUILayout.Space(20);

        // Deletion UI
        GUILayout.Label("Delete an Existing Tile Category", EditorStyles.boldLabel);

        if (userDefinedTileCategories == null || userDefinedTileCategories.Length == 0)
        {
            UpdateUserDefinedTileCategories();
        }

        selectedCategoryToDelete = (TileCategory)EditorGUILayout.EnumPopup("Tile Category", selectedCategoryToDelete);

        if (GUILayout.Button("Delete Tile Category"))
        {
            if (selectedCategoryToDelete == TileCategory.Default)
            {
                Debug.LogError("Cannot delete the default category.");
            }
            else if (EditorUtility.DisplayDialog("Confirm Deletion", "Are you sure you want to delete the tile category [" + selectedCategoryToDelete.ToString() + "]?", "Yes", "No"))
            {
                DeleteTileCategory(selectedCategoryToDelete.ToString().Replace("Spawner", ""));
            }
        }
    }

    private void CreateTileCategory()
    {
        // Step 1: Check if this is in tileCategoryManagement
        var checkIfCreated = tileCategoryManagement.GetValue(categoryName);
        if (checkIfCreated != null) // If this isn't null, then that means this category has been created. Halt everything.
        {
            Debug.LogError("There is already a tile category named [" + categoryName + "]. Please delete the existing [" + categoryName + "] tile category or choose a new name.");
            return;
        }

        // Step 2: Add to tileCategoryManagement.cs
        tileCategoryManagement.SetValue(categoryName, folderName);

        // Step 3: Create C# Script
        string scriptPath = $"Assets/EnvironmentCreator/Scripts/SpawningItems/{categoryName}.cs";
        // The content of the new script (Nothing)
        string scriptContent = $"using UnityEngine;\n\npublic class {categoryName} : MonoBehaviour\n{{\n    // Your code here\n}}";
        File.WriteAllText(scriptPath, scriptContent);
        AssetDatabase.Refresh();

        // Step 4: Create Folder
        string folderPath = $"Assets/EnvironmentCreator/Prefabs/{folderName}";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/EnvironmentCreator/Prefabs", folderName);
        }

        // Step 5: Create Default Entity
        GameObject defaultEntity = new GameObject("DefaultEntity");
        defaultEntity.AddComponent(System.Type.GetType(categoryName));
        defaultEntity.AddComponent<FolderPlacement>();
        try
        {
            defaultEntity.tag = categoryName; // This is so other scripts can access who is the parent object
        }
        catch
        {
            TagHelper.AddTag(categoryName);
            defaultEntity.tag = categoryName;
        }
        PrefabUtility.SaveAsPrefabAsset(defaultEntity, $"{folderPath}/DefaultEntity.prefab");
        DestroyImmediate(defaultEntity);

        // Step 6: Update TileCategory Enum
        UpdateTileCategoryEnum(categoryName);

        // Step 7: Update TileCategory Script
        UpdateTileCategoryScript(categoryName);

        // Step 8: Update WorldBuilder Script
        UpdateWorldBuilderScript(categoryName, folderName);

        // Step 9: Update Spawner Script
        UpdateSpawnerScript(categoryName, folderName);

        AssetDatabase.Refresh();
    }

    private void UpdateTileCategoryEnum(string categoryName)
    {
        string enumPath = "Assets/EnvironmentCreator/Scripts/TileCategory.cs";
        string enumContent = File.ReadAllText(enumPath);
        string newEnumValue = $"{categoryName}Spawner";

        if (!enumContent.Contains(newEnumValue))
        {
            int insertIndex = enumContent.IndexOf('}');
            // Should find the FIRST }
            enumContent = enumContent.Insert(insertIndex - 1, $",\n    //{categoryName} spawn on these tiles (USER DEFINED TILE CATEGORY)\n    {newEnumValue}");
            File.WriteAllText(enumPath, enumContent);
        }
    }

    private void UpdateTileCategoryScript(string categoryName)
    {
        string scriptPath = "Assets/EnvironmentCreator/Scripts/TileCategory.cs";
        string scriptContent = File.ReadAllText(scriptPath);

        // Add necessary using statements if not present
        if (!scriptContent.Contains("using System;"))
        {
            scriptContent = "using System;\n" + scriptContent;
        }

        // Add a new field for the spawner
        string spawnerField = $"    private Spawner {categoryName.ToLower()}Spawner = new {categoryName}Spawner();";
        if (!scriptContent.Contains(spawnerField))
        {
            int insertIndex = scriptContent.IndexOf("    //User defined spawners above");
            scriptContent = scriptContent.Insert(insertIndex, spawnerField + "\n\n");
        }

        // Add a new case to the switch statement
        string caseStatement = $"case TileCategory.{categoryName}Spawner:\n                {categoryName.ToLower()}Spawner.Spawn(objSpawnName, transform.position);\n                break;\n            ";
        if (!scriptContent.Contains(caseStatement))
        {
            int switchIndex = scriptContent.LastIndexOf("default:");
            scriptContent = scriptContent.Insert(switchIndex, caseStatement);
        }

        File.WriteAllText(scriptPath, scriptContent);
    }

    private void UpdateWorldBuilderScript(string categoryName, string folderName)
    {
        string scriptPath = "Assets/EnvironmentCreator/Editor/WorldBuilder.cs";
        string scriptContent = File.ReadAllText(scriptPath);

        string caseStatement = $"case TileCategory.{categoryName}Spawner:\n                folderPath = \"Assets/EnvironmentCreator/Prefabs/{folderName}\";\n                break;\n            ";
        if (!scriptContent.Contains(caseStatement))
        {
            int switchIndex = scriptContent.LastIndexOf("default:");
            scriptContent = scriptContent.Insert(switchIndex, caseStatement);
        }

        File.WriteAllText(scriptPath, scriptContent);
    }

    private void UpdateSpawnerScript(string categoryName, string folderName)
    {
        string scriptPath = "Assets/EnvironmentCreator/Scripts/SpawningItems/Spawner.cs";
        string scriptContent = File.ReadAllText(scriptPath);

        string newClass = $"public class {categoryName}Spawner : Spawner\r\n{{\r\n    public {categoryName}Spawner() : base(\"{folderName}\") {{ }}\r\n\r\n    protected override string GetDefaultPrefab()\r\n    {{\r\n        return \"DefaultEntity\";\r\n    }}\r\n}}";
        if (!scriptContent.Contains(newClass))
        {
            int switchIndex = scriptContent.LastIndexOf("//User defined Spawners go here:");
            scriptContent = scriptContent.Insert(switchIndex, newClass);
        }

        File.WriteAllText(scriptPath, scriptContent);
    }

    private void DeleteTileCategory(string categoryName)
    {
        // Step 1: Find folder associated with category
        var folderName = tileCategoryManagement.GetValue(categoryName);
        if (folderName == null)
        {
            Debug.LogError("Cannot find TileCategory named [" + categoryName + "] in dictionary.");
            return;
        }

        // Step 2: Delete C# Script
        string scriptPath = $"Assets/EnvironmentCreator/Scripts/SpawningItems/{categoryName}.cs";
        if (File.Exists(scriptPath))
        {
            File.Delete(scriptPath);
        }

        // Step 3: Delete Folder
        string folderPath = $"Assets/EnvironmentCreator/Prefabs/{folderName}";
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            FileUtil.DeleteFileOrDirectory(folderPath);
        }

        // Step 4: Remove from TileCategory Enum
        RemoveFromTileCategoryEnum(categoryName+"Spawner");

        // Step 5: Update TileCategory Script
        RemoveFromTileCategoryScript(categoryName);

        // Step 6: Update WorldBuilder Script
        RemoveFromWorldBuilderScript(categoryName, folderName);

        // Step 7: Update Spawner Script
        RemoveFromSpawnerScript(categoryName, folderName);

        // Step 8: Remove this tile category's spot in the dictionary
        tileCategoryManagement.DeleteValue(categoryName);

        AssetDatabase.Refresh();
        UpdateUserDefinedTileCategories();
    }

    private void RemoveFromTileCategoryEnum(string categoryName)
    {
        string enumPath = "Assets/EnvironmentCreator/Scripts/TileCategory.cs";
        string enumContent = File.ReadAllText(enumPath);
        string enumValue = $"{categoryName}";

        if (enumContent.Contains(enumValue))
        {
            enumContent = enumContent.Replace($",\n    //{categoryName} spawn on these tiles (USER DEFINED TILE CATEGORY)\n    {enumValue}", "");
            File.WriteAllText(enumPath, enumContent);
        }
    }

    private void RemoveFromTileCategoryScript(string categoryName)
    {
        string scriptPath = "Assets/EnvironmentCreator/Scripts/TileCategory.cs";
        string scriptContent = File.ReadAllText(scriptPath);

        // Remove field
        string spawnerField = $"    private Spawner {categoryName.ToLower()}Spawner = new {categoryName}Spawner();";
        if (scriptContent.Contains(spawnerField))
        {
            scriptContent = scriptContent.Replace(spawnerField + "\n\n", "");
        }

        // Remove case from switch statement
        string caseStatement = $"case TileCategory.{categoryName}Spawner:\n                {categoryName.ToLower()}Spawner.Spawn(objSpawnName, transform.position);\n                break;\n            ";
        if (scriptContent.Contains(caseStatement))
        {
            scriptContent = scriptContent.Replace(caseStatement, "");
        }

        File.WriteAllText(scriptPath, scriptContent);
    }

    private void RemoveFromWorldBuilderScript(string categoryName, string folderName)
    {
        string scriptPath = "Assets/EnvironmentCreator/Editor/WorldBuilder.cs";
        string scriptContent = File.ReadAllText(scriptPath);

        string caseStatement = $"case TileCategory.{categoryName}Spawner:\n                folderPath = \"Assets/EnvironmentCreator/Prefabs/{folderName}\";\n                break;\n            ";
        if (scriptContent.Contains(caseStatement))
        {
            scriptContent = scriptContent.Replace(caseStatement, "");
        }

        File.WriteAllText(scriptPath, scriptContent);
    }

    private void RemoveFromSpawnerScript(string categoryName, string folderName)
    {
        string scriptPath = "Assets/EnvironmentCreator/Scripts/SpawningItems/Spawner.cs";
        string scriptContent = File.ReadAllText(scriptPath);
        string newClass = $"public class {categoryName}Spawner : Spawner\r\n{{\r\n    public {categoryName}Spawner() : base(\"{folderName}\") {{ }}\r\n\r\n    protected override string GetDefaultPrefab()\r\n    {{\r\n        return \"DefaultEntity\";\r\n    }}\r\n}}";
        if (scriptContent.Contains(newClass))
        {
            scriptContent = scriptContent.Replace(newClass, "");
        }

        File.WriteAllText(scriptPath, scriptContent);
    }

    private void UpdateUserDefinedTileCategories()
    {
        var enumValues = System.Enum.GetValues(typeof(TileCategory));
        userDefinedTileCategories = enumValues.OfType<TileCategory>()
                                              .Select(e => e.ToString())
                                              .Where(e => e.EndsWith("Spawner"))
                                              .ToArray();
    }
}

