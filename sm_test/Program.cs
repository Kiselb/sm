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
        class Point
        {
            public double x;
            public double y;
            public double z;

            public Point(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }
        class Edge
        {
            public Point center;
            public double radius;

            public Point point1;
            public Point point2;
            public Point point3;
            public Point point4;

            public Edge(double x, double y, double z, double r)
            {
                this.center = new Point(x, y, z);
                this.radius = r;

                this.point1 = new Point(0, 0, 0);
                this.point2 = new Point(0, 0, 0);
                this.point3 = new Point(0, 0, 0);
                this.point4 = new Point(0, 0, 0);
            }
        }
        static Edge EdgeProcess(Dictionary<String, Element> elements, Element e)
        {
            Element element = elements[e.parameters[1]];    // edge loop
            element = elements[element.parameters[1]];      // oriented edge
            element = elements[element.parameters[3]];      // edge curve
            element = elements[element.parameters[3]];      // circle
            double radius = double.Parse(element.parameters[2]);
            element = elements[element.parameters[1]];      // axis placement
            element = elements[element.parameters[1]];      // cartesian point

            Edge edge = new Edge(double.Parse(element.parameters[1]), double.Parse(element.parameters[2]), double.Parse(element.parameters[3]), radius);

            edge.point1.x = edge.center.x - edge.radius;
            edge.point1.y = edge.center.y;
            edge.point1.z = edge.center.z;

            edge.point2.x = edge.center.x + edge.radius;
            edge.point2.y = edge.center.y;
            edge.point2.z = edge.center.z;

            edge.point3.x = edge.center.x;
            edge.point3.y = edge.center.y - edge.radius;
            edge.point3.z = edge.center.z;

            edge.point4.x = edge.center.x;
            edge.point4.y = edge.center.y + edge.radius;
            edge.point4.z = edge.center.z;

            return edge;
        }
        static void EdgesProcess(Dictionary<String, Element> elements, Element e1, Element e2)
        {
            Edge edge1 = EdgeProcess(elements, e1);
            Edge edge2 = EdgeProcess(elements, e2);

            Console.WriteLine("Edge 1");
            Console.WriteLine("Point 1: {0} {1} {2}", edge1.point1.x, edge1.point1.y, edge1.point1.z);
            Console.WriteLine("Point 2: {0} {1} {2}", edge1.point2.x, edge1.point2.y, edge1.point2.z);
            Console.WriteLine("Point 3: {0} {1} {2}", edge1.point3.x, edge1.point3.y, edge1.point3.z);
            Console.WriteLine("Point 4: {0} {1} {2}", edge1.point4.x, edge1.point4.y, edge1.point4.z);
            Console.WriteLine("Center: {0} {1} {2}", edge1.center.x, edge1.center.y, edge1.center.z);
            Console.WriteLine("Radius: {0}", edge1.radius);

            Console.WriteLine("Edge 2");
            Console.WriteLine("Point 1: {0} {1} {2}", edge2.point1.x, edge2.point1.y, edge2.point1.z);
            Console.WriteLine("Point 2: {0} {1} {2}", edge2.point2.x, edge2.point2.y, edge2.point2.z);
            Console.WriteLine("Point 3: {0} {1} {2}", edge2.point3.x, edge2.point3.y, edge2.point3.z);
            Console.WriteLine("Point 4: {0} {1} {2}", edge2.point4.x, edge2.point4.y, edge2.point4.z);
            Console.WriteLine("Center: {0} {1} {2}", edge2.center.x, edge2.center.y, edge2.center.z);
            Console.WriteLine("Radius: {0}", edge2.radius);

            Console.WriteLine("Height: {0}", Math.Abs(edge1.center.z - edge2.center.z));
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

                        EdgesProcess(elements, edge1, edge2);

                        //Console.WriteLine("Angle: {0} Height: {1}", angle, height);

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
                StreamReader sr = new StreamReader(@"D:\EDUCATION\SM\ппд 1х3х5 д0,3 с фасками.stp");
                //StreamReader sr = new StreamReader(file_name); // (@"D:\EDUCATION\SM\ппд 1х3х5 д0,3 с фасками.stp");
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
