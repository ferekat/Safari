using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Persistence
{
    public class SaveData
    {
        public string Time { get; private set; }
        public string Money { get; private set; }
        public string Name { get; private set; }
        public string Difficulty { get; private set; }
        private SaveData() 
        {
            Time = "";
            Money = "";
            Name = "";
            Difficulty = "";
        }

        public static (SaveData,bool) GetSaveData(string filePath)
        {
            if (!File.Exists(filePath)) return (new SaveData(), false);
            SaveData save = new SaveData();

            using (StreamReader reader = new StreamReader(filePath))
            {
                save.Money = reader.ReadLine() + "$";
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();
                string day = reader.ReadLine() + "";
                string week = reader.ReadLine() + "";
                string month = reader.ReadLine() + "";
                save.Time = $"{month}. month/{week}. week/{day}.day";
                //TODO név és nehézség
                save.Name = "TempName";
                save.Difficulty = "TempDifficulty";

                return (save, true);
            }
        }

    }
}
