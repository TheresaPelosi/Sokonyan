using System;

namespace Sokoban
{
    public interface Heuristic
    {
        int apply(State state);
    }

    public class NullHeuristic : Heuristic
    {
        public int apply(State state)
        {
            return 0;
        }
    }
    
    class UnMatchedBoxesHeuristic : Heuristic
    {
        public int apply(State state)
        {
            int unmatched = 0;
            foreach (var box in state.boxes)
            {
                if (!state.spots.Contains(box))
                {
                    unmatched += 1;
                }
            }

            return unmatched;
        }
    }

    class Manhattan : Heuristic
    {
        public int apply(State state)
        {
            int sum = 0;
            foreach (var (bx, by) in state.boxes)
            {
                int min = Int32.MaxValue;
                foreach (var (sx, sy) in state.spots)
                {
                    min = Math.Min(min, Math.Abs(bx - sx) + Math.Abs(by + sy));
                }

                sum += min;
            }

            return sum;
        }
    }
}