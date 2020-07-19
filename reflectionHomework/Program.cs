using System;
using System.Reflection;
using System.IO;
using System.Text;


class Program
{
    static void Main(string[] args)
    {
        Console.Write("Укажите название файла(без расширения): ");
        string fileName = Console.ReadLine();
        fileName += ".sql";
        Console.Write("Укажите путь, где хотите сохранить файл: ");
        string pathToFile = Console.ReadLine();
        //pathToFile += "/";
#if _WIN32
        pathToFile += @"\";
#else
        pathToFile += "/";
#endif
        pathToFile += fileName;
        Console.Write("Укажите путь на сборку: ");
        string pathToLibrary = Console.ReadLine();
        Assembly myAssembly = Assembly.LoadFrom(pathToLibrary);
        //Assembly myAssembly = Assembly.LoadFrom("/Users/sanya/csharp_study/csharp_source/STEP_classwork/lesson_20/solution/ClassLibraryProjects/Library/bin/Debug/netstandard2.0/Library.dll");
        string databaseName = myAssembly.FullName;
        string tableName;
        //string[] tablesName = new string[1000];
        string[] tableContent = new string[1000];
        string[] fieldsName = new string[1000];
        string[] fieldsType = new string[1000];
        string[] sqlText = new string[10000];
        int tablesCount = 0;
        int count = 0;
        int fieldsCount = 0;

        for (int i = 0; i < databaseName.Length; i++)
        {
            if (databaseName[i] == ',')
            {
                break;
            }
            count++;
        }
        databaseName = databaseName.Substring(0, count);
        sqlText[0] = "CREATE DATABASE " + databaseName;

        foreach (Type type in myAssembly.GetTypes())
        {
            //tablesName[tablesCount] = type.Name;
            //tablesCount++;
            tableName = type.Name;
            tablesCount++;

            foreach (MemberInfo memberInfo in type.GetMembers())
            {
                if (memberInfo is FieldInfo)
                {
                    var info = memberInfo as FieldInfo;
                    fieldsName[fieldsCount] = info.Name;
                    if (info.FieldType == typeof(System.Int32))
                    {
                        fieldsType[fieldsCount] = "INT";
                    }
                    else if (info.FieldType == typeof(System.String))
                    {
                        fieldsType[fieldsCount] = "NVARCHAR(MAX)";
                    }
                    else if (info.FieldType == typeof(System.Double))
                    {
                        fieldsType[fieldsCount] = "DOUBLE";
                    }
                    else if (info.FieldType == typeof(System.Boolean)) // в SQL нет типа boolean, но я решил, что в этом проекте он будет))
                    {
                        fieldsType[fieldsCount] = "BOOLEAN";
                    }
                    fieldsCount++;
                }

                if (memberInfo is PropertyInfo)
                {
                    var info = memberInfo as PropertyInfo;
                    fieldsName[fieldsCount] = info.Name;
                    if (info.PropertyType == typeof(System.Int32))
                    {
                        fieldsType[fieldsCount] = "INT";
                    }
                    else if (info.PropertyType == typeof(System.String))
                    {
                        fieldsType[fieldsCount] = "NVARCHAR(MAX)";
                    }
                    else if (info.PropertyType == typeof(System.Double))
                    {
                        fieldsType[fieldsCount] = "DOUBLE";
                    }
                    else if (info.PropertyType == typeof(System.Boolean)) // в SQL нет типа boolean, но я решил, что в этом проекте он будет))
                    {
                        fieldsType[fieldsCount] = "BOOLEAN";
                    }
                    fieldsCount++;
                }
            }

            for (int i = 0, j = 0; j < tablesCount; i += 2)
            {
                if (sqlText[i] == null)
                {
                    sqlText[i - 1] = "\n";
                    //sqlText[i] = "CREATE TABLE " + tablesName[j] + "(";
                    sqlText[i] = "CREATE TABLE " + tableName + "(";
                    for (int h = 0; h < fieldsCount; h++)
                    {
                        i += 2;
                        sqlText[i - 1] = "\n";
                        if (fieldsName[h].ToLower() == "id")
                        {
                            sqlText[i] = fieldsName[h] + " " + fieldsType[h] + " IDENTITY" + " NOT NULL " + "PRIMARY KEY" + ",";
                        }
                        else
                        {
                            sqlText[i] = fieldsName[h] + " " + fieldsType[h] + " NOT NULL" + ",";
                        }
                    }
                    j++;
                }
                count = i;
            }

            sqlText[count] += ")\n";

            fieldsCount = 0;
            while (fieldsType[fieldsCount] != null)
            {
                fieldsType[fieldsCount] = null;
                fieldsCount++;
            }
            fieldsCount = 0;

            fieldsCount = 0;
            while (fieldsName[fieldsCount] != null)
            {
                fieldsName[fieldsCount] = null;
                fieldsCount++;
            }
            fieldsCount = 0;
            tablesCount = 0;

            /*tablesCount = 0;
            while (tablesName[tablesCount] != null)
            {
                tablesName[tablesCount] = null;
                tablesCount++;
            }
            tablesCount = 0;*/
        }

        string textToWrite;
        textToWrite = String.Join("", sqlText);
        //Console.WriteLine(textToWrite);
        //Console.WriteLine();

        using (var stream = new FileStream(pathToFile, FileMode.OpenOrCreate))
        {
            var bytes = Encoding.UTF8.GetBytes(textToWrite);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}