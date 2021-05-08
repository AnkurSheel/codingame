import requests
import json 
print("go")
payload =["34351598238a5ea185e3d0c7737f05a0f5afea36",{"code":"\n\n\nusing System.Collections.Generic;\nusing System.Linq;\nusing SpringChallenge2021.Actions;\nusing SpringChallenge2021.Common.Services;\nusing SpringChallenge2021.Models;\nusing SpringChallenge2021.Agents;\nusing System;\nusing System.IO;\n\n\n // 08/05/2021 02:21\n\nnamespace SpringChallenge2021\n{\n    public class Constants\n    {\n        public const int DayCutOff = 22;\n        public const int MaxDays = 24;\n    }\n}\n\nnamespace SpringChallenge2021\n{\n    public class Game\n    {\n        private int _nutrients;\n        private readonly Player _opponentPlayer;\n\n        public List<IAction> PossibleActions { get; }\n\n        public Dictionary<int, Cell> Board { get; }\n\n        public Dictionary<int, Tree> Trees { get; }\n\n        public Player MyPlayer { get; }\n\n        public int Day { get; private set; }\n\n        public HexDirection SunDirection { get; set; }\n\n        public HashSet<Cell>? Shadows { get; }\n\n        public Game()\n        {\n            Board = new Dictionary<int, Cell>();\n            PossibleActions = new List<IAction>();\n            Trees = new Dictionary<int, Tree>();\n            MyPlayer = new Player();\n            _opponentPlayer = new Player();\n\n            GenerateBoard();\n            Shadows = new HashSet<Cell>();\n        }\n\n        public void ReInit()\n        {\n            Trees.Clear();\n            PossibleActions.Clear();\n            Shadows.Clear();\n\n            MyPlayer.ReInit();\n            _opponentPlayer.ReInit();\n\n            ReadGameState();\n\n            SunDirection = (HexDirection) (Day % 6);\n            SetupShadows();\n        }\n\n        private void ReadGameState()\n        {\n            Day = int.Parse(Io.ReadLine()); // the game lasts 24 days: 0-23\n            _nutrients = int.Parse(Io.ReadLine()); // the base score you gain from the next COMPLETE action\n\n            MyPlayer.Parse();\n            _opponentPlayer.Parse();\n\n            ReadTrees();\n            ReadPossibleActions();\n        }\n\n        private void GenerateBoard()\n        {\n            var numberOfCells = int.Parse(Io.ReadLine()); // 37\n            for (var i = 0; i < numberOfCells; i++)\n            {\n                Board.Add(i, new Cell());\n            }\n\n            for (var i = 0; i < numberOfCells; i++)\n            {\n                var inputs = Io.ReadLine().Split(' ');\n                var index = int.Parse(inputs[0]); // 0 is the center cell, the next cells spiral outwards\n                var richness = int.Parse(inputs[1]); // 0 if the cell is unusable, 1-3 for usable cells\n                var neigh0 = int.Parse(inputs[2]); // the index of the neighbouring cell for each direction\n                var neigh1 = int.Parse(inputs[3]);\n                var neigh2 = int.Parse(inputs[4]);\n                var neigh3 = int.Parse(inputs[5]);\n                var neigh4 = int.Parse(inputs[6]);\n                var neigh5 = int.Parse(inputs[7]);\n\n                var neighbourCells = new Dictionary<HexDirection, Cell?>\n                {\n                    {0, neigh0 == -1 ? null : Board[neigh0]},\n                    {(HexDirection) 1, neigh1 == -1 ? null : Board[neigh1]},\n                    {(HexDirection) 2, neigh2 == -1 ? null : Board[neigh2]},\n                    {(HexDirection) 3, neigh3 == -1 ? null : Board[neigh3]},\n                    {(HexDirection) 4, neigh4 == -1 ? null : Board[neigh4]},\n                    {(HexDirection) 5, neigh5 == -1 ? null : Board[neigh5]},\n                };\n\n                var cell = new Cell(index, (SoilQuality) richness, neighbourCells);\n                Board[index] = cell;\n            }\n        }\n\n        private void ReadTrees()\n        {\n            Io.Debug(\"Reading Trees\");\n\n            var numberOfTrees = int.Parse(Io.ReadLine()); // the current amount of trees\n            for (var i = 0; i < numberOfTrees; i++)\n            {\n                var inputs = Io.ReadLine().Split(' ');\n                var cellIndex = int.Parse(inputs[0]); // location of this tree\n                var size = int.Parse(inputs[1]); // size of this tree: 0-3\n                var isMine = inputs[2] != \"0\"; // 1 if this is your tree\n                var isDormant = inputs[3] != \"0\"; // 1 if this tree is dormant\n                var tree = new Tree(cellIndex, (TreeSize) size, isMine, isDormant);\n                Trees.Add(cellIndex, tree);\n                if (isMine)\n                {\n                    MyPlayer.AddTree(tree);\n                }\n                else\n                {\n                    _opponentPlayer.AddTree(tree);\n                }\n            }\n        }\n\n        private void ReadPossibleActions()\n        {\n            Io.Debug(\"Getting possible actions\");\n\n            var numberOfPossibleMoves = int.Parse(Io.ReadLine());\n            for (var i = 0; i < numberOfPossibleMoves; i++)\n            {\n                var possibleMove = Io.ReadLine();\n\n                var parts = possibleMove.Split(\" \");\n                IAction action = parts[0] switch\n                {\n                    \"SEED\" => new SeedAction(int.Parse(parts[1]), int.Parse(parts[2])),\n                    \"GROW\" => new GrowAction(int.Parse(parts[1])),\n                    \"COMPLETE\" => new CompleteAction(int.Parse(parts[1])),\n                    _ => new WaitAction()\n                };\n\n                PossibleActions.Add(action);\n            }\n        }\n\n        private void SetupShadows()\n        {\n            foreach (var (treeIndex, tree) in Trees)\n            {\n                var cell = Board[treeIndex];\n                Io.Debug($\"Cell {cell.Index} {cell.Neighbours.Select(x => x.Value)}\");\n                for (var i = 0; i < (int) tree.Size && cell != null; i++)\n                {\n                    var neighbour = cell.Neighbours[SunDirection];\n                    if (neighbour != null)\n                    {\n                        Shadows.Add(neighbour);\n                    }\n\n                    cell = neighbour;\n                }\n            }\n\n            Io.Debug($\"Shadows {Shadows.Count}\");\n            foreach (var shadow in Shadows)\n            {\n                Io.Debug($\"{shadow.Index}\");\n            }\n        }\n    }\n}\n\nnamespace SpringChallenge2021\n{\n    internal class Program\n    {\n        private static void Main(string[] args)\n        {\n            Io.Initialize();\n\n            var game = new Game();\n            var agent = new SimpleAgent();\n\n            // game loop\n            while (true)\n            {\n                game.ReInit();\n\n                var action = agent.GetAction(game);\n                Io.WriteLine(action.GetOutputAction());\n            }\n        }\n    }\n}\nnamespace SpringChallenge2021.Actions\n{\n    public class CompleteAction : IAction\n    {\n        public int Index { get; }\n\n        public CompleteAction(int index)\n        {\n            Index = index;\n        }\n\n        public string GetOutputAction()\n        {\n            return $\"COMPLETE {Index}\";\n        }\n    }\n}\nnamespace SpringChallenge2021.Actions\n{\n    public class GrowAction : IAction\n    {\n        public int Index { get; }\n\n        public GrowAction(int index)\n        {\n            Index = index;\n        }\n\n        public string GetOutputAction()\n        {\n            return $\"GROW {Index}\";\n        }\n    }\n}\nnamespace SpringChallenge2021.Actions\n{\n    public interface IAction\n    {\n        string GetOutputAction();\n    }\n}\nnamespace SpringChallenge2021.Actions\n{\n    public class SeedAction : IAction\n    {\n        private readonly int _srcTreeIndex;\n        public int SeedIndex { get; }\n\n        public SeedAction(int srcTreeIndex, int seedIndex)\n        {\n            _srcTreeIndex = srcTreeIndex;\n            SeedIndex = seedIndex;\n        }\n\n        public string GetOutputAction()\n        {\n            return $\"SEED {_srcTreeIndex} {SeedIndex}\";\n        }\n    }\n}\nnamespace SpringChallenge2021.Actions\n{\n    public class WaitAction : IAction\n    {\n        public string GetOutputAction()\n        {\n            return \"WAIT\";\n        }\n    }\n}\n\nnamespace SpringChallenge2021.Agents\n{\n    public class SimpleAgent\n    {\n        public IAction GetAction(Game game)\n        {\n            var completeAction = GetBestCompleteAction(game);\n            if (completeAction != null)\n            {\n                return completeAction;\n            }\n\n            if (game.Day < Constants.DayCutOff + 1)\n            {\n                var growAction = GetBestGrowAction(game);\n                if (growAction != null)\n                {\n                    return growAction;\n                }\n\n                var seedAction = GetBestSeedAction(game);\n                if (seedAction != null)\n                {\n                    return seedAction;\n                }\n            }\n\n            return new WaitAction();\n        }\n\n        private static IAction? GetBestCompleteAction(Game game)\n        {\n            if (game.MyPlayer.Trees[TreeSize.Large].Count < 3 && game.Day < Constants.DayCutOff)\n            {\n                return null;\n            }\n\n            var completeActions = game.PossibleActions.OfType<CompleteAction>().ToList();\n            var bestCompleteAction = completeActions.FirstOrDefault();\n            var bestSoilQuality = SoilQuality.Unusable;\n            foreach (var completeAction in completeActions)\n            {\n                var cellSoilQuality = game.Board[completeAction.Index].SoilQuality;\n                if (bestSoilQuality < cellSoilQuality)\n                {\n                    bestSoilQuality = cellSoilQuality;\n                    bestCompleteAction = completeAction;\n                }\n            }\n\n            return bestCompleteAction;\n        }\n\n        private static IAction? GetBestGrowAction(Game game)\n        {\n            var growActions = game.PossibleActions.OfType<GrowAction>().ToList();\n            var bestSoilQuality = SoilQuality.Unusable;\n            var bestTreeSize = TreeSize.Seed;\n            var bestGrowAction = growActions.FirstOrDefault();\n            foreach (var growAction in growActions)\n            {\n                var cellTreeSize = game.Trees[growAction.Index].Size;\n\n                if (bestTreeSize < cellTreeSize)\n                {\n                    bestTreeSize = cellTreeSize;\n                    bestGrowAction = growAction;\n                    bestSoilQuality = game.Board[growAction.Index].SoilQuality;\n                }\n                else if (bestTreeSize == cellTreeSize)\n                {\n                    var cellSoilQuality = game.Board[growAction.Index].SoilQuality;\n                    if (bestSoilQuality < cellSoilQuality)\n                    {\n                        bestSoilQuality = cellSoilQuality;\n                        bestGrowAction = growAction;\n                    }\n                }\n            }\n\n            return bestGrowAction;\n        }\n\n        private IAction? GetBestSeedAction(Game game)\n        {\n            if (game.MyPlayer.Trees[TreeSize.Seed].Any())\n            {\n                return null;\n            }\n\n            var seedActions = game.PossibleActions.OfType<SeedAction>().ToList();\n            var bestSoilQuality = SoilQuality.Unusable;\n            var bestSeedAction = seedActions.FirstOrDefault();\n            foreach (var seedAction in seedActions)\n            {\n                var cellSoilQuality = game.Board[seedAction.SeedIndex].SoilQuality;\n                if (bestSoilQuality < cellSoilQuality)\n                {\n                    bestSoilQuality = cellSoilQuality;\n                    bestSeedAction = seedAction;\n                }\n            }\n\n            return bestSeedAction;\n        }\n    }\n}\n\nnamespace SpringChallenge2021.Agents\n{\n    public class SimpleAgentV1\n    {\n        public IAction GetAction(Game game)\n        {\n            var completeAction = game.PossibleActions.FirstOrDefault(x => x is CompleteAction);\n            if (completeAction != null)\n            {\n                return completeAction;\n            }\n\n            var growAction = game.PossibleActions.FirstOrDefault(x => x is GrowAction);\n            if (growAction != null)\n            {\n                return growAction;\n            }\n\n            var seedAction = game.PossibleActions.FirstOrDefault(x => x is SeedAction);\n            if (seedAction != null)\n            {\n                return seedAction;\n            }\n\n            return new WaitAction();\n        }\n    }\n}\nnamespace SpringChallenge2021.Common\n{\n    public static class Constants\n    {\n        public const bool IsDebugOn = true;\n\n        public const bool IsForInput = true;\n\n        public const bool IsLocalRun = false;\n\n        public const bool ShowInput = false;\n    }\n}\n\nnamespace SpringChallenge2021.Models\n{\n    public class Cell\n    {\n        public int Index { get; }\n\n        public Dictionary<HexDirection, Cell?> Neighbours { get; }\n\n        public SoilQuality SoilQuality { get; }\n\n        public Cell()\n        {\n        }\n\n        public Cell(int index, SoilQuality soilQuality, Dictionary<HexDirection, Cell?> neighbours)\n        {\n            Index = index;\n            SoilQuality = soilQuality;\n            Neighbours = neighbours;\n        }\n    }\n}\nnamespace SpringChallenge2021.Models\n{\n    public enum HexDirection\n    {\n        East = 0,\n        NorthEast = 1,\n        NorthWest = 2,\n        West = 3,\n        SouthWest = 4,\n        SouthEast = 5\n    }\n}\n\nnamespace SpringChallenge2021.Models\n{\n    public class Player\n    {\n        private int _sunPoints;\n        private int _score;\n        private bool _isWaiting;\n\n        public Dictionary<TreeSize, List<Tree>> Trees { get; }\n\n        public Player()\n        {\n            Trees = new Dictionary<TreeSize, List<Tree>>\n            {\n                {TreeSize.Seed, new List<Tree>()},\n                {TreeSize.Small, new List<Tree>()},\n                {TreeSize.Medium, new List<Tree>()},\n                {TreeSize.Large, new List<Tree>()},\n            };\n        }\n\n        public void ReInit()\n        {\n            foreach (var (_, treeList) in Trees)\n            {\n                treeList.Clear();\n            }\n        }\n\n        public void Parse()\n        {\n            Io.Debug(\"Reading Player State\");\n            var inputs = Io.ReadLine().Split(' ');\n\n            _sunPoints = int.Parse(inputs[0]);\n            _score = int.Parse(inputs[1]); // your current score\n            _isWaiting = false;\n\n            if (inputs.Length == 3)\n            {\n                _isWaiting = inputs[2] != \"0\"; // whether your opponent is asleep until the next day\n            }\n        }\n\n        public void AddTree(Tree tree)\n        {\n            Trees[tree.Size].Add(tree);\n        }\n    }\n}\nnamespace SpringChallenge2021.Models\n{\n    public enum SoilQuality\n    {\n        Unusable = 0,\n        Low = 1,\n        Medium = 2,\n        High = 3\n    }\n}\nnamespace SpringChallenge2021.Models\n{\n    public class Tree\n    {\n        private bool _isMine;\n        private bool _isDormant;\n\n        public int CellIndex { get; }\n        public TreeSize Size { get; }\n\n        public Tree(int cellIndex, TreeSize size, bool isMine, bool isDormant)\n        {\n            CellIndex = cellIndex;\n            Size = size;\n            _isMine = isMine;\n            _isDormant = isDormant;\n        }\n    }\n}\nnamespace SpringChallenge2021.Models\n{\n    public enum TreeSize\n    {\n        Seed = 0,\n        Small = 1,\n        Medium = 2,\n        Large = 3\n    }\n}\n\nnamespace SpringChallenge2021.Common.Services\n{\n    public static class Io\n    {\n        private static StreamReader _file;\n\n        public static void Initialize()\n        {\n            if (Constants.IsLocalRun)\n            {\n                _file = new StreamReader(@\".\\in.txt\");\n            }\n        }\n\n        public static void Debug(string output)\n        {\n            if (Constants.IsDebugOn || Constants.IsForInput)\n            {\n                Console.Error.WriteLine(output);\n            }\n        }\n\n        public static void WriteLine(string output)\n        {\n            Console.WriteLine(output);\n        }\n\n        public static string ReadLine()\n        {\n            if (Constants.IsLocalRun)\n            {\n                return _file.ReadLine();\n            }\n            else\n            {\n                var input = Console.ReadLine();\n\n                if (Constants.IsForInput)\n                {\n                    Debug(\"IN\");\n                    Debug(input);\n                    Debug(\"/IN\");\n                }\n                else if(Constants.ShowInput)\n                {\n                    Debug(input);\n                }\n\n                return input;\n            }\n        }\n    }\n}","programmingLanguageId":"C#","multi":{"agentsIds":[3568301,-1],"gameOptions":"seed=-7116672829038362600\n"}}]



