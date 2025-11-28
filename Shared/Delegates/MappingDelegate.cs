using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Delegates
{
    public class MappingDelegate
    {
        public delegate TDestination MapperDelagate<in TSource, out TDestination>(TSource source);
    }
}
