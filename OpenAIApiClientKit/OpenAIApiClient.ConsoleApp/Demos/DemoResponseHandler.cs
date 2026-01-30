using OpenAIApiClient.OrchestrationNEW04;
using System.Text;

namespace OpenAIApiClient.ConsoleApp.Demos
{
    public sealed class DemoResponseHandler : IResponseHandler
    {
        public string HandleSingle(ModelResponse response)
        {
            if (!response.IsSuccessful)
                return $"[ERROR from {response.Model.Name}] {response.ErrorMessage}";

            return $"[Single Model: {response.Model.Name}]\n{response.RawOutput}";
        }

        public string HandleEnsemble(IReadOnlyList<ModelResponse> responses)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[Ensemble Output]");

            foreach (var r in responses)
            {
                sb.AppendLine();
                sb.AppendLine($"--- {r.Model.Name} ---");

                if (r.IsSuccessful)
                    sb.AppendLine(r.RawOutput);
                else
                    sb.AppendLine($"ERROR: {r.ErrorMessage}");
            }

            return sb.ToString();
        }
    }
}
