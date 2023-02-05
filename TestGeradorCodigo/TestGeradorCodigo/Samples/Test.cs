namespace TestGeradorCodigo.Samples
{
    public class Test
    {
        public string Name { get; set; } = "John";
     public DateTime Time { get; set; } = DateTime.Now;

        public Test()
        {

        }

        public string HelloWorld(string name = null)
        {
            if (string.IsNullOrEmpty(name))
                name = Name;

            return "Hello, " + Name + ".Time is: " + Time.ToString("MMM dd, yyyy");
        }
    }
}
