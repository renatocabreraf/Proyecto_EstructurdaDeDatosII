using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlmacenEbenEzer
{
    public class SDES
    {
        List<byte> k1 = new List<byte>();
        List<byte> k2 = new List<byte>();
        List<byte> key = new List<byte>();
        string[,] SBox0 = new string[4, 4];
        string[,] SBox1 = new string[4, 4];

        public SDES() { }

        public SDES(string keyString)
        {
            //S0
            SBox0[0, 0] = "01";
            SBox0[0, 1] = "00";
            SBox0[0, 2] = "11";
            SBox0[0, 3] = "10";

            SBox0[1, 0] = "11";
            SBox0[1, 1] = "10";
            SBox0[1, 2] = "01";
            SBox0[1, 3] = "00";

            SBox0[2, 0] = "00";
            SBox0[2, 1] = "10";
            SBox0[2, 2] = "01";
            SBox0[2, 3] = "11";

            SBox0[3, 0] = "11";
            SBox0[3, 1] = "01";
            SBox0[3, 2] = "11";
            SBox0[3, 3] = "10";

            //S1
            SBox1[0, 0] = "00";
            SBox1[0, 1] = "01";
            SBox1[0, 2] = "10";
            SBox1[0, 3] = "11";

            SBox1[1, 0] = "10";
            SBox1[1, 1] = "00";
            SBox1[1, 2] = "01";
            SBox1[1, 3] = "11";

            SBox1[2, 0] = "11";
            SBox1[2, 1] = "00";
            SBox1[2, 2] = "01";
            SBox1[2, 3] = "00";

            SBox1[3, 0] = "10";
            SBox1[3, 1] = "01";
            SBox1[3, 2] = "00";
            SBox1[3, 3] = "11";

            generateKeys(keyString);
        }

        public List<byte> P10(List<byte> input)
        {
            List<byte> response = new List<byte>();
            //E	1	2	3	4	5	6	7	8	9	10
            //S	3	5	2	7	4	10	1	9	8	6
            response.Add(input[2]);
            response.Add(input[4]);
            response.Add(input[1]);
            response.Add(input[6]);
            response.Add(input[3]);
            response.Add(input[9]);
            response.Add(input[0]);
            response.Add(input[8]);
            response.Add(input[7]);
            response.Add(input[5]);

            return response;
        }

        public List<byte> P8(List<byte> left, List<byte> rigth)
        {
            List<byte> aux = new List<byte>();
            List<byte> response = new List<byte>();
            aux.AddRange(left);
            aux.AddRange(rigth);

            //E   1   2   3   4   5   6   7   8   9   10
            //S   6   3   7   4   8   5   10  9
            response.Add(aux[5]);
            response.Add(aux[2]);
            response.Add(aux[6]);
            response.Add(aux[3]);
            response.Add(aux[7]);
            response.Add(aux[4]);
            response.Add(aux[9]);
            response.Add(aux[8]);

            return response;
        }

        public List<byte> P4(List<byte> input)
        {
            List<byte> response = new List<byte>();

            //E	1	2	3	4
            //S	2	4	3	1
            response.Add(input[1]);
            response.Add(input[3]);
            response.Add(input[2]);
            response.Add(input[0]);

            return response;
        }

        public List<byte> EnP(List<byte> input)
        {
            List<byte> response = new List<byte>();

            //E	1	2	3	4				
            //S 4   1   2   3   2   3   4   1
            response.Add(input[3]);
            response.Add(input[0]);
            response.Add(input[1]);
            response.Add(input[2]);
            response.Add(input[1]);
            response.Add(input[2]);
            response.Add(input[3]);
            response.Add(input[0]);

            return response;
        }

        public List<byte> IP(List<byte> input)
        {
            List<byte> response = new List<byte>();

            //E   1   2   3   4   5   6   7   8
            //S   2   6   3   1   4   8   5   7
            response.Add(input[1]);
            response.Add(input[5]);
            response.Add(input[2]);
            response.Add(input[0]);
            response.Add(input[3]);
            response.Add(input[7]);
            response.Add(input[4]);
            response.Add(input[6]);

            return response;
        }

        public List<byte> LS1(List<byte> input)
        {
            byte aux = input[0];
            input.RemoveAt(0);
            input.Add(aux);
            return input;
        }

        public void generateKeys(string keyString)
        {
            if (keyString != "") // si el usuario ingreso la clave
            {
                this.key = keyString.Select(c => Convert.ToByte(c.ToString())).ToList();
            }
            else
            {
                keyString = "1001100110";
                this.key = keyString.Select(c => Convert.ToByte(c.ToString())).ToList();
            }

            List<byte> auxiliarKey = P10(key);
            //extraer las mitades 
            List<byte> left = new List<byte>();
            List<byte> rigth = new List<byte>();

            for (int i = 0; i < key.Count; i++)
            {
                if (i < 5)
                {
                    left.Add(auxiliarKey[i]);
                }
                else
                {
                    rigth.Add(auxiliarKey[i]);
                }
            }

            left = LS1(left);
            rigth = LS1(rigth);

            this.k1 = P8(left, rigth);

            left = LS1(left);
            left = LS1(left);
            rigth = LS1(rigth);
            rigth = LS1(rigth);

            this.k2 = P8(left, rigth);
        }

        public List<byte> searchSBoxes(List<byte> input)
        {
            List<byte> response = new List<byte>();
            List<byte> left = new List<byte>();
            List<byte> rigth = new List<byte>();

            for (int i = 0; i < input.Count; i++)
            {
                if (i < 4)
                {
                    left.Add(input[i]);
                }
                else
                {
                    rigth.Add(input[i]);
                }
            }

            int extreme = Convert.ToInt32((left[0].ToString() + left[3].ToString()), 2);
            int medium = Convert.ToInt32((left[1].ToString() + left[2].ToString()), 2);
            string s0 = SBox0[extreme, medium];

            extreme = Convert.ToInt32((rigth[0].ToString() + rigth[3].ToString()), 2);
            medium = Convert.ToInt32((rigth[1].ToString() + rigth[2].ToString()), 2);
            string s1 = SBox1[extreme, medium];

            string binary = s0 + s1;
            response = binary.Select(c => Convert.ToByte(c.ToString())).ToList();

            return response;
        }

        public List<byte> XOR(List<byte> input, List<byte> key)
        {
            List<byte> response = new List<byte>();

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] == key[i])
                {
                    response.Add(0);
                }
                else
                {
                    response.Add(1);
                }
            }

            return response;
        }

        public List<byte> fk(List<byte> input, List<byte> key)
        {
            List<byte> response = new List<byte>();
            List<byte> left = new List<byte>();
            List<byte> rigth = new List<byte>();

            for (int i = 0; i < input.Count; i++)
            {
                if (i < 4)
                {
                    left.Add(input[i]);
                }
                else
                {
                    rigth.Add(input[i]);
                }
            }

            response = EnP(rigth);
            response = XOR(response, key);
            response = searchSBoxes(response);
            response = P4(response);
            response = XOR(response, left);
            response.AddRange(rigth);

            return response;
        }

        public List<byte> Switch(List<byte> input)
        {
            List<byte> left = new List<byte>();
            List<byte> rigth = new List<byte>();
            for (int i = 0; i < input.Count; i++)
            {
                if (i < 4)
                {
                    left.Add(input[i]);
                }
                else
                {
                    rigth.Add(input[i]);
                }
            }

            input = new List<byte>();
            input.AddRange(rigth);
            input.AddRange(left);

            return input;
        }

        public List<byte> IP_1(List<byte> input)
        {
            List<byte> response = new List<byte>();

            //E   1   2   3   4   5   6   7   8
            //S   2   6   3   1   4   8   5   7
            response.Add(input[3]);
            response.Add(input[0]);
            response.Add(input[2]);
            response.Add(input[4]);
            response.Add(input[6]);
            response.Add(input[1]);
            response.Add(input[7]);
            response.Add(input[5]);

            return response;
        }

        private char cipherChar(char input)
        {
            List<byte> response = new List<byte>();
            string auxiliar = "";

            string binary = Convert.ToString(input, 2);
            binary = binary.PadLeft(8, '0');
            List<byte> sequence = binary.Select(c => Convert.ToByte(c.ToString())).ToList();

            sequence = IP(sequence);
            sequence = fk(sequence, k1);
            sequence = Switch(sequence);
            sequence = fk(sequence, k2);
            response = IP_1(sequence);

            auxiliar = string.Join("", response);
            int auxInt = Convert.ToInt32(auxiliar, 2);

            return Convert.ToChar((byte)auxInt);
        }

        private char decipherChar(char input)
        {
            List<byte> response = new List<byte>();
            string auxiliar = "";

            string binary = Convert.ToString(input, 2);
            binary = binary.PadLeft(8, '0');
            List<byte> sequence = binary.Select(c => Convert.ToByte(c.ToString())).ToList();

            sequence = IP(sequence);
            sequence = fk(sequence, k2);
            sequence = Switch(sequence);
            sequence = fk(sequence, k1);
            response = IP_1(sequence);

            auxiliar = string.Join("", response);
            int auxInt = Convert.ToInt32(auxiliar, 2);

            return Convert.ToChar((byte)auxInt);
        }

        public string cipher(string param)
        {
            string response = "";

            for (int i = 0; i < param.Length; i++)
            {
                response += cipherChar(param[i]);
            }

            return response;
        }

        public string decipher(string param)
        {
            string response = "";

            for (int i = 0; i < param.Length; i++)
            {
                response += decipherChar(param[i]);
            }

            return response;
        }
    }
}