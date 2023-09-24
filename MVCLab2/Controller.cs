using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Encodings;

using static MVCLab2.Model;
using System.Text;

namespace MVCLab2
{
    class Controller
    {
        public class Nodo
        {
            public Persona Value { get; set; }
            public Nodo Left { get; set; }
            public Nodo Right { get; set; }
            public int Height { get; set; }

            public Nodo(Persona value)
            {
                Value = value;
                Left = null;
                Right = null;
                Height = 1;
            }
        }

        public class AVLTree
        {
            public Nodo Root;
            public AVLTree()
            {
                Root = null;
            }

            public bool IsEmpty()
            {
                return Root == null;
            }

            public int GetBalance(Nodo n1)
            {
                if (n1.Left == null && n1.Right == null)
                {
                    return 0;
                }
                else if (n1.Left == null)
                {
                    return -n1.Right.Height;
                }
                else if (n1.Right == null)
                {
                    return n1.Left.Height;
                }
                else
                {
                    return n1.Left.Height - n1.Right.Height;
                }
            }

            private void SetHeight(Nodo nodo)
            {
                if (nodo.Left == null || nodo.Right == null)
                {
                    if (nodo.Left == null && nodo.Right == null)
                    {
                        nodo.Height = 1;
                    }
                    else if (nodo.Left == null)
                    {
                        nodo.Height = 1 + nodo.Right.Height;
                    }
                    else
                    {
                        nodo.Height = 1 + nodo.Left.Height;
                    }
                }
                else if (nodo.Left.Height > nodo.Right.Height)
                {
                    nodo.Height = 1 + nodo.Left.Height;
                }
                else
                {
                    nodo.Height = 1 + nodo.Right.Height;
                }
            }

            public void Add(Persona item)
            {
                Root = AddInAVL(Root, item);
            }

            private Nodo AddInAVL(Nodo nodo, Persona item)
            {
                if (nodo == null)
                {
                    nodo = new Nodo(item);
                }
                else
                {
                    int comparison = item.name.CompareTo(nodo.Value.name);
                    if (comparison < 0)
                    {
                        nodo.Left = AddInAVL(nodo.Left, item);
                    }
                    else if (comparison > 0)
                    {
                        nodo.Right = AddInAVL(nodo.Right, item);
                    }
                }

                nodo = ReBalance(nodo);
                return nodo;
            }

            public void Delete(Persona item)
            {
                Root = DeleteInAVL(Root, item);
            }

            private Nodo DeleteInAVL(Nodo nodo, Persona item)
            {
                if (nodo == null)
                {
                    return nodo;
                }

                int comparison = item.name.CompareTo(nodo.Value.name);

                if (comparison < 0)
                {
                    nodo.Left = DeleteInAVL(nodo.Left, item);
                }
                else if (comparison > 0)
                {
                    nodo.Right = DeleteInAVL(nodo.Right, item);
                }
                else
                {
                    if (nodo.Left == null && nodo.Right == null)
                    {
                        nodo = null;
                        return nodo;
                    }
                    else if (nodo.Left == null && nodo.Right != null)
                    {
                        Nodo temp = nodo.Right;
                        nodo.Right = null;
                        nodo = temp;
                    }
                    else if (nodo.Left != null && nodo.Right == null)
                    {
                        Nodo temp = nodo.Left;
                        nodo.Left = null;
                        nodo = temp;
                    }
                    else
                    {
                        Nodo temp = RightestfromLeft(nodo.Left);
                        nodo.Value = temp.Value;
                        nodo.Left = DeleteInAVL(nodo.Left, temp.Value);
                    }
                }

                nodo = ReBalance(nodo);
                return nodo;
            }

            public void Patch(Persona item)
            {
                if (item == null)
                {
                    return;
                }
                if (IsEmpty())
                {
                    return;
                }
                bool flag = false;
                Nodo temporal = this.Root;
                while (temporal != null && flag != true)
                {
                    int comparison = item.name.CompareTo(temporal.Value.name);
                    if (comparison < 0)
                    {
                        temporal = temporal.Left;
                    }
                    else if (comparison > 0)
                    {
                        temporal = temporal.Right;
                    }
                    else
                    {
                        flag = true;
                    }
                }
                if (temporal != null)
                {
                    temporal.Value = item;
                }
            }

            public void QueryResults(Nodo temporal, Persona item, List<Persona> Results)
            {
                if (temporal == null)
                {
                    return;
                }
                if (temporal.Left != null)
                {
                    QueryResults(temporal.Left, item, Results);
                }
                if (item.dpi.CompareTo(temporal.Value.dpi) == 0)
                {
                    Results.Add(temporal.Value);
                }
                if (temporal.Right != null)
                {
                    QueryResults(temporal.Right, item, Results);
                }
            }

            private Nodo ReBalance(Nodo nodo)
            {
                SetHeight(nodo);
                int FE = GetBalance(nodo);
                if (FE < -1)
                {
                    if (GetBalance(nodo.Right) < 0)
                    {
                        nodo = LeftRotation(nodo);
                    }
                    else
                    {
                        nodo = DoubleLeftRotation(nodo);
                    }
                }
                else if (FE > 1)
                {
                    if (GetBalance(nodo.Left) > 0)
                    {
                        nodo = RightRotation(nodo);
                    }
                    else
                    {
                        nodo = DoubleRightRotation(nodo);
                    }
                }

                return nodo;
            }

            private Nodo RightRotation(Nodo nodo)
            {
                Nodo temp = nodo.Left;
                nodo.Left = temp.Right;
                temp.Right = nodo;
                SetHeight(nodo);
                SetHeight(temp);
                return temp;
            }

            private Nodo LeftRotation(Nodo nodo)
            {
                Nodo temp = nodo.Right;
                nodo.Right = temp.Left;
                temp.Left = nodo;
                SetHeight(nodo);
                SetHeight(temp);
                return temp;
            }

            private Nodo DoubleRightRotation(Nodo nodo)
            {
                Nodo temp = nodo.Left;
                temp = LeftRotation(temp);
                nodo.Left = temp;
                nodo = RightRotation(nodo);
                return nodo;
            }

            private Nodo DoubleLeftRotation(Nodo nodo)
            {
                Nodo temp = nodo.Right;
                temp = RightRotation(temp);
                nodo.Right = temp;
                nodo = LeftRotation(nodo);
                return nodo;
            }

            private Nodo RightestfromLeft(Nodo nodo)
            {
                Nodo nodoTemp = nodo;
                while (nodoTemp.Right != null)
                {
                    nodoTemp = nodoTemp.Right;
                }
                return nodoTemp;
            }

            public List<Persona> GetAllPersons()
            {
                List<Persona> allPersons = new List<Persona>();
                GetAllPersonsRecursive(Root, allPersons);
                return allPersons;
            }

            private void GetAllPersonsRecursive(Nodo nodo, List<Persona> allPersons)
            {
                if (nodo == null)
                {
                    return;
                }

                GetAllPersonsRecursive(nodo.Left, allPersons);
                allPersons.Add(nodo.Value);
                GetAllPersonsRecursive(nodo.Right, allPersons);
            }


        }
      
    }
}