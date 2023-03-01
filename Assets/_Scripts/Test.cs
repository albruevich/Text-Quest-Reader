using System.Collections.Generic;
using UnityEngine;
using Z.Expressions;

// можно юзать все операторы
//https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/

//ссылки на Z.Expressions 
//https://dotnetfiddle.net/iKY3h9
//https://www.nuget.org/packages/Z.Expressions.Eval/3.1.10
//https://dotnetfiddle.net/COq6FC
//https://github.com/zzzprojects/Eval-Expression.NET

public class Test : MonoBehaviour
{
    Dictionary<string, object> paramsDict = new Dictionary<string, object>();

    // Start is called before the first frame update
    void Start()
    {
        // Test1();
        // Test2();
        // Test3();
        //Test4();
        //Test5();
        Test6();
    }

    void Test1()
    {
        var code = "X > Y";

        // same string evaluated multiple time
        for (int i = 0; i < 10; i++)
        {
            var variable = new Variable { X = i % 3, Y = i % 4 };
            var result = Eval.Execute(code, variable);

            print(string.Format("x = {0}, y = {1}, r = {2}", variable.X, variable.Y, result));
        }
    }

    public class Variable
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    void Test2()
    {
        // Parameter: Anonymous Type
        int result = Eval.Execute<int>("X + Y", new { X = 1, Y = 2 });

        print(result);
    }

    void Test3()
    {
        // Parameter: Argument Position
        int result = Eval.Execute<int>("{0} + {1}", 1, 2);

        print(result);
    }

    void Test4()
    {
        // Parameter: Dictionary Key
        var code = "X + Y";
        var values = new Dictionary<string, object> { { "X", 1 }, { "Y", 2 } };
        int result = Eval.Execute<int>(code, values);

        print(result);
    }

    void Test5()
    {
        var code = "X / Y";
        float X = 5;
        float Y = 4;
        
        var values = new Dictionary<string, object> { { "X", X }, { "Y", Y } };

        float result = Eval.Execute<float>(code, values);
        print(result);

        int result2 = Eval.Execute<int>(code, values);
        print(result2);
    }

    void Test6()
    {
        var code = "P1 / (P2 + 5)";

        paramsDict.Add("P1", 4f);
        paramsDict.Add("P2", 5f);
        paramsDict.Add("P3", 6f); // параметр, не учавствует в формуле, и все ок

        float result = Eval.Execute<float>(code, paramsDict);
        print(result);        
    }
}