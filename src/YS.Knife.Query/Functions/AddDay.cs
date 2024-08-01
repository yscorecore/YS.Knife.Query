using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Query.Expressions;

namespace YS.Knife.Query.Functions
{
    internal class AddDay : InstanceFunction<DateTime, DateTime>
    {
        public AddDay() : base((s) => s.AddDays(It.Arg<double>()))
        {
        }
    }
}
