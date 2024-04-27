using FurAffinityFs;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Demo {
    class Program {
        static async Task Main() {
            var m = await FurAffinity.ListPostOptionsAsync();
            Console.WriteLine(m);
        }
    }
}