referer = "https://www.codingame.com/ide/34351598238a5ea185e3d0c7737f05a0f5afea36"


cookie = "_ga=GA1.2.395211775.1620369496; _gid=GA1.2.1761001152.1620369496; _fbp=fb.1.1620369496407.2118331531; rememberMe=150697057be2c8ff5cd4f2fecd221ef8ccc2ca6; cgSession=f456a280-8289-4df8-a3af-cbc8296f4fca; AWSALB=3VVFNIwDn6ERjTDOMdjBFU6ziCB9cjThiQGEJiNV1MTzr6ApN6HsD/DB7JMTDVPpPps6Pex6v/q0GiCiea97Yqg26uTVJeMirETuiJIMpWXT7kDyh/g0mqihFBE8; AWSALBCORS=3VVFNIwDn6ERjTDOMdjBFU6ziCB9cjThiQGEJiNV1MTzr6ApN6HsD/DB7JMTDVPpPps6Pex6v/q0GiCiea97Yqg26uTVJeMirETuiJIMpWXT7kDyh/g0mqihFBE8"
headers = {'Content-type':'application/json;charset=UTF-8', 
            'Accept':'application/json, text/plain, */*',
            'Referer': referer,
            'Cookie': cookie}
r = requests.post("https://www.codingame.com/services/TestSession/play", 
                    data=json.dumps(payload),
                    headers=headers)
data = r.json()['frames']

with open("../src/Multiplayer/SpringChallenge2021/temp.txt", 'w') as output_file:
    for key in data:
        if 'stderr' in key:
            output = key['stderr']
            output_file.write(output)
            output_file.write("\n")
			
with open("../src/Multiplayer/SpringChallenge2021/temp.txt", 'r') as input_file:
    with open("../src/Multiplayer/SpringChallenge2021/in.txt", 'w') as output_file:
        writeLine = False
        lines = input_file.readlines()
        for i, line in enumerate(lines):
            line = line.rstrip()
            if line == "IN":
                writeLine = True
            elif line == "/IN":
                writeLine = False
            elif writeLine:   
                output_file.write(line + "\n")

