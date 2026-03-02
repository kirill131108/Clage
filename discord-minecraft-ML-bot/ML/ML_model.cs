using Discord;
using Microsoft.ML;
using MongoDB.Driver;

namespace Bot
{
    public class ML_model
    {

        private static MLContext ctx = new MLContext();
        private static ITransformer trainmodel;
        public static async Task ExecuteAsync(string message, ulong messageID, ITextChannel channel)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var dataBase = client.GetDatabase("test");
            var collection = dataBase.GetCollection<InputData>("data");

            // convert data type for model
            var enumereable = collection.Find(_ => true).ToList();
            // check for dublicate message
            var hasDuplicate = collection.Find(x => x.Message == message).Any();

            //load model
            trainmodel = ctx.Model.Load("model.zip", out var schema);

            var predEngine = ctx.Model.CreatePredictionEngine<InputData, OutputData>(trainmodel);

            var context = new InputData()
            {
                Message = message
            };

            
            var result = predEngine.Predict(context);

            // check of the model
            Console.WriteLine(result.Score);
            
            // check for message.
            if (result.Score >= 2)
            {
                if (!hasDuplicate)
                {
                    // add to bd
                    var resultInsert = dataBase.GetCollection<InputData>("data");
                    resultInsert.InsertOne(new InputData { Message = message, Label = false});
                } else
                {
                    Console.WriteLine("Is Duplicate");
                }
                if (channel != null) {};
                await channel.DeleteMessageAsync(messageID);
            } else if (result.Score <= -2)
            {
                if (!hasDuplicate)
                {
                    //add to db
                    var resultInsert = dataBase.GetCollection<InputData>("data");
                    resultInsert.InsertOne(new InputData { Message = message, Label = false});
                }
            } 
                // for o(n) algoritm
            /* else
            {
                hasDuplicate = false;
                return;
            } */
            await Task.CompletedTask;
        }
        public static async Task TrainModel(String path)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var dataBase = client.GetDatabase("test");
            var collection = dataBase.GetCollection<InputData>("data");

            var enumereable = collection.Find(_ => true).ToList();


            var file = File.Exists("model.zip");
            // check a model and load model
            if (file == true)
                trainmodel = ctx.Model.Load(path, out var inputSchema);

            IDataView dataView = ctx.Data.LoadFromEnumerable<InputData>(enumereable);

            var trainsplitdata = ctx.Data.TrainTestSplit(dataView, 0.2);
            //train model 
            IDataView trainer = trainsplitdata.TrainSet;
            IDataView trainer1 = trainsplitdata.TestSet;

            var dataProcessPipeLine = ctx.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(InputData.Message));

            var training = ctx.BinaryClassification.Trainers.AveragedPerceptron(labelColumnName:"Label", featureColumnName:"Features");
            var trainingPipeLine = dataProcessPipeLine.Append(training);

            ITransformer transformer = trainingPipeLine.Fit(trainer);

            ctx.Model.Save(transformer, dataView.Schema, "model.zip");
        }
        public static async Task RunTrainModel()
        {
            var time = new PeriodicTimer(TimeSpan.FromHours(3));

            await Task.Run(async () =>
            {
                while(await time.WaitForNextTickAsync()) {
                    await ML_model.TrainModel("model.zip");
                    await RunTrainModel();
                    return;
                }
            });
        }
    }
}
