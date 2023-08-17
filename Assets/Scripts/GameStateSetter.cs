using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateSetter : MonoBehaviour
{
    struct GridData
    {
        public bool northExit;
        public bool southExit;
        public bool eastExit;
        public bool westExit;
        public bool containsItem;
        public bool containsGoal;
    }

    GridData[,] grid = new GridData[3, 3];
    Vector2Int coordinates = new Vector2Int(0,0);

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j].northExit = false;
                grid[i, j].southExit = false;
                grid[i, j].eastExit = false;
                grid[i, j].westExit = false;
                grid[i, j].containsItem = false;
                grid[i, j].containsGoal = false;
            }
        }

        grid[0, 0].eastExit = true;
        grid[0, 0].southExit = true;
        grid[0, 1].westExit = true;
        grid[0, 1].southExit = true;
        grid[1, 0].northExit = true;
        grid[1, 0].eastExit = true;
        grid[1, 1].northExit = true;
        grid[1, 1].westExit = true;
        grid[1, 1].southExit = true;
        grid[1, 1].containsItem = true;
        grid[2, 1].northExit = true;
        grid[2, 1].westExit = true;
        grid[2, 1].eastExit = true;
        grid[2, 0].eastExit = true;
        grid[2, 2].westExit = true;
        grid[2, 2].containsGoal = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetCurrentLocation()
    {
        string description = "*Area " + coordinates.x + "." + coordinates.y + ": ";

        if(grid[coordinates.x, coordinates.y].containsItem)
        {
            description += "Important item is here. ";
        }
        if(grid[coordinates.x, coordinates.y].containsGoal)
        {
            description += "Game goal is here. ";
        }

        description += "Exits available are";
        bool previousExit = false;
        if(grid[coordinates.x, coordinates.y].northExit)
        {
            description += " north";
            previousExit = true;
        }
        if (grid[coordinates.x, coordinates.y].southExit)
        {
            if(previousExit)
            {
                description += " and";
            }
            description += " south";
            previousExit = true;
        }
        if (grid[coordinates.x, coordinates.y].westExit)
        {
            if (previousExit)
            {
                description += " and";
            }
            description += " west";
            previousExit = true;
        }
        if (grid[coordinates.x, coordinates.y].eastExit)
        {
            if (previousExit)
            {
                description += " and";
            }
            description += " east";
        }

        description += "*";
        return description;
    }

    public string MovePlayer(string direction)
    {
        if(direction.ToLower().Contains("north"))
        {
            if(!grid[coordinates.x, coordinates.y].northExit)
            {
                return "*player cannot go that direction*";
            }
            else
            {
                coordinates.x -= 1;
            }
        }
        else if(direction.ToLower().Contains("south"))
        {
            if (!grid[coordinates.x, coordinates.y].southExit)
            {
                return "*player cannot go that direction*";
            }
            else
            {
                coordinates.x += 1;
            }
        }
        else if (direction.ToLower().Contains("west"))
        {
            if (!grid[coordinates.x, coordinates.y].westExit)
            {
                return "*player cannot go that direction*";
            }
            else
            {
                coordinates.y -= 1;
            }
        }
        else if (direction.ToLower().Contains("east"))
        {
            if (!grid[coordinates.x, coordinates.y].eastExit)
            {
                return "*player cannot go that direction*";
            }
            else
            {
                coordinates.y += 1;
            }
        }
        else
        {
            // TODO: something bad happened
        }
        return GetCurrentLocation();
    }
}
