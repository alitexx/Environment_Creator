using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using ObjectField = UnityEditor.UIElements.ObjectField;

public class WorldBuilder : EditorWindow
{
    //teleporter gameobject
    public GameObject teleporterFade;

    //where we store the sprites we create. should be one room (or the world itself), players can change this using the load command
    public GameObject parentOBJ;
    
    private GameObject tileReferenceSize;

    // Changing Camera Color

    private static Camera camera;

    private Color bgColor = Color.white;

    //Determining if there's a sprite background in the camera

    private GameObject cameraBGsprite;

    //tilePlacement script

    private ObjectField objectField;

    tilePlacement tilePlacement = new tilePlacement();

    [SerializeField] private int m_SelectedIndex = -1;
    private VisualElement m_RightPane;

    private GameObject m_PlacedObjectPrefab; // The prefab to place

    private PopupField<string> itemDropdown; // Dropdown for items in folder
    private string spawnedItem = "Default";
    private Transform teleportDestination;
    private GameObject collectiblesMenu;
    private GameObject TomeUI;

    private bool allowPlacementBool = true;

    // Tile Category Variables

    private string tileName = "";
    private bool canCollide = false;
    private TileCategory tileCategory = TileCategory.Default;
    private int tileLayer = 0;


    private IntegerField intFieldX;
    private IntegerField intFieldY;

    [MenuItem("Window/Environment Creator/World Builder")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor.
        EditorWindow wnd = GetWindow<WorldBuilder>();
        wnd.titleContent = new GUIContent("WorldBuilder");

        // Limit size of the window.
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
    }

