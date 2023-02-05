using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;

using TestGeradorCodigo;

namespace TestProject1
{

    [TestClass]
    public class CompileAssemblyTests
    {
        [TestMethod]
        public void CompileClassAndExecute()
        {
            var code = @"
using System;

namespace Testing.Test {

public class Test
{
     public string Name {get; set; } = ""John"";
     public DateTime Time {get; set; } = DateTime.Now;

    public Test()
    {
        
    }

    public string HelloWorld(string name = null)
    {
        if (string.IsNullOrEmpty(name))
           name = Name;

        return ""Hello, "" + Name + "". Time is: "" + Time.ToString(""MMM dd, yyyy"");        
    } 
}

}
";
            var script = new CSharpScriptExecution()
            {
                SaveGeneratedCode = true
            };
            script.AddDefaultReferencesAndNamespaces();

            // dynamic required since host doesn't know about this new type
            dynamic gen = script.CompileClass(code);

            Assert.IsFalse(script.Error, script.ErrorMessage + "\n" + script.GeneratedClassCodeWithLineNumbers);

            gen.Name = "Rick";
            gen.Time = DateTime.Now.AddMonths(-1);

            var result = gen.HelloWorld();

            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"Error ({script.ErrorType}): {script.Error}");
            Console.WriteLine(script.ErrorMessage);
            Console.WriteLine(script.GeneratedClassCodeWithLineNumbers);

            Assert.IsFalse(script.Error, script.ErrorMessage);
            Assert.IsTrue(result.Contains("Time is:"));

        }

        [TestMethod]
        public void CompileInvalidClassDefinitionShouldNotThrow()
        {
            var code = @"
        namespace Testing.Test
        {
            public class Test
            {
                public void Foo();
            }
        }";

            var script = new CSharpScriptExecution() { ThrowExceptions = true };

            //script.AddDefaultReferencesAndNamespaces();

            AddNetCoreDefaultReferences();
            AddAssembly("System.Net.WebClient.dll");
            AddAssembly(typeof(StringUtils));

            dynamic result = script.CompileClass(code);

            // Should have an error
            Assert.IsTrue(script.Error, script.ErrorMessage);
        }

        [TestMethod]
        public void CompileClassAndExecuteFromStream()
        {
            var code = @"
using System;

namespace Testing.Test {

public class Test
{
     public string Name {get; set; } = ""John"";
     public DateTime Time {get; set; } = DateTime.Now;

    public Test()
    {
        
    }

    public string HelloWorld(string name = null)
    {
        if (string.IsNullOrEmpty(name))
           name = Name;

        return ""Hello, "" + Name + "". Time is: "" + Time.ToString(""MMM dd, yyyy"");        
    } 
}

}
";
            // typically this will be a file stream
            using (var stream = StringUtils.StringToStream(code))
            {
                var script = new CSharpScriptExecution() { SaveGeneratedCode = true };
                script.AddDefaultReferencesAndNamespaces();

                dynamic gen = script.CompileClass(stream);

                Assert.IsFalse(script.Error, script.ErrorMessage + "\n" + script.GeneratedClassCodeWithLineNumbers);

                gen.Name = "Rick";
                gen.Time = DateTime.Now.AddMonths(-1);

                var result = gen.HelloWorld();

                Console.WriteLine($"Result: {result}");
                Console.WriteLine($"Error ({script.ErrorType}): {script.Error}");
                Console.WriteLine(script.ErrorMessage);
                Console.WriteLine(script.GeneratedClassCodeWithLineNumbers);

                Assert.IsFalse(script.Error, script.ErrorMessage);
                Assert.IsTrue(result.Contains("Time is:"));
            }
        }


