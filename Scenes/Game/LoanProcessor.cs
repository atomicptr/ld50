namespace LD50.Constants {
    public class LoanProcessor {
        private int index = 0;

        private readonly int[] amounts = new[] {
            5000,
            10000,
            20000,
            40000,
            800000,
            100000,
            200000,
            400000,
            1000000,
        };

        public int Stage {
            get => index;
        }

        public int? Next {
            get {
                // you won the game?
                if (index >= amounts.Length) {
                    return null;
                }

                return amounts[index];
            }
        }

        public void AdvanceToNextStage() {
            index++;
        }
    }
}
