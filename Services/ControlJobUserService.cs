using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using MudBlazor6.Models;
using Newtonsoft.Json;

namespace MudBlazor6.Services
{
    // сервис для работы с данными пользователя в базе данных и в защищенном хранилище браузера
    // подключается к внешней базе данных через CJDataContext
    // подключается к защищенному хранилищу браузера через ProtectedLocalStorage
    // 
    public class ControlJobUserService
    {
        private readonly CJDataContext dbContext;
        private readonly ProtectedLocalStorage browserStorage;
        private readonly string controlJobStorageKey = "ControlJobIdentity"; // ключ в хранилище браузера
        public ControlJobUserService(CJDataContext _context, ProtectedLocalStorage _storage) { dbContext = _context; browserStorage = _storage; }

        // поиск пользователя в базе данных по имени и паролю
        public async Task<User?> FindUserFromDBAsync(string username, string password)
        {
            User? user = await dbContext.Users.Where(u => (u.UserName == username && u.Password == password)).FirstOrDefaultAsync();
            if (user!=null) await PersistUserToBrowserAsync(user);
            return user;
        }

        // сохранение пользователя в защищенном хранилище браузера в виде Json
        public async Task PersistUserToBrowserAsync (User user)
        {
            string userJson=JsonConvert.SerializeObject(user);
            await browserStorage.SetAsync(controlJobStorageKey, userJson);
        }

        // извлечение пользователя с ключом controlJobStorageKey из хранилища браузера
        public async Task<User?> FetchUserFromBrowserAsync()
        {
            try
            {
                var fetchedUserResult = await browserStorage.GetAsync<string>(controlJobStorageKey);
                if (fetchedUserResult.Success && !string.IsNullOrEmpty(fetchedUserResult.Value))
                {
                    var user = JsonConvert.DeserializeObject<User>(fetchedUserResult.Value);
                    return user;
                }
            }
            catch { }
            return null;
        }

        // очистка хранилища браузера от данных с ключом controlJobStorageKey
        public async Task ClearBrowserUserDataAsync() => await browserStorage.DeleteAsync(controlJobStorageKey);
    }
}
