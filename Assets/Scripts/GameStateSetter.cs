using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateSetter : MonoBehaviour
{
    public struct GridData
    {
        public bool northExit;
        public bool southExit;
        public bool eastExit;
        public bool westExit;
        public bool containsItem;
        public bool containsGoal;
        public int hasMonster;
        public int monsterHealth;
        public int monsterBlocks;
    }

    GridData[,] grid = new GridData[10, 10];
    Vector2Int coordinates = new Vector2Int(0,0);

    public int playerHealth = 10; // TODO: don't make public
    public bool hasImportantItem = false;

    // Start is called before the first frame update
    void Start()
    {
        ClearGrid();

        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ClearGrid()
    {
        // clear it all
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
                grid[i, j].hasMonster = 0;
                grid[i, j].monsterHealth = 0;
                grid[i, j].monsterBlocks = 0;
            }
        }
    }

    void GenerateGrid()
    {
        // manual grid for testing
        grid[0, 0].eastExit = true;
        grid[0, 0].southExit = true;
        grid[0, 1].westExit = true;
        grid[0, 1].southExit = true;
        grid[0, 1].hasMonster = 1;
        grid[0, 1].monsterHealth = 5;
        grid[0, 1].monsterBlocks = 2;
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

    public string FightMonster()
    {
        if(grid[coordinates.x, coordinates.y].hasMonster > 0)
        {
            // TODO fight based on level of monster and level of weapon
            grid[coordinates.x, coordinates.y].monsterHealth -= 3;
            if(grid[coordinates.x, coordinates.y].monsterHealth <= 0)
            {
                grid[coordinates.x, coordinates.y].hasMonster = 0;
                return "*monster was defeated*";
            }
            else
            {
                // TODO logic on monster attack
                playerHealth -= 1;
                return "*Monster damaged for 3 and has 2 health remaning. Player damaged for 1 and has 9 heatlh remaining.*";
            }
        }
        else
        {
            return "*no monster to fight here*";
        }
    }

    public string SubdueMonster()
    {
        ref GridData currLoc = ref grid[coordinates.x, coordinates.y];
        if (currLoc.hasMonster > 0)
        {
            if(currLoc.monsterBlocks > 0)
            {
                currLoc.monsterBlocks = 0;
                return "*monster no longer blocks path*";
            }
            else
            {
                return "*monster was not blocking path*";
            }
        }
        else
        {
            return "*no monster here*";
        }
    }

    public string GetCurrentLocation()
    {
        GridData currentLoc = GetCurrentLocationData();

        string description = "*Area " + coordinates.x + "." + coordinates.y + ": ";

        if(currentLoc.containsItem)
        {
            description += "Important item is here. ";
        }
        if(currentLoc.containsGoal)
        {
            description += "Game goal is here. ";
        }

        if(currentLoc.hasMonster > 0)
        {
            description += "Level " + currentLoc.hasMonster + " monster is here";
            if(currentLoc.monsterBlocks > 0 && currentLoc.monsterBlocks <= 4)
            {
                description += " and blocks the ";
                switch(currentLoc.monsterBlocks)
                {
                    case 1:
                        description += "north";
                        break;
                    case 2:
                        description += "south";
                        break;
                    case 3:
                        description += "east";
                        break;
                    case 4:
                        description += "west";
                        break;
                }

                description += " exit";
            }

            description += ". ";
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
        if (grid[coordinates.x, coordinates.y].eastExit)
        {
            if (previousExit)
            {
                description += " and";
            }
            description += " east";
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

        description += "*";
        return description;
    }

    public GridData GetCurrentLocationData()
    {
        return grid[coordinates.x, coordinates.y];
    }

    public string MovePlayer(string direction)
    {
        GridData currentLoc = GetCurrentLocationData();
        
        // monster could be blocking movement
        if(currentLoc.monsterBlocks > 0 && currentLoc.hasMonster > 0 && currentLoc.monsterHealth > 0)
        {

        }
        if (direction.ToLower().Contains("north"))
        {
            if(!currentLoc.northExit)
            {
                return "*player cannot go that direction*";
            }
            // if monster blocks movement
            else if(currentLoc.monsterBlocks == 1 && currentLoc.hasMonster > 0 && currentLoc.monsterHealth > 0)
            {
                return "*the monster blocks that direction*";
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
            // if monster blocks movement
            else if (currentLoc.monsterBlocks == 2 && currentLoc.hasMonster > 0 && currentLoc.monsterHealth > 0)
            {
                return "*the monster blocks that direction*";
            }
            else
            {
                coordinates.x += 1;
            }
        }
        else if (direction.ToLower().Contains("east"))
        {
            if (!grid[coordinates.x, coordinates.y].eastExit)
            {
                return "*player cannot go that direction*";
            }
            // if monster blocks movement
            else if (currentLoc.monsterBlocks == 3 && currentLoc.hasMonster > 0 && currentLoc.monsterHealth > 0)
            {
                return "*the monster blocks that direction*";
            }
            else
            {
                coordinates.y += 1;
            }
        }
        else if (direction.ToLower().Contains("west"))
        {
            if (!grid[coordinates.x, coordinates.y].westExit)
            {
                return "*player cannot go that direction*";
            }
            // if monster blocks movement
            else if (currentLoc.monsterBlocks == 4 && currentLoc.hasMonster > 0 && currentLoc.monsterHealth > 0)
            {
                return "*the monster blocks that direction*";
            }
            else
            {
                coordinates.y -= 1;
            }
        }
        else
        {
            // TODO: something bad happened
        }
        return GetCurrentLocation();
    }
}
