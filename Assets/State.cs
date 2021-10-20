using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban
{
    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
    
    public class State
    {
        public HashSet<(int, int)> boxes;
        public HashSet<(int, int)> spots;
        private HashSet<(int, int)> walls;
        private HashSet<(int, int)> validBoxPositions;
        public (int, int) player;
        private int numRows;
        private int numCols;

        public static int NODES_EXPANDED { get; private set; }

        private State(State copy)
        {
            this.player = copyPos(copy.player);
            this.boxes = new HashSet<(int, int)>(copy.boxes);
            
            // Copy spots and walls by reference as these are never mutated
            this.spots = copy.spots;
            this.walls = copy.walls;
            this.validBoxPositions = copy.validBoxPositions;
            
            this.numRows = copy.numRows;
            this.numCols = copy.numCols;
        }

        private static (int, int) copyPos((int, int) pos)
        {
            return (pos.Item1, pos.Item2);
        }

        public State(String levelFilePath, int index) 
            : this( new List<string>(
                System.IO.File.ReadAllText(levelFilePath)
                    .Split(';')[index]
                    .Split('\n'))
                .FindAll(l => l.Length > 0)) {}

        public State(List<String> lines)
        {
            boxes= new HashSet<(int, int)>();
            spots = new HashSet<(int, int)>();
            walls = new HashSet<(int, int)>();
            validBoxPositions = new HashSet<(int, int)>();
            player = (-1, -1);

            numRows = lines.Count;

            NODES_EXPANDED = 0;
            
            for (int row = 0; row < numRows; ++row)
            {
                char[] values = lines[row].ToCharArray();
                numCols = values.Length;
                for (int col = 0; col < numCols; ++col)
                {
                    switch (values[col])
                    {
                        case ' ':
                            continue;
                        case '#':
                            walls.Add((row, col));
                            break;
                        case '.':
                            spots.Add((row, col));
                            break;
                        case '@':
                            player = (row, col);
                            break;
                        case '?':
                            spots.Add((row, col));
                            boxes.Add((row, col));
                            break;
                        case '$':
                            boxes.Add((row, col));
                            break;
                        default:
                            continue;
                    }
                }
            }
            calcDeadSquares();
        }

        private void calcDeadSquares()
        {
            Queue<(int, int)> frontier = new Queue<(int, int)>();
            // All spots are valid positions
            foreach (var spot in spots)
            {
                frontier.Enqueue(spot);
            }
            while (frontier.Count != 0)
            {
                (int, int) valid = frontier.Dequeue();
                if (validBoxPositions.Contains(valid))
                {
                    continue;
                }

                validBoxPositions.Add(valid);
                // Add all positions we could have pushed a box into this position from to the frontier
                foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                {
                    (int, int) pushedFrom = newPos(dir, valid);
                    // Position the player would need to be to push the box
                    (int, int) playerPos = newPos(dir, pushedFrom);
                    if (!(walls.Contains(playerPos) || walls.Contains(pushedFrom)))
                    {
                        frontier.Enqueue(pushedFrom);
                    }
                }
            }
        }

        private void validateLines(List<String> lines)
        {
            int len = -1;
            foreach (String line in lines)
            {
                if (len == -1)
                {
                    len = line.Length;
                }
                else if (len != line.Length)
                {
                    throw new Exception("Found invalid sokoban board (lines must be same length)");
                }
            }
        }

        public Dictionary<Direction, State> getSuccessors()
        {
            Dictionary<Direction, State> successors = new Dictionary<Direction, State>();
            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                Optional<State> successor = getSuccessor(dir);
                if (successor.isPresent())
                {
                    successors.Add(dir, successor.unwrap());
                }
            }

            NODES_EXPANDED++;
            return successors;
        }

        public List<Direction> getLegalActions()
        {
            List<Direction> actions = new List<Direction>();
            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                Optional<State> successor = getSuccessor(dir);
                if (successor.isPresent() && successor.unwrap().isSolvable())
                {
                    actions.Add(dir);
                }
            }

            return actions;
        }

        public Optional<State> getSuccessor(Direction dir)
        {
            Optional<State> none = new None<State>();
            (int, int) newPos = State.newPos(dir, player);
            int newRow = newPos.Item1;
            int newCol = newPos.Item2;
            if (newRow >= numRows || newCol >= numCols || newRow < 0 || newCol < 0)
            {
                return none;
            }

            if (walls.Contains(newPos))
            {
                return none;
            }

            if (!boxes.Contains(newPos))
            {
                return new Some<State>(justMovePlayer(newPos));
            }

            return canBoxBePushed(dir, newPos) ? new Some<State>(pushBox(dir, newPos)) : none;
        }

        private State justMovePlayer((int, int) newPos)
        {
            State newState = new State(this);
            newState.player = newPos;
            return newState;
        }

        private State pushBox(Direction dir, (int, int) newPos)
        {
            State newState = new State(this);
            newState.player = newPos;
            (int, int) newBoxPos = State.newPos(dir, newPos);
            newState.boxes.Remove(newPos);
            newState.boxes.Add(newBoxPos);
            return newState;
        }

        private bool canBoxBePushed(Direction dir, (int, int) pos)
        {
            (int, int) newPos = State.newPos(dir, pos);
            return (!(boxes.Contains(newPos) || walls.Contains(newPos)));
        }

        private static (int, int) newPos(Direction dir, (int, int) pos)
        {
            switch (dir)
            {
                case Direction.UP:
                    return (pos.Item1 - 1, pos.Item2);
                case Direction.DOWN:
                    return (pos.Item1 + 1, pos.Item2);
                case Direction.LEFT:
                    return (pos.Item1, pos.Item2 - 1);
                case Direction.RIGHT:
                    return (pos.Item1, pos.Item2 + 1);
            }

            throw new Exception("You shouldn't be able to get here");
        }

        public bool isGoalState()
        {
            foreach ((int, int) box in boxes)
            {
                if (!spots.Contains(box))
                {
                    return false;
                }
            }

            return true;
        }

        public int CountPlacedBoxes()
        {
            return boxes.Count(b => spots.Contains(b));
        }
        
        public void print()
        {
            Console.Out.WriteLine(this.ToString());           
        }
        
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int row = 0; row < numRows; ++row)
            {
                StringBuilder line = new StringBuilder();
                for (int col = 0; col < numCols; ++col)
                {
                    (int, int) pos = (row, col);
                    if (player == pos)
                    {
                        line.Append("@");
                    }
                    else if (walls.Contains(pos))
                    {
                        line.Append("#");
                    }
                    else if (spots.Contains(pos) && boxes.Contains(pos))
                    {
                        line.Append("?");
                    }
                    else if (spots.Contains(pos))
                    {
                        line.Append(".");
                    }
                    else if (boxes.Contains(pos))
                    {
                        line.Append("$");
                    }
                    else
                    {
                        line.Append(" ");
                    }
                }

                builder.Append(line + "\n");
            }

            return builder.ToString();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is State))
            {
                return false;
            }

            var other = (State) obj;
            return posEquals(other.player, player)
                   && posSetsEqual(walls, other.walls)
                   && posSetsEqual(boxes, other.boxes)
                   && posSetsEqual(spots, other.spots);
        }

        private static bool posSetsEqual(HashSet<(int, int)> set1, HashSet<(int, int)> set2)
        {
            if (set1.Count != set2.Count)
            {
                return false;
            }
            return set1.All(pos => set2.Contains(pos));
        }

        private static bool posEquals((int, int) pos1, (int, int) pos2)
        {
            return pos1.Item1 == pos2.Item1
                   && pos1.Item2 == pos2.Item2;
        }

        public bool isSolvable()
        {
            return NoBoxInDeadSpace()
            && NoBoxesFrozen();
        }

        private bool NoBoxesFrozen()
        {
            return boxes.All(box => spots.Contains(box) || !IsBoxFrozen(box, new HashSet<(int, int)>()));
        }

        private bool IsBoxFrozen((int, int) box, HashSet<(int, int)> seen)
        {
            bool frozenVert = IsBoxSimplyFrozenOnAxis(box, Direction.UP, seen);
            bool frozenHoriz = IsBoxSimplyFrozenOnAxis(box, Direction.LEFT, seen);

            seen.Add(box);

            if (frozenHoriz && frozenVert)
            {
                return true;
            } 
            else if (frozenHoriz)
            {
                // Check for vertical boxes
                return IsBoxFrozenByBoxAxis(box, Direction.UP, seen);
            } 
            else if (frozenVert)
            {
                // Check for horizontal boxes
                return IsBoxFrozenByBoxAxis(box, Direction.LEFT, seen);
            }
            else
            {
                // Check for frozen horizontal boxes
                // If none, check for frozen vertical boxes
                return IsBoxFrozenByBoxAxis(box, Direction.UP, seen) 
                       && IsBoxFrozenByBoxAxis(box, Direction.LEFT, seen);
            }
        }

        private bool IsBoxFrozenByBoxAxis((int, int) box, Direction dir, HashSet<(int, int)> seen)
        {
            (int, int) a = newPos(dir, box);
            (int, int) b = newPos(opposite(dir), box);
            
            return (boxes.Contains(a) && IsBoxFrozen(a, seen)) 
                   || (boxes.Contains(b) && IsBoxFrozen(b, seen));
        }

        private bool IsBoxSimplyFrozenOnAxis((int, int) box, Direction dir, HashSet<(int, int)> seen)
        {
            (int, int) adjAxis1 = newPos(dir, box);
            (int, int) adjAxis2 = newPos(opposite(dir), box);
            
            // We are frozen on this axis if either side is a wall or "seen" box
            if (walls.Contains(adjAxis1)
                || walls.Contains(adjAxis2)
                || seen.Contains(adjAxis1)
                || seen.Contains(adjAxis2))
            {
                return true;
            }
            
            // We are frozen on this axis if there is a dead space on both sides
            return !validBoxPositions.Contains(adjAxis1) && !validBoxPositions.Contains(adjAxis2);
        }

        private static Direction opposite(Direction dir)
        {
            switch (dir)
            {
                case Direction.UP:
                    return Direction.DOWN;
                case Direction.DOWN:
                    return Direction.UP;
                case Direction.LEFT:
                    return Direction.RIGHT;
                case Direction.RIGHT:
                    return Direction.LEFT;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }

        private bool NoBoxInDeadSpace()
        {
            return boxes.All(box => validBoxPositions.Contains(box));
        }
    }
}