    public void CreateGUI()
    {
        //
        //
        //          Create Dropdown
        //
        //

        var root = rootVisualElement;

        // Create a label for the dropdown-like control
        var label = new Label("\n<b>  Select the parent object: </b>");
        root.Add(label);

        // Create a draggable object field
        objectField = new ObjectField("Parent Object") { objectType = typeof(GameObject) };
        objectField.value = parentOBJ;

        // If the tag doesn't exist, create it
        if (!TagHelper.TagExists("Editing"))
        {
            TagHelper.AddTag("Editing", parentOBJ);
        } else
        {
            parentOBJ.tag = "Editing";
        }
        tilePlacement.parentTransform = parentOBJ.transform;
        objectField.RegisterValueChangedCallback(evt =>
        {
            //User may have deleted the parent object, check if it exists first
            if (parentOBJ != null)
            {
                parentOBJ.tag = "Untagged";
            }
            parentOBJ = evt.newValue as GameObject;
            parentOBJ.tag = "Editing";
            tilePlacement.parentTransform = parentOBJ.transform;
        });
        root.Add(objectField);

        // Create a draggable object field
        var objectField2 = new ObjectField("Reference Tile") { objectType = typeof(GameObject) };
        tileReferenceSize = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/EnvironmentCreator/Prefabs/ReferenceTile.prefab");
        if (!tileReferenceSize)
        {
            Debug.LogError("Reference tile not found! Please make sure there is an image in the reference tile, and the reference tile is located at [Assets/EnvironmentCreator/Prefabs/ReferenceTile.prefab].");
        }
        objectField2.value = tileReferenceSize;
        tilePlacement.placementBounds = tileReferenceSize.GetComponent<SpriteRenderer>().bounds;
        tilePlacement.xDimension = tilePlacement.placementBounds.size.x / 2;
        tilePlacement.yDimension = tilePlacement.placementBounds.size.y / 2;

        //placement bounds are the entire scene
        tilePlacement.placementBounds = new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));

        objectField2.RegisterValueChangedCallback(evt =>
        {
            tileReferenceSize = evt.newValue as GameObject;
            tilePlacement.placementBounds = tileReferenceSize.GetComponent<SpriteRenderer>().bounds;
            tilePlacement.xDimension = tilePlacement.placementBounds.size.x/2;
            tilePlacement.yDimension = tilePlacement.placementBounds.size.y/2;

            //placement bounds are the entire scene
            //tilePlacement.placementBounds = new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));

        });
        root.Add(objectField2);


        var colorField = new ColorField("Background Color")
        {
            value = bgColor // Set initial color
        };
        colorField.RegisterValueChangedCallback(evt =>
        {
            bgColor = evt.newValue; // Update the selected color
            camera.backgroundColor = bgColor;
        });
        root.Add(colorField);



        // Create a new ObjectField for selecting a Sprite
        var backgroundField = new ObjectField("Background Sprite")
        {
            objectType = typeof(Sprite),
            allowSceneObjects = false
        };

        // Register a callback for when the sprite is changed
        backgroundField.RegisterValueChangedCallback(evt =>
        {
            SetupBackground(evt.newValue as Sprite);
        });

        // Add the ObjectField to the root of the editor window
        rootVisualElement.Add(backgroundField);


        // Create a toggle for boolean input
        var allowPlacement = new Toggle("Placing Tiles")
        {
            value = allowPlacementBool
        };
        allowPlacement.RegisterValueChangedCallback(evt =>
        {
            allowPlacementBool = evt.newValue; // Update the boolean value
        });
        root.Add(allowPlacement);

        //
        //
        //          Display Image Choices
        //
        //


        var label3 = new Label("\n <b> Choose a tile:</b>");
        root.Add(label3);

        string folderPath = "Assets/EnvironmentCreator/Images/Tiles"; // Set your folder path here
        string[] searchFilters = { "t:Sprite" }; // Search only for sprites

        // Get a list of all sprites in the specified folder.
        var allObjectGuids = AssetDatabase.FindAssets(searchFilters[0], new string[] { folderPath });
        var allObjects = new List<Sprite>();
        foreach (var guid in allObjectGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            allObjects.Add(AssetDatabase.LoadAssetAtPath<Sprite>(assetPath));
        }

        // Create a two-pane view with the left pane being fixed.
        var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);

        // Add the panel to the visual tree by adding it as a child to the root element.
        rootVisualElement.Add(splitView);

        // A TwoPaneSplitView always needs two child elements.
        var leftPane = new ListView();
        splitView.Add(leftPane);

        m_RightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
        splitView.Add(m_RightPane);

        // Initialize the list view with all sprites' names.
        leftPane.makeItem = () => new Label();
        leftPane.bindItem = (item, index) => { (item as Label).text = allObjects[index].name; };
        leftPane.itemsSource = allObjects;

        // React to the user's selection.
        leftPane.selectionChanged += OnSpriteSelectionChange;

        // Restore the selection index from before the hot reload.
        leftPane.selectedIndex = m_SelectedIndex;

        // Store the selection index when the selection changes.
        leftPane.selectionChanged += (items) => { m_SelectedIndex = leftPane.selectedIndex; };




        // Tile Category Section

        var label2 = new Label("\n <b> Tile Properties</b>");
        root.Add(label2);

        // Create a text field for string input
        var textField = new TextField("Tile Name")
        {
            value = tileName // Set initial value
        };
        textField.RegisterValueChangedCallback(evt =>
        {
            tileName = evt.newValue; // Update the string value
        });

        // Add the text field to the root visual element
        rootVisualElement.Add(textField);

        // Create a toggle for boolean input
        var boolField = new Toggle("Can Collide")
        {
            value = canCollide // Set initial value
        };
        boolField.RegisterValueChangedCallback(evt =>
        {
            canCollide = evt.newValue; // Update the boolean value
        });
        root.Add(boolField);

        // Create an integer field for integer input
        var intField = new IntegerField("Order in Layer")
        {
            value = tileLayer // Set initial value
        };
        intField.RegisterValueChangedCallback(evt =>
        {
            tileLayer = evt.newValue; // Update the integer value
        });
        root.Add(intField);





        // Create a new ObjectField for selecting a Teleport To location
        var teleportTo = new ObjectField("Teleport Destination")
        {
            objectType = typeof(Transform)
        };

        // Register a callback for when the sprite is changed
        teleportTo.RegisterValueChangedCallback(evt =>
        {
            teleportDestination = evt.newValue as Transform;
        });

        // Create an enum field for enum input
        var enumField = new EnumField("Tile Category", tileCategory);
        enumField.RegisterValueChangedCallback(evt =>
        {
            tileCategory = (TileCategory)evt.newValue; // Update the enum value
            if (tileCategory == TileCategory.Default)
            {
                if (root.Contains(itemDropdown))
                {
                    root.Remove(itemDropdown);
                }
                if (root.Contains(teleportTo))
                {
                    root.Remove(teleportTo);
                }
            }
            else if (tileCategory == TileCategory.TeleportSpawner)
            {
                if (root.Contains(itemDropdown))
                {
                    root.Remove(itemDropdown);
                }
                if (!root.Contains(teleportTo))
                {
                    root.Add(teleportTo);
                }
            }
            else
            {
                if (root.Contains(teleportTo))
                {
                    root.Remove(teleportTo);
                }
                if (!root.Contains(itemDropdown))
                {
                    root.Add(itemDropdown);
                }
            }
            UpdateItemDropdown();
        });
        root.Add(enumField);


        // The specific item from a tile category. 
        itemDropdown = new PopupField<string>("Tile Category Item", new List<string>(), 0);
        itemDropdown.RegisterValueChangedCallback(evt =>
        {
            spawnedItem = evt.newValue as string;
        });
        

        // Initialize the item dropdown
        UpdateItemDropdown();


    }

    private void UpdateItemDropdown()
    {
        // Get items from the folder based on the enum value
        var items = GetItemsFromFolder(tileCategory);

        // Update the dropdown choices
        itemDropdown.choices = items;

        // Set the initial value if there are any items
        if (items.Count > 0)
        {
            itemDropdown.value = items[0];
        }
    }

    private List<string> GetItemsFromFolder(TileCategory tileCategory)
    {
        // Determine the folder path based on the enum value
        string folderPath = "";
        switch (tileCategory)
        {
            case TileCategory.Default:
                folderPath = "Assets/EnvironmentCreator/Prefabs/Misc/DONOTCHANGE";
                break;
            case TileCategory.NPCSpawner:
                folderPath = "Assets/EnvironmentCreator/Prefabs/NPC";
                break;
            case TileCategory.CollectibleSpawner:
                folderPath = "Assets/EnvironmentCreator/Prefabs/Collectible";
                break;
            case TileCategory.TomeSpawner:
                folderPath = "Assets/EnvironmentCreator/Prefabs/Tome";
                break;
            case TileCategory.PopUpSpawner:
                folderPath = "Assets/EnvironmentCreator/Prefabs/PopUp";
                break;
            case TileCategory.TeleportSpawner:
                folderPath = "Assets/EnvironmentCreator/Prefabs/Teleport";
                break;
            default:
                folderPath = "Assets/EnvironmentCreator/Prefabs/Misc/DONOTCHANGE";
                break;
        }

        // Get all files in the folder
        var guids = AssetDatabase.FindAssets("", new[] { folderPath });
        var items = new List<string>();
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);
            items.Add(fileName);
        }

        return items;
    }

    private void OnSpriteSelectionChange(IEnumerable<object> selectedItems)
    {
        // Clear all previous content from the pane.
        m_RightPane.Clear();

        var enumerator = selectedItems.GetEnumerator();

        if (enumerator.MoveNext())
        {
            var selectedSprite = enumerator.Current as Sprite;
            if (selectedSprite != null)
            {
                // Add a new Image control and display the sprite.
                var spriteImage = new Image();
                spriteImage.scaleMode = ScaleMode.ScaleToFit;
                spriteImage.sprite = selectedSprite;

                // Add the Image control to the right-hand pane.
                m_RightPane.Add(spriteImage);

                m_RightPane.contentContainer.Add(intFieldX);
                m_RightPane.contentContainer.Add(intFieldY);
                // Register click event to place object
                m_PlacedObjectPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/EnvironmentCreator/Prefabs/Tiles/" + selectedSprite.name + ".prefab");
                if (m_PlacedObjectPrefab == null)
                {
                    //Debug.LogWarning("There is no prefab for the sprite [" + selectedSprite.name + "]. Create a prefab in EnvironmentCreator/Prefabs with the name [" + selectedSprite.name + "] to use this sprite.");
                    
                    //create a prefab
                    //add image to prefab
                    //change how image is centered on prefab
                    //add tileCategory.cs

                    GameObject newPrefab = new GameObject("DefaultEntity");
                    newPrefab.AddComponent<SpriteRenderer>().sprite = selectedSprite;
                    newPrefab.AddComponent<tileCategory>();
                    PrefabUtility.SaveAsPrefabAsset(newPrefab, "Assets/EnvironmentCreator/Prefabs/Tiles/" + selectedSprite.name + ".prefab");
                    DestroyImmediate(newPrefab);
                    m_PlacedObjectPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/EnvironmentCreator/Prefabs/Tiles/" + selectedSprite.name + ".prefab");

                }
            }
        }
    }

    private void OnSceneGUI(SceneView sceneView) {
        // Check if we have a prefab to place and left mouse button is clicked
        // UPDATE! Now checks if we are allowed to place tiles
        if (m_PlacedObjectPrefab != null && Event.current.type == EventType.MouseDown && Event.current.button == 0 && allowPlacementBool)
        {
            // Check if there is a parent object, if there isn't then make a new one
            if (parentOBJ == null)
            {
                Debug.LogWarning("The parent object has been deleted or moved! Created new parent object named [Temp World (Game Object)].");
                parentOBJ = new GameObject("Temp World (Game Object)");
                objectField.value = parentOBJ;
                parentOBJ.tag = "Editing";
                tilePlacement.parentTransform = parentOBJ.transform;
            }

            // Calculate the position in the scene based on mouse click
            Vector3 position = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;

            // Check the validity of the position
            Vector3 validPosition = tilePlacement.CheckTileValidity(position, m_PlacedObjectPrefab.name);
            if (validPosition.x != 0.3939f)
            {
                GameObject tileBelow = tilePlacement.isPlacingOnOccupiedSpace(validPosition, m_PlacedObjectPrefab.name);

                //if there is a tile below it AND it is being placed on the same layer, delete it.
                if (tileBelow != null && tileBelow.GetComponent<SpriteRenderer>().sortingOrder == tileLayer)
                {
                    DestroyImmediate(tileBelow);
                }

                // Instantiate the object at the valid position
                GameObject instance = Instantiate(m_PlacedObjectPrefab, validPosition, Quaternion.identity);
                Undo.RegisterCreatedObjectUndo(instance, "Place Object");

                // Set the parent of the newly created object to the parent object
                instance.transform.parent = parentOBJ.transform;
                if (tileName != "")
                {
                    instance.name = tileName;
                } else
                {
                    instance.name = m_PlacedObjectPrefab.name;
                }
                instance.GetComponent<SpriteRenderer>().sortingOrder = tileLayer;
                // Check if the instance has the tile category script, if it does, change the tile category and can collide.
                if(instance.GetComponent<tileCategory>() != null)
                {
                    // If the tag doesn't exist, create it
                    if (!TagHelper.TagExists(tileCategory.ToString().Replace("Spawner", "")))
                    {
                        Debug.Log("No tag for the tile category [" + tileCategory.ToString().Replace("Spawner", "") + "] was found. Created new tag for this category; this message should not appear again for this tile category.");
                        TagHelper.AddTag(tileCategory.ToString().Replace("Spawner", ""));
                    }

                    //Teleport spawner is special, so it doesn't have values placed or an object spawned on it.
                    if (tileCategory == TileCategory.TeleportSpawner)
                    {

                        //add the fade if it isnt already in the scene
                        if (!teleporterFade)
                        {
                            // Instantiate the object at the valid position
                            teleporterFade = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/EnvironmentCreator/Prefabs/Teleport/DefaultTeleporter.prefab"), Vector3.zero, Quaternion.identity);
                            teleporterFade.tag = "TeleporterFade";
                        }
                        PolygonCollider2D collider2D = ComponentHelper.GetOrAddComponent<PolygonCollider2D>(instance);
                        collider2D.isTrigger = true;
                        Teleport teleportScript = instance.AddComponent<Teleport>();
                        teleportScript.targetLocation = teleportDestination;
                        Debug.Log("Tile with teleport tile category updated! Please check it's PolygonCollider2D component to make sure it fits your liking, you can also swap it for a BoxCollider2D for more personalized collisions.");
                        return;
                    }
                    else if (tileCategory == TileCategory.CollectibleSpawner)
                    {
                        //find prefab for collectible menu and put it in
                        //If there is not a game object with the collectibles menu tag, then there is not a collectible UI. add one.
                        if (collectiblesMenu == null)
                        {
                            collectiblesMenu = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/EnvironmentCreator/Prefabs/UI/Collectibles Menu/Collectible UI.prefab"), Vector3.zero, Quaternion.identity);
                            collectiblesMenu.tag = "CollectiblesMenu";
                            // Check if there is already an EventSystem in the scene
                            EventSystem eventSystem = FindObjectOfType<EventSystem>();
                            // If not, create a new EventSystem
                            if (eventSystem == null)
                            {
                                GameObject eventSystemObject = new GameObject("EventSystem");
                                eventSystemObject.AddComponent<EventSystem>();
                                eventSystemObject.AddComponent<StandaloneInputModule>();
                            }
                        }
                    }
                    else if (tileCategory == TileCategory.TomeSpawner)
                    {
                        //Similar to CollectibleUI, but add a UI each time a tome is spawned
                        TomeUI = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/EnvironmentCreator/Prefabs/UI/Tome UI.prefab"), Vector3.zero, Quaternion.identity);
                        TomeUI.tag = "UI";
                        TomeUI.name = spawnedItem + "_UI";
                        // Check if there is already an EventSystem in the scene
                        EventSystem eventSystem = FindObjectOfType<EventSystem>();
                        // If not, create a new EventSystem
                        if (eventSystem == null)
                        {
                            GameObject eventSystemObject = new GameObject("EventSystem");
                            eventSystemObject.AddComponent<EventSystem>();
                            eventSystemObject.AddComponent<StandaloneInputModule>();
                        }
                    }
                    instance.GetComponent<tileCategory>().SetValuesWhenPlaced(tileCategory, canCollide, spawnedItem);
                } else
                {
                    Debug.LogWarning("The prefab for the image [" + instance.name + "] does not have the [tileCategory.cs] script attached. Please attach [tileCategory.cs] to the prefab [" + instance.name + ".prefab]");
                }

            }
        }
    }


    private void OnEnable()
    {
        //Debug.Log("I am on!");
        tileReferenceSize = tilePlacement.tileReferenceSize;
        SceneView.duringSceneGui += OnSceneGUI;

        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        // Find the background child
        try
        {
            cameraBGsprite = camera.transform.Find("Background").gameObject;
        }
        catch
        {
            cameraBGsprite = null;
        }

        // create a game object to be the parent of the objects created
        parentOBJ = new GameObject("New World (Game Object)");

        // If the tag doesn't exist, create it
        if (!TagHelper.TagExists("Editing"))
        {
            TagHelper.AddTag("Editing", parentOBJ);
        }
        else
        {
            parentOBJ.tag = "Editing";
        }

        // This is for Tomes, I have to define it somewhere but there's not really a good place for it.
        if (!TagHelper.TagExists("UI"))
        {
            TagHelper.AddTag("UI", parentOBJ);
        }

        //Check if there's a Collectibles Menu object in the scene
        if (!TagHelper.TagExists("CollectiblesMenu"))
        {
            TagHelper.AddTag("CollectiblesMenu");
        } else
        {
            collectiblesMenu = GameObject.FindGameObjectWithTag("CollectiblesMenu");
        }


        //Check if there's a teleporter fade object in the scene
        if (!TagHelper.TagExists("TeleporterFade"))
        {
            TagHelper.AddTag("TeleporterFade");
        }
        else
        {
            teleporterFade = GameObject.FindGameObjectWithTag("TeleporterFade");
        }
    }

    private void OnDisable()
    {
        //Debug.Log("I am off!");
        tilePlacement.tileReferenceSize = tileReferenceSize;
        SceneView.duringSceneGui -= OnSceneGUI;

        // Check if the parent object has any children
        if (parentOBJ != null && parentOBJ.transform.childCount == 0)
        {
            // If it doesn't have any children, destroy the parent object
            DestroyImmediate(parentOBJ);
            parentOBJ.tag = null;
            parentOBJ = null;
        }
    }


    void SetupBackground(Sprite backgroundImage)
    {
        if (cameraBGsprite)
        {
            DestroyImmediate(cameraBGsprite);
        }
        // Create a new GameObject for the background
        GameObject background = new GameObject("Background");

        // Add a Sprite Renderer component
        SpriteRenderer renderer = ComponentHelper.GetOrAddComponent<SpriteRenderer>(background);
        renderer.sprite = backgroundImage;

        // Set the sorting layer to a background layer and the order to a low value
        renderer.sortingLayerName = "Background";
        renderer.sortingOrder = -10;

        // Parent the background to the camera
        background.transform.parent = Camera.main.transform;

        // Adjust the size to fit the camera view
        float cameraHeight = Camera.main.orthographicSize * 2;
        float cameraWidth = cameraHeight * Camera.main.aspect;

        background.transform.localScale = new Vector3(cameraWidth / renderer.sprite.bounds.size.x, cameraHeight / renderer.sprite.bounds.size.y, 1);

        // Position it at the camera's position
        background.transform.localPosition = new Vector3(0, 0, 10); // Keep it behind the camera view
        cameraBGsprite = background;

        //Check if there's a player. If there is, get or set the camera movement script and assign the player
        GameObject player = GameObject.FindGameObjectWithTag("PlayerCharacter");
        if (player)
        {
            Camera.main.gameObject.GetOrAddComponent<CameraMovement>().player = player.transform;
        }
    }
}
