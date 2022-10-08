using Markdig;

namespace MarkdownConverter
{
    public class MdPublisher
    {
        public static string Publish(string mdText)
        {
            // Configure the pipeline with all advanced extensions active
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var result = Markdown.ToHtml(mdText, pipeline);

            return result.ToString(); 
        }
    }
}

