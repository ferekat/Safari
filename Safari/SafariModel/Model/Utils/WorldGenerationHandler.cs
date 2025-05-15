using SafariModel.Model.InstanceEntity;
using SafariModel.Model.Tiles;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Channels;
using static System.Net.Mime.MediaTypeNames;

namespace SafariModel.Model.Utils
{

    public class WorldGenerationHandler
    {
        private Random random;
        private List<Tile> apexTiles = new();
        private List<Tile> riverSources = new();
        private EntityHandler entityHandler;

        private static int MIN_TERRAIN_DIFF = 100;  //100
        private static int MAX_TERRAIN_DIFF = 200; //200
        private static int MAX_APEX_HEIGHT = 600; //600

        private static int CHANCE_INC_PER_ITER = 50;   //jól be van állítva
        private static int MOUNTAIN_APEX_GEN_CHANCE = 2400; //jól be van állítva
        private static int RIVER_SOURCE_GEN_CHANCE = 2500; //jól be van állítva
        private static int GATE_GEN_CHANCE = 2000; //legyen nagy

        private TileMap tileMap;
       
        public WorldGenerationHandler(string seed, EntityHandler entityHandler)
        {
            this.entityHandler = entityHandler;
            random = new Random(ConvertSeed(seed));
            Tile[,] map = new Tile[TileMap.MAPSIZE, TileMap.MAPSIZE];
            for (int i = 0; i < TileMap.MAPSIZE; i++)
            {
                for (int j = 0; j < TileMap.MAPSIZE; j++)
                {
                    map[i, j] = new Tile(i, j, 0, TileType.GROUND);
                }
            }
            tileMap = new TileMap(map);
        }

        private int ConvertSeed(string seed)
        {
            int ret = 0;
            for (int i = 1; i < seed.Length; i++)
            {
                ret += i * (int)seed[i];
            }
            return ret;
        }


        public TileMap GenerateRandomMapFromSeed()
        {
            GenerateMountains();
            SmoothTerrain();
            GenerateRivers();
            SmoothRivers();
            GenerateBoundaries();
            ConnectGates();
            StartingEntities();
            return tileMap;
        }
        private void StartingEntities()
        {

        }
        private void ConnectGates()
        {

        }
        private bool FreeForGate(Tile tile)
        {
            bool validGatePos = false;

            foreach (Tile neigh in tileMap.GetNeighbourTiles(tile))
            {
                if (neigh.TileType == TileType.GROUND)
                {
                    validGatePos = true;
                }
            }
            return validGatePos;
        }

        private PathTile? GeneratedGateOnFence(List<Tile> fence,TileType gateType)
        {
            int chance = 0;
            for (int attempts = 0; attempts < 10; attempts++)
            {
                chance += random.Next(GATE_GEN_CHANCE);
                int fenceIdx = (chance % TileMap.MAPSIZE - 11);
                if (fenceIdx < 10)
                {
                    fenceIdx += 10;
                }
                Tile fenceTile = fence[fenceIdx];
                if (FreeForGate(fenceTile))
                {
                    fenceTile.SetType(gateType);
                    return new PathTile(fenceTile, PathTileType.ROAD);
                }
            }
            return null;
        }
        private void GenerateBoundaries()
        {
            List<Tile> topFence = new List<Tile>();
            List<Tile> leftFence = new List<Tile>();
            List<Tile> bottomFence = new List<Tile>();
            List<Tile> rightFence = new List<Tile>();
            for (int i = 0; i < TileMap.MAPSIZE - 1; i++)
            {
                Tile t1 = tileMap.Map[i, 0];
                Tile t2 = tileMap.Map[i, TileMap.MAPSIZE - 1];
                topFence.Add(t1);
                bottomFence.Add(t2);
                t1.SetType(TileType.FENCE);
                t2.SetType(TileType.FENCE);
            }
            for (int j = 0; j < TileMap.MAPSIZE - 1; j++)
            {
                Tile t1 = tileMap.Map[0, j];
                Tile t2 = tileMap.Map[TileMap.MAPSIZE - 1, j];
                leftFence.Add(t1);
                rightFence.Add(t2);
                t1.SetType(TileType.FENCE);
                t2.SetType(TileType.FENCE);
            }
            tileMap.Map[0, 0].SetType(TileType.FENCE);
            tileMap.Map[TileMap.MAPSIZE - 1, TileMap.MAPSIZE - 1].SetType(TileType.FENCE);
            tileMap.Map[TileMap.MAPSIZE - 1, 0].SetType(TileType.FENCE);
            tileMap.Map[0, TileMap.MAPSIZE - 1].SetType(TileType.FENCE);


            PathTile? entrance = null;
            PathTile? exit = null;

            //TESZTELÉSRE:
            entrance = new PathTile(tileMap.Map[0, 8], PathTileType.ROAD);
            exit = new PathTile(tileMap.Map[10, 0], PathTileType.ROAD);
            /////////////
            ///

            //while (entrance == null || exit == null)
            //{
            //    int isHorizontal = random.Next(2);
            //    if (isHorizontal == 0)
            //    {
            //        entrance = GeneratedGateOnFence(leftFence, TileType.ENTRANCE);
            //        exit = GeneratedGateOnFence(rightFence, TileType.EXIT);
            //    }
            //    else
            //    {
            //        entrance = GeneratedGateOnFence(topFence, TileType.ENTRANCE);
            //        exit = GeneratedGateOnFence(bottomFence, TileType.EXIT);
            //    }
            //}


            tileMap.Entrance = entrance; 
            tileMap.Exit = exit;
          
        }
       
