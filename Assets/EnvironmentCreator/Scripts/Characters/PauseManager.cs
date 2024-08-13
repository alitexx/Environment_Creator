using UnityEngine;

public static class PauseManager
{
    public static void SetNPCPauseState(bool isPaused)
    {
        NPCMovement[] npcs = GameObject.FindObjectsOfType<NPCMovement>();
        foreach (NPCMovement npc in npcs)
        {
            npc.isPaused = isPaused;
        }
    }

    public static void SetPlayerFreezeState(bool freeze)
    {
        GameObject player = GameObject.FindGameObjectWithTag("PlayerCharacter");
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.Freeze(freeze);
            }
        }
    }

    public static void FreezeAllMovingObject(bool freeze)
    {
        SetPlayerFreezeState(freeze);
        SetNPCPauseState(freeze);
    }
}
