using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    //The interract key, how users interract with the environment, such as picking up a collectible or talking to an NPC
    public static KeyCode interactKey = KeyCode.E;

    //When talking to NPCs, this key speeds up the text. This cannot be the same key as the interact key.
    public static KeyCode hurryUpKey = KeyCode.Return;

    //The range that users can interact with objects, see pop ups, etc.
    public static float interactRange = 5;
}
