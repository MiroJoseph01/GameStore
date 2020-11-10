using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameStroe.Mobile.Services.Interfaces
{
    public interface IReadOnlyDataStore<T>
    {
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }
}
