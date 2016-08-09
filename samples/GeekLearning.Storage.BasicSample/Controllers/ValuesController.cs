using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GeekLearning.Storage.BasicSample.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        private TemplatesStore templates;

        public ValuesController(TemplatesStore templates)
        {
            this.templates = templates;
        }

        // GET: api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {

            return new string[] { await templates.Store.ReadAllTextAsync("json.json"), "value2" };
        }

        // GET api/values/5
        [HttpGet("files")]
        public async Task<IEnumerable<string>> Get(int id)
        {
            var files = await templates.Store.ListAsync("");
            return files.Select(x => x.PublicUrl);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