        [TestMethod]
        public void CompileClassAndManuallyLoadClass()
        {
            var code = @"
using System;

namespace Testing.Test {

public class Test
{
     public string Name {get; set; } = ""John"";
     public DateTime Time {get; set; } = DateTime.Now;

    public Test()
    {
        
    }

    public string HelloWorld(string name = null)
    {
        if (string.IsNullOrEmpty(name))
           name = Name;

        return ""Hello, "" + Name + "". Time is: "" + Time.ToString(""MMM dd, yyyy"");        
    } 
}

}
";
            var script = new CSharpScriptExecution()
            {
                SaveGeneratedCode = true
            };
            script.AddDefaultReferencesAndNamespaces();

            var type = script.CompileClassToType(code);

            // Manually create the instance with Reflection
            dynamic gen = Activator.CreateInstance(type);   // assumes parameterless ctor()

            Assert.IsFalse(script.Error, script.ErrorMessage + "\n" + script.GeneratedClassCodeWithLineNumbers);

            gen.Name = "Rick";
            gen.Time = DateTime.Now.AddMonths(-1);

            var result = gen.HelloWorld();

            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"Error ({script.ErrorType}): {script.Error}");
            Console.WriteLine(script.ErrorMessage);
            Console.WriteLine(script.GeneratedClassCodeWithLineNumbers);

            Assert.IsFalse(script.Error, script.ErrorMessage);
            Assert.IsTrue(result.Contains("Time is:"));

            var x = new { Name = "Rick", Time = DateTime.Now };
        }


        public HashSet<PortableExecutableReference> References { get; set; } =
         new HashSet<PortableExecutableReference>();

        public bool AddAssembly(Type type)
        {
            try
            {
                if (References.Any(r => r.FilePath == type.Assembly.Location))
                    return true;

                var systemReference = MetadataReference.CreateFromFile(type.Assembly.Location);
                References.Add(systemReference);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool AddAssembly(string assemblyDll)
        {
            if (string.IsNullOrEmpty(assemblyDll)) return false;

            var file = Path.GetFullPath(assemblyDll);

            if (!File.Exists(file))
            {
                // check framework or dedicated runtime app folder
                var path = Path.GetDirectoryName(typeof(object).Assembly.Location);
                file = Path.Combine(path, assemblyDll);
                if (!File.Exists(file))
                    return false;
            }

            if (References.Any(r => r.FilePath == file)) return true;

            try
            {
                var reference = MetadataReference.CreateFromFile(file);
                References.Add(reference);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void AddNetFrameworkDefaultReferences()
        {
            AddAssembly("mscorlib.dll");
            AddAssembly("System.dll");
            AddAssembly("System.Core.dll");
            AddAssembly("Microsoft.CSharp.dll");
            AddAssembly("System.Net.Http.dll");
        }

        public void AddAssemblies(params string[] assemblies)
        {
            foreach (var file in assemblies)
                AddAssembly(file);
        }


        public void AddNetCoreDefaultReferences()
        {
            var rtPath = Path.GetDirectoryName(typeof(object).Assembly.Location) +
                         Path.DirectorySeparatorChar;

            AddAssemblies(
                rtPath + "System.Private.CoreLib.dll",
                rtPath + "System.Runtime.dll",
                rtPath + "System.Console.dll",
                rtPath + "netstandard.dll",

                rtPath + "System.Text.RegularExpressions.dll", // IMPORTANT!
                rtPath + "System.Linq.dll",
                rtPath + "System.Linq.Expressions.dll", // IMPORTANT!

                rtPath + "System.IO.dll",
                rtPath + "System.Net.Primitives.dll",
                rtPath + "System.Net.Http.dll",
                rtPath + "System.Private.Uri.dll",
                rtPath + "System.Reflection.dll",
                rtPath + "System.ComponentModel.Primitives.dll",
                rtPath + "System.Globalization.dll",
                rtPath + "System.Collections.Concurrent.dll",
                rtPath + "System.Collections.NonGeneric.dll",
                rtPath + "Microsoft.CSharp.dll"
            );
        }

    }
}
