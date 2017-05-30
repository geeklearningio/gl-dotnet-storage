namespace GeekLearning.Storage.BasicSample.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private TemplatesStore templates;

        public ValuesController(TemplatesStore templates)
        {
            this.templates = templates;
        }

        [HttpGet]
        public async ValueTask<IEnumerable<string>> Get()
        {
            return new string[] { await templates.Store.ReadAllTextAsync("json.json"), "value2" };
        }

        [HttpGet("files")]
        public async ValueTask<IEnumerable<string>> Get(int id)
        {
            var files = await templates.Store.ListAsync("");
            return files.Select(x => x.PublicUrl);
        }

        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
