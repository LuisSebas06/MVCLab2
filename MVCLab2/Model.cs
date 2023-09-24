    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Text.Encodings;
    using static MVCLab2.Controller;
    using System.Text.Json;

    namespace MVCLab2
    {

        class Model
        {
            public class Persona
            {
                public string name { get; set; }
                public string dpi { get; set; }
                public string datebirth { get; set; }
                public string address { get; set; }
                public List<string> companies { get; set; }
                //public List<Tuple<int, int, char>> encodedCompanies { get; set; } // Cambio aquí
                //public string decodedCompanies { get; set; } // Cambio aquí

                public Persona()
                {
                  //  encodedCompanies = new List<Tuple<int, int, char>>(); // Cambio aquí

                }
            }
        }
    }
