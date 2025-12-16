using Microsoft.ML.Data;

namespace Bot
{
    class OutputData
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }
        public float Probability { get; set; }

        public float Score { get; set; }
    }
}