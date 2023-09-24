using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Encodings;
using static MVCLab2.Controller;
using static MVCLab2.Model;
using System.Text.Encodings.Web;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text;

namespace MVCLab2
{
    class View
    {
        public class CompanyConverter : JsonConverter<List<string>>
        {
            public override List<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            public override void Write(Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
            {
                // Escribir las compañías directamente sin escapar los caracteres de control
                writer.WriteStartArray();
                foreach (var company in value)
                {
                    writer.WriteStringValue(company);
                }
                writer.WriteEndArray();
            }
        }
          public class LZ77
        {
            public static List<Tuple<int, int, char>> Encode(string input)
            {
                List<Tuple<int, int, char>> encodedData = new List<Tuple<int, int, char>>();
                int currentIndex = 0;

                while (currentIndex < input.Length)
                {
                    int maxLength = 0;
                    int maxOffset = 0;
                    char nextChar = input[currentIndex];

                    for (int searchOffset = 1; searchOffset <= currentIndex; searchOffset++)
                    {
                        int matchingLength = 0;
                        while (currentIndex + matchingLength < input.Length && input[currentIndex + matchingLength] == input[currentIndex - searchOffset + matchingLength])
                        {
                            matchingLength++;
                        }

                        if (matchingLength > maxLength)
                        {
                            maxLength = matchingLength;
                            maxOffset = searchOffset;
                            if (currentIndex + matchingLength < input.Length)
                            {
                                nextChar = input[currentIndex + matchingLength];
                            }
                            else
                            {
                                // Si estás al final de la cadena, asigna un valor especial o maneja el caso según sea necesario.
                                nextChar = '\0'; // Puedes asignar cualquier valor deseado o lanzar una excepción aquí.
                            }
                        }
                    }

                    encodedData.Add(new Tuple<int, int, char>(maxOffset, maxLength, nextChar));
                    currentIndex += maxLength + 1;
                }

                return encodedData;
            }

            public static string Decode(List<Tuple<int, int, char>> encodedData)
            {
                StringBuilder decodedString = new StringBuilder();

                foreach (var tuple in encodedData)
                {
                    int offset = tuple.Item1;
                    int length = tuple.Item2;
                    char nextChar = tuple.Item3;

                    if (offset == 0)
                    {
                        decodedString.Append(nextChar);
                    }
                    else
                    {
                        int startIndex = decodedString.Length - offset;
                        for (int i = 0; i < length; i++)
                        {
                            if (startIndex + i >= 0 && startIndex + i < decodedString.Length)
                            {
                                decodedString.Append(decodedString[startIndex + i]);
                            }
                            else
                            {
                                // Si estás fuera de los límites de la cadena, puedes manejar el caso según sea necesario.
                                // Por ejemplo, podrías lanzar una excepción o agregar un carácter especial.
                                decodedString.Append(' '); // Agregar un espacio en blanco en caso de fuera de límites
                            }
                        }
                        decodedString.Append(nextChar);
                    }
                }

                return decodedString.ToString();
            }
        }
        public static void Mostrar()
        {

            AVLTree arbolPersonas = new AVLTree();
            List<Persona> personas = new List<Persona>();

            string route = @"C:\Users\usuario\input.csv";

            if (File.Exists(route))
            {
                string[] FileData = File.ReadAllLines(route);
                foreach (var item in FileData)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        string[] valor = item.Split(";");

                        Persona persona = JsonSerializer.Deserialize<Persona>(valor[1]);

                        if (valor[0] == "INSERT")
                        {
                            arbolPersonas.Add(persona);
                        }
                        else if (valor[0] == "DELETE")
                        {
                            arbolPersonas.Delete(persona);
                        }
                        else if (valor[0] == "PATCH")
                        {
                            arbolPersonas.Patch(persona);
                        }
                    }
                }




                personas.Clear();
                Console.WriteLine("");

                List<Persona> todasLasPersonas = arbolPersonas.GetAllPersons();
                Console.WriteLine("Lista completa de personas codificadas en el árbol:");

                var options = new JsonSerializerOptions();
                options.Converters.Add(new CompanyConverter());

