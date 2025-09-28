//using Swashbuckle.Examples;

//namespace WebApiDemo.Models
//{
//    public class TopicsRequestExample : IExamplesProvider
//    {
//        public TopicsRequest GetExamples()
//        {
//            return new TopicsRequest
//            {
//                Topics = new Dictionary<string, double>
//            {
//                { "reading", 20 },
//                { "math", 50 },
//                { "science", 30 },
//                { "history", 15 },
//                { "art", 10 }
//            }
//            };
//        }

//        object IExamplesProvider.GetExamples()
//        {
//            return GetExamples();
//        }
//    }
//}
using Swashbuckle.AspNetCore.Filters;
using WebApiDemo.Models;

public class TopicsRequestExample : IExamplesProvider<TopicsRequest>
{
    public TopicsRequest GetExamples()
    {
        return new TopicsRequest
        {
            Topics = new Dictionary<string, double>
            {
                { "reading", 20 },
                { "math", 50 },
                { "science", 30 },
                { "history", 15 },
                { "art", 10 }
            }
        };
    }
}