using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameStroe.Mobile.Services.Interfaces
{
    public interface IDataStore<T> : IReadOnlyDataStore<T>
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
    }
}
