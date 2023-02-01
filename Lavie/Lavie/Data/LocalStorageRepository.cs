using Lavie.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Data
{
    public partial class LavieDatabase
    {
        public async Task<bool> ContainsKeyAsync(string key)
        {
            return (await database.Table<LocalStorage>().Where(u => u.Key == key).CountAsync()) > 0;
        }

        public Task<List<LocalStorage>> GetLocalStorageAsync()
        {
            return GetModelsAsync<LocalStorage>();
        }

        public Task<LocalStorage> GetLocalStorageAsync(string key)
        {
            return database.Table<LocalStorage>().Where(u => u.Key == key).FirstOrDefaultAsync();
        }

        public async Task<int> SaveLocalStorageAsync(LocalStorage model)
        {
            var t = await GetLocalStorageAsync(model.Key);
            if (t != null)
            {
                await DeleteModelAsync<LocalStorage>(t);
            }
            return await database.InsertAsync(model);
        }

        public async Task<int> DeleteLocalStorageAsync(LocalStorage model)
        {
            var t = await GetLocalStorageAsync(model.Key);
            if (t != null)
            {
                return await DeleteModelAsync<LocalStorage>(t);
            }
            else
            {
                return 0;
            }
        }
    }
}
