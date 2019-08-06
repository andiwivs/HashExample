using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HashExample
{
    class Program
    {
        static void Main(string[] args)
        {

            // Note: we begin with the happy path, creating a "protected" model

            var rawModel = new ModelDto
            {
                Name = "Fred Flintstone",
                Hobbies = new[] { "Bowling", "Dining" }
            };

            var protectedModel = new ProtectedModelDto
            {
                InnerModel = rawModel,
                ModelHash = GenerateHash(rawModel)
            };


            // TODO: serialise / desrialise via Api call, etc

            var tamperedModel = protectedModel;
            tamperedModel.InnerModel.Name = "Barny Rubble";


            // Note: we are now simulating the receipt of a tampered model, ie the below represents a separate Api method

            var receivedModel = tamperedModel;

            var validationHash = GenerateHash(receivedModel.InnerModel);

            Console.WriteLine($"Received hash: {protectedModel.ModelHash}");
            Console.WriteLine($"Re-generated hash from received model: {validationHash}");
            Console.WriteLine("Hash valid: " + protectedModel.ModelHash.Equals(validationHash));
            Console.WriteLine("---------------------------------------------------------------------------------------------");


            // Let's put it back to how it was and check if the hash validates

            var correctedModel = receivedModel;
            correctedModel.InnerModel.Name = "Fred Flintstone";

            validationHash = GenerateHash(correctedModel.InnerModel);

            Console.WriteLine($"Received hash: {receivedModel.ModelHash}");
            Console.WriteLine($"Re-generated hash from corrected model: {validationHash}");
            Console.WriteLine("Hash valid: " + receivedModel.ModelHash.Equals(validationHash));
        }

        private static string GenerateHash(object source)
        {
            var rawModel = JsonConvert.SerializeObject(source);

            return string.Concat(
                    SHA256.Create()
                        .ComputeHash(Encoding.UTF8.GetBytes(rawModel))
                        .Select(item => item.ToString("x2"))
                );
        }
    }

    public class ModelDto
    {
        public string Name { get; set; }
        public string[] Hobbies { get; set; }
    }

    public class ProtectedModelDto
    {
        public ModelDto InnerModel { get; set; }
        public string ModelHash { get; set; }
    }
}
