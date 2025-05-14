using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.AbstractEntity
{
    public class EntityData
    {
        public Queue<bool?> bools;
        public Queue<int?> ints;
        public Queue<double?> doubles;
        public Queue<Point?> points;

        private static EntityData? data;
        public static EntityData GetInstance()
        {
            if (data == null) data = new EntityData();
            return data;
        }
        private EntityData()
        {
            bools = new Queue<bool?>();
            ints = new Queue<int?>();
            doubles = new Queue<double?>();
            points = new Queue<Point?>();
        }

        public void Reset()
        {
            bools.Clear();
            ints.Clear();
            doubles.Clear();
            points.Clear();
        }
    }
}