                foreach (var persona in todasLasPersonas)
                {
                    // Codificar las compañías utilizando LZ77
                    List<Tuple<int, int, char>> companiesEncoded = LZ77.Encode(string.Join(",", persona.companies));

                    // Crear una lista de strings a partir de la codificación
                    List<string> encodedCompanyList = new List<string>();
                    foreach (var tuple in companiesEncoded)
                    {
                        encodedCompanyList.Add($"({tuple.Item1},{tuple.Item2},{tuple.Item3})");
                    }

                    // Actualizar el valor de compañías en la persona
                    persona.companies = encodedCompanyList;

                    // Limpiar caracteres de control en la lista de compañías
                    persona.companies = persona.companies.Select(company => new string(company.Where(c => !char.IsControl(c)).ToArray())).ToList();

                    // Serializar el objeto Persona con las opciones personalizadas
                    string serializedPersona = JsonSerializer.Serialize(persona, options);

                    Console.WriteLine("");
                    Console.WriteLine(serializedPersona);
                }

                Console.WriteLine("");

                Console.WriteLine("----------------------------------------------------------------------");


                foreach (var item in personas)
                {
                    // Decodificar las compañías utilizando LZ77
                    string encodedCompanies = string.Join("", item.companies);
                    List<Tuple<int, int, char>> companiesDecoded = new List<Tuple<int, int, char>>();
                    int index = 0;

                    while (index < encodedCompanies.Length)
                    {
                        if (encodedCompanies[index] == '(')
                        {
                            int endIndex = encodedCompanies.IndexOf(')', index + 1);
                            if (endIndex != -1)
                            {
                                string tupleStr = encodedCompanies.Substring(index + 1, endIndex - index - 1);
                                string[] tupleParts = tupleStr.Split(',');
                                if (tupleParts.Length == 3)
                                {
                                    if (int.TryParse(tupleParts[0], out int offset) &&
                                        int.TryParse(tupleParts[1], out int length))
                                    {
                                        char nextChar = tupleParts[2][0];
                                        companiesDecoded.Add(new Tuple<int, int, char>(offset, length, nextChar));
                                    }
                                }
                            }
                            index = endIndex + 1;
                        }
                        else
                        {
                            // Si no encuentra un '(' en la posición actual, avanza al siguiente caracter
                            index++;
                        }
                    }

                    // Decodificar la lista de tuplas y actualizar el valor de compañías en la persona
                    string decodedCompanies = LZ77.Decode(companiesDecoded);
                    item.companies.Clear();
                    item.companies.Add(decodedCompanies);

                    Console.WriteLine(JsonSerializer.Serialize<Persona>(item));
                    Console.WriteLine("");

                }

                Console.Write("Ingrese el DPI del cliente que desea buscar: ");
                string dpiABuscar = Console.ReadLine();

                List<Persona> resultados = new List<Persona>();
                arbolPersonas.QueryResults(arbolPersonas.Root, new Persona { dpi = dpiABuscar }, resultados);

                // ...

                // ...

                if (resultados.Count > 0)
                {
                    Console.WriteLine("Resultados de la búsqueda:");

                    foreach (var persona in resultados)
                    {
                        List<Tuple<int, int, char>> companiesDecoded = new List<Tuple<int, int, char>>();
                        foreach (var encodedCompany in persona.companies)
                        {
                            int index = 0;
                            while (index < encodedCompany.Length)
                            {
                                if (encodedCompany[index] == '(')
                                {
                                    int endIndex = encodedCompany.IndexOf(')', index + 1);
                                    if (endIndex != -1)
                                    {
                                        string tupleStr = encodedCompany.Substring(index + 1, endIndex - index - 1);
                                        string[] tupleParts = tupleStr.Split(',');
                                        if (tupleParts.Length == 3)
                                        {
                                            if (int.TryParse(tupleParts[0], out int offset) &&
                                                int.TryParse(tupleParts[1], out int length))
                                            {
                                                if (tupleParts[2].Length > 0)
                                                {
                                                    char nextChar = tupleParts[2][0];
                                                    companiesDecoded.Add(new Tuple<int, int, char>(offset, length, nextChar));
                                                }
                                            }
                                        }
                                    }
                                    index = endIndex + 1;
                                }
                                else
                                {
                                    // Si no encuentra un '(' en la posición actual, avanza al siguiente caracter
                                    index++;
                                }
                            }
                        }

                        string decodedCompanies = LZ77.Decode(companiesDecoded);

                        // Actualizar el valor de las compañías decodificadas en persona.companies
                        persona.companies.Clear();
                        persona.companies.Add(decodedCompanies);

                        // Mostrar la información de la persona con las compañías decodificadas
                        Console.WriteLine(JsonSerializer.Serialize(persona));
                        Console.WriteLine("");
                    }
                }

                // ...

            }
        }
    }
}
