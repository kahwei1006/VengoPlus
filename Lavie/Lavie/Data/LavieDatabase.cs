using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lavie.Models;
using SQLite;

namespace Lavie.Data
{
    public partial class LavieDatabase
    {
        readonly SQLiteAsyncConnection database;

        public LavieDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<LocalStorage>().Wait();
        }

        public Task<int> GetModelCountAsync<T>() where T : DbRecord, new()
        {
            return database.Table<T>().CountAsync();
        }

        public Task<List<T>> GetModelsAsync<T>() where T : DbRecord, new()
        {
            return database.Table<T>().ToListAsync();
        }

        public Task<T> GetModelAsync<T>(int id) where T : DbRecord, new()
        {
            return database.Table<T>().Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        protected Task<int> SaveModelAsync<T>(T model) where T : DbRecord, new()
        {
            if (model.Id > 0)
            {
                return database.UpdateAsync(model);
            }
            else
            {
                return database.InsertAsync(model);
            }
        }

        public Task<int> DeleteModelAsync<T>(T model) where T : DbRecord, new()
        {
            return database.DeleteAsync(model);
        }
    }
}
