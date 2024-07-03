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
    public Vector3 CheckTileValidity(Vector3 posToPlace, string tileName)
    {

        // Extract the tile size from the name (assuming format is (WxH) Name)
        string[] parts = tileName.Split(')'); //Remove everything after the closing parenthesis
        string sizePart = parts[0] + ")"; // (WxH) (Add back the ending parenthesis for trimming/consistency)
        string[] dimensions = sizePart.Trim('(', ')').Split('x');
        int tileWidth = int.Parse(dimensions[0]);
        int tileHeight = int.Parse(dimensions[1]);

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
        //Debug.Log(Mathf.Abs((xSnap / xDimension) % 2));

        //Debug.Log(Mathf.Abs((ySnap / yDimension) % 2));

        // If this tile is a square use square logic

        if (tileHeight == tileWidth)
        {
            //Debug.Log(Mathf.Abs((xSnap / xDimension) % 2));

            //Debug.Log(Mathf.Abs((ySnap / yDimension) % 2));
            // Wrong space vertically
            if (Mathf.Abs((ySnap / yDimension) % 2) >= 0.9f && Mathf.Abs((xSnap / xDimension) % 2) <= 0.05f && Mathf.Abs((ySnap / yDimension) % 2) < 1.95f)
            {
                //Debug.Log("Wrong space vertically");
                //ySnap = Mathf.Round(posToPlace.y / (yDimension * 2)) * (yDimension * 2);
                Vector3 closestTile = FindClosestTile(xSnap, ySnap, posToPlace);
                //Debug.Log("Adding " + closestTile.x + "(x) AND " + closestTile.y +"(y)");
                xSnap += closestTile.x;
                ySnap += closestTile.y;
            }
            // Wrong space horizontally
            else if (Mathf.Abs((ySnap / yDimension) % 2) <= 0.05f && Mathf.Abs((xSnap / xDimension) % 2) >= 0.9f)
            {
                //Debug.Log("Wrong space horizontally");
                //xSnap = Mathf.Round(posToPlace.x / (xDimension * 2)) * (xDimension * 2);
                Vector3 closestTile = FindClosestTile(xSnap, ySnap, posToPlace);
                //Debug.Log("Adding " + closestTile.x + "(x) AND " + closestTile.y + "(y)");
                xSnap += closestTile.x;
                ySnap += closestTile.y;
            }
            else if ((Mathf.Abs((ySnap / yDimension) % 2) >= 1.9f && Mathf.Abs((xSnap / xDimension) % 2) >= 0.9f))
            {
                //Debug.Log("Wrong space horizontally (New function)");
                //xSnap = Mathf.Round(posToPlace.x / (xDimension * 2)) * (xDimension * 2);
                Vector3 closestTile = FindClosestTile(xSnap, ySnap, posToPlace);
                //Debug.Log("Adding " + closestTile.x + "(x) AND " + closestTile.y + "(y)");
                xSnap += closestTile.x;
                ySnap += closestTile.y;
            }
            else
            {
                ySnap -= (yDimension);
            }
        }
        else
        {
            xSnap += (((tileWidth - 1) * xDimension) / tileWidth) + (xDimension * (1+(0.5f * Mathf.Pow(2, tileHeight - 2))));
            ySnap += (((tileHeight - 1) * yDimension) / tileHeight) - (yDimension * (1+((tileHeight-1)*0.5f)));
            // Adjust for tile size to center it correctly
            if (tileHeight <= 1)
            {
                xSnap += (((tileWidth) * xDimension) / 2);
            }
            else if (tileWidth <= 1)
            {
                ySnap += (((tileHeight) * yDimension) / 2);
            }


            if (tileWidth > tileHeight)
            {
                ySnap += (((tileHeight) * yDimension));
                xSnap -= (((tileWidth) * xDimension));

            }

            //Debug.Log("X = " + (Mathf.Abs((xSnap / xDimension) % (2))));

            //Debug.Log("Y = " + (Mathf.Abs((ySnap / yDimension) % (2))));

            //Debug.Log("Y Actual = " + (Mathf.Abs((ySnap / yDimension) % (2))) + " must be greater than " +(0.9f) + " and less than " + (1.1f));

            //Debug.Log("X Actual = " + (Mathf.Abs((xSnap / xDimension) % (2))) + " must be greater than " + (1.4f) + " and less than " + (1.6f));


            ////if x is around 1.5 and y is around 1
            //if (Mathf.Abs((ySnap / yDimension) % (2)) >= (0.9f) && Mathf.Abs((ySnap / yDimension) % (2)) <= (1.1f) && Mathf.Abs((xSnap / xDimension) % (2*tileWidth)) >= (1.4f*tileWidth) && Mathf.Abs((xSnap / xDimension) % (2)) <= (1.6f))
            //{
            //    Debug.Log("Wrong space: X = 1.5, Y = 1");
            //    //ySnap = Mathf.Round(posToPlace.y / (yDimension * 2)) * (yDimension * 2);
            //    //Vector3 closestTile = FindClosestTile(xSnap, ySnap, posToPlace);
            //    //Debug.Log("Adding " + closestTile.x + "(x) AND " + closestTile.y + "(y)");
            //    //xSnap += closestTile.x;
            //    //ySnap += closestTile.y;

            ////if x is around 0.5 and y is around 2
            //} else if (Mathf.Abs((ySnap / yDimension) % (2)) >= (1.9f) && Mathf.Abs((ySnap / yDimension) % (2)) <= (2.1f) && Mathf.Abs((xSnap / xDimension) % (2)) >= (0.4f) && Mathf.Abs((xSnap / xDimension) % (2)) <= (0.6f))
            //{
            //    Debug.Log("Wrong space: X = 0.5, Y = 2");
            //    //ySnap = Mathf.Round(posToPlace.y / (yDimension * 2)) * (yDimension * 2);
            //    //Vector3 closestTile = FindClosestTile(xSnap, ySnap, posToPlace);
            //    //Debug.Log("Adding " + closestTile.x + "(x) AND " + closestTile.y + "(y)");
            //    //xSnap += closestTile.x;
            //    //ySnap += closestTile.y;

            ////if x is around 0.5 and y is around 1
            //}
            //else if (Mathf.Abs((ySnap / yDimension) % (2)) >= (0.9f) && Mathf.Abs((ySnap / yDimension) % (2)) <= (1.1f) && Mathf.Abs((xSnap / xDimension) % (2)) >= (0.4f) && Mathf.Abs((xSnap / xDimension) % (2)) <= (0.6f))
            //{
            //    Debug.Log("Wrong space: X = 0.5, Y = 1");
            //    //xSnap = Mathf.Round(posToPlace.x / (xDimension * 2)) * (xDimension * 2);
            //    //Vector3 closestTile = FindClosestTile(xSnap, ySnap, posToPlace);
            //    //Debug.Log("Adding " + closestTile.x + "(x) AND " + closestTile.y + "(y)");
            //    //xSnap += closestTile.x;
            //    //ySnap += closestTile.y;
            
            ////if x is around 1.5 and y is around 2
            //}
            //else if (Mathf.Abs((ySnap / yDimension) % (2)) >= (1.9f) && Mathf.Abs((ySnap / yDimension) % (2)) <= (2.1f) && Mathf.Abs((xSnap / xDimension) % (2)) >= (1.4f) && Mathf.Abs((xSnap / xDimension) % (2)) <= (1.6f))
            //{
            //    Debug.Log("Wrong space: X = 0.5, Y = 2");
            //    //xSnap = Mathf.Round(posToPlace.x / (xDimension * 2)) * (xDimension * 2);
            //    //Vector3 closestTile = FindClosestTile(xSnap, ySnap, posToPlace);
            //    //Debug.Log("Adding " + closestTile.x + "(x) AND " + closestTile.y + "(y)");
            //    //xSnap += closestTile.x;
            //    //ySnap += closestTile.y;
            //}

            //else
            //{
            //    //ySnap -= (yDimension);
            //}

            if (posToPlace.x > 0)
            {
                xSnap -= (xDimension);
            }
            else
            {
                xSnap += (xDimension);
            }

            if (posToPlace.y > 0)
            {
                ySnap += (yDimension);
            }
            else
            {
                ySnap -= (yDimension);
            }

            if(posToPlace.x < 0 && posToPlace.y < 0)
            {
                xSnap -= (xDimension*2);
                //ySnap += (yDimension);
            }
            //ySnap -= (yDimension);
            //ySnap -= (yDimension*2);
        }


        // Return the nearest valid position
        return new Vector3(xSnap, ySnap, 0);
    }
    
    private Vector3 FindClosestTile(float xSnap, float ySnap, Vector3 cursorPos)
    {
        //x snap = where we WERE going to place the tile, x pos
        //y snap = where we WERE going to place the tile, y pos
        //cursorPos = where the cursor clicked

        //default it to negative
        Vector3 positionMovement = new Vector3();
        positionMovement.x = -(xDimension);
        positionMovement.y = -(yDimension);

        float xOffset = cursorPos.x - xSnap;
        float yOffset = cursorPos.y - ySnap;

        if(xOffset > 0)
        {
            positionMovement.x = (xDimension);
        }
        if(yOffset > 0)
        {
            positionMovement.y = (yDimension);
        }

        if (cursorPos.x < 0)
        {
            //This number is negative. Switch the symbol of the x value
            positionMovement.x = -positionMovement.x;
        }
        if (cursorPos.y < 0)
        {
            //This number is negative. Switch the symbol of the y value
            positionMovement.y = -positionMovement.y;
        }

        return positionMovement;
    }



    // this runs BEFORE the tile is placed. So, if a user wants to place a tile it doesn't check the one they're actively putting down (if that makes sense)
    public GameObject isPlacingOnOccupiedSpace(Vector3 position, string tileName)
    {

        // Extract the tile size from the name (assuming format is (WxH) Name)
        string[] parts = tileName.Split(')'); //Remove everything after the closing parenthesis
        string sizePart = parts[0] + ")"; // (WxH) (Add back the ending parenthesis for trimming/consistency)
        string[] dimensions = sizePart.Trim('(', ')').Split('x');
        int tileWidth = int.Parse(dimensions[0]);
        int tileHeight = int.Parse(dimensions[1]);

        foreach (Transform childTransform in parentTransform)
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

