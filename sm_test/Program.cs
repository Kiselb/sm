using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace sm_test
{
    class Program
    {
        class Element
        {
            public String code;
            public String[] parameters;

            public Element(String code, String parameters_string)
            {
                this.code = code;
                this.parameters = parameters_string.Replace("(", "").Replace(")", "").Split(',');
            }
        }

        static void EdgeProcess(Dictionary<String, Element> elements, Element edge)
        {
            Element element = elements[edge.parameters[1]]; // edge loop
            element = elements[element.parameters[1]];      // oriented edge
            element = elements[element.parameters[3]];      // edge curve
            element = elements[element.parameters[3]];      // circle
            double radius = double.Parse(element.parameters[2]);
            element = elements[element.parameters[1]];      // axis placement
            element = elements[element.parameters[1]];      // cartesian point
            double x = double.Parse(element.parameters[1]);
            double y = double.Parse(element.parameters[2]);
            double z = double.Parse(element.parameters[3]);

            Console.WriteLine("Center: {0} {1} {2} Radius: {3}", x, y, z, radius);
        }
        static void ConeProcess(Dictionary<String, Element> elements)
        {
            foreach(KeyValuePair<string, Element> pair in elements)
            {
                if (pair.Value.code == "ADVANCED_FACE" && pair.Value.parameters.Length == 5)
                {
                    if (elements[pair.Value.parameters[3]].code == "CONICAL_SURFACE")
                    {
                        Console.WriteLine("CONE FOUND: " + pair.Key.ToString());
                        //Console.WriteLine(pair.Value.parameters);

                        Element edge1 = elements[pair.Value.parameters[1]];
                        Element edge2 = elements[pair.Value.parameters[2]];

                        Element canonical_surface = elements[pair.Value.parameters[3]];

                        double radius = double.Parse(canonical_surface.parameters[2]);
                        double angle  = double.Parse(canonical_surface.parameters[3]);
                        double height = double.Parse(canonical_surface.parameters[2]) * Math.Tan((Math.PI / 2.0) - angle);

                        EdgeProcess(elements, edge1);
                        EdgeProcess(elements, edge2);

                        Console.WriteLine("Angle: {0} Height: {1}", angle, height);

                    }
                }
            }

        }
        static void Main(string[] args)
        {
            String line;
            Dictionary<String, Element> elements = new Dictionary<String, Element>();

            try
            {
                Console.WriteLine("Введите имя файла");

                String file_name = Console.ReadLine();
                StreamReader sr = new StreamReader(file_name); // (@"D:\EDUCATION\SM\ппд 1х3х5 д0,3 с фасками.stp");
                line = sr.ReadLine();

                while(line != null) {
                    line = line.Replace(" ", "");
                    if (line[0] == '#')
                    {
                        String[] parts = line.Split('=');
                        Element element = new Element(parts[1].Substring(0, parts[1].IndexOf('(')), parts[1].Substring(parts[1].IndexOf('(') + 1, parts[1].LastIndexOf(')') - parts[1].IndexOf('(') - 1));
                        elements.Add(parts[0], element);
                    }
                    line = sr.ReadLine();
                }

                ConeProcess(elements);

                sr.Close();
                Console.ReadLine();

            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                Console.ReadLine();
            }
            finally
            {
            }
        }
    }
}
