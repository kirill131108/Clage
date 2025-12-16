using Microsoft.ML;

namespace Bot
{
    public class ML_model
    {
        public static void ExecuteAsync(string message)
        {
            var ctx = new MLContext();

            IDataView dataView = ctx.Data.LoadFromTextFile<InputData>("data.csv", hasHeader: false, separatorChar: ';');

            var trainsplitdata = ctx.Data.TrainTestSplit(dataView, 0.2);
            IDataView trainingData = trainsplitdata.TrainSet; 
            IDataView testData = trainsplitdata.TrainSet;

            var dataProcessPipeLine = ctx.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(InputData.Message));

            var trainer = ctx.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName:"Label", featureColumnName:"Features");
            var trainingPipeLine = dataProcessPipeLine.Append(trainer);

            ITransformer trainmodel = trainingPipeLine.Fit(trainingData);

            var predictions = trainmodel.Transform(testData);
            var metrics = ctx.BinaryClassification.Evaluate(data: predictions, labelColumnName:"Label", scoreColumnName:"Score");

            var predEngine = ctx.Model.CreatePredictionEngine<InputData, OutputData>(trainmodel);

            var context = new InputData()
            {
                Message = message
            };


            var result = predEngine.Predict(context);

            Console.WriteLine(result.Score);

            if (result.Score >= 3)
            {
                File.AppendAllText("data.csv", $"1{message}");
            } else if (result.Score <= -3)
            {
                File.AppendAllText("data.csv", $"0{message}");
            } else
            {
                return;
            }
        }
    }
}