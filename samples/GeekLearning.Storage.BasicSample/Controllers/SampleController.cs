namespace GeekLearning.Storage.BasicSample.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    public class SampleController : Controller
    {
        private IStore sharedAssets;

        public SampleController(IStorageFactory storageFactory)
        {
            this.sharedAssets = storageFactory.GetStore("SharedAssets");
        }

        [HttpGet]
        public async ValueTask<IEnumerable<string>> Get()
        {
            var summaries = await this.sharedAssets.ListAsync("summaries", "*.txt", recursive: true, withMetadata: false);
            return summaries.Select(x => x.Path);
        }

        [HttpGet]
        public async ValueTask<string> Get(string path)
        {
            var summary = await this.sharedAssets.GetAsync(path);
            return await summary.ReadAllTextAsync();
        }

        [HttpPut()]
        public async Task Put(string path, [FromBody]string value)
        {
            await sharedAssets.SaveAsync(Encoding.UTF8.GetBytes(value), path, "text/plain");
        }

        [HttpDelete()]
        public async Task Delete(string path)
        {
            await sharedAssets.DeleteAsync(path);
        }
    }
}
