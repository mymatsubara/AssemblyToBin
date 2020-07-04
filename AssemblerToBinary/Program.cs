using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AssemblerToBinary
{
    class Program
    {
        static void Main(string[] args)
        {
            //string filePath = @"C:\Users\xdmur\Desktop\Faculdade\5º Ano - 1º Semetre\Arquitetura e Organização de Computadores\Trabalho\Sort.txt";
            string filePath = args[0];
            string outputPath =  $"NewBinary_{Path.GetFileName(filePath)}";

            var sb = new StringBuilder();
            string fileContent = File.ReadAllText(filePath);

            foreach(var match in Regex.Matches(fileContent, @"\s{2}.*;"))
            {
                sb.Append(IntructionToBin(match.ToString()) + "\n");
            }

            File.WriteAllText(outputPath, sb.ToString());
        }

        private static string IntructionToBin(string instruction)
        {
            string[] instrParts = instruction.Trim(';').Trim().Split(' ').Where(s => !string.IsNullOrEmpty(s) && !s.StartsWith("[")).ToArray();
            string result = "";
            switch (instrParts[0])
            {
                case "nop":
                    result += "00000000000000000000000000000000";
                    break;
                case "add":
                    result += RInstrunction(32, instrParts);
                    break;
                case "slt":
                    result += RInstrunction(42, instrParts);
                    break;
                case "jr":
                    result += $"000000{IntToBin(instrParts[1], 5)}000000000000000001000";
                    break;
                case "addu":
                    result += RInstrunction(33, instrParts);
                    break;
                case "sll":
                    result += RInstrunction(0, instrParts);
                    break;
                case "lw":
                    result += MemInstuction(35, instrParts);
                    break;
                case "sw":
                    result += MemInstuction(43, instrParts);
                    break;
                case "addi":
                    result += IInstruction(8, instrParts);
                    break;
                case "beq":
                    result += IInstruction(4, instrParts);
                    break;
                case "slti":
                    result += IInstruction(10, instrParts);
                    break;
                case "bne":
                    result += IInstruction(5, instrParts);
                    break;
                case "jal":
                    result += JInstruction(3, instrParts);
                    break;
                case "j":
                    result += JInstruction(2, instrParts);
                    break;
                default:
                    result += "NOT IMPLEMENTED";
                    break;
            }
            return result + " - " + string.Join(' ', instrParts);
        }
        private static string IntToBin(string value, int bits)
        {
            if (bits <= 16)
                return Convert.ToString(Int16.Parse(value.Replace("$", "").Replace(",", "")), 2).PadLeft(bits, '0');
            return Convert.ToString(Int32.Parse(value.Replace("$", "").Replace(",", "")), 2).PadLeft(bits, '0');
        }

        private static string RInstrunction(int funct, string[] instrParts)
        {
            string instruction = "";

            // sll
            if (funct == 0)
                return $"00000000000{IntToBin(instrParts[2], 5)}{IntToBin(instrParts[1], 5)}{IntToBin(instrParts[3], 5)}000000";

            // cop
            instruction += "000000";

            // rs
            instruction += IntToBin(instrParts[2], 5);

            // rt
            instruction += IntToBin(instrParts[3], 5);

            // rd
            instruction += IntToBin(instrParts[1], 5);

            // Shamt
            instruction += "00000";

            // Func
            instruction += Convert.ToString(funct, 2).PadLeft(5, '0');

            return instruction;
        }

        private static string MemInstuction(int cop, string[] instrParts)
        {
            string result = "";
            // fazendo parse de addr($rs)
            var matches = Regex.Matches(instrParts[2], @"\d*").Where(m => !string.IsNullOrEmpty(m.Value)).ToArray();

            // cop
            result += Convert.ToString(cop, 2).PadLeft(6, '0');

            // rs
            result += IntToBin(matches[1].ToString(), 5);

            //rt
            result += IntToBin(instrParts[1], 5);

            // addr
            result += IntToBin(matches[0].ToString(), 16);

            return result;
        }

        private static string IInstruction(int cop, string[] instrParts)
        {
            string result = "";

            // cop
            result += Convert.ToString(cop, 2).PadLeft(6, '0');

            // rs
            result += IntToBin(instrParts[1], 5);

            // rt
            result += IntToBin(instrParts[2], 5);

            // imm
            result += IntToBin(instrParts[3], 16);

            return result;
        }

        private static string JInstruction(int cop, string[] instrParts)
        {
            string result = "";

            // cop
            result += Convert.ToString(cop, 2).PadLeft(6, '0');

            // jal
            result += Convert.ToString(Convert.ToInt32(instrParts[1], 16), 2).PadLeft(26, '0');

            return result;
        }
    }
}
