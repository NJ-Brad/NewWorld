//import { Block } from "./Block";
//import { StringBuilder } from "../Stringbuilder";
//import { StringStream } from "./StringStream";
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DslParser
{
    public class BlockParser
    {
        public Block ParseText(StreamReader sr)
        {
            Block block;
            block = new Block { 
                blockText = "Top Level"
            };
            this.Parse(block.children, sr, 1);

            return block;
        }

        public void Parse(List<Block> blocks, StreamReader sr, int level)
        {

            Block rtnVal = new ();
            bool inQuote = false;

            StringBuilder sb;
            sb = new StringBuilder();

            char character;
            bool keepGoing;
            keepGoing = true;

            while (!sr.EndOfStream && keepGoing)
            {
                character = (char)sr.Read();
                switch (character)
                {
                    case '"':
                        {
                            inQuote = !inQuote;
                            sb.Append(character);
                            break;
                        }
                    case '\n':
                    case '\r':
                        if (sb.Length > 0)
                        {
                            rtnVal.blockText = sb.ToString().Trim();
                            if (rtnVal.blockText.Length > 0)
                            {
                                blocks.Add(rtnVal);
                            }
                            sb.Clear();

                            rtnVal = new Block();
                        }
                        break;
                    case '[':
                        if (!inQuote)
                        {
                            // add to the previous node (Not the potential new one)
                            this.Parse(blocks[blocks.Count - 1].children, sr, level + 1);
                        }
                        else
                        {
                            sb.Append(character);
                        }
                        break;
                    case ']':
                        if (!inQuote)
                        {
                            level--;
                            keepGoing = false;
                        }
                        else
                        {
                            sb.Append(character);
                        }
                        break;
                    default:
                        sb.Append(character);
                        break;
                }
            }

            if (sb.Length > 0)
            {
                rtnVal.blockText = sb.ToString().Trim();
                if (rtnVal.blockText.Length > 0)
                {
                    blocks.Add(rtnVal);
                }
                sb.Clear();
            }
            return;
        }
    }
}
