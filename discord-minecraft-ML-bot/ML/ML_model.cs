using Discord;
using Microsoft.ML;
using MongoDB.Driver;

namespace Bot
{
    public class ML_model
    {
        public static async Task ExecuteAsync(string message, ulong messageID, ITextChannel channel)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var dataBase = client.GetDatabase("test");
            var collection = dataBase.GetCollection<InputData>("data");

            var enumereable = collection.Find(_ => true).ToList();
            var hasDuplicate = collection.Find(x => x.Message == message).Any();

            var ctx = new MLContext();

            IDataView dataView = ctx.Data.LoadFromEnumerable<InputData>(enumereable);

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
            Console.WriteLine(metrics.Accuracy);

            string[] datacsvLines = File.ReadAllLines("configure/data.csv");
            for (int i = 0; i < datacsvLines.Length; i++)
                {
                    if (datacsvLines[i] == $"1;{message}" || datacsvLines[i] == $"0;{message}")
                    {
                        hasDuplicate = true;
                        Console.WriteLine("Yes" + hasDuplicate);
                    }
                }

            if (result.Score >= 2)
            {
                if (hasDuplicate == false)
                {
                    var resultInsert = dataBase.GetCollection<InputData>("data");
                    resultInsert.InsertOne(new InputData { Message = message, Label = false});
                } else
                {
                    Console.WriteLine("Is Duplicate");
                }
                if (channel != null) 
                await channel.DeleteMessageAsync(messageID);
            } else if (result.Score <= -2)
            {
                if (hasDuplicate == false)
                {
                    var resultInsert = dataBase.GetCollection<InputData>("data");
                    resultInsert.InsertOne(new InputData { Message = message, Label = false});
                }
            } else
            {
                hasDuplicate = false;
                return;
            }
            await Task.CompletedTask;
        }
    }
}