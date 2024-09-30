using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LogReader
{
    public class Translate
    {
        private string name;
        private string logname;
        private List<string> translation; //[0]->EN;[1]->DE;[2]->RU;[3]->??;

        public Translate(string logname, List<string> translation)
        {
            this.logname = logname;
            this.translation = translation;
        }

        public string GetTranslation(int index)
        {
            return this.translation[index];
        }
        public string GetLogname()
        {
            return this.logname;
        }
    }
}
