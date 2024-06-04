using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tilePlacement
{
    // Save previous selected template

    public GameObject tileReferenceSize;

    public Transform parentTransform;

    // Grid dimensions
    public float xDimension;
    public float yDimension;

    // Scene bounds or area where tiles can be placed
    public Bounds placementBounds;

    // Calculate the nearest valid position for placing a tile
    public Vector3 CheckTileValidity(Vector3 posToPlace)
    {
        // Ensure that the position is within the placement bounds
        if (!placementBounds.Contains(posToPlace))
        {
            Debug.LogWarning("Position is outside of placement bounds.");
            return new Vector3(0.3939f, 0, 0);
        }

        // Calculate the nearest valid position based on grid dimensions
        float xSnap = (Mathf.Round((posToPlace.x) / xDimension) * xDimension);
        float ySnap = (Mathf.Round((posToPlace.y) / yDimension) * yDimension);

        //determines if the picture is off centered or not
        // 1 means that its off centered, 0 means its not
        Debug.Log(Mathf.Abs((xSnap/xDimension)%2));

        Debug.Log(Mathf.Abs((ySnap / yDimension) % 2));

        //wrong space vertically
        if (Mathf.Abs((ySnap / yDimension) % 2) >= 0.9f && Mathf.Abs((xSnap / xDimension) % 2) == 0)
        {
            //xSnap = (Mathf.Round((posToPlace.x) / (xDimension*2)) * (xDimension*2));
            ySnap = (Mathf.Round((posToPlace.y) / (yDimension*2)) * (yDimension*2));
            //wrong space horizontally
        } else if (Mathf.Abs((ySnap / yDimension) % 2) == 0 && Mathf.Abs((xSnap / xDimension) % 2) >= 0.9f)
        {
            //this sprite is off center. Delete it! Kill it!!
            xSnap = (Mathf.Round((posToPlace.x) / (xDimension * 2)) * (xDimension * 2));
            //ySnap = (Mathf.Round((posToPlace.y) / (yDimension * 2)) * (yDimension * 2));
        }

        // Return the nearest valid position
        return new Vector3(xSnap, ySnap, 0);
    }
    
    // this runs BEFORE the tile is placed. So, if a user wants to place a tile it doesn't check the one they're actively putting down (if that makes sense)
    public GameObject isPlacingOnOccupiedSpace(Vector3 position)
    {
        foreach(Transform childTransform in parentTransform)
        {
            if (childTransform == null) continue;
            if (childTransform.position == position)
            {
                //we are placing a tile on a previously placed tile. Delete that bottom tile.
                return childTransform.gameObject;
            }
        }
        return null;
    }
}