        private void GenerateMountains()
        {
            foreach (Tile[,] chunk in tileMap.Chunks)
            {
                int chunkChance = 0;
                foreach (Tile tile in chunk)
                {
                    chunkChance += random.Next(CHANCE_INC_PER_ITER);
                    if (chunkChance > MOUNTAIN_APEX_GEN_CHANCE && tile.TileType != TileType.FENCE && tile.TileType != TileType.ENTRANCE && tile.TileType != TileType.EXIT)
                    {
                        apexTiles.Add(tile);
                        break;
                    }
                }
            }
            foreach (Tile apex in apexTiles)
            {
                BranchTerrain(apex, BlockedForMountain, MountainSpines, 1);
            }
        }
        private void GenerateRivers()
        {
            foreach (Tile[,] chunk in tileMap.Chunks)
            {
                int chunkChance = 0;
                foreach (Tile tile in chunk)
                {
                    chunkChance += random.Next(CHANCE_INC_PER_ITER);
                    if (chunkChance > RIVER_SOURCE_GEN_CHANCE && tile.TileType == TileType.GROUND)
                    {
                        riverSources.Add(tile);
                        break;
                    }
                }
            }
            foreach (Tile source in riverSources)
            {
                BranchTerrain(source, BlockedForRiver, CarveRivers, 2);

            }
        }
        private void SmoothRivers()
        {
            int wetTiles = 0;  //ha 3nál több víz veszi körül
            bool areWetTiles = wetTiles > 0;
            do
            {
                wetTiles = 0;
                foreach (Tile tile in tileMap.Map)
                {
                    int waterTileCount = tileMap.GetNeighbourTiles(tile).Count((t) => (t.IsWater()));
                    if (waterTileCount == 0)
                    {
                        tile.H = tile.H;
                        continue;
                    }
                    if (waterTileCount >= 3)
                    {
                        wetTiles++;
                        tile.SetType(TileType.SHALLOW_WATER);
                    }
                }
            }
            while (areWetTiles);


            foreach (Tile tile in tileMap.Map)
            {
                bool deepWater = true;
                List<Tile> farNeighours = new List<Tile>(); //szomszédok szomszédai
                foreach (Tile neigh in tileMap.GetNeighbourTiles(tile))
                {
                    farNeighours = farNeighours.Concat(tileMap.GetNeighbourTiles(neigh)).ToList();
                }
                foreach (Tile farNeigh in farNeighours)
                {
                    if (!farNeigh.IsWater())
                    {
                        deepWater = false;
                        break;
                    }
                }
                if (deepWater)
                {
                    tile.SetType(TileType.DEEP_WATER);
                }
            }
            foreach (Tile tile in tileMap.Map)
            {
                if (tile.TileType == TileType.DEEP_WATER && tileMap.GetNeighbourTiles(tile).Count((t) => (t.TileType == TileType.DEEP_WATER)) == 0)
                {
                    tile.SetType(TileType.SHALLOW_WATER);
                }
            }
        }
        private void SmoothTerrain()
        {
            int weight = 1;
            for (int i = 0; i < TileMap.MAPSIZE; i++)
            {
                for (int j = 0; j < TileMap.MAPSIZE; j++)
                {
                    Tile curr = tileMap.Map[i, j];
                    if (i == 0)
                    {
                        curr.H = tileMap.Map[i + 1, j].H / 2;
                    }
                    else if (i == TileMap.MAPSIZE - 1)
                    {
                        curr.H = tileMap.Map[i - 1, j].H / 2;
                    }
                    else
                    {
                        int prevH = tileMap.Map[i - 1, j].H;
                        int nextH = tileMap.Map[i + 1, j].H;
                        curr.H = (prevH + curr.H * weight * weight + nextH) / (3 * weight);
                    }

                }
            }

            for (int i = 0; i < TileMap.MAPSIZE; i++)
            {
                for (int j = 0; j < TileMap.MAPSIZE; j++)
                {
                    Tile curr = tileMap.Map[i, j];
                    if (j == 0)
                    {
                        curr.H = tileMap.Map[i, j + 1].H / 2;
                    }
                    else if (j == TileMap.MAPSIZE - 1)
                    {
                        curr.H = tileMap.Map[i, j - 1].H / 2;
                    }
                    else
                    {
                        int prevH = tileMap.Map[i, j - 1].H;
                        int nextH = tileMap.Map[i, j + 1].H;
                        curr.H = (prevH + curr.H * weight * weight + nextH) / (3 * weight);
                    }
                }
            }
        }
        private void CarveRivers(Tile startingPoint)
        {
            List<Tile> neighs = tileMap.GetNeighbourTiles(startingPoint);

            foreach (Tile n in neighs)
            {
                if (n.TileType == TileType.GROUND)
                {
                    n.SetType(TileType.SHALLOW_WATER);
                }

            }
        }
        private void MountainSpines(Tile apex)
        {

            apex.H = random.Next(MAX_APEX_HEIGHT);


            Spine(apex, 30, 1, 0);
            Spine(apex, 30, 0, 1);
        }
        private void Spine(Tile apexTile, int maxOffset, int dx, int dy)
        {
            int[] diffs = new int[4];

            for (int i = 0; i < diffs.Length; i++)
            {
                diffs[i] = random.Next(MIN_TERRAIN_DIFF, MAX_TERRAIN_DIFF);
            }
            for (int dir = 0; dir < 4; dir++)
            {
                int dirI = dir < 2 ? 1 : -1;
                int dirJ = dir == 0 || dir == 2 ? 1 : -1;
                Tile curr = apexTile;

                for (int offset = 1; offset < maxOffset; offset++)
                {
                    int x = curr.I + dx * dirI;
                    int y = curr.J + dy * dirJ;

                    if (!TileMap.IsTileCoordInBounds(x, y))
                        break;

                    if (offset % 3 == 0)
                    {

                        Spine(curr, 3, dx, dy);
                        Spine(curr, 3, dy, dx);
                    }
                    Tile next = tileMap.Map[x, y];
                    int nextH;
                    if (next.H < curr.H)
                    {
                        nextH = curr.H - diffs[dir];
                    }
                    else
                    {
                        break;
                    }


                    if (nextH < 0)
                    {
                        next.H = 0;
                        break;
                    }

                    if (next.H <= nextH)
                        next.H = nextH;

                    curr = next;
                }
            }
        }
        private bool BlockedForRiver(Tile tile)
        {
            return tile.TileType != TileType.GROUND || tile.TileType != TileType.GROUND_SMALL;
        }
        private bool BlockedForMountain(Tile tile)
        {
            return tile.TileType != TileType.GROUND;
        }
        private void BranchTerrain(Tile startingTile, Predicate<Tile> blockingCondition, Action<Tile> actionPerTile, int maxAttempts)
        {
            for (int attempts = 0; attempts < maxAttempts; attempts++)
            {
                bool isSpaceFree = true;

                int randNextI = random.Next(-5, 5);
                int randNextJ = random.Next(-5, 5);

                int stepI = randNextI > 0 ? 1 : -1;
                int stepJ = randNextJ > 0 ? 1 : -1;
                if (!TileMap.IsTileCoordInBounds(startingTile.I + randNextI, startingTile.J + randNextJ)) return;
                Tile branchTile = tileMap.Map[startingTile.I + randNextI, startingTile.J + randNextJ];


                for (int i = startingTile.I; i < branchTile.I; i += stepI)
                {
                    for (int j = startingTile.J; j < branchTile.J; j += stepJ)
                    {
                        if (blockingCondition(tileMap.Map[i, j]))
                        {
                            isSpaceFree = false;
                            break;
                        }
                    }
                }
                if (isSpaceFree)
                {
                    BresenhamLine(startingTile, branchTile, actionPerTile);
                    BranchTerrain(branchTile, blockingCondition, actionPerTile, maxAttempts);
                    break;
                }
            }
        }

        private void BresenhamLine(Tile tile0, Tile tile1, Action<Tile> actionPerTile)
        {
            int i0 = tile0.I;
            int j0 = tile0.J;
            int i1 = tile1.I;
            int j1 = tile1.J;


            int deltaI = Math.Abs(i1 - i0);
            int deltaJ = Math.Abs(j1 - j0);

            int stepI = i0 < i1 ? 1 : -1;
            int stepJ = j0 < j1 ? 1 : -1;

            int err = deltaI - deltaJ;

            while (true)
            {
                Tile curr = tileMap.Map[i0, j0];
                actionPerTile(curr);
                if (i0 == i1 && j0 == j1) break;

                int e2 = 2 * err;

                if (e2 > -deltaJ)
                {
                    err -= deltaJ;
                    i0 += stepI;
                }

                if (e2 < deltaI)
                {
                    err += deltaI;
                    j0 += stepJ;
                }
            }
        }


    }
}
