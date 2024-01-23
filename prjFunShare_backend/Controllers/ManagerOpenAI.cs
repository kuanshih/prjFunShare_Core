using Microsoft.AspNetCore.Mvc;
using prjFunShare_backend.Models.ManagerOpenAI;
using System.Net.Http.Headers;
using System.Text;

namespace prjFunShare_backend.Controllers
{
    public class ManagerOpenAI : Controller
    {
        // OpenAI自動產生文案功能
        public IActionResult Index()
        {
            return View();
        }
                

        [HttpPost]
        [Consumes("application/json")]
        [Produces("text/plain")]
        public async Task<IActionResult> AISuggest([FromBody] AISuggestParameter parameter) 
        {
            HttpClient http = new HttpClient();

            string apiKey = ""; // 需加入OpenAI Code

            IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            string bearer = Config.GetSection("apiKey").Value;

            //authorization
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

            //處理傳入的指示
            StringBuilder sb = new StringBuilder();

            int i = 1;
            foreach (var sentence in parameter.instruct)
            {
                sb.Append(i);
                sb.Append(sentence);
                sb.Append(" ");
                i++;
            }
            string instruct = sb.ToString();
            ChatRequestParameter body = new ChatRequestParameter()
            {
                model = "gpt-3.5-turbo",
                messages = new List<ChatRequestMessageParameter>() {
                    new ChatRequestMessageParameter(){
                        role="system",
                        content="可以透過AI的力量產生各種文案"
                    },
                    new ChatRequestMessageParameter(){
                        role="user",
                        content=instruct
                    }
                }                
            };

            HttpResponseMessage response = await http.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", body);
            http.Dispose();

            string SuggestContent = "";
            if (response.IsSuccessStatusCode)
            {
                ChatResponseParameter json = await response.Content.ReadFromJsonAsync<ChatResponseParameter>();
                SuggestContent = json.choices.ElementAt(0).message.content;
                return Content(SuggestContent);
            }
            Response.StatusCode = 500;
            return Content("失敗");
        }
    }

}

