using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass_Rater_2
{
    class GlassPanelModel
    {
        public string glassType;
        public int glassWidth;
        double[,] constants;

        public GlassPanelModel(string type, int width)
        {
            glassType = type;
            glassWidth = width;
            constants = new double[4, 8];
        }

        public void setConstantsRow(int rowNum, string[] csvRow)
        {
            //the first row is just the constant name so skip it
            for(int i=1; i<csvRow.Length; i++)
            {
                double d;
                double.TryParse(csvRow[i], out d);
                constants[rowNum, i-1] = d;
            }
        }

        public double[] getConstantsColumn(int ARColumn)
        {
            //the first arcolumn is k1, k2 etc.
            ARColumn += 1;
            double[] result = new double[4];
            for(int i = 0; i < 4; i++)
            {
                result[i] = constants[i, ARColumn];
            }
            return result;
        }
    }
}
