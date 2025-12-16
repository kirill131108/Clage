using Microsoft.ML.Data;

namespace Bot
{
    public class InputData
    {
        [LoadColumn(0)]
        public bool Label { get; set; }
        [LoadColumn(1)]
        public string? Message { get; set; }
    }
}