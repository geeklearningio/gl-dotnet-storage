using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GeekLearning.Storage.BasicSample.Controllers
{
    [Route("api/[controller]")]
    public class SampleController : Controller
    {
        private IStore sharedAssets;

        public SampleController(IStorageFactory storageFactory)
        {
            this.sharedAssets = storageFactory.GetStore("SharedAssets");
        }

        // GET: api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var summaries = await this.sharedAssets.ListAsync("summaries", "*.txt", recursive: true, withMetadata: false);
            return summaries.Select(x => x.Path);
        }

        // GET api/values/5
        [HttpGet]
        public async Task<string> Get(string path)
        {
            var summary = await this.sharedAssets.GetAsync(path);
            return await summary.ReadAllTextAsync();
        }

        // PUT api/values/5
        [HttpPut()]
        public async Task Put(string path, [FromBody]string value)
        {
            await sharedAssets.SaveAsync(Encoding.UTF8.GetBytes(value), path, "text/plain");
        }

        // DELETE api/values/5
        [HttpDelete()]
        public async Task Delete(string path)
        {
            await sharedAssets.DeleteAsync(path);
        }
    }
}
