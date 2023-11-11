using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RacingGame
{
    public static class DictionaryExtension
    {
        /// <summary>
        /// loops through each object in the dictionary, and Invokes the labelDelegate and passes the label to it.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="labelDelegate">The delegate that will run for each label in the dictionary.</param>
        public static void RunForEach(this Dictionary<string, Label> dictionary, LabelDelegate labelDelegate)
        {
            foreach (KeyValuePair<string, Label> kvp in dictionary) labelDelegate.Invoke(kvp.Value);
        }

        /// <summary>
        /// delegate required for the Dictionary{string, label}.RunForEach Extension.
        /// </summary>
        /// <param name="label">The label that gets passed to each delegate</param>
        public delegate void LabelDelegate(Label label);
    }
}
