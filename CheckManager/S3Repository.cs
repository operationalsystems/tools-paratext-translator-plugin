using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvpMain.Check;

namespace TvpMain.CheckManager
{
    class S3Repository : IRepository
    {
        public void AddCheckAndFixItem(CheckAndFixItem item)
        {
            throw new NotImplementedException();
        }

        public Task AddCheckAndFixItemAsync(CheckAndFixItem item)
        {
            throw new NotImplementedException();
        }

        public List<CheckAndFixItem> GetCheckAndFixItems()
        {
            throw new NotImplementedException();
        }

        public Task<List<CheckAndFixItem>> GetCheckAndFixItemsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
