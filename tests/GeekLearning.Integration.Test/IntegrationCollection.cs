using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GeekLearning.Integration.Test
{
    [CollectionDefinition(nameof(IntegrationCollection))]
    public class IntegrationCollection: ICollectionFixture<StoresFixture>
    {

    }
}
