using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GeekLearning.Integration.Test
{
    [CollectionDefinition(nameof(Integration))]
    public class Integration: ICollectionFixture<StoresFixture>
    {

    }
}
