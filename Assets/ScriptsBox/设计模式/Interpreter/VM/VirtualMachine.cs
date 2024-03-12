using System.Collections.Generic;
using Sirenix.Serialization.Utilities;


namespace ScriptsBox.Interpreter
{
    public enum EInstruction
    {
        InstSetHealth      = 0x00,
        InstSetWisdom      = 0x01,
        InstSetAgility     = 0x02,
        InstPlaySound      = 0x03,
        InstSpawnParticles = 0x04
    };
    public class VirtualMachine
    {
        private const int MaxStack = 128;
        private Stack<int> stack = new Stack<int>(MaxStack);

        private void Push(int value)
        {
            stack.Push(value);
        }

        private int Pop()
        {
            return stack.Pop();
        }


        public void Interpret(char[] bytecode,int size)
        {
            for (int i = 0; i < size; i++)
            {
                char instruction = bytecode[i];
                switch (instruction)
                {
                    case (char)EInstruction.InstPlaySound:
                        break;
                    case (char)EInstruction.InstSetAgility:
                        break;
                    case (char)EInstruction.InstSetHealth:
                        break;
                    case (char)EInstruction.InstSetWisdom:
                        break;
                    case (char)EInstruction.InstSpawnParticles:
                        break;
                }
            }
        }
    }
    
}