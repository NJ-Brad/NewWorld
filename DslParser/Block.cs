using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DslParser
{
    public class Block
    {
        public string blockText = "";
        public List<Block> children = new ();
    }
}
