using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace Glass_Rater_2
{
    class GlassPanelController
    {
        List<GlassPanelModel> Annealed = new List<GlassPanelModel>();
        List<GlassPanelModel> Laminated = new List<GlassPanelModel>();
        List<GlassPanelModel> Heated = new List<GlassPanelModel>();
        List<GlassPanelModel> Toughened = new List<GlassPanelModel>();

        //20,24 only apply to laminated but should be fine here
        int[] glassThicknesses = new int[] { 3, 4, 5, 6, 7, 8, 10, 12, 15, 16, 19, 24, 25 };

        //C1, C2, C3, C4
        double[] pressureRatings = new double[] { 1.800, 2.700, 4.000, 5.300};

        public GlassPanelController()
        {
            getData();
        }

        private void getData()
        {
            using (TextFieldParser parser = new TextFieldParser(@"C:\Users\micha\Desktop\constants.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                int rowNum = 0;
                while (!parser.EndOfData)
                {
                    //Title row (glass thickness, string glass Type)
                    string[] fields = parser.ReadFields();
                    //get the glass thinkness
                    int w;
                    int.TryParse(fields[0], out w);
                    GlassPanelModel gp = new GlassPanelModel(fields[1], w);

                    //4 rows of constants
                    for(int i=0; i < 4; i++)
                    {
                        string[] Kfields = parser.ReadFields();
                        gp.setConstantsRow(i, Kfields);
                    }

                    //Add it to the appropriate list
                    if (gp.glassType == "Annealed") { Annealed.Add(gp); }
                    else if (gp.glassType == "Laminated") { Laminated.Add(gp); }
                    else if (gp.glassType == "Heated") { Heated.Add(gp); }
                    else if (gp.glassType == "Toughened") { Toughened.Add(gp); }
                    else { throw new Exception("Not a type of glass"); }
                }
            }
        }

        private static double stringToDouble(string input)
        {
            double d;
            double.TryParse(input,out d);
            return d;
        }

        private int getAspectRatioColumn(GlassPanelModel gp, int width, int height)
        {
            //Finding the AspectRatio (aspect ratio is also y-component of gpmodel)
            double[] ARs = new double[] { 1, 1.25, 1.5, 1.75, 2, 2.5, 3, 5 };

            //set shortest and longest lengths
            double longestLength;
            double shortestLength;
            if (width >= height) { longestLength = width; shortestLength = height; }
            else { longestLength = height; shortestLength = width; }

            double aspectRatio = longestLength / shortestLength;
            if (aspectRatio > 5)
            {
                throw new Exception("Aspect ratio is greater than 5");
            }
            int i = 0;
            while(i<ARs.Length)
            {
                if (aspectRatio <= ARs[i])
                {
                    //i is now the value of the ARs array
                    break;
                }
                i++;
            }
            return i;
            
        }


        private double calcAllowableSpanB(double[] constants, double pressureRating)
        {
            
            return constants[0]*Math.Pow((pressureRating + constants[1]), constants[2]) + constants[3];
        }

        public int getRequiredGlassThckness(int width, int height, string glassType, string pressureRating)
        {
            List<GlassPanelModel> lGPM = getgpmListFromString(glassType);
            //Dirty loop to find lowest to highest glass thickness
            for (int i = 0; i < glassThicknesses.Length; i++)
            {
                foreach (GlassPanelModel gp in lGPM)
                {
                    if (gp.glassWidth == glassThicknesses[i])
                    {
                        //have the correct thickness
                        //if calcAllowableSpanB is less than the actual span we are good
                        if (isAllowable(width, height, gp, pressureRating))
                        {
                            return gp.glassWidth;
                        }
                    }
                }
            }
            return 0;
        }

        private List<GlassPanelModel> getgpmListFromString(string glassType)
        {
            if (glassType == "Annealed") { return Annealed; }
            else if (glassType == "Laminated") { return Laminated; }
            else if (glassType == "Heated") { return Heated; }
            else if (glassType == "Toughened") { return Toughened; }
            else { throw new Exception("Bad glass Type"); }
        }

        private bool isAllowable(int width, int height, GlassPanelModel gp, string pressureRating)
        {
            double actualPressure = pressureRatingStringToInt(pressureRating);
            int AR = getAspectRatioColumn(gp, width, height);
            //getConstants
            double[] constants = gp.getConstantsColumn(AR);

            double allowableSpanB = calcAllowableSpanB(constants, actualPressure);

            //Span B is the smallest dimension
            int SpanB;
            if(width<= height) { SpanB = width; }
            else { SpanB = height; }

            return SpanB <= allowableSpanB;
            
        }
        
        private double pressureRatingStringToInt(string pressureRating)
        {
            if (pressureRating == "C1") { return pressureRatings[0]; }
            else if (pressureRating == "C2") { return pressureRatings[1]; }
            else if (pressureRating == "C3") { return pressureRatings[2]; }
            else if (pressureRating == "C4") { return pressureRatings[3]; }
            else { throw new Exception(); }
        }
    }
}
