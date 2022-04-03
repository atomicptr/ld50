namespace LD50.Constants {
    public class LoanProcessor {
        private int index = 0;

        private readonly int[] amounts = new[] {
            10000,
            20000,
            40000,
            80000,
            100000,
            150000,
            200000,
            500000,
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
