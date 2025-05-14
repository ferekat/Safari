using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Persistence
{
    internal class GameDataException : Exception
    {
        public GameDataException(string message) : base(message) { }
    }
}
