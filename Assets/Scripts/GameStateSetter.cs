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
        public bool active;
    }

    GridData[,] grid = new GridData[10, 10];
    Vector2Int coordinates = new Vector2Int(0,0);

    Vector2Int startLocation;

    public int playerHealth = 10; // TODO: don't make public
    public bool hasImportantItem = false;

    // Start is called before the first frame update
    void Start()
    {
        ClearGrid();

        //GenerateManualGrid();

        GenerateRandomGrid();
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
                grid[i, j].active = false;
            }
        }
    }

    void GenerateManualGrid()
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

    struct RandomWalker
    {
        public int dir;
        public Vector2Int pos;
    }
    List<RandomWalker> walkers;

    int maxIterations = 10;

    float chanceWalkerChangeDir = 0.5f;
    float chanceWalkerSpawn = 0.5f;
    float chanceWalkerDestoy = 0.01f;
    int maxWalkers = 10;
    float percentToFill = 0.09f;

    void GenerateRandomGrid()
    {
        startLocation = new Vector2Int(grid.GetLength(0) / 2, grid.GetLength(1) / 2); // start in middle

        walkers = new List<RandomWalker>();

        RandomWalker firstWalker = new RandomWalker();
        RandomWalker secondWalker = new RandomWalker();

        firstWalker.pos = new Vector2Int(startLocation.x, startLocation.y);
        firstWalker.dir = Random.Range(0, 4);

        secondWalker.pos = new Vector2Int(startLocation.x, startLocation.y);
        secondWalker.dir = Random.Range(0, 4);

        walkers.Add(firstWalker);
        walkers.Add(secondWalker);

        grid[startLocation.x, startLocation.y].active = true;

        int iterations = 0;
        do
        {
            // chance destroy walker
            int numberChecks = walkers.Count;
            for (int i = 0; i < numberChecks; i++)
            {
                if (Random.value < chanceWalkerDestoy && walkers.Count > 1)
                {
                    walkers.RemoveAt(i);
                    break;
                }
            }

            //chance: Walker pick new direction
            for (int i = 0; i < walkers.Count; i++)
            {
                if (Random.value < chanceWalkerChangeDir)
                {
                    RandomWalker thisWalker = walkers[i];
                    thisWalker.dir = Random.Range(0, 4);
                    walkers[i] = thisWalker;
                }
            }

            //chance: spawn new Walker
            numberChecks = walkers.Count;
            for (int i = 0; i < numberChecks; i++)
            {
                if (Random.value < chanceWalkerSpawn && walkers.Count < maxWalkers)
                {
                    RandomWalker walker = new RandomWalker();
                    walker.dir = Random.Range(0, 4);
                    walker.pos = new Vector2Int(walkers[i].pos.x, walkers[i].pos.y);
                    walkers.Add(walker);
                }
            }

            //move Walkers
            for (int i = 0; i < walkers.Count; i++)
            {
                RandomWalker walker = walkers[i];

                switch(walker.dir)
                {
                    case 0: // north
                        // check check first if it can move this direction
                        if(walker.pos.y != 0)
                        {
                            // make path to north
                            grid[walker.pos.x, walker.pos.y].northExit = true;

                            // move north, make sure room is listed as active, and make path back south
                            walker.pos.y -= 1;
                            grid[walker.pos.x, walker.pos.y].active = true;
                            grid[walker.pos.x, walker.pos.y].southExit = true;
                        }
                        break;
                    case 1: // south
                        // check check first if it can move this direction
                        if (walker.pos.y != grid.GetLength(1)-1)
                        {
                            // make path to south
                            grid[walker.pos.x, walker.pos.y].southExit = true;

                            // move north, make sure room is listed as active, and make path back north
                            walker.pos.y += 1;
                            grid[walker.pos.x, walker.pos.y].active = true;
                            grid[walker.pos.x, walker.pos.y].northExit = true;
                        }
                        break;
                    case 2: // west
                        // check check first if it can move this direction
                        if (walker.pos.x != 0)
                        {
                            // make path to west
                            grid[walker.pos.x, walker.pos.y].westExit = true;

                            // move west, make sure room is listed as active, and make path back east
                            walker.pos.x -= 1;
                            grid[walker.pos.x, walker.pos.y].active = true;
                            grid[walker.pos.x, walker.pos.y].eastExit = true;
                        }
                        break;
                    case 3: // east
                        // check check first if it can move this direction
                        if (walker.pos.x != grid.GetLength(0) - 1)
                        {
                            // make path to east
                            grid[walker.pos.x, walker.pos.y].eastExit = true;

                            // move east, make sure room is listed as active, and make path back west
                            walker.pos.x += 1;
                            grid[walker.pos.x, walker.pos.y].active = true;
                            grid[walker.pos.x, walker.pos.y].westExit = true;
                        }
                        break;
                }

                walkers[i] = walker;
            }

            //check to exit loop
            int numberOfRooms = 0;
        
            // count rooms
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if(grid[i,j].active)
                    {
                        numberOfRooms++;
                    }
                }
            }

            if ((float)numberOfRooms / (float)grid.Length >= percentToFill)
            {
                Debug.Log("Rooms are " + numberOfRooms);
                break;
            }

            iterations++;
        } while (iterations < maxIterations);

        Debug.Log("iterations " + iterations);

        coordinates.x = startLocation.x;
        coordinates.y = startLocation.y;
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

    public string getItem()
    {
        ref GridData currLocR = ref GetCurrentloactionDataRef();
        //GetCurrentloactionDataRef();
        if (currLocR.containsItem)
        {
            hasImportantItem = true;
            currLocR.containsItem = false;
            return "*player obtains item*";
        }
        else
        {
            return "*item is not at this location*";
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
            previousExit = true;
        }
        if (grid[coordinates.x, coordinates.y].westExit)
        {
            if (previousExit)
            {
                description += " and";
            }
            description += " west";
        }

        description += "*";
        return description;
    }

    public GridData GetCurrentLocationData()
    {
        return grid[coordinates.x, coordinates.y];
    }

    private ref GridData GetCurrentloactionDataRef()
    {
        return ref grid[coordinates.x, coordinates.y];
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
                coordinates.y -= 1;
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
                coordinates.y += 1;
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
                coordinates.x += 1;
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
                coordinates.x -= 1;
            }
        }
        else
        {
            // TODO: something bad happened
        }
        return GetCurrentLocation();
    }
}
