using DynamicPatcher;
using PatcherYRpp;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GeneralHooks
{
    public class CSFLoader
    {
        readonly Pointer<CSFLabel> Labels;
        readonly Pointer<int> LabelCount;

        readonly Pointer<IntPtr> Values;
        readonly Pointer<int> ValueCount;
        readonly Dictionary<string, Pointer<CSFLabel>> Map;

        CSFLoader(Pointer<CSFLabel> labels, Pointer<int> labelCount, Pointer<IntPtr> values, Pointer<int> valueCount)
        {
            Dictionary<string, Pointer<CSFLabel>> map = new(labelCount.Ref);
            for (int i = 0; i < labelCount.Ref; i++)
            {
                map[labels[i].Name.ToString()] = labels + i;
            }
            Map = map;
            Labels = labels;
            LabelCount = labelCount;
            Values = values;
            ValueCount = valueCount;
        }

        unsafe string this[string key]
        {
            get
            {
                char* pStr;
                if (Map.TryGetValue(key, out var value) && (int)(pStr = (char*)Values[value.Ref.FirstValueIndex]) != 0)
                {
                    return new string(pStr);
                }
                return null;
            }
            set
            {
                if (Map.TryGetValue(key, out var label))
                {
                    Pointer<IntPtr> pStr = (IntPtr)(Values + label.Ref.FirstValueIndex);

                    if (IntPtr.Zero != pStr.Ref)
                    {
                        YRMemory.Deallocate(pStr.Ref);
                    }

                    pStr.Ref = UniString(value);

                }
                else
                {
                    int index = ValueCount.Ref;
                    int labelindex = LabelCount.Ref;

                    label = YRMemory.Allocate(40);
                    Pointer<byte> pName = (IntPtr)label;


                    int i;
                    for (i = 0; i < key.Length; i++)
                    {
                        pName[i] = (byte)key[i];
                    }
                    label.Ref.FirstValueIndex = index;
                    (Values + index).Ref = UniString(value);

                    Labels[labelindex] = label.Ref;
                    Map[key] = label;
                    ValueCount.Ref++;
                    LabelCount.Ref++;
                }
            }
        }

        IntPtr UniString(string str)
        {
            Pointer<char> ptr =  YRMemory.Allocate((uint)((str.Length + 1) * 2));
            int i;
            for (i = 0; i < str.Length; i++)
            {
                ptr[i] = str[i];
            }
            ptr[i] = '\0';
            return ptr;
        }

        void Sort()
        {
            QSort(Labels, ValueCount.Ref, 40, new IntPtr(0x7C8D20));
            unsafe static void QSort(IntPtr pBase, int num, int size, IntPtr cmp)
            {
                var func = (delegate* unmanaged[Cdecl]<IntPtr, int, int, IntPtr, void>)0x7C8B48;
                func(pBase, num, size, cmp);
            }
        }



        [Hook(HookType.AresHook, Address = 0x6BD886, Size = 5)]
        public static unsafe UInt32 CSF_LoadExtraFiles(REGISTERS* R)
        {
            const string fileName = "lcstring{0:00}.ecs";


            CSFLoader loader = new(new Pointer<Pointer<CSFLabel>>(0xB1CF74).Ref, new Pointer<int>(0xB1CF6C), new Pointer<IntPtr>(0xB1CF78).Ref, new Pointer<int>(0xB1CF70));

            for (int i = 0; i < 10; i++)
            {
                LoadExtraFiles(string.Format(fileName, i), loader);
            }
            loader.Sort();
            return 0;
        }



        [StructLayout(LayoutKind.Explicit, Size = 40)]
        private struct CSFLabel
        {
            [FieldOffset(0)] private byte f_name; //limits the label name length to 31
            public AnsiStringPointer Name => Pointer<byte>.AsPointer(ref f_name);
            [FieldOffset(32)] public int NumValues; //one label can have multiple values attached, that's never used though
            [FieldOffset(36)] public int FirstValueIndex; //in the global StringTable::Values() array
        };

        static unsafe string ReadTextFileUTF8(string file)
        {
            string text = null;
            var pFile = YRMemory.Allocate<CCFileClass>().Construct(file);

            if (pFile.IsNotNull)
            {
                IntPtr ptr;
                if (pFile.Ref.Exists() && IntPtr.Zero != (ptr = pFile.Ref.ReadWholeFile()))
                {

                    text = Encoding.UTF8.GetString((byte*)ptr, pFile.Ref.FileSize);
                }

                YRMemory.Delete(pFile);
            }
            return text;
        }

        static void LoadExtraFiles(string file, CSFLoader loader)
        {
            string text;
            if(null != (text = ReadTextFileUTF8(file)))
            {
#if ECSDEBUG
                Console.WriteLine(text);
#endif
                Dictionary<string, string> map = GetCSFPairs(text);
                foreach (var p in map)
                {
                    loader[p.Key] = p.Value;
#if ECSDEBUG
                    Console.WriteLine(loader[p.Key]);
                    Console.WriteLine("---------------------");
                    Console.WriteLine("key:");
                    Console.WriteLine(p.Key);
                    Console.WriteLine(p.Key.Length);
                    Console.WriteLine();
                    Console.WriteLine("value:");
                    Console.WriteLine(p.Value);
                    Console.WriteLine("---------------------");
#endif
                }

               Logger.Log($"读取可编辑CSF文件 {file} 中的{map.Count}对键值");
            }
        }
        static Dictionary<string, string> GetCSFPairs(string text)
        {
            var stream = new TextSteam(text);

            Dictionary<string, string> map = new Dictionary<string, string>();


            string key = default;
            StringBuilder value = new StringBuilder();


            while (true)
            {
                if (stream.Match('#'))
                {
                    if (!stream.ReadLine(out _))
                    {
                        break;
                    }
                }
                else if (stream.Match2(' ', ' '))
                {
                    if (stream.Move(2) && stream.ReadLine(out var range))
                    {
                        if (key is not null)
                        {
                            stream.Append(value, range);
                        }

                    }
                    else
                    {
                        if (key is not null)
                        {
                            map[key] = value.ToString();
                            key = null;
                            value.Clear();
                        }

                        break;
                    }
                }
                else if (stream.Match('\n'))
                {
                    if (!stream.Move(1))
                    {
                        if (key is not null)
                        {
                            map[key] = value.ToString();
                            key = null;
                            value.Clear();
                        }

                        break;

                    }
                }
                else if (stream.Match2('\r', '\n'))
                {
                    if (!stream.Move(2))
                    {
                        if (key is not null)
                        {
                            map[key] = value.ToString();
                            key = null;
                            value.Clear();
                        }

                        break;

                    }

                }
                else
                {
                    if (stream.ReadBefore('>', out var range))
                    {
                        if (key is not null)
                        {
                            map[key] = value.ToString();
                            key = null;
                            value.Clear();
                        }
                        key = stream.GetString(range);

                        if (stream.Match2('>', ' '))
                        {
                            if (stream.Move(2) && stream.ReadLine(out var valueRange))
                            {
                                stream.Append(value, valueRange);
                                //value.Append(stream.String, valueRange.Start, valueRange.Count);
                            }
                            else
                            {
                                if (key is not null)
                                {
                                    map[key] = value.ToString();
                                    key = null;
                                    value.Clear();
                                }


                                break;
                            }
                        }
                        else if (stream.Match2('>', '>'))
                        {
                            if (!stream.Move(2) || !stream.ReadLine(out _))
                            {
                                if (key is not null)
                                {
                                    map[key] = value.ToString();
                                    key = null;
                                    value.Clear();
                                }


                                break;
                            }

                        }
                        else
                        {
                            stream.ReadLine(out _);
                        }
                    }
                    else
                    {
                        if (!stream.ReadLine(out _))
                        {


                            if (key is not null)
                            {
                                map[key] = value.ToString();
                                key = null;
                                value.Clear();
                            }


                            break;
                        }
                    }
                }
            }

            return map;
        }

        class TextSteam
        {
            readonly string Text;
            readonly int Length;
            int Position;
            public string String => Text;
            public TextSteam(string text)
            {
                Text = text;
                Length = text.Length;
            }

            public bool ReadChar(out char value)
            {
                value = default;
                if (Position >= Length)
                {
                    return false;
                }

                value = Text[Position];
                Position++;
                return true;
            }

            public bool Read(int length, out TextRange range)
            {
                range = default;
                if (Position >= Length)
                {
                    return false;
                }
                int startCount = Position;

                int endCount = Position + length;

                if (endCount >= Length)
                {
                    Position = Length;
                    range = new TextRange(startCount, Position - startCount);
                    return true;
                }

                Position = endCount;
                range = new TextRange(startCount, length);

                return true;
            }

            public bool ReadUntil(char target, out TextRange range)
            {
                range = default;
                if (Position >= Length)
                {
                    return false;
                }

                range.Start = Position;

                int i;
                for (i = Position; i < Length; i++)
                {
                    if (Text[i] == target)
                    {
                        i++;
                        break;
                    }
                }
                Position = i;
                range.Count = i - range.Start;
                return true;
            }

            public bool ReadBefore(char target, out TextRange range)
            {
                range = default;
                if (Position >= Length)
                {
                    return false;
                }

                range.Start = Position;

                int i;
                for (i = Position; i < Length; i++)
                {
                    if (Text[i] == target)
                    {
                        break;
                    }
                }
                Position = i;
                range.Count = i - range.Start;
                return true;
            }

            public bool ReadLine(out TextRange range) => ReadUntil('\n', out range);


            public bool Match(char target)
            {
                if (Position >= Length)
                {
                    return false;
                }
                return target == Text[Position];
            }

            public bool Match2(char target1, char target2)
            {
                if (Position >= Length || Position + 1 >= Length)
                {
                    return false;
                }
                return target1 == Text[Position] && target2 == Text[Position + 1];
            }

            public string GetString(TextRange range)
            {
                return Text.Substring(range.Start, range.Count);
            }

            public bool Move(int offset)
            {
                if (Position >= Length)
                {
                    return false;
                }
                int index = Position + offset;
                if (index < 0 || index >= Length)
                {
                    return false;
                }
                Position = index;
                return true;
            }

            public void Append(StringBuilder builder, TextRange range)
            {
                if (builder.Length != 0)
                {
                    builder.Append('\r').Append('\n').Append(Text, range.Start, range.Count - 2);
                }
                else
                {
                    builder.Append(Text, range.Start, range.Count - 2);
                }
            }

            public struct TextRange
            {
                public TextRange(int start, int count)
                {
                    Start = start;
                    Count = count;
                }

                public int Start;
                public int Count;
            }

        }

    }
}
