using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Persistence
{
    public interface IDataAccess
    {
        public Task<GameData> LoadAsync(string filepath);
        public Task SaveAsync(string filepath, GameData data);
    }
}
