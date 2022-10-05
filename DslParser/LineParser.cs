//import { StringBuilder } from "../Stringbuilder";
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DslParser
{
    public class LineParser
    {
        // Escape characters are not available in strucurizr  https://github.com/neovim/neovim/issues/17413
        public static List<string> Parse(string lineText)
        {
            List<string> parts = new ();
            StringBuilder sb = new ();

            bool inQuote = false;

            char character;

            for (var i = 0; i < lineText.Length; i++)
            {
                character = lineText[i];
                switch (character)
                {
                    case '"':
                        inQuote = !inQuote;
                        //sb.Append(character);
                        break;
                    case ' ':
                    case '\t':
                        if (inQuote)
                        {
                            sb.Append(character);
                        }
                        else
                        {
                            // this avoids creating blank fields
                            if (sb.Length > 0)
                            {
                                // treat as end of field
                                parts.Add(sb.ToString().TrimEnd());
                                sb.Clear();
                            }
                        }
                        break;
                    default:
                        sb.Append(character);
                        break;
                }
            }

            if (sb.Length > 0)
            {
                parts.Add(sb.ToString().TrimEnd());
                sb.Clear();
            }

            return parts;
        }
        public static List<string> Parse2(string lineText, char separator)
        {
            List<string> parts = new ();
            StringBuilder sb = new ();

            bool inQuote = false;

            char character;

            for (var i = 0; i < lineText.Length; i++)
            {
                character = lineText[i];

                if (character == separator)
                {
                    if (inQuote)
                    {
                        sb.Append(character);
                    }
                    else
                    {
                        // this avoids creating blank fields
                        if (sb.Length > 0)
                        {
                            // treat as end of field
                            parts.Add(sb.ToString().TrimEnd());
                            sb.Clear();
                        }
                    }
                }
                else
                {

                    switch (character)
                    {
                        case '"':
                            inQuote = !inQuote;
                            //sb.Append(character);
                            break;
                        default:
                            sb.Append(character);
                            break;
                    }
                }
            }

            if (sb.Length > 0)
            {
                parts.Add(sb.ToString().TrimEnd());
                sb.Clear();
            }

            return parts;
        }
    }
}
