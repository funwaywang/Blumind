using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blumind.Configuration.Models
{
    class MemoryOptions : Options
    {
        public MemoryOptions()
        {
        }

        public MemoryOptions(Options options)
            : base(options)
        {
        }

        public override void Load(string[] args)
        {
            throw new NotSupportedException();
        }

        public override bool Save()
        {
            throw new NotSupportedException();
        }
    }
}
