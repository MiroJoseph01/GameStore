using System;
using System.Linq;
using GameStore.DAL.Entities;

namespace GameStore.DAL.Pipeline.Util
{
    public class OrderOptionModel
    {
        public string Name { get; set; }

        public string Text { get; set; }

        public Func<IQueryable<Game>, IOrderedQueryable<Game>> Func { get; set; }
    }
}
