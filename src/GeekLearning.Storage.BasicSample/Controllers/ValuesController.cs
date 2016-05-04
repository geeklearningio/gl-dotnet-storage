using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace GeekLearning.Storage.BasicSample.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private TemplatesStore templates;

        public ValuesController(TemplatesStore templates)
        {
            this.templates = templates;
        }

        // GET: api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {

            return new string[] { await templates.Store.ReadAllText("json.json"), "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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
